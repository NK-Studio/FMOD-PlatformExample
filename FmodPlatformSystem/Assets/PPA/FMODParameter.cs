using System;
using BehaviorDesigner.Runtime.Tasks;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [TaskCategory("FMODPlus")]
    [TaskIcon("Assets/Gizmos/FMODPlus/FMODParameter.png")]
    [TaskDescription("사운드의 파라미터를 변경합니다.")]
    public class FMODParameter : Action
    {
        public SharedFMODAudioSource FMODAudioSource;
        public Parameter Parameter;
        public float Value;
    }

    public class Parameter
    {
        public string ParamRef;
    }
}