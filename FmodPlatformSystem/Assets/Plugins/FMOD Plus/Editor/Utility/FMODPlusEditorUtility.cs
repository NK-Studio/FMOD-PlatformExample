using System;
using FMODPlus;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class FMODPlusEditorUtility
{
    private static VisualElement _cachedContextWidthElement;
    private static VisualElement _cachedInspectorElement;

    internal const string DefaultKey = "New Key";

    internal static bool IsDarkTheme => EditorGUIUtility.isProSkin;

    internal static void RegisterValueChangeCallback(this PropertyField element,
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

    internal static VisualElement CreateNotFoundField()
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

    internal static void UpdateParamsOnEmitter(SerializedObject serializedObject, string path, int type = 0)
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
    
    internal static VisualElement Space(float height)
    {
        VisualElement space = new();
        space.style.height = height;
        return space;
    }

    internal static void Close(this Foldout element)
    {
        element.value = false;
    }

    internal static VisualElement Line(Color color, float height, float topBottomMargin = 1f,
        float leftRightMargin = 0f)
    {
        var line = new VisualElement {
            style = {
                backgroundColor = new StyleColor(color),
                marginTop = topBottomMargin,
                marginBottom = topBottomMargin,
                marginLeft = leftRightMargin,
                marginRight = leftRightMargin,
                height = height,
            }
        };

        return line;
    }

    internal static void SetActive(this VisualElement field, bool active)
    {
        field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        field.style.visibility = active ? Visibility.Visible : Visibility.Hidden;
    }

    /// <summary>
    /// It converts between label field spacing and input field spacing.
    /// </summary>
    /// <param name="parent">Parent element with element of label area and input area</param>
    /// <param name="labelArea">Elements of the label area</param>
    /// <param name="inputArea">Elements of the input area</param>
    /// <param name="debug">If true, the area is colored.</param>
    internal static void ApplyFieldArea(VisualElement parent, VisualElement labelArea, VisualElement inputArea,
        bool debug = false)
    {
        #region Setup Name
        parent.name = "BaseFieldLayout";
        labelArea.name = "LabelArea";
        inputArea.name = "InputArea";
        #endregion

        parent.style.height = EditorGUIUtility.singleLineHeight;

        parent.style.flexDirection = FlexDirection.Row;
        labelArea.style.flexDirection = FlexDirection.Row;
        inputArea.style.flexDirection = FlexDirection.Row;

        labelArea.style.alignItems = Align.Center;
        inputArea.style.alignItems = Align.Center;

        labelArea.style.paddingLeft = 0;
        labelArea.style.paddingRight = 0;
        labelArea.style.paddingTop = 0;
        labelArea.style.paddingBottom = 0;

        inputArea.style.paddingLeft = 0;
        inputArea.style.paddingRight = 0;
        inputArea.style.paddingTop = 0;
        inputArea.style.paddingBottom = 0;

        if (debug)
        {
            var debugRed = Color.red;
            debugRed.a = 0.1f;

            var debugGreen = Color.green;
            debugGreen.a = 0.1f;

            labelArea.style.backgroundColor = new StyleColor(debugRed);
            inputArea.style.backgroundColor = new StyleColor(debugGreen);
        }

        // These fields are essential classes,
        // and input fields provided by Unity provide classes with additional padding and margin,
        // so it is recommended to add them separately if needed.
        parent.AddToClassList("unity-base-field__aligned");
        labelArea.AddToClassList("unity-base-field__label");
        inputArea.AddToClassList("unity-base-field__input");
        inputArea.AddToClassList("unity-base-text-field__input--single-line");

        parent.RegisterCallback(
            new EventCallback<AttachToPanelEvent>(evt => OnAttachToPanel(evt, parent, labelArea)));
    }

    internal static void ApplyIcon(Texture2D darkIcon, Texture2D whiteIcon, UnityEngine.Object targetObject)
    {
        if (!darkIcon || !whiteIcon)
        {
            // Debug : Debug.LogWarning($"{targetObject.name} : No Binding Icon");
            EditorGUIUtility.SetIconForObject(targetObject, null);
            return;
        }

        bool isDarkMode = EditorGUIUtility.isProSkin;

        if (isDarkMode)
        {
            if (darkIcon)
                EditorGUIUtility.SetIconForObject(targetObject, darkIcon);
        }
        else
        {
            if (whiteIcon)
                EditorGUIUtility.SetIconForObject(targetObject, whiteIcon);
        }
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
    
    private static void OnAttachToPanel(AttachToPanelEvent e, VisualElement thisTarget, VisualElement labelArea)
    {
        if (e.destinationPanel == null || e.destinationPanel.contextType == ContextType.Player)
            return;

        for (VisualElement parent = thisTarget.parent; parent != null; parent = parent.parent)
        {
            if (parent.ClassListContains("unity-inspector-element"))
                _cachedInspectorElement = parent;
            if (parent.ClassListContains("unity-inspector-main-container"))
            {
                _cachedContextWidthElement = parent;
                break;
            }
        }

        if (_cachedInspectorElement == null)
            return;

        thisTarget.RegisterCallback<GeometryChangedEvent>(_ =>
            OnInspectorFieldGeometryChanged(thisTarget, labelArea));
    }

    private static void OnInspectorFieldGeometryChanged(VisualElement thisTarget, VisualElement labelArea) =>
        AlignLabel(thisTarget, labelArea);

    private static void AlignLabel(VisualElement thisTarget, VisualElement labelArea)
    {
        if (!thisTarget.ClassListContains("unity-base-field__aligned"))
            return;

        const float labelWidthRatio = 0.45f;
        const float labelExtraPadding = 37f;
        const float labelBaseMinWidth = 123f;
        const float labelExtraContextWidth = 1f;

        Rect worldBound = thisTarget.worldBound;
        double x1 = worldBound.x;
        worldBound = _cachedInspectorElement.worldBound;
        double x2 = worldBound.x;
        float num1 = (float)(x1 - x2) - _cachedInspectorElement.resolvedStyle.paddingLeft;
        float num2 = labelExtraPadding + num1 + thisTarget.resolvedStyle.paddingLeft;
        float a = labelBaseMinWidth - num1 - thisTarget.resolvedStyle.paddingLeft;
        VisualElement visualElement = _cachedContextWidthElement ?? _cachedInspectorElement;
        labelArea.style.minWidth = Mathf.Max(a, 0.0f);
        float b = (visualElement.resolvedStyle.width + labelExtraContextWidth)*labelWidthRatio - num2;
        if (Mathf.Abs(labelArea.resolvedStyle.width - b) <= 1.0000000031710769E-30)
            return;
        labelArea.style.width = Mathf.Max(0.0f, b);
    }
}
