using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpScript : MonoBehaviour
{
    public InputActionReference JumpButton;
    public int jumpDuration = 10;

    private int jumpCounter = 0;
    private bool canJump = true;

    private void OnEnable()
    {
        JumpButton.action.Enable();
    }

    private void OnDisable()
    {
        JumpButton.action.Disable();
    }

    void Update()
    {
        if (jumpCounter > 0)
        {
            transform.position += Vector3.up;
            jumpCounter--;
        }
        else if (JumpButton.action.triggered && canJump)
        {
            jumpCounter = jumpDuration;
            canJump = false;
        }

        // Check if the player is on the ground
        if (transform.position.y <= 2f)
        {
            canJump = true;
        }
    }
}