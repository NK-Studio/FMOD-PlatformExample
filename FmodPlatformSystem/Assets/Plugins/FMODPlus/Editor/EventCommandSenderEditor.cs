#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using FMODUnity;
using NKStudio;
using NKStudio.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(EventCommandSender))]
    public class EventCommandSenderEditor : Editor
    {
        private EventCommandSender _commandSender;
        private ParameterValueView _parameterValueView;

        private EditorEventRef editorEvent;

        private ClipStyle oldClipStyle;
        private string oldEventPath;

        [SerializeField] private StyleSheet boxGroupStyle;

        private const string ClipsID = "Clips";
        private const string ListID = "list"; 

        private void OnEnable()
        {
            _parameterValueView = new ParameterValueView(serializedObject);

            // Event Command Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/EventCommandSender.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupSheet = AssetDatabase.GUIDToAssetPath("6a25e899d15eb994b85241dddfd90559");
            boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupSheet);
        }


        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            _commandSender = (EventCommandSender)target;
            root.styleSheets.Add(boxGroupStyle);

            var root0 = new VisualElement();
            root0.AddToClassList("GroupBoxStyle");
            var behaviourField = new PropertyField(serializedObject.FindProperty("BehaviourStyle"));
            var audioSourceField = new ObjectField("Audio Source");
            audioSourceField.objectType = typeof(FMODAudioSource);
            audioSourceField.bindingPath = "Source";
            audioSourceField.AddToClassList("unity-base-field__aligned");

            var root1 = new VisualElement();
            root1.AddToClassList("GroupBoxStyle");
            var clip = serializedObject.FindProperty("Clip");
            var clipField = new PropertyField(clip);
            clipField.label = "Event";

            var useGlobalKeyList = serializedObject.FindProperty("UseGlobalKeyList");
            var useGlobalKeyListField = new PropertyField(useGlobalKeyList);
            var clipStyleField = new PropertyField(serializedObject.FindProperty("ClipStyle"));
            var keyField = new PropertyField(serializedObject.FindProperty("Key"));
            keyField.label = "Event Key";

            var fadeField = new PropertyField(serializedObject.FindProperty("Fade"));
            var sendOnStart = new PropertyField(serializedObject.FindProperty("SendOnStart"));

            var root2 = new VisualElement();
            var onPlaySend = new PropertyField(serializedObject.FindProperty("OnPlaySend"));
            var onStopSend = new PropertyField(serializedObject.FindProperty("OnStopSend"));

            string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
                ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
                : "Fade function requires AHDSR module to work.";

            var fadeHelpBox = new HelpBox(appSystemLanguage, HelpBoxMessageType.Info);

            var helpBox = new HelpBox();
            helpBox.ElementAt(0).style.flexGrow = 1;
            helpBox.messageType = HelpBoxMessageType.Error;
            helpBox.style.marginTop = 6;
            helpBox.style.marginBottom = 6;

            var localKeyListField = new ObjectField();
            var localKeyList = serializedObject.FindProperty("keyList");
            localKeyListField.label = "Key List";
            localKeyListField.objectType = typeof(LocalKeyList);
            localKeyListField.BindProperty(localKeyList);
            localKeyListField.AddToClassList("unity-base-field__aligned");

            Color lineColor = Color.black;
            lineColor.a = 0.4f;
            VisualElement line = NKEditorUtility.Line(lineColor, 1.5f, 4f, 3f);
            
            root.Add(root0);
            root.Add(Space(5));
            root0.Add(behaviourField);
            root0.Add(audioSourceField);

            root.Add(root1);
            root1.Add(useGlobalKeyListField);
            root1.Add(localKeyListField);
            root1.Add(line);
            root1.Add(clipStyleField);
            root1.Add(clipField);
            root1.Add(keyField);
            VisualElement parameterArea = new();
            parameterArea.name = "ParameterArea";
            parameterArea.SetActive(false);

            (VisualElement baseFieldLayout, Foldout titleToggleLayout, DropdownField addButton) =
                _parameterValueView.InitParameterView(root1, parameterArea, _commandSender);

            titleToggleLayout.value = false;
            _parameterValueView.DrawValues(true);

            oldClipStyle = _commandSender.ClipStyle;

            VisualElement notFoundField = FMODEditorUtility.CreateNotFoundField();

            root1.Add(notFoundField);
            root1.Add(helpBox);
            root1.Add(parameterArea);
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
                audioSourceField, clipStyleField, clipField, fadeField, onPlaySend, onStopSend, eventSpace,
                fadeHelpBox, parameterArea, useGlobalKeyListField
            };

            ControlField(visualElements);

            root.Add(new IMGUIContainer(() =>
            {
                baseFieldLayout.SetActive(false);
                clipField.SetActive(false);
                keyField.SetActive(false);
                helpBox.SetActive(false);
                notFoundField.SetActive(false);

                if (_commandSender.BehaviourStyle is AudioBehaviourStyle.Stop or AudioBehaviourStyle.StopOnAPI)
                    return;

                if (_commandSender.BehaviourStyle != AudioBehaviourStyle.PlayOnAPI)
                {
                    if (!_commandSender.Source)
                    {
                        _parameterValueView.Dispose();

                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "FMOD Audio Source가 연결되어 있지 않습니다."
                            : "FMOD Audio Source is not connected.";

                        helpBox.text = msg;
                        helpBox.SetActive(true);
                        return;
                    }
                }

                if (_commandSender.ClipStyle == ClipStyle.EventReference)
                    clipField.SetActive(true);
                else
                    keyField.SetActive(true);

                bool isTypingPath;

                if (_commandSender.ClipStyle == ClipStyle.EventReference)
                    isTypingPath = !string.IsNullOrWhiteSpace(_commandSender.Clip.Path);
                else
                    isTypingPath = !string.IsNullOrWhiteSpace(_commandSender.Key);

                if (!isTypingPath)
                {
                    _parameterValueView.Dispose();

                    string msg;

                    if (_commandSender.ClipStyle == ClipStyle.EventReference)
                        msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "Event가 비어있습니다."
                            : "Event is empty.";
                    else
                    {
                        notFoundField.SetActive(true);

                        msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "Key가 비어있습니다."
                            : "Key is empty.";
                    }

                    helpBox.text = msg;
                    helpBox.SetActive(true);
                    return;
                }

                EditorEventRef existEvent;

                if (_commandSender.ClipStyle == ClipStyle.EventReference)
                    existEvent = EventManager.EventFromPath(_commandSender.Clip.Path);
                else
                {
                    bool useGlobalList = useGlobalKeyList.boolValue;

                    if (useGlobalList)
                        existEvent = KeyList.Instance.GetEventRef(_commandSender.Key);
                    else
                    {
                        existEvent = null;

                        if (localKeyList.objectReferenceValue != null)
                        {
                            LocalKeyList targetKeyList = (LocalKeyList)localKeyList.objectReferenceValue;
                            SerializedObject targetLocalKeyList = new(targetKeyList);
                            SerializedProperty lists = targetLocalKeyList.FindProperty(ClipsID)
                                .FindPropertyRelative(ListID);
                            foreach (SerializedProperty list in lists)
                            {
                                string targetKey = list.FindPropertyRelative("Key").stringValue;
                                string targetPath = list.FindPropertyRelative("Value").FindPropertyRelative("Path")
                                    .stringValue;

                                if (_commandSender.Key == targetKey)
                                {
                                    existEvent = EventManager.EventFromPath(targetPath);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (existEvent != null)
                {
                    if (!string.IsNullOrWhiteSpace(oldEventPath))
                        if (!oldEventPath.Equals(existEvent.Path))
                        {
                            SerializedProperty paramsProperty = serializedObject.FindProperty("Params");
                            paramsProperty.ClearArray();

                            serializedObject.ApplyModifiedProperties();
                            _parameterValueView.DrawValues(true);
                        }

                    oldEventPath = existEvent.Path;

                    helpBox.SetActive(false);
                }
                else
                {
                    string msg = Application.systemLanguage == SystemLanguage.Korean
                        ? "연결된 이벤트 주소가 유효하지 않습니다."
                        : "The connected event address is invalid.";

                    helpBox.text = msg;
                    helpBox.SetActive(true);
                    return;
                }

                // 스타일이 Base 방식일 때만 처리로 현재는 되어있다.
                if (Event.current.type == EventType.Layout)
                    _parameterValueView.RefreshPropertyRecords(existEvent);

                baseFieldLayout.SetActive(true);
                helpBox.SetActive(false);
            }));

            useGlobalKeyListField.RegisterValueChangeCallback(evt =>
            {
                bool isGlobal = evt.changedProperty.boolValue;

                if (isGlobal)
                    localKeyListField.SetActive(false);
                else
                    localKeyListField.SetActive(true);
            });

            titleToggleLayout.RegisterValueChangedCallback(evt =>
            {
                bool isExpanded = evt.newValue;
                parameterArea.SetActive(isExpanded);
            });

            clipStyleField.RegisterValueChangeCallback(evt =>
            {
                ControlField(visualElements);

                ClipStyle newClipStyle = (ClipStyle)evt.changedProperty.enumValueIndex;

                if (oldClipStyle != newClipStyle)
                {
                    _parameterValueView.Dispose();
                    parameterArea.Clear();
                    addButton.SetEnabled(true);
                }

                oldClipStyle = _commandSender.ClipStyle;
            });

            behaviourField.RegisterValueChangeCallback(_ =>
            {
                ControlField(visualElements);
            });

            fadeField.RegisterValueChangeCallback(_ =>
            {
                ControlField(visualElements);
            });

            return root;
        }

        private void ControlField(IReadOnlyList<VisualElement> elements)
        {
            var audioSourceField = elements[0];
            var clipStyleField = elements[1];
            var clipField = elements[2];
            var fadeField = elements[3];
            var onPlaySend = elements[4];
            var onStopSend = elements[5];
            var eventSpace = elements[6];
            var helpBox = elements[7];
            var parameterArea = elements[8];
            var useGlobalKeyListField = elements[9];

            // 일단 전부 비활성화
            foreach (var visualElement in elements)
                visualElement.SetActive(false);

            if (_commandSender.BehaviourStyle == AudioBehaviourStyle.Play)
            {
                audioSourceField.SetActive(true);
                clipStyleField.SetActive(true);
                clipField.SetActive(true);
                AutoParameterOpen();
            }
            else if (_commandSender.BehaviourStyle == AudioBehaviourStyle.PlayOnAPI)
            {
                clipStyleField.SetActive(true);
                clipField.SetActive(true);
                onPlaySend.SetActive(true);
                eventSpace.SetActive(true);
                AutoParameterOpen();

                if (_commandSender.ClipStyle == ClipStyle.EventReference)
                    useGlobalKeyListField.SetActive(false);
                else
                    useGlobalKeyListField.SetActive(true);
            }
            else if (_commandSender.BehaviourStyle == AudioBehaviourStyle.Stop)
            {
                parameterArea.SetActive(false);
                audioSourceField.SetActive(true);
                fadeField.SetActive(true);
                ControlFadeHelpBoxField(helpBox);
            }
            else // if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.StopOnAPI)
            {
                parameterArea.SetActive(false);
                fadeField.SetActive(true);
                onStopSend.SetActive(true);
                eventSpace.SetActive(true);
                ControlFadeHelpBoxField(helpBox);
            }
        }

        private void AutoParameterOpen()
        {
            if (_commandSender.Params.Length > 0)
                _parameterValueView.SetOpenParameterArea(true);
            else
                _parameterValueView.SetOpenParameterArea(false);
        }

        private void ControlFadeHelpBoxField(VisualElement fadeHelpBox)
        {
            if (_commandSender.Fade)
                fadeHelpBox.SetActive(true);
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
            private SerializedObject _serializedTargets;

            // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private DropdownField _addButton;
            private VisualElement _parameterArea;
            private VisualElement _parameterLayout;
            private Foldout _titleText;

            private EventCommandSender _commandSender;

            public ParameterValueView(SerializedObject serializedTargets)
            {
                _serializedTargets = serializedTargets;
            }

            private class PropertyRecord
            {
                public string Name => paramRef.Name;

                public EditorParamRef paramRef;
                public List<SerializedProperty> valueProperties;
            }

            public void Dispose()
            {
                _commandSender.Params = Array.Empty<ParamRef>();
                _propertyRecords.Clear();
                _missingParameters.Clear();
                _titleText.value = false;
            }

            public Tuple<VisualElement, Foldout, DropdownField> InitParameterView(VisualElement root,
                VisualElement parameterArea,
                EventCommandSender commandSender)
            {
                _parameterArea = parameterArea;
                _commandSender = commandSender;

                VisualElement baseFieldLayout = new();
                VisualElement labelArea = new();
                VisualElement inputArea = new();

                _titleText = new Foldout();
                _titleText.text = "Initial Parameter Values";

                _addButton = new DropdownField();
                _addButton.value = "Add";
                _addButton.style.flexGrow = 1;
                _addButton.style.marginLeft = 0;

                root.Add(baseFieldLayout);
                baseFieldLayout.Add(labelArea);
                baseFieldLayout.Add(inputArea);

                labelArea.Add(_titleText);
                inputArea.Add(_addButton);
                _addButton.RegisterCallback<MouseDownEvent>(_ => DrawAddButton(_addButton.worldBound));

                NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

                return new Tuple<VisualElement, Foldout, DropdownField>(baseFieldLayout, _titleText, _addButton);
            }

            public void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            public void SetOpenParameterArea(bool open)
            {
                _titleText.value = open;
                _parameterArea.SetActive(open);
            }

            public void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record = _propertyRecords.Find(r => r.Name == name);

                    if (record != null)
                        record.valueProperties.Add(valueProperty);
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);

                        if (paramRef != null)
                        {
                            _propertyRecords.Add(
                                new PropertyRecord()
                                {
                                    paramRef = paramRef,
                                    valueProperties = new List<SerializedProperty>() { valueProperty }
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
            }

            public void DrawValues(bool preRefresh = false)
            {
                if (preRefresh)
                {
                    string path;

                    if (_commandSender.ClipStyle == ClipStyle.EventReference)
                        path = _commandSender.Clip.Path;
                    else
                    {
                        EditorEventRef eventRef = KeyList.Instance.GetEventRef(_commandSender.Key);
                        path = eventRef == null ? string.Empty : eventRef.Path;
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        EditorEventRef eventRef = EventManager.EventFromPath(path);
                        RefreshPropertyRecords(eventRef);
                    }
                }

                // parameterArea 자식들은 모두 제거하기
                _parameterArea.Clear();

                foreach (PropertyRecord record in _propertyRecords)
                {
                    _parameterArea.Add(AdaptiveParameterField(record));
                }

                if (preRefresh)
                    CalculateEnableAddButton();
            }

            private SimpleBaseField AdaptiveParameterField(PropertyRecord record)
            {
                float value = 0;

                if (record.valueProperties.Count == 1)
                    value = record.valueProperties[0].floatValue;
                else
                {
                    bool first = true;

                    foreach (SerializedProperty property in record.valueProperties)
                        if (first)
                        {
                            value = property.floatValue;
                            first = false;
                        }
                }

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
                            value = value
                        };

                        foreach (SerializedProperty property in record.valueProperties)
                            floatSlider.value = property.floatValue;

                        baseField.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                        {
                            foreach (SerializedProperty property in record.valueProperties)
                                property.floatValue = evt.newValue;
                        });

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
                            value = (int)value
                        };

                        baseField.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt =>
                        {
                            foreach (SerializedProperty property in record.valueProperties)
                                property.floatValue = evt.newValue;
                        });

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
                            index = (int)value
                        };

                        baseField.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ =>
                        {
                            foreach (SerializedProperty property in record.valueProperties)
                                property.floatValue = dropdown.index;
                        });

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

                btn.clicked += () =>
                {
                    DeleteParameter(record.Name);
                    DrawValues(true);
                };

                return baseField;
            }

            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("All"), false, () =>
                {
                    foreach (EditorParamRef parameter in _missingParameters)
                        AddParameter(parameter);

                    DrawValues(true);
                    SetOpenParameterArea(true);
                });

                menu.AddSeparator(string.Empty);

                foreach (EditorParamRef parameter in _missingParameters)
                {
                    menu.AddItem(new GUIContent(parameter.Name), false,
                        (userData) =>
                        {
                            AddParameter(userData as EditorParamRef);

                            DrawValues(true);
                            SetOpenParameterArea(true);
                        },
                        parameter);
                }

                menu.DropDown(position);
            }

            // 매개변수가 없는 모든 선택된 객체에 주어진 매개변수에 대한 초기값을 추가합니다.
            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_commandSender.Params, p => p.Name == parameter.Name) < 0)
                {
                    SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    _serializedTargets.ApplyModifiedProperties();
                }
            }

            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    string paramName = child.FindPropertyRelative("Name").stringValue;
                    if (paramName == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                _serializedTargets.ApplyModifiedProperties();
            }
        }
    }
}
#endif