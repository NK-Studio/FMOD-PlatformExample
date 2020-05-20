using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(Folder))]
    public class FolderEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Folder", false, -1)]
        private static void CreateFolder()
        {
            var go = new GameObject("Folder", typeof(Folder));
            if(Selection.activeGameObject != null) 
                go.transform.parent = Selection.activeGameObject.transform;
        }

        private SerializedProperty m_Color;

        private void OnEnable() => 
            m_Color = serializedObject.FindProperty("Color");

        public override bool HasPreviewGUI() => false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField("Folder Color", m_Color.colorValue);
            if(EditorGUI.EndChangeCheck())
            {
                m_Color.colorValue = color;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

