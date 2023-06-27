using System;
using GameplayIngredients;
using Managers;
using UnityEngine;

namespace FMODUnity
{
    public enum EAudioBehaviour
    {
        Play,
        Stop
    }
    
    public class AudioSelecor : MonoBehaviour
    {
        private AudioManager _audioManager => Manager.Get<AudioManager>();

        [Tooltip("Audio Manager에 추가된 클립의 이름을 Key로 사용합니다.")]
        public string Key;

        public bool Fade;

        public EAudioBehaviour Behaviour;

        private void Start()
        {
            switch (Behaviour)
            {
                case EAudioBehaviour.Play:
                    if (_audioManager.Clip.TryGetValue(Key, out var clip))
                    {
                        _audioManager.BgmAudioSource.Clip = clip;
//                            _audioManager.BgmAudioSource.ChangeEvent(clip,Fade);
                        _audioManager.BgmAudioSource.Play();
                    }
                    break;
                case EAudioBehaviour.Stop:
                    _audioManager.BgmAudioSource.AllowFadeout = Fade;
                    _audioManager.BgmAudioSource.Stop();
                    break;
            }
        }
    }
}