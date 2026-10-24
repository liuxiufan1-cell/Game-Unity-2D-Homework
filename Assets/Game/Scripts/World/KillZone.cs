using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerCharacter player))
        {
            player.Kill();
        }
        else if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.Die();
        }
    }
}
