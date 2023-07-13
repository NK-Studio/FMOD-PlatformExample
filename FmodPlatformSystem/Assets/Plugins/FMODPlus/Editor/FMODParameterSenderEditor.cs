#if UNITY_EDITOR
using System;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODPlus
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

            string boxGroupStyleSheetPath = AssetDatabase.GUIDToAssetPath("6a25e899d15eb994b85241dddfd90559");
            groupBoxStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupStyleSheetPath);

            string buttonStyleSheetPath = AssetDatabase.GUIDToAssetPath("db197c96211fc47319d2b84dcd02aacd");
            buttonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(buttonStyleSheetPath);
        }

        public override VisualElement CreateInspectorGUI()
        {
            parameterSender = (FMODParameterSender)target;

            VisualElement root = new();
            root.styleSheets.Add(groupBoxStyleSheet);
            root.styleSheets.Add(buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            PropertyField behaviourStyleField = new(serializedObject.FindProperty("BehaviourStyle"));
            PropertyField sourceField = new(serializedObject.FindProperty("Source"));

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            TextField parameterFiled = new();
            parameterFiled.label = "Parameter";
            parameterFiled.BindProperty(serializedObject.FindProperty("Parameter"));
            parameterFiled.AddToClassList("unity-base-field__aligned");

            PropertyField globalParameterFiled = new(serializedObject.FindProperty("Parameter"));

            PropertyField valueField = new(serializedObject.FindProperty("Value"));
            PropertyField sendOnStartField = new(serializedObject.FindProperty("SendOnStart"));
            PropertyField isGlobalParameterField = new(serializedObject.FindProperty("IsGlobalParameter"));

            var helpBox = new HelpBox();
            helpBox.text = "Audio Source가 연결되어 있지 않습니다.";
            helpBox.messageType = HelpBoxMessageType.Error;

            PropertyField onSendField = new(serializedObject.FindProperty("OnSend"));

            Button button = new(() => parameterSender.SendValue());
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

            SimpleBaseField simpleBaseField = new();
            root1.Add(simpleBaseField);

            root1.Add(valueField);
            root1.Add(helpBox);
            root1.Add(sendOnStartField);
            root.Add(Space(5f));
            root.Add(onSendField);
            root.Add(Space(5f));
            root.Add(button);

            VisualElement[] visualElements =
            {
                sourceField, onSendField, behaviourStyleField, line, parameterFiled, globalParameterFiled,
                simpleBaseField, valueField, helpBox
            };

            //Init
            _oldIsGlobalParameter = parameterSender.IsGlobalParameter;

            ControlField(visualElements);
            RefreshParameterSenderValue();

            // root.RegisterCallbackAll(() =>
            // {
            //     Debug.Log("go");
            //     ControlField(visualElements);
            // });

            behaviourStyleField.RegisterValueChangeCallback(_ =>
                ControlField(visualElements));

            // isGlobalParameterField.RegisterValueChangeCallback(evt =>
            // {
            //     if (_oldIsGlobalParameter != evt.changedProperty.boolValue)
            //     {
            //         parameterSender.Parameter = string.Empty;
            //         _oldIsGlobalParameter = evt.changedProperty.boolValue;
            //     }
            //
            //     ControlField(visualElements);
            // });

            // root.Add(new IMGUIContainer(RefreshParameterSenderValue));

            // if (!EditorApplication.isPlaying)
            // {
            //     button.tooltip = Application.systemLanguage == SystemLanguage.Korean
            //         ? "에디터 모드에서는 사용하지 못합니다."
            //         : "Can't use in Editor Mode.";
            //     button.SetEnabled(false);
            // }
            // else
            // {
            //     button.tooltip = "Send Parameter.";
            //     button.SetEnabled(true);
            // }

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

        private void OnValidate()
        {
            Debug.Log("go");
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
            var helpBox = elements[8] as HelpBox;

            foreach (VisualElement element in elements)
                SetActiveField(element, false);

            SetActiveField(sourceField, true);

            if (!parameterSender.Source)
            {
                SetActiveField(helpBox, true);
                return;
            }

            bool isConnectEventRef = !string.IsNullOrWhiteSpace(parameterSender.Source.Clip.Path);

            if (!isConnectEventRef)
            {
                helpBox.text = "FMOD Audio Source에 Clip이 연결되어 있지 않습니다.";
                SetActiveField(helpBox, true);
            }

            //  if (parameterSender.BehaviourStyle == FMODParameterSender.AudioBehaviourStyle.Base)
            SetActiveField(sourceField, true);

            //  SetActiveField(onSendField, true);
            // else

            // if (parameterSender.IsGlobalParameter)
            // {
            //     SetActiveField(sourceField, true);
            //     SetActiveField(onSendField, true);
            //     SetActiveField(behaviourStyleField, true);
            //     SetActiveField(line, true);
            //     SetActiveField(parameterFiled, true);
            //     SetActiveField(valueField, true);
            // }
            // else
            // {
            //     SetActiveField(globalParameterField, true);
            //     SetActiveField(simpleBaseField, true);
            // }
        }

        private void SetActiveField(VisualElement field, bool active)
        {
            field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement Space(float height)
        {
            var space = new VisualElement();
            space.style.height = height;
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