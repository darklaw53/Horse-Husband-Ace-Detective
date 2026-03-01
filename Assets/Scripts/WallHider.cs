using UnityEngine;

public class WallHider : MonoBehaviour
{
    [Header("Trigger Area")]
    public BoxCollider triggerCollider;

    [Header("Camera Reference")]
    public Camera targetCamera; 

    Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetCamera.gameObject)
            SetVisible(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetCamera.gameObject)
            SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }
}