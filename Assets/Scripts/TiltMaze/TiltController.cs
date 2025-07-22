using UnityEngine;

public class TiltController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How fast the object rotates horizontally (Z-axis).")]
    public float horizontalRotationSpeed = 100f;

    [Tooltip("How fast the object rotates vertically (X-axis).")]
    public float verticalRotationSpeed = 100f;

    [Tooltip("Maximum rotation on the Z axis (left/right tilt).")]
    public float maxZRotation = 45f;

    [Tooltip("Maximum rotation on the X axis (up/down tilt).")]
    public float maxXRotation = 45f;

    private float screenCenterX;
    private float screenCenterY;

    void Start()
    {
        screenCenterX = Screen.width / 2f;
        screenCenterY = Screen.height / 2f;
    }

    void Update()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        float horizontalOffset = (mouseX - screenCenterX) / screenCenterX;
        float verticalOffset = (mouseY - screenCenterY) / screenCenterY;

        float rotationZ = Mathf.Clamp(horizontalOffset * horizontalRotationSpeed, -maxZRotation, maxZRotation);
        float rotationX = Mathf.Clamp(-verticalOffset * verticalRotationSpeed, -maxXRotation, maxXRotation);

        transform.rotation = Quaternion.Euler(-rotationX, 0f, -rotationZ);
    }
}