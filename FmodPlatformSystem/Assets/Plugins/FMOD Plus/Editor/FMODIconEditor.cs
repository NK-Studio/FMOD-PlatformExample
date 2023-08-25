#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FMODPlus
{
    public class FMODIconEditor : Editor
    {
        #region Studio Listener Icon Change
        private const string TargetPath = "Assets/Plugins/FMOD/src/Editor/StudioListenerEditor.cs";
        

        // 추가할 코드 준비
        private const string AdditionalCode01 = @"
        // StudioListener
        string darkIconGuid = AssetDatabase.GUIDToAssetPath(""fd3f51f8ed3d44d0d8a412645ed82cca"");
        var darkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

        string whiteIconGuid = AssetDatabase.GUIDToAssetPath(""ed40e8a0a0a02464d86ddbb4bab2b6e8"");
        var whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

        string path = ""Assets/Plugins/FMOD/src/StudioListener.cs"";
        var studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        ApplyIcon(darkIcon, whiteIcon, studioListener);
";

        // 위 코드를 private const string로 만들어서 추가할 수 있도록 준비
        private const string AdditionalCode02 = @"
        public static void ApplyIcon(Texture2D darkIcon, Texture2D whiteIcon, Object targetObject)
        {
            if (!darkIcon || !whiteIcon)
            {
                // Debug : Debug.LogWarning($""{targetObject.name} : No Binding Icon"");
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
    ";

        [InitializeOnLoadMethod]
        private static void StartEditor()
        {
            MonoScript fileContents = AssetDatabase.LoadAssetAtPath<MonoScript>(TargetPath);
            // 첫번째 라인에 A글자가 있는지 확인
            string[] lines = fileContents.text.Split('\n');

            string firstLine = lines[0];
            if (!firstLine.Contains(FMODPlusUtility.FMODPlusDefine))
            {
                string modifiedCode = FMODPlusUtility.FMODPlusDefine + "\n" + fileContents;

                // 코드 삽입 위치 찾기
                int insertionIndex01 = modifiedCode.IndexOf("attenuationObject = serializedObject.FindProperty(\"attenuationObject\");", StringComparison.Ordinal)
                    + "attenuationObject = serializedObject.FindProperty(\"attenuationObject\");".Length;

                // 코드 삽입
                modifiedCode = modifiedCode.Insert(insertionIndex01, AdditionalCode01);

                int insertionIndex02 = modifiedCode.IndexOf("public override void OnInspectorGUI()", StringComparison.Ordinal);
                modifiedCode = modifiedCode.Insert(insertionIndex02, AdditionalCode02);

                File.WriteAllText(TargetPath, modifiedCode);
            }
        }
        #endregion

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
    }
}
#endif
