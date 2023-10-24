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
    public class CommandSenderEditor : Editor
    {
        private CommandSender _commandSender;
        private ParameterValueView _parameterValueView;
        private EditorEventRef _editorEvent;

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

        private string _oldKey;
        private string _oldPath;
        private string _oldParameterPath;
        private bool _oldUseGlobalKeyList;
        private bool _oldFade;
        private ClipStyle _oldClipStyle;
        private AudioType _oldAudioStyle;
        private LocalKeyList _oldLocalKeyList;
        private CommandBehaviourStyle _oldCommandBehaviourStyle;

        private HelpBox _helpBox;
        private HelpBox _fadeHelpBox;
        private Button _parameterSendButton;
        private Foldout _titleToggleLayout;
        private ObjectField _audioSourceField;
        private ObjectField _localKeyListField;
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
        private VisualElement _root;
        private VisualElement _line;
        private VisualElement _parameterArea;
        private VisualElement _initializeField;
        private VisualElement _notFoundField;

        private StyleSheet _boxGroupStyle;
        private StyleSheet _buttonStyleSheet;

        private void OnEnable()
        {
            _parameterValueView = new ParameterValueView(serializedObject);

            // Event Command Sender
            string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
            Texture2D darkIcon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

            string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
            Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

            string path = AssetDatabase.GUIDToAssetPath("1fa131d2be3348a8bd8940d0037f6bfb");
            MonoScript commandSenderScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            FMODPlusEditorUtility.ApplyIcon(darkIcon, whiteIcon, commandSenderScript);

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
            InitControl();
            return _root;
        }

        /// <summary>
        /// Find the property.
        /// </summary>
        private void FindProperty()
        {
            _params = serializedObject.FindProperty("Params");
            _parameter = serializedObject.FindProperty("Parameter");
            _value = serializedObject.FindProperty("Value");
            _audioSource = serializedObject.FindProperty("audioSource");
            _behaviourStyle = serializedObject.FindProperty("BehaviourStyle");
            _clip = serializedObject.FindProperty("Clip");
            _clipPath = _clip.FindPropertyRelative("Path");
            _useGlobalKeyList = serializedObject.FindProperty("useGlobalKeyList");
            _clipStyle = serializedObject.FindProperty("ClipStyle");
            _key = serializedObject.FindProperty("Key");
            _fade = serializedObject.FindProperty("Fade");
            _sendOnAwake = serializedObject.FindProperty("SendOnAwake");
            _localKeyList = serializedObject.FindProperty("keyList");
            _audioStyle = serializedObject.FindProperty("AudioStyle");
        }

        /// <summary>
        /// UI 요소를 그려냅니다.
        /// </summary>
        private void MakeElement()
        {
            _root = new VisualElement();
            _root.styleSheets.Add(_boxGroupStyle);
            _root.styleSheets.Add(_buttonStyleSheet);

            VisualElement root0 = new();
            root0.AddToClassList("GroupBoxStyle");

            VisualElement root1 = new();
            root1.AddToClassList("GroupBoxStyle");

            VisualElement root2 = new();

            _behaviourStyleField = new PropertyField();
            _behaviourStyleField.BindProperty(_behaviourStyle);

            _audioSourceField = new ObjectField();
            _audioSourceField.objectType = typeof(FMODAudioSource);
            _audioSourceField.BindProperty(_audioSource);
            _audioSourceField.AddToClassList("unity-base-field__aligned");

            _clipField = new PropertyField();
            _clipField.BindProperty(_clip);

            _useGlobalKeyListField = new PropertyField();
            _useGlobalKeyListField.BindProperty(_useGlobalKeyList);

            _clipStyleField = new PropertyField();
            _clipStyleField.BindProperty(_clipStyle);

            _keyField = new PropertyField();
            _keyField.BindProperty(_key);

            _globalParameterField = new PropertyField();
            _globalParameterField.BindProperty(_parameter);

            _valueField = new PropertyField();
            _valueField.BindProperty(_value);

            _fadeField = new PropertyField();
            _fadeField.BindProperty(_fade);

            _sendOnStart = new PropertyField();
            _sendOnStart.BindProperty(_sendOnAwake);

            _fadeHelpBox = new HelpBox();
            _fadeHelpBox.messageType = HelpBoxMessageType.Info;

            _helpBox = new HelpBox();
            _helpBox.ElementAt(0).style.flexGrow = 1;
            _helpBox.messageType = HelpBoxMessageType.Error;
            _helpBox.style.marginTop = 6;
            _helpBox.style.marginBottom = 6;

            _localKeyListField = new ObjectField();
            _localKeyListField.objectType = typeof(LocalKeyList);
            _localKeyListField.BindProperty(_localKeyList);
            _localKeyListField.AddToClassList("unity-base-field__aligned");

            Color lineColor = Color.black;
            lineColor.a = 0.4f;
            _line = FMODPlusEditorUtility.Line(lineColor, 1.5f, 4f, 3f);

            _audioStyleField = new PropertyField();
            _audioStyleField.BindProperty(_audioStyle);

            _parameterArea = new VisualElement();
            _parameterArea.style.marginLeft = 15;
            _parameterArea.SetActive(false);

            _parameterSendButton = new Button();
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

        /// <summary>
        /// 요소에 Callback을 등록합니다.
        /// </summary>
        private void RegisterCallback()
        {
            // 제목 토글 값이 변경되면 콜백이 호출됩니다.
            _titleToggleLayout.RegisterValueChangedCallback(_ => Refresh());

            // 클립 값이 변경되면 콜백이 호출됩니다.
            ClipRegisterValueChangeCallback();


            // 키 값이 변경되면 콜백이 호출됩니다.
            _oldKey = _key.stringValue;
            _keyField.RegisterValueChangeCallback(evt =>
            {
                if (_oldKey != evt.changedProperty.stringValue)
                {
                    Refresh();
                    _oldKey = evt.changedProperty.stringValue;
                }
            });

            // 전역 키 목록 사용 값이 변경되면 콜백이 호출됩니다.
            _oldUseGlobalKeyList = _useGlobalKeyList.boolValue;
            _useGlobalKeyListField.RegisterValueChangeCallback(evt =>
            {
                if (_oldUseGlobalKeyList != evt.changedProperty.boolValue)
                {
                    Refresh();
                    _oldUseGlobalKeyList = evt.changedProperty.boolValue;
                    _parameterValueView.Dispose(true);
                }
            });

            // 클립 스타일 값이 변경되면 콜백이 호출됩니다.
            _oldClipStyle = (ClipStyle)_clipStyle.enumValueIndex;
            _clipStyleField.RegisterValueChangeCallback(evt =>
            {
                if (_oldClipStyle != (ClipStyle)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    _parameterArea.Clear();
                    _addButton.SetEnabled(true);

                    Refresh();

                    // 새롭게 그리는 처리를 해야함
                    _oldClipStyle = (ClipStyle)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the audio style value changes.
            _oldAudioStyle = (AudioType)_audioStyle.enumValueIndex;
            _audioStyleField.RegisterValueChangeCallback(evt =>
            {
                if (_oldAudioStyle != (AudioType)evt.changedProperty.enumValueIndex)
                {
                    _parameterValueView.Dispose();
                    _parameterArea.Clear();
                    _addButton.SetEnabled(true);

                    Refresh();
                    _oldAudioStyle = (AudioType)evt.changedProperty.enumValueIndex;
                }
            });

            // A callback is called when the behaviour style value changes.
            _oldCommandBehaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;
            _behaviourStyleField.RegisterValueChangeCallback(evt =>
            {
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
                        case CommandBehaviourStyle.KeyOff:
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

                    Refresh();
                    _oldCommandBehaviourStyle = behaviourStyle;
                }
            });

            // 페이드 값이 변경되면 콜백이 호출됩니다.
            _oldFade = _fade.boolValue;
            _fadeField.RegisterValueChangeCallback(evt =>
            {
                if (_oldFade != evt.changedProperty.boolValue)
                {
                    Refresh();
                    _oldFade = evt.changedProperty.boolValue;
                }
            });

            // 로컬 키 목록이 변경되면 콜백이 요청됩니다.
            _oldLocalKeyList = (LocalKeyList)_localKeyList.objectReferenceValue;
            _localKeyListField.RegisterValueChangedCallback(evt =>
            {
                if (_oldLocalKeyList != (LocalKeyList)evt.newValue)
                {
                    Refresh();
                    _oldLocalKeyList = (LocalKeyList)evt.newValue;
                }
            });

            // 전역 매개변수 값이 변경되면 콜백이 호출됩니다.
            GlobalParameterFieldRegisterValueChangeCallback();
        }

        /// <summary>
        /// 이벤트 클립 부분이 변경되면 Callback이 호출됩니다.
        /// </summary>
        private void ClipRegisterValueChangeCallback()
        {
            // Init
            _oldPath = _clip.FindPropertyRelative("Path").stringValue;
            _clipField.schedule.Execute(() =>
            {
                if (_oldPath != _clip.FindPropertyRelative("Path").stringValue)
                {
                    _oldPath = _clip.FindPropertyRelative("Path").stringValue;
                    _parameterValueView.Dispose();
                    Refresh();
                }
            }).Every(100);
        }

        /// <summary>
        /// 글로벌 파라미터의 값이 변경되면 동작합니다.
        /// </summary>
        private void GlobalParameterFieldRegisterValueChangeCallback()
        {
            // Init
            _oldParameterPath = _parameter.stringValue;
            _globalParameterField.schedule.Execute(() =>
            {
                if (_oldParameterPath != _parameter.stringValue)
                {
                    _oldParameterPath = _parameter.stringValue;
                    _value.floatValue = 0f;
                    serializedObject.ApplyModifiedProperties();
                    Refresh();
                }
            }).Every(100);
        }

        /// <summary>
        /// 초기 컨트롤
        /// </summary>
        private void InitControl()
        {
            // params 값이 있다면, 파라미터 창을 오픈합니다.
            if (_params.arraySize > 0)
                _parameterValueView.SetOpenParameterArea(true);

            // 요소를 전부 비활성화 처리
            DisableAll();

            var behaviourStyle = (CommandBehaviourStyle)_behaviourStyle.enumValueIndex;

            ClipStyle clipStyle;

            switch (behaviourStyle)
            {
                case CommandBehaviourStyle.Play:
                    _clipField.label = "Event";
                    _titleToggleLayout.text = "Override Init Parameter";
                    _parameterArea.style.marginLeft = 15;
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);

                    _clipStyleField.SetActive(true);
                    clipStyle = (ClipStyle)_clipStyle.enumValueIndex;
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

                    _fadeField.SetActive(true);
                    _sendOnStart.SetActive(true);

                    if (_fade.boolValue)
                        _fadeHelpBox.SetActive(true);

                    break;
                case CommandBehaviourStyle.KeyOff:
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);
                    
                    _sendOnStart.SetActive(true);

                    break;
                case CommandBehaviourStyle.Parameter:
                    _clipField.label = "Event";
                    _titleToggleLayout.text = "Override Parameter";
                    _behaviourStyleField.SetActive(true);
                    _audioSourceField.SetActive(true);
                    _parameterSendButton.SetActive(true);
                    _parameterArea.style.marginLeft = 15;

                    _clipStyleField.SetActive(true);
                    clipStyle = (ClipStyle)_clipStyle.enumValueIndex;
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
                        HandleMessage("연결된 이벤트 주소가 유효하지 않습니다.", "The connected event address is invalid.");
                        _notFoundField.SetActive(true);
                    }

                    _parameterSendButton.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshParameterSendButton();
        }

        /// <summary>
        /// UI를 전부 안보이도록 처리합니다.
        /// </summary>
        private void DisableAll()
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

        /// <summary>
        /// UI를 새로고침 합니다.
        /// </summary>
        private void Refresh()
        {
            DisableAll();
            InitControl();
        }

        /// <summary>
        /// 글로벌 키 리스트가 체크되어있다면, AMB, BGM, SFX 키 리스트를 검사하고,
        /// 존재하면 successAction을 호출하며, 존재하지 않으면 failAction을 호출합니다.
        /// </summary>
        /// <param name="successAction">성공했을 때 추가 동작</param>
        /// <param name="failAction">실패했을 때 추가 동작</param>
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
                            .FindProperty("ClipList")
                            .FindPropertyRelative("EventRefList");

                        foreach (SerializedProperty list in lists)
                        {
                            string targetKey = list.FindPropertyRelative("Key")
                                .stringValue;
                            string targetPath = list.FindPropertyRelative("Value")
                                .FindPropertyRelative("Path")
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

        /// <summary>
        /// pass 값이 비어져 있지 않다면,
        /// 실재로 존재하는 Event인지 검사하고,
        /// 존재하면 successAction을 호출하며, 존재하지 않으면 failAction을 호출합니다.
        /// </summary>
        /// <param name="path">검사할 path</param>
        /// <param name="successAction">존재했을 때 추가 동작</param>
        /// <param name="failAction">존재하지 않았을 때 추가 동작</param>
        private void HandleEventRef(string path, Action successAction = null, Action failAction = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                EditorEventRef existEvent = EventManager.EventFromPath(path);

                if (existEvent != null)
                {
                    _helpBox.SetActive(false);
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

        /// <summary>
        /// 파라미터값을 보여주는 View를 새로고침하고, 새로운 값을 그립니다.
        /// Add Button을 재계산하고, AddButton을 활성화 합니다.
        /// </summary>
        /// <param name="existEvent"></param>
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

        /// <summary>
        /// 전달 버튼을 새로고침합니다.
        /// </summary>
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

        private class ParameterValueView
        {
            // 이것은 현재 선택의 각 객체에 대해 하나의 SerializedObject를 보유합니다.
            private readonly SerializedObject _serializedObject;

            // EditorParamRef에서 현재 선택 항목의 모든 속성에 대한 초기 매개변수 값 속성으로 매핑합니다.
            private readonly List<PropertyRecord> _propertyRecords = new();

            // 현재 이벤트에 있지만 현재 선택의 일부 개체에서 누락된 매개변수는 "추가" 메뉴에 넣을 수 있습니다.
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
                    var useGlobalKeyList = _serializedObject.FindProperty("useGlobalKeyList").boolValue;

                    switch (_commandSender.BehaviourStyle)
                    {
                        case CommandBehaviourStyle.Play:
                        case CommandBehaviourStyle.Parameter:
                            switch (_commandSender.ClipStyle)
                            {
                                case ClipStyle.EventReference:
                                    path = _serializedObject.FindProperty("Clip")
                                        .FindPropertyRelative("Path")
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
                                            .FindProperty("keyList").objectReferenceValue;
                                        
                                        if (!targetKeyList)
                                            return;

                                        SerializedObject targetLocalKeyList = new(targetKeyList);
                                        SerializedProperty lists = targetLocalKeyList.FindProperty("ClipList")
                                            .FindPropertyRelative("EventRefList");
                                        
                                        foreach (SerializedProperty list in lists)
                                        {
                                            string targetKey = list.FindPropertyRelative("Key").stringValue;
                                            string targetPath = list.FindPropertyRelative("Value")
                                                .FindPropertyRelative("Path")
                                                .stringValue;

                                            if (_serializedObject.FindProperty("Key").stringValue == targetKey)
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

                var value = _serializedObject.FindProperty("Value");

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
                    SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

                    int index = paramsProperty.arraySize;
                    paramsProperty.InsertArrayElementAtIndex(index);

                    SerializedProperty arrayElement = paramsProperty.GetArrayElementAtIndex(index);

                    arrayElement.FindPropertyRelative("Name").stringValue = parameter.Name;
                    arrayElement.FindPropertyRelative("Value").floatValue = parameter.Default;

                    _serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// Deletes the parameter.
            /// </summary>
            /// <param name="name">Removes a parameter by its name.</param>
            private void DeleteParameter(string name)
            {
                SerializedProperty paramsProperty = _serializedObject.FindProperty("Params");

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