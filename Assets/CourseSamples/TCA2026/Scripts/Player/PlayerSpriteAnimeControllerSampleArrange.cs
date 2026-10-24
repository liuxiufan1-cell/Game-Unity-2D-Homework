using JetBrains.Annotations;
using UnityEngine;

public class PlayerSpriteAnimeControllerSampleArrange : MonoBehaviour
{

    public PlayerController2DSampleArrange1 _playerController2DSample;
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
        isIdle = _playerController2DSample!.isIdle;
        isGrounded = _playerController2DSample!.isGrounded;

        if (isGrounded == false)
        {
            //ジャンプ
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
