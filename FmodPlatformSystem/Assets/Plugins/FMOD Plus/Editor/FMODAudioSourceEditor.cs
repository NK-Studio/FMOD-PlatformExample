#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using NKStudio;
using UnityEditor;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(FMODAudioSource))]
    [CanEditMultipleObjects]
    public class FMODAudioSourceEditor : Editor
    {
        private ParameterValueView parameterValueView;

        private void OnEnable()
        {
            parameterValueView = new ParameterValueView(serializedObject);

            // Audio Source
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a10e3143b46034572a9a5e38034e181c");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("ae854481e58584815befcbf213b32745");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("5d6ba1758301f46408fd7c2c251e5ac2");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            NKEditorUtility.ApplyIcon(darkIcon, whiteIcon, studioListener);
        }

        public void OnSceneGUI()
        {
            var audioSource = target as FMODAudioSource;

            EditorEventRef editorEvent = EventManager.EventFromGUID(audioSource.clip.Guid);
            if (editorEvent != null && editorEvent.Is3D)
            {
                EditorGUI.BeginChangeCheck();
                float minDistance = audioSource.OverrideAttenuation
                    ? audioSource.OverrideMinDistance
                    : editorEvent.MinDistance;
                float maxDistance = audioSource.OverrideAttenuation
                    ? audioSource.OverrideMaxDistance
                    : editorEvent.MaxDistance;
                minDistance = Handles.RadiusHandle(Quaternion.identity, audioSource.transform.position, minDistance);
                maxDistance = Handles.RadiusHandle(Quaternion.identity, audioSource.transform.position, maxDistance);
                if (EditorGUI.EndChangeCheck() && audioSource.OverrideAttenuation)
                {
                    Undo.RecordObject(audioSource, "Change Audio Source Bounds");
                    audioSource.OverrideMinDistance = Mathf.Clamp(minDistance, 0, audioSource.OverrideMaxDistance);
                    audioSource.OverrideMaxDistance = Mathf.Max(audioSource.OverrideMinDistance, maxDistance);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            var eventReference = serializedObject.FindProperty("_clip");
            var isMute = serializedObject.FindProperty("_mute");
            var playOnAwake = serializedObject.FindProperty("playOnAwake");
            var volume = serializedObject.FindProperty("volume");
            var pitch = serializedObject.FindProperty("pitch");
            var eventPath = eventReference.FindPropertyRelative("Path");
            var fadeout = serializedObject.FindProperty("AllowFadeout");
            var once = serializedObject.FindProperty("TriggerOnce");
            var preload = serializedObject.FindProperty("Preload");

            var overrideAtt = serializedObject.FindProperty("OverrideAttenuation");
            var minDistance = serializedObject.FindProperty("OverrideMinDistance");
            var maxDistance = serializedObject.FindProperty("OverrideMaxDistance");

            EditorGUI.BeginChangeCheck();

            const string EventReferenceLabel = "Event";

            EditorUtils.DrawLegacyEvent(serializedObject.FindProperty("Event"), EventReferenceLabel);

            EditorGUILayout.PropertyField(eventReference, new GUIContent(EventReferenceLabel));

            EditorEventRef editorEvent = EventManager.EventFromPath(eventPath.stringValue);

            if (EditorGUI.EndChangeCheck())
            {
                FMODEditorUtility.UpdateParamsOnEmitter(serializedObject, eventPath.stringValue);
            }

            // Attenuation
            if (editorEvent != null)
            {
                {
                    EditorGUI.BeginDisabledGroup(editorEvent == null || !editorEvent.Is3D);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(overrideAtt);
                    if (EditorGUI.EndChangeCheck() ||
                        (minDistance.floatValue == -1 && maxDistance.floatValue == -1) || // never been initialiased
                        !overrideAtt.boolValue &&
                        (minDistance.floatValue != editorEvent.MinDistance ||
                         maxDistance.floatValue != editorEvent.MaxDistance)
                       )
                    {
                        minDistance.floatValue = editorEvent.MinDistance;
                        maxDistance.floatValue = editorEvent.MaxDistance;
                    }

                    EditorGUI.BeginDisabledGroup(!overrideAtt.boolValue);
                    EditorGUIUtility.labelWidth = 30;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(minDistance, new GUIContent("Min"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        minDistance.floatValue = Mathf.Clamp(minDistance.floatValue, 0, maxDistance.floatValue);
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(maxDistance, new GUIContent("Max"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        maxDistance.floatValue = Mathf.Max(minDistance.floatValue, maxDistance.floatValue);
                    }

                    EditorGUIUtility.labelWidth = 0;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();
                }

                parameterValueView.OnGUI(editorEvent, !eventReference.hasMultipleDifferentValues);
                EditorGUILayout.Space(10);

                fadeout.isExpanded = EditorGUILayout.Foldout(fadeout.isExpanded, "Advanced Controls");
                if (fadeout.isExpanded)
                {
                    EditorGUILayout.PropertyField(preload, new GUIContent("Preload Sample Data"));
                    EditorGUILayout.PropertyField(fadeout, new GUIContent("Allow Fadeout When Stopping"));
                    EditorGUILayout.PropertyField(once, new GUIContent("Trigger Once"));
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(isMute, new GUIContent("Mute"));
            EditorGUILayout.PropertyField(playOnAwake, new GUIContent("Play On Awake"));
            EditorGUILayout.PropertyField(volume, new GUIContent("Volume"));
            EditorGUILayout.PropertyField(pitch, new GUIContent("Pitch"));

            serializedObject.ApplyModifiedProperties();
        }

        private class ParameterValueView
        {
            // The "Params" property from the SerializedObject we're editing in the inspector,
            // so we can expand/collapse it or revert to prefab.
            private SerializedProperty paramsProperty;

            // This holds one SerializedObject for each object in the current selection.
            private List<SerializedObject> serializedTargets = new List<SerializedObject>();

            // Mappings from EditorParamRef to initial parameter value property for all properties
            // found in the current selection.
            private List<PropertyRecord> propertyRecords = new List<PropertyRecord>();

            // Any parameters that are in the current event but are missing from some objects in
            // the current selection, so we can put them in the "Add" menu.
            private List<EditorParamRef> missingParameters = new List<EditorParamRef>();

            // A mapping from EditorParamRef to the initial parameter value properties in the
            // current selection that have the same name.
            // We need this because some objects may be missing some properties, and properties with
            // the same name may be at different array indices in different objects.
            private class PropertyRecord
            {
                public string name => paramRef.Name;

                public EditorParamRef paramRef;
                public List<SerializedProperty> valueProperties;
            }

            public ParameterValueView(SerializedObject serializedObject)
            {
                paramsProperty = serializedObject.FindProperty("Params");

                foreach (UnityEngine.Object target in serializedObject.targetObjects)
                {
                    serializedTargets.Add(new SerializedObject(target));
                }
            }

            // Rebuilds the propertyRecords and missingParameters collections.
            private void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                propertyRecords.Clear();

                foreach (SerializedObject serializedTarget in serializedTargets)
                {
                    SerializedProperty paramsProperty = serializedTarget.FindProperty("Params");

                    foreach (SerializedProperty parameterProperty in paramsProperty)
                    {
                        string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                        SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

                        PropertyRecord record = propertyRecords.Find(r => r.name == name);

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
                }

                // Only sort if there is a multi-selection. If there is only one object selected,
                // the user can revert to prefab, and the behaviour depends on the array order,
                // so it's helpful to show the true order.
                if (serializedTargets.Count > 1)
                {
                    propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.name, b.name));
                }

                missingParameters.Clear();
                missingParameters.AddRange(eventRef.LocalParameters.Where(
                    p =>
                    {
                        PropertyRecord record = propertyRecords.Find(r => r.name == p.Name);
                        return record == null || record.valueProperties.Count < serializedTargets.Count;
                    }));
            }

            public void OnGUI(EditorEventRef eventRef, bool matchingEvents)
            {
                foreach (SerializedObject serializedTarget in serializedTargets)
                {
                    serializedTarget.Update();
                }

                if (Event.current.type == EventType.Layout)
                {
                    RefreshPropertyRecords(eventRef);
                }

                DrawHeader(matchingEvents);

                if (paramsProperty.isExpanded)
                {
                    if (matchingEvents)
                    {
                        DrawValues();
                    }
                    else
                    {
                        var msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "다른 이벤트가 선택된 경우 매개변수를 변경할 수 없습니다."
                            : "Cannot change parameters when different events are selected";

                        GUILayout.Box(msg,
                            GUILayout.ExpandWidth(true));
                    }
                }

                foreach (SerializedObject serializedTarget in serializedTargets)
                {
                    serializedTarget.ApplyModifiedProperties();
                }
            }

            private void DrawHeader(bool enableAddButton)
            {
                Rect controlRect = EditorGUILayout.GetControlRect();

                Rect titleRect = controlRect;
                titleRect.width = EditorGUIUtility.labelWidth;

                // Let the user revert the whole Params array to prefab by context-clicking the title.
                EditorGUI.BeginProperty(titleRect, GUIContent.none, paramsProperty);

                paramsProperty.isExpanded = EditorGUI.Foldout(titleRect, paramsProperty.isExpanded,
                    "Initial Parameter Values");

                EditorGUI.EndProperty();

                Rect buttonRect = controlRect;
                buttonRect.xMin = titleRect.xMax;

                EditorGUI.BeginDisabledGroup(!enableAddButton);

                DrawAddButton(buttonRect);

                EditorGUI.EndDisabledGroup();
            }

            private void DrawAddButton(Rect position)
            {
                EditorGUI.BeginDisabledGroup(missingParameters.Count == 0);

                if (EditorGUI.DropdownButton(position, new GUIContent("Add"), FocusType.Passive))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("All"), false, () =>
                    {
                        foreach (EditorParamRef parameter in missingParameters)
                        {
                            AddParameter(parameter);
                        }
                    });

                    menu.AddSeparator(string.Empty);

                    foreach (EditorParamRef parameter in missingParameters)
                    {
                        menu.AddItem(new GUIContent(parameter.Name), false,
                            (userData) => { AddParameter(userData as EditorParamRef); },
                            parameter);
                    }

                    menu.DropDown(position);
                }

                EditorGUI.EndDisabledGroup();
            }

            private void DrawValues()
            {
                // We use this to defer deletion so we don't mess with arrays while using
                // SerializedProperties that refer to array elements, as this can throw exceptions.
                string parameterToDelete = null;

                foreach (PropertyRecord record in propertyRecords)
                {
                    if (record.valueProperties.Count == serializedTargets.Count)
                    {
                        DrawValue(record, out var delete);

                        if (delete)
                            parameterToDelete = record.name;
                    }
                }

                if (parameterToDelete != null)
                    DeleteParameter(parameterToDelete);
            }

            private void DrawValue(PropertyRecord record, out bool delete)
            {
                delete = false;

                GUIContent removeLabel = new GUIContent("Remove");

                Rect position = EditorGUILayout.GetControlRect();

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

                            menu.AddItem(new GUIContent($"Set to Value of '{targetObject.name}'"),
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

            // Add an initial value for the given parameter to all selected objects that don't have one.
            private void AddParameter(EditorParamRef parameter)
            {
                foreach (SerializedObject serializedTarget in serializedTargets)
                {
                    FMODAudioSource audioSource = serializedTarget.targetObject as FMODAudioSource;

                    if (Array.FindIndex(audioSource.Params, p => p.Name == parameter.Name) < 0)
                    {
                        SerializedProperty paramsProperty = serializedTarget.FindProperty("Params");

                        int index = paramsProperty.arraySize;
                        paramsProperty.InsertArrayElementAtIndex(index);

                        SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                        arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                        arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                        serializedTarget.ApplyModifiedProperties();
                    }
                }
            }

            // Delete initial parameter values for the given name from all selected objects.
            private void DeleteParameter(string name)
            {
                foreach (SerializedObject serializedTarget in serializedTargets)
                {
                    SerializedProperty paramsProperty = serializedTarget.FindProperty("Params");

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
}
#endif