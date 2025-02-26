using UnityEngine;

public class MovementHandler
{
    private Rigidbody rb;
    private Transform cameraTransform;
    private float acceleration;
    private float maxSpeed;
    private float rotationSpeed;
    private float movementDampening;
    private Vector2 inputDirection;

    public MovementHandler(Rigidbody rb, Transform cameraTransform, float acceleration, float maxSpeed, float rotationSpeed, float movementDampening)
    {
        this.rb = rb;
        this.cameraTransform = cameraTransform;
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
        this.rotationSpeed = rotationSpeed;
        this.movementDampening = movementDampening;
    }

    public void UpdateInput(Vector2 newInputDirection)
    {
        inputDirection = newInputDirection;
    }

    public void Move()
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
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void ApplyDampening()
    {
        if (inputDirection.magnitude < 0.1f)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
            horizontalVelocity *= movementDampening;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
        }
    }
}
