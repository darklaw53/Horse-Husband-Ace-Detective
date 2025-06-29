using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class CylinderBoundaryLimiter : MonoBehaviour
{
    [Header("Cylinder Boundary Settings")]
    public GameObject cylinderObject; 

    private float maxRadius;
    private Vector3 center;
    private ThirdPersonController controller;
    private Rigidbody rb;

    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        rb = GetComponent<Rigidbody>();

        if (controller == null || rb == null)
        {
            Debug.LogError("CylinderBoundaryLimiter requires ThirdPersonController and Rigidbody.");
            enabled = false;
            return;
        }

        if (cylinderObject == null)
        {
            Debug.LogError("CylinderBoundaryLimiter: Cylinder object reference not set.");
            enabled = false;
            return;
        }

        UpdateBoundsFromCylinder();
    }

    void FixedUpdate()
    {
        if (!controller.moveEnabled) return;

        UpdateBoundsFromCylinder();

        Vector3 flatPos = transform.position;
        flatPos.y = 0;
        Vector3 flatCenter = center;
        flatCenter.y = 0;

        float distance = Vector3.Distance(flatPos, flatCenter);

        if (distance > maxRadius)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            controller.moveEnabled = false;
        }
        else
        {
            controller.moveEnabled = true;
        }
    }

    void UpdateBoundsFromCylinder()
    {
        center = cylinderObject.transform.position;

        Vector3 scale = cylinderObject.transform.lossyScale;
        maxRadius = Mathf.Max(scale.x, scale.z) * 0.5f; 
    }

    void OnDrawGizmosSelected()
    {
        if (cylinderObject == null) return;

        Vector3 center = cylinderObject.transform.position;
        center.y += cylinderObject.transform.localScale.y;
        Vector3 scale = cylinderObject.transform.lossyScale;
        float gizmoRadius = Mathf.Max(scale.x, scale.z) * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, gizmoRadius);
    }
}