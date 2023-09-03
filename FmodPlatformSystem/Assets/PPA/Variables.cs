using System;
using BehaviorDesigner.Runtime;
using FMODPlus;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [Serializable]
    public class SharedLocalKeyList : SharedVariable<LocalKeyList>
    {
        public static implicit operator SharedLocalKeyList(LocalKeyList value)
        {
            return new SharedLocalKeyList { mValue = value };
        }
    }
}