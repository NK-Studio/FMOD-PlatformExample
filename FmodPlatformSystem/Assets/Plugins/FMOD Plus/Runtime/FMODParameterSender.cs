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

        private FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        public FMOD.Studio.PARAMETER_DESCRIPTION ParameterDescription { get { return parameterDescription; } }
        
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
                TriggerParameters();
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
                    result = RuntimeManager.StudioSystem.getParameterDescriptionByName(Parameter, out parameterDescription);
                    if (result != FMOD.RESULT.OK)
                    {
                        RuntimeUtils.DebugLogError(string.Format(("[FMOD] FMOD Parameter Sender failed to lookup parameter {0} : result = {1}"), Parameter, result));
                        return;
                    }
                }

                result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
                if (result != FMOD.RESULT.OK)
                {
                    RuntimeUtils.DebugLogError(string.Format(("[FMOD] FMOD Parameter Sender failed to set parameter {0} : result = {1}"), Parameter, result));
                }
            }
        }
    }
}