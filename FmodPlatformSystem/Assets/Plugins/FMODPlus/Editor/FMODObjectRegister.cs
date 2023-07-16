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
        }

        [MenuItem("GameObject/Audio/FMOD/Command Sender", priority = 6)]
        private static void FMODAudioCommandSenderCreate()
        {
            var obj = new GameObject("Command Sender");
            obj.AddComponent<EventCommandSender>();
            EditorUtility.SetDirty(obj);
        }

        [MenuItem("GameObject/Audio/FMOD/Parameter Sender", priority = 7)]
        private static void FMODParameterSenderCreate()
        {
            var obj = new GameObject("Parameter Sender");
            obj.AddComponent<FMODParameterSender>();
            EditorUtility.SetDirty(obj);
        }

        [MenuItem("FMOD/FMOD Plus/Key List")]
        private static void MoveKeyList()
        {
            Selection.activeObject = KeyList.Instance;
        }
    }
}
#endif