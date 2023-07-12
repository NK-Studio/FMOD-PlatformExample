using System;
using System.Collections.Generic;
using FMODPlus;
using UnityEngine;
using UnityEngine.Events;

namespace FMODUnity
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
            EventRefOrKeyCallback eventRefOrKeyCallback = new EventRefOrKeyCallback(Clip, Key, ClipStyle,Params);

            switch (BehaviourStyle)
            {
                case AudioBehaviourStyle.Play:
                    if (Source)
                    {
                        Source.Clip = Clip;

                        foreach (var param in Params)
                            Source.SetParameter(param.Name, param.Value);

                        Source.Play();
                    }

                    break;
                case AudioBehaviourStyle.PlayOnAPI:
                    OnPlaySend?.Invoke(eventRefOrKeyCallback);
                    break;
                case AudioBehaviourStyle.Stop:
                    if (Source)
                        Source.Stop(Fade);
                    break;
                case AudioBehaviourStyle.StopOnAPI:
                    OnStopSend?.Invoke(Fade);
                    break;
            }
        }
    }
}