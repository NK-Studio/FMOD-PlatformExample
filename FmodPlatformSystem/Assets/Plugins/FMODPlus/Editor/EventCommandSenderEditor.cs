#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using FMODPlus;
using UnityEditor;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MyClass : VisualElement
{
    public DropdownMenu dropdownMenu { get; private set; }

    public MyClass()
    {
        dropdownMenu = new DropdownMenu();
    }
}


[CustomEditor(typeof(EventCommandSender))]
public class EventCommandSenderEditor : Editor
{
    private EventCommandSender eventCommandSender;
    private ParameterValueView parameterValueView;

    private EditorEventRef editorEvent;
    private string oldEventPath;

    [SerializeField] private StyleSheet boxGroupStyle;

    private void OnEnable()
    {
        parameterValueView = new ParameterValueView(serializedObject);

        // Event Command Sender
        string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
        Texture2D darkIcon =
            AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

        string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
        Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

        string path = "Assets/Plugins/FMODPlus/Runtime/EventCommandSender.cs";
        MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

        if (!boxGroupStyle)
            boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/FMODPlus/Editor/ButtonStyle.uss");
    }


    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        eventCommandSender = (EventCommandSender)target;
        root.styleSheets.Add(boxGroupStyle);

        var root0 = new VisualElement();
        root0.AddToClassList("GroupBoxStyle");
        var behaviourField = new PropertyField(serializedObject.FindProperty("BehaviourStyle"));
        var audioSourceField = new PropertyField(serializedObject.FindProperty("Source"));

        var root1 = new VisualElement();
        root1.AddToClassList("GroupBoxStyle");
        var clip = serializedObject.FindProperty("Clip");
        var clipPath = clip.FindPropertyRelative("Path");
        var clipField = new PropertyField(clip);
        var clipStyleField = new PropertyField(serializedObject.FindProperty("ClipStyle"));
        var keyField = new PropertyField(serializedObject.FindProperty("Key"));

        var fadeField = new PropertyField(serializedObject.FindProperty("Fade"));
        var sendOnStart = new PropertyField(serializedObject.FindProperty("SendOnStart"));

        var root2 = new VisualElement();
        var onPlaySend = new PropertyField(serializedObject.FindProperty("OnPlaySend"));
        var onStopSend = new PropertyField(serializedObject.FindProperty("OnStopSend"));


        string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
            ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
            : "Fade function requires AHDSR module to work.";

        var fadeHelpBox = new HelpBox(appSystemLanguage, HelpBoxMessageType.Info);

        root.Add(root0);
        root.Add(Space(5));
        root0.Add(behaviourField);
        root0.Add(audioSourceField);

        root.Add(root1);
        root1.Add(clipStyleField);
        root1.Add(clipField);

        var eventLayoutRoot = parameterValueView.CreateEventAddGUI(root1);

        // EditorEventRef editorEvent = EventManager.EventFromPath(clipPath.stringValue);
        //parameterValueView.OnGUI(editorEvent, !clip.hasMultipleDifferentValues, root1);

        root1.Add(keyField);
        root1.Add(fadeField);
        root1.Add(sendOnStart);
        root1.Add(fadeHelpBox);

        //Include root3 in root.
        var eventSpace = Space(5f);
        root2.Add(eventSpace);
        root2.Add(onPlaySend);
        root2.Add(onStopSend);
        root.Add(root2);

        //Init
        var visualElements = new[]
        {
            audioSourceField, clipStyleField, clipField, keyField, fadeField, onPlaySend, onStopSend, eventSpace,
            fadeHelpBox
        };

        ControlField(visualElements);


