#if UNITY_EDITOR
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(FMODEventTrigger))]
    public class FMODEventTriggerEditor : Editor
    {
        private void OnEnable()
        {
            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("07555e228b1ea40bb871f7540d3d2022");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3443518b4ee364e1b9cf151024acc8cc");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/FMODEventTrigger.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);
        }

        public override void OnInspectorGUI()
        {
            var source = serializedObject.FindProperty("Source");
            var begin = serializedObject.FindProperty("PlayEvent");
            var end = serializedObject.FindProperty("StopEvent");
            var tag = serializedObject.FindProperty("CollisionTag");

            EditorGUILayout.PropertyField(source, new GUIContent("Audio Source"));
            
            if ((begin.enumValueIndex >= (int)EmitterGameEvent.TriggerEnter &&
                 begin.enumValueIndex <= (int)EmitterGameEvent.TriggerExit2D) ||
                (end.enumValueIndex >= (int)EmitterGameEvent.TriggerEnter &&
                 end.enumValueIndex <= (int)EmitterGameEvent.TriggerExit2D))
            {
                tag.stringValue = EditorGUILayout.TagField("Collision Tag", tag.stringValue);
            }
            
            EditorGUILayout.PropertyField(begin, new GUIContent("Play Event"));
            EditorGUILayout.PropertyField(end, new GUIContent("Stop Event"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif