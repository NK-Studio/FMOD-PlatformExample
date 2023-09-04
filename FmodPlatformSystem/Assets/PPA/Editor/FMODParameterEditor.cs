using System.Reflection;
using BehaviorDesigner.Editor;
using FMODPlus;
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    // [CustomObjectDrawer(typeof(FMODParameter))]
    // public class FMODParameterEditor : ObjectDrawer
    // {
    //     public override void OnGUI(GUIContent label)
    //     {
    //         GUIStyle boldLabelStyle = new(EditorStyles.label);
    //         boldLabelStyle.fontStyle = FontStyle.Bold;
    //
    //         if (task is FMODParameter fmodParameter)
    //         {
    //             EditorGUILayout.LabelField("Setting", boldLabelStyle);
    //
    //             fmodParameter.FMODAudioSource = (SharedFMODAudioSource)FieldInspector.DrawSharedVariable(fmodParameter,
    //                 new GUIContent("FMOD Audio Source"),
    //                 fmodParameter.GetType().GetField("FMODAudioSource"),
    //                 typeof(SharedFMODAudioSource),
    //                 fmodParameter.FMODAudioSource);
    //
    //             EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    //             
    //             // fmodParameter.Parameter = (string)FieldInspector.DrawField(fmodParameter,
    //             //     new GUIContent("Para"),
    //             //     fmodParameter.GetType().GetField("Parameter", BindingFlags.Instance | BindingFlags.Public),
    //             //     fmodParameter.Parameter);
    //         }
    //     }
    // }

    [CustomObjectDrawer(typeof(Parameter))]
    public class ParamRefDrawer : ObjectDrawer
    {
        public bool MouseDrag(Event e)
        {
            bool isDragging = e.type == EventType.DragPerform;

            return isDragging;
        }

        public override void OnGUI(GUIContent label)
        {
            Texture browseIcon = EditorUtils.LoadImage("SearchIconBlack.png");
            // string pathProperty = value.GetType()
            EditorGUILayout.BeginVertical();
            Event e = Event.current;
            var position = EditorGUILayout.GetControlRect();
            var parameter = value as Parameter;

            // if (MouseDrag(e) && position.Contains(e.mousePosition))
            // {
            //     if (DragAndDrop.objectReferences.Length > 0 &&
            //         DragAndDrop.objectReferences[0] != null &&
            //         DragAndDrop.objectReferences[0].GetType() == typeof(EditorParamRef))
            //     {
            //         parameter.ParamRef = ((EditorParamRef)DragAndDrop.objectReferences[0]).Name;
            //         value = ((EditorParamRef)DragAndDrop.objectReferences[0]).Name;
            //         GUI.changed = true;
            //         e.Use();
            //     }
            // }

            // if (e.type == EventType.DragUpdated && position.Contains(e.mousePosition))
            // {
            //     if (DragAndDrop.objectReferences.Length > 0 &&
            //         DragAndDrop.objectReferences[0] != null &&
            //         DragAndDrop.objectReferences[0].GetType() == typeof(EditorParamRef))
            //     {
            //         DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            //         DragAndDrop.AcceptDrag();
            //         e.Use();
            //     }
            // }

            float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.top = 1;
            buttonStyle.padding.bottom = 1;

            Rect searchRect = new(position.x + position.width - browseIcon.width - 9, position.y, browseIcon.width + 8,
                baseHeight);
            Rect pathRect = new(position.x, position.y, position.width - (searchRect.width + 3), baseHeight);

            parameter.ParamRef = EditorGUILayout.TextField("Parameter", parameter.ParamRef);

            if (GUI.Button(searchRect, new GUIContent(browseIcon, "Search"), buttonStyle))
            {
                var eventBrowser = ScriptableObject.CreateInstance<EventBrowser>();

                //eventBrowser.ChooseParameter(property);
                var windowRect = position;
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = pathRect.height + 1;
                eventBrowser.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}