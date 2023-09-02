#if UNITY_EDITOR
using System.Collections.Generic;
using FMODUnity;
using NKStudio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODPlus
{
    [CustomEditor(typeof(FMODEventTrigger))]
    public class FMODEventTriggerEditor : Editor
    {
        private SerializedProperty _triggerType;
        private SerializedProperty _source;
        private SerializedProperty _commandSender;
        private SerializedProperty _begin;
        private SerializedProperty _end;
        private SerializedProperty _tag;

        private StyleSheet _groupBoxStyleSheet;

        private VisualElement _root;

        private void OnEnable()
        {
            // Register Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("07555e228b1ea40bb871f7540d3d2022");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3443518b4ee364e1b9cf151024acc8cc");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("c27c81a62993d4462a47ac11bb3f8330");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            NKEditorUtility.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupStyleSheetPath = AssetDatabase.GUIDToAssetPath("5600a59cbafd24acf808fa415167310e");
            _groupBoxStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupStyleSheetPath);
        }

        private void FindProperty()
        {
            _triggerType = serializedObject.FindProperty("triggerType");
            _source = serializedObject.FindProperty("source");
            _commandSender = serializedObject.FindProperty("commandSender");
            _begin = serializedObject.FindProperty("PlayEvent");
            _end = serializedObject.FindProperty("StopEvent");
            _tag = serializedObject.FindProperty("CollisionTag");
        }

        private void InitializeRoot()
        {
            _root = new VisualElement();
            _root.styleSheets.Add(_groupBoxStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            PropertyField triggerTypeField = new(_triggerType);
            PropertyField audioSourceField = new(_source);
            PropertyField commandSenderField = new(_commandSender);
            
            EnumField beginField = new();
            beginField.BindProperty(_begin);
            beginField.label = "Play Event";
            beginField.AddToClassList("unity-base-field__aligned");
            
            EnumField endField = new();
            endField.BindProperty(_end);
            endField.label = "Stop Event";
            endField.AddToClassList("unity-base-field__aligned");
            
            TagField tagField = new();
            tagField.label = "Collision Tag";
            tagField.BindProperty(_tag);
            tagField.AddToClassList("unity-base-field__aligned");

            GroupBox groupBox = new();
            groupBox.Add(triggerTypeField);
            groupBox.Add(audioSourceField);
            groupBox.Add(commandSenderField);
            groupBox.AddToClassList("GroupBoxStyle");

            GroupBox groupBox2 = new();
            groupBox2.AddToClassList("GroupBoxStyle");
            groupBox2.Add(tagField);
            groupBox2.Add(beginField);
            groupBox2.Add(endField);
            
            _root.Add(groupBox);
            _root.Add(NKEditorUtility.Space(1));
            _root.Add(groupBox2);
            
            VisualElement[] elements =
                { 
                    triggerTypeField /*0*/,
                    audioSourceField /*1*/,
                    beginField /*2*/,
                    endField /*3*/,
                    tagField /*4*/,
                    commandSenderField /*5*/
                };

            InitControlField(elements);
        }

        private void InitControlField(IReadOnlyList<VisualElement> elements)
        {
            PropertyField triggerTypeField = (PropertyField)elements[0];
            EnumField beginField = (EnumField)elements[2];
            EnumField endField = (EnumField)elements[3];

            ControlField(elements);

            triggerTypeField.schedule.Execute(() =>
                triggerTypeField.RegisterValueChangeCallback(_ => ControlField(elements)));

            beginField.schedule.Execute(() =>
                beginField.RegisterValueChangedCallback(_ => ControlField(elements)));

            endField.schedule.Execute(() =>
                endField.RegisterValueChangedCallback(_ => ControlField(elements)));
        }

        private void ControlField(IReadOnlyList<VisualElement> elements)
        {
            VisualElement triggerTypeField = elements[0];
            VisualElement audioSourceField = elements[1];
            EnumField beginField = (EnumField)elements[2];
            EnumField endField = (EnumField)elements[3];
            VisualElement collisionTagField = elements[4];
            VisualElement commandSenderField = elements[5];

            foreach (VisualElement element in elements)
                element.SetActive(false);
            
            triggerTypeField.SetActive(true);
            
            switch (_triggerType.enumValueIndex)
            {
                case 0:
                {
                    audioSourceField.SetActive(true);
                    beginField.SetActive(true);
                    endField.SetActive(true);
                    beginField.label = "Play Event";
                
                    if (OpenCollisionTagField(_begin, _end))
                        collisionTagField.SetActive(true);
                    break;
                }
                case 1:
                    commandSenderField.SetActive(true);
                    beginField.SetActive(true);
                    beginField.label = "Send Event";
                    
                    if (OpenCollisionTagField(_begin, _end))
                        collisionTagField.SetActive(true);
                    break;
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperty();
            InitializeRoot();
            return _root;
        }

        private bool OpenCollisionTagField(SerializedProperty begin, SerializedProperty end)
        {
            return begin.enumValueIndex is >= (int)EmitterGameEvent.TriggerEnter
                       and <= (int)EmitterGameEvent.TriggerExit2D ||
                   end.enumValueIndex is >= (int)EmitterGameEvent.TriggerEnter
                       and <= (int)EmitterGameEvent.TriggerExit2D;
        }
    }
}
#endif