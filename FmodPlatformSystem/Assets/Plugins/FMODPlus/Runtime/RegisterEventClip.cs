using FMODPlus;
using UnityEngine;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Register Event Clip")]
    public class RegisterEventClip : MonoBehaviour
    {
        [SerializeField] private AudioPathByString clips;

        public bool TryFindClip(string key, out EventReference clip)
        {
            if (clips.TryGetValue(key, out EventReference eventReference))
            {
                clip = eventReference;
                return true;
            }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"Key: {key}을(를) 찾을 수 없습니다."
                : $"Key: {key} is not found.";

            Debug.LogError(msg);
            clip = default;
            return false;
        }
    }
}