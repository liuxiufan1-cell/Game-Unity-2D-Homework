using UnityEngine;

public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        Instance = this;
        Score = 0;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Add(int points)
    {
        Score += points;
        Debug.Log($"Coin collected. Score: {Score}");
    }
}
