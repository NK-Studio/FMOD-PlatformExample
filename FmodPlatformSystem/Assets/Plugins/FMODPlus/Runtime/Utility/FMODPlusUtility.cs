using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODPlus
{
    public static class FMODEditorUtility
    {
        public static void UpdateParamsOnEmitter(SerializedObject serializedObject, string path, int type = 0)
        {
            if (string.IsNullOrEmpty(path) || EventManager.EventFromPath(path) == null)
            {
                return;
            }

            var eventRef = EventManager.EventFromPath(path);
            serializedObject.ApplyModifiedProperties();

            if (serializedObject.isEditingMultipleObjects)
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    switch (type)
                    {
                        case 0:
                            UpdateParamsOnEmitter(obj, eventRef);
                            break;
                        case 1:
                            UpdateParamsOnEmitterOnlyCommandSender(obj, eventRef);
                            break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        UpdateParamsOnEmitter(serializedObject.targetObject, eventRef);
                        break;
                    case 1:
                        UpdateParamsOnEmitterOnlyCommandSender(serializedObject.targetObject, eventRef);
                        break;
                }
            }

            serializedObject.Update();
        }

        private static void UpdateParamsOnEmitter(UnityEngine.Object obj, EditorEventRef eventRef)
        {
            var emitter = obj as FMODAudioSource;
            if (emitter == null)
            {
                // Custom game object
                return;
            }

            for (int i = 0; i < emitter.Params.Length; i++)
            {
                if (!eventRef.LocalParameters.Exists((x) => x.Name == emitter.Params[i].Name))
                {
                    int end = emitter.Params.Length - 1;
                    emitter.Params[i] = emitter.Params[end];
                    Array.Resize(ref emitter.Params, end);
                    i--;
                }
            }

            emitter.OverrideAttenuation = false;
            emitter.OverrideMinDistance = eventRef.MinDistance;
            emitter.OverrideMaxDistance = eventRef.MaxDistance;
        }

        private static void UpdateParamsOnEmitterOnlyCommandSender(UnityEngine.Object obj, EditorEventRef eventRef)
        {
            var emitter = obj as EventCommandSender;
            if (emitter == null)
            {
                // Custom game object
                return;
            }

            for (int i = 0; i < emitter.Params.Length; i++)
            {
                if (!eventRef.LocalParameters.Exists((x) => x.Name == emitter.Params[i].Name))
                {
                    int end = emitter.Params.Length - 1;
                    emitter.Params[i] = emitter.Params[end];
                    Array.Resize(ref emitter.Params, end);
                    i--;
                }
            }
        }
    }

    public static class AudioPathDictionary
    {
        public static void RegisterCallbackAll(this VisualElement ve, Action action)
        {
            ve.RegisterCallback<MouseLeaveEvent>(_ => action.Invoke());
            ve.RegisterCallback<MouseOverEvent>(_ => action.Invoke());
            ve.RegisterCallback<MouseEnterEvent>(_ => action.Invoke());
            ve.RegisterCallback<MouseMoveEvent>(_ => action.Invoke());
            ve.RegisterCallback<MouseDownEvent>(_ => action.Invoke());
            ve.RegisterCallback<MouseUpEvent>(_ => action.Invoke());
            ve.RegisterCallback<KeyDownEvent>(_ => action.Invoke());
            ve.RegisterCallback<KeyUpEvent>(_ => action.Invoke());
            ve.RegisterCallback<WheelEvent>(_ => action.Invoke());
            ve.RegisterCallback<GeometryChangedEvent>(_ => action.Invoke());
        }

        /// <summary>
        /// Returns the length of the event's playback.
        /// </summary>
        /// <returns>Returns the length of the current Event in seconds.</returns>
        public static float Length(EventReference Clip)
        {
            if (string.IsNullOrWhiteSpace(Clip.Path))
                return 0f;

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
    public struct EventReferenceByKey
    {
        public string Key;
        public EventReference Value;
        public ParamRef[] Params;
        public bool ShowInfo;
    }

    [Serializable]
    public class AudioPathByString
    {
        [SerializeField] private List<EventReferenceByKey> _list = new();

        private const string DefaultKey = "New Key";

        public ParamRef[] GetParam(int index)
        {
            return _list[index].Params;
        }

        public void Reset()
        {
            _list.Clear();
        }

        public void Add()
        {
            var item = new EventReferenceByKey();

            int i = _list.Count(list => list.Key.Contains(DefaultKey));

            if (i > 0)
                item.Key = $"New Key ({i})";
            else
                item.Key = "New Key";

            _list.Add(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
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
            _dictionary = new Dictionary<string, EventReference>(_list.Count);
            _dictionaryParamRefs = new Dictionary<string, ParamRef[]>(_list.Count);

            foreach (var pair in _list)
            {
                _dictionary.Add(pair.Key, pair.Value);
                _dictionaryParamRefs.Add(pair.Key, pair.Params);
            }
        }
    }
}