using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;

public class PlayerController2DSample : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;

    public bool isGrounded;   //着地している？
    public bool isIdle;       //待機状態？
    public bool IsDameged;     //ダメージをうけた？

    public GameManager _gameManager;
    public PlayerManager _playerManager;    //プレイヤーの管理クラス

    private Vector3 lookRight = new Vector3(1, 1, 1); // 右を向く
    private Vector3 lookLeft =  new Vector3(-1, 1, 1); // 左を向く
    private Vector2 move2d = new Vector2(0, 0);

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        lookRight = Vector3.Scale(lookRight , this.transform.localScale);
        lookLeft = Vector3.Scale(lookLeft, this.transform.localScale);

        //GameManagerを登録
        _gameManager = FindFirstObjectByType< GameManager > ();
        //PlayerManagerを登録
        _playerManager = FindAnyObjectByType<PlayerManager>();

        GetPlayerCamera();

        IsDameged = false;
    }

    private void GetPlayerCamera()
    {
        Transform children = this.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            //カメラか？
            if (ob.name == "Main Camera")
            {
                  var cameraObject = ob.gameObject;
                _playerManager.ActiveOnPlayerCamera(cameraObject);
                return;
            }
        }
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
        //Debug.Log("Left X Button");
    }

    //Space Key
    private void OnPlayerDownButton(InputAction.CallbackContext context)
    {
        if (isGrounded == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
        }

        //Debug.Log("Down A Button");
    }

    // 地面（Tilemapなど）に触れている間の判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collision.gameObject.name + "と接触。");


        if (collision.gameObject.CompareTag("Enemy"))
        {
            IsDameged = true;
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("DeathLine"))  //落下してデスラインに触れた？
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Debug.Log($"落下してデスラインに触れた。");

        _gameManager.OnPlayerDeath();

        StartCoroutine(PlayerDeathRequest());

    }

    IEnumerator PlayerDeathRequest()
    {
        // 0.7秒間処理を一時停止
        yield return new WaitForSeconds(0.7f);

        //カメラ切り替え
        _playerManager.ActiveOffPlayerCamera();

        // 0.1秒間処理を一時停止
        yield return new WaitForSeconds(0.1f);

        //削除
        Destroy(this.gameObject);
    }

}
