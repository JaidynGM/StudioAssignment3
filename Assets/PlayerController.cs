using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // General paramters and calls
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleJumpForce = 4f;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cameraTransform;

    // Movement parameters
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementDampening = 0.9f;

    // Double jump parameters
    [SerializeField] private bool doubleJumpEnabled = true;
    [SerializeField] private float doubleJumpCooldown = 1.0f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    private float doubleJumpCooldownTimer = 0f;
    private Vector2 currentInputDirection;

    private void Start()
    {
        inputManager.OnMove.AddListener(HandleMoveInput);
        rb = GetComponent<Rigidbody>();
        inputManager.OnSpacePressed.AddListener(Jump);

        if (groundCheck != null)
        {
            groundCheck.OnGroundChange.AddListener(UpdateGroundedStatus);
        }
    }

    private void Update()
    {
        if (doubleJumpCooldownTimer > 0)
        {
            doubleJumpCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleMoveInput(Vector2 inputDirection)
    {
        currentInputDirection = inputDirection;
    }

    private void FixedUpdate()
    {
        MovePlayer(currentInputDirection);

        if (currentInputDirection.magnitude < 0.1f)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
            horizontalVelocity *= movementDampening;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
        }
    }

    private void MovePlayer(Vector2 inputDirection)
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        if (camForward.magnitude < 0.001f) camForward = Vector3.forward;
        if (camRight.magnitude < 0.001f) camRight = Vector3.right;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * inputDirection.y + camRight * inputDirection.x;

        if (moveDirection.magnitude > 0.01f)
        {
            moveDirection.Normalize();

            rb.AddForce(acceleration * moveDirection, ForceMode.Acceleration);

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            PerformJump(jumpForce);

            if (doubleJumpEnabled)
            {
                canDoubleJump = true;
                hasDoubleJumped = false;
            }
        }
        else if (doubleJumpEnabled && canDoubleJump && !hasDoubleJumped && doubleJumpCooldownTimer <= 0)
        {
            PerformDoubleJump();
        }
    }

    private void PerformJump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    private void PerformDoubleJump()
    {
        PerformJump(doubleJumpForce);

        hasDoubleJumped = true;
        canDoubleJump = false;
        doubleJumpCooldownTimer = doubleJumpCooldown;
    }

    private void UpdateGroundedStatus(bool grounded)
    {
        isGrounded = grounded;

        if (grounded)
        {
            canDoubleJump = false;
            hasDoubleJumped = false;
        }
    }
}