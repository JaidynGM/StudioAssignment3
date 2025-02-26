using UnityEngine;

public class JumpHandler
{
    private Rigidbody rb;
    private float jumpForce;
    private float doubleJumpForce;
    private bool doubleJumpEnabled;
    private float doubleJumpCooldown;
    private float doubleJumpCooldownTimer = 0f;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool hasDoubleJumped;

    public JumpHandler(Rigidbody playerRb, float jumpForce, float doubleJumpForce, bool doubleJumpEnabled, float doubleJumpCooldown)
    {
        this.rb = playerRb;
        this.jumpForce = jumpForce;
        this.doubleJumpForce = doubleJumpForce;
        this.doubleJumpEnabled = doubleJumpEnabled;
        this.doubleJumpCooldown = doubleJumpCooldown;
    }

    public void UpdateJumpCooldown(float deltaTime)
    {
        if (doubleJumpCooldownTimer > 0)
        {
            doubleJumpCooldownTimer -= deltaTime;
        }
    }

    public void Jump()
    {
        if (!GameManager.Instance.jumpEnabled)
        {
            return;
        }

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

    public void UpdateGroundedStatus(bool grounded)
    {
        isGrounded = grounded;

        if (grounded)
        {
            canDoubleJump = false;
            hasDoubleJumped = false;
        }
    }

}

