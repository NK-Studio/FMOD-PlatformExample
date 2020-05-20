using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdvancedHierarchyIconAttribute : Attribute
    {
        public readonly string icon;

        public AdvancedHierarchyIconAttribute() => 
            icon = "cs Script Icon";

        public AdvancedHierarchyIconAttribute(string icon) =>
            this.icon = icon;
    }
}
