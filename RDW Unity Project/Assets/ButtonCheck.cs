using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonCheck : MonoBehaviour
{
    public InputActionReference FlyButton;

    private void OnEnable()
    {
        FlyButton.action.Enable();
    }

    private void OnDisable()
    {
        FlyButton.action.Disable();
    }

    void Update()
    {
        // Check if the button is pressed
        if (FlyButton.action.ReadValue<float>() > 0.5f)
        {
            // Your code when the button is pressed

        }
    }
}

