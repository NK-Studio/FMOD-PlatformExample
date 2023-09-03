#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace FMODPlus
{
    public class BGMKeyList : ScriptableObject
    {
        private const string AssetsToPluginsPath = "Assets/Plugins";
        private const string AssetsToFMODPlusPath = "Assets/Plugins/FMOD Plus";
        private const string AssetsToResourcePath = "Assets/Plugins/FMOD Plus/Resources";
        private const string KeyListFilePath = "Assets/Plugins/FMOD Plus/Resources/BGM-KeyList.asset";

        public AudioPathByString Clips;

        [SerializeField, UsedImplicitly] private List<EventReferenceByKey> _cachedSearchClips;

        private static BGMKeyList _instance;

        public static BGMKeyList Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = Resources.Load<BGMKeyList>("BGM-KeyList");

#if UNITY_EDITOR
                if (_instance == null)
                {
                    if (!AssetDatabase.IsValidFolder(AssetsToPluginsPath))
                        AssetDatabase.CreateFolder("Assets", "Plugins");

                    if (!AssetDatabase.IsValidFolder(AssetsToFMODPlusPath))
                        AssetDatabase.CreateFolder(AssetsToPluginsPath, "FMOD Plus");

                    if (!AssetDatabase.IsValidFolder(AssetsToResourcePath))
                        AssetDatabase.CreateFolder(AssetsToFMODPlusPath, "Resources");

                    _instance = AssetDatabase.LoadAssetAtPath<BGMKeyList>(KeyListFilePath);

                    if (_instance == null)
                    {
                        _instance = CreateInstance<BGMKeyList>();
                        AssetDatabase.CreateAsset(_instance, KeyListFilePath);
                    }
                }
#endif

                return _instance;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns an editor event reference via key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
        /// Find EventReference via Key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public bool TryGetClip(string key, out EventReference clip)
        {
            if (Clips.TryGetValue(key, out EventReference eventReference))
            {
                clip = eventReference;
                return true;
            }
            
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";
            
            Debug.LogError(msg);
#endif

            clip = default;
            return false;
        }
        
        /// <summary>
        /// Find EventReference and ParamRef via Key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clip"></param>
        /// <param name="paramRefs"></param>
        /// <returns></returns>
        public bool TryGetClipAndParams(string key, out EventReference clip, out ParamRef[] paramRefs)
        {
            if (Clips.TryGetValue(key, out EventReference eventReference))
                if (Clips.TryGetParamRef(key, out ParamRef[] parameters))
                {
                    clip = eventReference;
                    paramRefs = parameters;
                    return true;
                }
            
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";
            
            Debug.LogError(msg);
#endif

            clip = default;
            paramRefs = default;
            return false;
        }
    }
}