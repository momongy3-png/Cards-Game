using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ImageEditor : EditorWindow
{
    private Vector2 scroll;
    private List<ImageItem> images = new List<ImageItem>();

    public static void Open()
    {
        GetWindow<ImageEditor>("Image Editor");
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh Images", GUILayout.Height(30)))
        {
            Refresh();
        }

        GUILayout.Space(10);

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var img in images)
        {
            DrawImageItem(img);
        }

        GUILayout.EndScrollView();
    }

    private void Refresh()
    {
        images.Clear();

        string assetsPath = Application.dataPath;
        string[] files = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);

        foreach (string fullPath in files)
        {
            if (fullPath.EndsWith(".meta"))
                continue;

            string ext = Path.GetExtension(fullPath).ToLower();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                continue;

            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "").Replace("\\", "/");

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (tex == null)
                continue;

            FileInfo info = new FileInfo(fullPath);

            images.Add(new ImageItem
            {
                Path = assetPath,
                Texture = tex,
                Width = tex.width,
                Height = tex.height,
                SizeBytes = info.Length
            });
        }

        // Sort by size (biggest first)
        images = images.OrderByDescending(i => i.SizeBytes).ToList();
    }

    private void DrawImageItem(ImageItem img)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUILayout.Label(img.Texture, GUILayout.Width(64), GUILayout.Height(64));

        GUILayout.BeginVertical();

        GUILayout.Label(Path.GetFileName(img.Path));
        GUILayout.Label(FormatSize(img.SizeBytes));

        GUILayout.BeginHorizontal();
        GUILayout.Label("W", GUILayout.Width(15));
        img.Width = EditorGUILayout.IntField(img.Width, GUILayout.Width(60));

        GUILayout.Label("H", GUILayout.Width(15));
        img.Height = EditorGUILayout.IntField(img.Height, GUILayout.Width(60));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Convert to JPG toggle
        img.ConvertToJPG = EditorGUILayout.ToggleLeft("Convert To JPG", img.ConvertToJPG);

        // JPG compression slider
        GUILayout.BeginHorizontal();
        GUILayout.Label("JPG Quality", GUILayout.Width(80));
        img.JPGQuality = EditorGUILayout.IntSlider(img.JPGQuality, 0, 100);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (GUILayout.Button("Edit", GUILayout.Width(80)))
        {
            ResizeTextureFile(img);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void ResizeTextureFile(ImageItem img)
    {
        if (img.Width <= 0 || img.Height <= 0)
        {
            Debug.LogError("Invalid resolution");
            return;
        }

        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string fullPath = Path.Combine(projectPath, img.Path);

        if (!File.Exists(fullPath))
        {
            Debug.LogError("File not found: " + fullPath);
            return;
        }

        byte[] fileData = File.ReadAllBytes(fullPath);

        Texture2D src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        src.LoadImage(fileData);

        Texture2D resized = new Texture2D(img.Width, img.Height, TextureFormat.RGBA32, false);

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                float u = x / (float)img.Width;
                float v = y / (float)img.Height;
                Color col = src.GetPixelBilinear(u, v);
                resized.SetPixel(x, y, col);
            }
        }

        resized.Apply();

        bool convertToJPG = img.ConvertToJPG;
        string extension = Path.GetExtension(fullPath).ToLower();

        byte[] newFileData;
        string newFullPath = fullPath;
        string newAssetPath = img.Path;

        if (convertToJPG)
        {
            newFileData = resized.EncodeToJPG(img.JPGQuality);

            if (extension != ".jpg" && extension != ".jpeg")
            {
                newFullPath = Path.ChangeExtension(fullPath, ".jpg");
                newAssetPath = Path.ChangeExtension(img.Path, ".jpg");

                File.Delete(fullPath);
            }
        }
        else
        {
            if (extension == ".png")
                newFileData = resized.EncodeToPNG();
            else
                newFileData = resized.EncodeToJPG(img.JPGQuality);
        }

        File.WriteAllBytes(newFullPath, newFileData);

        AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceUpdate);

        img.Path = newAssetPath;
        img.Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);
        img.SizeBytes = new FileInfo(newFullPath).Length;

        DestroyImmediate(src);
        DestroyImmediate(resized);

        Debug.Log($"Edited: {newAssetPath} → {img.Width}x{img.Height} | JPG Quality: {img.JPGQuality}");
    }
    private string FormatSize(long bytes)
    {
        if (bytes > 1024 * 1024)
            return (bytes / (1024f * 1024f)).ToString("0.00") + " MB";
        if (bytes > 1024)
            return (bytes / 1024f).ToString("0.0") + " KB";

        return bytes + " B";
    }

    private class ImageItem
    {
        public string Path;
        public Texture2D Texture;
        public int Width;
        public int Height;
        public long SizeBytes;

        public bool ConvertToJPG = false;
        public int JPGQuality = 90;
    }
}