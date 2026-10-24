using UnityEngine;

public class PlayerSpriteAnimeController : MonoBehaviour
{

    public  PlayerController2D _playerController2D;
    private Animator _animator;

    private bool isIdle;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isIdle = _playerController2D!.isIdle;
        isGrounded = _playerController2D!.isGrounded;

        if (isGrounded == false)
        {
            //ƒWƒƒƒ“ƒv
            _animator.SetBool("isGrounded", false);
            _animator.SetBool("isIdle", true);
        }
        else
        {
            _animator.SetBool("isGrounded", true);
            if (isIdle == false)
            {
                //Run
                _animator.SetBool("isIdle", false);
            }
            else
            {
                //Idle
                _animator.SetBool("isIdle", true);
            }
        }
    }
}
