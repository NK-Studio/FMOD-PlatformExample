using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODUnity
{
    [CustomEditor(typeof(FMODParameterSender))]
    [CanEditMultipleObjects]
    public class FMODParameterSenderEditor : Editor
    {
        private FMODParameterSender parameterSender;

        [SerializeField] private StyleSheet groupBoxStyleSheet;
        [SerializeField] private StyleSheet buttonStyleSheet;

        private void OnEnable()
        {
            // Parameter Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("74cfbd073c7464035ba232171ef31f0f");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("6531cd3743c664274b21aa41c9b00c5c");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = "Assets/Plugins/FMODPlus/Runtime/FMODParameterSender.cs";
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);
        }

        public override VisualElement CreateInspectorGUI()
        {
            parameterSender = (FMODParameterSender)target;

            var root = new VisualElement();
            root.styleSheets.Add(groupBoxStyleSheet);
            root.styleSheets.Add(buttonStyleSheet);

            var root0 = new VisualElement();
            root0.AddToClassList("GroupBoxStyle");

            var behaviourStyleField = new PropertyField(serializedObject.FindProperty("BehaviourStyle"));
            var sourceField = new PropertyField(serializedObject.FindProperty("Source"));

            var root1 = new VisualElement();
            root1.AddToClassList("GroupBoxStyle");

            var parameterFiled = new PropertyField(serializedObject.FindProperty("ParameterName"));
            var valueField = new PropertyField(serializedObject.FindProperty("Value"));
            var sendOnStartField = new PropertyField(serializedObject.FindProperty("SendOnStart"));
            var isGlobalParameterField = new PropertyField(serializedObject.FindProperty("IsGlobalParameter"));
            var onSendField = new PropertyField(serializedObject.FindProperty("OnSend"));

            var button = new Button(() => parameterSender.SendValue())
            {
                text = "Send Parameter"
            };
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
            root1.Add(valueField);
            root1.Add(sendOnStartField);
            root.Add(Space(5f));
            root.Add(onSendField);
            root.Add(Space(5f));
            root.Add(button);

            var visualElements = new[]
            {
                sourceField, onSendField, behaviourStyleField, line
            };

            ControlField(visualElements);

            behaviourStyleField.RegisterValueChangeCallback(_ =>
                ControlField(visualElements));

            isGlobalParameterField.RegisterValueChangeCallback(_ =>
                ControlField(visualElements));

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
        }

        private void ControlField(VisualElement[] elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];
            var behaviourStyleField = elements[2];
            var line = elements[3];

            SetActiveField(behaviourStyleField, true);
            SetActiveField(line, true);

            if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
            {
                SetActiveField(sourceField, true);
                SetActiveField(onSendField, false);
            }
            else
            {
                SetActiveField(sourceField, false);
                SetActiveField(onSendField, true);
            }

            if (parameterSender.IsGlobalParameter)
            {
                SetActiveField(sourceField, false);
                SetActiveField(onSendField, false);
                SetActiveField(behaviourStyleField, false);
                SetActiveField(line, false);
            }
        }

        private void SetActiveField(VisualElement field, bool active)
        {
            field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement Space(float height)
        {
            var space = new VisualElement();
            space.style.height = height;
            // Debug : space.style.backgroundColor = new StyleColor(Color.red);
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
    }
}