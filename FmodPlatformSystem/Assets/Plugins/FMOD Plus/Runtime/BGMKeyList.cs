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

        public EventReferenceByKey[] EventRefList;

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
            foreach (EventReferenceByKey referenceByKey in EventRefList)
                if (referenceByKey.Key == key)
                {
                    EventReference eventRef = referenceByKey.Value;
                    return EventManager.EventFromPath(eventRef.Path);
                }

            return null;
        }
#endif

        /// <summary>
        /// Find EventReference and ParamRef via Key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clip"></param>
        /// <param name="paramRefs"></param>
        /// <returns></returns>
        public bool TryFindClipAndParams(string key, out EventReference clip, out ParamRef[] paramRefs)
        {
            if (TryGetValue(key, out EventReference eventReference))
                if (TryGetParamRef(key, out ParamRef[] parameters))
                {
                    clip = eventReference;
                    paramRefs = parameters;
                    return true;
                }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";

            Debug.LogError(msg);
            clip = default;
            paramRefs = default;
            return false;
        }

        /// <summary>
        /// Key를 통해 EventReference를 찾습니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="eventReference"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out EventReference eventReference)
        {
            for (int i = 0; i < EventRefList.Length; i++)
            {
                if (EventRefList[i].Key == key)
                {
                    eventReference = EventRefList[i].Value;
                    return true;
                }
            }
            eventReference = default;
            return false;
        }

        /// <summary>
        /// key를 통해 ParamRef를 찾습니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool TryGetParamRef(string key, out ParamRef[] parameters)
        {
            for (int i = 0; i < EventRefList.Length; i++)
            {
                if (EventRefList[i].Key == key)
                {
                    parameters = EventRefList[i].Params;
                    return true;
                }
            }

            parameters = default;
            return false;
        }

        /// <summary>
        /// Key를 통해 ParamRef를 찾습니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramRefs"></param>
        /// <returns></returns>
        public bool TryFindParamRefs(string key, out ParamRef[] paramRefs)
        {
            if (TryGetParamRef(key, out ParamRef[] parameter))
            {
                paramRefs = parameter;
                return true;
            }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";

            Debug.LogError(msg);
            paramRefs = default;
            return false;
        }

        /// <summary>
        /// 키로 EventReference를 찾습니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public bool TryFindClip(string key, out EventReference clip)
        {
            if (TryGetValue(key, out EventReference eventReference))
            {
                clip = eventReference;
                return true;
            }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";

            Debug.LogError(msg);
            clip = default;
            return false;
        }
    }
}
