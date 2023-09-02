#if UNITY_EDITOR
using System;
using UnityEditor;
using System.IO;
#endif
#if FMODPlus
using FMODUnity;
#endif
using UnityEngine;

namespace FMODPlus
{
    public class FMODRuntimeManagerHock
    {
        #region Studio Listener Icon Change

        private const string SLETargetPath = "Assets/Plugins/FMOD/src/Editor/StudioListenerEditor.cs";


        // 추가할 코드 준비
        private const string SLEAdditionalCode01 = @"
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
        private const string SLEAdditionalCode02 = @"
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

        #endregion

        #region RuntimeManager Icon Change

        private const string RMTargetPath = "Assets/Plugins/FMOD/src/RuntimeManager.cs";

        private const string RMAdditionalCode01 = @"
#if FMODPlus
        public static Action UpdateActiveAudioSource;
#endif
";

        private const string RMAdditionalCode02 = @"
#if FMODPlus
                UpdateActiveAudioSource?.Invoke();
#endif
";

        #endregion

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
#if FMODPlus
            RuntimeManager.UpdateActiveAudioSource = FMODAudioSource.UpdateActiveAudioSource;
#endif
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod, MenuItem("FMOD/FMOD Plus/Other/Force Update", priority = 2000-1)]
        private static void StartEditor()
        {
            RuntimeManagerHock();
            StudioListenerEditorHock();

            AssetDatabase.Refresh();
            
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!currentDefines.Contains("FMODPlus"))
            {
                currentDefines += ";FMODPlus"; // 새로운 디파인 추가
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, currentDefines);
            }
        }

        private static void RuntimeManagerHock()
        {
            MonoScript fileContents = AssetDatabase.LoadAssetAtPath<MonoScript>(RMTargetPath);

            // 첫번째 라인에 A글자가 있는지 확인
            string[] lines = fileContents.text.Split('\n');

            string firstLine = lines[0];
            if (!firstLine.Contains(FMODPlusUtility.FMODPlusDefine))
            {
                string modifiedCode = FMODPlusUtility.FMODPlusDefine + "\n" + fileContents;

                // 코드 삽입 위치 찾기
                int insertionIndex01 =
                    modifiedCode.IndexOf("public static FMOD.Studio.System StudioSystem", StringComparison.Ordinal);

                // 코드 삽입
                modifiedCode = modifiedCode.Insert(insertionIndex01, RMAdditionalCode01 + "\n\n\t\t");

                int insertionIndex02 =
                    modifiedCode.IndexOf("StudioEventEmitter.UpdateActiveEmitters();", StringComparison.Ordinal);
                modifiedCode = modifiedCode.Insert(insertionIndex02, RMAdditionalCode02 + "\n\t\t\t\t");

                File.WriteAllText(RMTargetPath, modifiedCode);
            }
        }

        private static void StudioListenerEditorHock()
        {
            MonoScript fileContents = AssetDatabase.LoadAssetAtPath<MonoScript>(SLETargetPath);
            // 첫번째 라인에 A글자가 있는지 확인
            string[] lines = fileContents.text.Split('\n');

            string firstLine = lines[0];
            if (!firstLine.Contains(FMODPlusUtility.FMODPlusDefine))
            {
                string modifiedCode = FMODPlusUtility.FMODPlusDefine + "\n" + fileContents;

                // 코드 삽입 위치 찾기
                int insertionIndex01 =
                    modifiedCode.IndexOf("attenuationObject = serializedObject.FindProperty(\"attenuationObject\");",
                        StringComparison.Ordinal)
                    + "attenuationObject = serializedObject.FindProperty(\"attenuationObject\");".Length;

                // 코드 삽입
                modifiedCode = modifiedCode.Insert(insertionIndex01, SLEAdditionalCode01);

                int insertionIndex02 =
                    modifiedCode.IndexOf("public override void OnInspectorGUI()", StringComparison.Ordinal);
                modifiedCode = modifiedCode.Insert(insertionIndex02, SLEAdditionalCode02);

                File.WriteAllText(SLETargetPath, modifiedCode);
            }
        }

#endif
    }
}