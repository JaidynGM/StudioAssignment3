using UnityEngine;

public class DashHandler
{
    private Rigidbody rb;
    private float dashForce;
    private float dashCooldown;
    private float dashCooldownTimer = 0f;
    public bool isDashing { get; private set; } = false;

    public DashHandler(Rigidbody playerRb, float dashForce, float dashCooldown)
    {
        this.rb = playerRb;
        this.dashForce = dashForce;
        this.dashCooldown = dashCooldown;
    }

    public void UpdateDashCooldown(float deltaTime)
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= deltaTime;
        }
    }

    public void Dash(Transform playerTransform)
    {
        if (dashCooldownTimer <= 0)
        {
            Vector3 dashDirection = playerTransform.forward;
            rb.linearVelocity = new Vector3(dashDirection.x * dashForce, rb.linearVelocity.y, dashDirection.z * dashForce);

            isDashing = true;
            dashCooldownTimer = dashCooldown;

            rb.gameObject.GetComponent<PlayerController>().StartCoroutine(EndDash());
        }
    }

    private System.Collections.IEnumerator EndDash()
    {
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
    }
}

