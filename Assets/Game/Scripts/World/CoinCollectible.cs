using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class CoinCollectible : MonoBehaviour
{
    [SerializeField, Min(1)] private int points = 10;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.TryGetComponent(out PlayerCharacter player))
        {
            return;
        }

        collected = true;
        ScoreCounter.Instance?.Add(points);
        Destroy(gameObject);
    }
}
