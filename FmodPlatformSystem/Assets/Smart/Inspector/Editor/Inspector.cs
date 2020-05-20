using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;

    public partial class Inspector : EditorWindow
    {
        [MenuItem("Tools/Smart/Inspector")]
        static void Open()
        {
            GetWindow<Inspector>("Smart Inspector");
        }

        private void OnEnable()
        {
            autoRepaintOnSceneChange = true;
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeChanged;
        }

        void PlayModeChanged(PlayModeStateChange state)
        {
            CLOSE();
        }

        private void OnSelectionChange()
        {
            if(open && !locked) { CLOSE(); }
            selection.Clear();
            Repaint();
        }

        private void OnGUI()
        {
            GUI_RECT();
            INSPECTOR_EVENT();
        }

        void GUI_RECT()
        {
            rect = BeginVertical();
            GUI_SCROLL_VIEW();
            EndVertical();

            PREVIEW_BUTTON(open ? opened.ToArray() : editors);
        }

        void GUI_SCROLL_VIEW()
        {
            scroll = BeginScrollView(scroll);
            GUI_CONTENT();
            GL.FlexibleSpace();
            EndScrollView();
        }

        void GUI_CONTENT()
        {
            EditorGUIUtility.wideMode = true;

            INIT_FILTERS();
            TYPE_SELECTION();
            FILTER();
            SELECTION();
            ADD_COMPONENT_BUTTON();
        }
    }
}
