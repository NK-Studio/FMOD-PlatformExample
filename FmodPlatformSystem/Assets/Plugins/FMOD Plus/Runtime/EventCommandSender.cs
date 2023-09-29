using System;
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace FMODPlus
{
    public enum CommandBehaviourStyle
    {
        Play,
        PlayOnAPI,
        Stop,
        StopOnAPI,
        Parameter,
        ParameterOnAPI,
        GlobalParameter,
    }

    public enum ClipStyle
    {
        EventReference,
        Key
    }

    [AddComponentMenu("FMOD Studio/FMOD Event Command Sender")]
    public class EventCommandSender : MonoBehaviour
    {
        public CommandBehaviourStyle BehaviourStyle;

        public FMODAudioSource Source;

        public ClipStyle ClipStyle = ClipStyle.EventReference;

        [SerializeField] private bool useGlobalKeyList;

        [SerializeField] private LocalKeyList keyList;

        public AudioType AudioStyle = AudioType.BGM;

        [SerializeField, UsedImplicitly] private EventReference previewEvent;
        [ParamRef] public string Parameter;
        public float Value;

        public EventReference Clip;
        public ParamRef[] Params = Array.Empty<ParamRef>();

        public string Key;

        public bool Fade;

        public bool SendOnStart = true;

        public UnityEvent<EventRefCallback> OnPlaySend;
        public UnityEvent<bool> OnStopSend;
        public UnityEvent<ParamRef[]> OnParameterSend;

        private FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;

        public FMOD.Studio.PARAMETER_DESCRIPTION ParameterDescription
        {
            get { return parameterDescription; }
        }

        private void Start()
        {
            if (!SendOnStart)
                return;

            SendCommand();
        }

        /// <summary>
        /// operate the command.
        /// </summary>
        public void SendCommand()
        {
            switch (BehaviourStyle)
            {
                case CommandBehaviourStyle.Play:
                    if (ClipStyle == ClipStyle.EventReference)
                    {
                        if (Source)
                        {
#if UNITY_EDITOR
                            if (!string.IsNullOrWhiteSpace(Clip.Path))
                            {
                                EditorEventRef existEvent = EventManager.EventFromPath(Clip.Path);
                                if (existEvent != null)
                                {
#endif
                                    Source.clip = Clip;

                                    foreach (var param in Params)
                                        Source.SetParameter(param.Name, param.Value);

                                    Source.Play();
#if UNITY_EDITOR
                                }
                                else
                                    ShowEventNotFindEventManager();
                            }
                            else
                                ShowEmptyEvent();
#endif
                        }
                        else
                            ShowEmptyAudioSource();
                    }
                    else // if (ClipStyle == ClipStyle.Key)
                    {
                        bool useLocalKeyList = !useGlobalKeyList;
                        if (useLocalKeyList)
                        {
                            if (keyList)
                            {
                                if (!string.IsNullOrWhiteSpace(Key))
                                {
                                    foreach (EventReferenceByKey list in keyList.Clips.GetList())
                                        if (list.Key == Key)
                                        {
#if UNITY_EDITOR
                                            EditorEventRef existEvent = EventManager.EventFromPath(list.Value.Path);
                                            if (existEvent != null)
#endif
                                            {
                                                Source.clip = list.Value;

                                                #region 없으면 추가하고 있으면 덮어씌운다.

                                                List<ParamRef> overrideParameter = new(list.Params);

                                                foreach (ParamRef paramRef in Params)
                                                {
                                                    ParamRef hasItem =
                                                        overrideParameter.Find(x => x.Name == paramRef.Name);
                                                    if (hasItem == null)
                                                        overrideParameter.Add(paramRef);
                                                    else
                                                        hasItem.Value = paramRef.Value;
                                                }

                                                #endregion

                                                foreach (var param in overrideParameter)
                                                    Source.SetParameter(param.Name, param.Value);

                                                Source.Play();
                                            }
#if UNITY_EDITOR
                                            else
                                                ShowEventNotFindEventManager(Key);
#endif

                                            break;
                                        }
                                }
                                else
                                    ShowEmptyKey();
                            }
                            else
                                ShowNotConnectKeyList();
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(Key))
                            {
                                List<EventReferenceByKey> audioList;
                                switch (AudioStyle)
                                {
                                    case AudioType.AMB:
                                        audioList = AMBKeyList.Instance.Clips.GetList();
                                        break;
                                    case AudioType.BGM:
                                        audioList = BGMKeyList.Instance.Clips.GetList();
                                        break;
                                    case AudioType.SFX:
                                        audioList = SFXKeyList.Instance.Clips.GetList();
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                // 키 리스트에 등록한 이벤트&키를 가져온다.
                                foreach (EventReferenceByKey list in audioList)
                                    // 키가 같은 이벤트를 찾는다.
                                    if (list.Key == Key)
                                    {
#if UNITY_EDITOR
                                        EditorEventRef existEvent;
                                        switch (AudioStyle)
                                        {
                                            case AudioType.AMB:
                                                existEvent = AMBKeyList.Instance.GetEventRef(Key);
                                                break;
                                            case AudioType.BGM:
                                                existEvent = BGMKeyList.Instance.GetEventRef(Key);
                                                break;
                                            case AudioType.SFX:
                                                existEvent = SFXKeyList.Instance.GetEventRef(Key);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }

                                        if (existEvent != null)
#endif
                                        {
                                            Source.clip = list.Value;

                                            #region 없으면 추가하고 있으면 덮어씌운다.

                                            List<ParamRef> overrideParameter = new(list.Params);

                                            foreach (ParamRef paramRef in Params)
                                            {
                                                ParamRef hasItem = overrideParameter.Find(x => x.Name == paramRef.Name);
                                                if (hasItem == null)
                                                    overrideParameter.Add(paramRef);
                                                else
                                                    hasItem.Value = paramRef.Value;
                                            }

                                            #endregion

                                            foreach (var param in overrideParameter)
                                                Source.SetParameter(param.Name, param.Value);

                                            Source.Play();
                                        }
#if UNITY_EDITOR
                                        else
                                            ShowEventNotFindEventManager(Key);
#endif

                                        break;
                                    }
                            }
                            else
                                ShowEmptyKey();
                        }
                    }

                    break;
                case CommandBehaviourStyle.PlayOnAPI:
                    if (ClipStyle == ClipStyle.EventReference)
                    {
#if UNITY_EDITOR
                        if (!string.IsNullOrWhiteSpace(Clip.Path))
                        {
                            EditorEventRef existEvent = EventManager.EventFromPath(Clip.Path);
                            if (existEvent != null)
#endif
                            {
                                EventRefCallback eventRefCallback = new(Clip, ClipStyle, Params);
                                OnPlaySend?.Invoke(eventRefCallback);
                            }
#if UNITY_EDITOR
                            else
                                ShowEventNotFindEventManager();
                        }
                        else
                            ShowEmptyEvent();
#endif
                    }
                    else // if (ClipStyle == ClipStyle.Key)
                    {
                        bool useLocalKeyList = !useGlobalKeyList;
                        if (useLocalKeyList)
                        {
                            if (keyList)
                            {
                                if (!string.IsNullOrWhiteSpace(Key))
                                {
                                    foreach (EventReferenceByKey list in keyList.Clips.GetList())
                                        if (list.Key == Key)
                                        {
#if UNITY_EDITOR
                                            EditorEventRef existEvent = EventManager.EventFromPath(list.Value.Path);
                                            if (existEvent != null)
#endif
                                            {
                                                #region 없으면 추가하고 있으면 덮어씌운다.

                                                ParamRef[] overrideParameter = list.Params;

                                                foreach (ParamRef paramRef in Params)
                                                {
                                                    ParamRef hasItem = Array.Find(overrideParameter,
                                                        x => x.Name == paramRef.Name);

                                                    if (hasItem == null)
                                                    {
                                                        Array.Resize(ref overrideParameter,
                                                            overrideParameter.Length + 1);
                                                        overrideParameter[overrideParameter.Length - 1] = paramRef;
                                                    }
                                                    else
                                                        hasItem.Value = paramRef.Value;
                                                }

                                                #endregion

                                                EventRefCallback eventRefCallback = new(list.Value, ClipStyle,
                                                    overrideParameter);
                                                OnPlaySend?.Invoke(eventRefCallback);
                                            }
#if UNITY_EDITOR
                                            else
                                                ShowEventNotFindEventManager(Key);
#endif
                                            break;
                                        }
                                }
                                else
                                    ShowEmptyKey();
                            }
                            else
                                ShowNotConnectKeyList();
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(Key))
                            {
                                List<EventReferenceByKey> audioList;
                                switch (AudioStyle)
                                {
                                    case AudioType.AMB:
                                        audioList = AMBKeyList.Instance.Clips.GetList();
                                        break;
                                    case AudioType.BGM:
                                        audioList = BGMKeyList.Instance.Clips.GetList();
                                        break;
                                    case AudioType.SFX:
                                        audioList = SFXKeyList.Instance.Clips.GetList();
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                foreach (EventReferenceByKey list in audioList)
                                    if (list.Key == Key)
                                    {
#if UNITY_EDITOR
                                        EditorEventRef existEvent;
                                        switch (AudioStyle)
                                        {
                                            case AudioType.AMB:
                                                existEvent = AMBKeyList.Instance.GetEventRef(Key);
                                                break;
                                            case AudioType.BGM:
                                                existEvent = BGMKeyList.Instance.GetEventRef(Key);
                                                break;
                                            case AudioType.SFX:
                                                existEvent = SFXKeyList.Instance.GetEventRef(Key);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }

                                        if (existEvent != null)
#endif
                                        {
                                            #region 없으면 추가하고 있으면 덮어씌운다.

                                            ParamRef[] overrideParameter = list.Params;

                                            foreach (ParamRef paramRef in Params)
                                            {
                                                ParamRef hasItem = Array.Find(overrideParameter,
                                                    x => x.Name == paramRef.Name);

                                                if (hasItem == null)
                                                {
                                                    Array.Resize(ref overrideParameter,
                                                        overrideParameter.Length + 1);
                                                    overrideParameter[overrideParameter.Length - 1] = paramRef;
                                                }
                                                else
                                                    hasItem.Value = paramRef.Value;
                                            }

                                            #endregion

                                            EventRefCallback eventRefCallback =
                                                new(list.Value, ClipStyle, overrideParameter);
                                            OnPlaySend?.Invoke(eventRefCallback);
                                        }
#if UNITY_EDITOR
                                        else
                                            ShowEventNotFindEventManager(Key);
#endif
                                        break;
                                    }
                            }
                            else
                                ShowEmptyKey();
                        }
                    }

                    break;
                case CommandBehaviourStyle.Stop:
                    if (Source)
                        Source.Stop(Fade);
                    else
                        ShowEmptyAudioSource();
                    break;
                case CommandBehaviourStyle.StopOnAPI:
                    OnStopSend?.Invoke(Fade);
                    break;
                case CommandBehaviourStyle.Parameter:
                    Source.ApplyParameter(Params);
                    break;
                case CommandBehaviourStyle.ParameterOnAPI:
                    OnParameterSend?.Invoke(Params);
                    break;
                case CommandBehaviourStyle.GlobalParameter:
                    TriggerParameters();
                    break;
            }
        }

        private void ShowEventNotFindEventManager(string key)
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 {key}로 연결된 이벤트 주소가 유효하지 않습니다."
                : $"Key is empty in Command Sender of {gameObject.name}.";
            Debug.LogError(msg);
        }

        private void ShowEventNotFindEventManager()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 연결된 이벤트 주소가 유효하지 않습니다."
                : $"Event is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void ShowKeyNotFindByKeyList()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 Key가 List에 존재하지 않습니다."
                : $"Key is not exist in List in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void ShowNotConnectKeyList()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 연결된 Key List가 없습니다."
                : $"Key List is not connected in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void ShowEmptyEvent()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 이벤트가 비어있습니다."
                : $"Event is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void ShowEmptyKey()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 연결된 Key가 비어있습니다."
                : $"Key is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void ShowEmptyAudioSource()
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 FMOD Audio Source가 없습니다."
                : $"FMOD Audio Source is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
        }

        private void TriggerParameters()
        {
            bool paramNameSpecified = !string.IsNullOrEmpty(Parameter);
            if (paramNameSpecified)
            {
                FMOD.RESULT result;
                bool paramIDNeedsLookup = string.IsNullOrEmpty(parameterDescription.name);
                if (paramIDNeedsLookup)
                {
                    result = RuntimeManager.StudioSystem.getParameterDescriptionByName(Parameter,
                        out parameterDescription);
                    if (result != FMOD.RESULT.OK)
                    {
                        RuntimeUtils.DebugLogError(string.Format(
                            ("[FMOD] FMOD Parameter Sender failed to lookup parameter {0} : result = {1}"), Parameter,
                            result));
                        return;
                    }
                }

                result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
                if (result != FMOD.RESULT.OK)
                {
                    RuntimeUtils.DebugLogError(string.Format(
                        ("[FMOD] FMOD Parameter Sender failed to set parameter {0} : result = {1}"), Parameter,
                        result));
                }
            }
        }
    }
}