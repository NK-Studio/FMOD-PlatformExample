using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace FMODPlus
{
    [CustomPropertyDrawer(typeof(ParamRefAttribute))]
    public class ParamRefDrawer : PropertyDrawer
    {
        public bool MouseDrag(Event e)
        {
            bool isDragging = e.type == EventType.DragPerform;

            return isDragging;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Texture browseIcon = EditorUtils.LoadImage("SearchIconBlack.png");

            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty pathProperty = property;

            Event e = Event.current;
            if (MouseDrag(e) && position.Contains(e.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorParamRef))
                {
                    pathProperty.stringValue = ((EditorParamRef)DragAndDrop.objectReferences[0]).Name;
                    GUI.changed = true;
                    e.Use();
                }
            }

            if (e.type == EventType.DragUpdated && position.Contains(e.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorParamRef))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    DragAndDrop.AcceptDrag();
                    e.Use();
                }
            }

            float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.top = 1;
            buttonStyle.padding.bottom = 1;
            
            Rect searchRect = new(position.x + position.width - browseIcon.width - 9, position.y, browseIcon.width + 8, baseHeight);
            Rect pathRect = new(position.x, position.y, position.width - (searchRect.width + 3), baseHeight);

            EditorGUI.PropertyField(pathRect, pathProperty, GUIContent.none);

            if (GUI.Button(searchRect, new GUIContent(browseIcon, "Search"), buttonStyle))
            {
                var eventBrowser = ScriptableObject.CreateInstance<EventBrowser>();

                eventBrowser.ChooseParameter(property);
                var windowRect = position;
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = pathRect.height + 1;
                eventBrowser.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));
            }
        }
    }
}