#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using FMODUnity;
using NKStudio;
using NKStudio.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(EventCommandSender))]
    public class EventCommandSenderEditor : Editor
    {
        private EventCommandSender _commandSender;
        private ParameterValueView _parameterValueView;

        private EditorEventRef _editorEvent;

        private StyleSheet _boxGroupStyle;
        private StyleSheet _buttonStyleSheet;

        private SerializedProperty _params;
        private SerializedProperty _parameter;
        private SerializedProperty _value;
        private SerializedProperty _behaviourStyle;
        private SerializedProperty _audioSource;
        private SerializedProperty _clip;
        private SerializedProperty _clipPath;
        private SerializedProperty _useGlobalKeyList;
        private SerializedProperty _clipStyle;
        private SerializedProperty _key;
        private SerializedProperty _fade;
        private SerializedProperty _sendOnStart;
        private SerializedProperty _onPlaySend;
        private SerializedProperty _onStopSend;
        private SerializedProperty _onParameterSend;
        private SerializedProperty _localKeyList;
        private SerializedProperty _audioStyle;
        private VisualElement _root;

        /// <summary>
        /// Structure that stores Property Name
        /// </summary>
        private struct PropNames
        {
            public const string Clip = "Clip";
            public const string Clips = "Clips";
            public const string List = "list";
            public const string Key = "Key";
            public const string Value = "Value";
            public const string Path = "Path";
            public const string Params = "Params";
            public const string Name = "Name";
            public const string KeyList = "keyList";
            public const string UseGlobalKeyList = "useGlobalKeyList";
            public const string Parameter = "Parameter";
            public const string Source = "Source";
            public const string BehaviourStyle = "BehaviourStyle";
            public const string Fade = "Fade";
            public const string SendOnStart = "SendOnStart";
            public const string OnPlaySend = "OnPlaySend";
            public const string OnStopSend = "OnStopSend";
            public const string OnParameterSend = "OnParameterSend";
            public const string AudioStyle = "AudioStyle";
            public const string ClipStyle = "ClipStyle";
        }

        private string _oldKey;
        private string _oldPath;
        private string _oldParameterPath;
        private string _oldTargetPath;
        private bool _oldUseGlobalKeyList;
        private bool _oldFade;
        private ClipStyle _oldClipStyle;
        private AudioType _oldAudioStyle;
        private LocalKeyList _oldLocalKeyList;
        private FMODAudioSource _oldAudioSource;
        private CommandBehaviourStyle _oldCommandBehaviourStyle;

        private void OnEnable()
        {
            _parameterValueView = new ParameterValueView(serializedObject);

            // Event Command Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("684e21c44f6bd46aab39bb29fdda6b69");
            MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            NKEditorUtility.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupSheet = AssetDatabase.GUIDToAssetPath("5600a59cbafd24acf808fa415167310e");
            _boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupSheet);

            string buttonStyleSheetPath = AssetDatabase.GUIDToAssetPath("db197c96211fc47319d2b84dcd02aacd");
            _buttonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(buttonStyleSheetPath);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _commandSender = (EventCommandSender)target;
            FindProperty();
            InitializeRoot();
            return _root;
        }

        /// <summary>
        /// Find the property.
        /// </summary>
        private void FindProperty()
        {
            _params = serializedObject.FindProperty(PropNames.Params);
            _parameter = serializedObject.FindProperty(PropNames.Parameter);
            _value = serializedObject.FindProperty(PropNames.Value);
            _audioSource = serializedObject.FindProperty(PropNames.Source);
            _behaviourStyle = serializedObject.FindProperty(PropNames.BehaviourStyle);
            _clip = serializedObject.FindProperty(PropNames.Clip);
            _clipPath = _clip.FindPropertyRelative(PropNames.Path);
            _useGlobalKeyList = serializedObject.FindProperty(PropNames.UseGlobalKeyList);
            _clipStyle = serializedObject.FindProperty(PropNames.ClipStyle);
            _key = serializedObject.FindProperty(PropNames.Key);
            _fade = serializedObject.FindProperty(PropNames.Fade);
            _sendOnStart = serializedObject.FindProperty(PropNames.SendOnStart);
            _onPlaySend = serializedObject.FindProperty(PropNames.OnPlaySend);
            _onStopSend = serializedObject.FindProperty(PropNames.OnStopSend);
            _onParameterSend = serializedObject.FindProperty(PropNames.OnParameterSend);
            _localKeyList = serializedObject.FindProperty(PropNames.KeyList);
            _audioStyle = serializedObject.FindProperty(PropNames.AudioStyle);
        }

        /// <summary>
        /// Initialize root.
        /// </summary>
        private void InitializeRoot()
        {
            _root = new();
            _root.styleSheets.Add(_boxGroupStyle);
            _root.styleSheets.Add(_buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            VisualElement root2 = new();

            PropertyField behaviourStyleField = new();
            behaviourStyleField.BindProperty(_behaviourStyle);

            ObjectField audioSourceField = new();
            audioSourceField.label = "Audio Source";
            audioSourceField.objectType = typeof(FMODAudioSource);
            audioSourceField.BindProperty(_audioSource);
            audioSourceField.AddToClassList("unity-base-field__aligned");

            PropertyField clipField = new();
            clipField.BindProperty(_clip);
            clipField.label = "Event";

            PropertyField useGlobalKeyListField = new();
            useGlobalKeyListField.BindProperty(_useGlobalKeyList);

            PropertyField clipStyleField = new();
            clipStyleField.BindProperty(_clipStyle);

            PropertyField keyField = new();
            keyField.BindProperty(_key);
            keyField.label = "Event Key";

            PropertyField globalParameterFiled = new();
            globalParameterFiled.BindProperty(_parameter);

            PropertyField valueField = new();
            valueField.BindProperty(_value);

            PropertyField fadeField = new();
            fadeField.BindProperty(_fade);

            PropertyField sendOnStart = new();
            sendOnStart.BindProperty(_sendOnStart);

            PropertyField onPlaySend = new();
            onPlaySend.BindProperty(_onPlaySend);

            PropertyField onStopSend = new();
            onStopSend.BindProperty(_onStopSend);

            PropertyField onParameterSend = new();
            onParameterSend.BindProperty(_onParameterSend);

            string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
                ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
                : "Fade function requires AHDSR module to work.";

            HelpBox fadeHelpBox = new();
            fadeHelpBox.messageType = HelpBoxMessageType.Info;
            fadeHelpBox.text = appSystemLanguage;

            HelpBox helpBox = new();
            helpBox.ElementAt(0).style.flexGrow = 1;
            helpBox.messageType = HelpBoxMessageType.Error;
            helpBox.style.marginTop = 6;
            helpBox.style.marginBottom = 6;

            ObjectField localKeyListField = new();
            localKeyListField.label = "Key List";
            localKeyListField.objectType = typeof(LocalKeyList);
            localKeyListField.BindProperty(_localKeyList);
            localKeyListField.AddToClassList("unity-base-field__aligned");

            Color lineColor = Color.black;
            lineColor.a = 0.4f;
            VisualElement line = NKEditorUtility.Line(lineColor, 1.5f, 4f, 3f);

            PropertyField audioStyleField = new();
            audioStyleField.BindProperty(_audioStyle);

            VisualElement parameterArea = new();
            parameterArea.style.marginLeft = 15;
            parameterArea.name = "ParameterArea";
            parameterArea.SetActive(false);

            Button parameterSendButton = new();
            parameterSendButton.clicked += () => _commandSender.SendCommand();
            parameterSendButton.text = "Send Parameter";
            parameterSendButton.AddToClassList("ButtonStyle");

            (VisualElement initializeField, Foldout titleToggleLayout, DropdownField addButton) =
                _parameterValueView.InitParameterView(parameterArea, _commandSender);

            VisualElement notFoundField = FMODEditorUtility.CreateNotFoundField();
            VisualElement eventSpace = NKEditorUtility.Space(5f);

            _root.Add(root0);
            _root.Add(NKEditorUtility.Space(5));
            root0.Add(behaviourStyleField);
            root0.Add(audioSourceField);

            _root.Add(root1);
            root1.Add(clipStyleField);
            root1.Add(line);
            root1.Add(useGlobalKeyListField);
            root1.Add(localKeyListField);
            root1.Add(audioStyleField);
            root1.Add(clipField);
            root1.Add(keyField);
            root1.Add(initializeField);

            titleToggleLayout.Close();
            _parameterValueView.DrawValues(true);

            root1.Add(globalParameterFiled);
            root1.Add(valueField);
            root1.Add(notFoundField);
            root1.Add(helpBox);
            root1.Add(parameterArea);
            root1.Add(fadeField);
            root1.Add(sendOnStart);
            root1.Add(fadeHelpBox);

            _root.Add(root2);
            root2.Add(eventSpace);
            root2.Add(onPlaySend);
            root2.Add(onStopSend);
            root2.Add(onParameterSend);

            _root.Add(parameterSendButton);

            //Init
            VisualElement[] visualElements =
            {
                audioSourceField /*0*/, clipStyleField /*1*/, clipField /*2*/, fadeField /*3*/, onPlaySend /*4*/,
                onStopSend /*5*/, eventSpace /*6*/,
                fadeHelpBox /*7*/, parameterArea /*8*/, useGlobalKeyListField /*9*/, localKeyListField /*10*/,
                line /*11*/, sendOnStart /*12*/,
                audioStyleField /*13*/, titleToggleLayout /*14*/, behaviourStyleField /*15*/, addButton /*16*/,
                notFoundField /*17*/, keyField /*18*/,
                initializeField /*19*/, helpBox /*20*/, parameterSendButton /*21*/, onParameterSend /*22*/,
                globalParameterFiled /*23*/,
                valueField /*24*/
            };

            // Init
            InitControlField(visualElements);
            RuntimeActive(parameterSendButton);
        }

        /// <summary>
        /// Handles Button state (Button Only)
        /// </summary>
        /// <param name="element">Button Element</param>
        private void RuntimeActive(VisualElement element)
        {
            if (!EditorApplication.isPlaying)
            {
                element.tooltip = Application.systemLanguage == SystemLanguage.Korean
                    ? "에디터 모드에서는 사용하지 못합니다."
                    : "Can't use in Editor Mode.";
                element.SetEnabled(false);
            }
            else
            {
                element.tooltip = "Send Parameter.";
                element.SetEnabled(true);
            }
        }

        /// <summary>
        /// Check whether the AudioSource Path has changed.
        /// </summary>
        /// <param name="callback">Callback requested when value changes</param>
        private void RegisterAudioSourcePathValueChange(Action<string> callback)
        {
            _root.schedule.Execute(() =>
            {
                var audioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
                var tmp = string.Empty;

                if (audioSource)
                    tmp = audioSource.clip.Path;

                if (_oldTargetPath != tmp)
                {
                    callback.Invoke(tmp);
                    _oldTargetPath = tmp;
                }
            }).Every(5);
        }

        /// <summary>
        /// Register a callback.
        /// </summary>
        /// <param name="elements">UI Elements</param>
        private void InitControlField(IReadOnlyList<VisualElement> elements)
        {
            var audioSourceField = (ObjectField)elements[0];
            var clipStyleField = (PropertyField)elements[1];
            var clipField = (PropertyField)elements[2];
            var fadeField = (PropertyField)elements[3];
            var parameterArea = elements[8];
            var useGlobalKeyListField = (PropertyField)elements[9];
            var localKeyListField = (ObjectField)elements[10];
            var audioStyleField = (PropertyField)elements[13];
            var titleToggleLayout = (Foldout)elements[14];
            var behaviourStyleField = (PropertyField)elements[15];
            var addButton = (DropdownField)elements[16];
            var keyField = (PropertyField)elements[18];
            var globalParameterField = (PropertyField)elements[23];

            // Initial control field processing
            ControlField(elements);

            // If there is more than one parameter, open the parameter area.
            if (_params.arraySize > 0)
                _parameterValueView.SetOpenParameterArea(true);

            // A callback is called when the audio source value changes.
            _oldTargetPath = string.Empty;
            RegisterAudioSourcePathValueChange(evt =>
            {
                if (string.IsNullOrEmpty(evt))
                    _parameterValueView.Dispose(true);

                ControlField(elements);
            });

            // A callback is called when the title toggle value changes.
            titleToggleLayout.RegisterValueChangedCallback(_ => ControlField(elements));

            // A callback is called when the clip value changes.
            clipField.RegisterValueChangeCallback(_clip, _oldPath, _ =>
            {
                _parameterValueView.Dispose();
                ControlField(elements);
            });

            // A callback is called when the key value changes.
            _oldKey = _key.stringValue;
            keyField.RegisterValueChangeCallback(evt =>
            {
                if (_oldKey != evt.changedProperty.stringValue)
                {
                    ControlField(elements);
                    _oldKey = evt.changedProperty.stringValue;
                }
            });

            // A callback is called when the audio source value changes.
            _oldAudioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
            audioSourceField.RegisterValueChangedCallback(evt =>
            {
                if (_oldAudioSource != (FMODAudioSource)evt.newValue)
                {
                    ControlField(elements);
                    _oldAudioSource = (FMODAudioSource)evt.newValue;
                }
            });

            // A callback is called when the use global key list value changes.
            _oldUseGlobalKeyList = _useGlobalKeyList.boolValue;
            useGlobalKeyListField.RegisterValueChangeCallback(evt =>
            {
                if (_oldUseGlobalKeyList != evt.changedProperty.boolValue)
                {
                    ControlField(elements);
                    _oldUseGlobalKeyList = evt.changedProperty.boolValue;
                    _parameterValueView.Dispose(true);
                }
            });

            // A callback is called when the clip style value changes.
            _oldClipStyle = (ClipStyle)_clipStyle.enumValueIndex;
            clipStyleField.RegisterValueChangeCallback(evt =>
            {
                if (_oldClipStyle != (ClipStyle)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    parameterArea.Clear();
                    addButton.SetEnabled(true);

                    ControlField(elements);
                    _oldClipStyle = (ClipStyle)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the audio style value changes.
            _oldAudioStyle = (AudioType)_audioStyle.enumValueIndex;
            audioStyleField.RegisterValueChangeCallback(evt =>
            {
                if (_oldAudioStyle != (AudioType)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    parameterArea.Clear();
                    addButton.SetEnabled(true);

                    ControlField(elements);
                    _oldAudioStyle = (AudioType)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the behaviour style value changes.
            _oldCommandBehaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;
            behaviourStyleField.RegisterValueChangeCallback(evt =>
            {
                var behaviourStyle = (CommandBehaviourStyle)evt.changedProperty.enumValueIndex;
                if (_oldCommandBehaviourStyle != behaviourStyle)
                {
                    switch (behaviourStyle)
                    {
                        case CommandBehaviourStyle.Play:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.PlayOnAPI:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.Stop:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.StopOnAPI:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.Parameter:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.ParameterOnAPI:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.GlobalParameter:
                            _parameterValueView.Dispose(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ControlField(elements);
                    _oldCommandBehaviourStyle = behaviourStyle;
                }
            });

            // A callback is called when the fade value changes.
            _oldFade = _fade.boolValue;
            fadeField.RegisterValueChangeCallback(evt =>
            {
                if (_oldFade != evt.changedProperty.boolValue)
                {
                    ControlField(elements);
                    _oldFade = evt.changedProperty.boolValue;
                }
            });

            // A callback is requested when the local key list changes.
            _oldLocalKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
            localKeyListField.RegisterValueChangedCallback(evt =>
            {
                if (_oldLocalKeyList != (LocalKeyList)evt.newValue)
                {
                    ControlField(elements);
                    _oldLocalKeyList = (LocalKeyList)evt.newValue;
                }
            });

            // A callback is called when the global parameter value changes.
            globalParameterField.RegisterValueChangeCallback(_parameter, _oldParameterPath,
                _ =>
                {
                    _value.floatValue = 0f;
                    serializedObject.ApplyModifiedProperties();
                    ControlField(elements);
                });
        }

        /// <summary>
        /// Control fields.
        /// </summary>
        /// <param name="elements">UI Elements</param>
        private void ControlField(IReadOnlyList<VisualElement> elements)
        {
            var audioSourceField = elements[0];
            var clipStyleField = elements[1];
            var clipField = (PropertyField)elements[2];
            var fadeField = elements[3];
            var onPlaySend = elements[4];
            var onStopSend = elements[5];
            var eventSpace = elements[6];
            var fadeHelpBox = (HelpBox)elements[7];
            var parameterArea = elements[8];
            var useGlobalKeyListField = elements[9];
            var localKeyListField = elements[10];
            var line = elements[11];
            var sendOnStart = elements[12];
            var audioStyleField = elements[13];
            var titleToggleLayout = (Foldout)elements[14];
            var behaviourStyleField = elements[15];
            var addButton = (DropdownField)elements[16];
            var notFoundField = elements[17];
            var keyField = elements[18];
            var initializeField = elements[19];
            var helpBox = (HelpBox)elements[20];
            var sendButton = elements[21];
            var onParameterSend = elements[22];
            var parameterField = elements[23];

            // Disable everything first
            foreach (VisualElement visualElement in elements)
                visualElement.SetActive(false);

            var behaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;
            ClipStyle clipStyle;
            switch (behaviourStyle)
            {
                case CommandBehaviourStyle.Play:
                    clipField.label = "Event";
                    titleToggleLayout.text = "Override Init Parameter";
                    parameterArea.style.marginLeft = 15;
                    behaviourStyleField.SetActive(true);
                    audioSourceField.SetActive(true);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        clipStyleField.SetActive(true);
                        clipStyle = (ClipStyle)_clipStyle.enumValueIndex;
                        switch (clipStyle)
                        {
                            case ClipStyle.EventReference:
                                clipField.SetActive(true);
                                HandleEventRef(_clipPath.stringValue);
                                break;
                            case ClipStyle.Key:
                                useGlobalKeyListField.SetActive(true);
                                HandleKey(null, () => notFoundField.SetActive(true));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "FMOD Audio Source가 연결되어 있지 않습니다.",
                            "FMOD Audio Source is not connected.");

                        _parameterValueView.Dispose();
                    }

                    break;
                case CommandBehaviourStyle.PlayOnAPI:
                    clipField.label = "Event";
                    titleToggleLayout.text = "Override Init Parameter";
                    parameterArea.style.marginLeft = 15;
                    behaviourStyleField.SetActive(true);
                    clipStyleField.SetActive(true);
                    onPlaySend.SetActive(true);
                    eventSpace.SetActive(true);

                    clipStyle = (ClipStyle)_clipStyle.enumValueIndex;

                    switch (clipStyle)
                    {
                        case ClipStyle.EventReference:
                            clipField.SetActive(true);
                            HandleEventRef(_clipPath.stringValue);
                            break;
                        case ClipStyle.Key:
                            useGlobalKeyListField.SetActive(true);
                            HandleKey(null, () => notFoundField.SetActive(true));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case CommandBehaviourStyle.Stop:

                    behaviourStyleField.SetActive(true);
                    audioSourceField.SetActive(true);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        fadeField.SetActive(true);
                        sendOnStart.SetActive(true);
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "FMOD Audio Source가 연결되어 있지 않습니다.",
                            "FMOD Audio Source is not connected.");

                        _parameterValueView.Dispose();
                    }

                    if (_fade.boolValue)
                        fadeHelpBox.SetActive(true);

                    break;
                case CommandBehaviourStyle.StopOnAPI:

                    behaviourStyleField.SetActive(true);
                    fadeField.SetActive(true);
                    onStopSend.SetActive(true);
                    eventSpace.SetActive(true);
                    sendOnStart.SetActive(true);

                    if (_fade.boolValue)
                        fadeHelpBox.SetActive(true);

                    break;
                case CommandBehaviourStyle.Parameter:
                    titleToggleLayout.text = "Override Parameter";
                    behaviourStyleField.SetActive(true);
                    audioSourceField.SetActive(true);
                    sendButton.SetActive(true);

                    var targetAudioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
                    if (targetAudioSource != null)
                    {
                        // 클립 입력을 요구하지 않음(clipField.SetActive(true);)
                        HandleEventRef(targetAudioSource.clip.Path);
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "FMOD Audio Source가 연결되어 있지 않습니다.",
                            "FMOD Audio Source is not connected.");

                        _parameterValueView.Dispose();
                    }
                    break;
                case CommandBehaviourStyle.ParameterOnAPI:
                    clipField.label = "Parameter Load";
                    titleToggleLayout.text = "Override Parameter";
                    eventSpace.SetActive(true);
                    clipStyleField.SetActive(true);
                    onParameterSend.SetActive(true);
                    behaviourStyleField.SetActive(true);

                    clipStyle = (ClipStyle)_clipStyle.enumValueIndex;

                    switch (clipStyle)
                    {
                        case ClipStyle.EventReference:
                            clipField.SetActive(true);
                            HandleEventRef(_clipPath.stringValue);
                            break;
                        case ClipStyle.Key:
                            useGlobalKeyListField.SetActive(true);
                            HandleKey(null, () => notFoundField.SetActive(true));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case CommandBehaviourStyle.GlobalParameter:
                    behaviourStyleField.SetActive(true);
                    parameterField.SetActive(true);

                    EditorParamRef editorParamRef = EventManager.ParamFromPath(_parameter.stringValue);
                    if (editorParamRef != null)
                    {
                        _parameterValueView.DrawGlobalValues(true);
                        parameterArea.SetActive(true);
                        sendOnStart.SetActive(true);
                        parameterArea.style.marginLeft = 0;
                    }
                    else
                    {
                        HandleMessage(helpBox, "연결된 이벤트 주소가 유효하지 않습니다.", "The connected event address is invalid.");
                        notFoundField.SetActive(true);
                    }

                    sendButton.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return;

            void HandleEvent(EditorEventRef existEvent)
            {
                _parameterValueView.RefreshPropertyRecords(existEvent);
                _parameterValueView.DrawValues();
                _parameterValueView.CalculateEnableAddButton();

                addButton.SetActive(true);
                titleToggleLayout.SetActive(true);
                initializeField.SetActive(true);
                sendOnStart.SetActive(true);

                var toggleOnOff = titleToggleLayout.value;
                parameterArea.SetActive(toggleOnOff);
            }

            void HandleEventRef(string path, Action successAction = null, Action failAction = null)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    EditorEventRef existEvent = EventManager.EventFromPath(path);

                    if (existEvent != null)
                    {
                        HandleEvent(existEvent);
                        successAction?.Invoke();
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "연결된 이벤트 주소가 유효하지 않습니다.",
                            "The connected event address is invalid.");

                        failAction?.Invoke();
                        _parameterValueView.Dispose();
                    }
                }
                else
                {
                    HandleMessage(helpBox,
                        "Event가 비어있습니다.",
                        "Event is empty.");

                    failAction?.Invoke();
                    _parameterValueView.Dispose();
                }
            }

            void HandleKey(Action successAction = null, Action failAction = null)
            {
                // When using a global key list
                if (_useGlobalKeyList.boolValue)
                {
                    line.SetActive(true);
                    keyField.SetActive(true);
                    audioStyleField.SetActive(true);

                    bool keyExists = !string.IsNullOrWhiteSpace(_key.stringValue);
                    if (keyExists)
                    {
                        EditorEventRef existEvent;

                        switch (_commandSender.AudioStyle)
                        {
                            case AudioType.AMB:
                                existEvent = AMBKeyList.Instance.GetEventRef(_commandSender.Key);
                                break;
                            case AudioType.BGM:
                                existEvent = BGMKeyList.Instance.GetEventRef(_commandSender.Key);
                                break;
                            case AudioType.SFX:
                                existEvent = SFXKeyList.Instance.GetEventRef(_commandSender.Key);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (existEvent != null)
                        {
                            successAction?.Invoke();
                            HandleEvent(existEvent);
                        }
                        else
                        {
                            HandleMessage(helpBox,
                                "연결된 이벤트 주소가 유효하지 않습니다.",
                                "The connected event address is invalid.");

                            failAction?.Invoke();
                            _parameterValueView.Dispose();
                        }
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "Key가 비어있습니다.",
                            "Key is empty.");

                        failAction?.Invoke();
                        _parameterValueView.Dispose();
                    }
                }
                // When using a local key list
                else
                {
                    line.SetActive(true);
                    localKeyListField.SetActive(true);
                    if (_localKeyList.objectReferenceValue != null)
                    {
                        keyField.SetActive(true);

                        bool keyExists = !string.IsNullOrWhiteSpace(_key.stringValue);
                        if (keyExists)
                        {
                            EditorEventRef existEvent = null;

                            LocalKeyList targetKeyList =
                                (LocalKeyList)_localKeyList.objectReferenceValue;
                            SerializedObject targetLocalKeyList = new(targetKeyList);
                            SerializedProperty lists = targetLocalKeyList
                                .FindProperty(PropNames.Clips)
                                .FindPropertyRelative(PropNames.List);

                            foreach (SerializedProperty list in lists)
                            {
                                string targetKey = list.FindPropertyRelative(PropNames.Key)
                                    .stringValue;
                                string targetPath = list.FindPropertyRelative(PropNames.Value)
                                    .FindPropertyRelative(PropNames.Path)
                                    .stringValue;

                                if (_key.stringValue == targetKey)
                                {
                                    existEvent = EventManager.EventFromPath(targetPath);
                                    break;
                                }
                            }

                            if (existEvent != null)
                            {
                                parameterArea.style.marginLeft = 0;
                                HandleEvent(existEvent);
                                successAction?.Invoke();
                            }
                            else
                            {
                                HandleMessage(helpBox,
                                    "연결된 이벤트 주소가 유효하지 않습니다.",
                                    "The connected event address is invalid.");

                                failAction?.Invoke();
                                _parameterValueView.Dispose();
                            }
                        }
                        else
                        {
                            HandleMessage(helpBox,
                                "Key가 비어있습니다.",
                                "Key is empty.");

                            failAction?.Invoke();
                            _parameterValueView.Dispose();
                        }
                    }
                    else
                    {
                        HandleMessage(helpBox,
                            "Key List가 연결되어있지 않습니다.",
                            "Key List is not connected.");

                        _parameterValueView.Dispose();
                        failAction?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Displays a message in the HelpBox.
        /// </summary>
        /// <param name="helpBox">UI Element</param>
        /// <param name="koreanMessage">Korean message</param>
        /// <param name="englishMessage">ENGLISH MESSAGE</param>
        private void HandleMessage(HelpBox helpBox, string koreanMessage, string englishMessage)
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean ? koreanMessage : englishMessage;
            helpBox.text = msg;
            helpBox.SetActive(true);
        }

        private class ParameterValueView
        {
            // This holds one SerializedObject for each object in the current selection.
            private readonly SerializedObject _serializedObject;

            // Mapping from EditorParamRef to initial parameter value properties for all properties in the current selection.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // Any parameters that are currently in the event but are missing from some objects in the current selection can be put into the "Add" menu.
            private readonly List<EditorParamRef> _missingParameters = new();

            private Foldout _titleText;
            private DropdownField _addButton;

            private VisualElement _parameterArea;
            private VisualElement _parameterLayout;

            private EventCommandSender _commandSender;
            private EditorParamRef _editorParamRef;

            public ParameterValueView(SerializedObject serializedObject)
            {
                _serializedObject = serializedObject;
            }

            private class PropertyRecord
            {
                public string Name => ParamRef.Name;

                public EditorParamRef ParamRef;
                public List<SerializedProperty> ValueProperties;
            }

            /// <summary>
            /// Initialize the view.
            /// </summary>
            /// <param name="clearClipAndKey">클립과 키를 함께 초기화합니다.</param>
            public void Dispose(bool clearClipAndKey = false)
            {
                _commandSender.Params = Array.Empty<ParamRef>();
                _propertyRecords.Clear();
                _missingParameters.Clear();
                _editorParamRef = null;
                _titleText.value = false;

                if (clearClipAndKey)
                {
                    _commandSender.Clip = new EventReference();
                    _commandSender.Key = string.Empty;
                    _commandSender.Parameter = string.Empty;
                    _commandSender.Value = 0f;
                }
            }

            /// <summary>
            ///  Draw a parameter view.
            /// </summary>
            /// <param name="parameterArea"></param>
            /// <param name="commandSender"></param>
            /// <returns></returns>
            public Tuple<VisualElement, Foldout, DropdownField> InitParameterView(
                VisualElement parameterArea,
                EventCommandSender commandSender)
            {
                _parameterArea = parameterArea;
                _commandSender = commandSender;

                VisualElement baseFieldLayout = new();
                baseFieldLayout.style.marginLeft = 15;

                VisualElement labelArea = new();
                VisualElement inputArea = new();

                _titleText = new Foldout();
                _titleText.text = "Override Init Parameter";

                _addButton = new DropdownField();
                _addButton.value = "Add";
                _addButton.style.flexGrow = 1;
                _addButton.style.marginLeft = 0;

                baseFieldLayout.Add(labelArea);
                baseFieldLayout.Add(inputArea);

                labelArea.Add(_titleText);
                inputArea.Add(_addButton);
                _addButton.RegisterCallback<ClickEvent>(_ => DrawAddButton(_addButton.worldBound));

                NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

                return new Tuple<VisualElement, Foldout, DropdownField>(baseFieldLayout, _titleText, _addButton);
            }

            /// <summary>
            /// Draw the Add button according to MissingParameter.
            /// </summary>
            public void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            /// <summary>
            /// Open the parameter area.
            /// </summary>
            private void OpenParameterArea()
            {
                _titleText.value = true;
            }

            /// <summary>
            /// Opens or closes the parameter area.
            /// </summary>
            /// <param name="open">Opens when true.</param>
            public void SetOpenParameterArea(bool open)
            {
                _titleText.value = open;
                _parameterArea.SetActive(open);
            }

            /// <summary>
            /// Refresh the property record.
            /// </summary>
            /// <param name="eventRef">Requests an event reference.</param>
            public void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = _serializedObject.FindProperty(PropNames.Params);

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative(PropNames.Name).stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative(PropNames.Value);

                    PropertyRecord record = _propertyRecords.Find(r => r.Name == name);

                    if (record != null)
                        record.ValueProperties.Add(valueProperty);
                    else
                    {
                        EditorParamRef paramRef = eventRef.LocalParameters.Find(parameter => parameter.Name == name);

                        if (paramRef != null)
                        {
                            _propertyRecords.Add(
                                new PropertyRecord()
                                {
                                    ParamRef = paramRef,
                                    ValueProperties = new List<SerializedProperty>() { valueProperty }
                                });
                        }
                    }
                }

                // Sort only when there are multiple selections. When there is only one object selected
                // The user can revert to the prefab and its behavior will depend on the arrangement order, so it is helpful to show the actual order.
                _propertyRecords.Sort((a, b) => EditorUtility.NaturalCompare(a.Name, b.Name));
                _missingParameters.Clear();

                if (eventRef != null)
                    foreach (var parameter in eventRef.LocalParameters)
                    {
                        PropertyRecord record = _propertyRecords.Find(p => p.Name == parameter.Name);

                        if (record == null)
                            _missingParameters.Add(parameter);
                    }
                else
                    Dispose();
            }

            /// <summary>
            /// Draws a value slide.
            /// </summary>
            /// <param name="preRefresh">Refresh in the beginning.</param>
            public void DrawValues(bool preRefresh = false)
            {
                if (preRefresh)
                {
                    string path = string.Empty;
                    EditorEventRef eventRef = null;
                    var useGlobalKeyList = _serializedObject.FindProperty(PropNames.UseGlobalKeyList).boolValue;

                    switch (_commandSender.BehaviourStyle)
                    {
                        case CommandBehaviourStyle.Play:
                        case CommandBehaviourStyle.PlayOnAPI:
                        case CommandBehaviourStyle.ParameterOnAPI:
                            switch (_commandSender.ClipStyle)
                            {
                                case ClipStyle.EventReference:
                                    path = _serializedObject.FindProperty(PropNames.Clip)
                                        .FindPropertyRelative(PropNames.Path)
                                        .stringValue;
                                    break;
                                case ClipStyle.Key:

                                    if (useGlobalKeyList)
                                    {
                                        switch (_commandSender.AudioStyle)
                                        {
                                            case AudioType.AMB:
                                                eventRef = AMBKeyList.Instance.GetEventRef(_commandSender.Key);
                                                break;
                                            case AudioType.BGM:
                                                eventRef = BGMKeyList.Instance.GetEventRef(_commandSender.Key);
                                                break;
                                            case AudioType.SFX:
                                                eventRef = SFXKeyList.Instance.GetEventRef(_commandSender.Key);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                    }
                                    else
                                    {
                                        LocalKeyList targetKeyList = (LocalKeyList)_serializedObject
                                            .FindProperty(PropNames.KeyList).objectReferenceValue;

                                        if (!targetKeyList)
                                            return;

                                        SerializedObject targetLocalKeyList = new(targetKeyList);
                                        SerializedProperty lists = targetLocalKeyList.FindProperty(PropNames.Clips)
                                            .FindPropertyRelative(PropNames.List);

                                        foreach (SerializedProperty list in lists)
                                        {
                                            string targetKey = list.FindPropertyRelative(PropNames.Key).stringValue;
                                            string targetPath = list.FindPropertyRelative(PropNames.Value)
                                                .FindPropertyRelative(PropNames.Path)
                                                .stringValue;

                                            if (_serializedObject.FindProperty(PropNames.Key).stringValue == targetKey)
                                            {
                                                eventRef = EventManager.EventFromPath(targetPath);
                                                break;
                                            }
                                        }
                                    }

                                    path = eventRef == null ? string.Empty : eventRef.Path;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            break;
                        case CommandBehaviourStyle.Parameter:

                            var clipEvent = (FMODAudioSource)_serializedObject.FindProperty(PropNames.Source)
                                .objectReferenceValue;

                            if (clipEvent)
                                path = clipEvent.clip.Path;

                            break;
                        case CommandBehaviourStyle.GlobalParameter:
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        eventRef = EventManager.EventFromPath(path);
                        RefreshPropertyRecords(eventRef);
                    }
                }

                // parameterArea 자식들은 모두 제거하기
                _parameterArea.Clear();

                foreach (PropertyRecord record in _propertyRecords)
                    _parameterArea.Add(AdaptiveParameterField(record));

                if (preRefresh)
                    CalculateEnableAddButton();
            }

            /// <summary>
            /// Draws a global variable slider.
            /// </summary>
            /// <param name="preRefresh">Refresh in the beginning.</param>
            public void DrawGlobalValues(bool preRefresh = false)
            {
                if (preRefresh)
                    _editorParamRef = EventManager.ParamFromPath(_commandSender.Parameter);

                if (_editorParamRef == null)
                    return;

                var value = _serializedObject.FindProperty(PropNames.Value);

                value.floatValue =
                    Mathf.Clamp(value.floatValue, _editorParamRef.Min, _editorParamRef.Max);

                _serializedObject.ApplyModifiedProperties();

                _parameterArea.Clear();
                _parameterArea.Add(AdaptiveParameterField(_editorParamRef));
            }

            /// <summary>
            /// Parameter fields are automatically created according to the record type.
            /// </summary>
            /// <param name="record">Request property record</param>
            /// <returns>Field UI returned</returns>
            private SimpleBaseField AdaptiveParameterField(PropertyRecord record)
            {
                float value = 0;

                if (record.ValueProperties.Count == 1)
                    value = record.ValueProperties[0].floatValue;
                else
                {
                    bool first = true;

                    foreach (SerializedProperty property in record.ValueProperties)
                        if (first)
                        {
                            value = property.floatValue;
                            first = false;
                        }
                }

                var baseField = new SimpleBaseField
                {
                    Label = record.Name,
                    style =
                    {
                        marginTop = 0,
                        marginBottom = 0
                    }
                };

                #region BaseField ContentContainer Style

                baseField.contentContainer.style.borderTopWidth = 0;
                baseField.contentContainer.style.borderBottomWidth = 0;
                baseField.contentContainer.style.paddingTop = 0;
                baseField.contentContainer.style.paddingBottom = 0;

                #endregion

                switch (record.ParamRef.Type)
                {
                    case ParameterType.Continuous:

                        var floatSlider = new Slider(record.ParamRef.Min, record.ParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = value
                        };

                        foreach (SerializedProperty property in record.ValueProperties)
                            floatSlider.value = property.floatValue;

                        baseField.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                        {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;
                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)record.ParamRef.Min, (int)record.ParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)value
                        };

                        baseField.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt =>
                        {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;
                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = record.ParamRef.Labels.ToList(),
                            index = (int)value
                        };

                        baseField.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ =>
                        {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = dropdown.index;
                            _serializedObject.ApplyModifiedProperties();
                        });

                        break;
                }

                var btn = new Button
                {
                    text = "Remove",
                    style =
                    {
                        marginRight = 0f
                    }
                };

                baseField.contentContainer.Add(btn);

                btn.clicked += () =>
                {
                    DeleteParameter(record.Name);
                    DrawValues(true);
                };

                return baseField;
            }

            /// <summary>
            /// In the context of the editor, automatically creates parameter fields based on the provided record type and returns created UI field.
            /// </summary>
            /// <param name="editorParamRef">The record to be used as a reference for creating editor parameter fields.</param>
            /// <returns>The created UI field for the editor parameters.</returns>
            private SimpleBaseField AdaptiveParameterField(EditorParamRef editorParamRef)
            {
                var globalParameterLayout = new SimpleBaseField
                {
                    name = $"{editorParamRef.Name} Field Layout",
                    Label = "Override Value",
                    style =
                    {
                        marginTop = 0,
                        marginBottom = 0
                    }
                };

                #region global Parameter Layout ContentContainer Style

                globalParameterLayout.contentContainer.style.borderTopWidth = 0;
                globalParameterLayout.contentContainer.style.borderBottomWidth = 0;
                globalParameterLayout.contentContainer.style.paddingTop = 0;
                globalParameterLayout.contentContainer.style.paddingBottom = 0;

                #endregion

                switch (editorParamRef.Type)
                {
                    // 여기에서 Value에 알맞는 값으 전달해야함.
                    case ParameterType.Continuous:

                        var floatSlider = new Slider(editorParamRef.Min, editorParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = _commandSender.Value
                        };

                        globalParameterLayout.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt =>
                            _commandSender.Value = evt.newValue);

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)editorParamRef.Min, (int)editorParamRef.Max)
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)_commandSender.Value
                        };

                        globalParameterLayout.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt =>
                            _commandSender.Value = evt.newValue);

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField
                        {
                            style =
                            {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = editorParamRef.Labels.ToList(),
                            index = (int)_commandSender.Value
                        };

                        globalParameterLayout.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ =>
                            _commandSender.Value = dropdown.index);

                        break;
                }

                return globalParameterLayout;
            }

            /// <summary>
            /// Draw the Add button.
            /// </summary>
            /// <param name="position">Where to draw the menu</param>
            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("All"), false, () =>
                {
                    foreach (EditorParamRef parameter in _missingParameters)
                        AddParameter(parameter);

                    // 토글을 펼칩니다.
                    OpenParameterArea();

                    DrawValues(true);
                    SetOpenParameterArea(true);
                });

                menu.AddSeparator(string.Empty);

                foreach (EditorParamRef parameter in _missingParameters)
                {
                    menu.AddItem(new GUIContent(parameter.Name), false,
                        (userData) =>
                        {
                            AddParameter(userData as EditorParamRef);

                            // 토글을 펼칩니다.
                            OpenParameterArea();

                            DrawValues(true);
                            SetOpenParameterArea(true);
                        },
                        parameter);
                }

                menu.DropDown(position);
            }

            /// <summary>
            /// Adds initial values for the given parameters to all selected objects that have no parameters.
            /// </summary>
            /// <param name="parameter">If the parameter does not exist, add it.</param>
            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_commandSender.Params, p => p.Name == parameter.Name) < 0)
                {
                    SerializedProperty paramsProperty = _serializedObject.FindProperty(PropNames.Params);

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative(PropNames.Name).stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative(PropNames.Value).floatValue = parameter.Default;

                    _serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// Deletes the parameter.
            /// </summary>
            /// <param name="name">Removes a parameter by its name.</param>
            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = _serializedObject.FindProperty(PropNames.Params);

                foreach (SerializedProperty child in paramsProperty)
                {
                    string paramName = child.FindPropertyRelative(PropNames.Name).stringValue;
                    if (paramName == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif