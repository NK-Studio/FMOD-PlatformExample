using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class AnimationEventHandle : MonoBehaviour
    {
        public UnityEvent[] events;

        public void OnAnimationEvent(int id)
        {
            events[id]?.Invoke();
        }
    }
}
