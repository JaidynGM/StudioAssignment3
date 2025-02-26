using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cameraTransform;

    // Movement parameters
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementDampening = 0.9f;

    // Jump parameters
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleJumpForce = 4f;
    [SerializeField] private bool doubleJumpEnabled = true;
    [SerializeField] private float doubleJumpCooldown = 1.0f;

    // Dash parameters
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 2.0f;

    private Rigidbody rb;
    private JumpHandler jumpHandler;
    public DashHandler dashHandler { get; private set; }
    private MovementHandler movementHandler;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        jumpHandler = new JumpHandler(rb, jumpForce, doubleJumpForce, doubleJumpEnabled, doubleJumpCooldown);
        dashHandler = new DashHandler(rb, dashForce, dashCooldown);
        movementHandler = new MovementHandler(rb, cameraTransform, acceleration, maxSpeed, rotationSpeed, movementDampening);

        inputManager.OnMove.AddListener(movementHandler.UpdateInput);
        inputManager.OnSpacePressed.AddListener(jumpHandler.Jump);
        inputManager.OnDashPressed.AddListener(() => dashHandler.Dash(transform));

        if (groundCheck != null)
        {
            groundCheck.OnGroundChange.AddListener(UpdateGroundedStatus);
        }
    }

    private void Update()
    {
        jumpHandler.UpdateJumpCooldown(Time.deltaTime);
        dashHandler.UpdateDashCooldown(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        movementHandler.Move();
        movementHandler.ApplyDampening();
    }

    private void UpdateGroundedStatus(bool grounded)
    {
        isGrounded = grounded;
        jumpHandler.UpdateGroundedStatus(grounded);
    }
}

