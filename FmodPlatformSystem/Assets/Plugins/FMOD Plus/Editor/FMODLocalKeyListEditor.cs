using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using NKStudio;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using ListView = UnityEngine.UIElements.ListView;

namespace FMODPlus
{
    [CustomEditor(typeof(LocalKeyList))]
    public class FMODLocalKeyListEditor : Editor
    {
        private List<ParameterValueView> _parameterValueView;
        private List<KeyAndPath> _oldRefAndKey = new();

        //private ReorderableList _reorderableList;
        //private ReorderableList _searchList;
        private LocalKeyList _localKeyList;

        private float _lineHeight;
        private float _lineHeightSpacing;
        private string _searchText = string.Empty;

        private const string kClips = "Clips";
        private const string kCachedSearchClips = "cachedSearchClips";
        private const string kKey = "Key";
        private const string kGUID = "GUID";
        private const string kShowInfo = "ShowInfo";
        private const string kValue = "Value";
        private const string kPath = "Path";
        private const string kParams = "Params";
        private const string kName = "Name";
        private const string kDefaultKey = "New Key";
        private const string kStageTextField = "StageTextField";

        private SerializedProperty clipList;
        private SerializedProperty cachedClipList;

        [SerializeField]
        private VisualTreeAsset fmodLocalKeyListUXML;
        [SerializeField]
        private VisualTreeAsset KeyListItemUXML;
        [SerializeField]
        private VisualTreeAsset UnbindItemUXML;

        private VisualElement _root;
        private Label _numberLabel;
        private ListView _reorderableList;

        /// <summary>
        /// 프로퍼티를 찾습니다.
        /// </summary>
        private void FindProperties()
        {
            clipList = serializedObject.FindProperty("EventRefList");
            _localKeyList = (LocalKeyList)serializedObject.targetObject;
        }

        /// <summary>
        /// 초기화하는 내용이 들어갑니다.
        /// </summary>
        private void Init()
        {
            _parameterValueView = new List<ParameterValueView>();
            for (int i = 0; i < clipList.arraySize; i++)
                _parameterValueView.Add(new ParameterValueView(serializedObject));
        }

        /// <summary>
        /// 요소를 만들어냅니다.
        /// </summary>
        private void MakeElement()
        {
            _root = fmodLocalKeyListUXML.Instantiate();
            _reorderableList = _root.Q<ListView>("KeyList");
            _numberLabel = _root.Q<Label>("Number");
        }

        /// <summary>
        /// 바인딩하는 내용이 들어갑니다.
        /// </summary>
        private void BindElement()
        {
            _numberLabel.text = clipList.arraySize.ToString();
        }

        /// <summary>
        /// Callback을 등록합니다.
        /// </summary>
        private void RegisterCallback()
        {
            _reorderableList.itemsSource = _localKeyList.EventRefList;
            _reorderableList.unbindItem = UnbindItem;
            _reorderableList.makeItem = MakeItem;
            _reorderableList.bindItem = BindItem;
            _reorderableList.itemsAdded += AddItem;
            _reorderableList.itemsRemoved += OnRemoveItem;
            _root.Q<Button>("reset-button").clicked += OnResetButton;
        }

