using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;

	public partial class Inspector
    {
        bool AssertEditor(Editor editor)
        {
            if (null == editor) { return false; }
            if (null == editor.target) { return false; }
            if (null == editor.targets) { return false; }

            return true;
        }

        void DRAW_MULTI_EDITING()
        {
            if (!multiEditing) { return; }
            
            GUIStyle label = "label";
            label.wordWrap = true;

            BeginVertical("Helpbox");
            GL.Space(10);
            GL.Label("Components that are only on some of the selected objects cannot be multi-edited", label);
            GL.Space(10);
            EndVertical();
        }
    }
}
