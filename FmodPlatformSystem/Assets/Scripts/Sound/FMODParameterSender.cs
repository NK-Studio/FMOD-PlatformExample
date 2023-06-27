using System;
using GameplayIngredients;
using Managers;
using UnityEngine;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Parameter Sender")]
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
            if (UseBGMAPI) // DeleteTarget
                Manager.Get<AudioManager>().BgmAudioSource.SetParameter(ParameterName, Value); // DeleteTarget
            else // DeleteTarget
                Source.SetParameter(ParameterName, Value);
        }
    }
}