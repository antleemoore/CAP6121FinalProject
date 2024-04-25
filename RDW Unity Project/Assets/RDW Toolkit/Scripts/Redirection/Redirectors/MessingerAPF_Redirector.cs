// extended version, suitable for Nonconvex tracking space
// Effects of tracking area shape and size on artificial potential field redirected walking
// https://www.cs.purdue.edu/cgvlab/courses/490590VR/notes/VRLocomotion/MultiuserRedirectedWalking/TrackingAreaShapeSizeEffects2019.pdf

// original version
// Multi-user redirected walking and resetting using artificial potential fields
// https://www.cs.purdue.edu/cgvlab/courses/490590VR/notes/VRLocomotion/MultiuserRedirectedWalking/APFRedirectedWalking2019.pdf

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Redirection;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

public class MessingerAPF_Redirector : APF_Redirector
{
    private static readonly float targetSegLength = 1;//split edges of boundaries/obstacles into small segments with length equal to targetSegLength

    //constant parameters used by the paper
    private static readonly float C = 0.00897f;
    private static readonly float lamda = 2.656f;
    private static readonly float gamma = 3.091f;
    private static readonly float angScaleDilate = 1.3f;
    private static readonly float angScaleCompress = 0.85f;

    private const float M = 15;//unit:degree, the maximum Steering rate in proximity-Based Steering Rate Scaling strategy

    public override void ApplyRedirection()
    {
        var physicalSpace = redirectionManager.GetTrackedSpaceSegments();

        // var obstaclePolygons = redirectionManager.globalConfiguration.obstaclePolygons;
        // var trackingSpacePoints = redirectionManager.globalConfiguration.trackingSpacePoints;
        // var userTransforms = redirectionManager.GetAvatarTransforms();

        //calculate total force by the paper
        var forceT = GetTotalForce(physicalSpace);
        //forceT = forceT.normalized;

        UpdateTotalForcePointer(forceT);

        //apply redirection according to the total force
        InjectRedirectionByForce(forceT, physicalSpace);

    }
    //calculate the total force by the paper
    public Vector2 GetTotalForce(List<Vector2> physicalSpace)
    {
        var w = Vector2.zero;
        //int userIndex = movementManager.physicalSpaceIndex; // we only consider the space where the current user's at
        //not considering multiple users
        for (int i = 0; i < physicalSpace.Count; i++)
            w += GetW_Force(physicalSpace[i], physicalSpace[(i + 1) % physicalSpace.Count]);
        //not considering objects
        //not considering multi-user

        return w;
    }
    // get force contributed by every edge of the obstacle or border
    public Vector2 GetW_Force(Vector2 p, Vector2 q)
    {
        var wForce = Vector2.zero;
        //split long edge to short segments then accumulate
        var length = (p - q).magnitude;
        var segNum = (int)(length / targetSegLength);
        if (segNum * targetSegLength < length)
            segNum++;
        var segLength = length / segNum;
        var unitVec = (q - p).normalized;
        for (int i = 1; i <= segNum; i++)
        {
            var tmpP = p + unitVec * (i - 1) * segLength;
            var tmpQ = p + unitVec * i * segLength;
            wForce += GetW_ForceEverySeg(tmpP, tmpQ);
        }
        return wForce;
    }

    //get force contributed by a segment
    public Vector2 GetW_ForceEverySeg(Vector2 p, Vector2 q)
    {
        //get center point
        var c = (p + q) / 2;

        var currPos = Utilities.FlattenedPos2D(redirectionManager.currPosReal);
        var d = currPos - c;
        //normal towards walkable side
        var n = Utilities.RotateVector(q - p, -90).normalized;

        if (Vector2.Dot(n, d.normalized) > 0)
            return C * (q - p).magnitude * d.normalized * 1 / Mathf.Pow(d.magnitude, lamda);
        else
            return Vector2.zero;
    }
    
    //do redirection by MessingerAPF
    public void InjectRedirectionByForce(Vector2 force, List<Vector2> spaceSegments)
    {
        var desiredFacingDirection = Utilities.UnFlatten(force).normalized;//total force vector in physical space
        int desiredSteeringDirection = (-1) * (int)Mathf.Sign(Utilities.GetSignedAngle(redirectionManager.currDirReal, desiredFacingDirection));

        //calculate walking speed
        var v = redirectionManager.deltaPos.magnitude / redirectionManager.GetDeltaTime();
        float movingRate = 0;

        float s = 1; //2.5f * force.magnitude / averageForceVector.magnitude;

        if (v > 0.1f)
        {
            movingRate = 360 * v / (2 * Mathf.PI * redirectionManager.CURVATURE_RADIUS);
            //only consider static obstacles
            var distToObstacle = Utilities.GetNearestDistAndPosToObstacleAndTrackingSpace(spaceSegments,
                Utilities.FlattenedPos2D(redirectionManager.currPosReal)).Item1;

            //distance smaller than curvature radius，use Proximity-Based Steering Rate Scaling strategy
            if (distToObstacle < redirectionManager.CURVATURE_RADIUS)
            {
                var h = movingRate;
                var m = distToObstacle;
                var t = 1 - m / redirectionManager.CURVATURE_RADIUS;
                var appliedSteeringRate = (1 - t) * h + t * M;
                movingRate = appliedSteeringRate; //calculate steering rate of curvature gain
            }

            movingRate = Mathf.Clamp(s * Mathf.Abs(movingRate), 0, 15);
        }

        //SetCurvature(desiredSteeringDirection * movingRate * redirectionManager.GetDeltaTime() / Mathf.Rad2Deg / Mathf.Max(0.001f, redirectionManager.deltaPos.magnitude)); // WARNING: this could result in a curvature above imperceptible levels

        /*if (redirectionManager.deltaDir * desiredSteeringDirection < 0)
        {//rotate away total force vector
            SetRotationGain(redirectionManager.MIN_ROT_GAIN);
        }
        else
        {//rotate towards total force vector
            SetRotationGain(redirectionManager.MAX_ROT_GAIN);
        }*/
        float headRate = 0f;
        if (v < 0.1f)
        {
            var yawRate = redirectionManager.deltaDir / redirectionManager.GetDeltaTime();
            headRate = yawRate * ((redirectionManager.deltaDir * desiredSteeringDirection < 0)
                ? angScaleCompress
                : angScaleDilate);
            headRate = Mathf.Clamp(s * Mathf.Abs(headRate), 0, 30);
        }

        //SetRotationGain(headRate);
        //SetTranslationGain(1);


        var appliedRotation = desiredSteeringDirection * Mathf.Max(1.5f, Mathf.Max(movingRate, headRate)) * redirectionManager.GetDeltaTime();

        if (redirectionManager.isWalking)
        {
            InjectCurvature(appliedRotation);
        }
        else
        {
            InjectRotation(appliedRotation);
        }
        //ApplyGains();
    }
}