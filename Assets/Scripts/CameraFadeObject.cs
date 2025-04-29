using UnityEngine;

public class CameraFadeObject : MonoBehaviour
{
    GameObject previousObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            if (previousObject != null)
            {
                previousObject.SetActive(true);
            }

            previousObject = other.gameObject;
            other.gameObject.SetActive(false);
        }
    }
}