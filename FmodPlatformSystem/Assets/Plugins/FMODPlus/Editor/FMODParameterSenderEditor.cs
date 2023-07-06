#if UNITY_EDITOR
using System;
using NKStudio.UIElements;
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
        [SerializeField] private EditorParamRef editorParamRef;
        
        private bool _oldIsGlobalParameter;
        private string _currentPath;

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

            if (!groupBoxStyleSheet)
                groupBoxStyleSheet =
                    AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/FMODPlus/Editor/BoxGroupStyle.uss");
            
            if (!buttonStyleSheet)
                buttonStyleSheet =
                    AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/FMODPlus/Editor/ButtonStyle.uss");
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

            TextField parameterFiled = new TextField();
            parameterFiled.label = "Parameter";
            parameterFiled.BindProperty(serializedObject.FindProperty("Parameter"));
            parameterFiled.AddToClassList("unity-base-field__aligned");

            var globalParameterFiled = new PropertyField(serializedObject.FindProperty("Parameter"));

            var valueField = new PropertyField(serializedObject.FindProperty("Value"));
            var sendOnStartField = new PropertyField(serializedObject.FindProperty("SendOnStart"));
            var isGlobalParameterField = new PropertyField(serializedObject.FindProperty("IsGlobalParameter"));
            var onSendField = new PropertyField(serializedObject.FindProperty("OnSend"));

            var button = new Button(() => parameterSender.SendValue());
            button.text = "Send Parameter";
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
            root1.Add(globalParameterFiled);

            var simpleBaseField = new SimpleBaseField();
            root1.Add(simpleBaseField);

            root1.Add(valueField);
            root1.Add(sendOnStartField);
            root.Add(Space(5f));
            root.Add(onSendField);
            root.Add(Space(5f));
            root.Add(button);

            var visualElements = new[]
            {
                sourceField, onSendField, behaviourStyleField, line, parameterFiled, globalParameterFiled,
                simpleBaseField, valueField
            };

            //Init
            _oldIsGlobalParameter = parameterSender.IsGlobalParameter;
            
            ControlField(visualElements);
            RefreshParameterSenderValue();

            behaviourStyleField.RegisterValueChangeCallback(_ =>
                ControlField(visualElements));

            isGlobalParameterField.RegisterValueChangeCallback(evt =>
            {
                if (_oldIsGlobalParameter != evt.changedProperty.boolValue)
                {
                    parameterSender.Parameter = string.Empty;
                    _oldIsGlobalParameter = evt.changedProperty.boolValue;
                }
                
                ControlField(visualElements);
            });

            root.Add(new IMGUIContainer(RefreshParameterSenderValue));

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

            void RefreshParameterSenderValue()
            {
                if (!parameterSender.IsGlobalParameter)
                    return;
                
                if (parameterSender.Parameter != _currentPath)
                {
                    _currentPath = parameterSender.Parameter;

                    if (string.IsNullOrEmpty(parameterSender.Parameter))
                    {
                        editorParamRef = null;
                    }
                    else
                    {
                        editorParamRef = EventManager.ParamFromPath(parameterSender.Parameter);
                        parameterSender.Value =
                            Mathf.Clamp(parameterSender.Value, editorParamRef.Min, editorParamRef.Max);
                    }

                    RefreshGlobalParameterField(simpleBaseField);
                }
            }
        }

        private void ControlField(VisualElement[] elements)
        {
            var sourceField = elements[0];
            var onSendField = elements[1];
            var behaviourStyleField = elements[2];
            var line = elements[3];
            var parameterFiled = elements[4];
            var globalParameterField = elements[5];
            var simpleBaseField = elements[6];
            var valueField = elements[7];

            SetActiveField(sourceField, true);
            SetActiveField(onSendField, true);
            SetActiveField(behaviourStyleField, true);
            SetActiveField(line, true);
            SetActiveField(parameterFiled, true);
            SetActiveField(globalParameterField, true);
            SetActiveField(simpleBaseField, true);
            SetActiveField(valueField, true);

            if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
            {
                SetActiveField(onSendField, false);
            }
            else
            {
                SetActiveField(sourceField, false);
            }

            if (parameterSender.IsGlobalParameter)
            {
                SetActiveField(sourceField, false);
                SetActiveField(onSendField, false);
                SetActiveField(behaviourStyleField, false);
                SetActiveField(line, false);
                SetActiveField(parameterFiled, false);
                SetActiveField(valueField, false);
            }
            else
            {
                SetActiveField(globalParameterField, false);
                SetActiveField(simpleBaseField, false);
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

        private void RefreshGlobalParameterField(SimpleBaseField simpleBaseField)
        {
            try
            {
                int childElement = simpleBaseField.contentContainer.childCount;


                if (childElement == 1)
                    simpleBaseField.contentContainer.RemoveAt(0);

                if (editorParamRef != null)
                {
                    simpleBaseField.Label = "Override Value";

                    var content = new IMGUIContainer();
                    content.onGUIHandler = () =>
                    {
                        parameterSender.Value =
                            EditorUtils.DrawParameterValueLayout(parameterSender.Value, editorParamRef);
                    };

                    simpleBaseField.contentContainer.Add(content);
                }
                else
                {
                    simpleBaseField.Label = string.Empty;

                    Texture2D warningIcon = EditorUtils.LoadImage("NotFound.png");

                    var icon = new VisualElement();
                    icon.style.backgroundImage = new StyleBackground(warningIcon);
                    icon.style.width = warningIcon.width;
                    icon.style.height = warningIcon.height;

                    var textField = new Label();
                    textField.text = "Parameter Not Found";

                    var innerContainer = new VisualElement
                    {
                        name = "innerContainer",
                        style =
                        {
                            flexDirection = FlexDirection.Row
                        }
                    };

                    innerContainer.Add(icon);
                    innerContainer.Add(textField);
                    simpleBaseField.contentContainer.Add(innerContainer);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
#endif