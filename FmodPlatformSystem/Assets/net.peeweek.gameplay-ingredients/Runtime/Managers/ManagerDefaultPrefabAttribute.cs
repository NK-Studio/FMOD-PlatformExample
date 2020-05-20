using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagerDefaultPrefabAttribute : Attribute
    {
        public string prefab { get; }

        public ManagerDefaultPrefabAttribute(string prefabName) => 
            prefab = prefabName;
    }
}

