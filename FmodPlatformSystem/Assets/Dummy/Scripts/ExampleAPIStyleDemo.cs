using FMODUnity;
using FMODPlus;
using AutoManager;
using Managers;
using UnityEngine;

namespace Dummy
{
    public class ExampleAPIStyleDemo : MonoBehaviour
    {
        private AudioManager AudioManager => Manager.Get<AudioManager>();
        private FMODAudioSource audioSource => AudioManager.BgmAudioSource;

        public void PlayWithKeyStyle(EventRefOrKeyCallback eventRefOrKeyCallback)
        {
            if (eventRefOrKeyCallback.TryGetClipKey(out string key))
                if (AudioManager.RegisterEvent.TryFindClipAndParams(key, out EventReference clip,
                        out ParamRef[] paramRefs))
                {
                    audioSource.Clip = clip;
                    audioSource.Play();
                    audioSource.ApplyParameter(paramRefs);
                }
        }

        public void PlayWithEventReferenceStyle(EventRefOrKeyCallback eventRefOrKeyCallback)
        {
            if (eventRefOrKeyCallback.TryGetClip(out EventReference clip))
            {
                audioSource.Clip = clip;
                audioSource.Play();
                audioSource.ApplyParameter(eventRefOrKeyCallback.Params);
            }
        }

        public void Stop(bool fade)
        {
            audioSource.Stop(fade);
        }

        public void ChangeParameter(string parameterName, float value)
        {
            foreach (var paramRef in audioSource.Params)
            {
                if (paramRef.Name == parameterName)
                {
                    paramRef.Value = value;
                    break;
                }
            }

            audioSource.SetParameter(parameterName, value);
        }

        public void KeyOff()
        {
            AudioManager.KeyOff();
        }
    }
}