using UnityEngine;

public class FinishFlagHit : MonoBehaviour
{

    public GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    //Collider2DのisTriggerがOFFの場合のHit時に呼ばれる関数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collision.gameObject.name + "と接触。");

        if (collision.gameObject.CompareTag("Player"))
        {
            //ゲームクリア
            //_gameManager!.isGameClear = true;
            _gameManager.OnGameClear();
        }
    }

    //Collider2DのisTriggerがONの場合のHit時に呼ばれる関数
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //当たった相手をデバッグ表示
        Debug.Log(this.gameObject.name + "は、" + collider.gameObject.name + "とTrigger接触。");

    }


}
