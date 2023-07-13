#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FMODUnity
{
    [CustomEditor(typeof(RegisterEventClip))]
    public class FMODRegisterEventClipEditor : Editor
    {
        private List<ParameterValueView> parameterValueView = new();

        private ReorderableList _reorderableList;
        private RegisterEventClip _registerEventClip;

        private float lineHeight;
        private float lineHeightSpacing;

        private void OnEnable()
        {
            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("e9081b0172b4f4735ac3dc549b17a6b8");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("2be0f2ec703f2444b8795748b7ffcee3");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/RegisterEventClip.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

            if (target == null)
                return;

            lineHeight = EditorGUIUtility.singleLineHeight;
            lineHeightSpacing = lineHeight + 10;

            _registerEventClip = target as RegisterEventClip;

            SerializedProperty clips = serializedObject.FindProperty("clips");
            SerializedProperty list = clips.FindPropertyRelative("_list");

            // 리스트 초기화
            for (var i = 0; i < list.arraySize; i++)
                parameterValueView.Add(new ParameterValueView());

            _reorderableList = new ReorderableList(serializedObject, list, true,
                true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    if (EditorApplication.isPlaying)
                        EditorGUI.LabelField(rect, "Event Clip List (Editable only Edit mode)");
                    else
                        EditorGUI.LabelField(rect, "Event Clip List");


                    var p = rect;
                    p.width = 50;
                    p.x = rect.width - 20;

                    // 리스트 크기 필드 표시
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.IntField(p, list.arraySize);
                    EditorGUI.EndDisabledGroup();

                    p.width = 50;
                    p.x = rect.width - 70;

                    bool isLock = list.arraySize == 0;
                    EditorGUI.BeginDisabledGroup(isLock);
                    if (GUI.Button(new Rect(p.x, p.y, p.width, lineHeight), "Reset"))
                    {
                        string title = Application.systemLanguage == SystemLanguage.Korean ? "경고" : "Warning";
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "정말로 리셋하시겠습니까?"
                            : "Do you really want to reset?";
                        string yes = Application.systemLanguage == SystemLanguage.Korean ? "넵" : "Yes";
                        string no = Application.systemLanguage == SystemLanguage.Korean ? "아니요.." : "No";

                        bool result = EditorUtility.DisplayDialog(title, msg, yes, no);

                        if (result)
                            _registerEventClip.ResetList();

                        serializedObject.Update();
                    }

                    EditorGUI.EndDisabledGroup();
                    serializedObject.ApplyModifiedProperties();
                }
            };

            _reorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                var keyProperty = element.FindPropertyRelative("Key");
                string label = keyProperty.stringValue;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight),
                    label);

                var showInfo = element.FindPropertyRelative("ShowInfo");
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 80), rect.y, rect.width, lineHeight), "Show Info");
                var showInfoFoldout = EditorGUI.Toggle(new Rect(rect.x + (rect.width - 15), rect.y, 20, 20)
                    , showInfo.boolValue);

                showInfo.boolValue = showInfoFoldout;

                if (!showInfo.boolValue)
                    return;
                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                label = EditorGUI.TextField(new Rect(rect.x, rect.y + lineHeightSpacing, rect.width, lineHeight),
                    label);
                keyProperty.stringValue = label;

                var valueValue = element.FindPropertyRelative("Value");
                var eventPath = valueValue.FindPropertyRelative("Path");

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpacing * 2, rect.width, lineHeight),
                    valueValue, new GUIContent("Event"));

                EditorEventRef editorEvent = EventManager.EventFromPath(eventPath.stringValue);
                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();

                if (!editorEvent)
                {
                    EditorGUI.EndDisabledGroup();
                    return;
                }

                parameterValueView[index].OnGUI(rect, element, editorEvent, !element.hasMultipleDifferentValues);
                EditorGUI.EndDisabledGroup();
                serializedObject.ApplyModifiedProperties();
            };

            _reorderableList.elementHeightCallback = (index) =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var showInfo = element.FindPropertyRelative("ShowInfo");

                if (!showInfo.boolValue)
                    return lineHeightSpacing;

                SerializedProperty foldoutField = element.FindPropertyRelative("Value");

                float defaultHeight = lineHeightSpacing * 3.4f; // Top margin;

                if (!string.IsNullOrWhiteSpace(foldoutField.FindPropertyRelative("Path").stringValue))
                {
                    defaultHeight += lineHeightSpacing;

                    if (foldoutField.isExpanded)
                    {
                        defaultHeight += lineHeightSpacing * 3;
                    }
                }

                var parameterField = element.FindPropertyRelative("Params");

                if (parameterField.isExpanded)
                {
                    int parameterCount = parameterField.arraySize;

                    for (int i = 0; i < parameterCount; i++)
                        defaultHeight += lineHeightSpacing;
                }

                //return height;
                return defaultHeight;
            };

            _reorderableList.onAddDropdownCallback = (rect, reorderList) =>
            {
                parameterValueView.Add(new ParameterValueView());
                // 요소 추가 전 배열 크기 조정
                _registerEventClip.Add();
            };

            _reorderableList.onRemoveCallback = (ReorderableList reorderList) =>
            {
                parameterValueView.RemoveAt(reorderList.index);
                _registerEventClip.RemoveClip(reorderList.index);
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(6);
            _reorderableList.DoLayoutList();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        private class ParameterValueView
        {
            // This holds one SerializedObject for each object in the current selection.
            private List<SerializedObject> serializedTargets = new List<SerializedObject>();

            // Mappings from EditorParamRef to initial parameter value property for all properties
            // found in the current selection.
            private List<PropertyRecord> propertyRecords =
                new List<PropertyRecord>();

            // Any parameters that are in the current event but are missing from some objects in
            // the current selection, so we can put them in the "Add" menu.
            private List<EditorParamRef> missingParameters = new List<EditorParamRef>();

            private readonly float lineHeight;
            private readonly float lineHeightSpacing;

            private int parameterIndex;
            private const int ParameterCount = 3;

            public bool IsOpenParameterArea;

            // A mapping from EditorParamRef to the initial parameter value properties in the
            // current selection that have the same name.
            // We need this because some objects may be missing some properties, and properties with
            // the same name may be at different array indices in different objects.
            private class PropertyRecord
            {
                public string name
                {
                    get { return paramRef.Name; }
                }

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
                propertyRecords.Clear();

                SerializedProperty paramsProperty = serializedTarget.FindPropertyRelative("Params");

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                    PropertyRecord record =
                        propertyRecords.Find(r => r.name == name);

                    if (record != null)
                    {
                        record.valueProperties.Add(valueProperty);
                    }
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(p => p.Name == name);

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

                // Only sort if there is a multi-selection. If there is only one object selected,
                // the user can revert to prefab, and the behaviour depends on the array order,
                // so it's helpful to show the true order.

                propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.name, b.name));

                missingParameters.Clear();
                missingParameters.AddRange(eventRef.LocalParameters.Where(
                    p =>
                    {
                        PropertyRecord record =
                            propertyRecords.Find(r => r.name == p.Name);
                        return record == null || record.valueProperties.Count < serializedTargets.Count;
                    }));
            }

            public void OnGUI(Rect rect, SerializedProperty target, EditorEventRef eventRef, bool matchingEvents)
            {
                parameterIndex = 0;
                target.serializedObject.Update();

                if (Event.current.type == EventType.Layout)
                    RefreshPropertyRecords(target, eventRef);

                SerializedProperty paramsProperty = target.FindPropertyRelative("Params");

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
                SerializedProperty eventInfo = target.FindPropertyRelative("Value");
                SerializedProperty paramsProperty = target.FindPropertyRelative("Params");

                int nextHeight = 3;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                Rect titleRect = new Rect(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width, lineHeight)
                {
                    width = EditorGUIUtility.labelWidth
                };

                // Let the user revert the whole Params array to prefab by context-clicking the title.
                EditorGUI.BeginProperty(titleRect, GUIContent.none, paramsProperty);

                paramsProperty.isExpanded = EditorGUI.Foldout(titleRect, paramsProperty.isExpanded,
                    "Initial Parameter Values");

                EditorGUI.EndProperty();

                EditorGUI.BeginDisabledGroup(!enableAddButton);

                Rect position = new Rect(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width, lineHeight);
                position.xMin = titleRect.xMax;
                DrawAddButton(position, paramsProperty);

                EditorGUI.EndDisabledGroup();
            }

            private void DrawAddButton(Rect position, SerializedProperty paramsProperty)
            {
                EditorGUI.BeginDisabledGroup(missingParameters.Count == 0);

                if (EditorGUI.DropdownButton(position, new GUIContent("Add"), FocusType.Passive))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("All"), false, () =>
                    {
                        foreach (EditorParamRef parameter in missingParameters)
                            AddParameter(parameter, paramsProperty);
                    });

                    menu.AddSeparator(string.Empty);

                    foreach (EditorParamRef parameter in missingParameters)
                    {
                        menu.AddItem(new GUIContent(parameter.Name), false,
                            (userData) => { AddParameter(userData as EditorParamRef, paramsProperty); },
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

                foreach (PropertyRecord record in propertyRecords)
                    if (record.valueProperties.Count == 1)
                    {
                        DrawValue(rect, target, record, out var delete);

                        if (delete)
                            parameterToDelete = record.name;
                    }

                if (parameterToDelete != null)
                    DeleteParameter(parameterToDelete, target);
            }

            private void DrawValue(Rect rect, SerializedProperty target, PropertyRecord record, out bool delete)
            {
                SerializedProperty eventInfo = target.FindPropertyRelative("Value");

                int nextHeight = 4 + parameterIndex;
                if (eventInfo.isExpanded)
                    nextHeight += ParameterCount;

                delete = false;

                GUIContent removeLabel = new GUIContent("Remove");
                Rect position = new Rect(rect.x, rect.y + lineHeightSpacing * nextHeight, rect.width,
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

                GUIContent nameLabel = new GUIContent(record.name);

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
                        GenericMenu menu = new GenericMenu();

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
                    string paramName = paramProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;

                    if (paramName == parameter.Name)
                        return;
                }

                int index = paramProperty.arraySize;
                paramProperty.InsertArrayElementAtIndex(index);
                SerializedProperty arrayElement = paramProperty.GetArrayElementAtIndex(index);
                arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;
                paramProperty.serializedObject.ApplyModifiedProperties();
            }

            // Delete initial parameter values for the given name from all selected objects.
            private void DeleteParameter(string name, SerializedProperty target)
            {
                SerializedProperty paramsProperty = target.FindPropertyRelative("Params");
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