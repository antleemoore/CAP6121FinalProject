using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHookBehavior : MonoBehaviour, IGrabber
{
    public bool grapplingHookEnabled = true;
    public LayerMask layersToHit;
    private IGrabbable _selectedFruit;
    private LineRenderer _line;
    public float grapplingHookRange = 30f;
    public Transform hand;
    
    public bool HasFruitSelected => _selectedFruit is not null;

    public InputActionProperty grabAction;
    public InputActionProperty translateAction;

    private const float TranslationScale = 0.1f;

    private const float ProximityLimit = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        grapplingHookEnabled = true;
    }

    private void OnDisable()
    {
        grapplingHookEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!grapplingHookEnabled) return;
       
        _line.SetPosition(0, hand.position);
        
        if (_selectedFruit is not null)
        {
            //update Fruit position
            float translation = translateAction.action.ReadValue<Vector2>().y * TranslationScale;
            Vector3 position = hand.position + hand.forward * grapplingHookRange;
            // IGrabbable should always be cast-able to Component

            if (_selectedFruit is Component selectedFruit) //translate fruit if possible
            {
                Vector3 localPos = selectedFruit.transform.localPosition;
                Vector3 direction = Vector3.Normalize(localPos);
                Vector3 displacement = direction * translation;
                float diff = Vector3.Distance(localPos + displacement,
                    Vector3.zero) - grapplingHookRange;
                
                //would be beyond the range
                if (diff > 0)
                {
                    displacement -= diff * direction;
                }
                else if (diff < ProximityLimit - grapplingHookRange)
                {
                    displacement -= (diff + (grapplingHookRange - ProximityLimit)) * direction;
                }

                //how to check if behind the hand limit to 0 - 50 distance
                
                selectedFruit.transform.Translate(displacement, hand);
                
                // update line position
                position = selectedFruit.transform.position;
            }
            
            _line.SetPosition(1, position);
            
            // check if throw button pressed
            if (grabAction.action.triggered)
            {
                Throw();
            }
            return;
        }
        
        Ray ray = new Ray(hand.position, hand.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, grapplingHookRange, 
                layersToHit.value, QueryTriggerInteraction.Ignore))
        {
            _line.SetPosition(1, hit.point);
            
            IGrabbable enemy = hit.collider.GameObject().GetComponent<IGrabbable>();
            if (enemy is not null && grabAction.action.triggered) Grab(enemy);
        }
        else
        {
            _line.SetPosition(1, hand.position + hand.forward * grapplingHookRange);
        }
    }
    /// <summary>
    /// Confirm selection
    /// </summary>
    /// <param name="fruit"></param>
    /// <returns></returns>
    public bool Grab(IGrabbable fruit)
    {
        if (_selectedFruit is not null) return false;
        if (!fruit.Select(this)) return false;
        _selectedFruit = fruit;

        return true;
    }

    public void Clear()
    {
        _selectedFruit = null;
    }

    public Transform GetTransform()
    {
        return transform;
    }
    
    /// <summary>
    /// Throw the fruit - the throw method on the fruit can maybe calculate dmg
    /// to do to the fruit or something or just used for repositioning enemies
    /// </summary>
    /// <returns></returns>
    public bool Throw()
    {
        _selectedFruit.Throw();
        _selectedFruit = null;
        return true;
    }
}
