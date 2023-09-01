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
using static NKStudio.NKEditorUtility;
using Object = UnityEngine.Object;

namespace FMODPlus
{
    [CustomEditor(typeof(FMODParameterSender))]
    [CanEditMultipleObjects]
    public class FMODParameterSenderEditor : Editor
    {
        private FMODParameterSender _parameterSender;
        private ParameterValueView _parameterValueView;

        private StyleSheet _groupBoxStyleSheet;
        private StyleSheet _buttonStyleSheet;

        private string _oldPath;
        private string _oldParameterPath;
        private string _oldTargetPath;

        private SerializedProperty _behaviourStyle;
        private SerializedProperty _params;
        private SerializedProperty _audioSource;
        private SerializedProperty _parameter;
        private SerializedProperty _value;
        private SerializedProperty _sendOnStart;
        private SerializedProperty _isGlobalParameter;
        private SerializedProperty _onSend;
        private SerializedProperty _previewEvent;
        private SerializedProperty _previewEventPath;

        private Object _oldSource;
        private AudioBehaviourStyle _oldBehaviourStyle;
        private bool _oldIsGlobalParameter;

        private VisualElement _root;

        private void OnEnable()
        {
            _parameterValueView = new ParameterValueView(serializedObject);

            // Parameter Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("74cfbd073c7464035ba232171ef31f0f");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("6531cd3743c664274b21aa41c9b00c5c");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("0842e81344c5e4019b977bcb20b7266b");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupStyleSheetPath = AssetDatabase.GUIDToAssetPath("5600a59cbafd24acf808fa415167310e");
            _groupBoxStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupStyleSheetPath);

            string buttonStyleSheetPath = AssetDatabase.GUIDToAssetPath("db197c96211fc47319d2b84dcd02aacd");
            _buttonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(buttonStyleSheetPath);
        }

        private void FindProperty()
        {
            _audioSource = serializedObject.FindProperty("Source");
            _params = serializedObject.FindProperty("Params");
            _behaviourStyle = serializedObject.FindProperty("BehaviourStyle");
            _parameter = serializedObject.FindProperty("Parameter");
            _value = serializedObject.FindProperty("Value");
            _sendOnStart = serializedObject.FindProperty("SendOnStart");
            _isGlobalParameter = serializedObject.FindProperty("IsGlobalParameter");
            _onSend = serializedObject.FindProperty("OnSend");
            _previewEvent = serializedObject.FindProperty("previewEvent");
            _previewEventPath = serializedObject.FindProperty("previewEvent").FindPropertyRelative("Path");
        }

