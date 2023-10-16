using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace NKStudio
{
    [CustomPropertyDrawer(typeof(IMGUIDrawerType))]
    public class IMGUILink : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
    
    [CustomPropertyDrawer(typeof(UIElementsDrawerType))]
    public class EventRefAtKeyDrawer : IMGUILink
    {
        private ListView _reorderableList;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var p= property.FindPropertyRelative("EventRefList");
            return new Label("hi");
        }

        
    }
}
