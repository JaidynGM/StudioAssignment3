using UnityEngine;
using System.Collections;

public class DashHandler
{
    private Rigidbody rb;
    private float dashForce;
    private float dashCooldown;
    private float dashDuration = 0.2f;
    private float dashCooldownTimer = 0f;
    private Transform cameraTransform;
    private Vector2 currentInputDirection;

    public bool isDashing { get; private set; } = false;
    public float DashCooldownRemaining => dashCooldownTimer;
    public float DashCooldownPercent => dashCooldownTimer / dashCooldown;

    public DashHandler(Rigidbody playerRb, float dashForce, float dashCooldown, Transform cameraTransform, float dashDuration = 0.2f)
    {
        this.rb = playerRb;
        this.dashForce = dashForce;
        this.dashCooldown = dashCooldown;
        this.dashDuration = dashDuration;
        this.cameraTransform = cameraTransform;
    }

    public void UpdateDashCooldown(float deltaTime)
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= deltaTime;
        }
    }

    public void SetInputDirection(Vector2 inputDirection)
    {
        this.currentInputDirection = inputDirection;
    }

    public bool CanDash()
    {
        return dashCooldownTimer <= 0 && !isDashing;
    }

    public void Dash(Transform playerTransform)
    {
        if (!CanDash())
        {
            return;
        }

        Vector3 dashDirection;

        // If we have camera and input direction, use camera-relative direction
        if (cameraTransform != null && currentInputDirection.magnitude > 0.1f)
        {
            // Calculate camera-relative direction
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            if (camForward.magnitude < 0.001f) camForward = Vector3.forward;
            if (camRight.magnitude < 0.001f) camRight = Vector3.right;

            camForward.Normalize();
            camRight.Normalize();

            // Use the same direction calculation as the movement system
            dashDirection = camForward * currentInputDirection.y + camRight * currentInputDirection.x;
            dashDirection.Normalize();
        }
        else
        {
            // Fall back to player's forward direction if no input or camera
            dashDirection = playerTransform.forward;
        }

        // Apply dash force
        rb.linearVelocity = new Vector3(
            dashDirection.x * dashForce,
            rb.linearVelocity.y, // Preserve vertical velocity
            dashDirection.z * dashForce
        );

        // Set dashing state and reset cooldown
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        // Start dash end coroutine
        MonoBehaviour monoBehaviour = rb.GetComponent<MonoBehaviour>();
        if (monoBehaviour != null)
        {
            monoBehaviour.StartCoroutine(EndDashAfterDuration());
        }
    }

    private IEnumerator EndDashAfterDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
}

