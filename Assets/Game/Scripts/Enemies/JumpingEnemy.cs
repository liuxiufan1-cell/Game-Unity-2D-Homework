using UnityEngine;

public sealed class JumpingEnemy : EnemyBase
{
    [SerializeField] private float jumpPower = 7.5f;
    [SerializeField] private float jumpInterval = 1.2f;

    private float jumpTimer = 0.8f;

    private void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }

        if (!ReverseIfFacingWall())
        {
            ReverseAtPatrolBoundary();
        }

        bool grounded = IsGrounded() && Body.linearVelocity.y <= 0.05f;

        if (!grounded)
        {
            Body.linearVelocity = new Vector2(Direction * moveSpeed, Body.linearVelocity.y);
            SetMoving(true);
            return;
        }

        Body.linearVelocity = new Vector2(0f, Body.linearVelocity.y);
        SetMoving(false);
        jumpTimer -= Time.fixedDeltaTime;

        if (grounded && jumpTimer <= 0f)
        {
            Body.linearVelocity = new Vector2(Direction * moveSpeed, jumpPower);
            jumpTimer = jumpInterval;
        }
    }
}
