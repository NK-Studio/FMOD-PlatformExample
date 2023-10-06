using UnityEditor;

namespace NKStudio
{
    [InitializeOnLoad]
    public class FMODLink
    {
        static FMODLink()
        {
            const string fmodFolder = "Assets/Plugins/FMOD";

            if (AssetDatabase.IsValidFolder(fmodFolder))
            {
                var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                    BuildTargetGroup.Standalone);

                if (!symbols.Contains("USE_FMOD"))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                        BuildTargetGroup.Standalone, symbols + ";USE_FMOD");
                }
            }
        }
    }
}
