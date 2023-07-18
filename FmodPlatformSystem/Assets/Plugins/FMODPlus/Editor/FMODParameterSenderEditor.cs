#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using NKStudio;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODPlus
{
    [CustomEditor(typeof(FMODParameterSender))]
    [CanEditMultipleObjects]
    public class FMODParameterSenderEditor : Editor
    {
        private FMODParameterSender parameterSender;

        private ParameterValueView _parameterValueView;

        [SerializeField] private StyleSheet groupBoxStyleSheet;
        [SerializeField] private StyleSheet buttonStyleSheet;

        private string _oldPath;
        private FMODParameterSender.AudioBehaviourStyle _oldBehaviourStyle;

        private bool _oldIsGlobalParameter;

        private void OnEnable()
        {
            _parameterValueView = new ParameterValueView(serializedObject);

            // Parameter Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("74cfbd073c7464035ba232171ef31f0f");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("6531cd3743c664274b21aa41c9b00c5c");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/FMODParameterSender.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupStyleSheetPath = AssetDatabase.GUIDToAssetPath("6a25e899d15eb994b85241dddfd90559");
            groupBoxStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupStyleSheetPath);

            string buttonStyleSheetPath = AssetDatabase.GUIDToAssetPath("db197c96211fc47319d2b84dcd02aacd");
            buttonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(buttonStyleSheetPath);
        }

        public override VisualElement CreateInspectorGUI()
        {
            parameterSender = (FMODParameterSender)target;

            VisualElement root = new();
            root.styleSheets.Add(groupBoxStyleSheet);
            root.styleSheets.Add(buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            PropertyField behaviourStyleField = new(serializedObject.FindProperty("BehaviourStyle"));
            ObjectField sourceField = new()
            {
                objectType = typeof(FMODAudioSource),
                label = "Audio Source",
                bindingPath = "Source"
            };
            sourceField.AddToClassList("unity-base-field__aligned");

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            PropertyField globalParameterFiled = new(serializedObject.FindProperty("Parameter"));
            PropertyField sendOnStartField = new(serializedObject.FindProperty("SendOnStart"));
            PropertyField isGlobalParameterField = new(serializedObject.FindProperty("IsGlobalParameter"));

            var helpBox = new HelpBox();
            helpBox.ElementAt(0).style.flexGrow = 1;
            helpBox.messageType = HelpBoxMessageType.Error;

            PropertyField onSendField = new(serializedObject.FindProperty("OnSend"));

            Button button = new(() => parameterSender.SendValue());
            button.text = "Send Parameter";
            button.AddToClassList("ButtonStyle");

            root.Add(root0);
            root0.Add(isGlobalParameterField);

            Color lineColor = Color.black;
            lineColor.a = 0.4f;

            VisualElement line = NKEditorUtility.Line(lineColor, 1.5f, 4f, 3f);

            root0.Add(line);
            root0.Add(behaviourStyleField);
            root0.Add(sourceField);

            root.Add(Space(5f));

            root.Add(root1);
            root1.Add(globalParameterFiled);

            var parameterLoad = serializedObject.FindProperty("previewEvent");
            var parameterLoadField = new PropertyField();
            parameterLoadField.BindProperty(parameterLoad);
            parameterLoadField.SetActive(false);
            parameterLoadField.label = "Parameter Load";
            root1.Add(parameterLoadField);

            VisualElement parameterArea = new();

            (VisualElement baseFieldLayout, Foldout titleToggleLayout) =
                _parameterValueView.InitParameterView(root1, parameterArea, parameterSender);

            parameterArea.name = "ParameterArea";

            VisualElement notFoundField = FMODEditorUtility.CreateNotFoundField();
            notFoundField.SetActive(false);
            root1.Add(notFoundField);

            root1.Add(parameterArea);
            root1.Add(helpBox);

            root1.Add(sendOnStartField);
            root.Add(Space(5f));
            root.Add(onSendField);
            root.Add(Space(5f));
            root.Add(button);

            //Init
            _oldIsGlobalParameter = parameterSender.IsGlobalParameter;
            _oldBehaviourStyle = parameterSender.BehaviourStyle;
            _oldPath = parameterLoad.FindPropertyRelative("Path").stringValue;

            VisualElement[] visualElements =
            {
                sourceField, onSendField, behaviourStyleField, line, globalParameterFiled,
                parameterArea, parameterLoadField
            };

            ControlField(visualElements);

            root.Add(new IMGUIContainer(() =>
            {
                baseFieldLayout.SetActive(false);

                if (!parameterSender.IsGlobalParameter)
                {
                    parameterArea.SetActive(false);
                    notFoundField.SetActive(false);

                    if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                        helpBox.SetActive(true);
                    else
                        helpBox.SetActive(false);

                    if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                        if (!parameterSender.Source)
                        {
                            _parameterValueView.Dispose();

                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                ? "FMOD Audio Source가 연결되어 있지 않습니다."
                                : "FMOD Audio Source is not connected.";

                            helpBox.text = msg;
                            return;
                        }

                    if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                    {
                        bool hasEvent = !string.IsNullOrWhiteSpace(parameterSender.Source.Clip.Path);
                        if (!hasEvent)
                        {
                            _parameterValueView.Dispose();

                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                ? "FMOD Audio Source에 Clip이 연결되어 있지 않습니다."
                                : "Clip is not connected to FMOD Audio Source.";

                            helpBox.text = msg;
                            return;
                        }
                    }

                    EditorEventRef existEvent;

                    if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                        existEvent = EventManager.EventFromPath(parameterSender.Source.Clip.Path);
                    else
                    {
                        SerializedProperty path = serializedObject.FindProperty("previewEvent")
                            .FindPropertyRelative("Path");
                        existEvent = EventManager.EventFromPath(path.stringValue);
                    }

                    if (existEvent != null)
                    {
                        if (!string.IsNullOrWhiteSpace(_oldPath))
                        {
                            //전이랑 현재랑 다르다면,
                            if (!_oldPath.Equals(existEvent.Path))
                            {
                                SerializedProperty paramsProperty = serializedObject.FindProperty("Params");
                                paramsProperty.ClearArray();

                                serializedObject.ApplyModifiedProperties();
                                _parameterValueView.DrawValues(true);
                                titleToggleLayout.value = false;
                            }
                        }
                        else
                        {
                            // SerializedProperty paramsProperty = serializedObject.FindProperty("Params");
                            // paramsProperty.ClearArray();

                            serializedObject.ApplyModifiedProperties();
                            _parameterValueView.DrawValues(true);
                            titleToggleLayout.value = false;
                        }

                        _oldPath = existEvent.Path;

                        helpBox.SetActive(false);
                    }
                    else
                    {
                        _parameterValueView.Dispose();
                        parameterArea.SetActive(false);
                        parameterArea.Clear();
                        _oldPath = string.Empty;

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

                    parameterArea.SetActive(true);
                    baseFieldLayout.SetActive(true);
                    helpBox.SetActive(false);
                }
                else
                {
                    helpBox.SetActive(false);

                    if (!parameterSender)
                        return;

                    if (string.IsNullOrWhiteSpace(parameterSender.Parameter))
                    {
                        _parameterValueView.Dispose();
                        notFoundField.SetActive(true);
                        return;
                    }

                    notFoundField.SetActive(false);

                    var editorParamRef = EventManager.ParamFromPath(parameterSender.Parameter);

                    if (editorParamRef == null)
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
                        _parameterValueView.RefreshEditorParamRef(editorParamRef);

                    if (_oldPath != parameterSender.Parameter)
                        _parameterValueView.DrawGlobalValues();

                    _oldPath = parameterSender.Parameter;

                    parameterArea.SetActive(true);
                }
            }));

            titleToggleLayout.RegisterValueChangedCallback(evt =>
            {
                bool isExpanded = evt.newValue;
                parameterArea.SetActive(isExpanded);
            });

            sourceField.RegisterValueChangedCallback(evt =>
            {
                if (!parameterSender.IsGlobalParameter)
                {
                    if (evt.newValue != null)
                        _parameterValueView.DrawValues(true);
                }
            });

            behaviourStyleField.RegisterValueChangeCallback(_ =>
            {
                if (_oldBehaviourStyle != parameterSender.BehaviourStyle)
                    _parameterValueView.Dispose();

                _oldBehaviourStyle = parameterSender.BehaviourStyle;

                ControlField(visualElements);
            });

            isGlobalParameterField.RegisterValueChangeCallback(evt =>
            {
                if (_oldIsGlobalParameter != evt.changedProperty.boolValue)
                {
                    parameterSender.Parameter = string.Empty;
                    _oldIsGlobalParameter = evt.changedProperty.boolValue;
                }

                ControlField(visualElements);
            });

            RuntimeActive(button);

            return root;
        }

        private void RuntimeActive(VisualElement element)
        {
            if (!EditorApplication.isPlaying)
            {
                element.tooltip = Application.systemLanguage == SystemLanguage.Korean
                    ? "에디터 모드에서는 사용하지 못합니다."
                    : "Can't use in Editor Mode.";
                element.SetEnabled(false);
            }
            else
            {
                element.tooltip = "Send Parameter.";
                element.SetEnabled(true);
            }
        }

        private void ControlField(VisualElement[] elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];
            var behaviourStyleField = elements[2];
            var line = elements[3];
            var globalParameterField = elements[4];
            var parameterArea = elements[5];
            var parameterLoadField = elements[6];

            foreach (VisualElement element in elements)
                element.SetActive(false);

            if (parameterSender.IsGlobalParameter)
            {
                globalParameterField.SetActive(true);
                parameterArea.style.marginLeft = 0;
            }
            else
            {
                parameterArea.style.marginLeft = 17;

                line.SetActive(true);
                behaviourStyleField.SetActive(true);

                if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                {
                    sourceField.SetActive(true);
                    parameterLoadField.SetActive(false);
                }
                else
                {
                    onSendField.SetActive(true);
                    parameterLoadField.SetActive(true);
                }
            }
        }


        private VisualElement Space(float height)
        {
            var space = new VisualElement();
            space.style.height = height;
            return space;
        }

        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private SerializedObject serializedObject;

            // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private EditorParamRef _editorParamRef;

            private Foldout _titleText;
            private DropdownField _addButton;
            private VisualElement _parameterArea;

            private class PropertyRecord
            {
                public string Name => paramRef.Name;

                public EditorParamRef paramRef;
                public List<SerializedProperty> valueProperties;
            }

            private FMODParameterSender _parameterSender;

            public ParameterValueView(SerializedObject serializedObject)
            {
                this.serializedObject = serializedObject;
            }

            public void Dispose()
            {
                _parameterSender.Params = Array.Empty<ParamRef>();
                _propertyRecords.Clear();
                _missingParameters.Clear();
                _editorParamRef = null;
                _titleText.value = false;
            }

            // propertyRecords 및 missingParameters 컬렉션을 다시 빌드합니다.
            public void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = serializedObject.FindProperty("Params");

                // 파라미터 한개씩 순례
                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record = _propertyRecords.Find(r => r.Name == name);
                    // 이미 존재할 경우 값 프로퍼티에 추가한다.
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

            public void RefreshEditorParamRef(EditorParamRef eventRef)
            {
                _editorParamRef = eventRef;
            }

            public Tuple<VisualElement, Foldout> InitParameterView(VisualElement root, VisualElement parameterArea,
                FMODParameterSender parameterSender)
            {
                _parameterArea = parameterArea;
                _parameterSender = parameterSender;

                VisualElement baseFieldLayout = new();
                VisualElement labelArea = new();
                VisualElement inputArea = new();

                _titleText = new();
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

                return new Tuple<VisualElement, Foldout>(baseFieldLayout, _titleText);
            }

            private void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("All"), false, () =>
                {
                    foreach (EditorParamRef parameter in _missingParameters)
                        AddParameter(parameter);

                    // 토글을 펼칩니다.
                    OpenParameterArea();
                    Refresh();
                    DrawValues();
                });

                menu.AddSeparator(string.Empty);

                foreach (EditorParamRef parameter in _missingParameters)
                {
                    menu.AddItem(new GUIContent(parameter.Name), false,
                        (userData) =>
                        {
                            AddParameter(userData as EditorParamRef);

                            // 토글을 펼칩니다.
                            OpenParameterArea();
                            Refresh();
                            DrawValues();
                        },
                        parameter);
                }

                menu.DropDown(position);
            }

            private void OpenParameterArea()
            {
                _titleText.value = true;
            }

            private void Refresh()
            {
                string path;
                if (_parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                    path = _parameterSender.Source.Clip.Path;
                else
                {
                    var previewEvent = serializedObject.FindProperty("previewEvent").FindPropertyRelative("Path");
                    path = previewEvent.stringValue;
                }

                EditorEventRef refreshEvent = EventManager.EventFromPath(path);
                RefreshPropertyRecords(refreshEvent);
                CalculateEnableAddButton();
            }

            public void DrawValues(bool preRefresh = false)
            {
                if (preRefresh)
                {
                    if (_parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                    {
                        if (_parameterSender.Source)
                        {
                            var path = _parameterSender.Source.Clip.Path;
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                EditorEventRef eventRef = EventManager.EventFromPath(path);
                                RefreshPropertyRecords(eventRef);
                            }
                        }
                    }
                    else
                    {
                        var previewEvent = serializedObject.FindProperty("previewEvent").FindPropertyRelative("Path");
                        var path = previewEvent.stringValue;

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            EditorEventRef eventRef = EventManager.EventFromPath(path);
                            RefreshPropertyRecords(eventRef);
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();

                // parameterArea 자식들은 모두 제거하기
                _parameterArea.schedule.Execute(() => _parameterArea.Clear());
                
                foreach (PropertyRecord record in _propertyRecords)
                    _parameterArea.schedule.Execute(() => _parameterArea.Add(AdaptiveParameterField(record)));

                if (preRefresh)
                    CalculateEnableAddButton();
            }

            public void DrawGlobalValues(bool preRefresh = false)
            {
                if (preRefresh)
                    _editorParamRef = EventManager.ParamFromPath(_parameterSender.Parameter);

                if (_editorParamRef == null)
                    return;

                var value = serializedObject.FindProperty("Value");

                value.floatValue =
                    Mathf.Clamp(value.floatValue, _editorParamRef.Min, _editorParamRef.Max);

                serializedObject.ApplyModifiedProperties();

                _parameterArea.schedule.Execute(()=>_parameterArea.Clear());
                _parameterArea.schedule.Execute(() => _parameterArea.Add(AdaptiveParameterField(_editorParamRef)));
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
                    {
                        if (first)
                        {
                            value = property.floatValue;
                            first = false;
                        }
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

                        baseField.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                        {
                            foreach (SerializedProperty property in record.valueProperties)
                                property.floatValue = evt.newValue;

                            serializedObject.ApplyModifiedProperties();
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

                            serializedObject.ApplyModifiedProperties();
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
                        };

                        dropdown.index = (int)value;

                        baseField.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ =>
                        {
                            foreach (SerializedProperty property in record.valueProperties)
                                property.floatValue = dropdown.index;

                            serializedObject.ApplyModifiedProperties();
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

            private SimpleBaseField AdaptiveParameterField(EditorParamRef editorParamRef)
            {
                var globalParameterLayout = new SimpleBaseField
                {
                    name = $"{editorParamRef.Name} Field Layout",
                    Label = "Override Value",
                    style =
                    {
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

                switch (editorParamRef.Type)
                {
                    // 여기에서 Value에 알맞는 값으 전달해야함.
                    case ParameterType.Continuous:
2323
                        var floatSlider = new Slider(editorParamRef.Min, editorParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = editorParamRef.Default
                        };

                        globalParameterLayout.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                            _parameterSender.Value = evt.newValue);

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)editorParamRef.Min, (int)editorParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)editorParamRef.Default
                        };

                        globalParameterLayout.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt =>
                            _parameterSender.Value = evt.newValue);

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = editorParamRef.Labels.ToList(),
                            index = (int)editorParamRef.Default
                        };

                        globalParameterLayout.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ =>
                            _parameterSender.Value = dropdown.index);

                        break;
                }

                return globalParameterLayout;
            }

            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = serializedObject.FindProperty("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    if (child.FindPropertyRelative("Name").stringValue == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_parameterSender.Params, p => p.Name == parameter.Name) < 0)
                {
                    SerializedProperty paramsProperty = serializedObject.FindProperty("Params");

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif