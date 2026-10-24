using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class LevelGoal : MonoBehaviour
{
    private bool completed;
    private int finalScore;
    private GUIStyle titleStyle;
    private GUIStyle scoreStyle;

    public bool IsCompleted => completed;
    public int FinalScore => finalScore;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed || !other.TryGetComponent(out PlayerCharacter player))
        {
            return;
        }

        completed = true;
        finalScore = ScoreCounter.Instance == null ? 0 : ScoreCounter.Instance.Score;
        Debug.Log($"GAME CLEAR! Final Score: {finalScore}");
    }

    private void OnGUI()
    {
        if (!completed)
        {
            return;
        }

        titleStyle ??= new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 48,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        scoreStyle ??= new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 26,
            normal = { textColor = new Color(1f, 0.85f, 0.2f) }
        };

        float panelWidth = Mathf.Min(560f, Screen.width * 0.72f);
        float panelHeight = 190f;
        Rect panel = new(
            (Screen.width - panelWidth) * 0.5f,
            (Screen.height - panelHeight) * 0.5f,
            panelWidth,
            panelHeight);

        Color previousColor = GUI.color;
        GUI.color = new Color(0.04f, 0.08f, 0.12f, 0.88f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;
        GUI.Label(new Rect(panel.x, panel.y + 24f, panel.width, 72f), "GAME CLEAR!", titleStyle);
        GUI.Label(new Rect(panel.x, panel.y + 102f, panel.width, 48f), $"FINAL SCORE: {finalScore}", scoreStyle);
    }
}
