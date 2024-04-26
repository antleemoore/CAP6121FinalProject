using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimateHandOnInputAndGrab : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    public bool ObjectSelected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ObjectSelected)
        {
            float gripValue = 0.66f;
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger", triggerValue);
            float gripValue = gripAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gripValue);
        }

        //if ()
    }
}