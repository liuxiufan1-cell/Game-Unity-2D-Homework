using UnityEngine;

//プレイヤーの登場を処理するクラス
public class PlayerSpawn : MonoBehaviour
{
    public GameObject _player;
    public bool _isSpawn = false;

    private PlayerManager _playerManager;
    private PlayerGeneratorManager _generator;

    private void Awake()
    {
        _isSpawn = false;
    }

    private void Start()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _generator = FindAnyObjectByType<PlayerGeneratorManager>();

        _generator._playerSpawn = this;
    }

    void Update()
    {
        if (_isSpawn == true)
        {
            OnPlayerSpawn();

            _isSpawn = false;
        }
    }

    private void OnPlayerSpawn()
    {
        Debug.Log("Playerを生みます。");

        Vector3 spawnPosition = this.transform.position;

        //Enemyを1つ生成する。
        Instantiate(_player,
                        spawnPosition,
                        Quaternion.identity);

    }
}
