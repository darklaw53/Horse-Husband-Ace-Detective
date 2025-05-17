using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 dir = mainCamera.transform.position - transform.position;
        transform.forward = -dir.normalized;
    }
}
