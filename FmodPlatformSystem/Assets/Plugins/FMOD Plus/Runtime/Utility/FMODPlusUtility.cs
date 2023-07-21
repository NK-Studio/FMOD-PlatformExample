﻿using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace FMODPlus
{
    public static class FMODPlusUtility
    {
        /// <summary>
        /// Returns the length of the event's playback.
        /// </summary>
        /// <returns>Returns the length of the current Event in seconds.</returns>
        public static float Length(EventReference Clip)
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(Clip.Path))
                return 0f;
#endif
            var currentEventRef = RuntimeManager.GetEventDescription(Clip);

            if (currentEventRef.isValid())
            {
                currentEventRef.getLength(out int length);
                float convertSecond = length / 1000f;

                return convertSecond;
            }

            return 0f;
        }
    }

    public class EventRefOrKeyCallback
    {
        private EventReference _reference;
        private string _key;
        private bool _fade;
        private ClipStyle _clipStyle;
        private ParamRef[] _params;

        public EventRefOrKeyCallback(EventReference reference, string key, ClipStyle clipStyle, ParamRef[] parameters)
        {
            _reference = reference;
            _key = key;
            _clipStyle = clipStyle;
            _params = parameters;
        }

        /// <summary>
        /// Return parameters. (Only EventReference Style)
        /// </summary>
        public ParamRef[] Params
        {
            get
            {
                if (_clipStyle == ClipStyle.Key)
                {
                    Debug.LogWarning("Key Style does not return Param.");
                    return Array.Empty<ParamRef>();
                }

                return _params;
            }
        }

        /// <summary>
        /// Returns the Key according to the clip style.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool TryGetClipKey(out string clip)
        {
            switch (_clipStyle)
            {
                case ClipStyle.EventReference:
                    string msg = Application.systemLanguage == SystemLanguage.Korean
                        ? "해당 클래스의 클립 스타일은 Key입니다."
                        : "The clip style of this class is Key.";
                    Debug.LogError(msg);
                    clip = string.Empty;
                    return false;
                case ClipStyle.Key:
                    clip = _key;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns an EventReference based on the clip style.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool TryGetClip(out EventReference clip)
        {
            switch (_clipStyle)
            {
                case ClipStyle.EventReference:
                    clip = _reference;
                    return true;
                case ClipStyle.Key:
                    string msg = Application.systemLanguage == SystemLanguage.Korean
                        ? "해당 클래스의 클립 스타일은 EventReference입니다."
                        : "The clip style of this class is EventReference.";
                    Debug.LogError(msg);
                    clip = default;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    public class KeyAndPath
    {
        public string Key;
        public string Path;
        public string GUID;

        public KeyAndPath(string key, string path,string guid)
        {
            Key = key;
            Path = path;
            GUID = guid;
        }

        public override string ToString()
        {
            return $"{Key} : {Path} : {GUID}";
        }
    }
    
    [Serializable]
    public class EventReferenceByKey
    {
        public string Key;
        public EventReference Value;
        public ParamRef[] Params;
        public bool ShowInfo;
        public string GUID;

        public EventReferenceByKey()
        {
            CreateGUID();
        }

        public void CreateGUID()
        {
            GUID = Guid.NewGuid().ToString();
        }
    }

    [Serializable]
    public class AudioPathByString
    {
        [SerializeField] private List<EventReferenceByKey> list = new();

        private const string DefaultKey = "New Key";

        public void Remove(EventReferenceByKey target)
        {
            list.Remove(target);
        }
        
        public void OverrideListByKey(EventReferenceByKey newValue)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (newValue.Key != list[i].Key) continue;
                list[i] = newValue;
                return;
            }
        }

        public List<EventReferenceByKey> GetList()
        {
            return list;
        }

        public int Count => list.Count;

        public EventReferenceByKey GetEventRef(int index)
        {
            return list[index];
        }
        
        public EventReferenceByKey GetEventRef(string key)
        {
            return list.Find((x) => x.Key == key);
        }

        public string GetKey(int index)
        {
            return list[index].Key;
        }

        public ParamRef[] GetParam(int index)
        {
            return list[index].Params;
        }

        public void Reset()
        {
            list.Clear();
        }

        public void Add()
        {
            var item = new EventReferenceByKey();
            int i = list.Count(list => list.Key.Contains(DefaultKey));

            if (i > 0)
                item.Key = $"New Key ({i})";
            else
                item.Key = "New Key";

            list.Add(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public bool TryGetParamRef(string key, out ParamRef[] paramRefs)
        {
            return DictionaryParamRefs.TryGetValue(key, out paramRefs);
        }

        public bool TryGetValue(string key, out EventReference path)
        {
            return Dictionary.TryGetValue(key, out path);
        }

        private Dictionary<string, EventReference> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    InitializeAudioPathDictionary();
                }

                return _dictionary;
            }
        }

        private Dictionary<string, ParamRef[]> DictionaryParamRefs
        {
            get
            {
                if (_dictionaryParamRefs == null)
                    InitializeAudioPathDictionary();

                return _dictionaryParamRefs;
            }
        }

        private Dictionary<string, EventReference> _dictionary;
        private Dictionary<string, ParamRef[]> _dictionaryParamRefs;

        private void InitializeAudioPathDictionary()
        {
            _dictionary = new Dictionary<string, EventReference>(list.Count);
            _dictionaryParamRefs = new Dictionary<string, ParamRef[]>(list.Count);

            foreach (var pair in list)
            {
                _dictionary.Add(pair.Key, pair.Value);
                _dictionaryParamRefs.Add(pair.Key, pair.Params);
            }
        }
    }
}