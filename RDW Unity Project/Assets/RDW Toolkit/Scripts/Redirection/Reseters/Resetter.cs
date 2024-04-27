using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Redirection;

public abstract class Resetter : MonoBehaviour {

    [HideInInspector]
    public RedirectionManager redirectionManager;
    private static float toleranceAngleError = 1;//Allowable angular error to prevent jamming
    [HideInInspector]
    public Vector2 targetPos; // the target position we want user to be at when the reset ends
    [HideInInspector]
    public Vector2 targetDir; // the target direction we want user to face when the reset ends
    protected Transform prefabHUD = null;
    
    protected Transform instanceHUD;

    enum Boundary { Top, Bottom, Right, Left };

    float maxX, maxZ;

    /// <summary>
    /// Function called when reset trigger is signaled, to see if resetter believes resetting is necessary.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsResetRequired();

    public abstract void InitializeReset();

    public abstract void ApplyResetting();

    public abstract void FinalizeReset();

    public abstract void SimulatedWalkerUpdate();

    private void Awake()
    {
        redirectionManager = GetComponent<RedirectionManager>();

        targetDir = new Vector2(1, 0);
        targetPos = Vector2.zero;
    }

    public void InjectRotation(float rotationInDegrees)
    {
        this.transform.RotateAround(Utilities.FlattenedPos3D(redirectionManager.headTransform.position), Vector3.up, rotationInDegrees);
        //this.GetComponentInChildren<KeyboardController>().SetLastRotation(rotationInDegrees);
        redirectionManager.statisticsLogger.Event_Rotation_Gain_Reorientation(rotationInDegrees / redirectionManager.deltaDir, rotationInDegrees);
    }

    public void Initialize()
    {
        maxX = 0.5f * (redirectionManager.trackedSpace.localScale.x) - redirectionManager.resetTrigger.RESET_TRIGGER_BUFFER;// redirectionManager.resetTrigger.xLength);// + USER_CAPSULE_COLLIDER_DIAMETER);
        maxZ = 0.5f * (redirectionManager.trackedSpace.localScale.z) - redirectionManager.resetTrigger.RESET_TRIGGER_BUFFER;
        //print("PRACTICAL MAX X: " + maxX);
    }

    public bool IsUserOutOfBounds()
    {
        return Mathf.Abs(redirectionManager.currPosReal.x) >= maxX || Mathf.Abs(redirectionManager.currPosReal.z) >= maxZ;
    }


    Boundary getNearestBoundary()
    {
        Vector3 position = redirectionManager.currPosReal;
        if (position.x >= 0 && Mathf.Abs(maxX - position.x) <= Mathf.Min(Mathf.Abs(maxZ - position.z), Mathf.Abs(-maxZ - position.z))) // for a very wide rectangle, you can find that the first condition is actually necessary
            return Boundary.Right;
        if (position.x <= 0 && Mathf.Abs(-maxX - position.x) <= Mathf.Min(Mathf.Abs(maxZ - position.z), Mathf.Abs(-maxZ - position.z)))
            return Boundary.Left;
        if (position.z >= 0 && Mathf.Abs(maxZ - position.z) <= Mathf.Min(Mathf.Abs(maxX - position.x), Mathf.Abs(-maxX - position.x)))
            return Boundary.Top;
        return Boundary.Bottom;
    }

    Vector3 getAwayFromNearestBoundaryDirection()
    {
        Boundary nearestBoundary = getNearestBoundary();
        switch (nearestBoundary)
        {
            case Boundary.Top:
                return -Vector3.forward;
            case Boundary.Bottom:
                return Vector3.forward;
            case Boundary.Right:
                return -Vector3.right;
            case Boundary.Left:
                return Vector3.right;
        }
        return Vector3.zero;
    }

    float getUserAngleWithNearestBoundary() // Away from Wall is considered Zero
    {
        return Utilities.GetSignedAngle(redirectionManager.currDirReal, getAwayFromNearestBoundaryDirection());
    }

    protected bool isUserFacingAwayFromWall()
    {
        return Mathf.Abs(getUserAngleWithNearestBoundary()) < 90;
    }

    public float getTrackingAreaHalfDiameter()
    {
        return Mathf.Sqrt(maxX * maxX + maxZ * maxZ);
    }

    public float getDistanceToCenter()
    {
        return redirectionManager.currPosReal.magnitude;
    }

