using FMODPlus;
using FMODUnity;
using UnityEngine;

namespace Dummy
{
    [AddComponentMenu("")]
    public class ExamplePlayOneShotDemo02 : MonoBehaviour
    {
        public FMODAudioSource AudioSource;
        public EventReference Clip;

        public string ParameterName;
        [Range(0, 2)] public int Value;

        public void TestPlay()
        {
            AudioSource.PlayOneShot(Clip, ParameterName, Value);
        }

        public void ChangeValue(float value)
        {
            Value = (int)value;
        }
    }
}