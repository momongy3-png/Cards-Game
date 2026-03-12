using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UnusedAssetsFinder : EditorWindow
{
    private List<SceneAsset> scenes = new List<SceneAsset>();
    private Vector2 scroll;
    private List<string> unusedAssets = new List<string>();

    public static void Open()
    {
        GetWindow<UnusedAssetsFinder>("Unused Assets Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes To Scan", EditorStyles.boldLabel);

        for (int i = 0; i < scenes.Count; i++)
        {
            GUILayout.BeginHorizontal();
            scenes[i] = (SceneAsset)EditorGUILayout.ObjectField(scenes[i], typeof(SceneAsset), false);

            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                scenes.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Scene"))
        {
            scenes.Add(null);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Unused Assets", GUILayout.Height(30)))
        {
            Scan();
        }

        GUILayout.Space(10);

        if (unusedAssets.Count > 0)
        {
            GUILayout.Label($"Unused Assets ({unusedAssets.Count})", EditorStyles.boldLabel);

            scroll = GUILayout.BeginScrollView(scroll);

            foreach (string path in unusedAssets)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                GUILayout.Label(path);

                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    EditorGUIUtility.PingObject(obj);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("DELETE ALL UNUSED ASSETS", GUILayout.Height(35)))
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Delete Unused Assets",
                    $"You are about to permanently delete {unusedAssets.Count} assets.\n\n" +
                    "This cannot be undone.\n\nAre you sure?",
                    "Yes, delete them",
                    "Cancel"
                );

                if (confirm)
                {
                    DeleteAllUnused();
                }
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void Scan()
    {
        unusedAssets.Clear();

        HashSet<string> usedAssets = new HashSet<string>();

        foreach (var scene in scenes)
        {
            if (scene == null)
                continue;

            string scenePath = AssetDatabase.GetAssetPath(scene);
            string[] deps = AssetDatabase.GetDependencies(scenePath, true);

            foreach (string dep in deps)
            {
                if (dep.StartsWith("Assets/"))
                    usedAssets.Add(dep);
            }
        }

        string assetsPath = Application.dataPath;
        string[] allFiles = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);

        foreach (string fullPath in allFiles)
        {
            if (fullPath.EndsWith(".meta"))
                continue;

            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "").Replace("\\", "/");

            if (!usedAssets.Contains(assetPath))
            {
                unusedAssets.Add(assetPath);
            }
        }

        unusedAssets = unusedAssets.OrderBy(a => a).ToList();

        Debug.Log($"Scan complete. Unused assets found: {unusedAssets.Count}");
    }

    private void DeleteAllUnused()
    {
        AssetDatabase.StartAssetEditing();

        int deleted = 0;

        foreach (string path in unusedAssets)
        {
            bool success = AssetDatabase.DeleteAsset(path);
            if (success)
                deleted++;
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();

        Debug.Log($"Deleted {deleted} unused assets.");

        unusedAssets.Clear();
    }
}