        /// <summary>
        /// 컨트롤 하는 내용이 들어갑니다.
        /// </summary>
        private void Control()
        {
            for (int i = 0; i < _parameterValueView.Count; i++)
            {
                //_parameterValueView[i].DrawValues();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();
            Init();
            MakeElement();
            BindElement();
            RegisterCallback();
            Control();

            return _root;
        }

        private void OnResetButton()
        {
            string title = Application.systemLanguage == SystemLanguage.Korean ? "경고" : "Warning";
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? "정말로 리셋하시겠습니까?"
                : "Do you really want to reset?";
            string yes = Application.systemLanguage == SystemLanguage.Korean ? "넵" : "Yes";
            string no = Application.systemLanguage == SystemLanguage.Korean ? "아니요.." : "No";

            bool result = EditorUtility.DisplayDialog(title, msg, yes, no);

            if (result)
            {
                Undo.RecordObject(target, "Reset List");
                _localKeyList.EventRefList.Clear();
                _parameterValueView.Clear();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void UnbindItem(VisualElement arg1, int arg2)
        {
            _numberLabel.text = clipList.arraySize.ToString();
        }

        private VisualElement MakeItem()
        {
            TemplateContainer item = KeyListItemUXML.Instantiate();

            var keyField = item.Q<TextField>("keyField");
            var itemFoldout = item.Q<Foldout>("ItemFoldout");
            var showInfoToggle = item.Q<Toggle>("ShowInfo-Toggle");
            var baseFieldLayout = item.Q<VisualElement>("BaseFieldLayout");
            var labelArea = item.Q<VisualElement>("LabelArea");
            var inputArea = item.Q<VisualElement>("InputArea");
            NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);


            showInfoToggle.RegisterValueChangedCallback(evt => itemFoldout.value = evt.newValue);

            keyField.RegisterValueChangedCallback(evt => {
                if (item.userData != null)
                {
                    int index = (int)item.userData;

                    var keyProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
                    keyProperty.stringValue = evt.newValue;
                    item.Q<Label>("KeyName").text = $"{keyProperty.stringValue} : ";
                }
            });

            item.Q<DropdownField>("Add-Button").RegisterCallback<ClickEvent>(evt => {
                
            });

            PropertyField eventRefField = item.Q<PropertyField>("EventRef-Field");
            eventRefField.RegisterValueChangeCallback(clipList,"", property => {
                
            });

            return item;
        }
        
        /// <summary>
        /// 이벤트 레퍼런스가 변했는지 체크합니다.
        /// </summary>
        /// <param name="callback">Callback requested when value changes</param>
        private void RegisterEventRefValueChange(EventReference eventRef, Action<string> callback)
        {
            _root.schedule.Execute(() =>
            {
                var tmp = string.Empty;
                
                    tmp = eventRef.Path;

                if (_oldTargetPath != tmp)
                {
                    callback.Invoke(tmp);
                    _oldTargetPath = tmp;
                }
            }).Every(5);
        }

        private void BindItem(VisualElement element, int index)
        {
            // 업데이트를 넣어야 Add 버튼을 눌렀을 때 ClipList에 추가된 것이 바로 적용이 된 상태로 시스템을 구성할 수 있음
            //serializedObject.Update();
            var keyProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            element.Q<Label>("KeyName").text = $"{keyProperty.stringValue} : ";

            var keyField = element.Q<TextField>("keyField");
            keyField.BindProperty(keyProperty);

            var eventRefProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
            var eventRefField = element.Q<PropertyField>("EventRef-Field");
            eventRefField.BindProperty(eventRefProperty);
            eventRefField.label = "Event";

            var showInfoProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("ShowInfo");
            element.Q<Toggle>("ShowInfo-Toggle").BindProperty(showInfoProperty);
            element.Q<Foldout>("ItemFoldout").value = showInfoProperty.boolValue;

            element.Q<DropdownField>("Add-Button").value = "Add";

            element.userData = index;
        }

        private void AddItem(IEnumerable<int> obj)
        {
            serializedObject.Update();
            _numberLabel.text = clipList.arraySize.ToString();
             int count = 0;
            
             for (int i = 0; i < clipList.arraySize; i++)
             {
                 SerializedProperty key = clipList.GetArrayElementAtIndex(i).FindPropertyRelative(kKey);
            
                 string checkFirstTest;
                 if (key.stringValue.Length < kDefaultKey.Length)
                     checkFirstTest = key.stringValue;
                 else
                     checkFirstTest = key.stringValue.Substring(0, kDefaultKey.Length);
            
                 if (checkFirstTest == kDefaultKey)
                     count += 1;
             }
            
             SerializedProperty element = clipList.GetArrayElementAtIndex(clipList.arraySize - 1);
            
             EventReferenceByKey item = new();
             item.Key = count > 0 ? $"New Key ({count})" : "New Key";
             element.FindPropertyRelative(kKey).stringValue = item.Key;
             element.FindPropertyRelative(kValue).FindPropertyRelative(kPath).stringValue = string.Empty;
             element.FindPropertyRelative(kParams).ClearArray();
            
             SerializedProperty guid = element.FindPropertyRelative(kValue).FindPropertyRelative("Guid");
             guid.FindPropertyRelative("Data1").intValue = 0;
             guid.FindPropertyRelative("Data2").intValue = 0;
             guid.FindPropertyRelative("Data3").intValue = 0;
             guid.FindPropertyRelative("Data4").intValue = 0;
            
             element.FindPropertyRelative(kGUID).stringValue = item.GUID;
             serializedObject.ApplyModifiedProperties();

            //추가적인 파라미터 뷰 생성
            _parameterValueView.Add(new ParameterValueView(serializedObject));

        }

        private void OnRemoveItem(IEnumerable<int> obj)
        {
            // 파라미터 뷰 제거
            int index = obj.First();
            Debug.Log(index);
        }

        /// <summary>
        /// 리스트에 있는 요소들에는 각각 파라미터 뷰가 존재한다.
        /// </summary>
        public class ParameterValueView
        {
            private SerializedObject _serializedObject;

            private VisualElement _parameterArea;

            // EditorParamRef에서 현재 선택 항목의 모든 속성에 대한 초기 매개변수 값 속성으로 매핑합니다.
            private readonly List<PropertyRecord> _propertyRecords = new();

            /// <summary>
            /// 파라미터 영역에 아직 추가되지 않은 녀석들을 관리하는 리코드 입니다.
            /// </summary>
            private class PropertyRecord
            {
                public string ParameterName => ParamRef.Name;

                public EditorParamRef ParamRef;
                public List<SerializedProperty> ValueProperties;
            }

            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="serializedObject"></param>
            public ParameterValueView(SerializedObject serializedObject)
            {
                _serializedObject = serializedObject;
            }

            // void HandleEventRef(string path, Action successAction = null, Action failAction = null)
            // {
            //     if (!string.IsNullOrWhiteSpace(path))
            //     {
            //         EditorEventRef existEvent = EventManager.EventFromPath(path);
            //
            //         if (existEvent != null)
            //         {
            //             HandleEvent(existEvent);
            //             successAction?.Invoke();
            //         }
            //         else
            //         {
            //             HandleMessage(helpBox,
            //                 "연결된 이벤트 주소가 유효하지 않습니다.",
            //                 "The connected event address is invalid.");
            //
            //             failAction?.Invoke();
            //             _parameterValueView.Dispose();
            //         }
            //     }
            //     else
            //     {
            //         HandleMessage(helpBox,
            //             "Event가 비어있습니다.",
            //             "Event is empty.");
            //
            //         failAction?.Invoke();
            //         _parameterValueView.Dispose();
            //     }
            // }

            // void HandleEvent(EditorEventRef existEvent)
            // {
            //     _parameterValueView.RefreshPropertyRecords(existEvent);
            //     _parameterValueView.DrawValues();
            //     _parameterValueView.CalculateEnableAddButton();
            //
            //     addButton.SetActive(true);
            //     titleToggleLayout.SetActive(true);
            //     initializeField.SetActive(true);
            //     sendOnStart.SetActive(true);
            //
            //     var toggleOnOff = titleToggleLayout.value;
            //     parameterArea.SetActive(toggleOnOff);
            // }

            public void SetParameterArea(VisualElement parameterArea)
            {
                _parameterArea = parameterArea;
            }

            /// <summary>
            /// 속성 기록을 새로 고칩니다.
            /// </summary>
            /// <param name="eventRef">이벤트 참조를 요청합니다.</param>
            // public void RefreshPropertyRecords(EditorEventRef eventRef)
            // {
            //     _propertyRecords.Clear();
            //
            //     // 여기에서는 파라미터의 배열을 전달한다.
            //     SerializedProperty paramsProperty = _serializedObject.FindProperty(PropNames.Params);
            //
            //     foreach (SerializedProperty parameterProperty in paramsProperty)
            //     {
            //         string name = parameterProperty.FindPropertyRelative(PropNames.Name).stringValue;
            //         SerializedProperty valueProperty = parameterProperty.FindPropertyRelative(PropNames.Value);
            //
            //         PropertyRecord record = _propertyRecords.Find(r => r.Name == name);
            //
            //         if (record != null)
            //             record.ValueProperties.Add(valueProperty);
            //         else
            //         {
            //             EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);
            //
            //             if (paramRef != null)
            //             {
            //                 _propertyRecords.Add(
            //                     new PropertyRecord() {
            //                         ParamRef = paramRef,
            //                         ValueProperties = new List<SerializedProperty>() { valueProperty }
            //                     });
            //             }
            //         }
            //     }
            //
            //     // Sort only when there are multiple selections. When there is only one object selected
            //     // The user can revert to the prefab and its behavior will depend on the arrangement order, so it is helpful to show the actual order.
            //     _propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.Name, b.Name));
            //     _missingParameters.Clear();
            //
            //     if (eventRef != null)
            //         foreach (var parameter in eventRef.LocalParameters)
            //         {
            //             PropertyRecord record = _propertyRecords.Find(p => p.Name == parameter.Name);
            //
            //             if (record == null)
            //                 _missingParameters.Add(parameter);
            //         }
            //     else
            //         Dispose();
            // }
            //
            // private void PreRefresh()
            // {
            //     string path = string.Empty;
            //     EditorEventRef eventRef = null;
            //
            //     path = _serializedObject.FindProperty(PropNames.Clip)
            //         .FindPropertyRelative(PropNames.Path)
            //         .stringValue;
            // }

            // public void DrawValues(bool preRefresh = false)
            // {
            //     if (preRefresh)
            //         PreRefresh();
            //
            //     // 파라미터 영역에 있는 요소들을 모두 제거합니다.
            //     _parameterArea.Clear();
            //
            //     // 파라미터
            //     foreach (PropertyRecord record in _propertyRecords)
            //         _parameterArea.Add(AdaptiveParameterField(record));
            // }

            /// <summary>
            /// 매개변수 필드는 레코드 유형에 따라 자동으로 생성됩니다.
            /// </summary>
            /// <param name="record">Request property record</param>
            /// <returns>Field UI returned</returns>
            private SimpleBaseField AdaptiveParameterField(PropertyRecord record)
            {
                float value = 0;

                if (record.ValueProperties.Count == 1)
                    value = record.ValueProperties[0].floatValue;
                else
                {
                    bool first = true;

                    foreach (SerializedProperty property in record.ValueProperties)
                        if (first)
                        {
                            value = property.floatValue;
                            first = false;
                        }
                }

                var baseField = new SimpleBaseField {
                    Label = record.ParameterName,
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

                        foreach (SerializedProperty property in record.ValueProperties)
                            floatSlider.value = property.floatValue;

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
                            index = (int)value
                        };

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
                    // DeleteParameter(record.ParameterName);
                    // DrawValues(true);
                };

                return baseField;
            }
        }
    }
}
