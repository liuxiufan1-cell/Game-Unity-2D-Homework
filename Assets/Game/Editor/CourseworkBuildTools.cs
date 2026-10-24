using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CourseworkBuildTools
{
    public const string MainScene = "Assets/Game/Scenes/2DGameCoursework.unity";

    [MenuItem("Coursework/Validate Project Assets")]
    public static void ValidateAssets()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
        Require(scenes.Length == 1 && scenes[0].path == MainScene, "Unexpected enabled build scenes.");
        int objects = 0;
        int references = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Game" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Require(root != null, "Prefab could not be loaded: " + path);
            Inspect(root, path, ref objects, ref references);
        }

        var scene = EditorSceneManager.OpenScene(MainScene);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Inspect(root, MainScene, ref objects, ref references);
        }

        foreach (string dependency in AssetDatabase.GetDependencies(MainScene, true))
        {
            if (dependency.StartsWith("Assets/", StringComparison.Ordinal))
            {
                Require(File.Exists(dependency), "Missing scene dependency: " + dependency);
            }
        }

        Debug.Log($"COURSEWORK_ASSET_VALIDATION_PASSED: {objects} objects, {references} assigned references.");
    }

    private static void Inspect(GameObject root, string path, ref int objects, ref int references)
    {
        foreach (Transform item in root.GetComponentsInChildren<Transform>(true))
        {
            objects++;
            Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject) == 0,
                $"Missing script: {path}/{item.name}");
            foreach (Component component in item.GetComponents<Component>())
            {
                if (component == null) continue;
                using SerializedObject serialized = new(component);
                SerializedProperty property = serialized.GetIterator();
                while (property.Next(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                    Require(property.objectReferenceValue != null || property.objectReferenceInstanceIDValue == 0,
                        $"Broken reference: {path}/{item.name}/{component.GetType().Name}.{property.propertyPath}");
                    if (property.objectReferenceValue != null) references++;
                }
            }
        }
    }

    [MenuItem("Coursework/Build Windows Player")]
    public static void BuildWindows()
    {
        ValidateAssets();
        string output = Path.GetFullPath("Artifacts/Build/Windows/2DGameCoursework.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { MainScene },
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        BuildSummary summary = report.summary;
        string result = $"{summary.result}: errors={summary.totalErrors}, warnings={summary.totalWarnings}, bytes={summary.totalSize}";
        File.WriteAllText("Artifacts/windows-build-summary.txt", result + Environment.NewLine + output);
        Require(summary.result == BuildResult.Succeeded, result);
        Debug.Log("COURSEWORK_WINDOWS_BUILD_PASSED: " + result);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