        private void InitializeRoot()
        {
            _root = new VisualElement();
            _root.styleSheets.Add(_groupBoxStyleSheet);
            _root.styleSheets.Add(_buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            PropertyField behaviourStyleField = new(_behaviourStyle);
            PropertyField globalParameterFiled = new(_parameter);
            PropertyField sendOnStartField = new(_sendOnStart);
            PropertyField isGlobalParameterField = new(_isGlobalParameter);
            PropertyField onSendField = new(_onSend);

            ObjectField sourceField = new();
            sourceField.objectType = typeof(FMODAudioSource);
            sourceField.label = "Audio Source";
            sourceField.bindingPath = "Source";
            sourceField.AddToClassList("unity-base-field__aligned");

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            HelpBox helpBox = new HelpBox();
            helpBox.ElementAt(0).style.flexGrow = 1;
            helpBox.messageType = HelpBoxMessageType.Error;

            Button button = new();
            button.clicked += () => _parameterSender.SendValue();
            button.text = "Send Parameter";
            button.AddToClassList("ButtonStyle");

            Color lineColor = Color.black;
            lineColor.a = 0.4f;

            VisualElement line = Line(lineColor, 1.5f, 4f, 3f);

            PropertyField parameterLoadField = new(_previewEvent);
            parameterLoadField.label = "Parameter Load";

            VisualElement parameterArea = new();
            parameterArea.style.marginLeft = 15;
            parameterArea.name = "ParameterArea";

            (VisualElement initialParameterField, Foldout titleToggleLayout) =
                _parameterValueView.InitParameterView(parameterArea, _parameterSender);

            VisualElement notFoundField = FMODEditorUtility.CreateNotFoundField();
            notFoundField.SetActive(false);

            _root.Add(root0);
            root0.Add(isGlobalParameterField);
            root0.Add(line);
            root0.Add(behaviourStyleField);
            root0.Add(sourceField);
            _root.Add(Space(5f));
            _root.Add(root1);
            root1.Add(globalParameterFiled);
            root1.Add(parameterLoadField);
            root1.Add(initialParameterField);
            root1.Add(notFoundField);
            root1.Add(parameterArea);
            root1.Add(helpBox);
            root1.Add(sendOnStartField);
            _root.Add(Space(5f));
            _root.Add(onSendField);
            _root.Add(Space(5f));
            _root.Add(button);

            VisualElement[] visualElements = {
                sourceField, onSendField, behaviourStyleField, line, globalParameterFiled,
                parameterArea, parameterLoadField, helpBox, initialParameterField, sendOnStartField, titleToggleLayout, isGlobalParameterField, notFoundField
            };

            // Init
            InitControlField(visualElements);

            RuntimeActive(button);
        }

        private void RegisterAudioSourcePathValueChange(Action<string> callback)
        {
            _root.schedule.Execute(() => {
                if (_parameterSender.Source)
                    if (_oldTargetPath != _parameterSender.Source.Clip.Path)
                    {
                        callback.Invoke(_parameterSender.Source.Clip.Path);
                        _oldTargetPath = _parameterSender.Source.Clip.Path;
                    }
            }).Every(5);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _parameterSender = (FMODParameterSender)target;
            FindProperty();
            InitializeRoot();
            return _root;
        }

        /// <summary>
        /// 초기에 간단한 선언들과 Callback 등록과 같은 간단한 수행함 처리함
        /// </summary>
        /// <param name="elements"></param>
        private void InitControlField(VisualElement[] elements)
        {
            var sourceField = (ObjectField)elements[0];
            var behaviourStyleField = (PropertyField)elements[2];
            var globalParameterField = (PropertyField)elements[4];
            var parameterArea = elements[5];
            var parameterLoadField = (PropertyField)elements[6];
            var titleToggleLayout = (Foldout)elements[10];
            var isGlobalParameterField = (PropertyField)elements[11];

            ControlField(elements);

            RegisterAudioSourcePathValueChange(_ => {
                ControlField(elements);
            });

            globalParameterField.RegisterValueChangeCallback(_parameter, _oldParameterPath, _ => {
                ControlField(elements);
            });

            parameterLoadField.RegisterValueChangeCallback(_previewEvent, _oldPath, _ => {
                _params.ClearArray();
                _parameterValueView.Dispose();
                serializedObject.ApplyModifiedProperties();
                titleToggleLayout.Close();

                ControlField(elements);
            });

            titleToggleLayout.RegisterValueChangedCallback(evt => {
                bool isExpanded = evt.newValue;
                parameterArea.SetActive(isExpanded);
            });

            _oldSource = _audioSource.objectReferenceValue;
            sourceField.RegisterValueChangedCallback(evt => {
                if (_oldSource != evt.newValue)
                {
                    ControlField(elements);
                    _oldSource = evt.newValue;
                }
            });

            _oldBehaviourStyle = (AudioBehaviourStyle)_behaviourStyle.enumValueIndex;
            behaviourStyleField.RegisterValueChangeCallback(evt => {
                var newBehaviourStyle = (AudioBehaviourStyle)evt.changedProperty.enumValueIndex;
                if (_oldBehaviourStyle != newBehaviourStyle)
                {
                    _parameterValueView.Dispose();
                    ControlField(elements);
                    _oldBehaviourStyle = newBehaviourStyle;
                }
            });

            _oldIsGlobalParameter = _isGlobalParameter.boolValue;
            isGlobalParameterField.RegisterValueChangeCallback(evt => {
                if (_oldIsGlobalParameter != evt.changedProperty.boolValue)
                {
                    _parameter.stringValue = string.Empty;
                    _params.ClearArray();
                    _parameterValueView.Dispose();
                    _value.floatValue = 0f;
                    serializedObject.ApplyModifiedProperties();
                    titleToggleLayout.Close();
                    ControlField(elements);
                    _oldIsGlobalParameter = evt.changedProperty.boolValue;
                }
            });
        }

        /// <summary>
        /// Init되었을 때 보여줘야 하는 처리들과 트리거 되었을 때 보여줘야하는 모든 처리를 수행함
        /// </summary>
        /// <param name="elements"></param>
        private void ControlField(IReadOnlyList<VisualElement> elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];
            var behaviourStyleField = elements[2];
            var line = elements[3];
            var globalParameterField = elements[4];
            var parameterArea = elements[5];
            var parameterLoadField = elements[6];
            var helpBox = (HelpBox)elements[7];
            var initialParameterField = elements[8];
            var sendOnStartField = elements[9];
            var titleToggleLayout = (Foldout)elements[10];
            var isGlobalParameterField = (PropertyField)elements[11];
            var notFoundField = elements[12];


            foreach (VisualElement element in elements)
                element.SetActive(false);

            isGlobalParameterField.SetActive(true);

            if (_isGlobalParameter.boolValue)
            {
                globalParameterField.SetActive(true);
                parameterArea.style.marginLeft = 0;

                if (!string.IsNullOrWhiteSpace(_parameter.stringValue))
                {
                    var existEvent = EventManager.ParamFromPath(_parameter.stringValue);

                    if (existEvent != null)
                    {
                        _parameterValueView.DrawGlobalValues(true);
                        helpBox.SetActive(false);
                        notFoundField.SetActive(false);
                        parameterArea.SetActive(true);
                        sendOnStartField.SetActive(true);
                    }
                    else
                    {
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "연결된 이벤트 주소가 유효하지 않습니다."
                            : "The connected event address is invalid.";
                        helpBox.text = msg;
                        helpBox.SetActive(true);
                        notFoundField.SetActive(true);
                        parameterArea.SetActive(false);
                        sendOnStartField.SetActive(false);
                    }
                }
                else
                {
                    var editorParamRef = EventManager.ParamFromPath(_parameter.stringValue);

                    if (editorParamRef == null)
                    {
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "연결된 이벤트 주소가 유효하지 않습니다."
                            : "The connected event address is invalid.";
                        helpBox.text = msg;
                        helpBox.SetActive(true);
                        notFoundField.SetActive(true);
                        parameterArea.SetActive(false);
                        sendOnStartField.SetActive(false);
                    }
                }
            }
            else // Local Parameter
            {
                parameterArea.style.marginLeft = 17;

                line.SetActive(true);
                behaviourStyleField.SetActive(true);

                var behaviourStyle = (FMODParameterSender.AudioBehaviourStyle)_behaviourStyle.enumValueIndex;

                if (behaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
                {
                    sourceField.SetActive(true);
                    parameterLoadField.SetActive(false);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        var targetAudioSource = new SerializedObject(_audioSource.objectReferenceValue);
                        var targetPath = targetAudioSource.FindProperty("clip").FindPropertyRelative("Path");

                        bool hasEvent = !string.IsNullOrWhiteSpace(targetPath.stringValue);
                        if (hasEvent)
                        {
                            EditorEventRef existEvent = EventManager.EventFromPath(targetPath.stringValue);

                            if (existEvent != null)
                            {
                                // 스타일이 Base 방식일 때만 처리로 현재는 되어있다.
                                _parameterValueView.RefreshPropertyRecords(existEvent);
                                _parameterValueView.DrawValues();
                                _parameterValueView.CalculateEnableAddButton();

                                titleToggleLayout.SetActive(true);
                                parameterArea.SetActive(true);
                                initialParameterField.SetActive(true);
                                helpBox.SetActive(false);
                                sendOnStartField.SetActive(true);
                            }
                            else
                            {
                                string msg = Application.systemLanguage == SystemLanguage.Korean
                                    ? "연결된 이벤트 주소가 유효하지 않습니다."
                                    : "The connected event address is invalid.";
                                helpBox.text = msg;

                                _oldPath = string.Empty;
                                _parameterValueView.Dispose();
                                titleToggleLayout.Close();
                                parameterArea.Clear();

                                initialParameterField.SetActive(false);
                                parameterArea.SetActive(false);
                                sendOnStartField.SetActive(false);
                                helpBox.SetActive(true);
                            }
                        }
                        else
                        {
                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                ? "FMOD Audio Source에 Clip이 연결되어 있지 않습니다."
                                : "Clip is not connected to FMOD Audio Source.";

                            helpBox.text = msg;
                            helpBox.SetActive(true);
                            titleToggleLayout.SetActive(false);
                            parameterArea.SetActive(false);
                            initialParameterField.SetActive(false);
                            sendOnStartField.SetActive(false);
                        }
                    }
                    else
                    {

                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "FMOD Audio Source가 연결되어 있지 않습니다."
                            : "FMOD Audio Source is not connected.";

                        helpBox.text = msg;
                        helpBox.SetActive(true);
                        titleToggleLayout.SetActive(false);
                        parameterArea.SetActive(false);
                        initialParameterField.SetActive(false);
                        sendOnStartField.SetActive(false);
                    }
                }
                else // FMODParameterSender.AudioBehaviourStyle.API
                {
                    onSendField.SetActive(true);

                    if (_parameterSender.IsGlobalParameter)
                        parameterLoadField.SetActive(false);
                    else
                    {
                        parameterLoadField.SetActive(true);

                        if (!string.IsNullOrWhiteSpace(_previewEventPath.stringValue))
                        {
                            var existEvent = EventManager.EventFromPath(_previewEventPath.stringValue);

                            if (existEvent != null)
                            {
                                _parameterValueView.RefreshPropertyRecords(existEvent);
                                _parameterValueView.DrawValues();
                                _parameterValueView.CalculateEnableAddButton();

                                titleToggleLayout.SetActive(true);
                                parameterArea.SetActive(true);
                                initialParameterField.SetActive(true);
                                helpBox.SetActive(false);
                                sendOnStartField.SetActive(true);
                            }
                            else
                            {
                                string msg = Application.systemLanguage == SystemLanguage.Korean
                                    ? "연결된 이벤트 주소가 유효하지 않습니다."
                                    : "The connected event address is invalid.";
                                helpBox.text = msg;
                                helpBox.SetActive(true);
                                titleToggleLayout.SetActive(false);
                                parameterArea.SetActive(false);
                                initialParameterField.SetActive(false);
                                sendOnStartField.SetActive(false);
                            }
                        }
                        else
                        {
                            helpBox.SetActive(true);
                            titleToggleLayout.SetActive(false);
                            parameterArea.SetActive(false);
                            initialParameterField.SetActive(false);
                            helpBox.SetActive(false);
                            sendOnStartField.SetActive(false);
                        }
                    }
                }
            }
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

        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private SerializedObject _serializedObject;

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
                public string Name => ParamRef.Name;

                public EditorParamRef ParamRef;
                public List<SerializedProperty> ValueProperties;
            }

