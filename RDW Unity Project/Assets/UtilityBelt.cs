using System;
using Interfaces;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class UtilityBelt : XRBaseInteractable
{
    // refrence to the two items you can hold
    public GameObject GrapplingHook;
    public GameObject Shuriken;
    public NinjaBehavior player;
    private GrapplingHookBehavior _grappleHook;
    private void Start()
    {
        _grappleHook = GrapplingHook.GetComponent<GrapplingHookBehavior>();
        GrapplingHook.SetActive(false);
    }

    // function to switch between the two items
    public void SwitchItems()
    {
        // if item1 is active, deactivate it and activate item2
        if (GrapplingHook.activeSelf && !_grappleHook.HasFruitSelected)
        {
            GrapplingHook.SetActive(false);
            Shuriken.SetActive(true);
            player.shurikenActive = true;
        }
        // if item2 is active, deactivate it and activate item1
        else if (Shuriken.activeSelf)
        {
            Shuriken.SetActive(false);
            GrapplingHook.SetActive(true);
            player.shurikenActive = false;
        }
    }
}