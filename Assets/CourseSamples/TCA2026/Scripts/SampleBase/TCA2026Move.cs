using UnityEngine;

public class TCA2026Move : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed = 0f;

    private Vector2 move2d = new Vector2(0, 0);

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        CharaMove();
    }

    private void CharaMove()
    {
        move2d.x = speed;
        move2d.y = rb.linearVelocity.y;
        rb.linearVelocity = move2d;

    }

    private void EnemyStop()
    {
        speed = 0;
    }

}
