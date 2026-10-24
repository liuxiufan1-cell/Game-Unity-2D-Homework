using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyPrefab;

    public EnemyBase SpawnedEnemy { get; private set; }

    public void Configure(EnemyBase prefab)
    {
        enemyPrefab = prefab;
    }

    private void Start()
    {
        SpawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Debug.Log($"Enemy instantiated: {enemyPrefab.name}");
    }
}
