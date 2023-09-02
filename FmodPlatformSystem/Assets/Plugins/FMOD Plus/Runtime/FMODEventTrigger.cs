using System;
using FMODUnity;
using UnityEngine;
using EventHandler = FMODUnity.EventHandler;

namespace FMODPlus
{
    [AddComponentMenu("FMOD Studio/FMOD Event Trigger")]
    public class FMODEventTrigger : EventHandler
    {
        private enum TriggerType
        {
            AudioSource,
            CommandSender
        }
        
        [SerializeField]
        private TriggerType triggerType;
        
        [SerializeField]
        private FMODAudioSource source;
        
        [SerializeField]
        private EventCommandSender commandSender;

        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                if (gameEvent != EmitterGameEvent.ObjectStart)
                {
                    switch (triggerType)
                    {
                        case TriggerType.AudioSource:
                            source.Play();
                            break;
                        case TriggerType.CommandSender:
                            commandSender.SendCommand();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (StopEvent == gameEvent)
            {
                if (gameEvent != EmitterGameEvent.ObjectDestroy) 
                    source.Stop();
            }
        }
    }
}