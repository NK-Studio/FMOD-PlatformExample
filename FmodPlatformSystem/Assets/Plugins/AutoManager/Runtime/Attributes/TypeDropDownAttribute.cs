using System;
using UnityEngine;

namespace AutoManager
{
    public class TypeDropDownAttribute : PropertyAttribute
    {
        public readonly Type BaseType;

        public TypeDropDownAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
