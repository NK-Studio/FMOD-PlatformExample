using System;
using FMODUnity;
using FMODPlus;
using Managers;
using UnityEngine;

namespace Dummy
{
    public class CommandSenderLink : MonoBehaviour
    {
        private CommandSender _sender;
        
        private void Awake()
        {
            TryGetComponent(out _sender);
            _sender.audioSource = AudioManager.Instance.BgmAudioSource;
        }

        public void Play(EventRefCallback eventRefCallback)
        {
            if (eventRefCallback.TryGetClip(out EventReference clip))
            {
                AudioManager.Instance.BgmAudioSource.clip = clip;
                AudioManager.Instance.BgmAudioSource.Play();
                AudioManager.Instance.BgmAudioSource.ApplyParameter(eventRefCallback.Params);
            }
        }

        public void Stop(bool fade)
        {
            AudioManager.Instance.BgmAudioSource.Stop(fade);
        }

        public void ChangeParameter(ParamRef[] paramRefs)
        {
            AudioManager.Instance.BgmAudioSource.ApplyParameter(paramRefs);
        }

        public void KeyOff()
        {
            AudioManager.Instance.BgmAudioSource.KeyOff();
        }
    }
}