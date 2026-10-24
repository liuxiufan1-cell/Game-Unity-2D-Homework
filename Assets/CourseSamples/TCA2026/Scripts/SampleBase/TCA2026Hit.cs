using UnityEngine;

public class TCA2026Hit : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //〇〇〇Collider2DのisTriggerがOFFの場合のHit時に呼ばれる関数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collision.gameObject.name + "と接触。");

    }

    //〇〇〇Collider2DのisTriggerがONの場合のHit時に呼ばれる関数
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collider.gameObject.name + "とTrigger接触。");

    }


}
