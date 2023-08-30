// Define FMOD Plus
using UnityEditor;
using UnityEngine;

namespace FMODUnity
{
    [CustomEditor(typeof(StudioListener))]
    [CanEditMultipleObjects]
    public class StudioListenerEditor : Editor
    {
        public SerializedProperty attenuationObject;

        private void OnEnable()
        {
            attenuationObject = serializedObject.FindProperty("attenuationObject");
        // StudioListener
        string darkIconGuid = AssetDatabase.GUIDToAssetPath("fd3f51f8ed3d44d0d8a412645ed82cca");
        var darkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

        string whiteIconGuid = AssetDatabase.GUIDToAssetPath("ed40e8a0a0a02464d86ddbb4bab2b6e8");
        var whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

        string path = "Assets/Plugins/FMOD/src/StudioListener.cs";
        var studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        ApplyIcon(darkIcon, whiteIcon, studioListener);

        }

        
        public static void ApplyIcon(Texture2D darkIcon, Texture2D whiteIcon, Object targetObject)
        {
            if (!darkIcon || !whiteIcon)
            {
                // Debug : Debug.LogWarning($"{targetObject.name} : No Binding Icon");
                EditorGUIUtility.SetIconForObject(targetObject, null);
                return;
            }
    
            bool isDarkMode = EditorGUIUtility.isProSkin;
    
            if (isDarkMode)
            {
                if (darkIcon)
                    EditorGUIUtility.SetIconForObject(targetObject, darkIcon);
            }
            else
            {
                if (whiteIcon)
                    EditorGUIUtility.SetIconForObject(targetObject, whiteIcon);
            }
        }
    public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            int index = ((StudioListener)serializedObject.targetObject).ListenerNumber;
            EditorGUILayout.IntSlider("Listener Index", index, 0, FMOD.CONSTANTS.MAX_LISTENERS - 1);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(attenuationObject);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
