using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagerDefaultPrefabAttribute : Attribute
    {
        public string Prefab { get; }

        public ManagerDefaultPrefabAttribute(string prefabName) => 
            Prefab = prefabName;
    }
}

