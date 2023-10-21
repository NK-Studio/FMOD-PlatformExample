#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using FMODUnity;
using NKStudio.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace FMODPlus
{
    [CustomEditor(typeof(CommandSender))]
    public class EventCommandSenderEditor : Editor
    {
        private CommandSender _commandSender;
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
        private SerializedProperty _sendOnAwake;
        private SerializedProperty _localKeyList;
        private SerializedProperty _audioStyle;
        private VisualElement _root;

        /// <summary>
        /// Structure that stores Property Name
        /// </summary>
        private struct PropNames
        {
            public const string Clip = "Clip";
            public const string Clips = "ClipList";
            public const string List = "EventRefList";
            public const string Key = "Key";
            public const string Value = "Value";
            public const string Path = "Path";
            public const string Params = "Params";
            public const string Name = "Name";
            public const string KeyList = "keyList";
            public const string UseGlobalKeyList = "useGlobalKeyList";
            public const string Parameter = "Parameter";
            public const string Source = "audioSource";
            public const string BehaviourStyle = "BehaviourStyle";
            public const string Fade = "Fade";
            public const string SendOnAwake = "SendOnAwake";
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

        private Button _parameterSendButton;
        private Foldout _titleToggleLayout;
        private ObjectField _audioSourceField;
        private ObjectField _localKeyListField;
        private VisualElement _parameterArea;
        private DropdownField _addButton;
        private PropertyField _clipField;
        private PropertyField _keyField;
        private PropertyField _useGlobalKeyListField;
        private PropertyField _clipStyleField;
        private PropertyField _audioStyleField;
        private PropertyField _behaviourStyleField;
        private PropertyField _fadeField;
        private PropertyField _globalParameterField;
        private PropertyField _valueField;
        private PropertyField _sendOnStart;

        private VisualElement _line;
        private VisualElement _initializeField;
        private VisualElement _notFoundField;

        private HelpBox _fadeHelpBox;
        private HelpBox _helpBox;


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
            FMODPlusEditorUtility.ApplyIcon(darkIcon, whiteIcon, studioListener);

            string boxGroupSheet = AssetDatabase.GUIDToAssetPath("5600a59cbafd24acf808fa415167310e");
            _boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(boxGroupSheet);

            string buttonStyleSheetPath = AssetDatabase.GUIDToAssetPath("db197c96211fc47319d2b84dcd02aacd");
            _buttonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(buttonStyleSheetPath);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _commandSender = (CommandSender)target;
            FindProperty();
            MakeElement();
            BindElement();
            RegisterCallback();
            InitControll();
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
            _sendOnAwake = serializedObject.FindProperty(PropNames.SendOnAwake);
            _localKeyList = serializedObject.FindProperty(PropNames.KeyList);
            _audioStyle = serializedObject.FindProperty(PropNames.AudioStyle);
        }

        /// <summary>
        /// UI 요소를 그려냅니다.
        /// </summary>
        private void MakeElement()
        {
            _root = new();
            _root.styleSheets.Add(_boxGroupStyle);
            _root.styleSheets.Add(_buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            VisualElement root2 = new();

            _behaviourStyleField = new();
            _behaviourStyleField.BindProperty(_behaviourStyle);

            _audioSourceField = new() {
                objectType = typeof(FMODAudioSource),
                bindingPath = "audioSource"
            };
            _audioSourceField.AddToClassList("unity-base-field__aligned");

            _clipField = new();
            _clipField.BindProperty(_clip);
            
            _useGlobalKeyListField = new();
            _useGlobalKeyListField.BindProperty(_useGlobalKeyList);

            _clipStyleField = new();
            _clipStyleField.BindProperty(_clipStyle);

            _keyField = new();
            _keyField.BindProperty(_key);
            
            _globalParameterField = new();
            _globalParameterField.BindProperty(_parameter);

            _valueField = new();
            _valueField.BindProperty(_value);

            _fadeField = new();
            _fadeField.BindProperty(_fade);

            _sendOnStart = new();
            _sendOnStart.BindProperty(_sendOnAwake);
            
            _fadeHelpBox = new HelpBox();
            _fadeHelpBox.messageType = HelpBoxMessageType.Info;
            
            _helpBox = new();
            _helpBox.ElementAt(0).style.flexGrow = 1;
            _helpBox.messageType = HelpBoxMessageType.Error;
            _helpBox.style.marginTop = 6;
            _helpBox.style.marginBottom = 6;

            _localKeyListField = new();
            _localKeyListField.objectType = typeof(LocalKeyList);
            _localKeyListField.BindProperty(_localKeyList);
            _localKeyListField.AddToClassList("unity-base-field__aligned");

            Color lineColor = Color.black;
            lineColor.a = 0.4f;
            _line = FMODPlusEditorUtility.Line(lineColor, 1.5f, 4f, 3f);

            _audioStyleField = new();
            _audioStyleField.BindProperty(_audioStyle);

            _parameterArea = new();
            _parameterArea.style.marginLeft = 15;
            _parameterArea.SetActive(false);

            _parameterSendButton = new();
            _parameterSendButton.clicked += () => _commandSender.SendCommand();
            _parameterSendButton.AddToClassList("ButtonStyle");

            (VisualElement initializeField, Foldout titleToggleLayout, DropdownField addButton) =
                _parameterValueView.InitParameterView(_parameterArea, _commandSender);

            _initializeField = initializeField;
            _titleToggleLayout = titleToggleLayout;
            _addButton = addButton;

            _notFoundField = FMODPlusEditorUtility.CreateNotFoundField();
            VisualElement eventSpace = FMODPlusEditorUtility.Space(5f);

            _root.Add(root0);
            _root.Add(FMODPlusEditorUtility.Space(5));
            root0.Add(_behaviourStyleField);
            root0.Add(_audioSourceField);

            _root.Add(root1);
            root1.Add(_clipStyleField);
            root1.Add(_line);
            root1.Add(_useGlobalKeyListField);
            root1.Add(_localKeyListField);
            root1.Add(_audioStyleField);
            root1.Add(_clipField);
            root1.Add(_keyField);
            root1.Add(_initializeField);

            _titleToggleLayout.Close();
            _parameterValueView.DrawValues(true);

            root1.Add(_globalParameterField);
            root1.Add(_valueField);
            root1.Add(_notFoundField);
            root1.Add(_helpBox);
            root1.Add(_parameterArea);
            root1.Add(_fadeField);
            root1.Add(_sendOnStart);
            root1.Add(_fadeHelpBox);

            _root.Add(root2);
            root2.Add(eventSpace);

            _root.Add(_parameterSendButton);
        }

        /// <summary>
        /// 요소에 데이터를 바인딩합니다.
        /// </summary>
        private void BindElement()
        {
            _audioSourceField.label = "Audio Source";
            
            _clipField.label = "Event";
            
            _keyField.label = "Event Key";
            
            string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
                ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
                : "Fade function requires AHDSR module to work.";
            _fadeHelpBox.text = appSystemLanguage;
            
            _localKeyListField.label = "Key List";
            
            _parameterArea.name = "ParameterArea";
            
            _parameterSendButton.text = "Send Parameter";
        }

        #region RegisterCallback
        /// <summary>
        /// 요소에 Callback을 등록합니다.
        /// </summary>
        private void RegisterCallback()
        {
            // 오디오 소스 값이 변경되면 콜백이 호출됩니다.
            _oldAudioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
            _audioSourceField.RegisterValueChangedCallback(evt => {
                if (_oldAudioSource != (FMODAudioSource)evt.newValue)
                {
                    Debug.Log("오디오 소스 오브젝트 필드 값이 변경됨");
                    _oldAudioSource = (FMODAudioSource)evt.newValue;
                }
            });

            // 오디오 소스 경로 값이 변경됨
            _oldTargetPath = string.Empty;
            RegisterAudioSourcePathValueChange(evt => {
                if (string.IsNullOrEmpty(evt))
                    _parameterValueView.Dispose(true);

                // 오디오 소스가 변경되면 클립과 키를 초기화합니다.
            });

            // 제목 토글 값이 변경되면 콜백이 호출됩니다.
            _titleToggleLayout.RegisterValueChangedCallback(_ => {
                Debug.Log("타이틀을 토글하였음");
            });

            // 클립 값이 변경되면 콜백이 호출됩니다.
            _clipField.RegisterValueChangeCallback(_clip, _oldPath, _ => {
                _parameterValueView.Dispose();

                Debug.Log("클립 필드 내용을 변경함");
            });

            // 키 값이 변경되면 콜백이 호출됩니다.
            _oldKey = _key.stringValue;
            _keyField.RegisterValueChangeCallback(evt => {
                if (_oldKey != evt.changedProperty.stringValue)
                {
                    Debug.Log("키 값을 변경함");
                    _oldKey = evt.changedProperty.stringValue;
                }
            });

            // A callback is called when the use global key list value changes.
            _oldUseGlobalKeyList = _useGlobalKeyList.boolValue;
            _useGlobalKeyListField.RegisterValueChangeCallback(evt => {
                if (_oldUseGlobalKeyList != evt.changedProperty.boolValue)
                {
                    Debug.Log("글로벌 키 리스트 변경");
                    _oldUseGlobalKeyList = evt.changedProperty.boolValue;
                    _parameterValueView.Dispose(true);
                }
            });

            // A callback is called when the clip style value changes.
            _oldClipStyle = (ClipStyle)_clipStyle.enumValueIndex;
            _clipStyleField.RegisterValueChangeCallback(evt => {
                if (_oldClipStyle != (ClipStyle)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    _parameterArea.Clear();
                    _addButton.SetEnabled(true);

                    // 새롭게 그리는 처리를 해야함
                    _oldClipStyle = (ClipStyle)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the audio style value changes.
            _oldAudioStyle = (AudioType)_audioStyle.enumValueIndex;
            _audioStyleField.RegisterValueChangeCallback(evt => {
                if (_oldAudioStyle != (AudioType)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    _parameterArea.Clear();
                    _addButton.SetEnabled(true);

                    //ControlField(elements);
                    // 컨트롤 필드 처리가 필요함
                    _oldAudioStyle = (AudioType)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the behaviour style value changes.
            _oldCommandBehaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;
            _behaviourStyleField.RegisterValueChangeCallback(evt => {
                var behaviourStyle = (CommandBehaviourStyle)evt.changedProperty.enumValueIndex;
                if (_oldCommandBehaviourStyle != behaviourStyle)
                {
                    switch (behaviourStyle)
                    {
                        case CommandBehaviourStyle.Play:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.Stop:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.Parameter:
                            _parameterValueView.Dispose(true);
                            break;
                        case CommandBehaviourStyle.GlobalParameter:
                            _parameterValueView.Dispose(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    //ControlField(elements); 컨트롤 필드 처리가 필요함
                    _oldCommandBehaviourStyle = behaviourStyle;
                }
            });

            // A callback is called when the fade value changes.
            _oldFade = _fade.boolValue;
            _fadeField.RegisterValueChangeCallback(evt => {
                if (_oldFade != evt.changedProperty.boolValue)
                {
                    //ControlField(elements); 컨트롤 필드 처리가 필요함
                    _oldFade = evt.changedProperty.boolValue;
                }
            });

            // A callback is requested when the local key list changes.
            _oldLocalKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
            _localKeyListField.RegisterValueChangedCallback(evt => {
                if (_oldLocalKeyList != (LocalKeyList)evt.newValue)
                {
                    //ControlField(elements); 컨트롤 필드 처리가 필요함
                    _oldLocalKeyList = (LocalKeyList)evt.newValue;
                }
            });

            // A callback is called when the global parameter value changes.
            _globalParameterField.RegisterValueChangeCallback(_parameter, _oldParameterPath,
                _ => {
                    _value.floatValue = 0f;
                    serializedObject.ApplyModifiedProperties();
                    //ControlField(elements);  컨트롤 필드 처리가 필요함
                });
        }

        /// <summary>
        /// Check whether the AudioSource Path has changed.
        /// </summary>
        /// <param name="callback">Callback requested when value changes</param>
        private void RegisterAudioSourcePathValueChange(Action<string> callback)
        {
            _root.schedule.Execute(() => {
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
        #endregion

        private void InitControll()
        {
            // params 값이 있다면, 파라미터 창을 오픈합니다.
            if (_params.arraySize > 0)
                _parameterValueView.SetOpenParameterArea(true);

            // 요소를 전부 비활성화 처리
            DisalbeAll();

            var behaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;

            switch (behaviourStyle)
            {
                case CommandBehaviourStyle.Play :
                    _clipField.label = "Event";
                    _titleToggleLayout.text = "Override Init Parameter";
                    _parameterArea.style.marginLeft = 15;
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);

                    _clipStyleField.SetActive(true);
                    ClipStyle clipStyle = (ClipStyle)_clipStyle.enumValueIndex;
                    switch (clipStyle)
                    {
                        case ClipStyle.EventReference:
                            _clipField.SetActive(true);
                            HandleEventRef(_clipPath.stringValue);
                            break;
                        case ClipStyle.Key:
                            _useGlobalKeyListField.SetActive(true);
                            HandleKey(null, () =>
                                _notFoundField.SetActive(true));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case CommandBehaviourStyle.Stop:
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);

                    if (_audioSource.objectReferenceValue != null)
                    {
                        _fadeField.SetActive(true);
                        _sendOnStart.SetActive(true);
                    }
                    else
                    {
                        HandleMessage(
                            "FMOD Audio Source가 연결되어 있지 않습니다.",
                            "FMOD Audio Source is not connected.");

                        _parameterValueView.Dispose();
                    }

                    if (_fade.boolValue)
                        _fadeHelpBox.SetActive(true);

                    break;
                case CommandBehaviourStyle.Parameter:
                    _titleToggleLayout.text = "Override Parameter";
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);
                    _parameterSendButton.SetActive(true);

                    var targetAudioSource = (FMODAudioSource)_audioSource.objectReferenceValue;
                    if (targetAudioSource != null)
                    {
                        // 클립 입력을 요구하지 않음(clipField.SetActive(true);)
                        HandleEventRef(targetAudioSource.clip.Path);
                    }
                    else
                    {
                        HandleMessage(
                            "FMOD Audio Source가 연결되어 있지 않습니다.",
                            "FMOD Audio Source is not connected.");

                        _parameterValueView.Dispose();
                    }
                    break;
                case CommandBehaviourStyle.GlobalParameter:
                    _behaviourStyleField.SetActive(true);
                    _globalParameterField.SetActive(true);

                    EditorParamRef editorParamRef = EventManager.ParamFromPath(_parameter.stringValue);
                    if (editorParamRef != null)
                    {
                        _parameterValueView.DrawGlobalValues(true);
                        _parameterArea.SetActive(true);
                        _sendOnStart.SetActive(true);
                        _parameterArea.style.marginLeft = 0;
                    }
                    else
                    {
                        HandleMessage( "연결된 이벤트 주소가 유효하지 않습니다.", "The connected event address is invalid.");
                        _notFoundField.SetActive(true);
                    }

                    _parameterSendButton.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshParameterSendButton();
        }

        // UI를 전부 안보이도록 처리합니다.
        private void DisalbeAll()
        {
            _audioSourceField.SetActive(false);
            _clipField.SetActive(false);
            _clipStyleField.SetActive(false);
            _keyField.SetActive(false);
            _useGlobalKeyListField.SetActive(false);
            _localKeyListField.SetActive(false);
            _audioStyleField.SetActive(false);
            _globalParameterField.SetActive(false);
            _valueField.SetActive(false);
            _fadeField.SetActive(false);
            _parameterArea.SetActive(false);
            _parameterSendButton.SetActive(false);
            _behaviourStyleField.SetActive(false);
            _sendOnStart.SetActive(false);
            _fadeHelpBox.SetActive(false);
            _helpBox.SetActive(false);
            _initializeField.SetActive(false);
            _notFoundField.SetActive(false);
            _line.SetActive(false);
        }

        private void HandleKey(Action successAction = null, Action failAction = null)
        {
            // When using a global key list
            if (_useGlobalKeyList.boolValue)
            {
                _line.SetActive(true);
                _keyField.SetActive(true);
                _audioStyleField.SetActive(true);

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
                        HandleMessage(
                            "연결된 이벤트 주소가 유효하지 않습니다.",
                            "The connected event address is invalid.");

                        failAction?.Invoke();
                        _parameterValueView.Dispose();
                    }
                }
                else
                {
                    HandleMessage(
                        "Key가 비어있습니다.",
                        "Key is empty.");

                    failAction?.Invoke();
                    _parameterValueView.Dispose();
                }
            }
            // When using a local key list
            else
            {
                _line.SetActive(true);
                _localKeyListField.SetActive(true);
                if (_localKeyList.objectReferenceValue != null)
                {
                    _keyField.SetActive(true);

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
                            _parameterArea.style.marginLeft = 0;
                            HandleEvent(existEvent);
                            successAction?.Invoke();
                        }
                        else
                        {
                            HandleMessage(
                                "연결된 이벤트 주소가 유효하지 않습니다.",
                                "The connected event address is invalid.");

                            failAction?.Invoke();
                            _parameterValueView.Dispose();
                        }
                    }
                    else
                    {
                        HandleMessage(
                            "Key가 비어있습니다.",
                            "Key is empty.");

                        failAction?.Invoke();
                        _parameterValueView.Dispose();
                    }
                }
                else
                {
                    HandleMessage(
                        "Key List가 연결되어있지 않습니다.",
                        "Key List is not connected.");

                    _parameterValueView.Dispose();
                    failAction?.Invoke();
                }
            }
        }

        private void HandleEventRef(string path, Action successAction = null, Action failAction = null)
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
                    HandleMessage(
                        "연결된 이벤트 주소가 유효하지 않습니다.",
                        "The connected event address is invalid.");

                    failAction?.Invoke();
                    _parameterValueView.Dispose();
                }
            }
            else
            {
                HandleMessage(
                    "Event가 비어있습니다.",
                    "Event is empty.");

                failAction?.Invoke();
                _parameterValueView.Dispose();
            }
        }

        private void HandleEvent(EditorEventRef existEvent)
        {
            _parameterValueView.RefreshPropertyRecords(existEvent);
            _parameterValueView.DrawValues();
            _parameterValueView.CalculateEnableAddButton();

            _addButton.SetActive(true);
            _titleToggleLayout.SetActive(true);
            _initializeField.SetActive(true);
            _sendOnStart.SetActive(true);

            var toggleOnOff = _titleToggleLayout.value;
            _parameterArea.SetActive(toggleOnOff);
        }
        
        /// <summary>
        /// HelpBox에 메시지를 표시합니다.
        /// </summary>
        /// <param name="koreanMessage">Korean message</param>
        /// <param name="englishMessage">ENGLISH MESSAGE</param>
        private void HandleMessage(string koreanMessage, string englishMessage)
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean ? koreanMessage : englishMessage;
            _helpBox.text = msg;
            _helpBox.SetActive(true);
        }

        #region Button
        private void RefreshParameterSendButton()
        {
            if (!EditorApplication.isPlaying)
            {
                _parameterSendButton.tooltip = Application.systemLanguage == SystemLanguage.Korean
                    ? "에디터 모드에서는 사용하지 못합니다."
                    : "Can't use in Editor Mode.";
                _parameterSendButton.SetEnabled(false);
            }
            else
            {
                _parameterSendButton.tooltip = "Send Parameter.";
                _parameterSendButton.SetEnabled(true);
            }
        }
        #endregion

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

            private CommandSender _commandSender;
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
                CommandSender commandSender)
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

                FMODPlusEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);

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
                                new PropertyRecord() {
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
                            _serializedObject.ApplyModifiedProperties();
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
                            _serializedObject.ApplyModifiedProperties();
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
                            _serializedObject.ApplyModifiedProperties();
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

            /// <summary>
            /// In the context of the editor, automatically creates parameter fields based on the provided record type and returns created UI field.
            /// </summary>
            /// <param name="editorParamRef">The record to be used as a reference for creating editor parameter fields.</param>
            /// <returns>The created UI field for the editor parameters.</returns>
            private SimpleBaseField AdaptiveParameterField(EditorParamRef editorParamRef)
            {
                var globalParameterLayout = new SimpleBaseField {
                    name = $"{editorParamRef.Name} Field Layout",
                    Label = "Override Value",
                    style = {
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

                        var floatSlider = new Slider(editorParamRef.Min, editorParamRef.Max) {
                            style = {
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
                        var intSlider = new SliderInt((int)editorParamRef.Min, (int)editorParamRef.Max) {
                            style = {
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
                        var dropdown = new DropdownField {
                            style = {
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
                menu.AddItem(new GUIContent("All"), false, () => {
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
                        (userData) => {
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
