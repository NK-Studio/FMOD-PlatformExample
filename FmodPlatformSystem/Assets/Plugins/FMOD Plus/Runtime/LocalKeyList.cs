using System;
using FMODUnity;
using NKStudio;
using UnityEngine;

namespace FMODPlus
{
    [AddComponentMenu("FMOD Studio/FMOD Plus/FMOD Local Key List")]
    [DisallowMultipleComponent]
    public class LocalKeyList : KeyListMono
    {
        public EventRefAtKey ClipList;

        [HideInInspector, Obsolete("더 이상 사용되지 않습니다, ClipList를 사용하세요.")]
        public EventReferenceByKey[] EventRefList;

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
            for (int i = 0; i < ClipList.Length; i++)
            {
                if (ClipList.EventRefList[i].Key == key)
                {
                    eventReference = ClipList.EventRefList[i].Value;
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
            for (int i = 0; i < ClipList.Length; i++)
            {
                if (ClipList.EventRefList[i].Key == key)
                {
                    parameters = ClipList.EventRefList[i].Params;
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

        [ContextMenu("EventRef Convert")]
        private void EventRefConvert()
        {
#pragma warning disable CS0618
            // EventRefList를 ClipList로 변환합니다.
            ClipList.EventRefList = EventRefList;
#pragma warning restore CS0618
        }
    }
}
