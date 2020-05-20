﻿using UnityEditor;
using UnityEngine;
using GL = UnityEngine.GUILayout;
using GLE = UnityEditor.EditorGUILayout;
using GLU = UnityEditor.EditorGUIUtility;
using static UnityEditor.ActiveEditorTracker;

public class SmartEditorViewer : EditorWindow
{
    [MenuItem("Tools/Smart/Editor Viewer")]
    public static void Open()
    {
        GetWindow<SmartEditorViewer>();
    }

    private void OnEnable()
    {
        autoRepaintOnSceneChange = true;
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    Vector2 scroll;
    float labelWidth;
    float fieldWidth;
    private void OnGUI()
    {
        Editor[] editors = Resources.FindObjectsOfTypeAll<Editor>();

        labelWidth = GLU.labelWidth;
        fieldWidth = GLU.fieldWidth;

        GLU.labelWidth = position.width / 3;
        GLU.fieldWidth = position.width / 3;
        GLU.wideMode = true;

        //GLE.LabelField("Editor Name", "Editor");
        scroll = GLE.BeginScrollView(scroll);

        GUIContent content;
        float width = position.width / 4;
        for (int i = 0; i < editors.Length; i++)
        {
            GLE.BeginHorizontal("Helpbox");
            {
                GL.Label(editors[i].target.GetType().Name, GL.Width(width));

                content = GLU.ObjectContent(editors[i].target, editors[i].target.GetType());
                GLE.ObjectField(content, editors[i], typeof(Object), false);
            }
            GLE.EndHorizontal();

            GL.Label(string.Format("Has Preview: {0}", editors[i].HasPreviewGUI() ? "YES" : "NO"));
        }
        GLE.EndScrollView();

        GLU.labelWidth = labelWidth;

        if(GL.Button("Clear"))
        {
            for(int i = 0; i < editors.Length; i++)
            {
                DestroyImmediate(editors[i]);
            }

            sharedTracker.ForceRebuild();
        }
    }
}