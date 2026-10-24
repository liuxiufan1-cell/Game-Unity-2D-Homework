using System;
using System.Linq;
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

    private static int phase;
    private static double phaseStarted;
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
        SessionState.SetBool(PendingKey, true);
        SessionState.SetInt(ResultKey, -1);
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
            phase = 0;
            phaseStarted = EditorApplication.timeSinceStartup;
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
            if (result == 0)
            {
                Debug.Log("COURSEWORK_PLAYMODE_SMOKE_TEST_PASSED");
            }
            EditorApplication.Exit(result);
        }
    }

    private static void Tick()
    {
        try
        {
            double elapsed = EditorApplication.timeSinceStartup - phaseStarted;
            if (phase == 0 && elapsed >= 1.6d)
            {
                ValidateStartupAndTriggerActions();
                phase = 1;
                phaseStarted = EditorApplication.timeSinceStartup;
            }
            else if (phase == 1 && elapsed >= 0.9d)
            {
                ValidateDestruction();
                phase = 2;
                phaseStarted = EditorApplication.timeSinceStartup;
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
