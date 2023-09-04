using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using FMODPlus;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
  
    
    [Serializable]
    public class SharedFMODAudioSource : SharedVariable<FMODAudioSource>
    {
        public static implicit operator SharedFMODAudioSource(FMODAudioSource value)
        {
            return new SharedFMODAudioSource { mValue = value };
        }
    }
    
    [Serializable]
    public class SharedLocalKeyList : SharedVariable<LocalKeyList>
    {
        public static implicit operator SharedLocalKeyList(LocalKeyList value)
        {
            return new SharedLocalKeyList { mValue = value };
        }
    }
}