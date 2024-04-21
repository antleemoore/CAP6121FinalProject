using UnityEngine;

public abstract class APF_Redirector : Redirector
{
    public Vector2 totalForce;//vector calculated by artificial potential fields(total force or negtive gradient), can be used by apf-resetting
    public GameObject totalForcePointer;//visualization of totalForce
    Vector3 translation;
    float rotationInDegrees;

    public void UpdateTotalForcePointer(Vector2 forceT)
    {
        //record this new force
        totalForce = forceT;

        /*if (totalForcePointer == null)
        {
            totalForcePointer = Instantiate(redirectionManager.globalConfiguration.negArrow);
            totalForcePointer.transform.SetParent(transform);
            totalForcePointer.transform.position = Vector3.zero;
        }

        if (totalForcePointer != null)
        {
            totalForcePointer.transform.position = redirectionManager.currPos;

            if (forceT.magnitude > 0)
                totalForcePointer.transform.forward = transform.rotation * Utilities.UnFlatten(forceT);
        }*/
    }

    private void OnDestroy()
    {
        if (totalForcePointer != null)
            Destroy(totalForcePointer);
    }
    public void ApplyGains()
    {
        transform.Translate(translation, Space.World);
        transform.RotateAround(Redirection.Utilities.FlattenedPos3D(redirectionManager.headTransform.position), Vector3.up, rotationInDegrees);
    }

    protected void SetTranslationGain(float gt)
    {
        gt = Mathf.Max(gt, redirectionManager.MIN_TRANS_GAIN);
        gt = Mathf.Min(gt, redirectionManager.MAX_TRANS_GAIN);
        //redirectionManager.gt = gt;
        translation = redirectionManager.deltaPos * (gt - 1);
        redirectionManager.statisticsLogger.Event_Translation_Gain(Mathf.Sign(Vector3.Dot(translation, redirectionManager.deltaPos)) * translation.magnitude / redirectionManager.deltaPos.magnitude, Redirection.Utilities.FlattenedPos2D(translation));
    }
    protected void SetRotationGain(float gr)
    {
        if (redirectionManager.isRotating)
        {
            gr = Mathf.Max(gr, redirectionManager.MIN_ROT_GAIN);
            gr = Mathf.Min(gr, redirectionManager.MAX_ROT_GAIN);
            //redirectionManager.gr = gr;
            var rotationInDegreesGR = redirectionManager.deltaDir * (gr - 1);
            if (Mathf.Abs(rotationInDegreesGR) > Mathf.Abs(rotationInDegrees))
            {
                rotationInDegrees = rotationInDegreesGR;
                //GetComponentInChildren<KeyboardController>().SetLastRotation(rotationInDegreesGR);
            }
            redirectionManager.statisticsLogger.Event_Rotation_Gain(rotationInDegrees / redirectionManager.deltaPos.magnitude, rotationInDegrees);
        }
    }
    protected void SetCurvature(float curvature)// positive means turning left
    {
        if (redirectionManager.isWalking)
        {
            curvature = Mathf.Max(curvature, -1 / redirectionManager.CURVATURE_RADIUS);
            curvature = Mathf.Min(curvature, 1 / redirectionManager.CURVATURE_RADIUS);
            //redirectionManager.curvature = curvature;
            var rotationInDegreesGC = Mathf.Rad2Deg * redirectionManager.deltaPos.magnitude * curvature;
            if (Mathf.Abs(rotationInDegreesGC) > Mathf.Abs(rotationInDegrees))
            {
                rotationInDegrees = rotationInDegreesGC;
                //GetComponentInChildren<KeyboardController>().SetLastRotation(rotationInDegreesGC);
            }
            redirectionManager.statisticsLogger.Event_Curvature_Gain(rotationInDegrees / redirectionManager.deltaPos.magnitude, rotationInDegrees);
        }
    }
    
    public void ClearGains()
    {
        translation = Vector3.zero;
        rotationInDegrees = 0;
        
        Debug.Log("Cleared Gains");
    }
    
}
