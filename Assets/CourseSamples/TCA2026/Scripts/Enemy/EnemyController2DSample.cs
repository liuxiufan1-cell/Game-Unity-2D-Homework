using UnityEngine;
//using UnityEngine.Windows;

public class EnemyController2DSample : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public int animeNumber;
    public float speed = 0f;
    public int damage;

    public GameManager _gameManager;

    private Vector2 move2d = new Vector2(0, 0);
    private Vector3 frontScale = new Vector3(0, 0, 0);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        boxCollider = this.GetComponent<BoxCollider2D>();

        animeNumber = 0;

        //GameManagerを登録
        _gameManager = FindFirstObjectByType<GameManager>();

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

    // PlayerAttackに触れればダメージを受ける。
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collider.gameObject.name + "とTrigger接触。");

        if (collider.gameObject.CompareTag("PlayerAttack"))
        {
            TakeDamage(1);
        }
    }

    //地形のデスラインに触れれば即死。
    //床以外と接触すれば向きを変える。
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collision.gameObject.name + "と接触。");

        if (collision.gameObject.CompareTag("DeathLine")) 
        {
            this.OnDeath();
        }
        else if (collision.gameObject.CompareTag("Ground") == false)
        {
            //向きを反転する。
            speed *= -1.0f;
            frontScale = this.transform.localScale;
            frontScale.x *= -1.0f;
            this.transform.localScale = frontScale;
            Debug.Log(this.gameObject.name + "向きを反転。");
        }
    }

    //ダメージを受ける
    public void TakeDamage(int _damage)
    {
        damage = _damage;
        animeNumber = 2;
        EnemyStop();
    }

    //即死。
    public void OnDeath()
    {
        Destroy(this.gameObject , 1);
    }

}
