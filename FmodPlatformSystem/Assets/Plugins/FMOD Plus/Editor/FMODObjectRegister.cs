#if UNITY_EDITOR
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace FMODPlus
{
    public class FMODObjectRegister : Editor
    {
        [MenuItem("GameObject/Audio/FMOD/Audio Source", priority = 5)]
        private static void FMODAudioSourceCreate()
        {
            var obj = new GameObject("Audio Source");
            obj.AddComponent<FMODAudioSource>();
            EditorUtility.SetDirty(obj);
            Selection.activeObject = obj;
        }

        [MenuItem("GameObject/Audio/FMOD/Command Sender", priority = 6)]
        private static void FMODAudioCommandSenderCreate()
        {
            var obj = new GameObject("Command Sender");
            obj.AddComponent<EventCommandSender>();
            EditorUtility.SetDirty(obj);
            Selection.activeObject = obj;
        }

        [MenuItem("GameObject/Audio/FMOD/Parameter Sender", priority = 7)]
        private static void FMODParameterSenderCreate()
        {
            var obj = new GameObject("Parameter Sender");
            obj.AddComponent<FMODParameterSender>();
            EditorUtility.SetDirty(obj);
            Selection.activeObject = obj;
        }
        
        [MenuItem("GameObject/Audio/FMOD/Key List", priority = 8)]
        private static void FMODLocalKeyListCreate()
        {
            var obj = new GameObject("Local Key List");
            obj.AddComponent<LocalKeyList>();
            EditorUtility.SetDirty(obj);
            Selection.activeObject = obj;
        }

        [MenuItem("FMOD/FMOD Plus/BGM Key List")]
        private static void MoveBGMKeyList()
        {
            Selection.activeObject = BGMKeyList.Instance;
        }
        
        [MenuItem("FMOD/FMOD Plus/AMB Key List")]
        private static void MoveAMBKeyList()
        {
            Selection.activeObject = AMBKeyList.Instance;
        }
        
        [MenuItem("FMOD/FMOD Plus/SFX Key List")]
        private static void MoveSFXKeyList()
        {
            Selection.activeObject = SFXKeyList.Instance;
        }
    }
}
#endif