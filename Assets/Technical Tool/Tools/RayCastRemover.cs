using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RayCastRemover : EditorWindow
{
    private GameObject prefab;

    public static void Open()
    {
        GetWindow<RayCastRemover>("UI Raycast Optimizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab UI Raycast Optimizer", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField(
            "UI Prefab",
            prefab,
            typeof(GameObject),
            false
        );

        GUILayout.Space(10);

        if (prefab != null)
        {
            if (GUILayout.Button("Optimize Raycast Targets", GUILayout.Height(30)))
            {
                OptimizePrefab();
            }
        }
    }

    private void OptimizePrefab()
    {
        string path = AssetDatabase.GetAssetPath(prefab);

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

        Image[] images = prefabRoot.GetComponentsInChildren<Image>(true);

        int modified = 0;

        foreach (var img in images)
        {
            Button btn = img.GetComponent<Button>();

            if (btn != null)
                continue;

            if (img.raycastTarget)
            {
                img.raycastTarget = false;
                modified++;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log($"UI Raycast Optimization complete. Modified {modified} Images.");
    }
}
