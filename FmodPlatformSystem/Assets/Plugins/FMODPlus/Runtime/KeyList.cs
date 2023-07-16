#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using NKStudio;
using UnityEngine;

namespace FMODPlus
{
    public class KeyList : ScriptableObject
    {
        private const string KeyListDirectory = "Assets/Plugins/FMODPlus/Resources";
        private const string KeyListFilePath = "Assets/Plugins/FMODPlus/Resources/KeyList.asset";

        public AudioPathByString Clips;

        [SerializeField] [UsedImplicitly] private List<EventReferenceByKey> cachedSearchClips;

        private static KeyList _instance;

        public static KeyList Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = Resources.Load<KeyList>("KeyList");

#if UNITY_EDITOR
                if (_instance == null)
                {
                    if (!AssetDatabase.IsValidFolder(KeyListDirectory))
                    {
                        AssetDatabase.CreateFolder("Assets/Plugins/FMODPlus", "Resources");
                    }

                    _instance = AssetDatabase.LoadAssetAtPath<KeyList>(KeyListFilePath);

                    if (_instance == null)
                    {
                        _instance = CreateInstance<KeyList>();
                        AssetDatabase.CreateAsset(_instance, KeyListFilePath);
                    }
                }
#endif

                return _instance;
            }
        }

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
    }
}