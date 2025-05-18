using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonController : Singleton<ThirdPersonController>
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public Transform cameraPivot;
    public Transform cameraTransform;
    public float cameraRotateSpeed = 100f;

    private Rigidbody rb;

    [HideInInspector]
    public Interactabe targetInteractable;

    float horizontal;
    float vertical;
    float rotateInput;
    bool isMoving;

    [HideInInspector]
    public bool moveEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraPivot == null && cameraTransform != null)
            cameraPivot = cameraTransform.parent;
    }

    private void Update()
    {
        if (moveEnabled)
        {
            GatherInput();
        }
    }

    private void FixedUpdate()
    {
        MovementUpdate();
    }

    void GatherInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        rotateInput = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotateInput = -1f;
        else if (Input.GetKey(KeyCode.E))
            rotateInput = 1f;

        if (Input.GetKeyDown(KeyCode.Space)) Interact();
    }

    void MovementUpdate()
    {
        if (rotateInput != 0f && cameraPivot != null)
        {
            cameraPivot.Rotate(Vector3.up, rotateInput * cameraRotateSpeed * Time.deltaTime);
        }

        if (horizontal == 0 && vertical == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

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

    void Interact()
    {
        if (!isMoving && targetInteractable != null)
        {
            targetInteractable.GetComponent<Interactabe>().Activate();
        }
    }
}