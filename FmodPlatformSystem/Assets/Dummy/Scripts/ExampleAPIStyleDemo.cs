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

        public void PlayWithKeyStyle(EventRefOrKeyCallback eventRefOrKeyCallback)
        {
            if (eventRefOrKeyCallback.TryGetClipKey(out string key))
                if (KeyList.Instance.TryFindClipAndParams(key, out EventReference clip,
                        out ParamRef[] paramRefs))
                {
                    Source.Clip = clip;
                    Source.Play();
                    Source.ApplyParameter(paramRefs);
                }
        }

        public void PlayWithEventReferenceStyle(EventRefOrKeyCallback eventRefOrKeyCallback)
        {
            if (eventRefOrKeyCallback.TryGetClip(out EventReference clip))
            {
                Source.Clip = clip;
                Source.Play();
                Source.ApplyParameter(eventRefOrKeyCallback.Params);
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