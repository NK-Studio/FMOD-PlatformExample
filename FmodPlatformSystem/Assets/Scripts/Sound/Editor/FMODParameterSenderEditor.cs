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
        public Texture2D DarkIcon;
        public Texture2D WhiteIcon;

        private FMODParameterSender parameterSender;

        private void OnEnable()
        {
            InitIcon();
        }

        public override VisualElement CreateInspectorGUI()
        {
            parameterSender = (FMODParameterSender)target;

            var root = new VisualElement();

            // DeleteTarget
            var useBGMAPIField = new PropertyField(serializedObject.FindProperty("UseBGMAPI"))
            {
                label = "Use BGM API"
            };

            var sourceField = new PropertyField(serializedObject.FindProperty("Source"));
            var parameterFiled = new PropertyField(serializedObject.FindProperty("ParameterName"));
            var valueField = new PropertyField(serializedObject.FindProperty("Value"));
            var sendOnStart = new PropertyField(serializedObject.FindProperty("SendOnStart"));
            var button = new Button(() => parameterSender.SendValue())
            {
                text = "Send Parameter"
            };

            root.Add(useBGMAPIField); // DeleteTarget
            root.Add(sourceField);
            root.Add(parameterFiled);
            root.Add(valueField);
            root.Add(sendOnStart);
            root.Add(button);

            ControlField(sourceField); // DeleteTarget
            useBGMAPIField.RegisterValueChangeCallback(_ => ControlField(sourceField)); // DeleteTarget

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
        private void ControlField(VisualElement sourceField)
        {
            if (parameterSender.UseBGMAPI)
                SetActiveField(sourceField, false);
            else
                SetActiveField(sourceField, true);
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

        // DeleteTarget
        private void SetActiveField(VisualElement field, bool active)
        {
            field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}