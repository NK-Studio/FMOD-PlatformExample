using System;
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace FMODPlus
{
    public enum CommandBehaviourStyle
    {
        Play,
        Stop,
        KeyOff,
        Parameter,
        GlobalParameter,
    }

    public enum ClipStyle
    {
        EventReference,
        Key
    }

    [AddComponentMenu("FMOD Studio/FMOD Plus/FMOD Command Sender")]
    public class CommandSender : MonoBehaviour
    {
        public CommandBehaviourStyle BehaviourStyle;

        public FMODAudioSource audioSource;

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

        public bool SendOnAwake = true;

        private FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;

        public FMOD.Studio.PARAMETER_DESCRIPTION ParameterDescription
        {
            get { return parameterDescription; }
        }

        private void OnEnable()
        {
            if (!SendOnAwake)
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
                        if (audioSource)
                        {
#if UNITY_EDITOR
                            if (!string.IsNullOrWhiteSpace(Clip.Path))
                            {
                                EditorEventRef existEvent = EventManager.EventFromPath(Clip.Path);
                                if (existEvent != null)
                                {
#endif
                                    audioSource.clip = Clip;

                                    foreach (var param in Params)
                                        audioSource.SetParameter(param.Name, param.Value);

                                    audioSource.Play();
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
                        if (audioSource)
                        {
                            bool useLocalKeyList = !useGlobalKeyList;
                            if (useLocalKeyList)
                            {
                                if (keyList)
                                {
                                    if (!string.IsNullOrWhiteSpace(Key))
                                    {
                                        foreach (EventReferenceByKey list in keyList.ClipList.EventRefList)
                                            if (list.Key == Key)
                                            {
#if UNITY_EDITOR
                                                EditorEventRef existEvent = EventManager.EventFromPath(list.Value.Path);
                                                if (existEvent != null)
#endif
                                                {
                                                    audioSource.clip = list.Value;

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
                                                        audioSource.SetParameter(param.Name, param.Value);

                                                    audioSource.Play();
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
                                    EventReferenceByKey[] audioList;
                                    switch (AudioStyle)
                                    {
                                        case AudioType.AMB:
                                            audioList = AMBKeyList.Instance.ClipList.EventRefList;
                                            break;
                                        case AudioType.BGM:
                                            audioList = BGMKeyList.Instance.ClipList.EventRefList;
                                            break;
                                        case AudioType.SFX:
                                            audioList = SFXKeyList.Instance.ClipList.EventRefList;
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
                                                audioSource.clip = list.Value;

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
                                                    audioSource.SetParameter(param.Name, param.Value);

                                                audioSource.Play();
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
                        else
                            ShowEmptyAudioSource();
                    }

                    break;
                case CommandBehaviourStyle.Stop:
                    if (audioSource)
                        audioSource.Stop(Fade);
                    else
                        ShowEmptyAudioSource();
                    break;
                case CommandBehaviourStyle.KeyOff:
                    if (audioSource)
                        audioSource.KeyOff();
                    else
                        ShowEmptyAudioSource();
                    break;
                case CommandBehaviourStyle.Parameter:
                    if (audioSource)
                    {
                        if (ClipStyle == ClipStyle.EventReference)
                        {
                            if (audioSource.clip.Guid == Clip.Guid)
                                audioSource.ApplyParameter(Params);
                            else
                            {
#if UNITY_EDITOR
                                Debug.LogError("현재 Audio Source에 있는 이벤트 Path와 Clip의 Path가 다릅니다.");
#endif
                            }
                        }
                        else
                        {
                            if (useGlobalKeyList)
                            {
                                EventReference clip;
                                switch (AudioStyle)
                                {
                                    case AudioType.AMB:
                                        if (AMBKeyList.Instance.TryFindClip(Key, out clip))
                                            ApplyParameterFromKeyList(clip);
                                        break;
                                    case AudioType.BGM:
                                        if (BGMKeyList.Instance.TryFindClip(Key, out clip))
                                            ApplyParameterFromKeyList(clip);
                                        break;
                                    case AudioType.SFX:
                                        if (SFXKeyList.Instance.TryFindClip(Key, out clip))
                                            ApplyParameterFromKeyList(clip);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                if (keyList.TryFindClip(Key, out EventReference clip)) 
                                    ApplyParameterFromKeyList(clip);
                            }
                        }
                    }
                    else
                        ShowEmptyAudioSource();

                    break;
                case CommandBehaviourStyle.GlobalParameter:
                    TriggerParameters();
                    break;
            }
        }

        private void ApplyParameterFromKeyList(EventReference clip)
        {
            if (audioSource.clip.Guid == clip.Guid)
                audioSource.ApplyParameter(Params);
            else
            {
#if UNITY_EDITOR
                Debug.LogError("현재 Audio Source에 있는 이벤트 Path와 Local Key List의 Path가 다릅니다.");
#endif
            }
        }

        private void ShowEventNotFindEventManager(string key)
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 {key}로 연결된 이벤트 주소가 유효하지 않습니다."
                : $"Key is empty in Command Sender of {gameObject.name}.";
            Debug.LogError(msg);
#endif
        }

        private void ShowEventNotFindEventManager()
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 연결된 이벤트 주소가 유효하지 않습니다."
                : $"Event is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
#endif
        }

        private void ShowNotConnectKeyList()
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 연결된 Key List가 없습니다."
                : $"Key List is not connected in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
#endif
        }

        private void ShowEmptyEvent()
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 이벤트가 비어있습니다."
                : $"Event is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
#endif
        }

        private void ShowEmptyKey()
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에 연결된 Key가 비어있습니다."
                : $"Key is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
#endif
        }

        private void ShowEmptyAudioSource()
        {
#if UNITY_EDITOR
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"{gameObject.name}에 있는 Command Sender에서 FMOD Audio Source가 없습니다."
                : $"FMOD Audio Source is empty in Command Sender of {gameObject.name}.";

            Debug.LogError(msg);
#endif
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