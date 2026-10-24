using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//敵の登場を管理するクラス
public class EnemyGeneratorManager : SingletonMonoBehaviour<EnemyGeneratorManager>
{
    private EnemySpawn[] enemySpawns;   //エネミースポーンの配列

    void Start()
    {
        //ヒエラルキーに存在するEnemySpawnを全て取得（処理重い。）
        enemySpawns = FindObjectsByType<EnemySpawn>(FindObjectsSortMode.None);

        EnemyAllSpawn();
    }

    public void EnemyAllSpawn()
    {
        //全てのEnemySpawnに敵を生むように指示。
        foreach (EnemySpawn spawn in enemySpawns)
        {
            spawn._isSpawn = true;
        }
    }


}
