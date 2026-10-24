using UnityEngine;
using System.Collections;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    public GameManager _gameManager;
    public EnemyManager _enemyManager;
    public CameraManager _cameraManager;

    public PlayerGeneratorManager _generator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
        _cameraManager = FindAnyObjectByType<CameraManager>();

        _generator = FindAnyObjectByType<PlayerGeneratorManager>();

        _generator.PlayerRespawnRequest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        ActiveOffPlayerCamera();
    }

    public void PlayerDeath()
    {
        //プレイヤーのリスポーン
        _generator.PlayerRespawnRequest();
    }

    public void ActiveOnPlayerCamera( GameObject camera)
    {
        _cameraManager.GetPlayerCameraObject(camera);
    }

    public void ActiveOffPlayerCamera()
    {
        _cameraManager.GetPlayerCameraObject(null);
    }
}
