using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New PlayerCharacterSettings", menuName = "Settings/PlayerCharacterSettings",
        order = int.MaxValue)]
    public class PlayerCharacterSettings : CharacterSettings
    {
        [BoxGroup("기본"), Tooltip("피버 모드일 때 이동 속도")]
        public float FeverMoveSpeed = 4f;

        [BoxGroup("사운드"), Tooltip("점프 사운드")] public EventReference JumpClip;
        [BoxGroup("사운드"), Tooltip("착지 사운드")] public EventReference LandClip;
    }
}