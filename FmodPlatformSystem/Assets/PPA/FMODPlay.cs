using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using FMODPlus;
using FMODUnity;
using Managers;
using NKStudio.FMODPlus.BehaviorDesigner;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using AudioType = FMODPlus.AudioType;

namespace NKStudio.FMODPlus.NMProject.BehaviorDesigner
{
    [TaskCategory("FMODPlus")]
    [TaskIcon("Assets/Gizmos/FMODPlus/FMODAudioSource.png")]
    [TaskDescription("사운드를 재생합니다.")]
    public class FMODPlay : Action
    {
        public KeyListHandler KeyListHandler;
        public SharedString KeyName;

        public override TaskStatus OnUpdate()
        {
            if (KeyListHandler.IsGlobalKeyList)
            {
                switch (KeyListHandler.AudioType)
                {
                    case AudioType.AMB:
                        break;
                    case AudioType.BGM:
                        if (BGMKeyList.Instance.TryGetClipAndParams(KeyName.Value, out EventReference clip, out ParamRef[] parameters))
                        {
                            AutoManager.Manager.Get<AudioManager>().ChangeParameter(AudioType.BGM, parameters);
                            AutoManager.Manager.Get<AudioManager>().ChangeClip(AudioType.BGM, clip);
                            AutoManager.Manager.Get<AudioManager>().Play(AudioType.BGM);
                        }
                        break;
                    case AudioType.SFX:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {

            }

            // if (KeyListHandler..TryFindClip(KeyName.Value, out EventReference clip))
            // {
            //     Debug.Log(clip.Path);
            // }
            // else
            // {
            //     Debug.LogWarning($"{KeyName.Value} : 없음");
            // }

            //AudioManager audioManager = ManagerX.AutoManager.Get<AudioManager>();
            //audioManager.PlayOneShot(clip);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            KeyName.Value = string.Empty;
        }
    }

    [System.Serializable]
    public class KeyListHandler
    {
        public bool IsGlobalKeyList;
        public AudioType AudioType;
        public SharedLocalKeyList LocalKeyList;
    }
}
