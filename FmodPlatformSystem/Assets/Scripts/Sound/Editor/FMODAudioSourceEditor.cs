using System;
using UnityEditor;
using UnityEngine;

namespace FMODUnity
{
    [CustomEditor(typeof(FMODAudioSource))]
    [CanEditMultipleObjects]
    public class FMODAudioSourceEditor : Editor
    {
        public Texture2D DarkIcon;
        public Texture2D WhiteIcon;

        private void OnEnable()
        {
            InitIcon();
        }

        private void InitIcon()
        {
            if (!DarkIcon || !WhiteIcon)
            {
                
                Debug.LogWarning("No Binding Icon");
                EditorGUIUtility.SetIconForObject(target, null);
                return;
            }

            bool isDarkMode = EditorGUIUtility.isProSkin;

            if (isDarkMode)
            {
                if (DarkIcon)
                    EditorGUIUtility.SetIconForObject(target, DarkIcon);
            }
            else
            {
                if (WhiteIcon)
                    EditorGUIUtility.SetIconForObject(target, WhiteIcon);
            }
        }

        public void OnSceneGUI()
        {
            var emitter = target as FMODAudioSource;

            EditorEventRef editorEvent = EventManager.EventFromGUID(emitter.Clip.Guid);
            if (editorEvent != null && editorEvent.Is3D)
            {
                EditorGUI.BeginChangeCheck();
                float minDistance = emitter.OverrideAttenuation ? emitter.OverrideMinDistance : editorEvent.MinDistance;
                float maxDistance = emitter.OverrideAttenuation ? emitter.OverrideMaxDistance : editorEvent.MaxDistance;
                minDistance = Handles.RadiusHandle(Quaternion.identity, emitter.transform.position, minDistance);
                maxDistance = Handles.RadiusHandle(Quaternion.identity, emitter.transform.position, maxDistance);
                if (EditorGUI.EndChangeCheck() && emitter.OverrideAttenuation)
                {
                    Undo.RecordObject(emitter, "Change Emitter Bounds");
                    emitter.OverrideMinDistance = Mathf.Clamp(minDistance, 0, emitter.OverrideMaxDistance);
                    emitter.OverrideMaxDistance = Mathf.Max(emitter.OverrideMinDistance, maxDistance);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            var eventReference = serializedObject.FindProperty("_clip");
            var isMute = serializedObject.FindProperty("_mute");
            var playOnAwake = serializedObject.FindProperty("PlayOnAwake");
            var volume = serializedObject.FindProperty("_volume");
            var pitch = serializedObject.FindProperty("_pitch");
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
                EditorUtils.UpdateParamsOnEmitter(serializedObject, eventPath.stringValue);
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
    }
}