using FMODUnity;
using FMODUtility;
using AutoManager;
using Managers;
using UnityEngine;

namespace Dummy
{
    public class ExampleAPIStyleDemo : MonoBehaviour
    {
        private AudioManager AudioManager => Manager.Get<AudioManager>(); // DeleteTarget

        public void PlayWithKeyStyle(EventReferenceOrKey eventReferenceOrKey)
        {
            if (eventReferenceOrKey.TryGetClipKey(out string key))
                if (AudioManager.RegisterEvent.TryFindClip(key, out EventReference clip))
                {
                    AudioManager.BgmAudioSource.Clip = clip;
                    AudioManager.BgmAudioSource.Play();
                }

            // Another Way
            // if (eventReferenceOrKey.TryGetClip(out EventReference clip))
            // {
            //     AudioManager.BgmAudioSource.Clip = clip;
            //     AudioManager.BgmAudioSource.Play();
            // }
        }

        public void Stop(bool fade)
        {
            AudioManager.BgmAudioSource.Stop(fade);
        }

        public void ChangeParameter(string parameterName, float Value)
        {
            var audioSource = Manager.Get<AudioManager>().BgmAudioSource;
            foreach (var paramRef in audioSource.Params)
            {
                if (paramRef.Name == parameterName)
                {
                    paramRef.Value = Value;
                    break;
                }
            }

            audioSource.SetParameter(parameterName, Value);
        }
    }
}