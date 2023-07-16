using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(KeyList))]
    public class KeyListEditor : Editor
    {
        private readonly List<ParameterValueView> _parameterValueView = new();

        private ReorderableList _reorderableList;
        private ReorderableList _searchList;
        private KeyList _keyList;
        private float _lineHeight;
        private float _lineHeightSpacing;

        private string _searchText = string.Empty;

        private const string ClipsID = "Clips";
        private const string ListID = "list";
        private const string CachedSearchClipsID = "cachedSearchClips";
        private const string KeyID = "Key";
        private const string ShowInfoID = "ShowInfo";
        private const string ValueID = "Value";
        private const string PathID = "Path";
        private const string ParamsID = "Params";
        private const string NameID = "Name";

        private SerializedProperty clip;
        private SerializedProperty clipList;
        private SerializedProperty cachedClipList;

        private void OnEnable()
        {
            clip = serializedObject.FindProperty(ClipsID);
            clipList = clip.FindPropertyRelative(ListID);
            cachedClipList = serializedObject.FindProperty(CachedSearchClipsID);

            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("e9081b0172b4f4735ac3dc549b17a6b8");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("2be0f2ec703f2444b8795748b7ffcee3");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/KeyList.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

            if (target == null)
                return;

            _lineHeight = EditorGUIUtility.singleLineHeight;
            _lineHeightSpacing = _lineHeight + 10;

            _keyList = target as KeyList;

            // 리스트 초기화
            for (var i = 0; i < clipList.arraySize; i++)
                _parameterValueView.Add(new ParameterValueView());

            #region 실제

            _reorderableList = new ReorderableList(serializedObject, clipList, true,
                true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    if (EditorApplication.isPlaying)
                        EditorGUI.LabelField(rect, "Event Clip List (Editable only Edit mode)");
                    else
                        EditorGUI.LabelField(rect, "Event Clip List");

                    var countRect = rect;
                    countRect.width = 50;
                    countRect.x = rect.width - 20;

                    // 리스트 크기 필드 표시
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.IntField(countRect, clipList.arraySize);
                    EditorGUI.EndDisabledGroup();

                    countRect.width = 50;
                    countRect.x = rect.width - 70;

                    bool isLock = clipList.arraySize == 0;
                    EditorGUI.BeginDisabledGroup(isLock || EditorApplication.isPlaying);
                    if (GUI.Button(new Rect(countRect.x, countRect.y, countRect.width, _lineHeight), "Reset"))
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
                            _keyList.Clips.Reset();
                            _parameterValueView.Clear();
                        }

                        serializedObject.Update();
                    }

                    EditorGUI.EndDisabledGroup();
                    serializedObject.ApplyModifiedProperties();
                }
            };

            _reorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                var boldLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };

                var keyProperty = element.FindPropertyRelative(KeyID);
                string label = keyProperty.stringValue;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, _lineHeight),
                    label, boldLabelStyle);

                var showInfo = element.FindPropertyRelative(ShowInfoID);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 80), rect.y, rect.width, _lineHeight),
                    "Show Info");
                var showInfoFoldout = EditorGUI.Toggle(new Rect(rect.x + (rect.width - 15), rect.y, 20, 20)
                    , showInfo.boolValue);

                showInfo.boolValue = showInfoFoldout;

                if (showInfo.boolValue)
                {
                    // 이전 값 기록을 위한 변수
                    EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                    label = EditorGUI.TextField(new Rect(rect.x, rect.y + _lineHeightSpacing, rect.width, _lineHeight),
                        label);
                    keyProperty.stringValue = label;

                    var valueValue = element.FindPropertyRelative(ValueID);
                    var eventPath = valueValue.FindPropertyRelative(PathID);

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + _lineHeightSpacing * 2, rect.width, _lineHeight),
                        valueValue, new GUIContent("Event"));

                    EditorEventRef editorEvent = EventManager.EventFromPath(eventPath.stringValue);
                    EditorGUI.EndChangeCheck();

                    string oldPath = _parameterValueView[index].GetPrePath();
                    if (!string.IsNullOrWhiteSpace(oldPath))
                        if (eventPath.stringValue != oldPath)
                        {
                            var parameterField = element.FindPropertyRelative(ParamsID);
                            parameterField.ClearArray();
                        }

                    serializedObject.ApplyModifiedProperties();

                    if (editorEvent)
                    {
                        _parameterValueView[index]
                            .OnGUI(rect, element, editorEvent, !element.hasMultipleDifferentValues);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                        EditorGUI.EndDisabledGroup();
                }
            };

            _reorderableList.elementHeightCallback = index =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var showInfo = element.FindPropertyRelative(ShowInfoID);

                if (!showInfo.boolValue)
                    return _lineHeightSpacing;

                SerializedProperty foldoutField = element.FindPropertyRelative(ValueID);

                float defaultHeight = _lineHeightSpacing * 3.4f; // Top margin;

                string eventPath = foldoutField.FindPropertyRelative(PathID).stringValue;
                bool findEvent = EventManager.EventFromPath(eventPath) != null;

                if (findEvent)
                {
                    defaultHeight += _lineHeightSpacing;

                    if (foldoutField.isExpanded)
                        defaultHeight += _lineHeightSpacing * 3;
                }

                var parameterField = element.FindPropertyRelative(ParamsID);

                if (parameterField.isExpanded)
                {
                    int parameterCount = parameterField.arraySize;

                    for (int i = 0; i < parameterCount; i++)
                        defaultHeight += _lineHeightSpacing;
                }
                
                return defaultHeight;
            };

            _reorderableList.onAddDropdownCallback = (rect, reorderList) =>
            {
                Undo.RecordObject(_keyList, "Create Clip");

                _parameterValueView.Add(new ParameterValueView());
                _keyList.Clips.Add();
            };

            _reorderableList.onRemoveCallback = reorderList =>
            {
                _reorderableList.DoLayoutList();
                Undo.RecordObject(_keyList, "Remove Clip");

                _parameterValueView.RemoveAt(reorderList.index);
                _keyList.Clips.RemoveAt(reorderList.index);
            };

            _reorderableList.onCanAddCallback += reorderableList =>
            {
                if (!string.IsNullOrWhiteSpace(_searchText))
                    return false;

                if (EditorApplication.isPlaying)
                    return false;

                return true;
            };

            _reorderableList.onCanRemoveCallback += reorderableList =>
            {
                if (!string.IsNullOrWhiteSpace(_searchText))
                    return false;

                if (EditorApplication.isPlaying)
                    return false;

                return true;
            };

            #endregion

            #region 검색

            _searchList = new ReorderableList(serializedObject, cachedClipList, false,
                true, false, false)
            {
                drawHeaderCallback = rect =>
                {
                    if (EditorApplication.isPlaying)
                        EditorGUI.LabelField(rect, "Event Clip List (Editable only Edit mode)");
                    else
                        EditorGUI.LabelField(rect, "Event Clip List");

                    var countRect = rect;
                    countRect.width = 50;
                    countRect.x = rect.width - 20;

                    // 리스트 크기 필드 표시
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.IntField(countRect, cachedClipList.arraySize);
                    EditorGUI.EndDisabledGroup();

                    countRect.width = 50;
                    countRect.x = rect.width - 70;

                    EditorGUI.EndDisabledGroup();
                    serializedObject.ApplyModifiedProperties();
                }
            };

            _searchList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = _searchList.serializedProperty.GetArrayElementAtIndex(index);
                var boldLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };
                var keyProperty = element.FindPropertyRelative(KeyID);
                string label = keyProperty.stringValue;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, _lineHeight),
                    label, boldLabelStyle);

                rect.x += 15;
                rect.width -= 15;
                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                label = EditorGUI.TextField(new Rect(rect.x, rect.y + _lineHeightSpacing, rect.width, _lineHeight),
                    label);
                keyProperty.stringValue = label;

                var valueValue = element.FindPropertyRelative(ValueID);
                var eventPath = valueValue.FindPropertyRelative(PathID);

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + _lineHeightSpacing * 2, rect.width, _lineHeight),
                    valueValue, new GUIContent("Event"));

                EditorEventRef editorEvent = EventManager.EventFromPath(eventPath.stringValue);
                EditorGUI.EndChangeCheck();

                string oldPath = _parameterValueView[index].GetPrePath();
                if (!string.IsNullOrWhiteSpace(oldPath))
                    if (eventPath.stringValue != oldPath)
                    {
                        var parameterField = element.FindPropertyRelative(ParamsID);
                        parameterField.ClearArray();
                    }

                serializedObject.ApplyModifiedProperties();

                if (editorEvent)
                {
                    try
                    {
                        _parameterValueView[index]
                            .OnGUI(rect, element, editorEvent, !element.hasMultipleDifferentValues);
                    }
                    catch (Exception)
                    {
                    }

                    EditorGUI.EndDisabledGroup();
                }
                else
                    EditorGUI.EndDisabledGroup();
            };

            _searchList.elementHeightCallback = index =>
            {
                SerializedProperty element = _searchList.serializedProperty.GetArrayElementAtIndex(index);

                SerializedProperty foldoutField = element.FindPropertyRelative(ValueID);

                float defaultHeight = _lineHeightSpacing * 3.4f; // Top margin;

                string eventPath = foldoutField.FindPropertyRelative(PathID).stringValue;
                bool findEvent = EventManager.EventFromPath(eventPath) != null;

                if (findEvent)
                {
                    defaultHeight += _lineHeightSpacing;

                    if (foldoutField.isExpanded)
                        defaultHeight += _lineHeightSpacing * 3;
                }

                var parameterField = element.FindPropertyRelative(ParamsID);

                if (parameterField.isExpanded)
                {
                    int parameterCount = parameterField.arraySize;

                    for (int i = 0; i < parameterCount; i++)
                        defaultHeight += _lineHeightSpacing;
                }

                return defaultHeight;
            };

            #endregion

            Undo.undoRedoEvent += (in UndoRedoInfo undo) =>
            {
                switch (undo.undoName)
                {
                    case "Create Clip":
                        _parameterValueView.RemoveAt(_reorderableList.count - 1);
                        break;
                    case "Remove Clip":
                        _parameterValueView.Add(new ParameterValueView());
                        break;
                    case "Reset List":
                        for (int i = 0; i < _keyList.Clips.Count; i++)
                            _parameterValueView.Add(new ParameterValueView());
                        break;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(6);
            Rect rect = EditorGUILayout.GetControlRect();

            // 클립들을 모두 순례돌아서 Cached의 Key와 동일한지 체크
            foreach (SerializedProperty clip in clipList)
                for (int i = 0; i < cachedClipList.arraySize; i++)
                {
                    string clipKey = clip.FindPropertyRelative(KeyID).stringValue;
                    string cachedSearch = cachedClipList.GetArrayElementAtIndex(i).FindPropertyRelative(KeyID)
                        .stringValue;

                    if (clipKey == cachedSearch)
                        clip.boxedValue = cachedClipList.GetArrayElementAtIndex(i).boxedValue;
                }

            clipList.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();

            Rect titleRect = rect;
            titleRect.width = 40;

            EditorGUI.LabelField(titleRect, "Find : ");
            Rect searchTextFieldRect = rect;
            searchTextFieldRect.x += 40;
            searchTextFieldRect.width -= 40;

            EditorGUI.BeginChangeCheck();
            _searchText = EditorGUI.TextField(searchTextFieldRect, _searchText,
                GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (EditorGUI.EndChangeCheck())
            {
                // _searchText와 글자가 동일한 녀석들을 리스트에 수록
                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    cachedClipList.ArrayClear();

                    foreach (SerializedProperty clip in clipList)
                    {
                        string key = clip.FindPropertyRelative(KeyID).stringValue;
                        if (key.Contains(_searchText))
                        {
                            cachedClipList.arraySize += 1;
                            cachedClipList.GetArrayElementAtIndex(cachedClipList.arraySize - 1).boxedValue =
                                clip.boxedValue;
                        }
                    }
                }
                else
                    cachedClipList.ArrayClear();

                cachedClipList.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.Update();

            if (!string.IsNullOrWhiteSpace(_searchText))
                _searchList.DoLayoutList();
            else
                _reorderableList.DoLayoutList();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        private class ParameterValueView
        {
            // This holds one SerializedObject for each object in the current selection.
            private List<SerializedObject> serializedTargets = new();

            // Mappings from EditorParamRef to initial parameter value property for all properties
            // found in the current selection.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // Any parameters that are in the current event but are missing from some objects in
            // the current selection, so we can put them in the "Add" menu.
            private readonly List<EditorParamRef> _missingParameters = new();

            private readonly float lineHeight;
            private readonly float lineHeightSpacing;

            private int parameterIndex;
            private const int ParameterCount = 3;

            public bool IsOpenParameterArea;
            private string oldEventPath;

            // A mapping from EditorParamRef to the initial parameter value properties in the
            // current selection that have the same name.
            // We need this because some objects may be missing some properties, and properties with
            // the same name may be at different array indices in different objects.
            private class PropertyRecord
            {
                public string Name => paramRef.Name;

                public EditorParamRef paramRef;
                public List<SerializedProperty> valueProperties;
            }

            public ParameterValueView()
            {
                lineHeight = EditorGUIUtility.singleLineHeight;
                lineHeightSpacing = lineHeight + 10;
            }

            // Rebuilds the propertyRecords and missingParameters collections.
            private void RefreshPropertyRecords(SerializedProperty serializedTarget, EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = serializedTarget.FindPropertyRelative(ParamsID);

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative(NameID).stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative(ValueID);

                    PropertyRecord record =
                        _propertyRecords.Find(r => r.Name == name);

                    if (record != null)
                    {
                        record.valueProperties.Add(valueProperty);
                    }
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(p => p.Name == name);

                        if (paramRef != null)
                        {
                            _propertyRecords.Add(
                                new PropertyRecord()
                                {
                                    paramRef = paramRef,
                                    valueProperties = new List<SerializedProperty>() { valueProperty },
                                });
                        }
                    }
                }

                // Only sort if there is a multi-selection. If there is only one object selected,
                // the user can revert to prefab, and the behaviour depends on the array order,
                // so it's helpful to show the true order.

                _propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.Name, b.Name));

                _missingParameters.Clear();
                _missingParameters.AddRange(eventRef.LocalParameters.Where(
                    p =>
                    {
                        PropertyRecord record =
                            _propertyRecords.Find(r => r.Name == p.Name);
                        return record == null || record.valueProperties.Count < serializedTargets.Count;
                    }));
            }

            public string GetPrePath()
            {
                return oldEventPath;
            }

            public void OnGUI(Rect rect, SerializedProperty target, EditorEventRef eventRef, bool matchingEvents)
            {
                parameterIndex = 0;
                oldEventPath = eventRef.Path;
                target.serializedObject.Update();

                if (Event.current.type == EventType.Layout)
                    RefreshPropertyRecords(target, eventRef);

                SerializedProperty paramsProperty = target.FindPropertyRelative(ParamsID);

                DrawHeader(rect, target, matchingEvents);

                if (paramsProperty.isExpanded)
                {
                    if (matchingEvents)
                    {
                        DrawValues(rect, target);
                    }
                    else
                    {
                        GUILayout.Box("Cannot change parameters when different events are selected",
                            GUILayout.ExpandWidth(true));
                    }
                }

                foreach (SerializedObject serializedTarget in serializedTargets)
                    serializedTarget.ApplyModifiedProperties();
            }

            private void DrawHeader(Rect rect, SerializedProperty target, bool enableAddButton)
            {
                SerializedProperty eventInfo = target.FindPropertyRelative(ValueID);
                SerializedProperty paramsProperty = target.FindPropertyRelative(ParamsID);

                int nextHeight = 3;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                Rect titleRect = new(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width, lineHeight)
                {
                    width = EditorGUIUtility.labelWidth
                };

                // Let the user revert the whole Params array to prefab by context-clicking the title.
                EditorGUI.BeginProperty(titleRect, GUIContent.none, paramsProperty);

                paramsProperty.isExpanded = EditorGUI.Foldout(titleRect, paramsProperty.isExpanded,
                    "Initial Parameter Values");

                EditorGUI.EndProperty();

                EditorGUI.BeginDisabledGroup(!enableAddButton);

                Rect position = new(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width, lineHeight);
                position.xMin = titleRect.xMax;
                DrawAddButton(position, paramsProperty);

                EditorGUI.EndDisabledGroup();
            }

            private void DrawAddButton(Rect position, SerializedProperty paramsProperty)
            {
                EditorGUI.BeginDisabledGroup(_missingParameters.Count == 0);

                if (EditorGUI.DropdownButton(position, new GUIContent("Add"), FocusType.Passive))
                {
                    GenericMenu menu = new();
                    menu.AddItem(new GUIContent("All"), false, () =>
                    {
                        foreach (EditorParamRef parameter in _missingParameters)
                            AddParameter(parameter, paramsProperty);
                    });

                    menu.AddSeparator(string.Empty);

                    foreach (EditorParamRef parameter in _missingParameters)
                    {
                        menu.AddItem(new GUIContent(parameter.Name), false,
                            (userData) =>
                            {
                                AddParameter(userData as EditorParamRef, paramsProperty);
                            },
                            parameter);
                    }

                    menu.DropDown(position);
                }

                EditorGUI.EndDisabledGroup();
            }

            private void DrawValues(Rect rect, SerializedProperty target)
            {
                // 배열 요소를 참조하는 SerializedProperties를 사용하는 동안 예외가 발생할 수 있으므로 배열을 엉망으로 만들지 않도록 삭제를 연기하는 데 이것을 사용합니다.
                string parameterToDelete = null;

                foreach (PropertyRecord record in _propertyRecords)
                    if (record.valueProperties.Count == 1)
                    {
                        DrawValue(rect, target, record, out var delete);

                        if (delete)
                            parameterToDelete = record.Name;
                    }

                if (parameterToDelete != null)
                    DeleteParameter(parameterToDelete, target);
            }

            private void DrawValue(Rect rect, SerializedProperty target, PropertyRecord record, out bool delete)
            {
                SerializedProperty eventInfo = target.FindPropertyRelative(ValueID);

                int nextHeight = 4 + parameterIndex;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                delete = false;

                GUIContent removeLabel = new("Remove");
                Rect position = new(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width,
                    lineHeight);

                parameterIndex += 1;

                Rect nameLabelRect = position;
                nameLabelRect.width = EditorGUIUtility.labelWidth;

                Rect removeButtonRect = position;
                removeButtonRect.width = EditorStyles.miniButton.CalcSize(removeLabel).x;
                removeButtonRect.x = position.xMax - removeButtonRect.width;

                Rect sliderRect = position;
                sliderRect.xMin = nameLabelRect.xMax;
                sliderRect.xMax = removeButtonRect.xMin - EditorStyles.miniButton.margin.left;

                GUIContent nameLabel = new(record.Name);

                float value = 0;
                bool mixedValues = false;

                // We use EditorGUI.BeginProperty when there is a single object selected, so
                // the user can revert the value to prefab by context-clicking the name.
                // We handle multi-selections ourselves, so that we can deal with
                // mismatched arrays nicely.
                if (record.valueProperties.Count == 1)
                {
                    value = record.valueProperties[0].floatValue;
                    EditorGUI.BeginProperty(position, nameLabel, record.valueProperties[0]);
                }
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
                        else if (property.floatValue != value)
                        {
                            mixedValues = true;
                            break;
                        }
                    }
                }

                EditorGUI.LabelField(nameLabelRect, nameLabel);

                if (record.paramRef.Type == ParameterType.Labeled)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    int newValue = EditorGUI.Popup(sliderRect, (int)value, record.paramRef.Labels);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.valueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }
                else if (record.paramRef.Type == ParameterType.Discrete)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    int newValue = EditorGUI.IntSlider(sliderRect, (int)value, (int)record.paramRef.Min,
                        (int)record.paramRef.Max);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.valueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    float newValue = EditorGUI.Slider(sliderRect, value, record.paramRef.Min, record.paramRef.Max);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.valueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }

                delete = GUI.Button(removeButtonRect, removeLabel, EditorStyles.miniButton);

                if (record.valueProperties.Count == 1)
                {
                    EditorGUI.EndProperty();
                }
                else
                {
                    // Context menu to set all values from one object in the multi-selection.
                    if (mixedValues && Event.current.type == EventType.ContextClick
                                    && nameLabelRect.Contains(Event.current.mousePosition))
                    {
                        GenericMenu menu = new();

                        foreach (SerializedProperty sourceProperty in record.valueProperties)
                        {
                            UnityEngine.Object targetObject = sourceProperty.serializedObject.targetObject;

                            menu.AddItem(new GUIContent(string.Format("Set to Value of '{0}'", targetObject.name)),
                                false,
                                (userData) => CopyValueToAll(userData as SerializedProperty, record.valueProperties),
                                sourceProperty);
                        }

                        menu.DropDown(position);
                    }
                }
            }

            // Copy the value from the source property to all target properties.
            private void CopyValueToAll(SerializedProperty sourceProperty, List<SerializedProperty> targetProperties)
            {
                foreach (SerializedProperty targetProperty in targetProperties)
                {
                    if (targetProperty != sourceProperty)
                    {
                        targetProperty.floatValue = sourceProperty.floatValue;
                        targetProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            // 매개변수가 없는 모든 선택된 객체에 주어진 매개변수에 대한 초기값을 추가합니다.
            private void AddParameter(EditorParamRef parameter, SerializedProperty paramProperty)
            {
                // 여기는 선택한 옵션이 파라미터 리스트에 존재하는지 확인하는 부분

                // 옵션을 추가했으니 식별 가능하도록 강제로 토글을 오픈합니다.
                paramProperty.isExpanded = true;

                for (int i = 0; i < paramProperty.arraySize; i++)
                {
                    string paramName = paramProperty.GetArrayElementAtIndex(i).FindPropertyRelative(NameID).stringValue;

                    if (paramName == parameter.Name)
                        return;
                }

                int index = paramProperty.arraySize;
                paramProperty.InsertArrayElementAtIndex(index);
                SerializedProperty arrayElement = paramProperty.GetArrayElementAtIndex(index);
                arrayElement.FindPropertyRelative(NameID).stringValue = parameter.Name;
                arrayElement.FindPropertyRelative(ValueID).floatValue = parameter.Default;
                paramProperty.serializedObject.ApplyModifiedProperties();
            }

            // Delete initial parameter values for the given name from all selected objects.
            private void DeleteParameter(string name, SerializedProperty target)
            {
                SerializedProperty paramsProperty = target.FindPropertyRelative(ParamsID);
                foreach (SerializedProperty child in paramsProperty)
                {
                    if (child.FindPropertyRelative(NameID).stringValue == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }
            }
        }
    }
}