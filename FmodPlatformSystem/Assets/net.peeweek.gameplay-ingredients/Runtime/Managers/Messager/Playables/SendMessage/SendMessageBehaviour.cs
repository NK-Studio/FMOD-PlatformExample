using System;
using UnityEngine;
using UnityEngine.Playables;

namespace GameplayIngredients.Playables
{
    [Serializable]
    public class SendMessageBehaviour : PlayableBehaviour
    {
        public string StartMessage;
        public GameObject Instigator;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if(StartMessage != string.Empty )
            {
                if (Application.isPlaying)
                    Messager.Send(StartMessage, Instigator);
                else
                    Debug.Log($"[SendMessageBehaviour] BroadcastMessage를 보냈습니다 : '{StartMessage}'");
            }
        }
    }
}
