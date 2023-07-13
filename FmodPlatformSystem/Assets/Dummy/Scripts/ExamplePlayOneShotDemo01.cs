using FMODPlus;
using FMODUnity;
using UnityEngine;

namespace Dummy
{
    [AddComponentMenu("")]
    public class ExamplePlayOneShotDemo01 : MonoBehaviour
    {
        public FMODAudioSource AudioSource;
        public EventReference Clip;

        public void TestPlay()
        {
            AudioSource.PlayOneShot(Clip);
        }
    }
}