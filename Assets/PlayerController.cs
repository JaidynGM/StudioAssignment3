using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementDampening = 0.9f;

    private Rigidbody rb;
    private bool isGrounded;
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
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * inputDirection.y + camRight * inputDirection.x).normalized;

        rb.AddForce(acceleration * moveDirection, ForceMode.Acceleration);

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
        }

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void UpdateGroundedStatus(bool grounded)
    {
        isGrounded = grounded;
    }
}