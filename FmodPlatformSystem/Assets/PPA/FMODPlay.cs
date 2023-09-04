using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using FMODPlus;
using FMODUnity;
using Managers;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using AudioType = FMODPlus.AudioType;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [TaskCategory("FMODPlus")]
    [TaskIcon("Assets/Gizmos/FMODPlus/FMODAudioSource.png")]
    [TaskDescription("사운드를 재생합니다.")]
    public class FMODPlay : Action
    {
        public PathStyle Style;
        public string Path;
        public bool UseGlobalKeyList;
        public AudioType AudioType;
        public SharedFMODAudioSource FMODAudioSource;
        public SharedLocalKeyList LocalKeyList;
        public string KeyName;
        
        public override TaskStatus OnUpdate()
        {
            if (FMODAudioSource.Value)
            {
                EventReference clip;
                ParamRef[] parameters;
                switch (Style)
                {
                    case PathStyle.EventReference:
                        
                        clip = RuntimeManager.PathToEventReference(Path);
#if UNITY_EDITOR
                        if (FMODEditorUtility.IsNull(clip))
                        {
                            Debug.LogError("Could not find the item in the key list");
                            return TaskStatus.Failure;
                        }
                        
                        FMODAudioSource.Value.Clip = RuntimeManager.PathToEventReference(Path);
                        FMODAudioSource.Value.Play();
                        return TaskStatus.Success;
#else
                            FMODAudioSource.Value.Clip = RuntimeManager.PathToEventReference(Path);
                            FMODAudioSource.Value.Play();
                            return TaskStatus.Success;
#endif
                    case PathStyle.Key:
                        if (UseGlobalKeyList)
                        {
                            switch (AudioType)
                            {
                                case AudioType.AMB:
                                    if (AMBKeyList.Instance.TryGetClipAndParams(KeyName, out clip, out parameters))
                                    {
                                        FMODAudioSource.Value.Clip = clip;
                                        FMODAudioSource.Value.SetParameter(parameters);
                                        FMODAudioSource.Value.Play();
                                        return TaskStatus.Success;
                                    }
#if UNITY_EDITOR
                                    Debug.LogError("Could not find the item in the key list");
#endif
                                    return TaskStatus.Failure;
                                case AudioType.BGM:
                                    if (BGMKeyList.Instance.TryGetClipAndParams(KeyName, out clip, out parameters))
                                    {
                                        FMODAudioSource.Value.Clip = clip;
                                        FMODAudioSource.Value.SetParameter(parameters);
                                        FMODAudioSource.Value.Play();
                                        return TaskStatus.Success;
                                    }
                                    Debug.LogError("Could not find the item in the key list");
                                    return TaskStatus.Failure;
                                case AudioType.SFX:
#if UNITY_EDITOR
                                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
#endif
                                    return TaskStatus.Failure;
                            }
                        }
                        else
                        {
                            if (LocalKeyList.Value.TryFindClipAndParams(KeyName, out  clip, out parameters))
                            {
                                FMODAudioSource.Value.Clip = clip;
                                FMODAudioSource.Value.SetParameter(parameters);
                                FMODAudioSource.Value.Play();
                                return TaskStatus.Success;
                            }
#if UNITY_EDITOR
                            Debug.LogError("Could not find the item in the key list");
#endif
                            return TaskStatus.Failure;
                        }
                        break;
                }
            }
            else
            {
                Debug.LogError("FMODAudioSource is null.");
                return TaskStatus.Failure;
            }
            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            KeyName = string.Empty;
        }
    }
}
