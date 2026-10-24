using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public sealed class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField, Min(1)] private int maxJumpCount = 2;
    [SerializeField] private float stompBouncePower = 7f;
    [SerializeField] private float deathDuration = 0.7f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.18f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D bodyCollider;

    private InputSystem_Actions inputActions;
    private Rigidbody2D body;
    private float horizontalInput;
    private int jumpCount;
    private bool grounded;
    private bool dead;
    private bool completed;

    public event Action<PlayerCharacter> Died;

    public bool IsDead => dead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Jump.started += OnJump;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void Update()
    {
        horizontalInput = dead || completed
            ? 0f
            : inputActions.Player.Move.ReadValue<Vector2>().x;

        if (!dead)
        {
            if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
            }

            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("Grounded", grounded);
        }
    }

    private void FixedUpdate()
    {
        if (dead || completed)
        {
            return;
        }

        bool touchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
        grounded = touchingGround && body.linearVelocity.y <= 0.05f;
        if (grounded)
        {
            jumpCount = 0;
        }

        body.linearVelocity = new Vector2(horizontalInput * moveSpeed, body.linearVelocity.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (dead || completed || jumpCount >= maxJumpCount)
        {
            return;
        }

        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        jumpCount++;
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || !collision.gameObject.TryGetComponent(out EnemyBase enemy) || enemy.IsDead)
        {
            return;
        }

        ContactPoint2D contact = collision.GetContact(0);
        bool stomped = body.linearVelocity.y <= 0.2f && contact.normal.y > 0.45f;
        if (stomped)
        {
            enemy.Die();
            body.linearVelocity = new Vector2(body.linearVelocity.x, stompBouncePower);
        }
        else
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (dead || completed)
        {
            return;
        }

        dead = true;
        horizontalInput = 0f;
        inputActions.Disable();
        body.linearVelocity = Vector2.zero;
        body.simulated = false;
        bodyCollider.enabled = false;
        animator.SetTrigger("Death");
        StartCoroutine(DeathSequence());
    }

    public void CompleteLevel()
    {
        if (dead || completed)
        {
            return;
        }

        completed = true;
        horizontalInput = 0f;
        inputActions.Disable();
        body.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0f);
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathDuration);
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
