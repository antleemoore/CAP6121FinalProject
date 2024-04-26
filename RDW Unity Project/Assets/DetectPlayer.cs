using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    private IEnemy parent;

    private void Start()
    {
        parent = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            parent.PlayerDetected = true;
            //gameObject.SetActive(false); //stop more triggers since I do not have time or energy
                                         //to implement "losing the player" requiring re-detect
        }
    }
}
