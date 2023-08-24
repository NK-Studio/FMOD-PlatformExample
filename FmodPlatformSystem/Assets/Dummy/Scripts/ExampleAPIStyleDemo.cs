using FMODUnity;
using FMODPlus;
using Managers;
using UnityEngine;

namespace Dummy
{
    public class ExampleAPIStyleDemo : MonoBehaviour
    {
        private AudioManager Manager => AutoManager.Manager.Get<AudioManager>();
        private FMODAudioSource Source => Manager.BgmAudioSource;
        
        public void Play(EventRefCallback eventRefCallback)
        {
            if (eventRefCallback.TryGetClip(out EventReference clip))
            {
                Source.Clip = clip;
                Source.Play();
                Source.ApplyParameter(eventRefCallback.Params);
            }
        }

        public void Stop(bool fade)
        {
            Source.Stop(fade);
        }

        public void ChangeParameter(ParamRef[] paramRefs)
        {
            Source.ApplyParameter(paramRefs);
        }

        public void KeyOff()
        {
            Manager.KeyOff();
        }
    }
}