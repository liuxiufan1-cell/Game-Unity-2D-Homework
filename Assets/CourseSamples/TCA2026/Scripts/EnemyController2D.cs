using UnityEngine;
using UnityEngine.Windows;

public class EnemyController2D : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;

    public int animeNumber;
    public float speed = 0f;
    private Vector2 move2d = new Vector2(0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animeNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMove();
    }

    private void EnemyMove()
    {
        move2d.x = speed;
        move2d.y = rb.linearVelocity.y;
        rb.linearVelocity = move2d;

    }

    private void EnemyStop()
    {
        speed = 0;
    }

    // プレイヤーが触れれば死ぬ。
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animeNumber = 2;
            EnemyStop();
        }
    }

}
