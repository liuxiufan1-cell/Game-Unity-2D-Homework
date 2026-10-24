using System.Collections;
using UnityEngine;

public sealed class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow2D cameraFollow;
    [SerializeField, Min(0f)] private float respawnDelay = 0.6f;

    public PlayerCharacter CurrentPlayer { get; private set; }

    public void Configure(PlayerCharacter prefab, Transform point, CameraFollow2D followCamera)
    {
        playerPrefab = prefab;
        spawnPoint = point;
        cameraFollow = followCamera;
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        CurrentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        CurrentPlayer.Died += HandlePlayerDied;
        cameraFollow.SetTarget(CurrentPlayer.transform);
        Debug.Log("Player instantiated.");
    }

    private void HandlePlayerDied(PlayerCharacter player)
    {
        player.Died -= HandlePlayerDied;
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnPlayer();
    }
}
