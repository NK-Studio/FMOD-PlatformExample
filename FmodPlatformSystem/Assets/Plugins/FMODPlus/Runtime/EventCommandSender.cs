using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace FMODPlus
{
    public enum AudioBehaviourStyle
    {
        Play,
        PlayOnAPI,
        Stop,
        StopOnAPI
    }

    public enum ClipStyle
    {
        EventReference,
        Key
    }

    [AddComponentMenu("FMOD Studio/FMOD Event Command Sender")]
    public class EventCommandSender : MonoBehaviour
    {
        public FMODAudioSource Source;

        public ClipStyle ClipStyle = ClipStyle.EventReference;

        [SerializeField] private bool UseGlobalKeyList;

        [SerializeField] private LocalKeyList keyList;

        public EventReference Clip;
        public ParamRef[] Params = Array.Empty<ParamRef>();

        public string Key;
        public AudioBehaviourStyle BehaviourStyle;

        public bool Fade;

        public bool SendOnStart = true;

        public UnityEvent<EventRefOrKeyCallback> OnPlaySend;
        public UnityEvent<bool> OnStopSend;

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
            EventRefOrKeyCallback eventRefOrKeyCallback = new(Clip, Key, ClipStyle, Params);

            switch (BehaviourStyle)
            {
                case AudioBehaviourStyle.Play:
                    if (ClipStyle == ClipStyle.EventReference)
                    {
                        if (Source)
                        {
                            if (!string.IsNullOrWhiteSpace(Clip.Path))
                            {
                                EditorEventRef existEvent = EventManager.EventFromPath(Clip.Path);
                                if (existEvent != null)
                                {
                                    Source.Clip = Clip;

                                    foreach (var param in Params)
                                        Source.SetParameter(param.Name, param.Value);

                                    Source.Play();
                                }
                                else
                                    ShowEventNotFindEventManager();
                            }
                            else
                                ShowEmptyEvent();
                        }
                        else
                            ShowEmptyAudioSource();
                    }
                    else // if (ClipStyle == ClipStyle.Key)
                    {
                        bool useLocalKeyList = !UseGlobalKeyList;
                        if (useLocalKeyList)
                        {
                            if (keyList)
                            {
                                if (!string.IsNullOrWhiteSpace(Key))
                                {
                                    foreach (EventReferenceByKey list in keyList.Clips.GetList())
                                        if (list.Key == Key)
                                        {
                                            EditorEventRef existEvent = EventManager.EventFromPath(list.Value.Path);
                                            if (existEvent != null)
                                            {
                                                Source.Clip = list.Value;

                                                foreach (var param in Params)
                                                    Source.SetParameter(param.Name, param.Value);

                                                Source.Play();
                                            }
                                            else
                                                ShowEventNotFindEventManager(Key);
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
                                foreach (EventReferenceByKey list in KeyList.Instance.Clips.GetList())
                                    if (list.Key == Key)
                                    {
                                        EditorEventRef existEvent = KeyList.Instance.GetEventRef(Key);
                                        if (existEvent != null)
                                        {
                                            Source.Clip = list.Value;

                                            foreach (var param in Params)
                                                Source.SetParameter(param.Name, param.Value);

                                            Source.Play();
                                        }
                                        else
                                            ShowEventNotFindEventManager(Key);
                                        break;
                                    }
                            }
                            else
                                ShowEmptyKey();
                        }
                    }

                    break;
                case AudioBehaviourStyle.PlayOnAPI:
                    if (ClipStyle == ClipStyle.EventReference)
                    {
                        if (!string.IsNullOrWhiteSpace(Clip.Path))
                        {
                            EditorEventRef existEvent = EventManager.EventFromPath(Clip.Path);
                            if (existEvent != null)
                                OnPlaySend?.Invoke(eventRefOrKeyCallback);
                            else
                                ShowEventNotFindEventManager();
                        }
                        else
                            ShowEmptyEvent();
                    }
                    else // if (ClipStyle == ClipStyle.Key)
                    {
                        bool useLocalKeyList = !UseGlobalKeyList;
                        if (useLocalKeyList)
                        {
                            if (keyList)
                            {
                                if (!string.IsNullOrWhiteSpace(Key))
                                {
                                    foreach (EventReferenceByKey list in keyList.Clips.GetList())
                                        if (list.Key == Key)
                                        {
                                            EditorEventRef existEvent = EventManager.EventFromPath(list.Value.Path);
                                            if (existEvent != null)
                                                OnPlaySend?.Invoke(eventRefOrKeyCallback);
                                            else
                                                ShowEventNotFindEventManager(Key);
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
                            EditorEventRef existEvent = KeyList.Instance.GetEventRef(Key);
                            if (existEvent != null)
                            {
                            }
                            else
                                ShowKeyNotFindByKeyList();
                        }
                    }

                    break;
                case AudioBehaviourStyle.Stop:
                    if (Source)
                        Source.Stop(Fade);
                    else
                        ShowEmptyAudioSource();
                    break;
                case AudioBehaviourStyle.StopOnAPI:
                    OnStopSend?.Invoke(Fade);
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
    }
}