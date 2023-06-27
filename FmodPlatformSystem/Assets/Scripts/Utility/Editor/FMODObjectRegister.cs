using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace FMODUnity
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

        [MenuItem("GameObject/Audio/FMOD/Audio Command Sender", priority = 6)]
        private static void FMODAudioCommandSender()
        {
            var obj = new GameObject("Audio Command Sender");
            obj.AddComponent<AudioCommandSender>();
            EditorUtility.SetDirty(obj);
        }
    }
}
