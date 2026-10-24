using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//プレイヤーの登場を管理するクラス
public class PlayerGeneratorManager : SingletonMonoBehaviour<PlayerGeneratorManager>
{
    public PlayerSpawn _playerSpawn;        //プレイヤーの登場クラス
    public PlayerManager _playerManager;    //プレイヤーの管理クラス

    private void Start()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    //プレイヤーを再登場させる。
    private void PlayerSpawn()
    {
        _playerSpawn._isSpawn = true;
    }

    //プレイヤーの再登場を要求する。
    public void PlayerRespawnRequest()
    {
        Debug.Log("1秒後にプレイヤー登場");
        StartCoroutine(RespawnRequest());
    }

    IEnumerator RespawnRequest()
    {
        // 1秒間処理を一時停止
        yield return new WaitForSeconds(1.0f);

        //プレイヤー再登場
        PlayerSpawn();
    }

}
