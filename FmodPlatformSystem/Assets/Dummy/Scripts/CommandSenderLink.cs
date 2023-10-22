using System;
using FMODPlus;
using Managers;
using UnityEngine;
using AudioType = FMODPlus.AudioType;

namespace Dummy
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-221)]
    public class CommandSenderLink : MonoBehaviour
    {
        private CommandSender[] _senders;
        
        [Tooltip("Audio Manager에 있는 오디오 타입을 선택합니다.")]
        public AudioType AudioType = AudioType.BGM;

        private void Awake()
        {
            _senders = GetComponentsInChildren<CommandSender>();

            foreach (CommandSender sender in _senders)
            {
                switch (AudioType)
                {
                    case AudioType.AMB:
                        sender.audioSource = AudioManager.Instance.AMBAudioSource;
                        break;
                    case AudioType.BGM:
                        sender.audioSource = AudioManager.Instance.BGMAudioSource;
                        break;
                    case AudioType.SFX:
                        sender.audioSource = AudioManager.Instance.SFXAudioSource;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                } 
            }
        }

        public void KeyOff()
        {
            foreach (CommandSender sender in _senders)
            {
                switch (AudioType)
                {
                    case AudioType.AMB:
                        AudioManager.Instance.AMBAudioSource.KeyOff();
                        break;
                    case AudioType.BGM:
                        AudioManager.Instance.BGMAudioSource.KeyOff();
                        break;
                    case AudioType.SFX:
                        AudioManager.Instance.SFXAudioSource.KeyOff();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
    }
}