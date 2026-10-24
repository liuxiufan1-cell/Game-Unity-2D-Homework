using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public static class CourseworkSceneBuilder
{
    private const string Root = "Assets/Game";
    private const string AnimationFolder = Root + "/Animations";
    private const string PrefabFolder = Root + "/Prefabs";
    private const string SceneFolder = Root + "/Scenes";
    private const string ScenePath = SceneFolder + "/2DGameCoursework.unity";
    private const string PlatformerSprites = "Assets/ThirdParty/Simple 2D Platformer BE2/Sprites/";
    private const string SpriteShapePrefabPath = "Assets/CourseSamples/TCA2026/Assets/Prefab/Sprite Shape Profile01.prefab";
    private const int GroundLayer = 6;
    private const int HazardLayer = 7;

    [MenuItem("Coursework/Regenerate Original Example Scene")]
    public static void BuildSubmissionScene()
    {
        if (!Application.isBatchMode && !EditorUtility.DisplayDialog(
            "Regenerate original example",
            "This replaces the authored level, prefabs and animations with the original generator layout. Use Build Windows Player to export the existing game.",
            "Replace assets", "Cancel"))
        {
            return;
        }

        EnsureFolders();
        ConfigureTagsAndLayers();

        Sprite[] playerSprites = LoadSprites(PlatformerSprites + "Player.png");
        Sprite[] enemySprites = LoadSprites(PlatformerSprites + "Enemy.png");
        Sprite[] coinSprites = LoadSprites(PlatformerSprites + "Coins.png");

        AnimatorController playerController = CreatePlayerAnimations(playerSprites);
        AnimatorController enemyController = CreateEnemyAnimations(enemySprites);
        AnimatorController coinController = CreateCoinAnimation(coinSprites);

        PlayerCharacter playerPrefab = CreatePlayerPrefab(playerSprites, playerController);
        WalkerEnemy walkerPrefab = CreateEnemyPrefab<WalkerEnemy>(
            "WalkerEnemy",
            new Color(0.2f, 0.95f, 0.85f),
            1.6f,
            7f,
            enemySprites,
            enemyController);
        JumpingEnemy jumpingPrefab = CreateEnemyPrefab<JumpingEnemy>(
            "JumpingEnemy",
            new Color(0.95f, 0.45f, 0.85f),
            2.2f,
            7f,
            enemySprites,
            enemyController);
        GameObject coinPrefab = CreateCoinPrefab(coinSprites, coinController);

        Scene scene = CreateLevelScene(playerPrefab, walkerPrefab, jumpingPrefab, coinPrefab);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ValidateGeneratedContent(scene);
        Debug.Log($"Coursework scene generated: {ScenePath}");
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets", "Game");
        EnsureFolder(Root, "Animations");
        EnsureFolder(Root, "Prefabs");
        EnsureFolder(Root, "Scenes");
        EnsureFolder(Root, "Editor");
        EnsureFolder(Root, "Scripts");
    }

    private static void EnsureFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static void ConfigureTagsAndLayers()
    {
        UnityEngine.Object[] tagAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (tagAssets.Length == 0)
        {
            throw new InvalidOperationException("TagManager.asset could not be loaded.");
        }

        SerializedObject tagManager = new(tagAssets[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");
        foreach (string tag in new[] { "Enemy", "Ground", "Hazard", "Finish", "DeathLine" })
        {
            bool exists = Enumerable.Range(0, tags.arraySize)
                .Any(index => tags.GetArrayElementAtIndex(index).stringValue == tag);
            if (!exists)
            {
                tags.InsertArrayElementAtIndex(tags.arraySize);
                tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            }
        }

        SerializedProperty layers = tagManager.FindProperty("layers");
        layers.GetArrayElementAtIndex(GroundLayer).stringValue = "Ground";
        layers.GetArrayElementAtIndex(HazardLayer).stringValue = "Hazard";
        tagManager.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Sprite[] LoadSprites(string path)
    {
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        if (sprites.Length == 0)
        {
            throw new InvalidOperationException($"No sprites found at {path}");
        }

        return sprites;
    }

    private static Sprite FindSprite(IEnumerable<Sprite> sprites, string name)
    {
        Sprite sprite = sprites.FirstOrDefault(candidate => candidate.name == name);
        return sprite != null ? sprite : throw new InvalidOperationException($"Sprite not found: {name}");
    }

    private static AnimatorController CreatePlayerAnimations(Sprite[] sprites)
    {
        AnimationClip idle = CreateSpriteClip(
            AnimationFolder + "/Player_Idle.anim",
            true,
            4f,
            FindSprite(sprites, "Idle 0"),
            FindSprite(sprites, "Idle 1"));
        AnimationClip run = CreateSpriteClip(
            AnimationFolder + "/Player_Run.anim",
            true,
            8f,
            FindSprite(sprites, "Run 0"),
            FindSprite(sprites, "Run 1"),
            FindSprite(sprites, "Run 2"),
            FindSprite(sprites, "Run 3"));
        AnimationClip jump = CreateSpriteClip(
            AnimationFolder + "/Player_Jump.anim",
            false,
            5f,
            FindSprite(sprites, "Jump 0"),
            FindSprite(sprites, "Jump 1"),
            FindSprite(sprites, "Jump 2"));
        AnimationClip death = CreateDeathClip(
            AnimationFolder + "/Player_Death.anim",
            FindSprite(sprites, "Idle 0"),
            0.7f);

        string path = AnimationFolder + "/PlayerAnimator.controller";
        DeleteAssetIfPresent(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Death", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle");
        AnimatorState runState = machine.AddState("Run");
        AnimatorState jumpState = machine.AddState("Jump");
        AnimatorState deathState = machine.AddState("Death");
        idleState.motion = idle;
        runState.motion = run;
        jumpState.motion = jump;
        deathState.motion = death;
        machine.defaultState = idleState;

        AddCondition(idleState.AddTransition(runState), AnimatorConditionMode.Greater, 0.1f, "Speed");
        AddCondition(runState.AddTransition(idleState), AnimatorConditionMode.Less, 0.1f, "Speed");
        AddCondition(idleState.AddTransition(jumpState), AnimatorConditionMode.IfNot, 0f, "Grounded");
        AddCondition(runState.AddTransition(jumpState), AnimatorConditionMode.IfNot, 0f, "Grounded");
        AnimatorStateTransition jumpToRun = jumpState.AddTransition(runState);
        AddCondition(jumpToRun, AnimatorConditionMode.If, 0f, "Grounded");
        jumpToRun.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        AnimatorStateTransition jumpToIdle = jumpState.AddTransition(idleState);
        AddCondition(jumpToIdle, AnimatorConditionMode.If, 0f, "Grounded");
        jumpToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
        AddCondition(machine.AddAnyStateTransition(deathState), AnimatorConditionMode.If, 0f, "Death");
        return controller;
    }

    private static AnimatorController CreateEnemyAnimations(Sprite[] sprites)
    {
        AnimationClip idle = CreateSpriteClip(
            AnimationFolder + "/Enemy_Idle.anim",
            true,
            6f,
            FindSprite(sprites, "Idle 0"),
            FindSprite(sprites, "Idle 1"),
            FindSprite(sprites, "Idle 2"),
            FindSprite(sprites, "Idle 3"));
        AnimationClip run = CreateSpriteClip(
            AnimationFolder + "/Enemy_Run.anim",
            true,
            8f,
            FindSprite(sprites, "Run 0"),
            FindSprite(sprites, "Run 1"),
            FindSprite(sprites, "Run 2"),
            FindSprite(sprites, "Run 3"));
        AnimationClip death = CreateDeathClip(
            AnimationFolder + "/Enemy_Death.anim",
            FindSprite(sprites, "Idle 0"),
            0.55f);

        string path = AnimationFolder + "/EnemyAnimator.controller";
        DeleteAssetIfPresent(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        controller.AddParameter("Moving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Death", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle");
        AnimatorState runState = machine.AddState("Run");
        AnimatorState deathState = machine.AddState("Death");
        idleState.motion = idle;
        runState.motion = run;
        deathState.motion = death;
        machine.defaultState = idleState;

        AddCondition(idleState.AddTransition(runState), AnimatorConditionMode.If, 0f, "Moving");
        AddCondition(runState.AddTransition(idleState), AnimatorConditionMode.IfNot, 0f, "Moving");
        AddCondition(machine.AddAnyStateTransition(deathState), AnimatorConditionMode.If, 0f, "Death");
        return controller;
    }

    private static AnimatorController CreateCoinAnimation(Sprite[] sprites)
    {
        AnimationClip spin = CreateSpriteClip(
            AnimationFolder + "/Coin_Spin.anim",
            true,
            8f,
            FindSprite(sprites, "Gold 0"),
            FindSprite(sprites, "Gold 1"),
            FindSprite(sprites, "Gold 2"),
            FindSprite(sprites, "Gold 3"));

        string path = AnimationFolder + "/CoinAnimator.controller";
        DeleteAssetIfPresent(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        AnimatorState spinState = controller.layers[0].stateMachine.AddState("Spin");
        spinState.motion = spin;
        controller.layers[0].stateMachine.defaultState = spinState;
        return controller;
    }

    private static AnimationClip CreateSpriteClip(string path, bool loop, float frameRate, params Sprite[] frames)
    {
        DeleteAssetIfPresent(path);
        AnimationClip clip = new() { frameRate = frameRate, name = System.IO.Path.GetFileNameWithoutExtension(path) };
        ObjectReferenceKeyframe[] keys = frames.Select((sprite, index) => new ObjectReferenceKeyframe
        {
            time = index / frameRate,
            value = sprite
        }).ToArray();
        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimationClip CreateDeathClip(string path, Sprite sprite, float duration)
    {
        DeleteAssetIfPresent(path);
        AnimationClip clip = new() { frameRate = 30f, name = System.IO.Path.GetFileNameWithoutExtension(path) };
        AnimationUtility.SetObjectReferenceCurve(
            clip,
            EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite"),
            new[] { new ObjectReferenceKeyframe { time = 0f, value = sprite } });
        SetCurve(clip, typeof(Transform), "m_LocalScale.x", new Keyframe(0f, 1f), new Keyframe(duration * 0.55f, 1.25f), new Keyframe(duration, 0.05f));
        SetCurve(clip, typeof(Transform), "m_LocalScale.y", new Keyframe(0f, 1f), new Keyframe(duration * 0.55f, 0.3f), new Keyframe(duration, 0.05f));
        SetCurve(clip, typeof(Transform), "localEulerAnglesRaw.z", new Keyframe(0f, 0f), new Keyframe(duration, 360f));
        SetCurve(clip, typeof(SpriteRenderer), "m_Color.a", new Keyframe(0f, 1f), new Keyframe(duration * 0.7f, 1f), new Keyframe(duration, 0f));
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void SetCurve(AnimationClip clip, Type type, string property, params Keyframe[] keys)
    {
        AnimationUtility.SetEditorCurve(
            clip,
            EditorCurveBinding.FloatCurve(string.Empty, type, property),
            new AnimationCurve(keys));
    }

    private static void AddCondition(AnimatorStateTransition transition, AnimatorConditionMode mode, float threshold, string parameter)
    {
        transition.hasExitTime = false;
        transition.duration = 0.04f;
        transition.canTransitionToSelf = false;
        transition.AddCondition(mode, threshold, parameter);
    }

    private static PlayerCharacter CreatePlayerPrefab(Sprite[] sprites, RuntimeAnimatorController controller)
    {
        GameObject root = new("Player");
        root.tag = "Player";

        Rigidbody2D body = root.AddComponent<Rigidbody2D>();
        body.gravityScale = 3f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CapsuleCollider2D collider = root.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.72f, 1.15f);
        collider.offset = new Vector2(0f, 0.02f);

        GameObject visual = CreateVisual("Visual", root.transform, FindSprite(sprites, "Idle 0"), controller, Color.white, 10);
        visual.transform.localScale = Vector3.one * 1.35f;
        Transform groundCheck = CreateChild("GroundCheck", root.transform, new Vector3(0f, -0.62f, 0f)).transform;

        PlayerCharacter player = root.AddComponent<PlayerCharacter>();
        SerializedObject serialized = new(player);
        SetObject(serialized, "groundCheck", groundCheck);
        SetObject(serialized, "spriteRenderer", visual.GetComponent<SpriteRenderer>());
        SetObject(serialized, "animator", visual.GetComponent<Animator>());
        SetObject(serialized, "bodyCollider", collider);
        serialized.FindProperty("groundMask").intValue = 1 << GroundLayer;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        string path = PrefabFolder + "/Player.prefab";
        DeleteAssetIfPresent(path);
        PrefabUtility.SaveAsPrefabAsset(root, path);
        UnityEngine.Object.DestroyImmediate(root);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        GameObject saved = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        PlayerCharacter savedPlayer = saved == null ? null : saved.GetComponent<PlayerCharacter>();
        return savedPlayer != null ? savedPlayer : throw new InvalidOperationException("Saved Player prefab is missing PlayerCharacter.");
    }

    private static T CreateEnemyPrefab<T>(string name, Color tint, float speed, float patrolDistance, Sprite[] sprites, RuntimeAnimatorController controller)
        where T : EnemyBase
    {
        GameObject root = new(name) { tag = "Enemy" };
        Rigidbody2D body = root.AddComponent<Rigidbody2D>();
        body.gravityScale = 2.5f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.82f, 0.82f);

        GameObject visual = CreateVisual("Visual", root.transform, FindSprite(sprites, "Idle 0"), controller, tint, 9);
        visual.transform.localScale = typeof(T) == typeof(JumpingEnemy)
            ? new Vector3(1.25f, 1.1f, 1f)
            : Vector3.one * 1.15f;
        Transform groundCheck = CreateChild("GroundCheck", root.transform, new Vector3(0f, -0.5f, 0f)).transform;

        T enemy = root.AddComponent<T>();
        SerializedObject serialized = new(enemy);
        serialized.FindProperty("moveSpeed").floatValue = speed;
        serialized.FindProperty("patrolDistance").floatValue = patrolDistance;
        serialized.FindProperty("groundMask").intValue = 1 << GroundLayer;
        SetObject(serialized, "groundCheck", groundCheck);
        SetObject(serialized, "animator", visual.GetComponent<Animator>());
        SetObject(serialized, "spriteRenderer", visual.GetComponent<SpriteRenderer>());
        SetObject(serialized, "bodyCollider", collider);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        string path = PrefabFolder + "/" + name + ".prefab";
        DeleteAssetIfPresent(path);
        PrefabUtility.SaveAsPrefabAsset(root, path);
        UnityEngine.Object.DestroyImmediate(root);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        GameObject saved = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        T savedEnemy = saved == null ? null : saved.GetComponent<T>();
        return savedEnemy != null ? savedEnemy : throw new InvalidOperationException($"Saved {name} prefab is missing {typeof(T).Name}.");
    }

    private static GameObject CreateCoinPrefab(Sprite[] sprites, RuntimeAnimatorController controller)
    {
        GameObject root = new("Coin");
        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sprite = FindSprite(sprites, "Gold 0");
        renderer.sortingOrder = 8;
        Animator animator = root.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        CircleCollider2D collider = root.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.4f;
        root.AddComponent<CoinCollectible>();

        string path = PrefabFolder + "/Coin.prefab";
        DeleteAssetIfPresent(path);
        PrefabUtility.SaveAsPrefabAsset(root, path);
        UnityEngine.Object.DestroyImmediate(root);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        GameObject saved = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return saved != null ? saved : throw new InvalidOperationException("Saved Coin prefab could not be loaded.");
    }

    private static GameObject CreateVisual(string name, Transform parent, Sprite sprite, RuntimeAnimatorController controller, Color tint, int order)
    {
        GameObject visual = CreateChild(name, parent, Vector3.zero);
        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = tint;
        renderer.sortingOrder = order;
        Animator animator = visual.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        return visual;
    }

    private static Scene CreateLevelScene(PlayerCharacter playerPrefab, WalkerEnemy walkerPrefab, JumpingEnemy jumpingPrefab, GameObject coinPrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject levelRoot = new("CourseworkLevel");
        GameObject backgrounds = CreateChild("SeasonalBackgrounds", levelRoot.transform, Vector3.zero);
        GameObject terrain = CreateChild("Terrain", levelRoot.transform, Vector3.zero);
        GameObject gameplay = CreateChild("Gameplay", levelRoot.transform, Vector3.zero);
        GameObject spawners = CreateChild("CharacterSpawners", gameplay.transform, Vector3.zero);

        CameraFollow2D cameraFollow = CreateCamera(levelRoot.transform);
        CreateGlobalLight(levelRoot.transform);
        CreateSeasonalBackgrounds(backgrounds.transform);
        CreateTilemapTerrain(terrain.transform);
        CreateSpriteShapeTerrain(scene, terrain.transform);
        CreateHazards(gameplay.transform);
        CreateCoins(scene, gameplay.transform, coinPrefab);
        new GameObject("ScoreCounter", typeof(ScoreCounter)).transform.SetParent(gameplay.transform, false);
        CreateGoal(gameplay.transform);
        CreatePlayerSpawner(spawners.transform, playerPrefab, cameraFollow);
        CreateEnemySpawners(spawners.transform, walkerPrefab, jumpingPrefab);
        return scene;
    }

    private static CameraFollow2D CreateCamera(Transform parent)
    {
        GameObject cameraObject = new("Main Camera") { tag = "MainCamera" };
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.position = new Vector3(8.9f, 2f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.4f, 0.7f, 0.9f);
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<UniversalAdditionalCameraData>();
        return cameraObject.AddComponent<CameraFollow2D>();
    }

    private static void CreateGlobalLight(Transform parent)
    {
        GameObject lightObject = new("Global Light 2D");
        lightObject.transform.SetParent(parent, false);
        Light2D light = lightObject.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Global;
        light.intensity = 1f;
    }

    private static void CreateSeasonalBackgrounds(Transform parent)
    {
        (string name, string path, float x)[] seasons =
        {
            ("Spring_Mountain", "Assets/ThirdParty/Free 2D Cartoon Parallax Background/FullBG/1_Mountain.png", 18f),
            ("Summer_Desert", "Assets/ThirdParty/Free 2D Cartoon Parallax Background/FullBG/2_Desert.png", 54f),
            ("Autumn_Graveyard", "Assets/ThirdParty/Free 2D Cartoon Parallax Background/FullBG/3_Graveyard.png", 90f),
            ("Winter_Snow", "Assets/ThirdParty/Free 2D Cartoon Parallax Background/FullBG/4_Snow.png", 126f)
        };

        foreach ((string name, string path, float x) in seasons)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                throw new InvalidOperationException($"Background sprite not found: {path}");
            }

            GameObject background = CreateChild(name, parent, new Vector3(x, 2f, 5f));
            SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = -100;
            float scale = 38f / sprite.bounds.size.x;
            background.transform.localScale = Vector3.one * scale;
        }
    }

    private static void CreateTilemapTerrain(Transform parent)
    {
        GameObject gridObject = CreateChild("Grid", parent, Vector3.zero);
        Grid grid = gridObject.AddComponent<Grid>();
        grid.cellSize = Vector3.one;

        GameObject tilemapObject = CreateChild("GroundTilemap", gridObject.transform, Vector3.zero);
        tilemapObject.layer = GroundLayer;
        tilemapObject.tag = "Ground";
        Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapObject.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = 0;

        TileBase top = LoadTile("Top");
        TileBase middle = LoadTile("Middle");
        TileBase bottom = LoadTile("Bottom");
        for (int x = 0; x < 144; x++)
        {
            if (!HasTileGroundAt(x))
            {
                continue;
            }

            int surface = SurfaceHeightAt(x);
            for (int y = -5; y <= surface; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), y == surface ? top : y == -5 ? bottom : middle);
            }
        }

        AddPlatform(tilemap, top, 5, 9, 3);
        AddPlatform(tilemap, top, 42, 46, 3);
        AddPlatform(tilemap, top, 79, 83, 3);
        AddPlatform(tilemap, top, 116, 120, 3);

        Rigidbody2D body = tilemapObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Static;
        TilemapCollider2D tileCollider = tilemapObject.AddComponent<TilemapCollider2D>();
        CompositeCollider2D composite = tilemapObject.AddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
#pragma warning disable CS0618
        tileCollider.usedByComposite = true;
#pragma warning restore CS0618
        tilemap.CompressBounds();
    }

    private static bool HasTileGroundAt(int x)
    {
        return x <= 19 ||
               (x >= 37 && x <= 56) ||
               (x >= 74 && x <= 93) ||
               (x >= 111 && x <= 124) ||
               x >= 142;
    }

    private static int SurfaceHeightAt(int x)
    {
        if (x is >= 10 and <= 14 || x is >= 47 and <= 51 || x is >= 84 and <= 88 || x is >= 117 and <= 121)
        {
            return 1;
        }

        return 0;
    }

    private static void AddPlatform(Tilemap tilemap, TileBase tile, int fromX, int toX, int y)
    {
        for (int x = fromX; x <= toX; x++)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }
    }

    private static TileBase LoadTile(string name)
    {
        string path = $"Assets/CourseSamples/TCA2026/Assets/TilePalette/{name}.asset";
        TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
        return tile != null ? tile : throw new InvalidOperationException($"Tile not found: {path}");
    }

    private static void CreateSpriteShapeTerrain(Scene scene, Transform parent)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(SpriteShapePrefabPath);
        if (source == null)
        {
            throw new InvalidOperationException("Sprite Shape prefab is missing.");
        }

        float[] positions = { 28.5f, 65.5f, 102.5f, 133.5f };
        string[] names = { "SpringHill", "SummerDune", "AutumnHill", "WinterHill" };
        Vector2[][] seasonCurves =
        {
            new[]
            {
                new Vector2(-8.5f, 1f), new Vector2(-5.5f, 1.1f), new Vector2(-2.5f, 1.8f),
                new Vector2(1.5f, 1.35f), new Vector2(5.5f, 1.05f), new Vector2(8.5f, 1f)
            },
            new[]
            {
                new Vector2(-8.5f, 1f), new Vector2(-5.5f, 1.05f), new Vector2(-2.5f, 1.45f),
                new Vector2(1f, 2.05f), new Vector2(4.5f, 1.45f), new Vector2(7f, 1.08f),
                new Vector2(8.5f, 1f)
            },
            new[]
            {
                new Vector2(-8.5f, 1f), new Vector2(-5.5f, 1.28f), new Vector2(-3f, 1.02f),
                new Vector2(0f, 1.55f), new Vector2(3f, 1.02f), new Vector2(6f, 1.28f),
                new Vector2(8.5f, 1f)
            },
            new[]
            {
                new Vector2(-8.5f, 1f), new Vector2(-5.5f, 1.1f), new Vector2(-3f, 1.75f),
                new Vector2(0f, 2.65f), new Vector2(3f, 1.75f), new Vector2(6f, 1.1f),
                new Vector2(8.5f, 1f)
            }
        };

        for (int index = 0; index < positions.Length; index++)
        {
            GameObject shape = (GameObject)PrefabUtility.InstantiatePrefab(source, scene);
            PrefabUtility.UnpackPrefabInstance(shape, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            shape.name = names[index] + "_SpriteShape";
            shape.transform.SetParent(parent, false);
            shape.transform.position = new Vector3(positions[index], 0f, 0f);
            shape.transform.localScale = Vector3.one;
            shape.layer = GroundLayer;
            shape.tag = "Ground";

            SpriteShapeController controller = shape.GetComponent<SpriteShapeController>();
            ConfigureSmoothSpline(controller, seasonCurves[index]);
            SpriteShapeRenderer renderer = shape.GetComponent<SpriteShapeRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 1;
            }
        }
    }

    private static void ConfigureSmoothSpline(SpriteShapeController controller, IReadOnlyList<Vector2> points)
    {
        if (controller == null)
        {
            throw new InvalidOperationException("Sprite Shape Controller is missing.");
        }

        Spline spline = controller.spline;
        spline.Clear();
        spline.isOpenEnded = true;
        for (int index = 0; index < points.Count; index++)
        {
            spline.InsertPointAt(index, points[index]);
        }

        for (int index = 0; index < points.Count; index++)
        {
            Vector2 previous = points[Mathf.Max(0, index - 1)];
            Vector2 next = points[Mathf.Min(points.Count - 1, index + 1)];
            Vector2 tangent = (next - previous) * 0.18f;
            spline.SetTangentMode(index, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(index, -tangent);
            spline.SetRightTangent(index, tangent);
            spline.SetHeight(index, 4.5f);
        }

        controller.colliderOffset = 0f;
        controller.splineDetail = 16;
        controller.colliderDetail = 16;
        controller.RefreshSpriteShape();
        controller.BakeMesh().Complete();
        controller.BakeCollider();
        EditorUtility.SetDirty(controller);
    }

    private static void CreateHazards(Transform parent)
    {
        Sprite[] objectSprites = LoadSprites(PlatformerSprites + "Objects.png");
        Sprite spikeSprite = FindSprite(objectSprites, "Spike");
        foreach (float x in new[] { 17f, 18f, 53f, 54f, 90f, 91f, 122f, 123f })
        {
            GameObject spike = CreateChild($"Spike_{x:000}", parent, new Vector3(x, SurfaceHeightAt(Mathf.RoundToInt(x)) + 1f, 0f));
            spike.layer = HazardLayer;
            spike.tag = "Hazard";
            SpriteRenderer renderer = spike.AddComponent<SpriteRenderer>();
            renderer.sprite = spikeSprite;
            renderer.sortingOrder = 7;
            BoxCollider2D collider = spike.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.75f, 0.65f);
            collider.offset = new Vector2(0f, -0.15f);
            spike.AddComponent<KillZone>();
        }

        GameObject deathLine = CreateChild("DeathLine", parent, new Vector3(72f, -8f, 0f));
        deathLine.layer = HazardLayer;
        deathLine.tag = "DeathLine";
        BoxCollider2D lineCollider = deathLine.AddComponent<BoxCollider2D>();
        lineCollider.isTrigger = true;
        lineCollider.size = new Vector2(160f, 1f);
        deathLine.AddComponent<KillZone>();
    }

    private static void CreateCoins(Scene scene, Transform parent, GameObject prefab)
    {
        Vector3[] positions =
        {
            new(3f, 2.2f), new(6f, 4.4f), new(9f, 4.4f), new(13f, 3.2f),
            new(22f, 2.4f), new(26f, 3.1f), new(30f, 2.8f), new(34f, 2.2f),
            new(40f, 2.2f), new(43f, 4.4f), new(46f, 4.4f), new(50f, 3.2f),
            new(59f, 2.3f), new(63f, 3.1f), new(67f, 3.6f), new(71f, 2.3f),
            new(77f, 2.2f), new(80f, 4.4f), new(83f, 4.4f), new(87f, 3.2f),
            new(96f, 2.4f), new(100f, 2.5f), new(104f, 3f), new(108f, 2.4f),
            new(114f, 2.2f), new(117f, 4.4f), new(120f, 4.4f), new(123f, 3.2f),
            new(127f, 2.5f), new(131f, 3.3f), new(135f, 4f), new(139f, 2.7f)
        };

        for (int index = 0; index < positions.Length; index++)
        {
            GameObject coin = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            coin.name = $"Coin_{index + 1:00}";
            coin.transform.SetParent(parent, false);
            coin.transform.position = positions[index];
        }
    }

    private static void CreateGoal(Transform parent)
    {
        Sprite[] objectSprites = LoadSprites(PlatformerSprites + "Objects.png");
        GameObject goal = CreateChild("Finish", parent, new Vector3(143f, 1.6f, 0f));
        goal.tag = "Finish";
        goal.transform.localScale = Vector3.one * 2f;
        SpriteRenderer renderer = goal.AddComponent<SpriteRenderer>();
        renderer.sprite = FindSprite(objectSprites, "Finish");
        renderer.sortingOrder = 6;
        BoxCollider2D collider = goal.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.45f, 0.95f);
        goal.AddComponent<LevelGoal>();
    }

    private static void CreatePlayerSpawner(Transform parent, PlayerCharacter prefab, CameraFollow2D cameraFollow)
    {
        GameObject spawnPoint = CreateChild("PlayerSpawnPoint", parent, new Vector3(2f, 2f, 0f));
        GameObject spawnerObject = CreateChild("PlayerSpawner", parent, Vector3.zero);
        PlayerSpawner spawner = spawnerObject.AddComponent<PlayerSpawner>();
        spawner.Configure(prefab, spawnPoint.transform, cameraFollow);
        EditorUtility.SetDirty(spawner);
    }

    private static void CreateEnemySpawners(Transform parent, WalkerEnemy walker, JumpingEnemy jumper)
    {
        CreateEnemySpawner(parent, "WalkerSpawn_01", new Vector3(11f, 2.3f), walker);
        CreateEnemySpawner(parent, "WalkerSpawn_02", new Vector3(42f, 2.3f), walker);
        CreateEnemySpawner(parent, "WalkerSpawn_03", new Vector3(78f, 2.3f), walker);
        CreateEnemySpawner(parent, "WalkerSpawn_04", new Vector3(114f, 2.3f), walker);
        CreateEnemySpawner(parent, "JumpingSpawn_01", new Vector3(49f, 3.3f), jumper);
        CreateEnemySpawner(parent, "JumpingSpawn_02", new Vector3(84f, 3.3f), jumper);
        CreateEnemySpawner(parent, "JumpingSpawn_03", new Vector3(119f, 3.3f), jumper);
    }

    private static void CreateEnemySpawner(Transform parent, string name, Vector3 position, EnemyBase prefab)
    {
        GameObject spawnerObject = CreateChild(name, parent, position);
        EnemySpawner spawner = spawnerObject.AddComponent<EnemySpawner>();
        spawner.Configure(prefab);
        EditorUtility.SetDirty(spawner);
    }

    private static GameObject CreateChild(string name, Transform parent, Vector3 localPosition)
    {
        GameObject child = new(name);
        child.transform.SetParent(parent, false);
        child.transform.localPosition = localPosition;
        return child;
    }

    private static void SetObject(SerializedObject serialized, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException($"Serialized property not found: {propertyName}");
        }

        property.objectReferenceValue = value;
    }

    private static void DeleteAssetIfPresent(string path)
    {
        if (AssetDatabase.LoadMainAssetAtPath(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
    }

    private static void ValidateGeneratedContent(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            throw new InvalidOperationException("Generated scene is not loaded.");
        }

        int coinCount = UnityEngine.Object.FindObjectsByType<CoinCollectible>(FindObjectsSortMode.None).Length;
        int shapeCount = UnityEngine.Object.FindObjectsByType<SpriteShapeController>(FindObjectsSortMode.None).Length;
        int spawnerCount = UnityEngine.Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None).Length;
        if (coinCount != 32 || shapeCount != 4 || spawnerCount != 7)
        {
            throw new InvalidOperationException($"Generated content count mismatch. Coins={coinCount}, SpriteShapes={shapeCount}, EnemySpawners={spawnerCount}");
        }

        if (UnityEngine.Object.FindFirstObjectByType<PlayerCharacter>() != null)
        {
            throw new InvalidOperationException("Player must be instantiated at runtime, not pre-placed in the scene.");
        }

        string[] states = { "Idle", "Run", "Jump", "Death" };
        AnimatorController player = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimationFolder + "/PlayerAnimator.controller");
        string[] actualStates = player.layers[0].stateMachine.states.Select(item => item.state.name).ToArray();
        if (states.Any(state => !actualStates.Contains(state)))
        {
            throw new InvalidOperationException("Player Animator is missing a required state.");
        }

        Debug.Log("Coursework validation passed: 32 coins, four Sprite Shapes, seven enemy spawners, runtime-only characters, and all Player animation states.");
    }
}
