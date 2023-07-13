using AutoManager;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Dummy
{
    [AddComponentMenu("")]
    public class ExamplePlayOneShotDemo04 : MonoBehaviour
    {
        public string ParameterName;
        public EventReference Clip;

        [Range(0, 2)] public int Value;

        public void TestPlay()
        {
            Manager.Get<AudioManager>().PlayOneShot(Clip, ParameterName, Value);
        }

        public void ChangeValue(float value)
        {
            Value = (int)value;
        }
    }
}