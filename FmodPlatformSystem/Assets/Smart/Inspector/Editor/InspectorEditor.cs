using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Smart
{
    using static EditorGUILayout;
    using static EditorGUIUtility;
    using static EditorGUISmart;
    using static InternalEditorUtility;

    using GL = GUILayout;

    public partial class Inspector
    {
        //
        // Default
        //

        bool DRAW_EDITOR_TITLE(Editor editor)
        {
            bool expanded = GetIsInspectorExpanded(editor);
            SetIsInspectorExpanded(editor, InspectorTitlebar(expanded, editor));
            //Object target = editor.target;
            //GUIContent content = ObjectContent(target, target.GetType());
            //content.text = target.GetType().Name;
            //SetIsInspectorExpanded(editor, Foldout(expanded, content, true));

            return expanded;
        }

        void DRAW_EDITOR(Editor editor)
        {
            if (!DRAW_EDITOR_TITLE(editor)) { return; }

            editor.OnInspectorGUI();
        }

        //
        // Title
        //

        void EDITOR_TITLE(Editor editor)
        {
            if(smartView)
            {
                BeginHorizontal();
                EditorTitleInternal(editor);
                EndHorizontal();

                return;
            }

            DRAW_EDITOR(editor);
        }

        void EditorTitleInternal(Editor editor)
        {
            Object target = editor.target;

            EditorTitleToggle(editor);
            GUIContent content = ObjectContent(target, target.GetType());
            content.text = target.GetType().Name;
            if(AddTitleName(target)) { content.text += string.Format(" ({0})", target.name); }
            GL.Label(content, Styles.boldlabel, GL.Height(singleLineHeight), GL.MaxWidth(halfCurrentView - 5));

            GL.FlexibleSpace();
            EditorMoveButtons(editor);
        }

        bool AddTitleName(Object target)
        {
            if(target is Material) { return true; }

            if(target is MeshFilter) { return true; }

            return false;
        }

        void EditorTitleToggle(Editor editor)
        {
            // Get serialized object
            SerializedObject so = editor.serializedObject;
            // Get enabled property
            SerializedProperty enabled = so.Get("m_Enabled");
            // Exit
            if (null == enabled) { return; }
            so.Update();
            // Draw toggle
            enabled.boolValue = GL.Toggle(enabled.boolValue, GUIContent.none, Styles.radio, GL.Height(singleLineHeight), GL.Width(10));
            so.ApplyModifiedProperties();
        }

        void EditorMoveButtons(Editor editor)
        {
            if (!moveButtons) { return; }
            
            if (GL.Button(GUIContent.none, "ol plus", DontExpandWidth)) { MoveComponentUp(editor); }
            if (GL.Button(GUIContent.none, "ol minus", DontExpandWidth)) { MoveComponentDown(editor); }
        }

        //
        // Editor Fields
        //

        void EDITOR_FIELDS(Editor editor)
        {
            BeginCenterArea(230);
            BeginVertical();

#if NAME_FIELD
            EditorName(editor);
#endif
            EditorReference(editor);

            EndVertical();
            EndCenterAreaWidth();
        }

        //
        // Name Field
        //
#if NAME_FIELD
        void EditorName(Editor editor)
        {
            if (!displayName) { return; }

            GUI.enabled = !(editor is MaterialEditor);
            EditorNameInternal(editor);
            GUI.enabled = true;
        }
        
        public void EditorNameInternal(Editor editor)
        {
            nameProperty = editor.serializedObject.FindProperty("m_Name");

            if (null == nameProperty) { return; }

            string name = nameProperty.stringValue;

            if (editor.targets.Length > 1)
            {
                PropertyField(nameProperty, GUIContent.none);
            }
            else
            {
                bool empty = string.IsNullOrWhiteSpace(name);
                EditorGUI.BeginChangeCheck();
                name = GL.TextField(name, empty ? Styles.InBigTitlePostGrey : Styles.InBigTitlePost);
                if (EditorGUI.EndChangeCheck())
                {
                    nameProperty.stringValue = name;

                    GetWindow<Inspector>().Repaint();
                }
            }

            KeyCode keyCode = Event.current.keyCode;

            if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter || keyCode == KeyCode.Escape)
            {
                nameProperty.stringValue = name;

                EditorGUI.FocusTextInControl(null);

                Repaint();
            }
        }
#endif

        //
        // Reference Field
        //
        
        public void EditorReference(Editor editor)
        {
            if (!displayReference) { return; }

            SerializedObject so = editor.serializedObject;
            referenceProperty = so.Get("m_Script");
            
            EditorReferenceInternal(editor);
        }

        void EditorReferenceInternal(Editor editor)
        {
            if (null == referenceProperty) { return; }

            GUI.enabled = false;
            PropertyField(referenceProperty, GUIContent.none);
            GUI.enabled = true;
        }

        //
        // Open
        //
        
        void EDITOR_OPEN(Editor editor)
        {
            if (!smartView) { return; }

            BeginHorizontal();
            GL.FlexibleSpace();
            EditorOpen1(editor);
            GL.FlexibleSpace();
            EndHorizontal();
        }

        void EditorOpen1(Editor editor)
        {
            bool isSelected = IsSelected(editor);
            string label = isSelected ? string.Format("Open ({0})", selectedCount) : "Open";

            GUI.enabled = selectedCount == 0 || isSelected;
            EditorOpen2(label, editor);
            GUI.enabled = true;
        }

        void EditorOpen2(string label, Editor editor)
        {
            if (GL.Button(label, activeButtonStyle, GL.MaxWidth(230)))
            {
                EditorOpen3(editor);
            }
        }

        void EditorOpen3(Editor editor)
        {
            if(!selection.Contains(editor))
            {
                selection.Add(editor);
            }

            OPEN();
        }

        //
        // Style
        //


        GUIStyle EDITOR_STYLE(Editor editor)
        {
            GUIStyle style = new GUIStyle(activeStyle);
            style.stretchHeight = true;
            style.fixedHeight = 0;

            if (IsSelected(editor))
            {
                //GUIStyle selectionRect = new GUIStyle("SelectionRect");
                GUIStyle selectionRect = new GUIStyle(Styles.progressBar);
                if (editorStyle == -1)
                {
                    style = selectionRect;
                }
                else
                {
                    style.border = selectionRect.border;
                    style.normal.background = selectionRect.normal.background;
                }
            }

            return style;
        }
    }
}
