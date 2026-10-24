using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 1.5f;
    [SerializeField] protected float patrolDistance = 4f;
    [SerializeField] private float deathDuration = 0.55f;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField, Min(0.02f)] private float wallCheckDistance = 0.18f;

    protected Rigidbody2D Body { get; private set; }
    protected float SpawnX { get; private set; }
    protected float Direction { get; set; } = -1f;

    public bool IsDead { get; private set; }

    protected virtual void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        SpawnX = transform.position.x;
    }

    protected void SetMoving(bool moving)
    {
        animator.SetBool("Moving", moving);
        if (moving && Mathf.Abs(Body.linearVelocity.x) > 0.01f)
        {
            spriteRenderer.flipX = Body.linearVelocity.x > 0f;
        }
    }

    protected void ReverseAtPatrolBoundary()
    {
        if (Mathf.Abs(transform.position.x - SpawnX) < patrolDistance)
        {
            return;
        }

        Direction = transform.position.x > SpawnX ? -1f : 1f;
    }

    protected bool IsGrounded()
    {
        if (groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, 0.16f, groundMask) != null)
        {
            return true;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 checkCenter = new(bounds.center.x, bounds.min.y - 0.04f);
        Vector2 checkSize = new(bounds.size.x * 0.7f, 0.12f);
        return Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundMask) != null;
    }

    protected bool ReverseIfFacingWall()
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new(bounds.center.x, bounds.min.y + bounds.size.y * 0.6f);
        Vector2 castDirection = Vector2.right * Direction;
        float castDistance = bounds.extents.x + wallCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(origin, castDirection, castDistance, groundMask);

        if (hit.collider == null || Vector2.Dot(hit.normal, castDirection) > -0.55f)
        {
            return false;
        }

        TurnAwayFromWall(hit.normal.x);
        return true;
    }

    protected bool ReverseIfNoGroundAhead()
    {
        if (!IsGrounded())
        {
            return false;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new(
            bounds.center.x + Direction * (bounds.extents.x + 0.08f),
            bounds.min.y + 0.12f);
        RaycastHit2D groundAhead = Physics2D.Raycast(origin, Vector2.down, 0.65f, groundMask);
        if (groundAhead.collider != null)
        {
            return false;
        }

        SetDirection(-Direction);
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDead || (groundMask.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            float wallNormalX = collision.GetContact(i).normal.x;
            if (Mathf.Abs(wallNormalX) < 0.5f)
            {
                continue;
            }

            TurnAwayFromWall(wallNormalX);
            return;
        }
    }

    private void TurnAwayFromWall(float wallNormalX)
    {
        SetDirection(Mathf.Sign(wallNormalX));
    }

    private void SetDirection(float newDirection)
    {
        Direction = newDirection;
        spriteRenderer.flipX = Direction > 0f;
        Body.linearVelocity = new Vector2(Direction * Mathf.Abs(Body.linearVelocity.x), Body.linearVelocity.y);
    }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        Body.linearVelocity = Vector2.zero;
        Body.simulated = false;
        bodyCollider.enabled = false;
        animator.SetTrigger("Death");
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(deathDuration);
        Destroy(gameObject);
    }
}
