using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class MoldPowerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        IEnemy enemy = other.GetComponent<IEnemy>();

        // if enemy is not null call Mold
        enemy?.Mold();
    }
}
