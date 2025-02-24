using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private inputManager inputManager;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private GroundCheck groundCheck;

    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        inputManager.OnMove.AddListener(MovePlayer);
        rb = GetComponent<Rigidbody>();
        inputManager.OnSpacePressed.AddListener(Jump);

        if(groundCheck != null)
        {
            groundCheck.OnGroundChange.AddListener(UpdateGroundedStatus);
        }
    }

    private void MovePlayer(Vector2 direction)
    {
        Vector3 moveDirection = new(direction.x, 0f, direction.y);
        rb.AddForce(speed * moveDirection);
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
