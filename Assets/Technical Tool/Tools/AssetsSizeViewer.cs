using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class AssetsSizeViewer : EditorWindow
{
    private Vector2 scroll;
    private List<FileData> files = new List<FileData>();

    public static void Open()
    {
        GetWindow<AssetsSizeViewer>("Assets Size Viewer");
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(30)))
        {
            Refresh();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawHeader();

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var file in files)
        {
            DrawRow(file);
        }

        GUILayout.EndScrollView();
    }

    private void Refresh()
    {
        files.Clear();

        string assetsPath = Application.dataPath;
        string[] allFiles = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);

        foreach (string fullPath in allFiles)
        {
            if (fullPath.EndsWith(".meta"))
                continue;

            FileInfo info = new FileInfo(fullPath);

            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "").Replace("\\", "/");

            FileData data = new FileData
            {
                Name = Path.GetFileName(fullPath),
                Path = assetPath,
                SizeBytes = info.Length,
                Type = Path.GetExtension(fullPath)
            };

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (asset is Texture2D tex)
            {
                data.IsTexture = true;
                data.Width = tex.width;
                data.Height = tex.height;
            }

            files.Add(data);
        }

        files = files.OrderByDescending(f => f.SizeBytes).ToList();
    }

    private void DrawHeader()
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Name", GUILayout.Width(220));
        GUILayout.Label("Type", GUILayout.Width(60));
        GUILayout.Label("Size", GUILayout.Width(80));
        GUILayout.Label("Resolution", GUILayout.Width(120));
        GUILayout.Label("Path");
        GUILayout.EndHorizontal();
    }

    private void DrawRow(FileData file)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUILayout.Label(file.Name, GUILayout.Width(220));
        GUILayout.Label(file.Type, GUILayout.Width(60));
        GUILayout.Label(FormatSize(file.SizeBytes), GUILayout.Width(80));

        if (file.IsTexture)
            GUILayout.Label($"{file.Width} x {file.Height}", GUILayout.Width(120));
        else
            GUILayout.Label("-", GUILayout.Width(120));

        GUILayout.Label(file.Path);

        GUILayout.EndHorizontal();
    }

    private string FormatSize(long bytes)
    {
        if (bytes > 1024 * 1024)
            return (bytes / (1024f * 1024f)).ToString("0.00") + " MB";
        if (bytes > 1024)
            return (bytes / 1024f).ToString("0.0") + " KB";

        return bytes + " B";
    }

    private class FileData
    {
        public string Name;
        public string Path;
        public string Type;
        public long SizeBytes;
        public bool IsTexture;
        public int Width;
        public int Height;
    }
}