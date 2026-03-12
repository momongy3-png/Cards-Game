using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class 
    TechnicalHub : EditorWindow
{
    private List<ToolButton> tools = new List<ToolButton>();

    [MenuItem("Tools/Technical Hub")]
    public static void Open()
    {
        GetWindow<TechnicalHub>("Technical Hub");
    }

    private void OnEnable()
    {
        tools.Clear();

        // Register tools here
        tools.Add(new ToolButton("Assets Size Viewer", AssetsSizeViewer.Open));
        tools.Add(new ToolButton("Image Editor", ImageEditor.Open));
        tools.Add(new ToolButton("Unused Assets", UnusedAssetsFinder.Open));
        tools.Add(new ToolButton("Scene Materials Viewer", SceneMaterialsViewer.Open));
        tools.Add(new ToolButton("Raycast Remover", RayCastRemover.Open));
        tools.Add(new ToolButton("Folder Flattener", FolderFlattener.Open));
    }

    private void OnGUI()
    {
        GUILayout.Label("Mon Technical Tools", EditorStyles.boldLabel);
        GUILayout.Space(10);

        foreach (var tool in tools)
        {
            if (GUILayout.Button(tool.Name, GUILayout.Height(30)))
            {
                tool.Action?.Invoke();
            }
        }
    }

    private class ToolButton
    {
        public string Name;
        public Action Action;

        public ToolButton(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
