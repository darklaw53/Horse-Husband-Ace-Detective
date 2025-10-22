using UnityEngine;

public class CameraFadeObject : MonoBehaviour
{
    GameObject previousObject;
    int previousLayer;
    Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (previousObject != null)
            {
                int mask = mainCam.cullingMask;
                mask |= (1 << previousLayer); 
                mainCam.cullingMask = mask;
            }

            previousObject = other.gameObject;
            previousLayer = previousObject.layer;

            int newMask = mainCam.cullingMask;
            newMask &= ~(1 << previousLayer); 
            mainCam.cullingMask = newMask;
        }
    }
}