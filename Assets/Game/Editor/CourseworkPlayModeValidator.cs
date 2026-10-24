using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

[InitializeOnLoad]
public static class CourseworkPlayModeValidator
{
    private const string ScenePath = "Assets/Game/Scenes/2DGameCoursework.unity";
    private const string PendingKey = "Coursework.PlayModeValidation.Pending";
    private const string ResultKey = "Coursework.PlayModeValidation.Result";
    private const string SavedPauseKey = "Coursework.PlayModeValidation.ErrorPause";
    private const string RestorePauseKey = "Coursework.PlayModeValidation.RestoreErrorPause";

    private static int phase;
    private static double phaseStarted;
    private static double validationStarted;
    private static PlayerCharacter originalPlayer;
    private static EnemyBase originalEnemy;

    static CourseworkPlayModeValidator()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Unity is already entering or in Play Mode.");
        }

        EditorSceneManager.OpenScene(ScenePath);
        EditorApplication.isPaused = false;
        SessionState.SetBool(PendingKey, true);
        SessionState.SetInt(ResultKey, -1);
        if (Application.isBatchMode)
        {
            Type logEntries = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntries", true);
            int flags = (int)logEntries.GetProperty("consoleFlags", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            SessionState.SetBool(SavedPauseKey, (flags & 4) != 0);
            SessionState.SetBool(RestorePauseKey, true);
            SetErrorPause(false);
            Debug.Log("COURSEWORK_SMOKE: Error Pause temporarily disabled for batch validation; error logs are retained.");
        }
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!SessionState.GetBool(PendingKey, false))
        {
            return;
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Application.runInBackground = true;
            phase = 0;
            phaseStarted = Time.timeAsDouble;
            validationStarted = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorApplication.update -= Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            int result = SessionState.GetInt(ResultKey, 1);
            SessionState.SetBool(PendingKey, false);
            if (SessionState.GetBool(RestorePauseKey, false))
            {
                SetErrorPause(SessionState.GetBool(SavedPauseKey, true));
                SessionState.SetBool(RestorePauseKey, false);
            }
            if (result == 0)
            {
                Debug.Log("COURSEWORK_PLAYMODE_SMOKE_TEST_PASSED");
            }
            EditorApplication.Exit(result);
        }
    }

    private static void SetErrorPause(bool enabled)
    {
        // Unity has no public API for this Console preference. Restore it after the isolated run.
        Type logEntries = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntries", true);
        logEntries.GetMethod("SetConsoleFlag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] { 4, enabled });
    }

    private static void Tick()
    {
        try
        {
            // In batch mode an editor update is not necessarily a completed game frame.
            EditorApplication.QueuePlayerLoopUpdate();
            Require(EditorApplication.timeSinceStartup - validationStarted < 90d,
                $"Play Mode validation timed out at phase {phase}, frame {Time.frameCount}, game time {Time.timeAsDouble}.");
            double elapsed = Time.timeAsDouble - phaseStarted;
            if (phase == 0 && elapsed >= 1.6d)
            {
                Debug.Log($"COURSEWORK_SMOKE_PHASE_STARTUP: frame={Time.frameCount}, gameTime={Time.timeAsDouble}");
                ValidateStartupAndTriggerActions();
                phase = 1;
                phaseStarted = Time.timeAsDouble;
            }
            else if (phase == 1 && elapsed >= 0.9d)
            {
                Debug.Log($"COURSEWORK_SMOKE_PHASE_DESTRUCTION: frame={Time.frameCount}, gameTime={Time.timeAsDouble}");
                ValidateDestruction();
                phase = 2;
                phaseStarted = Time.timeAsDouble;
            }
            else if (phase == 2 && elapsed >= 0.8d)
            {
                ValidateRespawnAndFinish();
                Finish(0);
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish(1);
        }
    }

    private static void ValidateStartupAndTriggerActions()
    {
        PlayerSpawner playerSpawner = UnityEngine.Object.FindFirstObjectByType<PlayerSpawner>();
        Require(playerSpawner != null, "PlayerSpawner is missing.");
        Require(playerSpawner.CurrentPlayer != null, "Player was not instantiated at runtime.");
        originalPlayer = playerSpawner.CurrentPlayer;

        EnemySpawner[] enemySpawners = UnityEngine.Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        // The authored scene includes JumpingSpawn_04 in addition to the original generator layout.
        Require(enemySpawners.Length == 8, $"Expected 8 enemy spawners, found {enemySpawners.Length}.");
        Require(enemySpawners.All(spawner => spawner.SpawnedEnemy != null), "An enemy was not instantiated.");
        Require(enemySpawners.Count(spawner => spawner.SpawnedEnemy is WalkerEnemy) == 4, "WalkerEnemy count mismatch.");
        Require(enemySpawners.Count(spawner => spawner.SpawnedEnemy is JumpingEnemy) == 4, "JumpingEnemy count mismatch.");

        Require(UnityEngine.Object.FindObjectsByType<CoinCollectible>(FindObjectsSortMode.None).Length == 32, "Coin count mismatch.");
        Require(UnityEngine.Object.FindObjectsByType<SpriteShapeController>(FindObjectsSortMode.None).Length == 4, "Sprite Shape count mismatch.");
        Require(UnityEngine.Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None).Length == 1, "Tilemap count mismatch.");
        Require(UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).Length == 0, "UI Canvas must not exist.");
        Require(ScoreCounter.Instance != null && ScoreCounter.Instance.Score == 0, "Initial score must be zero.");

        CoinCollectible coin = UnityEngine.Object.FindFirstObjectByType<CoinCollectible>();
        coin.SendMessage("OnTriggerEnter2D", originalPlayer.GetComponent<Collider2D>(), SendMessageOptions.RequireReceiver);

        originalEnemy = enemySpawners[0].SpawnedEnemy;
        originalEnemy.Die();
        originalPlayer.Kill();
    }

    private static void ValidateDestruction()
    {
        Require(ScoreCounter.Instance.Score == 10, $"Coin score expected 10, found {ScoreCounter.Instance.Score}.");
        Require(UnityEngine.Object.FindObjectsByType<CoinCollectible>(FindObjectsSortMode.None).Length == 31, "Collected coin was not destroyed.");
        Require(originalEnemy == null, "Enemy was not destroyed after Death animation.");
        Require(originalPlayer == null, "Player was not destroyed after Death animation.");
    }

    private static void ValidateRespawnAndFinish()
    {
        PlayerSpawner playerSpawner = UnityEngine.Object.FindFirstObjectByType<PlayerSpawner>();
        Require(playerSpawner.CurrentPlayer != null, "Player did not respawn through Instantiate.");
        Require(playerSpawner.CurrentPlayer != originalPlayer, "Respawn did not create a new Player instance.");

        LevelGoal goal = UnityEngine.Object.FindFirstObjectByType<LevelGoal>();
        Require(goal != null, "Level goal is missing.");
        goal.SendMessage("OnTriggerEnter2D", playerSpawner.CurrentPlayer.GetComponent<Collider2D>(), SendMessageOptions.RequireReceiver);
        Require(goal.IsCompleted, "Level goal did not enter the completed state.");
        Require(!playerSpawner.CurrentPlayer.IsDead, "Player was incorrectly killed or locked at the goal.");
        Require(ScoreCounter.Instance.Score == 10, "Final score changed unexpectedly.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void Finish(int result)
    {
        EditorApplication.update -= Tick;
        SessionState.SetInt(ResultKey, result);
        EditorApplication.isPlaying = false;
    }
}
