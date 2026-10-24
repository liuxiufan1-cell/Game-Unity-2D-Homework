using UnityEngine;

public class AnimeSample2D : MonoBehaviour
{

    public Animator animator;
    public RuntimeAnimatorController idle;
    public RuntimeAnimatorController walkF;
    public RuntimeAnimatorController Jump;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.runtimeAnimatorController = walkF;
    }


    public void OnIdle()
    {
        animator.runtimeAnimatorController = idle;
    }

    public void OnMove()
    {
        animator.runtimeAnimatorController = walkF;
    }

    public void OnJump()
    {
        animator.runtimeAnimatorController = Jump;
    }

    public void OnLanding()
    {
        animator.runtimeAnimatorController = idle;
    }

}
