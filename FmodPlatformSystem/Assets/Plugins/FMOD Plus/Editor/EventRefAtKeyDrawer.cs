using System.Collections.Generic;
using System.Linq;
using FMODPlus;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ListView = UnityEngine.UIElements.ListView;
namespace NKStudio
{
    [CustomPropertyDrawer(typeof(EventRefAtKey), true)]
    public class EventRefAtKeyDrawer : PropertyDrawer
    {
        private VisualTreeAsset _listTemplate;
        private VisualTreeAsset _listItemTemplate;

        private VisualElement _root;
        private Label _numberLabel;
        private ListView _reorderableList;

        private List<ParameterValueView> _parameterValueView;
        private List<string> _itemEventRefPathCached;

        private SerializedProperty _clipList;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            FindProperty(property);
            Init();
            MakeElement();
            BindElement();
            RegisterCallback();
            InitControl();

            return _root;
        }

        #region Default Draw
        private void FindProperty(SerializedProperty property)
        {
            _clipList = property.FindPropertyRelative("EventRefList");
        }

        private void Init()
        {
            _listTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/FMOD Plus/Editor/UXML/KeyListTemplate.uxml");
            _listItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/FMOD Plus/Editor/UXML/KeyListItem.uxml");

            _parameterValueView = new List<ParameterValueView>();
            _itemEventRefPathCached = new List<string>();

            for (int i = 0; i < _clipList.arraySize; i++)
            {
                _parameterValueView.Add(new ParameterValueView(_clipList));
                _itemEventRefPathCached.Add(_clipList.GetArrayElementAtIndex(i).FindPropertyRelative("Value")
                    .FindPropertyRelative("Path").stringValue);
            }
        }

        private void MakeElement()
        {
            _root = _listTemplate.Instantiate();
            _reorderableList = _root.Q<ListView>("KeyList");
            var unityContent = _root.Q<VisualElement>("unity-content");
            unityContent.schedule.Execute(() => {
                unityContent.style.display = DisplayStyle.Flex;
            });
            _reorderableList.BindProperty(_clipList);
            _numberLabel = _root.Q<Label>("Number");

            var numberBox = _root.Q<VisualElement>("NumberBox");

            if (FMODPlusEditorUtility.IsDarkTheme)
                numberBox.AddToClassList("NumberBox__Dark");
            else
                numberBox.AddToClassList("NumberBox__Light");

            var listViewTitle = _root.Q<Label>("ListViewTitle-Label");
            listViewTitle.text = "Key List";
        }

        private void BindElement()
        {
            _numberLabel.text = _clipList.arraySize.ToString();
        }

        private void RegisterCallback()
        {
            _reorderableList.unbindItem = UnbindItem;
            _reorderableList.makeItem = MakeItem;
            _reorderableList.bindItem = BindItem;
            _reorderableList.itemsAdded += AddItem;
            _reorderableList.itemsRemoved += OnRemoveItem;

            _root.schedule.Execute(UpdateEvent).Every(100);
        }

