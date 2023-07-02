using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace AutoManager.Editor
{
    [CustomPropertyDrawer(typeof(TypeDropDownAttribute))]
    public class TypeDropDownPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, List<string>> assignableTypeNames;

        private Type type;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(type == null)
                type = ((TypeDropDownAttribute)attribute).BaseType;

            CacheType(type);
            string typeName = type.FullName;

            int index = assignableTypeNames[typeName].IndexOf(property.stringValue);
            
            EditorGUI.BeginChangeCheck();
            
            int newVal = EditorGUI.Popup(position, index, assignableTypeNames[typeName].ToArray());
            
            if(EditorGUI.EndChangeCheck() && index != newVal)
                property.stringValue = assignableTypeNames[typeName][newVal];
        }

        private void CacheType(Type baseType)
        {
            if (assignableTypeNames == null)
            {
                assignableTypeNames = new Dictionary<string, List<string>>();

                string key = baseType.FullName;

                if (!assignableTypeNames.ContainsKey(key))
                    assignableTypeNames.Add(key, new List<string>());

                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach(Type type in assembly.GetTypes())
                        if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                            assignableTypeNames[key].Add(type.Name);
                }
            }
        }
    }

}
