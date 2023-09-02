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

        private string _oldPath;
        private string _oldParameterPath;
        private string _oldTargetPath;

        private StyleSheet _boxGroupStyle;
        private StyleSheet _buttonStyleSheet;

        private SerializedProperty _params;
        private SerializedProperty _parameter;
        private SerializedProperty _value;
        private SerializedProperty _behaviourStyle;
        private SerializedProperty _audioSource;
        private SerializedProperty _clip;
        private SerializedProperty _path;
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

        private SerializedProperty _previewEventPath;

        private VisualElement _root;

        private static readonly string kClip = "Clip";
        private static readonly string kClips = "Clips";
        private static readonly string kList = "list";
        private static readonly string kKey = "Key";
        private static readonly string kValue = "Value";
        private static readonly string kPath = "Path";
        private static readonly string kParams = "Params";
        private static readonly string kName = "Name";

        private string _oldKey;
        private FMODAudioSource _oldAudioSource;
        private bool _oldUseGlobalKeyList;
        private ClipStyle _oldClipStyle;
        private AudioType _oldAudioStyle;
        private CommandBehaviourStyle _oldCommandBehaviourStyle;
        private bool _oldFade;
        private LocalKeyList _oldLocalKeyList;

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

        private void FindProperty()
        {
            _params = serializedObject.FindProperty("Params");
            _parameter = serializedObject.FindProperty("Parameter");
            _value = serializedObject.FindProperty("Value");
            _audioSource = serializedObject.FindProperty("Source");
            _behaviourStyle = serializedObject.FindProperty("BehaviourStyle");
            _clip = serializedObject.FindProperty("Clip");
            _path = _clip.FindPropertyRelative("Path");
            _useGlobalKeyList = serializedObject.FindProperty("useGlobalKeyList");
            _clipStyle = serializedObject.FindProperty("ClipStyle");
            _key = serializedObject.FindProperty("Key");
            _fade = serializedObject.FindProperty("Fade");
            _sendOnStart = serializedObject.FindProperty("SendOnStart");
            _onPlaySend = serializedObject.FindProperty("OnPlaySend");
            _onStopSend = serializedObject.FindProperty("OnStopSend");
            _onParameterSend = serializedObject.FindProperty("OnParameterSend");
            _localKeyList = serializedObject.FindProperty("keyList");
            _audioStyle = serializedObject.FindProperty("AudioStyle");
            _previewEventPath = serializedObject.FindProperty("Clip").FindPropertyRelative(kPath);
        }

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
                _parameterValueView.InitParameterView(root1, parameterArea, _commandSender);

            VisualElement notFoundField = FMODEditorUtility.CreateNotFoundField();
            VisualElement eventSpace = NKEditorUtility.Space(5f);

            _root.Add(root0);
            _root.Add(NKEditorUtility.Space(5));
            root0.Add(behaviourStyleField);
            root0.Add(audioSourceField);

            _root.Add(root1);
            root1.Add(useGlobalKeyListField);
            root1.Add(localKeyListField);
            root1.Add(audioStyleField);
            root1.Add(line);
            root1.Add(clipStyleField);
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

            ControlField(elements);

            if (_params.arraySize > 0)
            {
                titleToggleLayout.Open();
                parameterArea.SetActive(true);
            }

            RegisterAudioSourcePathValueChange(_ => { ControlField(elements); });

            clipField.RegisterValueChangeCallback(_clip, _oldPath, _ =>
            {
                _parameterValueView.Dispose();
                ControlField(elements);
            });

            _oldKey = _key.stringValue;
            keyField.RegisterValueChangeCallback(evt =>
            {
                if (_oldKey != evt.changedProperty.stringValue)
                {
                    ControlField(elements);
                    _oldKey = evt.changedProperty.stringValue;
                }
            });

            _oldAudioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
            audioSourceField.RegisterValueChangedCallback(evt =>
            {
                if (_oldAudioSource != (FMODAudioSource)evt.newValue)
                {
                    ControlField(elements);
                    _oldAudioSource = (FMODAudioSource)evt.newValue;
                }
            });

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

            titleToggleLayout.RegisterValueChangedCallback(_ => { ControlField(elements); });

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

            _oldFade = _fade.boolValue;
            fadeField.RegisterValueChangeCallback(evt =>
            {
                if (_oldFade != evt.changedProperty.boolValue)
                {
                    ControlField(elements);
                    _oldFade = evt.changedProperty.boolValue;
                }
            });

            _oldLocalKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
            localKeyListField.RegisterValueChangedCallback(evt =>
            {
                if (_oldLocalKeyList != (LocalKeyList)evt.newValue)
                {
                    ControlField(elements);
                    _oldLocalKeyList = (LocalKeyList)evt.newValue;
                }
            });

            globalParameterField.RegisterValueChangeCallback(_parameter, _oldParameterPath,
                _ => { ControlField(elements); });
        }

        private void RegisterAudioSourcePathValueChange(Action<string> callback)
        {
            _root.schedule.Execute(() =>
            {
                if (_commandSender.Source)
                    if (_oldTargetPath != _commandSender.Source.Clip.Path)
                    {
                        callback.Invoke(_commandSender.Source.Clip.Path);
                        _oldTargetPath = _commandSender.Source.Clip.Path;
                    }
            }).Every(5);
        }

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

            // 일단 전부 비활성화
            foreach (VisualElement visualElement in elements)
                visualElement.SetActive(false);

            var behaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;
            ClipStyle clipStyl;
            switch (behaviourStyle)
            {
                case CommandBehaviourStyle.Play:
                    parameterArea.style.marginLeft = 15;
                    behaviourStyleField.SetActive(true);
                    audioSourceField.SetActive(true);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        var clipStyle = (ClipStyle)_clipStyle.enumValueIndex;
                        switch (clipStyle)
                        {
                            case ClipStyle.EventReference:
                                clipStyleField.SetActive(true);
                                clipField.SetActive(true);

                                if (!string.IsNullOrWhiteSpace(_path.stringValue))
                                {
                                    EditorEventRef existEvent = EventManager.EventFromPath(_path.stringValue);
                                    if (existEvent != null)
                                    {
                                        _parameterValueView.RefreshPropertyRecords(existEvent);
                                        _parameterValueView.DrawValues();
                                        _parameterValueView.CalculateEnableAddButton();

                                        addButton.SetActive(true);
                                        titleToggleLayout.SetActive(true);
                                        initializeField.SetActive(true);
                                        sendOnStart.SetActive(true);
                                    }
                                    else
                                    {
                                        string msg = Application.systemLanguage == SystemLanguage.Korean
                                            ? "연결된 이벤트 주소가 유효하지 않습니다."
                                            : "The connected event address is invalid.";

                                        helpBox.text = msg;
                                        helpBox.SetActive(true);
                                    }
                                }
                                else
                                {
                                    string msg = Application.systemLanguage == SystemLanguage.Korean
                                        ? "Event가 비어있습니다."
                                        : "Event is empty.";

                                    helpBox.text = msg;
                                    helpBox.SetActive(true);
                                }

                                break;
                            case ClipStyle.Key:
                                useGlobalKeyListField.SetActive(true);

                                if (_useGlobalKeyList.boolValue)
                                    audioStyleField.SetActive(true);
                                else
                                    localKeyListField.SetActive(true);

                                clipStyleField.SetActive(true);

                                if (_useGlobalKeyList.boolValue)
                                {
                                    line.SetActive(true);
                                    keyField.SetActive(true);

                                    if (!string.IsNullOrWhiteSpace(_key.stringValue))
                                    {
                                        EditorEventRef existEvent = null;

                                        if (_useGlobalKeyList.boolValue)
                                        {
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
                                        }
                                        else
                                        {
                                            LocalKeyList targetKeyList =
                                                (LocalKeyList)_localKeyList.objectReferenceValue;
                                            SerializedObject targetLocalKeyList = new(targetKeyList);
                                            SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                                                .FindPropertyRelative(kList);

                                            foreach (SerializedProperty list in lists)
                                            {
                                                string targetKey = list.FindPropertyRelative(kKey).stringValue;
                                                string targetPath = list.FindPropertyRelative(kValue)
                                                    .FindPropertyRelative(kPath)
                                                    .stringValue;


                                                if (_key.stringValue == targetKey)
                                                {
                                                    existEvent = EventManager.EventFromPath(targetPath);
                                                    break;
                                                }
                                            }
                                        }

                                        if (existEvent != null)
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
                                        else
                                        {
                                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                                ? "연결된 이벤트 주소가 유효하지 않습니다."
                                                : "The connected event address is invalid.";

                                            helpBox.text = msg;
                                            helpBox.SetActive(true);
                                            notFoundField.SetActive(true);
                                            _parameterValueView.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        string msg = Application.systemLanguage == SystemLanguage.Korean
                                            ? "Key가 비어있습니다."
                                            : "Key is empty.";

                                        helpBox.text = msg;
                                        helpBox.SetActive(true);
                                        notFoundField.SetActive(true);
                                        _parameterValueView.Dispose();
                                    }
                                }
                                else
                                {
                                    if (_localKeyList.objectReferenceValue != null)
                                    {
                                        line.SetActive(true);
                                        keyField.SetActive(true);

                                        if (!string.IsNullOrWhiteSpace(_key.stringValue))
                                        {
                                            EditorEventRef existEvent = null;

                                            if (_useGlobalKeyList.boolValue)
                                            {
                                                switch (_commandSender.AudioStyle)
                                                {
                                                    case AudioType.AMB:
                                                        existEvent =
                                                            AMBKeyList.Instance.GetEventRef(_commandSender.Key);
                                                        break;
                                                    case AudioType.BGM:
                                                        existEvent =
                                                            BGMKeyList.Instance.GetEventRef(_commandSender.Key);
                                                        break;
                                                    case AudioType.SFX:
                                                        existEvent =
                                                            SFXKeyList.Instance.GetEventRef(_commandSender.Key);
                                                        break;
                                                    default:
                                                        throw new ArgumentOutOfRangeException();
                                                }
                                            }
                                            else
                                            {
                                                LocalKeyList targetKeyList =
                                                    (LocalKeyList)_localKeyList.objectReferenceValue;
                                                SerializedObject targetLocalKeyList = new(targetKeyList);
                                                SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                                                    .FindPropertyRelative(kList);

                                                foreach (SerializedProperty list in lists)
                                                {
                                                    string targetKey = list.FindPropertyRelative(kKey).stringValue;
                                                    string targetPath = list.FindPropertyRelative(kValue)
                                                        .FindPropertyRelative(kPath)
                                                        .stringValue;


                                                    if (_key.stringValue == targetKey)
                                                    {
                                                        existEvent = EventManager.EventFromPath(targetPath);
                                                        break;
                                                    }
                                                }
                                            }

                                            if (existEvent != null)
                                            {
                                                _parameterValueView.RefreshPropertyRecords(existEvent);
                                                _parameterValueView.DrawValues(true);
                                                _parameterValueView.CalculateEnableAddButton();

                                                addButton.SetActive(true);
                                                titleToggleLayout.SetActive(true);
                                                initializeField.SetActive(true);
                                                sendOnStart.SetActive(true);

                                                var toggleOnOff = titleToggleLayout.value;
                                                parameterArea.SetActive(toggleOnOff);
                                            }
                                            else
                                            {
                                                string msg = Application.systemLanguage == SystemLanguage.Korean
                                                    ? "연결된 이벤트 주소가 유효하지 않습니다."
                                                    : "The connected event address is invalid.";

                                                helpBox.text = msg;
                                                helpBox.SetActive(true);
                                                notFoundField.SetActive(true);
                                                _parameterValueView.Dispose();
                                            }
                                        }
                                        else
                                        {
                                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                                ? "Key가 비어있습니다."
                                                : "Key is empty.";

                                            helpBox.text = msg;
                                            helpBox.SetActive(true);
                                            notFoundField.SetActive(true);
                                            _parameterValueView.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        string msg = Application.systemLanguage == SystemLanguage.Korean
                                            ? "Key List가 연결되어있지 않습니다."
                                            : "Key List is not connected.";

                                        helpBox.text = msg;
                                        helpBox.SetActive(true);
                                        _parameterValueView.Dispose();
                                    }
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        _parameterValueView.Dispose();

                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "FMOD Audio Source가 연결되어 있지 않습니다."
                            : "FMOD Audio Source is not connected.";

                        helpBox.text = msg;
                        helpBox.SetActive(true);
                    }

                    break;
                // case CommandBehaviourStyle.PlayOnAPI:
                //     clipField.label = "Event";
                //     parameterArea.style.marginLeft = 15;
                //     behaviourStyleField.SetActive(true);
                //     clipStyleField.SetActive(true);
                //     onPlaySend.SetActive(true);
                //     eventSpace.SetActive(true);
                //
                //     clipStyl = (ClipStyle)_clipStyle.enumValueIndex;
                //
                //     switch (clipStyl)
                //     {
                //         case ClipStyle.EventReference:
                //             useGlobalKeyListField.SetActive(false);
                //             clipField.SetActive(true);
                //
                //             if (!string.IsNullOrWhiteSpace(_path.stringValue))
                //             {
                //                 EditorEventRef existEvent = EventManager.EventFromPath(_path.stringValue);
                //                 if (existEvent != null)
                //                 {
                //                     _parameterValueView.RefreshPropertyRecords(existEvent);
                //                     _parameterValueView.DrawValues();
                //                     _parameterValueView.CalculateEnableAddButton();
                //
                //                     addButton.SetActive(true);
                //                     titleToggleLayout.SetActive(true);
                //                     initializeField.SetActive(true);
                //                     sendOnStart.SetActive(true);
                //
                //                     var toggleOnOff = titleToggleLayout.value;
                //                     parameterArea.SetActive(toggleOnOff);
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                         : "The connected event address is invalid.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                 }
                //             }
                //             else
                //             {
                //                 string msg = Application.systemLanguage == SystemLanguage.Korean
                //                     ? "Event가 비어있습니다."
                //                     : "Event is empty.";
                //
                //                 helpBox.text = msg;
                //                 helpBox.SetActive(true);
                //             }
                //
                //             break;
                //         case ClipStyle.Key:
                //
                //             useGlobalKeyListField.SetActive(true);
                //
                //             if (_useGlobalKeyList.boolValue)
                //             {
                //                 line.SetActive(true);
                //                 keyField.SetActive(true);
                //
                //                 audioStyleField.SetActive(true);
                //
                //                 if (!string.IsNullOrWhiteSpace(_key.stringValue))
                //                 {
                //                     EditorEventRef existEvent = null;
                //
                //                     AudioType audioType = (AudioType)_audioStyle.enumValueIndex;
                //                     switch (audioType)
                //                     {
                //                         case AudioType.AMB:
                //                             existEvent = AMBKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         case AudioType.BGM:
                //                             existEvent = BGMKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         case AudioType.SFX:
                //                             existEvent = SFXKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         default:
                //                             throw new ArgumentOutOfRangeException();
                //                     }
                //
                //                     if (existEvent != null)
                //                     {
                //                         _parameterValueView.RefreshPropertyRecords(existEvent);
                //                         _parameterValueView.DrawValues();
                //                         _parameterValueView.CalculateEnableAddButton();
                //
                //                         addButton.SetActive(true);
                //                         titleToggleLayout.SetActive(true);
                //                         initializeField.SetActive(true);
                //                         sendOnStart.SetActive(true);
                //
                //                         var toggleOnOff = titleToggleLayout.value;
                //                         parameterArea.SetActive(toggleOnOff);
                //                     }
                //                     else
                //                     {
                //                         string msg = Application.systemLanguage == SystemLanguage.Korean
                //                             ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                             : "The connected event address is invalid.";
                //
                //                         helpBox.text = msg;
                //                         helpBox.SetActive(true);
                //                         notFoundField.SetActive(true);
                //                         _parameterValueView.Dispose();
                //                     }
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "Key가 비어있습니다."
                //                         : "Key is empty.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                     notFoundField.SetActive(true);
                //                     _parameterValueView.Dispose();
                //                 }
                //             }
                //             else
                //             {
                //                 localKeyListField.SetActive(true);
                //
                //                 if (_localKeyList.objectReferenceValue != null)
                //                 {
                //                     line.SetActive(true);
                //                     keyField.SetActive(true);
                //
                //                     if (!string.IsNullOrWhiteSpace(_key.stringValue))
                //                     {
                //                         if (!_useGlobalKeyList.boolValue)
                //                             localKeyListField.SetActive(true);
                //
                //                         EditorEventRef existEvent = null;
                //
                //                         LocalKeyList targetKeyList =
                //                             (LocalKeyList)_localKeyList.objectReferenceValue;
                //                         SerializedObject targetLocalKeyList = new(targetKeyList);
                //                         SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                //                             .FindPropertyRelative(kList);
                //
                //                         foreach (SerializedProperty list in lists)
                //                         {
                //                             string targetKey = list.FindPropertyRelative(kKey).stringValue;
                //                             string targetPath = list.FindPropertyRelative(kValue)
                //                                 .FindPropertyRelative(kPath)
                //                                 .stringValue;
                //
                //                             if (_key.stringValue == targetKey)
                //                             {
                //                                 existEvent = EventManager.EventFromPath(targetPath);
                //                                 break;
                //                             }
                //                         }
                //
                //                         if (existEvent != null)
                //                         {
                //                             _parameterValueView.RefreshPropertyRecords(existEvent);
                //                             _parameterValueView.DrawValues();
                //                             _parameterValueView.CalculateEnableAddButton();
                //
                //                             addButton.SetActive(true);
                //                             titleToggleLayout.SetActive(true);
                //                             initializeField.SetActive(true);
                //                             sendOnStart.SetActive(true);
                //
                //                             var toggleOnOff = titleToggleLayout.value;
                //                             parameterArea.SetActive(toggleOnOff);
                //                         }
                //                         else
                //                         {
                //                             string msg = Application.systemLanguage == SystemLanguage.Korean
                //                                 ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                                 : "The connected event address is invalid.";
                //
                //                             helpBox.text = msg;
                //                             helpBox.SetActive(true);
                //                             notFoundField.SetActive(true);
                //                             _parameterValueView.Dispose();
                //                         }
                //                     }
                //                     else
                //                     {
                //                         string msg = Application.systemLanguage == SystemLanguage.Korean
                //                             ? "Key가 비어있습니다."
                //                             : "Key is empty.";
                //
                //                         helpBox.text = msg;
                //                         helpBox.SetActive(true);
                //                         notFoundField.SetActive(true);
                //                         _parameterValueView.Dispose();
                //                     }
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "Key List가 연결되어있지 않습니다."
                //                         : "Key List is not connected.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                     _parameterValueView.Dispose();
                //                 }
                //             }
                //
                //             break;
                //         default:
                //             throw new ArgumentOutOfRangeException();
                //     }
                //
                //     break;
                // case CommandBehaviourStyle.Stop:
                //
                //     behaviourStyleField.SetActive(true);
                //     audioSourceField.SetActive(true);
                //
                //     if (_audioSource.objectReferenceValue != null)
                //     {
                //         fadeField.SetActive(true);
                //         sendOnStart.SetActive(true);
                //     }
                //     else
                //     {
                //         string msg = Application.systemLanguage == SystemLanguage.Korean
                //             ? "FMOD Audio Source가 연결되어 있지 않습니다."
                //             : "FMOD Audio Source is not connected.";
                //
                //         helpBox.text = msg;
                //         helpBox.SetActive(true);
                //
                //         _parameterValueView.Dispose();
                //     }
                //
                //     if (_fade.boolValue)
                //         fadeHelpBox.SetActive(true);
                //
                //     break;
                // case CommandBehaviourStyle.StopOnAPI:
                //
                //     behaviourStyleField.SetActive(true);
                //     fadeField.SetActive(true);
                //     onStopSend.SetActive(true);
                //     eventSpace.SetActive(true);
                //     sendOnStart.SetActive(true);
                //
                //     if (_fade.boolValue)
                //         fadeHelpBox.SetActive(true);
                //
                //     break;
                // case CommandBehaviourStyle.Parameter:
                //     behaviourStyleField.SetActive(true);
                //     audioSourceField.SetActive(true);
                //     sendButton.SetActive(true);
                //
                //     if (_audioSource.objectReferenceValue != null)
                //     {
                //         var targetAudioSource = new SerializedObject(_audioSource.objectReferenceValue);
                //         var targetPath = targetAudioSource.FindProperty("clip").FindPropertyRelative("Path");
                //
                //         bool hasEvent = !string.IsNullOrWhiteSpace(targetPath.stringValue);
                //         if (hasEvent)
                //         {
                //             EditorEventRef existEvent = EventManager.EventFromPath(targetPath.stringValue);
                //
                //             if (existEvent != null)
                //             {
                //                 addButton.SetActive(true);
                //                 initializeField.SetActive(true);
                //
                //                 // 스타일이 Base 방식일 때만 처리로 현재는 되어있다.
                //                 _parameterValueView.RefreshPropertyRecords(existEvent);
                //                 _parameterValueView.DrawValues();
                //                 _parameterValueView.CalculateEnableAddButton();
                //
                //                 titleToggleLayout.SetActive(true);
                //                 parameterArea.SetActive(true);
                //                 helpBox.SetActive(false);
                //                 sendOnStart.SetActive(true);
                //             }
                //             else
                //             {
                //                 string msg = Application.systemLanguage == SystemLanguage.Korean
                //                     ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                     : "The connected event address is invalid.";
                //
                //                 helpBox.text = msg;
                //                 helpBox.SetActive(true);
                //                 notFoundField.SetActive(true);
                //                 _parameterValueView.Dispose();
                //                 titleToggleLayout.Close();
                //             }
                //         }
                //         else
                //         {
                //             string msg = Application.systemLanguage == SystemLanguage.Korean
                //                 ? "FMOD Audio Source에 Clip이 연결되어 있지 않습니다."
                //                 : "Clip is not connected to FMOD Audio Source.";
                //
                //             helpBox.text = msg;
                //             helpBox.SetActive(true);
                //             notFoundField.SetActive(true);
                //             _parameterValueView.Dispose();
                //             titleToggleLayout.Close();
                //         }
                //     }
                //     else
                //     {
                //         string msg = Application.systemLanguage == SystemLanguage.Korean
                //             ? "FMOD Audio Source가 연결되어 있지 않습니다."
                //             : "FMOD Audio Source is not connected.";
                //
                //         helpBox.text = msg;
                //         helpBox.SetActive(true);
                //         titleToggleLayout.SetActive(false);
                //         parameterArea.SetActive(false);
                //         sendOnStart.SetActive(false);
                //     }
                //
                //     break;
                // case CommandBehaviourStyle.ParameterOnAPI:
                //     clipField.label = "Parameter Name Load";
                //     eventSpace.SetActive(true);
                //     clipStyleField.SetActive(true);
                //     onParameterSend.SetActive(true);
                //     behaviourStyleField.SetActive(true);
                //     useGlobalKeyListField.SetActive(true);
                //
                //     clipStyl = (ClipStyle)_clipStyle.enumValueIndex;
                //
                //     switch (clipStyl)
                //     {
                //         case ClipStyle.EventReference:
                //             useGlobalKeyListField.SetActive(false);
                //             clipField.SetActive(true);
                //
                //             if (!string.IsNullOrWhiteSpace(_previewEventPath.stringValue))
                //             {
                //                 EditorEventRef existEvent = EventManager.EventFromPath(_previewEventPath.stringValue);
                //                 if (existEvent != null)
                //                 {
                //                     addButton.SetActive(true);
                //                     initializeField.SetActive(true);
                //
                //                     _parameterValueView.RefreshPropertyRecords(existEvent);
                //                     _parameterValueView.DrawValues();
                //                     _parameterValueView.CalculateEnableAddButton();
                //
                //                     titleToggleLayout.SetActive(true);
                //                     parameterArea.SetActive(true);
                //                     helpBox.SetActive(false);
                //                     sendOnStart.SetActive(true);
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                         : "The connected event address is invalid.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                     _parameterValueView.Dispose();
                //                     titleToggleLayout.Close();
                //                 }
                //             }
                //             else
                //             {
                //                 string msg = Application.systemLanguage == SystemLanguage.Korean
                //                     ? "Event가 비어있습니다."
                //                     : "Event is empty.";
                //
                //                 helpBox.text = msg;
                //                 helpBox.SetActive(true);
                //                 _parameterValueView.Dispose();
                //                 titleToggleLayout.Close();
                //             }
                //
                //             break;
                //         case ClipStyle.Key:
                //
                //             // 글로벌 키 리스트를 사용할 경우
                //             if (_useGlobalKeyList.boolValue)
                //             {
                //                 line.SetActive(true);
                //                 keyField.SetActive(true);
                //
                //                 audioStyleField.SetActive(true);
                //
                //                 if (!string.IsNullOrWhiteSpace(_key.stringValue))
                //                 {
                //                     EditorEventRef existEvent;
                //
                //                     AudioType audioType = (AudioType)_audioStyle.enumValueIndex;
                //                     switch (audioType)
                //                     {
                //                         case AudioType.AMB:
                //                             existEvent = AMBKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         case AudioType.BGM:
                //                             existEvent = BGMKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         case AudioType.SFX:
                //                             existEvent = SFXKeyList.Instance.GetEventRef(_commandSender.Key);
                //                             break;
                //                         default:
                //                             throw new ArgumentOutOfRangeException();
                //                     }
                //
                //                     if (existEvent != null)
                //                     {
                //                         _parameterValueView.RefreshPropertyRecords(existEvent);
                //                         _parameterValueView.DrawValues();
                //                         _parameterValueView.CalculateEnableAddButton();
                //
                //                         addButton.SetActive(true);
                //                         titleToggleLayout.SetActive(true);
                //                         initializeField.SetActive(true);
                //                         sendOnStart.SetActive(true);
                //
                //                         var toggleOnOff = titleToggleLayout.value;
                //                         parameterArea.SetActive(toggleOnOff);
                //                     }
                //                     else
                //                     {
                //                         string msg = Application.systemLanguage == SystemLanguage.Korean
                //                             ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                             : "The connected event address is invalid.";
                //
                //                         helpBox.text = msg;
                //                         helpBox.SetActive(true);
                //                         notFoundField.SetActive(true);
                //                         _parameterValueView.Dispose();
                //                     }
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "Key가 비어있습니다."
                //                         : "Key is empty.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                     notFoundField.SetActive(true);
                //                     _parameterValueView.Dispose();
                //                 }
                //             }
                //             else
                //             {
                //                 localKeyListField.SetActive(true);
                //
                //                 if (_localKeyList.objectReferenceValue != null)
                //                 {
                //                     line.SetActive(true);
                //                     keyField.SetActive(true);
                //
                //                     if (_useGlobalKeyList.boolValue)
                //                         audioStyleField.SetActive(true);
                //
                //                     if (!string.IsNullOrWhiteSpace(_key.stringValue))
                //                     {
                //                         if (!_useGlobalKeyList.boolValue)
                //                             localKeyListField.SetActive(true);
                //
                //                         EditorEventRef existEvent = null;
                //
                //                         LocalKeyList targetKeyList =
                //                             (LocalKeyList)_localKeyList.objectReferenceValue;
                //                         SerializedObject targetLocalKeyList = new(targetKeyList);
                //                         SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                //                             .FindPropertyRelative(kList);
                //
                //                         foreach (SerializedProperty list in lists)
                //                         {
                //                             string targetKey = list.FindPropertyRelative(kKey).stringValue;
                //                             string targetPath = list.FindPropertyRelative(kValue)
                //                                 .FindPropertyRelative(kPath)
                //                                 .stringValue;
                //
                //                             if (_key.stringValue == targetKey)
                //                             {
                //                                 existEvent = EventManager.EventFromPath(targetPath);
                //                                 break;
                //                             }
                //                         }
                //
                //                         if (existEvent != null)
                //                         {
                //                             _parameterValueView.RefreshPropertyRecords(existEvent);
                //                             _parameterValueView.DrawValues();
                //                             _parameterValueView.CalculateEnableAddButton();
                //
                //                             addButton.SetActive(true);
                //                             titleToggleLayout.SetActive(true);
                //                             initializeField.SetActive(true);
                //                             sendOnStart.SetActive(true);
                //
                //                             var toggleOnOff = titleToggleLayout.value;
                //                             parameterArea.SetActive(toggleOnOff);
                //                         }
                //                         else
                //                         {
                //                             string msg = Application.systemLanguage == SystemLanguage.Korean
                //                                 ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                                 : "The connected event address is invalid.";
                //
                //                             helpBox.text = msg;
                //                             helpBox.SetActive(true);
                //                             notFoundField.SetActive(true);
                //                             _parameterValueView.Dispose();
                //                         }
                //                     }
                //                     else
                //                     {
                //                         string msg = Application.systemLanguage == SystemLanguage.Korean
                //                             ? "Key가 비어있습니다."
                //                             : "Key is empty.";
                //
                //                         helpBox.text = msg;
                //                         helpBox.SetActive(true);
                //                         notFoundField.SetActive(true);
                //                         _parameterValueView.Dispose();
                //                     }
                //                 }
                //                 else
                //                 {
                //                     string msg = Application.systemLanguage == SystemLanguage.Korean
                //                         ? "Key List가 연결되어있지 않습니다."
                //                         : "Key List is not connected.";
                //
                //                     helpBox.text = msg;
                //                     helpBox.SetActive(true);
                //                     _parameterValueView.Dispose();
                //                 }
                //             }
                //
                //             break;
                //         default:
                //             throw new ArgumentOutOfRangeException();
                //     }
                //
                //     break;
                // case CommandBehaviourStyle.GlobalParameter:
                //     behaviourStyleField.SetActive(true);
                //     parameterField.SetActive(true);
                //
                //     if (!string.IsNullOrWhiteSpace(_parameter.stringValue))
                //     {
                //         EditorParamRef existEvent = EventManager.ParamFromPath(_parameter.stringValue);
                //
                //         if (existEvent != null)
                //         {
                //             _parameterValueView.DrawGlobalValues(true);
                //             helpBox.SetActive(false);
                //             notFoundField.SetActive(false);
                //             parameterArea.SetActive(true);
                //             parameterArea.style.marginLeft = 0;
                //             sendOnStart.SetActive(true);
                //         }
                //         else
                //         {
                //             string msg = Application.systemLanguage == SystemLanguage.Korean
                //                 ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                 : "The connected event address is invalid.";
                //             helpBox.text = msg;
                //             helpBox.SetActive(true);
                //             notFoundField.SetActive(true);
                //             parameterArea.SetActive(false);
                //             sendOnStart.SetActive(false);
                //         }
                //     }
                //     else
                //     {
                //         var editorParamRef = EventManager.ParamFromPath(_parameter.stringValue);
                //
                //         if (editorParamRef == null)
                //         {
                //             string msg = Application.systemLanguage == SystemLanguage.Korean
                //                 ? "연결된 이벤트 주소가 유효하지 않습니다."
                //                 : "The connected event address is invalid.";
                //             helpBox.text = msg;
                //             helpBox.SetActive(true);
                //             notFoundField.SetActive(true);
                //             parameterArea.SetActive(false);
                //             sendOnStart.SetActive(false);
                //         }
                //     }
                //
                //     sendButton.SetActive(true);
                //     break;
                // default:
                //     throw new ArgumentOutOfRangeException();
            }
        }

        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private SerializedObject _serializedObject;

            // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private DropdownField _addButton;
            private VisualElement _parameterArea;
            private VisualElement _parameterLayout;
            private Foldout _titleText;

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
                }
            }

            public Tuple<VisualElement, Foldout, DropdownField> InitParameterView(VisualElement root,
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

            public void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            private void SetOpenParameterArea(bool open)
            {
                _titleText.value = open;
                _parameterArea.SetActive(open);
            }

            public void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

                foreach (SerializedProperty parameterProperty in paramsProperty)
                {
                    string name = parameterProperty.FindPropertyRelative("Name").stringValue;
                    SerializedProperty valueProperty = parameterProperty.FindPropertyRelative("Value");

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

                // 다중 선택이 있는 경우에만 정렬합니다. 선택한 개체가 하나만 있는 경우
                // 사용자는 프리팹으로 되돌릴 수 있으며 동작은 배열 순서에 따라 달라지므로 실제 순서를 표시하는 것이 도움이 됩니다.
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

            public void DrawValues(bool preRefresh = false)
            {
                if (preRefresh)
                {
                    string path;

                    switch (_commandSender.ClipStyle)
                    {
                        case ClipStyle.EventReference:
                            path = _commandSender.Clip.Path;
                            break;
                        case ClipStyle.Key:
                            EditorEventRef eventRef;

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

                            path = eventRef == null ? string.Empty : eventRef.Path;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        EditorEventRef eventRef = EventManager.EventFromPath(path);
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

            public void DrawGlobalValues(bool preRefresh = false)
            {
                if (preRefresh)
                    _editorParamRef = EventManager.ParamFromPath(_commandSender.Parameter);

                if (_editorParamRef == null)
                    return;

                var value = _serializedObject.FindProperty("Value");

                value.floatValue =
                    Mathf.Clamp(value.floatValue, _editorParamRef.Min, _editorParamRef.Max);

                _serializedObject.ApplyModifiedProperties();

                _parameterArea.Clear();
                _parameterArea.Add(AdaptiveParameterField(_editorParamRef));
            }

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

            private void OpenParameterArea()
            {
                _titleText.value = true;
            }

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

            // 매개변수가 없는 모든 선택된 객체에 주어진 매개변수에 대한 초기값을 추가합니다.
            private void AddParameter(EditorParamRef parameter)
            {
                if (Array.FindIndex(_commandSender.Params, p => p.Name == parameter.Name) < 0)
                {
                    SerializedProperty paramsProperty = _serializedObject.FindProperty(kParams);

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    _serializedObject.ApplyModifiedProperties();
                }
            }

            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = _serializedObject.FindProperty(kParams);

                foreach (SerializedProperty child in paramsProperty)
                {
                    string paramName = child.FindPropertyRelative("Name").stringValue;
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