        private void InitControl()
        {
            // 처리가 모두 끝난 다음에 처리되길 원해서 Schedule을 사용했습니다.
            _root.schedule.Execute(() => {
                for (int i = 0; i < _clipList.arraySize; i++)
                {
                    int index = i;
                    SerializedProperty pathProperty = _clipList.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("Value").FindPropertyRelative("Path");
                    _parameterValueView[index].HandleEvent(pathProperty.stringValue);
                }
            });
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void UpdateEvent()
        {
            // 변화된 이벤트 레퍼런스에 대하여 해당 요소에 업데이트를 처리합니다.
            for (int i = 0; i < _clipList.arraySize; i++)
            {
                int index = i;
                SerializedProperty pathProperty = _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Value")
                    .FindPropertyRelative("Path");
                if (_itemEventRefPathCached[index] != pathProperty.stringValue)
                {
                    _parameterValueView[index].HandleEvent(pathProperty.stringValue);
                    _itemEventRefPathCached[index] = pathProperty.stringValue;    
                    _reorderableList.Rebuild();
                }
            }
        }
        #endregion

        #region ListView
        private VisualElement MakeItem()
        {
            TemplateContainer item = _listItemTemplate.Instantiate();

            var keyField = item.Q<TextField>("keyField");
            var itemFoldout = item.Q<Foldout>("ItemFoldout");
            var showInfoToggle = item.Q<Toggle>("ShowInfo-Toggle");
            var baseFieldLayout = item.Q<VisualElement>("BaseFieldLayout");
            var labelArea = item.Q<VisualElement>("LabelArea");
            var inputArea = item.Q<VisualElement>("InputArea");
            var keyName = item.Q<Label>("KeyName");

            FMODPlusEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

            showInfoToggle.RegisterValueChangedCallback(evt => itemFoldout.value = evt.newValue);

            keyField.RegisterValueChangedCallback(evt => {
                if (item.userData != null)
                {
                    int index = (int)item.userData;
                    SerializedProperty keyProperty =
                        _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
                    SerializedProperty pathProperty = _clipList.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("Value").FindPropertyRelative("Path");

                    // 만약 키가 비어있으면
                    if (string.IsNullOrWhiteSpace(evt.newValue))
                    {
                        #region 동일한 'New Key'키워드가 있으면 개수를 파악하여 New Key를 붙여줍니다.
                        int i = 0;
                        for (int j = 0; j < _clipList.arraySize; j++)
                        {
                            SerializedProperty key = _clipList.GetArrayElementAtIndex(j).FindPropertyRelative("Key");
                            if (key.stringValue.Contains(FMODPlusUtility.DefaultKey))
                                i += 1;
                        }

                        if (i > 0)
                            keyProperty.stringValue = $"New Key ({i})";
                        else
                            keyProperty.stringValue = "New Key";
                        #endregion

                        keyName.text = $"{keyProperty.stringValue} : {pathProperty.stringValue}";
                        Debug.LogWarning("키가 비어있습니다.");
                        _clipList.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    keyProperty.stringValue = evt.newValue;
                    keyName.text = $"{keyProperty.stringValue} : {pathProperty.stringValue}";
                    _clipList.serializedObject.ApplyModifiedProperties();
                }
            });

            var addButton = item.Q<DropdownField>("Add-Button");
            addButton.RegisterCallback<ClickEvent>(evt => {
                if (item.userData != null)
                {
                    int index = (int)item.userData;
                    _parameterValueView[index].DrawAddButton(addButton.worldBound);
                }
            });

            return item;
        }

        private void BindItem(VisualElement element, int index)
        {
            // 업데이트를 넣어야 Add 버튼을 눌렀을 때 ClipList에 추가된 것이 바로 적용이 된 상태로 시스템을 구성할 수 있음
            var keyProperty = _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            var pathProperty = _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Value")
                .FindPropertyRelative("Path");
            element.Q<Label>("KeyName").text = $"{keyProperty.stringValue} : {pathProperty.stringValue}";

            var keyField = element.Q<TextField>("keyField");
            keyField.BindProperty(keyProperty);

            var eventRefProperty = _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
            var eventRefField = element.Q<PropertyField>("EventRef-Field");
            eventRefField.BindProperty(eventRefProperty);
            eventRefField.label = "Event";

            var showInfoProperty = _clipList.GetArrayElementAtIndex(index).FindPropertyRelative("ShowInfo");
            element.Q<Toggle>("ShowInfo-Toggle").BindProperty(showInfoProperty);
            element.Q<Foldout>("ItemFoldout").value = showInfoProperty.boolValue;

            var baseFieldLayout = element.Q<VisualElement>("BaseFieldLayout");
            var parameterArea = element.Q<VisualElement>("ParameterArea");
            var addButton = element.Q<DropdownField>("Add-Button");
            _parameterValueView[index].Initialize(baseFieldLayout, index, addButton, parameterArea);
            addButton.value = "Add";
            element.userData = index;
        }

        private void AddItem(IEnumerable<int> obj)
        {
            _clipList.serializedObject.Update();
            _numberLabel.text = _clipList.arraySize.ToString();
            int count = 0;

            for (int i = 0; i < _clipList.arraySize; i++)
            {
                SerializedProperty key = _clipList.GetArrayElementAtIndex(i).FindPropertyRelative("Key");

                string checkFirstTest;
                if (key.stringValue.Length < FMODPlusEditorUtility.DefaultKey.Length)
                    checkFirstTest = key.stringValue;
                else
                    checkFirstTest = key.stringValue.Substring(0, FMODPlusEditorUtility.DefaultKey.Length);

                if (checkFirstTest == FMODPlusEditorUtility.DefaultKey)
                    count += 1;
            }

            SerializedProperty element = _clipList.GetArrayElementAtIndex(_clipList.arraySize - 1);

            EventReferenceByKey item = new();
            item.Key = count > 0 ? $"New Key ({count})" : "New Key";
            element.FindPropertyRelative("Key").stringValue = item.Key;
            element.FindPropertyRelative("Value").FindPropertyRelative("Path").stringValue = string.Empty;
            element.FindPropertyRelative("Params").ClearArray();

            SerializedProperty guid = element.FindPropertyRelative("Value").FindPropertyRelative("Guid");
            guid.FindPropertyRelative("Data1").intValue = 0;
            guid.FindPropertyRelative("Data2").intValue = 0;
            guid.FindPropertyRelative("Data3").intValue = 0;
            guid.FindPropertyRelative("Data4").intValue = 0;

            element.FindPropertyRelative("GUID").stringValue = item.GUID;
            _clipList.serializedObject.ApplyModifiedProperties();

            // ---- 데이터 처리 ----
            // 캐시용 EventRefPath를 추가합니다.
            _itemEventRefPathCached.Add(string.Empty);

            //추가적인 파라미터 뷰 생성
            _parameterValueView.Add(new ParameterValueView(_clipList));
        }

        private void OnRemoveItem(IEnumerable<int> obj)
        {
            // 파라미터 뷰 제거
            int index = obj.First();

            // ---- 데이터 처리 ----
            _itemEventRefPathCached.RemoveAt(index);
            _parameterValueView.RemoveAt(index);
        }

        private void UnbindItem(VisualElement arg1, int arg2)
        {
            _numberLabel.text = _clipList.arraySize.ToString();
        }
        
        #endregion

        /// <summary>
        /// 리스트에 있는 요소들에는 각각 파라미터 뷰가 존재한다.
        /// </summary>
        private class ParameterValueView
        {
            private readonly SerializedProperty _clipList;

            private VisualElement _parameterArea;
            private VisualElement _baseFieldLayout;
            private DropdownField _addButton;

            // EditorParamRef에서 현재 선택 항목의 모든 속성에 대한 초기 매개변수 값 속성으로 매핑합니다.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 매개변수는 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private int _index = -1;

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
            public ParameterValueView(SerializedProperty property)
            {
                _clipList = property;
            }

            /// <summary>
            /// 필요한 모든 구성 요소를 사용하여 필드를 설정합니다.
            /// </summary>
            /// <param name="baseFieldLayout"></param>
            /// <param name="index"></param>
            /// <param name="addButton"></param>
            /// <param name="parameterArea"></param>
            public void Initialize(VisualElement baseFieldLayout, int index, DropdownField addButton, VisualElement parameterArea)
            {
                _baseFieldLayout = baseFieldLayout;
                _index = index;
                _addButton = addButton;
                _parameterArea = parameterArea;
            }

            public void HandleEvent(string path)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    EditorEventRef findEventRef = EventManager.EventFromPath(path);

                    if (findEventRef == null)
                        return;

                    if (_baseFieldLayout != null)
                        _baseFieldLayout.SetActive(true);

                    if (_parameterArea != null)
                        _parameterArea.SetActive(true);

                    // 프로퍼티를 새로고침 합니다.
                    RefreshPropertyRecords(findEventRef);
                    DrawValues();
                    CalculateEnableAddButton();
                }
            }

            /// <summary>
            /// Add Button을 추가합니다.
            /// </summary>
            /// <param name="position">메뉴를 그릴 위치</param>
            public void DrawAddButton(Rect position)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("All"), false, () => {
                    foreach (EditorParamRef parameter in _missingParameters)
                        AddParameter(parameter);

                    DrawValues(true);
                    CalculateEnableAddButton();
                });

