using System;
using GameplayIngredients;
using Managers;
using UnityEngine;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/Parameter Sender")]
    public class FMODParameterSender : MonoBehaviour
    {
        public bool UseBGMAPI; // Delete Target
        public FMODAudioSource Source;
        public string ParameterName;
        public float Value;

        public bool SendOnStart;

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
            // DeleteTarget
            if (UseBGMAPI)
            {
                var audioSource = Manager.Get<AudioManager>().BgmAudioSource;
                foreach (var paramRef in audioSource.Params)
                {
                    if (paramRef.Name == ParameterName)
                    {
                        paramRef.Value = Value;
                        break;
                    }
                }

                audioSource.SetParameter(ParameterName, Value); // DeleteTarget
            }
            else
            {
                foreach (var paramRef in Source.Params)
                {
                    if (paramRef.Name == ParameterName)
                    {
                        paramRef.Value = Value;
                        break;
                    }
                }

                Source.SetParameter(ParameterName, Value);
            }
        }
    }
}