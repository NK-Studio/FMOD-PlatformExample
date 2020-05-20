using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;
    
    public class Preview : EditorWindow
    {
        List<Editor> previews = new List<Editor>();

        Editor[] editors = new Editor[0];

        int count
        {
            get => previews.Count;
        }

        float width
        {
            get => position.width;
        }

        float height
        {
            get => position.height;
        }
        
        public static Preview Open(Editor[] editors)
        {
            Preview window = GetWindow<Preview>("Smart Preview");
            window.editors = editors;
            window.SetPreviewables();
            return window;
        }

        private void OnEnable()
        {
            autoRepaintOnSceneChange = true;
        }

        private void OnSelectionChange()
        {
            editors = ActiveEditorTracker.sharedTracker.activeEditors;
            SetPreviewables();
            Repaint();
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(0, 0, width, height), GUIContent.none, "PreBackground");

            BeginHorizontal("PreToolbar", GL.Height(kBottomToolbarHeight));
            for (int i = 0; i < count; i++)
            {
                if(null == previews[i]) { continue; }

                float pw = width / count;
                Rect rect = new Rect(i * pw, kBottomToolbarHeight, pw, height - kBottomToolbarHeight);
                //previews[i].OnInteractivePreviewGUI(rect, "preBackground");
                //previews[i].OnPreviewGUI(rect, "preBackground");
                previews[i].DrawPreview(rect);
                if(previews[i].target is ParticleSystem) { Repaint(); }
                if (ToolBar(rect.x, rect.width, previews[i])) { break; }
            }
            EndHorizontal();
        }

        public void SetPreviewables()
        {
            previews.Clear();

            hasGameObject = false;

            for (int i = 0; i < editors.Length; i++)
            {
                if (Filter(editors[i])) { continue; }

                previews.Add(editors[i]);
            }
        }

        bool hasGameObject;

        bool Filter(Editor editor)
        {
            if (null == editor)
            {
                return true;
            }

            if(editor.target.GetType().Name is "PrefabImporter")
            {
                return true;
            }

            if(editor.target is GameObject)
            {
                hasGameObject = true;
            }

            if(editor.target is Material && hasGameObject)
            {
                return true;
            }
            
            if (!editor.HasPreviewGUI())
            {
                return true;
            }

            if (editor.target is ParticleSystemRenderer)
            {
                return true;
            }
            
            return false;
        }

        protected const float kBottomToolbarHeight = 17f;
        bool ToolBar(float x, float width, Editor editor)
        {
            bool close = false;

            BeginHorizontal(GL.Width(width));
            GL.Label(editor.GetPreviewTitle(), "PreToolbar2");
            GL.FlexibleSpace();
            editor.OnPreviewSettings();
            GL.Space(5);
            EndHorizontal();

            return close;
        }
    }
}
