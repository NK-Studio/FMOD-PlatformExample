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

        [SerializeField]
        private StyleSheet boxGroupStyle;

        private SerializedProperty _params;
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
        private SerializedProperty _localKeyList;
        private SerializedProperty _audioStyle;

        private VisualElement _root;

        private const string kClips = "Clips";
        private const string kList = "list";
        private const string kKey = "Key";
        private const string kValue = "Value";
        private const string kPath = "Path";

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
            FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupSheet = AssetDatabase.GUIDToAssetPath("5600a59cbafd24acf808fa415167310e");
            boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupSheet);
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
            _audioSource = serializedObject.FindProperty("Source");
            _behaviourStyle = serializedObject.FindProperty("BehaviourStyle");
            _clip = serializedObject.FindProperty("Clip");
            _path = _clip.FindPropertyRelative("Path");
            _useGlobalKeyList = serializedObject.FindProperty("UseGlobalKeyList");
            _clipStyle = serializedObject.FindProperty("ClipStyle");
            _key = serializedObject.FindProperty("Key");
            _fade = serializedObject.FindProperty("Fade");
            _sendOnStart = serializedObject.FindProperty("SendOnStart");
            _onPlaySend = serializedObject.FindProperty("OnPlaySend");
            _onStopSend = serializedObject.FindProperty("OnStopSend");
            _localKeyList = serializedObject.FindProperty("keyList");
            _audioStyle = serializedObject.FindProperty("AudioStyle");
        }
        private void InitializeRoot()
        {
            _root = new();
            _root.styleSheets.Add(boxGroupStyle);

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

            PropertyField fadeField = new();
            fadeField.BindProperty(_fade);

            PropertyField sendOnStart = new();
            sendOnStart.BindProperty(_sendOnStart);

            PropertyField onPlaySend = new();
            onPlaySend.BindProperty(_onPlaySend);

            PropertyField onStopSend = new();
            onStopSend.BindProperty(_onStopSend);

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

            //Init
            VisualElement[] visualElements = {
                audioSourceField /*0*/, clipStyleField /*1*/, clipField /*2*/, fadeField /*3*/, onPlaySend /*4*/, onStopSend /*5*/, eventSpace /*6*/,
                fadeHelpBox /*7*/, parameterArea /*8*/, useGlobalKeyListField /*9*/, localKeyListField /*10*/, line /*11*/, sendOnStart /*12*/,
                audioStyleField /*13*/, titleToggleLayout /*14*/, behaviourStyleField /*15*/, addButton /*16*/, notFoundField /*17*/, keyField /*18*/, initializeField /*19*/, helpBox /*20*/
            };

            // Init
            InitControlField(visualElements);
        }

        private void InitControlField(IReadOnlyList<VisualElement> elements)
        {
            ObjectField audioSourceField = (ObjectField)elements[0];
            PropertyField clipStyleField = (PropertyField)elements[1];
            PropertyField clipField = (PropertyField)elements[2];
            PropertyField fadeField = (PropertyField)elements[3];
            VisualElement parameterArea = elements[8];
            PropertyField useGlobalKeyListField = (PropertyField)elements[9];
            ObjectField localKeyListField = (ObjectField)elements[10];
            PropertyField audioStyleField = (PropertyField)elements[13];
            Foldout titleToggleLayout = (Foldout)elements[14];
            PropertyField behaviourStyleField = (PropertyField)elements[15];
            DropdownField addButton = (DropdownField)elements[16];
            PropertyField keyField = (PropertyField)elements[18];

            ControlField(elements);

            if (_params.arraySize > 0)
            {
                titleToggleLayout.Open();
                parameterArea.SetActive(true);
            }

            clipField.RegisterValueChangeCallback(_clip, _oldPath, _ => {
                _parameterValueView.Dispose();
                ControlField(elements);
            });

            keyField.schedule.Execute(() => keyField.RegisterValueChangeCallback(_ => {
                ControlField(elements);
            }));

            audioSourceField.schedule.Execute(() => audioSourceField.RegisterValueChangedCallback(_ => {
                ControlField(elements);
            }));

            useGlobalKeyListField.schedule.Execute(() => useGlobalKeyListField.RegisterValueChangeCallback(_ => {
                ControlField(elements);
            }));

            titleToggleLayout.schedule.Execute(() => titleToggleLayout.RegisterValueChangedCallback(_ => {
                ControlField(elements);
            }));

            clipStyleField.schedule.Execute(() => clipStyleField.RegisterValueChangeCallback(_ => {

                _parameterValueView.Dispose();
                parameterArea.Clear();
                addButton.SetEnabled(true);

                ControlField(elements);
            }));

            audioStyleField.schedule.Execute(() => audioStyleField.RegisterValueChangeCallback(_ => {

                _parameterValueView.Dispose();
                parameterArea.Clear();
                addButton.SetEnabled(true);

                ControlField(elements);
            }));

            behaviourStyleField.schedule.Execute(() => behaviourStyleField.RegisterValueChangeCallback(evt => {

                var behaviourStyle = (AudioBehaviourStyle)evt.changedProperty.enumValueIndex;

                switch (behaviourStyle)
                {
                    case AudioBehaviourStyle.Play:
                        _parameterValueView.Dispose(true);
                        break;
                    case AudioBehaviourStyle.PlayOnAPI:
                        _parameterValueView.Dispose(true);
                        break;
                    case AudioBehaviourStyle.Stop:
                        _parameterValueView.Dispose(true);
                        break;
                    case AudioBehaviourStyle.StopOnAPI:
                        _parameterValueView.Dispose(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ControlField(elements);
            }));

            fadeField.schedule.Execute(() => fadeField.RegisterValueChangeCallback(_ => {
                ControlField(elements);
            }));

            localKeyListField.schedule.Execute(() => localKeyListField.RegisterValueChangedCallback(_ => {
                ControlField(elements);
            }));
        }

        private void ControlField(IReadOnlyList<VisualElement> elements)
        {
            VisualElement audioSourceField = elements[0];
            VisualElement clipStyleField = elements[1];
            VisualElement clipField = elements[2];
            VisualElement fadeField = elements[3];
            VisualElement onPlaySend = elements[4];
            VisualElement onStopSend = elements[5];
            VisualElement eventSpace = elements[6];
            HelpBox fadeHelpBox = (HelpBox)elements[7];
            VisualElement parameterArea = elements[8];
            VisualElement useGlobalKeyListField = elements[9];
            VisualElement localKeyListField = elements[10];
            VisualElement line = elements[11];
            VisualElement sendOnStart = elements[12];
            VisualElement audioStyleField = elements[13];
            Foldout titleToggleLayout = (Foldout)elements[14];
            VisualElement behaviourStyleField = elements[15];
            DropdownField addButton = (DropdownField)elements[16];
            VisualElement notFoundField = elements[17];
            VisualElement keyField = elements[18];
            VisualElement initializeField = elements[19];
            HelpBox helpBox = (HelpBox)elements[20];

            // 일단 전부 비활성화
            foreach (VisualElement visualElement in elements)
                visualElement.SetActive(false);

            var behaviourStyle = (AudioBehaviourStyle)_behaviourStyle.enumValueIndex;

            switch (behaviourStyle)
            {
                case AudioBehaviourStyle.Play:

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
                                        _parameterValueView.DrawValues(true);
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
                                            LocalKeyList targetKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
                                            SerializedObject targetLocalKeyList = new(targetKeyList);
                                            SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                                                .FindPropertyRelative(kList);

                                            foreach (SerializedProperty list in lists)
                                            {
                                                string targetKey = list.FindPropertyRelative(kKey).stringValue;
                                                string targetPath = list.FindPropertyRelative(kValue).FindPropertyRelative(kPath)
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
                                    string msg = Application.systemLanguage == SystemLanguage.Korean
                                        ? "Key List가 연결되어있지 않습니다."
                                        : "Key List is not connected.";

                                    helpBox.text = msg;
                                    helpBox.SetActive(true);
                                    _parameterValueView.Dispose();
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
                case AudioBehaviourStyle.PlayOnAPI:

                    behaviourStyleField.SetActive(true);
                    clipStyleField.SetActive(true);
                    onPlaySend.SetActive(true);
                    eventSpace.SetActive(true);

                    ClipStyle clipStyl = (ClipStyle)_clipStyle.enumValueIndex;

                    switch (clipStyl)
                    {
                        case ClipStyle.EventReference:
                            useGlobalKeyListField.SetActive(false);
                            clipField.SetActive(true);

                            if (!string.IsNullOrWhiteSpace(_path.stringValue))
                            {
                                EditorEventRef existEvent = EventManager.EventFromPath(_path.stringValue);
                                if (existEvent != null)
                                {
                                    _parameterValueView.DrawValues(true);
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

                            if (!_useGlobalKeyList.boolValue)
                                localKeyListField.SetActive(true);

                            useGlobalKeyListField.SetActive(true);

                            if (_localKeyList.objectReferenceValue != null)
                            {
                                line.SetActive(true);
                                keyField.SetActive(true);

                                if (_useGlobalKeyList.boolValue)
                                    audioStyleField.SetActive(true);

                                if (!string.IsNullOrWhiteSpace(_key.stringValue))
                                {
                                    if (!_useGlobalKeyList.boolValue)
                                        localKeyListField.SetActive(true);

                                    EditorEventRef existEvent = null;

                                    if (_useGlobalKeyList.boolValue)
                                    {
                                        AudioType audioType = (AudioType)_audioStyle.enumValueIndex;
                                        switch (audioType)
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
                                        LocalKeyList targetKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
                                        SerializedObject targetLocalKeyList = new(targetKeyList);
                                        SerializedProperty lists = targetLocalKeyList.FindProperty(kClips)
                                            .FindPropertyRelative(kList);

                                        foreach (SerializedProperty list in lists)
                                        {
                                            string targetKey = list.FindPropertyRelative(kKey).stringValue;
                                            string targetPath = list.FindPropertyRelative(kValue).FindPropertyRelative(kPath)
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
                                string msg = Application.systemLanguage == SystemLanguage.Korean
                                    ? "Key List가 연결되어있지 않습니다."
                                    : "Key List is not connected.";

                                helpBox.text = msg;
                                helpBox.SetActive(true);
                                _parameterValueView.Dispose();
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case AudioBehaviourStyle.Stop:

                    behaviourStyleField.SetActive(true);
                    audioSourceField.SetActive(true);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        fadeField.SetActive(true);
                        sendOnStart.SetActive(true);
                    }
                    else
                    {
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? "FMOD Audio Source가 연결되어 있지 않습니다."
                            : "FMOD Audio Source is not connected.";

                        helpBox.text = msg;
                        helpBox.SetActive(true);

                        _parameterValueView.Dispose();
                    }

                    if (_fade.boolValue)
                        fadeHelpBox.SetActive(true);

                    break;
                case AudioBehaviourStyle.StopOnAPI:

                    behaviourStyleField.SetActive(true);
                    fadeField.SetActive(true);
                    onStopSend.SetActive(true);
                    eventSpace.SetActive(true);
                    sendOnStart.SetActive(true);

                    if (_fade.boolValue)
                        fadeHelpBox.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private SerializedObject _serializedTargets;

            // EditorParamRef에서 현재 선택에 있는 모든 속성에 대한 초기 매개변수 값 속성으로의 매핑.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 모든 매개변수를 "추가" 메뉴에 넣을 수 있습니다.
            private readonly List<EditorParamRef> _missingParameters = new();

            private DropdownField _addButton;
            private VisualElement _parameterArea;
            private VisualElement _parameterLayout;
            private Foldout _titleText;

            private EventCommandSender _commandSender;

            public ParameterValueView(SerializedObject serializedTargets)
            {
                _serializedTargets = serializedTargets;
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
                _titleText.value = false;

                if (clearClipAndKey)
                {
                    _commandSender.Clip = new EventReference();
                    _commandSender.Key = string.Empty;
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
                _titleText.text = "Initial Parameter Values";

                _addButton = new DropdownField();
                _addButton.value = "Add";
                _addButton.style.flexGrow = 1;
                _addButton.style.marginLeft = 0;

                baseFieldLayout.Add(labelArea);
                baseFieldLayout.Add(inputArea);

                labelArea.Add(_titleText);
                inputArea.Add(_addButton);
                _addButton.RegisterCallback<MouseDownEvent>(_ => DrawAddButton(_addButton.worldBound));

                NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

                return new Tuple<VisualElement, Foldout, DropdownField>(baseFieldLayout, _titleText, _addButton);
            }

            public void CalculateEnableAddButton()
            {
                _addButton.SetEnabled(_missingParameters.Count > 0);
            }

            public void SetOpenParameterArea(bool open)
            {
                _titleText.value = open;
                _parameterArea.SetActive(open);
            }

            public void RefreshPropertyRecords(EditorEventRef eventRef)
            {
                _propertyRecords.Clear();

                SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

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
                                new PropertyRecord() {
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

                var baseField = new SimpleBaseField {
                    Label = record.Name,
                    style = {
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

                        var floatSlider = new Slider(record.ParamRef.Min, record.ParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = value
                        };

                        foreach (SerializedProperty property in record.ValueProperties)
                            floatSlider.value = property.floatValue;

                        baseField.contentContainer.Add(floatSlider);

                        floatSlider.RegisterValueChangedCallback(evt => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;
                        });

                        break;
                    case ParameterType.Discrete:
                        var intSlider = new SliderInt((int)record.ParamRef.Min, (int)record.ParamRef.Max) {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            showInputField = true,
                            value = (int)value
                        };

                        baseField.contentContainer.Add(intSlider);

                        intSlider.RegisterValueChangedCallback(evt => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = evt.newValue;
                        });

                        break;
                    case ParameterType.Labeled:
                        var dropdown = new DropdownField {
                            style = {
                                marginLeft = 0f,
                                flexGrow = 1f
                            },
                            choices = record.ParamRef.Labels.ToList(),
                            index = (int)value
                        };

                        baseField.contentContainer.Add(dropdown);

                        dropdown.RegisterValueChangedCallback(_ => {
                            foreach (SerializedProperty property in record.ValueProperties)
                                property.floatValue = dropdown.index;
                        });

                        break;
                }

                var btn = new Button {
                    text = "Remove",
                    style = {
                        marginRight = 0f
                    }
                };

                baseField.contentContainer.Add(btn);

                btn.clicked += () => {
                    DeleteParameter(record.Name);
                    DrawValues(true);
                };

                return baseField;
            }

            private void DrawAddButton(Rect position)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("All"), false, () => {
                    foreach (EditorParamRef parameter in _missingParameters)
                        AddParameter(parameter);

                    DrawValues(true);
                    SetOpenParameterArea(true);
                });

                menu.AddSeparator(string.Empty);

                foreach (EditorParamRef parameter in _missingParameters)
                {
                    menu.AddItem(new GUIContent(parameter.Name), false,
                        (userData) => {
                            AddParameter(userData as EditorParamRef);

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
                    SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    _serializedTargets.ApplyModifiedProperties();
                }
            }

            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = _serializedTargets.FindProperty("Params");

                foreach (SerializedProperty child in paramsProperty)
                {
                    string paramName = child.FindPropertyRelative("Name").stringValue;
                    if (paramName == name)
                    {
                        child.DeleteCommand();
                        break;
                    }
                }

                _serializedTargets.ApplyModifiedProperties();
            }
        }
    }
}
#endif
