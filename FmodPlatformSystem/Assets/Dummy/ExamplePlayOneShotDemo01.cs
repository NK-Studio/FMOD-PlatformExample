using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ExamplePlayOneShotDemo01 : MonoBehaviour
{
    public FMODAudioSource AudioSource;
    public EventReference Clip;
    
    public void TestPlay()
    {
        AudioSource.PlayOneShot(Clip);
    }
}
