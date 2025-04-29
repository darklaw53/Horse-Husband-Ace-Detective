using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public Transform cameraPivot;
    public Transform cameraTransform;
    public float cameraRotateSpeed = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraPivot == null && cameraTransform != null)
            cameraPivot = cameraTransform.parent;
    }

    void FixedUpdate()
    {
        RotateCameraWithKeys();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
            moveDirection.Normalize();

            Vector3 targetVelocity = moveDirection * moveSpeed;
            Vector3 velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
            rb.velocity = velocity;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void RotateCameraWithKeys()
    {
        float rotateInput = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotateInput = -1f;
        else if (Input.GetKey(KeyCode.E))
            rotateInput = 1f;

        if (rotateInput != 0f && cameraPivot != null)
        {
            cameraPivot.Rotate(Vector3.up, rotateInput * cameraRotateSpeed * Time.deltaTime);
        }
    }
}