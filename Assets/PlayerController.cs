using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private inputManager inputManager;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cameraTransform;

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

        rb.AddForce(speed * moveDirection, ForceMode.Force);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
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
