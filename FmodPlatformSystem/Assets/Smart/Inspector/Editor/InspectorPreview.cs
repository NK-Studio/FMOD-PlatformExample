using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;
	public partial class Inspector
    {
        void PREVIEW_BUTTON(Editor[] previews)
        {
            if (!HasPreview(previews)) { return; }

            if (GL.Button("Preview (Experimental)", "PreButton"))
            {
                preview = Preview.Open(previews);
            }
        }

        bool HasPreview(Editor[] editors)
        {
            for(int i = 0; i < editors.Length; i++)
            {
                if (editors[i].HasPreviewGUI()) { return true; }
            }

            return false;
        }
    }
}
