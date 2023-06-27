using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace FMODUtility
{
    [Serializable]
    public struct EventReferenceByKey<TKey>
    {
        public TKey Key;
        public EventReference Value;
    }
    
    [Serializable]
    public class AudioPathDictionary<TKey>
    {
        [SerializeField]
        private List<EventReferenceByKey<TKey>> _list = new();

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