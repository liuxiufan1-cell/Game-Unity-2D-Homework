using UnityEngine;

public class AttackArea : MonoBehaviour
{

    // この攻撃判定が敵に与えるダメージ量（今はDebug.Logで表示するだけ）
    public int damageAmount = 1;

    void Start()
    {
        // ゲーム開始時は、攻撃判定コライダーをOFFにしておく（攻撃モーション中だけONにするため）
        OnAttackEnd();
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // もし衝突した相手が「Enemy」タグを持っていたら
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // プレイヤーにダメージを与える（今はDebugLogのみ）
            Debug.Log($"敵に {damageAmount} ダメージ！");

            // 将来的には、ここでプレイヤーのライフを減らす関数を呼ぶ
            collision.GetComponent<EnemyController2D>().TakeDamage(damageAmount);
        }
    }
    */

    public void OnAttackStart()
    {
        this.gameObject.SetActive(true);
    }

    public void OnAttackEnd()
    {
        this.gameObject.SetActive(false);
    }
}
