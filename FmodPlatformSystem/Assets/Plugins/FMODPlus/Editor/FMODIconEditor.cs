#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class FMODIconEditor : Editor
{
    [InitializeOnLoadMethod]
    private static void StartEditor()
    {
        InitIcon();
    }

    private static void InitIcon()
    {
        {
            // StudioListener
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("fd3f51f8ed3d44d0d8a412645ed82cca");
            var darkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("ed40e8a0a0a02464d86ddbb4bab2b6e8");
            var whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMOD/src/StudioListener.cs";
            var studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Parameter Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("74cfbd073c7464035ba232171ef31f0f");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("6531cd3743c664274b21aa41c9b00c5c");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/FMODParameterSender.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Event Command Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);
            
            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/EventCommandSender.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Audio Source
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a10e3143b46034572a9a5e38034e181c");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("ae854481e58584815befcbf213b32745");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/FMODAudioSource.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("e9081b0172b4f4735ac3dc549b17a6b8");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("2be0f2ec703f2444b8795748b7ffcee3");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/RegisterEventClip.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        
    }

    private static void ApplyIcon(Texture2D darkIcon, Texture2D whiteIcon, Object targetObject)
    {
        if (!darkIcon || !whiteIcon)
        {
            Debug.LogWarning($"{targetObject.name} : No Binding Icon");
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
#endif