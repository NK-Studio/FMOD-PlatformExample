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

        public UnityEvent<EventReferenceOrKey> OnPlaySend;
        public UnityEvent<bool, ParamRef[]> OnStopSend;

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
            EventReferenceOrKey eventReferenceOrKey = new EventReferenceOrKey(Clip, Key, ClipStyle);

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
                    OnPlaySend?.Invoke(eventReferenceOrKey);
                    break;
                case AudioBehaviourStyle.Stop:
                    if (Source)
                        Source.Stop(Fade);
                    break;
                case AudioBehaviourStyle.StopOnAPI:
                    OnStopSend?.Invoke(Fade, Params);
                    break;
            }
        }
    }
}