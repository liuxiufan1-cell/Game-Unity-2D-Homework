using UnityEngine;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    public float spawn1stTime = 2f;
    public float spawnLoopTime = 4f;

    public EnemyGeneratorManager _generator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _generator = FindAnyObjectByType<EnemyGeneratorManager>();

        // 2秒後から、4秒間隔でSpawnEnemyメソッドを繰り返し呼び出す
        InvokeRepeating("SpawnEnemy", spawn1stTime, spawnLoopTime);
    }

    void Update()
    {

    }

    void SpawnEnemy()
    {
        _generator.EnemyAllSpawn();
    }
}
