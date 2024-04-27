using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BatarangSpawn : MonoBehaviour
{
    public GameObject batarangPrefab;
    public XRGrabInteractable grabInteractable;
    private bool hasBatarang = false;

    private void Start()
    {
        grabInteractable.onSelectEntered.AddListener(SpawnBatarang);
        grabInteractable.onSelectExited.AddListener(RetrieveBatarang);
    }

    private void SpawnBatarang(XRBaseInteractor interactor)
    {
        if (!hasBatarang)
        {
            GameObject batarang = Instantiate(batarangPrefab, interactor.attachTransform.position, interactor.attachTransform.rotation);
            batarang.GetComponent<Rigidbody>().isKinematic = false;
            hasBatarang = true;

            // Optional: You can destroy the batarang after a certain time or when it hits something
            // Destroy(batarang, 10f);
        }
    }

    private void RetrieveBatarang(XRBaseInteractor interactor)
    {
        hasBatarang = false;
    }
}