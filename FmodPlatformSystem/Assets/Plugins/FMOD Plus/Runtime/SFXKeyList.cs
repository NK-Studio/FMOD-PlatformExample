#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace FMODPlus
{
    public class SFXKeyList : ScriptableObject
    {
        private const string AssetsToPluginsPath = "Assets/Plugins";
        private const string AssetsToFMODPlusPath = "Assets/Plugins/FMOD Plus";
        private const string AssetsToResourcePath = "Assets/Plugins/FMOD Plus/Resources";
        private const string KeyListFilePath = "Assets/Plugins/FMOD Plus/Resources/SFX-KeyList.asset";

        public AudioPathByString Clips;

        [SerializeField, UsedImplicitly] private List<EventReferenceByKey> _cachedSearchClips;

        private static SFXKeyList _instance;

        public static SFXKeyList Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = Resources.Load<SFXKeyList>("SFX-KeyList");

#if UNITY_EDITOR
                if (_instance == null)
                {
                    if (!AssetDatabase.IsValidFolder(AssetsToPluginsPath))
                        AssetDatabase.CreateFolder("Assets", "Plugins");

                    if (!AssetDatabase.IsValidFolder(AssetsToFMODPlusPath))
                        AssetDatabase.CreateFolder(AssetsToPluginsPath, "FMOD Plus");

                    if (!AssetDatabase.IsValidFolder(AssetsToResourcePath))
                        AssetDatabase.CreateFolder(AssetsToFMODPlusPath, "Resources");

                    _instance = AssetDatabase.LoadAssetAtPath<SFXKeyList>(KeyListFilePath);

                    if (_instance == null)
                    {
                        _instance = CreateInstance<SFXKeyList>();
                        AssetDatabase.CreateAsset(_instance, KeyListFilePath);
                    }
                }
#endif

                return _instance;
            }
        }

#if UNITY_EDITOR
        public EditorEventRef GetEventRef(string key)
        {
            List<EventReferenceByKey> clipList = Clips.GetList();

            foreach (EventReferenceByKey referenceByKey in clipList)
                if (referenceByKey.Key == key)
                {
                    EventReference eventRef = referenceByKey.Value;
                    return EventManager.EventFromPath(eventRef.Path);
                }

            return null;
        }
#endif

        /// <summary>
        /// Add clips.
        /// </summary>
        /// <param name="key">It is used to find the path through the key.</param>
        /// <param name="path">Event Path</param>
        public void Add(string key, string path)
        {
            Clips.Add(key, path);
        }

        /// <summary>
        /// Remove clips.
        /// </summary>
        /// <param name="key">Removes an element via its key.</param>
        public void Remove(string key)
        {
            EventReferenceByKey eventRef = Clips.GetEventRef(key);
            Clips.Remove(eventRef);
        }

        /// <summary>
        /// Find EventReference and ParamRef via Key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clip"></param>
        /// <param name="paramRefs"></param>
        /// <returns></returns>
        public bool TryFindClipAndParams(string key, out EventReference clip, out ParamRef[] paramRefs)
        {
            if (Clips.TryGetValue(key, out EventReference eventReference))
                if (Clips.TryGetParamRef(key, out ParamRef[] parameters))
                {
                    clip = eventReference;
                    paramRefs = parameters;
                    return true;
                }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";

#if UNITY_EDITOR
            Debug.LogError(msg);
#endif

            clip = default;
            paramRefs = default;
            return false;
        }
    }
}