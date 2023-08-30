using System;
using FMODPlus;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class FMODEditorUtility
{
    public static void RegisterValueChangeCallback(this PropertyField element,
        SerializedProperty property, string oldValue,
        Action<SerializedProperty> callback)
    {
        SerializedProperty pathProperty = null;
        
        // Find Property
        switch (property.type)
        {
            case nameof(EventReference):
                pathProperty = property.FindPropertyRelative("Path");
                break;
            
            case "string":
                pathProperty = property;
                break;
        }
        
        // Init
        if (pathProperty != null)
        {
            oldValue = pathProperty.stringValue;

            element.schedule.Execute(() => {
                if (oldValue != pathProperty.stringValue)
                    callback.Invoke(pathProperty);

                oldValue = pathProperty.stringValue;
            }).Every(5);
        }
    }
    
    public static VisualElement CreateNotFoundField()
    {
        var globalParameterLayout = new SimpleBaseField {
            name = "Global Parameter Layout",
            Label = "Override Value",
            style = {
                marginTop = 0,
                marginBottom = 0
            }
        };

        #region global Parameter Layout ContentContainer Style
        globalParameterLayout.contentContainer.style.borderTopWidth = 0;
        globalParameterLayout.contentContainer.style.borderBottomWidth = 0;
        globalParameterLayout.contentContainer.style.paddingTop = 0;
        globalParameterLayout.contentContainer.style.paddingBottom = 0;
        #endregion

        globalParameterLayout.Label = string.Empty;

        Texture2D warningIcon = EditorUtils.LoadImage("NotFound.png");

        var icon = new VisualElement();
        icon.style.backgroundImage = new StyleBackground(warningIcon);
        icon.style.width = warningIcon.width;
        icon.style.height = warningIcon.height;

        var textField = new Label();
        textField.text = "Parameter Not Found";

        var innerContainer = new VisualElement {
            name = "innerContainer",
            style = {
                flexDirection = FlexDirection.Row
            }
        };

        innerContainer.Add(icon);
        innerContainer.Add(textField);
        globalParameterLayout.contentContainer.Add(innerContainer);

        return globalParameterLayout;
    }

    public static void UpdateParamsOnEmitter(SerializedObject serializedObject, string path, int type = 0)
    {
        if (string.IsNullOrEmpty(path) || EventManager.EventFromPath(path) == null)
        {
            return;
        }

        var eventRef = EventManager.EventFromPath(path);
        serializedObject.ApplyModifiedProperties();

        if (serializedObject.isEditingMultipleObjects)
        {
            foreach (var obj in serializedObject.targetObjects)
            {
                switch (type)
                {
                    case 0:
                        UpdateParamsOnEmitter(obj, eventRef);
                        break;
                    case 1:
                        UpdateParamsOnEmitterOnlyCommandSender(obj, eventRef);
                        break;
                }
            }
        }
        else
        {
            switch (type)
            {
                case 0:
                    UpdateParamsOnEmitter(serializedObject.targetObject, eventRef);
                    break;
                case 1:
                    UpdateParamsOnEmitterOnlyCommandSender(serializedObject.targetObject, eventRef);
                    break;
            }
        }

        serializedObject.Update();
    }

    private static void UpdateParamsOnEmitter(UnityEngine.Object obj, EditorEventRef eventRef)
    {
        var emitter = obj as FMODAudioSource;
        if (emitter == null)
        {
            // Custom game object
            return;
        }

        for (int i = 0; i < emitter.Params.Length; i++)
        {
            if (!eventRef.LocalParameters.Exists((x) => x.Name == emitter.Params[i].Name))
            {
                int end = emitter.Params.Length - 1;
                emitter.Params[i] = emitter.Params[end];
                Array.Resize(ref emitter.Params, end);
                i--;
            }
        }

        emitter.OverrideAttenuation = false;
        emitter.OverrideMinDistance = eventRef.MinDistance;
        emitter.OverrideMaxDistance = eventRef.MaxDistance;
    }

    private static void UpdateParamsOnEmitterOnlyCommandSender(UnityEngine.Object obj, EditorEventRef eventRef)
    {
        var emitter = obj as EventCommandSender;
        if (emitter == null)
        {
            // Custom game object
            return;
        }

        for (int i = 0; i < emitter.Params.Length; i++)
        {
            if (!eventRef.LocalParameters.Exists((x) => x.Name == emitter.Params[i].Name))
            {
                int end = emitter.Params.Length - 1;
                emitter.Params[i] = emitter.Params[end];
                Array.Resize(ref emitter.Params, end);
                i--;
            }
        }
    }

}
