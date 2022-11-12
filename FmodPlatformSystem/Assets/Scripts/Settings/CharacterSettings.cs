using NaughtyAttributes;
using UnityEngine;

namespace Settings
{
    public abstract class CharacterSettings : ScriptableObject
    {
        [BoxGroup("기본"), Tooltip("이동 속도")] public float MoveSpeed = 1f;
        [BoxGroup("기본"), Tooltip("점프 파워")] public float JumpPower = 3f;

        [BoxGroup("중력"), Tooltip("일반 중력")] public float Gravity = 1;
        [BoxGroup("중력"), Tooltip("점프시 중력")] public float JumpGravity = 1;
        [BoxGroup("중력"), Tooltip("낙하시 중력")] public float FallGravity = 2;
    }
}