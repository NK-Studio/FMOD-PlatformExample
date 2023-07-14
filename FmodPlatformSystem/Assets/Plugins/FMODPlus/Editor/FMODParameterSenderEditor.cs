#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
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
        [SerializeField] private EditorParamRef editorParamRef;

        private bool _oldIsGlobalParameter;
        private string _currentPath;

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
            PropertyField sourceField = new(serializedObject.FindProperty("Source"));

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            TextField parameterFiled = new();
            parameterFiled.label = "Parameter";
            parameterFiled.BindProperty(serializedObject.FindProperty("Parameter"));
            parameterFiled.AddToClassList("unity-base-field__aligned");

            PropertyField globalParameterFiled = new(serializedObject.FindProperty("Parameter"));

            PropertyField valueField = new(serializedObject.FindProperty("Value"));
            PropertyField sendOnStartField = new(serializedObject.FindProperty("SendOnStart"));
            PropertyField isGlobalParameterField = new(serializedObject.FindProperty("IsGlobalParameter"));

            var helpBox = new HelpBox();
            helpBox.text = "Audio Source가 연결되어 있지 않습니다.";
            helpBox.messageType = HelpBoxMessageType.Error;

            PropertyField onSendField = new(serializedObject.FindProperty("OnSend"));

            Button button = new(() => parameterSender.SendValue());
            button.text = "Send Parameter";
            button.AddToClassList("ButtonStyle");

            root.Add(root0);
            root0.Add(isGlobalParameterField);

            Color lineColor = Color.black;
            lineColor.a = 0.4f;

            VisualElement line = Line(lineColor, 1.5f, 4f, 3f);

            root0.Add(line);
            root0.Add(behaviourStyleField);
            root0.Add(sourceField);

            root.Add(Space(5f));

            root.Add(root1);
            root1.Add(parameterFiled);
            root1.Add(globalParameterFiled);

            VisualElement baseFieldLayout = _parameterValueView.InitParameterView(root1, parameterSender);

            root1.Add(valueField);
            root1.Add(helpBox);

            root1.Add(sendOnStartField);
            root.Add(Space(5f));
            root.Add(onSendField);
            root.Add(Space(5f));
            root.Add(button);

            VisualElement[] visualElements =
            {
                sourceField, onSendField, behaviourStyleField, line, parameterFiled, globalParameterFiled,
                baseFieldLayout, valueField, helpBox
            };

            //Init
            _oldIsGlobalParameter = parameterSender.IsGlobalParameter;

            ControlField(visualElements);
            RefreshParameterSenderValue();

            root.Add(new IMGUIContainer(() =>
            {
                SetActiveField(helpBox, true);

                if (!parameterSender.Source)
                {
                    helpBox.text = "FMOD Audio Source가 연결되어 있지 않습니다.";
                    return;
                }

                bool hasEvent = string.IsNullOrWhiteSpace(parameterSender.Source.Clip.Path);
                if (hasEvent)
                {
                    helpBox.text = "FMOD Audio Source에 Clip이 연결되어 있지 않습니다.";
                    return;
                }

                EditorEventRef existEvent = EventManager.EventFromPath(parameterSender.Source.Clip.Path);

                if (existEvent != null)
                {
                    SetActiveField(helpBox, false);
                }
                else
                {
                    helpBox.text = "연결된 이벤트 주소가 유효하지 않습니다.";
                    return;
                }

                // 스타일이 Base 방식일 때만 처리로 현재는 되어있다.
                if (Event.current.type == EventType.Layout)
                    _parameterValueView.RefreshPropertyRecords(existEvent, parameterSender.Source.Params);

                SetActiveField(baseFieldLayout, true);
                SetActiveField(helpBox, false);
            }));

            // behaviourStyleField.RegisterValueChangeCallback(_ =>
            //     ControlField(visualElements));

            // isGlobalParameterField.RegisterValueChangeCallback(evt =>
            // {
            //     if (_oldIsGlobalParameter != evt.changedProperty.boolValue)
            //     {
            //         parameterSender.Parameter = string.Empty;
            //         _oldIsGlobalParameter = evt.changedProperty.boolValue;
            //     }
            //
            //     ControlField(visualElements);
            // });

            if (!EditorApplication.isPlaying)
            {
                button.tooltip = Application.systemLanguage == SystemLanguage.Korean
                    ? "에디터 모드에서는 사용하지 못합니다."
                    : "Can't use in Editor Mode.";
                button.SetEnabled(false);
            }
            else
            {
                button.tooltip = "Send Parameter.";
                button.SetEnabled(true);
            }

            return root;

            void RefreshParameterSenderValue()
            {
                if (!parameterSender.IsGlobalParameter)
                    return;

                if (parameterSender.Parameter != _currentPath)
                {
                    _currentPath = parameterSender.Parameter;

                    if (string.IsNullOrEmpty(parameterSender.Parameter))
                    {
                        editorParamRef = null;
                    }
                    else
                    {
                        editorParamRef = EventManager.ParamFromPath(parameterSender.Parameter);
                        parameterSender.Value =
                            Mathf.Clamp(parameterSender.Value, editorParamRef.Min, editorParamRef.Max);
                    }

                    //  RefreshGlobalParameterField(simpleBaseField);
                }
            }
        }


        private void ControlField(VisualElement[] elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];
            var behaviourStyleField = elements[2];
            var line = elements[3];
            var parameterFiled = elements[4];
            var globalParameterField = elements[5];
            var simpleBaseField = elements[6];
            var valueField = elements[7];
            var helpBox = elements[8] as HelpBox;

            foreach (VisualElement element in elements)
                SetActiveField(element, false);

            SetActiveField(sourceField, true);


            //  if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
            SetActiveField(sourceField, true);

            //  SetActiveField(onSendField, true);
            // else

            // if (parameterSender.IsGlobalParameter)
            // {
            //     SetActiveField(sourceField, true);
            //     SetActiveField(onSendField, true);
            //     SetActiveField(behaviourStyleField, true);
            //     SetActiveField(line, true);
            //     SetActiveField(parameterFiled, true);
            //     SetActiveField(valueField, true);
            // }
            // else
            // {
            //     SetActiveField(globalParameterField, true);
            //     SetActiveField(simpleBaseField, true);
            // }
        }

        private void SetActiveField(VisualElement field, bool active)
        {
            field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement Space(float height)
        {
            var space = new VisualElement();
            space.style.height = height;
            return space;
        }

        private VisualElement Line(Color color, float height, float topBottomMargin = 1f, float leftRightMargin = 0f)
        {
            var line = new VisualElement
            {
                style =
                {
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

        private void RefreshGlobalParameterField(SimpleBaseField simpleBaseField)
        {
            try
            {
                int childElement = simpleBaseField.contentContainer.childCount;


                if (childElement == 1)
                    simpleBaseField.contentContainer.RemoveAt(0);

                if (editorParamRef != null)
                {
                    simpleBaseField.Label = "Override Value";

                    var content = new IMGUIContainer();
                    content.onGUIHandler = () =>
                    {
                        parameterSender.Value =
                            EditorUtils.DrawParameterValueLayout(parameterSender.Value, editorParamRef);
                    };

                    simpleBaseField.contentContainer.Add(content);
                }
                else
                {
                    simpleBaseField.Label = string.Empty;

                    Texture2D warningIcon = EditorUtils.LoadImage("NotFound.png");

                    var icon = new VisualElement();
                    icon.style.backgroundImage = new StyleBackground(warningIcon);
                    icon.style.width = warningIcon.width;
                    icon.style.height = warningIcon.height;

                    var textField = new Label();
                    textField.text = "Parameter Not Found";

                    var innerContainer = new VisualElement
                    {
                        name = "innerContainer",
                        style =
                        {
                            flexDirection = FlexDirection.Row
                        }
                    };

                    innerContainer.Add(icon);
                    innerContainer.Add(textField);
                    simpleBaseField.contentContainer.Add(innerContainer);
                }
            }
            catch (Exception)
            {
            }
        }

        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private SerializedObject _serializedTargets;

            // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private class PropertyRecord
            {
                public string Name => paramRef.Name;

                public EditorParamRef paramRef;
                public ParamRef valueProperties;
            }

            private FMODParameterSender _parameterSender;

            public ParameterValueView(SerializedObject serializedTargets)
            {
                _serializedTargets = serializedTargets;
            }

            // propertyRecords 및 missingParameters 컬렉션을 다시 빌드합니다.
            public void RefreshPropertyRecords(EditorEventRef eventRef, ParamRef[] paramRefs)
            {
                _propertyRecords.Clear();

                // 파라미터 한개씩 순례
                foreach (ParamRef parameterProperty in paramRefs)
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

                Debug.Log(_missingParameters.Count);
            }
            

            public VisualElement InitParameterView(VisualElement root, FMODParameterSender parameterSender)
            {
                _parameterSender = parameterSender;

                VisualElement baseFieldLayout = new();
                VisualElement labelArea = new();
                VisualElement inputArea = new();

                Foldout titleText = new();
                titleText.text = "Initial Parameter Values";
                DropdownField addButton = new DropdownField();
                addButton.value = "Add";
                addButton.style.flexGrow = 1;
                addButton.style.marginLeft = 0;

                root.Add(baseFieldLayout);
                baseFieldLayout.Add(labelArea);
                baseFieldLayout.Add(inputArea);

                labelArea.Add(titleText);
                inputArea.Add(addButton);
                addButton.RegisterCallback<MouseDownEvent>(_ =>
                {
                    labelArea.SendEvent(new ExecuteCommandEvent());
                    //DrawAddButton(addButton.worldBound);
                });
                addButton.RegisterCallback<ExecuteCommandEvent>(evt =>
                {
                    
                    Debug.Log(_missingParameters.Count);
                });
                NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

                return baseFieldLayout;
            }

            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("All"), false, () =>
                {
                    foreach (EditorParamRef parameter in _missingParameters)
                    {
                        AddParameter(parameter);
                        Refresh();
                    }
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
                    //titleText.value = true;

                    var refreshEvent = EventManager.EventFromPath(_parameterSender.Source.Clip.Path);
                    RefreshPropertyRecords(refreshEvent, _parameterSender.Params);
                }
            }

            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_parameterSender.Params, p => p.Name == parameter.Name) < 0)
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
        }
    }
}
#endif

// var aa = new PropertyField(serializedObject.FindProperty("Value"));
// aa.style.marginLeft = 17;
// root1.Add(aa);