    public float getDistanceToNearestBoundary()
    {
        Vector3 position = redirectionManager.currPosReal;
        Boundary nearestBoundary = getNearestBoundary();
        switch (nearestBoundary)
        {
            case Boundary.Top:
                return Mathf.Abs(maxZ - position.z);
            case Boundary.Bottom:
                return Mathf.Abs(-maxZ - position.z);
            case Boundary.Right:
                return Mathf.Abs(maxX - position.x);
            case Boundary.Left:
                return Mathf.Abs(-maxX - position.x);
        }
        return 0;
    }

    public float getMaxWalkableDistanceBeforeReset()
    {
        Vector3 position = redirectionManager.currPosReal;
        Vector3 direction = redirectionManager.currDirReal;
        float tMaxX = direction.x != 0 ? Mathf.Max((maxX - position.x) / direction.x, (-maxX - position.x) / direction.x) : float.MaxValue;
        float tMaxZ = direction.z != 0 ? Mathf.Max((maxZ - position.z) / direction.z, (-maxZ - position.z) / direction.z) : float.MaxValue;
        //print("MaxX: " + maxX);
        //print("MaxZ: " + maxZ);
        return Mathf.Min(tMaxX, tMaxZ);
    }

    public bool IfCollisionHappens()
    {
        var realPos = new Vector2(redirectionManager.currPosReal.x, redirectionManager.currPosReal.z);
        var realDir = new Vector2(redirectionManager.currDirReal.x, redirectionManager.currDirReal.z).normalized;

        bool ifCollisionHappens = false;
        List<Vector2> trackingSpace = redirectionManager.GetTrackedSpaceSegments();
        
        for (int i = 0; i < trackingSpace.Count; i++)
        {
            var p = trackingSpace[i];
            var q = trackingSpace[(i + 1) % trackingSpace.Count];

            //judge vertices of polygons
            if (IfCollideWithPoint(realPos, realDir, p))
            {
                ifCollisionHappens = true;
                break;
            }

            //judge edge collision
            if (Vector3.Cross(q - p, realPos - p).magnitude / (q - p).magnitude <=
                redirectionManager.RESET_TRIGGER_BUFFER //distance
                && Vector2.Dot(q - p, realPos - p) >= 0 && Vector2.Dot(p - q, realPos - q) >= 0 //range
               )
            {
                //if collide with border
                if (Mathf.Abs(Cross(q - p, realDir)) > 1e-3 &&
                    Mathf.Sign(Cross(q - p, realDir)) != Mathf.Sign(Cross(q - p, realPos - p)))
                {
                    ifCollisionHappens = true;
                    break;
                }
            }
        }
        
        return ifCollisionHappens;
    }
    
    public bool IfCollideWithPoint(Vector2 realPos, Vector2 realDir, Vector2 obstaclePoint)
    {
        //judge point, if the avatar will walks into a circle obstacle
        var pointAngle = Vector2.Angle(obstaclePoint - realPos, realDir);
        return (obstaclePoint - realPos).magnitude <= redirectionManager.RESET_TRIGGER_BUFFER && pointAngle < 90 - toleranceAngleError;
    }
    
    private float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    // decide the actual reset position, which doesn't need to be the same with user's current position
    // a safer position could reduce possible resets in a live-user experiment
    public Vector2 DecideResetPosition(Vector2 currPosReal)
    {
        return currPosReal;
    }
    
    // initialize spin in place hint, rotateDir==1:rotate clockwise, otherwise, rotate counter clockwise
    public void SetHUD(int rotateDir)
    {
        if (prefabHUD == null)
            prefabHUD = Resources.Load<Transform>("Resetter HUD");


        instanceHUD = Instantiate(prefabHUD);
        instanceHUD.parent = redirectionManager.headTransform;
        instanceHUD.localPosition = instanceHUD.position;
        instanceHUD.localRotation = instanceHUD.rotation;

        //rotate clockwise
        if (rotateDir == 1)
        {
            instanceHUD.GetComponent<TextMesh>().text = "Spin in Place\n→";
        }
        else
        {
            instanceHUD.GetComponent<TextMesh>().text = "Spin in Place\n←";
        }
    }

    // destroy HUD object
    public void DestroyHUD()
    {
        if (instanceHUD != null)
            Destroy(instanceHUD.gameObject);
    }

}