            private FMODParameterSender _parameterSender;

            public ParameterValueView(SerializedObject serializedObject)
            {
                this._serializedObject = serializedObject;
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

                SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

                // 파라미터 한개씩 순례
                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record = _propertyRecords.Find(r => r.Name == name);
                    // 이미 존재할 경우 값 프로퍼티에 추가한다.
                    if (record != null)
                        record.ValueProperties.Add(valueProperty);
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);

                        if (paramRef != null)
                        {
                            _propertyRecords.Add(
                                new PropertyRecord() {
                                    ParamRef = paramRef,
                                    ValueProperties = new List<SerializedProperty>() { valueProperty }
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

            public Tuple<VisualElement, Foldout> InitParameterView(VisualElement parameterArea,
                FMODParameterSender parameterSender)
            {
                _parameterArea = parameterArea;
                _parameterSender = parameterSender;

                VisualElement baseFieldLayout = new();
                VisualElement labelArea = new();
                VisualElement inputArea = new();

                _titleText = new();
                _titleText.text = "Initial Parameter Values";

                baseFieldLayout.name = "Initial Parameter Field";
                baseFieldLayout.style.marginLeft = 15;

                _addButton = new DropdownField();
                _addButton.value = "Add";
                _addButton.style.flexGrow = 1;
                _addButton.style.marginLeft = 0;

                baseFieldLayout.Add(labelArea);
                baseFieldLayout.Add(inputArea);

                labelArea.Add(_titleText);
                inputArea.Add(_addButton);
                _addButton.RegisterCallback<ClickEvent>(_ => DrawAddButton(_addButton.worldBound));

                ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

                return new Tuple<VisualElement, Foldout>(baseFieldLayout, _titleText);
            }

            public void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new();
                menu.AddItem(new GUIContent("All"), false, () => {
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
                        (userData) => {
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
                    var previewEvent = _serializedObject.FindProperty("previewEvent").FindPropertyRelative("Path");
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
                        var previewEvent = _serializedObject.FindProperty("previewEvent").FindPropertyRelative("Path");
                        var path = previewEvent.stringValue;

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            EditorEventRef eventRef = EventManager.EventFromPath(path);
                            RefreshPropertyRecords(eventRef);
                        }
                    }
                }

                _serializedObject.ApplyModifiedProperties();

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

                var value = _serializedObject.FindProperty("Value");

                value.floatValue =
                    Mathf.Clamp(value.floatValue, _editorParamRef.Min, _editorParamRef.Max);

                _serializedObject.ApplyModifiedProperties();

                _parameterArea.Clear();
                _parameterArea.Add(AdaptiveParameterField(_editorParamRef));
            }

            private SimpleBaseField AdaptiveParameterField(PropertyRecord record)
            {
                float value = 0;

                if (record.ValueProperties.Count == 1)
                    value = record.ValueProperties[0].floatValue;
                else
                {
                    bool first = true;

                    foreach (SerializedProperty property in record.ValueProperties)
                    {
                        if (first)
                        {
                            value = property.floatValue;
                            first = false;
                        }
                    }
                }

                var baseField = new SimpleBaseField {
                    Label = record.Name,
                    style = {
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

                switch (record.ParamRef.Type)
                {
                    case ParameterType.Continuous:

                        var floatSlider = new Slider(record.ParamRef.Min, record.ParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = value
                        };

                        baseField.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;

                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)record.ParamRef.Min, (int)record.ParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)value
                        };

                        baseField.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;

                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = record.ParamRef.Labels.ToList(),
                        };

                        dropdown.index = (int)value;

                        baseField.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = dropdown.index;

                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                }

                var btn = new Button {
                    text = "Remove",
                    style = {
                        marginRight = 0f
                    }
                };

                baseField.contentContainer.Add(btn);

                btn.clicked += () => {
                    DeleteParameter(record.Name);
                    DrawValues(true);
                };

                return baseField;
            }

            private SimpleBaseField AdaptiveParameterField(EditorParamRef editorParamRef)
            {
                var globalParameterLayout = new SimpleBaseField {
                    name = $"{editorParamRef.Name} Field Layout",
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

                switch (editorParamRef.Type)
                {
                    // 여기에서 Value에 알맞는 값으 전달해야함.
                    case ParameterType.Continuous:

                        var floatSlider = new Slider(editorParamRef.Min, editorParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = _parameterSender.Value
                        };

                        globalParameterLayout.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                            _parameterSender.Value = evt.newValue);

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)editorParamRef.Min, (int)editorParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)_parameterSender.Value
                        };

                        globalParameterLayout.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt =>
                            _parameterSender.Value = evt.newValue);

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = editorParamRef.Labels.ToList(),
                            index = (int)_parameterSender.Value
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
                SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    if (child.FindPropertyRelative("Name").stringValue == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                _serializedObject.ApplyModifiedProperties();
            }

            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_parameterSender.Params, p => p.Name == parameter.Name) < 0)
                {
                    SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif
