using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyMove : MonoBehaviour
{
    public InputActionReference FlyButton;
    // reference to the the object to get what direction to fly
    public Transform objectReference;

    // public variable for fly speed
    public float flySpeed = 1.0f;

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
            // why are we multiplying by Time.deltaTime?
            // Time.deltaTime is the time it took to complete the last frame
            transform.position += objectReference.forward * flySpeed;
        }
        else
        {
            // set the players velocity to 0 to stop the player from moving
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}