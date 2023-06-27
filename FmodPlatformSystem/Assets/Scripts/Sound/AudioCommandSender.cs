using System;
using GameplayIngredients;
using Managers;
using UnityEngine;

namespace FMODUnity
{
    public enum AudioBehaviourStyle
    {
        Play,
        PlayOnAPI, // DeleteTarget
        Stop,
        StopOnAPI, // DeleteTarget
    }

    [AddComponentMenu("FMOD Studio/FMOD Audio Command Sender")]
    public class AudioCommandSender : MonoBehaviour
    {
        public FMODAudioSource Source;
        public EventReference Clip;
        public string Key; // DeleteTarget
        public AudioBehaviourStyle BehaviourStyle;

        public bool Fade;

        public bool SendOnStart = true;

        private AudioManager AudioManager => Manager.Get<AudioManager>(); // DeleteTarget

        private void Start()
        {
            if (!SendOnStart)
                return;

            SendCommand();
        }

        public void SendCommand()
        {
            switch (BehaviourStyle)
            {
                case AudioBehaviourStyle.Play:
                    Source.Clip = Clip;
                    Source.Play();
                    break;

                // DeleteTarget
                case AudioBehaviourStyle.PlayOnAPI:

                    if (AudioManager.Clip.TryGetValue(Key, out var clip))
                    {
                        AudioManager.BgmAudioSource.Clip = clip;
                        AudioManager.BgmAudioSource.Play();
                    }
                    else
                    {
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "해당 키로 클립을 찾지 못했습니다."
                            : "Couldn't find clip with that key.";

                        Debug.LogError(msg);
                    }

                    break;

                case AudioBehaviourStyle.Stop:
                    Source.Stop(Fade);
                    break;

                // DeleteTarget
                case AudioBehaviourStyle.StopOnAPI:
                    AudioManager.BgmAudioSource.Stop(Fade);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}