using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace FMODUtility
{
    public class EventReferenceOrKey
    {
        private EventReference _reference;
        private string _key;
        private bool _fade;
        private ClipStyle _clipStyle;

        public EventReferenceOrKey(EventReference reference, string key, bool fade, ClipStyle clipStyle)
        {
            _reference = reference;
            _key = key;
            _fade = fade;
            _clipStyle = clipStyle;
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
    public struct EventReferenceByKey<TKey>
    {
        public TKey Key;
        public EventReference Value;
    }

    [Serializable]
    public class AudioPathDictionary<TKey>
    {
        [SerializeField] private List<EventReferenceByKey<TKey>> _list = new();

        public bool TryGetValue(TKey key, out EventReference path)
        {
            return Dictionary.TryGetValue(key, out path);
        }

        private Dictionary<TKey, EventReference> Dictionary
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

        private Dictionary<TKey, EventReference> _dictionary = null;

        private void InitializeAudioPathDictionary()
        {
            _dictionary = new Dictionary<TKey, EventReference>(_list.Count);
            foreach (var pair in _list)
            {
                _dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    [Serializable]
    public class AudioPathByString : AudioPathDictionary<string>
    {
    }
}