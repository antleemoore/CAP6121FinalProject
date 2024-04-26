using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SwordBehavior : MonoBehaviour
{
    private const int SlashDamage = 10;

    private const int StabDamage = 5;

    //private float _timeDelta = 0f;
    //private float delay = 1f;

    private Vector3 _lastPos;
    private const int BufferSize = 10;

    private readonly Vector3[] _velocityBuffer = new Vector3[BufferSize];
    private int _idx = 0;
    
    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnEnable()
    {
        ClearBuffer();
    }

    // Update is called once per frame
    private void Update()
    {
        // only process every second not every frame?
        //_timeDelta += Time.deltaTime;
        
        //moving with parent?
        Vector3 curPos = transform.position;

        Vector3 velocity = Vector3.Normalize((curPos - _lastPos) / Time.deltaTime);

        _lastPos = curPos;
        
        SetVelFrame(velocity);
    }

    /// <summary>
    /// Triggers for slash attacks
    /// </summary>
    private static void Slash(IEnemy enemy)
    {
        enemy.Slash(SlashDamage);
    }

    /// <summary>
    /// Triggers for stab attacks
    /// </summary>
    private static void Stab(IEnemy enemy)
    {
        //should we have a stab and slash method for enemies to make certain
        //fruits better defeated by certain attacks
        enemy.Stab(StabDamage);
    }
    
    /// <summary>
    /// Parry to block an enemy attack
    /// </summary>
    /// <param name="enemy"></param>
    private void Parry(IEnemy enemy)
    {
        enemy.Parry();
    }

    private void OnTriggerEnter(Collider other)
    {
        // make sure we get a not null IEnemy enemy
        if (other.GetComponent<IEnemy>() is {} enemy)
        {
            bool isSlashing = false;
            bool isStabbing = false;
            bool isParrying = false;
            
            Vector3 velocity = Vector3.Normalize(AverageVelocity());
            
            // sword point is loc at negative y
            float stabConfidence = Vector3.Dot(velocity, Vector3.Normalize(transform.up)) * -1;
        
            // Can slash with either edge
            float slashConfidence = Math.Abs(Vector3.Dot(velocity, Vector3.Normalize(transform.right)));

            // not sure how to handle this, should it be velocity close to 0 or close to a certain location
            // float parryConfidence = Vector3.Dot(velocity, Vector3.Normalize(Vector3.forward));

            //also within a threshold
            float threshold = 0.707f;

            // set temp var to the max of the confidences and threshold
            float temp = Math.Max(stabConfidence, Math.Max(slashConfidence, threshold));

            if (stabConfidence >= temp) isStabbing = true;
            else if (slashConfidence >= temp) isSlashing = true;
            else isParrying = true;
        
            Debug.Log($"Velocity: {velocity}, stabConfidence: {stabConfidence}, " + 
                      $"slashConfidence: {slashConfidence}, _isStabbing: {isStabbing}, _isSlashing: {isSlashing}"
                      + $" _isParrying {isParrying}");
            
            if (isSlashing) Slash(enemy);
            else if (isStabbing) Stab(enemy);
            else if (isParrying) Parry(enemy); //melee attack
        }
        // else if (other.CompareTag("EnemyAttack"))
        // {
        //     if (_isParrying) Parry(); //ranged attack
        // }
    }

    private void SetVelFrame(Vector3 velocity)
    {
        _velocityBuffer[_idx] = velocity;
        _idx++;
        if (_idx >= BufferSize)
        {
            _idx = 0;
        }
    }

    // returns sum more so than avg just to eliminate hand shake
    private Vector3 AverageVelocity()
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < BufferSize; i++)
        {
            sum += _velocityBuffer[i];
        }

        return sum;
    }

    // may need to expose for clearing after TP bc vel will act funky since using world pos
    public void ClearBuffer()
    {
        for (int i = 0; i < BufferSize; i++)
        {
            _velocityBuffer[i] = Vector3.zero;
        }
    }
}
