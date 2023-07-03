using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;


[AddComponentMenu("FMOD Studio/FMOD Event Trigger")]
public class FMODEventTrigger : EventHandler
{
    public FMODAudioSource Source;

    public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
    public EmitterGameEvent StopEvent = EmitterGameEvent.None;

    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (PlayEvent == gameEvent)
            Source.Play();
        
        if (StopEvent == gameEvent)
            Source.Stop();
    }
}