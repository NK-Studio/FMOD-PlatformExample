using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using NKStudio;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(BGMKeyList))]
    public class BGMKeyListEditor : Editor
    {
        private readonly List<ParameterValueView> _parameterValueView = new();
        private List<KeyAndPath> _oldRefAndKey = new();

        private ReorderableList _reorderableList;
        private ReorderableList _searchList;
        private BGMKeyList _bgmKeyList;
        private float _lineHeight;
        private float _lineHeightSpacing;
        private string _searchText = string.Empty;

        /// <summary>
        /// Structure that stores Property Name
        /// </summary>
        private struct PropNames
        {
            public const string Clips = "Clips";
            public const string List = "list";
            public const string CachedSearchClips = "_cachedSearchClips";
            public const string Key = "Key";
            public const string Guid = "GUID";
            public const string ShowInfo = "ShowInfo";
            public const string Value = "Value";
            public const string Path = "Path";
            public const string Params = "Params";
            public const string Name = "Name";
            public const string DefaultKey = "New Key";
            public const string StageTextField = "StageTextField";
        }
        
        private SerializedProperty _clip;
        private SerializedProperty _clipList;
        private SerializedProperty _cachedClipList;

        private void OnEnable()
        {
            _clip = serializedObject.FindProperty(PropNames.Clips);
            _clipList = _clip.FindPropertyRelative(PropNames.List);
            _cachedClipList = serializedObject.FindProperty(PropNames.CachedSearchClips);

            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("e9081b0172b4f4735ac3dc549b17a6b8");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("2be0f2ec703f2444b8795748b7ffcee3");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("359dcd3866a11460fb6598e8519ce017");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            NKEditorUtility.ApplyIcon(darkIcon, whiteIcon, studioListener);

            if (target == null)
                return;

            _lineHeight = EditorGUIUtility.singleLineHeight;
            _lineHeightSpacing = _lineHeight + 10;

            _bgmKeyList = target as BGMKeyList;

            RefreshOldPath();

            // 리스트 초기화
            for (int i = 0; i < _clipList.arraySize; i++)
                _parameterValueView.Add(new ParameterValueView());

            #region 실제

            _reorderableList = new ReorderableList(serializedObject, _clipList, true,
                true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    if (EditorApplication.isPlaying)
                        EditorGUI.LabelField(rect, "Event Clip List (Editable only Edit mode)");
                    else
                        EditorGUI.LabelField(rect, "Event Clip List");

                    Rect countRect = rect;
                    countRect.width = 50;
                    countRect.x = rect.width - 20;

                    // 리스트 크기 필드 표시
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.IntField(countRect, _clipList.arraySize);
                    EditorGUI.EndDisabledGroup();

                    countRect.width = 50;
                    countRect.x = rect.width - 70;

                    bool isLock = _clipList.arraySize == 0;
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
                            _bgmKeyList.Clips.Reset();
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

                GUIStyle boldLabelStyle = new(EditorStyles.label);
                boldLabelStyle.fontStyle = FontStyle.Bold;
                
                SerializedProperty keyProperty = element.FindPropertyRelative(PropNames.Key);
                SerializedProperty valueValue = element.FindPropertyRelative(PropNames.Value);
                SerializedProperty eventPath = valueValue.FindPropertyRelative(PropNames.Path);
                SerializedProperty eventGuid = element.FindPropertyRelative(PropNames.Guid);
                SerializedProperty targetParams = element.FindPropertyRelative(PropNames.Params);

                string label = keyProperty.stringValue;
                string keyPath = eventPath.stringValue;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, _lineHeight),
                    $"{label} : {keyPath}", boldLabelStyle);

                SerializedProperty showInfo = element.FindPropertyRelative(PropNames.ShowInfo);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 80), rect.y, rect.width, _lineHeight),
                    "Show Info");

                bool showInfoFoldout = EditorGUI.Toggle(new Rect(rect.x + (rect.width - 15), rect.y, 20, 20)
                    , showInfo.boolValue);

                showInfo.boolValue = showInfoFoldout;

                if (showInfo.boolValue)
                {
                    // 이전 값 기록을 위한 변수
                    EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

                    GUI.SetNextControlName(PropNames.StageTextField);
                    label = EditorGUI.TextField(new Rect(rect.x, rect.y + _lineHeightSpacing, rect.width, _lineHeight),
                        label);

                    keyProperty.stringValue = label;

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + _lineHeightSpacing * 2, rect.width, _lineHeight),
                        valueValue, new GUIContent("Event"));

                    EditorEventRef editorEvent = EventManager.EventFromPath(eventPath.stringValue);
                    EditorGUI.EndChangeCheck();

                    ChangePathToDeleteParams(eventPath, targetParams, eventGuid.stringValue);
                    RefreshOldPath();

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
                }
            };

            _reorderableList.elementHeightCallback = index =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty showInfo = element.FindPropertyRelative(PropNames.ShowInfo);

                if (!showInfo.boolValue)
                    return _lineHeightSpacing;

                SerializedProperty foldoutField = element.FindPropertyRelative(PropNames.Value);

                float defaultHeight = _lineHeightSpacing * 3.4f; // Top margin;

                string eventPath = foldoutField.FindPropertyRelative(PropNames.Path).stringValue;
                bool findEvent = EventManager.EventFromPath(eventPath) != null;

                if (findEvent)
                {
                    defaultHeight += _lineHeightSpacing;

                    if (foldoutField.isExpanded)
                        defaultHeight += _lineHeightSpacing * 3;
                }

                SerializedProperty parameterField = element.FindPropertyRelative(PropNames.Params);

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
                Undo.RecordObject(_bgmKeyList, "Create Clip");
                int count = 0;

                for (int i = 0; i < _clipList.arraySize; i++)
                {
                    SerializedProperty key = _clipList.GetArrayElementAtIndex(i).FindPropertyRelative(PropNames.Key);

                    string checkFirstTest;
                    if (key.stringValue.Length < PropNames.DefaultKey.Length)
                        checkFirstTest = key.stringValue;
                    else
                        checkFirstTest = key.stringValue.Substring(0, PropNames.DefaultKey.Length);
                    
                    if (checkFirstTest == PropNames.DefaultKey)
                        count += 1;
                }

                _clipList.arraySize += 1;
                reorderList.index = _clipList.arraySize - 1;
                SerializedProperty element = _clipList.GetArrayElementAtIndex(reorderList.index);

                EventReferenceByKey item = new();
                item.Key = count > 0 ? $"New Key ({count})" : "New Key";
                element.FindPropertyRelative(PropNames.Key).stringValue = item.Key;
                element.FindPropertyRelative(PropNames.Value).FindPropertyRelative(PropNames.Path).stringValue = string.Empty;
                element.FindPropertyRelative(PropNames.Params).ClearArray();
                
                SerializedProperty guid = element.FindPropertyRelative(PropNames.Value).FindPropertyRelative("Guid");
                guid.FindPropertyRelative("Data1").intValue = 0;
                guid.FindPropertyRelative("Data2").intValue = 0;
                guid.FindPropertyRelative("Data3").intValue = 0;
                guid.FindPropertyRelative("Data4").intValue = 0;
                
                element.FindPropertyRelative(PropNames.Guid).stringValue = item.GUID;
                serializedObject.ApplyModifiedProperties();

                RefreshOldPathParameterValueView();
                RefreshOldPath();
            };

            _reorderableList.onRemoveCallback = reorderList =>
            {
                Undo.RecordObject(_bgmKeyList, "Remove Clip");

                int targetIndex = reorderList.index;
                _parameterValueView.RemoveAt(targetIndex);
                _clipList.DeleteArrayElementAtIndex(targetIndex);
                reorderList.index = targetIndex - 1;
                
                RefreshOldPathParameterValueView();
                RefreshOldPath();
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

            _reorderableList.onReorderCallback = list =>
            {
                RefreshOldPath();
            };

            #endregion

            Undo.undoRedoPerformed = () =>
            {
                if (target == null)
                    return;
                
                RefreshOldPath();
                RefreshCachedList();
                serializedObject.Update();
                RefreshOldPathParameterValueView();
            };

            void ChangePathToDeleteParams(SerializedProperty targetPath, SerializedProperty targetParams,
                string targetGuid)
            {
                for (int i = 0; i < _oldRefAndKey.Count; i++)
                    if (_oldRefAndKey[i].GUID == targetGuid)
                        if (targetPath.stringValue != _oldRefAndKey[i].Path)
                            targetParams.ClearArray();
            }
        }

        private void RefreshOldPath()
        {
            _oldRefAndKey.Clear();

            for (int i = 0; i < _clipList.arraySize; i++)
            {
                SerializedProperty list = _clipList.GetArrayElementAtIndex(i);
                SerializedProperty key = list.FindPropertyRelative(PropNames.Key);

                SerializedProperty path = list.FindPropertyRelative(PropNames.Value).FindPropertyRelative(PropNames.Path);
                SerializedProperty guid = list.FindPropertyRelative(PropNames.Guid);

                string oldKey = key.stringValue;
                string oldPath = path.stringValue;
                string oldGuid = guid.stringValue;

                _oldRefAndKey.Add(new KeyAndPath(oldKey, oldPath, oldGuid));
            }
        }

        private void RefreshOldPathParameterValueView()
        {
            _parameterValueView.Clear();
            
            for (int i = 0; i < _clipList.arraySize; i++)
                _parameterValueView.Add(new ParameterValueView());
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(6);
            Rect rect = EditorGUILayout.GetControlRect();

            if (Event.current.type == EventType.MouseUp &&
                GUI.GetNameOfFocusedControl() != PropNames.StageTextField)
            {
                ShowDeleteToMultiKeyMessage();
                ShowDeleteToEmptyKeyMessage(() =>
                {
                    RefreshOldPath();
                    RefreshCachedList();
                    _searchText = string.Empty;
                    _reorderableList.GrabKeyboardFocus();
                    serializedObject.Update();
                });
            }

            // 클립들을 모두 순례돌아서 Cached의 Key와 동일한지 체크
            for (int i = 0; i < _cachedClipList.arraySize; i++)
            for (int j = 0; j < _clipList.arraySize; j++)
            {
                string cachedSearchGuid = _cachedClipList.GetArrayElementAtIndex(i).FindPropertyRelative(PropNames.Guid)
                    .stringValue;
                string clipGuid = _clipList.GetArrayElementAtIndex(j).FindPropertyRelative(PropNames.Guid).stringValue;

                if (cachedSearchGuid == clipGuid)
                {
                    _clipList.GetArrayElementAtIndex(j).objectReferenceValue =
                        _cachedClipList.GetArrayElementAtIndex(i).objectReferenceValue;
                    break;
                }
            }

            _clipList.serializedObject.ApplyModifiedProperties();

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
                RefreshOldPath();
                RefreshCachedList();
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
                _searchList.DoLayoutList();
            else
                _reorderableList.DoLayoutList();

            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        private void RefreshCachedList()
        {
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                _cachedClipList.ArrayClear();

                foreach (SerializedProperty clip in _clipList)
                {
                    string key = clip.FindPropertyRelative(PropNames.Key).stringValue;
                    if (key.Contains(_searchText))
                    {
                        _cachedClipList.arraySize += 1;
                        _cachedClipList.GetArrayElementAtIndex(_cachedClipList.arraySize - 1).objectReferenceValue =
                            clip.objectReferenceValue;
                    }
                }
            }
            else
                _cachedClipList.ArrayClear();

            _cachedClipList.serializedObject.ApplyModifiedProperties();
        }


        private void ShowDeleteToEmptyKeyMessage(Action refresh)
        {
            if (_bgmKeyList.Clips.Count == 0)
                return;

            int count = 0;
            for (int i = 0; i < _bgmKeyList.Clips.Count; i++)
                if (string.IsNullOrWhiteSpace(_bgmKeyList.Clips.GetEventRef(i).Key))
                    count += 1;

            if (count == 0)
                return;

            string title = Application.systemLanguage == SystemLanguage.Korean ? "경고" : "Warning";
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? "키 값을 빈 값으로 설정할 수 없습니다.\n해당 대상은 삭제합니다."
                : "You cannot set the key value to an empty value.\nThe target will be deleted.";
            string yes = Application.systemLanguage == SystemLanguage.Korean ? "넵" : "Yes";

            bool result = EditorUtility.DisplayDialog(title, msg, yes);

            if (result)
            {
                var itemList = _bgmKeyList.Clips.GetList();

                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(_bgmKeyList.Clips.GetEventRef(i).Key))
                    {
                        int targetIndex = i;

                        _parameterValueView.RemoveAt(targetIndex);

                        _clipList.DeleteArrayElementAtIndex(targetIndex);

                        _reorderableList.index = targetIndex - 1;
                        refresh.Invoke();
                        break;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowDeleteToMultiKeyMessage()
        {
            if (_bgmKeyList.Clips.Count == 0)
                return;

            // _localKeyList.Clips.GetEventRef(i).Key가 리스트에 중복으로 2개 이상 있는지 전부 체크
            // 있으면 해당 인덱스를 리스트에 수록
            List<string> targetIndexList = new List<string>();
            for (int i = 0; i < _bgmKeyList.Clips.Count; i++)
            {
                string key = _bgmKeyList.Clips.GetEventRef(i).Key;
                int count = 0;
                for (int j = 0; j < _bgmKeyList.Clips.Count; j++)
                    if (_bgmKeyList.Clips.GetEventRef(j).Key == key)
                        count += 1;

                if (count > 1)
                {
                    // targetIndexList에 key가 없으면 수록
                    if (!targetIndexList.Contains(key))
                        targetIndexList.Add(key);
                }
            }

            if (targetIndexList.Count == 0)
                return;

            string targetIndexString = "";
            for (int i = 0; i < targetIndexList.Count; i++)
            {
                targetIndexString += targetIndexList[i];
                if (i != targetIndexList.Count - 1)
                    targetIndexString += ", ";
            }

            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? $"경고 : Key List에 {targetIndexString}가 중복으로 존재합니다.\n중복 대상을 삭제해주세요."
                : $"Warning : {targetIndexString} exists as a duplicate in the Key List.\nPlease delete the duplicate target.";

            Debug.LogError(msg);
        }

        private class ParameterValueView
        {
            // This holds one SerializedObject for each object in the current selection.
            private readonly List<SerializedObject> _serializedTargets = new();

            // Mappings from EditorParamRef to initial parameter value property for all properties
            // found in the current selection.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // Any parameters that are in the current event but are missing from some objects in
            // the current selection, so we can put them in the "Add" menu.
            private readonly List<EditorParamRef> _missingParameters = new();

            private readonly float _lineHeight;
            private readonly float _lineHeightSpacing;

            private int _parameterIndex;
            private const int ParameterCount = 3;

            public bool IsOpenParameterArea;
            private string _oldEventPath;

            // A mapping from EditorParamRef to the initial parameter value properties in the
            // current selection that have the same name.
            // We need this because some objects may be missing some properties, and properties with
            // the same name may be at different array indices in different objects.
            private class PropertyRecord
            {
                public string Name => ParamRef.Name;

                public EditorParamRef ParamRef;
                public List<SerializedProperty> ValueProperties;
            }

            public ParameterValueView()
            {
                _lineHeight = EditorGUIUtility.singleLineHeight;
                _lineHeightSpacing = _lineHeight + 10;
            }

            // Rebuilds the propertyRecords and missingParameters collections.
            private void RefreshPropertyRecords(SerializedProperty serializedTarget, EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = serializedTarget.FindPropertyRelative(PropNames.Params);

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative(PropNames.Name).stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative(PropNames.Value);

                    PropertyRecord record =
                        _propertyRecords.Find(r => r.Name == name);

                    if (record != null)
                    {
                        record.ValueProperties.Add(valueProperty);
                    }
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(p => p.Name == name);

                        if (paramRef != null)
                        {
                            _propertyRecords.Add(
                                new PropertyRecord()
                                {
                                    ParamRef = paramRef,
                                    ValueProperties = new List<SerializedProperty>() { valueProperty },
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
                        return record == null || record.ValueProperties.Count < _serializedTargets.Count;
                    }));
            }

            public string GetPrePath()
            {
                return _oldEventPath;
            }

            public void OnGUI(Rect rect, SerializedProperty target, EditorEventRef eventRef, bool matchingEvents)
            {
                _parameterIndex = 0;
                _oldEventPath = eventRef.Path;
                target.serializedObject.Update();

                if (Event.current.type == EventType.Layout)
                    RefreshPropertyRecords(target, eventRef);

                SerializedProperty paramsProperty = target.FindPropertyRelative(PropNames.Params);

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

                foreach (SerializedObject serializedTarget in _serializedTargets)
                    serializedTarget.ApplyModifiedProperties();
            }

            private void DrawHeader(Rect rect, SerializedProperty target, bool enableAddButton)
            {
                SerializedProperty eventInfo = target.FindPropertyRelative(PropNames.Value);
                SerializedProperty paramsProperty = target.FindPropertyRelative(PropNames.Params);

                int nextHeight = 3;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                Rect titleRect = new(rect.x, rect.y + _lineHeightSpacing * nextHeight, rect.width, _lineHeight)
                {
                    width = EditorGUIUtility.labelWidth
                };

                // Let the user revert the whole Params array to prefab by context-clicking the title.
                EditorGUI.BeginProperty(titleRect, GUIContent.none, paramsProperty);

                paramsProperty.isExpanded = EditorGUI.Foldout(titleRect, paramsProperty.isExpanded,
                    "Initial Parameter Values");

                EditorGUI.EndProperty();

                EditorGUI.BeginDisabledGroup(!enableAddButton);

                Rect position = new(rect.x, rect.y + _lineHeightSpacing * nextHeight, rect.width, _lineHeight);
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
                    if (record.ValueProperties.Count == 1)
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
                SerializedProperty eventInfo = target.FindPropertyRelative(PropNames.Value);

                int nextHeight = 4 + _parameterIndex;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                delete = false;

                GUIContent removeLabel = new("Remove");
                Rect position = new(rect.x, rect.y + _lineHeightSpacing * nextHeight, rect.width,
                    _lineHeight);

                _parameterIndex += 1;

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
                if (record.ValueProperties.Count == 1)
                {
                    value = record.ValueProperties[0].floatValue;
                    EditorGUI.BeginProperty(position, nameLabel, record.ValueProperties[0]);
                }
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
                        else if (property.floatValue != value)
                        {
                            mixedValues = true;
                            break;
                        }
                    }
                }

                EditorGUI.LabelField(nameLabelRect, nameLabel);

                if (record.ParamRef.Type == ParameterType.Labeled)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    int newValue = EditorGUI.Popup(sliderRect, (int)value, record.ParamRef.Labels);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.ValueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }
                else if (record.ParamRef.Type == ParameterType.Discrete)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    int newValue = EditorGUI.IntSlider(sliderRect, (int)value, (int)record.ParamRef.Min,
                        (int)record.ParamRef.Max);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.ValueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUI.showMixedValue = mixedValues;

                    float newValue = EditorGUI.Slider(sliderRect, value, record.ParamRef.Min, record.ParamRef.Max);

                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (SerializedProperty property in record.ValueProperties)
                        {
                            property.floatValue = newValue;
                        }
                    }
                }

                delete = GUI.Button(removeButtonRect, removeLabel, EditorStyles.miniButton);

                if (record.ValueProperties.Count == 1)
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

                        foreach (SerializedProperty sourceProperty in record.ValueProperties)
                        {
                            UnityEngine.Object targetObject = sourceProperty.serializedObject.targetObject;

                            menu.AddItem(new GUIContent(string.Format("Set to Value of '{0}'", targetObject.name)),
                                false,
                                (userData) => CopyValueToAll(userData as SerializedProperty, record.ValueProperties),
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
                    string paramName = paramProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PropNames.Name).stringValue;

                    if (paramName == parameter.Name)
                        return;
                }

                int index = paramProperty.arraySize;
                paramProperty.InsertArrayElementAtIndex(index);
                SerializedProperty arrayElement = paramProperty.GetArrayElementAtIndex(index);
                arrayElement.FindPropertyRelative(PropNames.Name).stringValue = parameter.Name;
                arrayElement.FindPropertyRelative(PropNames.Value).floatValue = parameter.Default;
                paramProperty.serializedObject.ApplyModifiedProperties();
            }

            // Delete initial parameter values for the given name from all selected objects.
            private void DeleteParameter(string name, SerializedProperty target)
            {
                SerializedProperty paramsProperty = target.FindPropertyRelative(PropNames.Params);
                foreach (SerializedProperty child in paramsProperty)
                {
                    if (child.FindPropertyRelative(PropNames.Name).stringValue == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }
            }
        }
    }
}