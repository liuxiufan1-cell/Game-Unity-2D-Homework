using UnityEngine;

public sealed class WalkerEnemy : EnemyBase
{
    [SerializeField] private float turnPause = 0.45f;
    private float pauseRemaining = 0.5f;

    private void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }

        if (pauseRemaining > 0f)
        {
            pauseRemaining -= Time.fixedDeltaTime;
            Body.linearVelocity = new Vector2(0f, Body.linearVelocity.y);
            SetMoving(false);
            return;
        }

        float previousDirection = Direction;
        if (!ReverseIfFacingWall() && !ReverseIfNoGroundAhead())
        {
            ReverseAtPatrolBoundary();
        }

        if (!Mathf.Approximately(previousDirection, Direction))
        {
            pauseRemaining = turnPause;
        }

        Body.linearVelocity = new Vector2(Direction * moveSpeed, Body.linearVelocity.y);
        SetMoving(true);
    }
}
