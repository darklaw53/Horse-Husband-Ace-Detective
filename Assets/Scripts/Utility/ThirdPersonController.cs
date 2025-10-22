using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonController : Singleton<ThirdPersonController>
{
    public float rotationSpeed = 10f;

    public Transform cameraPivot;
    public Transform cameraTransform;
    public float cameraRotateSpeed = 100f;

    public bool useCameraRelativeMovement = true;

    public Animator animator;

    [Header("Animation Settings")]
    [Tooltip("Controls how fast the animations play.")]
    public float animationSpeed = 1f;

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

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        HandleRotation();
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

    void UpdateAnimator()
    {
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        isMoving = inputDirection.magnitude >= 0.1f;

        animator.SetBool("IsMoving", isMoving);

        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }

    void HandleRotation()
    {
        if (rotateInput != 0f && cameraPivot != null)
        {
            cameraPivot.Rotate(Vector3.up, rotateInput * cameraRotateSpeed * Time.deltaTime);
        }

        if (!isMoving) return;

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        Vector3 moveDirection;
        if (useCameraRelativeMovement && cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
        }
        else
        {
            moveDirection = inputDirection;
        }

        moveDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    void Interact()
    {
        if (!isMoving && targetInteractable != null)
        {
            targetInteractable.GetComponent<Interactabe>().Activate();
        }
    }
}