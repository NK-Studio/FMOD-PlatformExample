using AutoManager;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Dummy
{
    [AddComponentMenu("")]
    public class ExamplePlayOneShotDemo03 : MonoBehaviour
    {
        public EventReference Clip;

        public void TestPlay()
        {
            Manager.Get<AudioManager>().PlayOneShot(Clip);
        }
    }
}