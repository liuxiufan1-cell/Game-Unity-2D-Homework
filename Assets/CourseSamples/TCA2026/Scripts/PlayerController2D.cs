using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController2D : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;

//    public AnimeSample2D _animeSample;

    public bool isGrounded;   //着地している？
    public bool isIdle;       //待機状態？

    private Vector3 lookRight = new Vector3(1, 1, 1); // 右を向く
    private Vector3 lookLeft =  new Vector3(-1, 1, 1); // 左を向く
    private Vector2 move2d = new Vector2(0, 0);

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
 //       _animeSample = this.GetComponent<AnimeSample2D>();
        lookRight = Vector3.Scale(lookRight , this.transform.localScale);
        lookLeft = Vector3.Scale(lookLeft, this.transform.localScale);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Attack.started += OnPlayerLeftButton;
        inputActions.Player.Jump.started += OnPlayerDownButton;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.started -= OnPlayerLeftButton;
        inputActions.Player.Jump.started -= OnPlayerDownButton;

        inputActions.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        // 1. 左右の入力を取得（-1 から 1）
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        // 2. 左右移動の速度をRigidbody2Dに伝える
        move2d.x = input.x * moveSpeed;
        move2d.y = rb.linearVelocity.y;
        rb.linearVelocity = move2d;

        // 3. 入力に合わせてキャラクターの向き（左右）を変える
        if (input.x > 0)
        {
            transform.localScale = lookRight; // 右を向く
        }
        else if (input.x < 0)
        {
            transform.localScale = lookLeft; // 左を向く（反転）
        }

        if(isGrounded==true && rb.linearVelocityX == 0.0f)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
        }

        Debug.Log("Down A Button");
    }


    // 地面（Tilemapなど）に触れている間の判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

}
