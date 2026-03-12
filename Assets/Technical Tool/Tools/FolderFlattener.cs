using System.IO;
using UnityEditor;
using UnityEngine;

public class FolderFlattener : EditorWindow
{
    private DefaultAsset folder;

    public static void Open()
    {
        GetWindow<FolderFlattener>("Folder Flattener");
    }

    private void OnGUI()
    {
        GUILayout.Label("Flatten Folder Files", EditorStyles.boldLabel);

        folder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Parent Folder",
            folder,
            typeof(DefaultAsset),
            false
        );

        GUILayout.Space(10);

        if (folder != null)
        {
            if (GUILayout.Button("Move All Files To Parent", GUILayout.Height(30)))
            {
                FlattenFolder();
            }
        }
    }

    private void FlattenFolder()
    {
        string parentPath = AssetDatabase.GetAssetPath(folder);

        string absoluteParent = Path.GetFullPath(parentPath);

        string[] allFiles = Directory.GetFiles(absoluteParent, "*", SearchOption.AllDirectories);

        int moved = 0;

        foreach (var file in allFiles)
        {
            if (file.EndsWith(".meta"))
                continue;

            string fileDir = Path.GetDirectoryName(file);

            if (fileDir == absoluteParent)
                continue;

            string fileName = Path.GetFileName(file);
            string targetPath = Path.Combine(absoluteParent, fileName);

            if (File.Exists(targetPath))
            {
                Debug.LogWarning($"Skipped (name exists): {fileName}");
                continue;
            }

            string assetFrom = file.Replace("\\", "/");
            string assetTo = (parentPath + "/" + fileName);

            AssetDatabase.MoveAsset(assetFrom.Substring(assetFrom.IndexOf("Assets")), assetTo);

            moved++;
        }

        AssetDatabase.Refresh();

        Debug.Log($"Folder flatten complete. Moved {moved} files.");
    }
}
