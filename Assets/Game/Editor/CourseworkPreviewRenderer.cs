using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CourseworkPreviewRenderer
{
    private const string ScenePath = "Assets/Game/Scenes/2DGameCoursework.unity";
    private static string OutputFolder => Path.GetFullPath(
        Path.Combine(Application.dataPath, "../Build/Previews"));

    public static void RenderSeasonPreviews()
    {
        EditorSceneManager.OpenScene(ScenePath);
        Camera camera = Object.FindFirstObjectByType<Camera>();
        if (camera == null)
        {
            throw new MissingReferenceException("Coursework camera is missing.");
        }

        Directory.CreateDirectory(OutputFolder);
        (string name, float x)[] seasons =
        {
            ("01-Spring", 18f),
            ("02-Summer", 54f),
            ("03-Autumn", 90f),
            ("04-Winter", 126f)
        };

        RenderTexture renderTexture = new(1280, 720, 24, RenderTextureFormat.ARGB32);
        Texture2D image = new(1280, 720, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;
        camera.targetTexture = renderTexture;

        foreach ((string name, float x) in seasons)
        {
            camera.transform.position = new Vector3(x, 2f, -10f);
            camera.Render();
            RenderTexture.active = renderTexture;
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(OutputFolder, name + ".png"), image.EncodeToPNG());
        }

        camera.targetTexture = null;
        RenderTexture.active = previous;
        Object.DestroyImmediate(image);
        Object.DestroyImmediate(renderTexture);
        Debug.Log($"COURSEWORK_PREVIEWS_RENDERED: {OutputFolder}");
    }
}
