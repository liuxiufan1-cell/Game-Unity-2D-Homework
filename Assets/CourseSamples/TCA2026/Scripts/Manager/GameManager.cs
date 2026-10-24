using UnityEngine;
using System;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isPlayerDeath;      //プレイヤー死亡フラグ
    public bool isGameClear;        //ゲームクリアフラグ

    public PlayerManager _playerManager;
    public EnemyManager _enemyManager;
    public CameraManager _cameraManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
        _cameraManager = FindAnyObjectByType<CameraManager>();

        isPlayerDeath = false;
        isGameClear = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerDeath();
        CheckGameClear();
    }

    private void CheckPlayerDeath()
    {
        if(isPlayerDeath == true)
        {
            OnPlayerDeath();
        }
    }

    private void CheckGameClear()
    {
        if (isGameClear == true)
        {
            OnGameClear();        }
    }

    public void OnPlayerDeath()
    {
        //プレイヤー死亡処理
        Debug.Log("<color=yellow>PLAYER DEATH!");

        _playerManager.PlayerDeath();
    }

    public void OnGameClear()
    {
        //ゲームクリア処理
        Debug.Log("<color=green>GAME CLEAR!");
    }

}
