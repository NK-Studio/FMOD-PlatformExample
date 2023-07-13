using FMODUnity;
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

        [ParamRef] public string Parameter;
        public float Value;

        public bool SendOnStart;
        public bool IsGlobalParameter;
        
        public UnityEvent<string, float> OnSend;

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
                        foreach (var paramRef in Source.Params)
                            if (paramRef.Name == Parameter)
                            {
                                paramRef.Value = Value;
                                break;
                            }

                        Source.SetParameter(Parameter, Value);
                        break;
                    case AudioBehaviourStyle.API:
                        OnSend?.Invoke(Parameter, Value);
                        break;
                }
            }
            else
                RuntimeManager.StudioSystem.setParameterByName(Parameter, Value);
        }
    }
}