                menu.AddSeparator(string.Empty);

                foreach (EditorParamRef parameter in _missingParameters)
                {
                    menu.AddItem(new GUIContent(parameter.Name), false,
                        (userData) => {
                            AddParameter(userData as EditorParamRef);
                            DrawValues(true);
                            CalculateEnableAddButton();
                        },
                        parameter);
                }

                menu.DropDown(position);
            }

            /// <summary>
            /// 매개변수가 없는 선택된 모든 객체에 지정된 매개변수의 초기값을 추가합니다.
            /// </summary>
            /// <param name="parameter">If the parameter does not exist, add it.</param>
            private void AddParameter(EditorParamRef parameter)
            {
                var paramsProperty = _clipList.GetArrayElementAtIndex(_index)
                    .FindPropertyRelative("Params");

                // 해당 파라미터를 가지고 있는지 체크합니다.
                bool hasParameter = false;
                for (int i = 0; i < paramsProperty.arraySize; i++)
                {
                    if (paramsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue ==
                        parameter.Name)
                    {
                        hasParameter = true;
                        break;
                    }
                }

                // 해당 파라미터를 가지고 있으면 밑에 파라미터를 추가하는 내용을 실행하지 않습니다.
                if (hasParameter)
                    return;

                // 마지막 인덱스를 가라킵니다.
                int index = paramsProperty.arraySize;

                // 새로운 요소를 Insert 합니다.
                paramsProperty.InsertArrayElementAtIndex(index);

                // index번째 파라미터를 가리킵니다.
                SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                // index번째 요소에 Name과 Value의 값을 변경합니다.
                arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                _clipList.serializedObject.ApplyModifiedProperties();
            }

            /// <summary>
            /// 속성 기록을 새로 고칩니다.
            /// </summary>
            /// <param name="eventRef">이벤트 참조를 요청합니다.</param>
            private void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                if (_index == -1)
                {
                    Debug.LogError("Index is -1");
                    return;
                }

                // 여기에서는 파라미터의 배열을 전달한다.
                SerializedProperty paramsProperty = _clipList
                    .GetArrayElementAtIndex(_index).FindPropertyRelative("Params");

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record = _propertyRecords.Find(r => r.ParameterName == name);

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
                                    ValueProperties = new List<SerializedProperty> { valueProperty }
                                });
                        }
                    }
                }

                // 여러 게임 오브젝트를 선택한 경우에만 정렬합니다.
                // 선택한 객체가 하나만 있는 경우 사용자는 프리팹으로 돌아갈 수 있으며 배열 순서에 따라 동작이 달라지므로 실제 순서를 표시하는 데 도움이 됩니다.
                _propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.ParameterName, b.ParameterName));
                _missingParameters.Clear();

                if (eventRef != null)
                    foreach (var parameter in eventRef.LocalParameters)
                    {
                        PropertyRecord record = _propertyRecords.Find(p => p.ParameterName == parameter.Name);

                        if (record == null)
                            _missingParameters.Add(parameter);
                    }
            }

            /// <summary>
            /// MissingParameter에 따라 AddButton을 활성화합니다. 
            /// </summary>
            private void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            /// <summary>
            /// 파라미터 영역을 그려냅니다.
            /// </summary>
            private void DrawValues(bool preRefresh = false)
            {
                if (preRefresh)
                {
                    string path = _clipList.GetArrayElementAtIndex(_index)
                        .FindPropertyRelative("Value").FindPropertyRelative("Path").stringValue;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        EditorEventRef eventRef = EventManager.EventFromPath(path);
                        RefreshPropertyRecords(eventRef);
                    }
                }

                _parameterArea.Clear();

                // 파라미터
                foreach (PropertyRecord record in _propertyRecords)
                    _parameterArea.Add(AdaptiveParameterField(record));
            }

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
                            _clipList.serializedObject.ApplyModifiedProperties();
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
                            _clipList.serializedObject.ApplyModifiedProperties();
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
                            _clipList.serializedObject.ApplyModifiedProperties();
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
                    DeleteParameter(record.ParameterName);
                    DrawValues(true);
                    CalculateEnableAddButton();
                };

                return baseField;
            }

            /// <summary>
            /// Deletes the parameter.
            /// </summary>
            /// <param name="name">Removes a parameter by its name.</param>
            private void DeleteParameter(string name)
            {
                var paramsProperty = _clipList.GetArrayElementAtIndex(_index)
                    .FindPropertyRelative("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    string paramName = child.FindPropertyRelative("Name").stringValue;
                    if (paramName == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                _clipList.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
