﻿using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace FMODPlus
{
    public enum AudioType
    {
        AMB,
        BGM,
        SFX
    }

    public static class FMODPlusUtility
    {
        public const string FMODPlusDefine = "// Define FMOD Plus";
        public const string DefaultKey = "New Key";
        
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

    public class EventRefCallback
    {
        private EventReference _reference;
        private bool _fade;
        private ClipStyle _clipStyle;
        private ParamRef[] _params;

        public EventRefCallback(EventReference reference, ClipStyle clipStyle, ParamRef[] parameters)
        {
            _reference = reference;
            _clipStyle = clipStyle;
            _params = parameters;
        }

        /// <summary>
        /// Return parameters. (Only EventReference Style)
        /// </summary>
        public ParamRef[] Params => _params;

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
                    clip = _reference;
                    return true;
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

        public KeyAndPath(string key, string path, string guid)
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
        public List<EventReferenceByKey> EventRefAndKeyList = new();

        

        public void Remove(EventReferenceByKey target)
        {
            EventRefAndKeyList.Remove(target);
        }

        public void OverrideListByKey(EventReferenceByKey newValue)
        {
            for (int i = 0; i < EventRefAndKeyList.Count; i++)
            {
                if (newValue.Key != EventRefAndKeyList[i].Key) continue;
                EventRefAndKeyList[i] = newValue;
                return;
            }
        }

        public List<EventReferenceByKey> GetList()
        {
            return EventRefAndKeyList;
        }

        public int Count => EventRefAndKeyList.Count;

        public EventReferenceByKey GetEventRef(int index)
        {
            return EventRefAndKeyList[index];
        }

        public EventReferenceByKey GetEventRef(string key)
        {
            return EventRefAndKeyList.Find((x) => x.Key == key);
        }

        public string GetKey(int index)
        {
            return EventRefAndKeyList[index].Key;
        }

        public ParamRef[] GetParam(int index)
        {
            return EventRefAndKeyList[index].Params;
        }

        public void Reset()
        {
            EventRefAndKeyList.Clear();
        }

        public void Add()
        {
            var item = new EventReferenceByKey();
            int i = EventRefAndKeyList.Count(list => list.Key.Contains(FMODPlusUtility.DefaultKey));

            if (i > 0)
                item.Key = $"New Key ({i})";
            else
                item.Key = "New Key";

            EventRefAndKeyList.Add(item);
        }

        public void Add(string key, string path)
        {
            var item = new EventReferenceByKey();

            int i = 0;
            
            foreach (EventReferenceByKey eventRef in EventRefAndKeyList)
                if (eventRef.Key.Length == key.Length)
                    if (eventRef.Key.Contains(key))
                        i += 1;

            if (i > 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning(key + "키는 이미 존재합니다.");
#endif
                return;
            }

            item.Key = key;
            
#if UNITY_EDITOR
            item.Value = EventReference.Find(path);
#else
            item.Value = RuntimeManager.PathToEventReference(path);
#endif

            EventRefAndKeyList.Add(item);
        }

        public void RemoveAt(int index)
        {
            EventRefAndKeyList.RemoveAt(index);
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
            _dictionary = new Dictionary<string, EventReference>(EventRefAndKeyList.Count);
            _dictionaryParamRefs = new Dictionary<string, ParamRef[]>(EventRefAndKeyList.Count);

            foreach (var pair in EventRefAndKeyList)
            {
                _dictionary.Add(pair.Key, pair.Value);
                _dictionaryParamRefs.Add(pair.Key, pair.Params);
            }
        }
    }
}