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
        parameterValueView = new ParameterValueView();

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

        var eventLayoutRoot = parameterValueView.CreateEventAddGUI(root1, eventCommandSender);

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
            fadeHelpBox, eventLayoutRoot
        };

        ControlField(visualElements);

        parameterValueView.RefreshPropertyRecords(editorEvent, eventCommandSender);

        clipField.contentContainer.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
            FMODEditorUtility.UpdateParamsOnEmitter(serializedObject, clipPath.stringValue, 1));

        root.RegisterCallbackAll(() =>
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
                    parameterValueView.RefreshPropertyRecords(editorEvent, eventCommandSender);
                }
            }
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
        var eventLayoutRoot = elements[9];

        // 일단 전부 비활성화
        foreach (var visualElement in elements)
            SetActiveField(visualElement, false);

        if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.Play)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(clipField, true);
            SetActiveField(eventLayoutRoot, true);
            parameterValueView.AutoHideParameterArea();
        }
        else if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.PlayOnAPI)
        {
            SetActiveField(clipStyleField, true);
            SetActiveField(clipField, true);
            SetActiveField(keyField, true);
            SetActiveField(onPlaySend, true);
            SetActiveField(eventSpace, true);
            SetActiveField(eventLayoutRoot, true);
            parameterValueView.AutoHideParameterArea();
            ControlClipStyleField(clipField, keyField, eventLayoutRoot);
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

    private void ControlClipStyleField(VisualElement clipField, VisualElement keyField, VisualElement eventLayoutRoot)
    {
        if (eventCommandSender.ClipStyle == ClipStyle.EventReference)
        {
            SetActiveField(keyField, false);
            SetActiveField(clipField, true);
        }
        else
        {
            SetActiveField(keyField, true);
            SetActiveField(clipField, false);
            SetActiveField(eventLayoutRoot, false);
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
        private SerializedObject serializedTargets;

        // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
        private readonly List<PropertyRecord> _propertyRecords = new();

        // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
        private readonly List<EditorParamRef> _missingParameters = new();

        private DropdownField addButton;
        private VisualElement parameterArea;
        private VisualElement parameterLayout;
        private Foldout titleText;

        private EventCommandSender commandSender;

        // EditorParamRef에서 이름이 같은 현재 선택 항목의 초기 매개변수 값 속성으로의 매핑입니다.
        // 일부 개체에 일부 속성이 누락될 수 있고 동일한 이름을 가진 속성이 다른 개체의 다른 배열 인덱스에 있을 수 있기 때문에 이것이 필요합니다.
        private class PropertyRecord
        {
            public string Name => paramRef.Name;

            public EditorParamRef paramRef;
            public ParamRef valueProperties;
        }

        public void Clear()
        {
            _propertyRecords.Clear();
            _missingParameters.Clear();

            commandSender.Params = Array.Empty<ParamRef>();
            SetActiveParameterArea(false);

            titleText.value = false;
        }

        private void SetActiveParameterArea(bool active)
        {
            parameterLayout.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void AutoHideParameterArea()
        {
            if (string.IsNullOrWhiteSpace(commandSender.Clip.Path))
                parameterLayout.style.display = DisplayStyle.None;
            else
                parameterLayout.style.display = DisplayStyle.Flex;
        }

        public VisualElement CreateEventAddGUI(VisualElement root, EventCommandSender sender)
        {
            commandSender = sender;

            #region Create Elements

            parameterLayout = new VisualElement();
            parameterArea = new VisualElement();
            var layout = new VisualElement();
            titleText = new Foldout();
            var baseField = new SimpleBaseField();
            addButton = new DropdownField();

            #endregion

            #region Register

            root.Add(parameterLayout);
            parameterLayout.Add(layout);
            parameterLayout.Add(parameterArea);

            layout.Add(baseField);
            layout.Add(titleText);
            baseField.contentContainer.Add(addButton);

            #endregion

            #region Layout Style

            layout.style.marginTop = 1f;
            parameterLayout.name = "ParameterLayout";

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

            addButton.SetEnabled(false);

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

            addButton.RegisterCallback<MouseDownEvent>(_ => DrawAddButton(addButton.worldBound));

            return parameterLayout;
        }

        private void RefreshAddButton()
        {
            addButton.SetEnabled(_missingParameters.Count > 0);
        }

        // propertyRecords 및 missingParameters 컬렉션을 다시 빌드합니다.
        public void RefreshPropertyRecords(EditorEventRef eventRef, EventCommandSender target)
        {
            if (!eventRef)
                return;

            _propertyRecords.Clear();

            // 해당 타겟의 추가된 파라미터 정보를 가져온다.
            ParamRef[] paramsProperty = target.Params;

            // 파라미터 한개씩 순례
            foreach (ParamRef parameterProperty in paramsProperty)
            {
                // 이름과 값 가져오기
                string name = parameterProperty.Name;
                ParamRef valueProperty = parameterProperty;

                // 파라미터 리코드에 있는지 조회 (사실상 없을 수 밖에 없음.)
                PropertyRecord record = _propertyRecords.Find(r => r.Name == name);

                // 이미 존재할 경우 값 프로퍼티에 추가한다.
                if (record != null)
                    record.valueProperties = valueProperty;
                else
                {
                    EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);

                    if (paramRef != null)
                    {
                        _propertyRecords.Add(
                            new PropertyRecord()
                            {
                                paramRef = paramRef,
                                valueProperties = valueProperty
                            });
                    }
                }
            }

            // 다중 선택이 있는 경우에만 정렬합니다. 선택한 개체가 하나만 있는 경우
            // 사용자는 프리팹으로 되돌릴 수 있으며 동작은 배열 순서에 따라 달라지므로 실제 순서를 표시하는 것이 도움이 됩니다.
            _propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.Name, b.Name));

            _missingParameters.Clear();

            foreach (var parameter in eventRef.LocalParameters)
            {
                PropertyRecord record = _propertyRecords.Find(p => p.Name == parameter.Name);

                if (record == null)
                    _missingParameters.Add(parameter);
            }

            RefreshAddButton();
            DrawValue();
        }

        private void DrawValue()
        {
            // parameterArea 자식들은 모두 제거하기
            parameterArea.Clear();

            foreach (PropertyRecord record in _propertyRecords)
                parameterArea.Add(AdaptiveParameterField(record));
        }

        private SimpleBaseField AdaptiveParameterField(PropertyRecord record)
        {
            var baseField = new SimpleBaseField
            {
                Label = record.Name,
                style =
                {
                    marginTop = 0,
                    marginBottom = 0
                }
            };

            #region BaseField ContentContainer Style

            baseField.contentContainer.style.borderTopWidth = 0;
            baseField.contentContainer.style.borderBottomWidth = 0;
            baseField.contentContainer.style.paddingTop = 0;
            baseField.contentContainer.style.paddingBottom = 0;

            #endregion

            switch (record.paramRef.Type)
            {
                case ParameterType.Continuous:

                    var floatSlider = new Slider(record.paramRef.Min, record.paramRef.Max)
                    {
                        style =
                        {
                            marginLeft = 0f,
                            flexGrow = 1f
                        },
                        showInputField = true,
                        value = record.valueProperties.Value
                    };

                    baseField.contentContainer.Add(floatSlider);

                    floatSlider.RegisterValueChangedCallback(evt => record.valueProperties.Value = evt.newValue);

                    break;
                case ParameterType.Discrete:
                    var intSlider = new SliderInt((int)record.paramRef.Min, (int)record.paramRef.Max)
                    {
                        style =
                        {
                            marginLeft = 0f,
                            flexGrow = 1f
                        },
                        showInputField = true,
                        value = (int)record.valueProperties.Value
                    };

                    baseField.contentContainer.Add(intSlider);

                    intSlider.RegisterValueChangedCallback(evt => record.valueProperties.Value = evt.newValue);

                    break;
                case ParameterType.Labeled:
                    var dropdown = new DropdownField
                    {
                        style =
                        {
                            marginLeft = 0f,
                            flexGrow = 1f
                        },
                        choices = record.paramRef.Labels.ToList(),
                        index = (int)record.valueProperties.Value
                    };

                    baseField.contentContainer.Add(dropdown);

                    dropdown.RegisterValueChangedCallback(_ => record.valueProperties.Value = dropdown.index);

                    break;
            }

            var btn = new Button
            {
                text = "Remove",
                style =
                {
                    marginRight = 0f
                }
            };

            baseField.contentContainer.Add(btn);

            btn.clicked += () => DeleteParameter(record);

            return baseField;
        }

        private void DrawAddButton(Rect position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("All"), false, () =>
            {
                foreach (EditorParamRef parameter in _missingParameters)
                    AddParameter(parameter);

                Refresh();
            });

            menu.AddSeparator(string.Empty);

            foreach (EditorParamRef parameter in _missingParameters)
            {
                menu.AddItem(new GUIContent(parameter.Name), false,
                    (userData) =>
                    {
                        AddParameter(userData as EditorParamRef);
                        Refresh();
                    },
                    parameter);
            }

            menu.DropDown(position);

            void Refresh()
            {
                // 토글을 펼칩니다.
                titleText.value = true;

                var refreshEvent = EventManager.EventFromPath(commandSender.Clip.Path);
                RefreshPropertyRecords(refreshEvent, commandSender);
            }
        }

        // 매개변수가 없는 모든 선택된 객체에 주어진 매개변수에 대한 초기값을 추가합니다.
        private void AddParameter(EditorParamRef parameter)
        {
            if (Array.FindIndex(commandSender.Params, p => p.Name == parameter.Name) < 0)
            {
                List<ParamRef> parameterList = new List<ParamRef>(commandSender.Params);

                var newValue = new ParamRef
                {
                    Name = parameter.Name,
                    Value = parameter.Default
                };

                parameterList.Add(newValue);

                commandSender.Params = parameterList.ToArray();
            }
        }

        // 선택한 모든 개체에서 지정된 이름에 대한 초기 매개변수 값을 삭제합니다.
        private void DeleteParameter(PropertyRecord record)
        {
            List<ParamRef> parameterList = new List<ParamRef>(commandSender.Params);

            parameterList.RemoveAll(p => p.Name == record.Name);

            commandSender.Params = parameterList.ToArray();

            var eventRef = EventManager.EventFromPath(commandSender.Clip.Path);
            RefreshPropertyRecords(eventRef, commandSender);
        }
    }
}
#endif