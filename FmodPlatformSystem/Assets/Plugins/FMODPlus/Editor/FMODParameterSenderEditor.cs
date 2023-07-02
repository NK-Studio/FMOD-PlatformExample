using Cinemachine.Editor;
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
            var onSendField = new PropertyField(serializedObject.FindProperty("OnSend"));

            var button = new Button(() => parameterSender.SendValue())
            {
                text = "Send Parameter"
            };
            button.AddToClassList("ButtonStyle");

            root.Add(root0);
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
                sourceField, onSendField
            };

            ControlField(visualElements);

            behaviourStyleField.RegisterValueChangeCallback(_ =>
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

        // DeleteTarget
        private void ControlField(VisualElement[] elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];

            if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Play)
            {
                SetActiveField(sourceField, true);
                SetActiveField(onSendField, false);
            }
            else
            {
                SetActiveField(sourceField, false);
                SetActiveField(onSendField, true);
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
    }
}