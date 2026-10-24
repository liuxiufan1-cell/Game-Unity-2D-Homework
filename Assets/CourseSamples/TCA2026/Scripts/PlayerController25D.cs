using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController25D : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    private Rigidbody rb;
    private InputSystem_Actions inputActions;

    public AnimeSample2D _animeSample;

    public bool isGrounded;   //着地している？

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        _animeSample = this.GetComponent<AnimeSample2D>();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Attack.started += OnPlayerLeftButton;
        inputActions.Player.Jump.started += OnPlayerDownButton;
        inputActions.Player.Crouch.started += OnPlayerRightButton;
        inputActions.Player.Interact.started += OnPlayerUpButton;
        inputActions.Player.Previous.started += OnPlayerPreviousButton;
        inputActions.Player.Next.started += OnPlayerNextButton;
        inputActions.Player.Sprint.canceled += OnPlayerShiftButton;

    }

    private void OnDisable()
    {
        inputActions.Player.Attack.started -= OnPlayerLeftButton;
        inputActions.Player.Jump.started -= OnPlayerDownButton;
        inputActions.Player.Crouch.started -= OnPlayerRightButton;
        inputActions.Player.Interact.started -= OnPlayerUpButton;
        inputActions.Player.Previous.started -= OnPlayerPreviousButton;
        inputActions.Player.Next.started -= OnPlayerNextButton;
        inputActions.Player.Sprint.canceled -= OnPlayerShiftButton;

        inputActions.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        // 1. 左右の入力を取得（-1 から 1）
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector2 move2d = new Vector3(input.x, 0, 0);
/*
        transform.Translate(move2d * Time.deltaTime * moveSpeed);
        if (input == Vector2.zero)
        {
            _animeSample?.OnIdle();
        }
        else
        {
            _animeSample?.OnMove();
        }
  */
        // 2. 左右移動の速度をRigidbody2Dに伝える
        rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);

        // 3. 入力に合わせてキャラクターの向き（左右）を変える
        if (input.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // 右を向く
        }
        else if (input.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 左を向く（反転）
        }

    }

    //Enter Key
    private void OnPlayerLeftButton(InputAction.CallbackContext context)
    {
        Debug.Log("Left X Button");
    }

    //Space Key
    private void OnPlayerDownButton(InputAction.CallbackContext context)
    {
        if (isGrounded == true)
        {
            //rb!.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
        }

        Debug.Log("Down A Button");
    }

    //C key
    private void OnPlayerRightButton(InputAction.CallbackContext context)
    {
        Debug.Log("Right B Button");
    }

    //E key
    private void OnPlayerUpButton(InputAction.CallbackContext context)
    {
        Debug.Log("Up Y Button");
    }

    //1 Key
    private void OnPlayerPreviousButton(InputAction.CallbackContext context)
    {
        Debug.Log("Previous Button");
    }

    //2 Key
    private void OnPlayerNextButton(InputAction.CallbackContext context)
    {
        Debug.Log("Next Button");
    }

    //Left Shift Key
    private void OnPlayerShiftButton(InputAction.CallbackContext context)
    {
        Debug.Log("Shift Button");
    }

    /*
    // 地面（Tilemapなど）に触れている間の判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    */

    // 地面（Tilemapなど）に触れている間の判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

}
