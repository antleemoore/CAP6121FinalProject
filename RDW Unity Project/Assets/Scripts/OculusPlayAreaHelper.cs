using UnityEngine;
using Oculus;

public static class OculusPlayAreaHelper
{
    // Gets the player's current location in the play area, relative to the center of the play area.
    public static Vector3 GetPlayerLocationInPlayArea()
    {
        // Ensure OVRManager is initialized and the tracking is ready.
        if (OVRManager.isHmdPresent)
        {
            // Returns the current position of the player's headset.
            return OVRManager.tracker.GetPose().position;
        }
        else
        {
            Debug.LogWarning("OVRManager is not present or initialized.");
            return Vector3.zero;
        }
    }

    // Checks if the player is currently within the play area boundaries.
    public static bool IsPlayerInPlayArea()
    {
        if (OVRManager.boundary.GetConfigured())
        {
            // Using OVRBoundary to check if the player is within the play area.
            // This method uses the player's headset position.
            return OVRManager.boundary.TestPoint(GetPlayerLocationInPlayArea(), OVRBoundary.BoundaryType.PlayArea).IsTriggering;
        }
        else
        {
            Debug.LogWarning("Oculus Guardian System is not configured.");
            return false;
        }
    }

    // Retrieves the dimensions of the play area defined by the Oculus boundary system.
    public static Vector3 GetPlayAreaDimensions()
    {
        if (OVRManager.boundary.GetConfigured())
        {
            // Returns the dimensions of the play area.
            return OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        }
        else
        {
            Debug.LogWarning("Oculus Guardian System is not configured.");
            return Vector3.zero;
        }
    }
}
