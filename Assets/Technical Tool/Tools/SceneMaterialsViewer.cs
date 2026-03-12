using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SceneMaterialsViewer : EditorWindow
{
    private Vector2 scroll;

    private Dictionary<Material, List<Renderer>> materials =
        new Dictionary<Material, List<Renderer>>();

    public static void Open()
    {
        GetWindow<SceneMaterialsViewer>("Scene Materials");
    }

    void OnEnable()
    {
        Refresh();
    }

    void OnGUI()
    {
        GUILayout.Space(5);

        if (GUILayout.Button("Refresh", GUILayout.Height(25)))
        {
            Refresh();
        }

        GUILayout.Space(10);

        GUILayout.Label("Total Unique Materials: " + materials.Count,
            EditorStyles.boldLabel);

        GUILayout.Space(5);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var pair in materials)
        {
            Material mat = pair.Key;
            List<Renderer> renderers = pair.Value;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.ObjectField(mat, typeof(Material), false);

            GUILayout.Label("Used By: " + renderers.Count,
                GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();

            foreach (var r in renderers)
            {
                if (GUILayout.Button(r.gameObject.name,
                    EditorStyles.miniButton))
                {
                    Selection.activeGameObject = r.gameObject;
                    EditorGUIUtility.PingObject(r.gameObject);
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    void Refresh()
    {
        materials.Clear();

        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (var r in renderers)
        {
            foreach (var mat in r.sharedMaterials)
            {
                if (mat == null) continue;

                if (!materials.ContainsKey(mat))
                    materials.Add(mat, new List<Renderer>());

                materials[mat].Add(r);
            }
        }
    }
}