        clipField.contentContainer.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
        {
            FMODEditorUtility.UpdateParamsOnEmitter(serializedObject, clipPath.stringValue, 1);
        });


        clipField.RegisterCallbackAll(() =>
        {
            string currentEventPath;
            editorEvent = EventManager.EventFromPath(clipPath.stringValue);

            if (editorEvent)
                currentEventPath = editorEvent.Path;
            else
                currentEventPath = string.Empty;

            if (oldEventPath != currentEventPath)
            {
                oldEventPath = currentEventPath;

                if (string.IsNullOrWhiteSpace(currentEventPath))
                {
                    SetActiveField(eventLayoutRoot, false);
                    parameterValueView.Clear();
                }
                else
                {
                    SetActiveField(eventLayoutRoot, true);
                    parameterValueView.RefreshPropertyRecords(editorEvent);
                }
            }

            parameterValueView.ApplyProperties();
        });

        clipStyleField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        behaviourField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        fadeField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        return root;
    }

    private void ControlField(VisualElement[] elements)
    {
        var audioSourceField = elements[0];
        var clipStyleField = elements[1];
        var clipField = elements[2];
        var keyField = elements[3];
        var fadeField = elements[4];
        var onPlaySend = elements[5];
        var onStopSend = elements[6];
        var eventSpace = elements[7];
        var helpBox = elements[8];

        // 일단 전부 비활성화
        foreach (var visualElement in elements)
            SetActiveField(visualElement, false);

        if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.Play)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(clipField, true);
        }
        else if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.PlayOnAPI)
        {
            SetActiveField(clipStyleField, true);
            SetActiveField(clipField, true);
            SetActiveField(keyField, true);
            SetActiveField(onPlaySend, true);
            SetActiveField(eventSpace, true);
            ControlClipStyleField(clipField, keyField);
        }
        else if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.Stop)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(fadeField, true);
            ControlFadeHelpBoxField(helpBox);
        }
        else // if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.StopOnAPI)
        {
            SetActiveField(fadeField, true);
            SetActiveField(onStopSend, true);
            SetActiveField(eventSpace, true);
            ControlFadeHelpBoxField(helpBox);
        }
    }

    private void ControlFadeHelpBoxField(VisualElement fadeHelpBox)
    {
        if (eventCommandSender.Fade)
            SetActiveField(fadeHelpBox, true);
    }

    private void ControlClipStyleField(VisualElement clipField, VisualElement keyField)
    {
        if (eventCommandSender.ClipStyle == ClipStyle.EventReference)
        {
            SetActiveField(clipField, true);
            SetActiveField(keyField, false);
        }
        else
        {
            SetActiveField(clipField, false);
            SetActiveField(keyField, true);
        }
    }

    private void SetActiveField(VisualElement field, bool active)
    {
        field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private VisualElement Space(float height)
    {
        var space = new VisualElement();
        space.style.height = height;
        // Debug : space.style.backgroundColor = new StyleColor(Color.red);
        return space;
    }

    private class ParameterValueView
    {
        // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
        private List<SerializedObject> serializedTargets = new List<SerializedObject>();

        // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
        private List<PropertyRecord> propertyRecords = new List<PropertyRecord>();

        // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
        private List<EditorParamRef> missingParameters = new List<EditorParamRef>();

        // EditorParamRef에서 이름이 같은 현재 선택 항목의 초기 매개변수 값 속성으로의 매핑입니다.
        // 일부 개체에 일부 속성이 누락될 수 있고 동일한 이름을 가진 속성이 다른 개체의 다른 배열 인덱스에 있을 수 있기 때문에 이것이 필요합니다.
        private class PropertyRecord
        {
            public string name => paramRef.Name;

            public EditorParamRef paramRef;
            public List<SerializedProperty> valueProperties;
        }

        public void Clear()
        {
            propertyRecords.Clear();
            missingParameters.Clear();
        }

        public ParameterValueView(SerializedObject serializedObject)
        {
            foreach (UnityEngine.Object target in serializedObject.targetObjects)
                serializedTargets.Add(new SerializedObject(target));

            Debug.Log("serializedTargets.Count : " + serializedTargets.Count());
        }

        public VisualElement CreateEventAddGUI(VisualElement root)
        {
            #region Create Elements

            var layout = new VisualElement();
            var baseField = new SimpleBaseField();
            var addButton = new DropdownField();
            var titleText = new Foldout();
            var parameterArea = new VisualElement();

            #endregion

            #region Register

            root.Add(layout);
            root.Add(parameterArea);

            layout.Add(baseField);
            layout.Add(titleText);
            baseField.contentContainer.Add(addButton);

            #endregion

            #region Layout Style

            layout.style.marginTop = 1f;

            #endregion

            #region BaseField Style

            baseField.Label = string.Empty;
            baseField.style.marginTop = 0f;
            baseField.style.marginBottom = 0f;
            baseField.style.position = Position.Absolute;
            baseField.style.marginLeft = 0;
            baseField.style.left = 0;
            baseField.style.right = 0;

            #endregion

            #region BaseField Content Style

            baseField.contentContainer.style.paddingTop = 0f;
            baseField.contentContainer.style.borderTopWidth = 0;
            baseField.contentContainer.style.borderBottomWidth = 0;

            #endregion

            #region Add Button Style

            addButton.style.flexGrow = 1f;
            addButton.style.marginLeft = 0f;
            addButton.style.marginRight = 0f;
            addButton.style.marginTop = 0f;
            addButton.style.marginBottom = 0f;

            addButton.value = "Add";

            #endregion

            #region TitleText Style

            titleText.text = "Initial Parameter Values";
            Length titleWidth = new Length(37, LengthUnit.Percent);
            titleText.style.width = new StyleLength(titleWidth);

            #endregion

            #region ParameterArea Style

            parameterArea.style.marginLeft = 18f;

            #endregion

            titleText.RegisterValueChangedCallback(evt =>
            {
                parameterArea.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            addButton.RegisterCallback<MouseDownEvent>(evt => DrawAddButton(addButton.worldBound));

            return layout;
        }

        // propertyRecords 및 missingParameters 컬렉션을 다시 빌드합니다.
        public void RefreshPropertyRecords(EditorEventRef eventRef)
        {
            Debug.Log("동작 중");
            propertyRecords.Clear();

            Debug.Log(serializedTargets.Count());

            foreach (SerializedObject serializedTarget in serializedTargets)
            {
                SerializedProperty paramsProperty = serializedTarget.FindProperty("Params");

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record = propertyRecords.Find(r => r.name == name);

                    if (record != null)
                    {
                        record.valueProperties.Add(valueProperty);
                    }
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);

                        Debug.Log("paramRef : " + paramRef.Name);

                        if (paramRef != null)
                        {
                            propertyRecords.Add(
                                new PropertyRecord()
                                {
                                    paramRef = paramRef,
                                    valueProperties = new List<SerializedProperty>() { valueProperty },
                                });
                        }
                    }
                }
            }

            // 다중 선택이 있는 경우에만 정렬합니다. 선택한 개체가 하나만 있는 경우
            // 사용자는 프리팹으로 되돌릴 수 있으며 동작은 배열 순서에 따라 달라지므로 실제 순서를 표시하는 것이 도움이 됩니다.
            if (serializedTargets.Count > 1)
            {
                propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.name, b.name));
            }

            Debug.Log("missingParameters.Count : " + missingParameters.Count());

            missingParameters.Clear();
            missingParameters.AddRange(eventRef.LocalParameters.Where(
                p =>
                {
                    PropertyRecord record = propertyRecords.Find(r => r.name == p.Name);
                    return record == null || record.valueProperties.Count < serializedTargets.Count;
                }));

            foreach (var param in missingParameters)
            {
                Debug.Log(param.Name);
            }
        }

        private SimpleBaseField AdaptiveParameterField(ParameterType parameterType)
        {
            var baseField = new SimpleBaseField();

            baseField.style.marginTop = 0;
            baseField.style.marginBottom = 0;

            baseField.contentContainer.style.borderTopWidth = 0;
            baseField.contentContainer.style.borderBottomWidth = 0;
            baseField.contentContainer.style.paddingTop = 0;
            baseField.contentContainer.style.paddingBottom = 0;

            switch (parameterType)
            {
                case ParameterType.Continuous:
                    var floatSlider = new Slider(0f, 1f);
                    floatSlider.style.marginLeft = 0f;
                    floatSlider.style.flexGrow = 1f;
                    floatSlider.showInputField = true;

                    baseField.contentContainer.Add(floatSlider);
                    break;
                case ParameterType.Discrete:
                    var intSlider = new SliderInt(0, 5);
                    intSlider.style.marginLeft = 0f;
                    intSlider.style.flexGrow = 1f;
                    intSlider.showInputField = true;

                    baseField.contentContainer.Add(intSlider);
                    break;
                case ParameterType.Labeled:
                    var dropdown = new DropdownField();
                    dropdown.style.marginLeft = 0f;
                    dropdown.style.flexGrow = 1f;

                    baseField.contentContainer.Add(dropdown);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameterType), parameterType, null);
            }

            var btn = new Button();
            btn.text = "Remove";
            btn.style.marginRight = 0f;


            baseField.contentContainer.Add(btn);

            return baseField;
        }

        private void DrawAddButton(Rect position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("All"), false, () =>
            {
                foreach (EditorParamRef parameter in missingParameters)
                    AddParameter(parameter);
            });

            menu.AddSeparator(string.Empty);

            foreach (EditorParamRef parameter in missingParameters)
            {
                menu.AddItem(new GUIContent(parameter.Name), false,
                    (userData) => AddParameter(userData as EditorParamRef),
                    parameter);
            }

            menu.DropDown(position);
        }

        public void ApplyProperties()
        {
            foreach (SerializedObject serializedTarget in serializedTargets)
            {
                serializedTarget.ApplyModifiedProperties();
            }
        }

        // 소스 속성의 값을 모든 대상 속성에 복사합니다.
        // private void CopyValueToAll(SerializedProperty sourceProperty, List<SerializedProperty> targetProperties)
        // {
        //     foreach (SerializedProperty targetProperty in targetProperties)
        //     {
        //         if (targetProperty != sourceProperty)
        //         {
        //             targetProperty.floatValue = sourceProperty.floatValue;
        //             targetProperty.serializedObject.ApplyModifiedProperties();
        //         }
        //     }
        // }

        // 매개변수가 없는 모든 선택된 객체에 주어진 매개변수에 대한 초기값을 추가합니다.
        private void AddParameter(EditorParamRef parameter)
        {
            foreach (SerializedObject serializedTarget in serializedTargets)
            {
                EventCommandSender audioSource = serializedTarget.targetObject as EventCommandSender;

                if (Array.FindIndex(audioSource.Params, p => p.Name == parameter.Name) < 0)
                {
                    int targetIndex = audioSource.Params.Length;
                    int wantIndexSize = targetIndex + 1;
                    Array.Resize(ref audioSource.Params, wantIndexSize);

                    Debug.Log(parameter.Name);
                    //audioSource.Params[targetIndex].Name = parameter.Name;
                }

                // var refreshEvent = EventManager.EventFromPath(audioSource.Clip.Path);
                // RefreshPropertyRecords(refreshEvent);
            }
        }

        // 선택한 모든 개체에서 지정된 이름에 대한 초기 매개변수 값을 삭제합니다.
        private void DeleteParameter(string name)
        {
            foreach (SerializedObject serializedTarget in serializedTargets)
            {
                SerializedProperty paramsProperty = serializedTarget.FindProperty("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    if (child.FindPropertyRelative("Name").stringValue == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }
            }
        }
    }
}
#endif