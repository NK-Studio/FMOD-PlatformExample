using UnityEditor;
using UnityEngine;

namespace AutoManager
{
    public class AutoManagerSetter : UnityEditor.Editor
    {
        private const string AutoManagerSettingDirectory = "Assets/Resources";
        private const string AutoManagerAssetPath = "Assets/Resources/AutoManagerSettings.asset";

        [MenuItem("Assets/Open Auto Manager")]
        private static void OpenAutoManager()
        {
            Selection.activeObject = FindAutoManager();
        }

        private static AutoManagerSettings FindAutoManager()
        {
            AutoManagerSettings autoManager = Resources.Load<AutoManagerSettings>("AutoManagerSettings");
            
            if (autoManager == null)
            {
                if (!AssetDatabase.IsValidFolder(AutoManagerSettingDirectory))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                autoManager = AssetDatabase.LoadAssetAtPath<AutoManagerSettings>(AutoManagerAssetPath);

                if (autoManager == null)
                {
                    autoManager = CreateInstance<AutoManagerSettings>();
                    AssetDatabase.CreateAsset(autoManager, AutoManagerAssetPath);     
                }
            }

            return autoManager;
        }
    }
}