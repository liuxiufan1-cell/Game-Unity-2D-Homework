using UnityEngine;

//敵の登場を処理するクラス
public class EnemySpawn : MonoBehaviour
{
    public GameObject _enemy;
    public bool _isSpawn = false;   //true...敵を生む。false...生まない。
    public float _speed = -2.0f;    //敵の横の速度。

    void Update()
    {
        if (_isSpawn == true)
        {
            OnEnemySpawn();

            _isSpawn = false;
        }
    }

    private void OnEnemySpawn()
    {
        Debug.Log("敵を生みます。");

        Vector3 spawnPosition = this.transform.position;

        //Enemyを1つ生成する。
        Instantiate( _enemy, spawnPosition, Quaternion.identity );

        //Enemyの初期化
        EnemySetup();
    }

    private void EnemySetup()
    {
        EnemyController2D enemyController = _enemy.GetComponent<EnemyController2D>();
        if (enemyController != null)
        {
            enemyController.speed = _speed;
        }
    }
}
