using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Redirection;


// align to the vector calculated by artificial potential fileds, rotate to the side of the larger angle
// R2G means reset-to-gradient 
public class R2G_Resetter : Resetter
{

    float requiredRotateSteerAngle = 0; // steering angle，rotate the physical plane and avatar together

    float requiredRotateAngle = 0; // normal rotation angle, only rotate avatar

    float rotateDir; // rotation direction, positive if rotate clockwise

    float speedRatio;

    APF_Redirector redirector;

    public override bool IsResetRequired()
    {
        return IfCollisionHappens();
    }

    public override void InitializeReset()
    {
        var redirectorTmp = redirectionManager.redirector;
        var currPos = Utilities.FlattenedPos2D(redirectionManager.currPosReal);
        targetPos = DecideResetPosition(currPos);
        if (redirectorTmp.GetType().IsSubclassOf(typeof(APF_Redirector)))
        {
            redirector = (APF_Redirector)redirectorTmp;
            targetDir = redirector.totalForce;
        }
        else
        {
            targetDir = getGradientForceByThomasAPF(currPos);
            // Debug.Log("RedirectorType: " + redirectorTmp.GetType());
            // Debug.LogError("non-APF redirector can't use R2G_resetter");
        }

        var currDir = Utilities.FlattenedDir2D(redirectionManager.currDirReal);

        var nearestDistAndPos =
            Utilities.GetNearestDistAndPosToObstacleAndTrackingSpace(redirectionManager.GetTrackedSpaceSegments(),
                currPos);
        var obstaclePos = nearestDistAndPos.Item2;

        var normal = (currPos - obstaclePos).normalized;
        if (Vector2.Dot(normal, targetDir) <= 0)
        {
            // choose a reasonable direction instead
            targetDir = normal;
        }


        var targetRealRotation = 360 - Vector2.Angle(targetDir, currDir); // required rotation angle in real world

        rotateDir = -(int)Mathf.Sign(Utilities.GetSignedAngle(redirectionManager.currDirReal,
            Utilities.UnFlatten(targetDir)));

        requiredRotateSteerAngle = 360 - targetRealRotation;

        requiredRotateAngle = targetRealRotation;

        speedRatio = requiredRotateSteerAngle / requiredRotateAngle;

        SetHUD((int)rotateDir);
    }

    public override void ApplyResetting()
    {
        var steerRotation = speedRatio * redirectionManager.deltaDir;
        if (Mathf.Abs(requiredRotateSteerAngle) <= Mathf.Abs(steerRotation) || requiredRotateAngle == 0)
        {
            // meet the rotation requirement
            InjectRotation(requiredRotateSteerAngle);

            // reset end
            redirectionManager.OnResetEnd();
            requiredRotateSteerAngle = 0;
        }
        else
        {
            // rotate the rotation calculated by ratio
            InjectRotation(steerRotation);
            requiredRotateSteerAngle -= Mathf.Abs(steerRotation);
        }
    }

    public override void FinalizeReset()
    {
        DestroyHUD();
    }

    public override void SimulatedWalkerUpdate()
    {
        redirectionManager.simulatedWalker.RotateInPlace();
    }

    // a basic method to get apf force
    // we can use other methods if needed
    public Vector2 getGradientForceByThomasAPF(Vector2 currPosReal)
    {
        var nearestPosList = new List<Vector2>();
        List<Vector2> space = redirectionManager.GetTrackedSpaceSegments();

        //physical borders' contributions
        for (int i = 0; i < space.Count; i++)
        {
            var p = space[i];
            var q = space[(i + 1) % space.Count];
            var nearestPos = Utilities.GetNearestPos(currPosReal, new List<Vector2> { p, q });
            var n = Utilities.RotateVector(q - p, -90).normalized;
            var d = currPosReal - nearestPos;
            if (Vector2.Dot(n, d.normalized) > 0)
            {
                nearestPosList.Add(nearestPos);
            }
        }

        var ng = Vector2.zero;
        foreach (var obPos in nearestPosList)
        {
            //get gradient contributions
            var gDelta = -1 / (currPosReal - obPos).magnitude * (currPosReal - obPos).normalized;

            ng += -gDelta;//negtive gradient
        }
        ng = ng.normalized;
        return ng;
    }
}
