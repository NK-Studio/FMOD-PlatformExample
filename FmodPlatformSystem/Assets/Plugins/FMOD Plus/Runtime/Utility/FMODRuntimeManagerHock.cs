#if UNITY_EDITOR
using System;
using UnityEditor;
using System.IO;
#endif
using FMODPlus;
#if FMODPlus
using FMODUnity;
#endif
using UnityEngine;

public class FMODRuntimeManagerHock
{
    private const string TargetPath = "Assets/Plugins/FMOD/src/RuntimeManager.cs";

    private const string AdditionalCode01 = @"
#if FMODPlus
        public static Action UpdateActiveAudioSource;
#endif
";
    private const string AdditionalCode02 = @"
#if FMODPlus
                UpdateActiveAudioSource?.Invoke();
#endif
";

    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
#if FMODPlus
        RuntimeManager.UpdateActiveAudioSource = FMODAudioSource.UpdateActiveAudioSource;
#endif
    }

#if UNITY_EDITOR
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
            int insertionIndex01 =
                modifiedCode.IndexOf("public static FMOD.Studio.System StudioSystem", StringComparison.Ordinal);

            // 코드 삽입
            modifiedCode = modifiedCode.Insert(insertionIndex01, AdditionalCode01 + "\n\n\t\t");

            int insertionIndex02 =
                modifiedCode.IndexOf("StudioEventEmitter.UpdateActiveEmitters();", StringComparison.Ordinal);
            modifiedCode = modifiedCode.Insert(insertionIndex02, AdditionalCode02 + "\n\t\t\t\t");

            File.WriteAllText(TargetPath, modifiedCode);
        }
        
        var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (!currentDefines.Contains("FMODPlus"))
        {
            currentDefines += ";FMODPlus"; // 새로운 디파인 추가
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, currentDefines);
        }
    }
#endif
}