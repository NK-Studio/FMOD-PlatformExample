using UnityEngine;
using UnityEngine.Events;

namespace FMODUnity
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
        public string ParameterName;
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
                            if (paramRef.Name == ParameterName)
                            {
                                paramRef.Value = Value;
                                break;
                            }
                        Source.SetParameter(ParameterName, Value);
                        break;
                    case AudioBehaviourStyle.API:
                        OnSend?.Invoke(ParameterName, Value);
                        break;
                }
            }
            else
                RuntimeManager.StudioSystem.setParameterByName(ParameterName, Value);
        }
    }
}