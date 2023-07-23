using System;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace FMODPlus
{
    [AddComponentMenu("FMOD Studio/FMOD Parameter Sender")]
    public class FMODParameterSender : MonoBehaviour
    {
        public enum AudioBehaviourStyle
        {
            Base,
            API,
        }

        public AudioBehaviourStyle BehaviourStyle;
        public FMODAudioSource Source;

        [SerializeField]
        [UsedImplicitly]
        private EventReference previewEvent;
        [ParamRef] public string Parameter;
        public float Value;

        public ParamRef[] Params = Array.Empty<ParamRef>();

        public bool SendOnStart;
        public bool IsGlobalParameter;

        public UnityEvent<ParamRef[]> OnSend;

        private void Start()
        {
            if (SendOnStart)
                SendValue();
        }

        /// <summary>
        /// 파라미터를 체인지합니다.
        /// </summary>
        public void SendValue()
        {
            if (!IsGlobalParameter)
            {
                switch (BehaviourStyle)
                {
                    case AudioBehaviourStyle.Base:
                        Source.ApplyParameter(Params);
                        break;
                    case AudioBehaviourStyle.API:
                        OnSend?.Invoke(Params);
                        break;
                }
            }
            else
                RuntimeManager.StudioSystem.setParameterByName(Parameter, Value);
        }
    }
}