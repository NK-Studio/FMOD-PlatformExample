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

            string guid = AssetDatabase.GUIDToAssetPath("86c6556701af9e04380698b89f691b6e");
            var studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(guid);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Parameter Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("74cfbd073c7464035ba232171ef31f0f");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("6531cd3743c664274b21aa41c9b00c5c");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string guid = AssetDatabase.GUIDToAssetPath("0842e81344c5e4019b977bcb20b7266b");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(guid);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Event Command Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);
            
            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);
            
            string guid = AssetDatabase.GUIDToAssetPath("1fa131d2be3348a8bd8940d0037f6bfb");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(guid);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Audio Source
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a10e3143b46034572a9a5e38034e181c");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("ae854481e58584815befcbf213b32745");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string guid = AssetDatabase.GUIDToAssetPath("acaec83597e9f4b2d86d2a9bf8aa80c2");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(guid);
            ApplyIcon(darkIcon, whiteIcon, studioListener);
        }
        {
            // Audio Source
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("e9081b0172b4f4735ac3dc549b17a6b8");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("2be0f2ec703f2444b8795748b7ffcee3");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string guid = AssetDatabase.GUIDToAssetPath("7da6d1fde29614e3e84019cd9e052acd");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(guid);
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