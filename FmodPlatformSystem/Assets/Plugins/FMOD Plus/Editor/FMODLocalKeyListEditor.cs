using System.Collections.Generic;
using NKStudio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using ListView = UnityEngine.UIElements.ListView;

namespace FMODPlus
{
    [CustomEditor(typeof(LocalKeyList))]
    public class FMODLocalKeyListEditor : Editor
    {
        //private List<ParameterValueView> _parameterValueView = new();
        private List<KeyAndPath> _oldRefAndKey = new();

        private ReorderableList _reorderableList;
        private ReorderableList _searchList;
        private LocalKeyList _localKeyList;

        private float _lineHeight;
        private float _lineHeightSpacing;
        private string _searchText = string.Empty;

        private const string kClips = "Clips";
        private const string kList = "EventRefAndKeyList";
        private const string kCachedSearchClips = "cachedSearchClips";
        private const string kKey = "Key";
        private const string kGUID = "GUID";
        private const string kShowInfo = "ShowInfo";
        private const string kValue = "Value";
        private const string kPath = "Path";
        private const string kParams = "Params";
        private const string kName = "Name";
        private const string kDefaultKey = "New Key";
        private const string kStageTextField = "StageTextField";

        private SerializedProperty clip;
        private SerializedProperty clipList;
        private SerializedProperty cachedClipList;

        [SerializeField]
        private VisualTreeAsset fmodLocalKeyListUXML;
        [SerializeField]
        private VisualTreeAsset KeyListItemUXML;
        [SerializeField]
        private VisualTreeAsset UnbindItemUXML;

        private VisualElement _root;
        private Label _numberLabel;

        private void FindProperties()
        {
            clip = serializedObject.FindProperty(kClips);
            clipList = clip.FindPropertyRelative(kList);

            _localKeyList = (LocalKeyList)serializedObject.targetObject;
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();

            _root = fmodLocalKeyListUXML.Instantiate();
            var listView = _root.Q<ListView>("KeyList");
            listView.itemsSource = _localKeyList.Clips.EventRefAndKeyList;
            listView.unbindItem = UnbindItem;
            listView.makeItem = MakeItem;
            listView.bindItem = BindItem;
            listView.itemsAdded += AddItem;

            _numberLabel = _root.Q<Label>("Number");
            _numberLabel.text = clipList.arraySize.ToString();

            _root.Q<Button>("reset-button").clicked += OnResetButton;

            return _root;
        }

        private void OnResetButton()
        {
            string title = Application.systemLanguage == SystemLanguage.Korean ? "경고" : "Warning";
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? "정말로 리셋하시겠습니까?"
                : "Do you really want to reset?";
            string yes = Application.systemLanguage == SystemLanguage.Korean ? "넵" : "Yes";
            string no = Application.systemLanguage == SystemLanguage.Korean ? "아니요.." : "No";

            bool result = EditorUtility.DisplayDialog(title, msg, yes, no);

            if (result)
            {
                Undo.RecordObject(target, "Reset List");
                _localKeyList.Clips.Reset();
                //_parameterValueView.Clear();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void UnbindItem(VisualElement arg1, int arg2)
        {
            _numberLabel.text = clipList.arraySize.ToString();
        }

        private VisualElement MakeItem()
        {
            TemplateContainer item = KeyListItemUXML.Instantiate();

            var keyField = item.Q<TextField>("keyField");
            keyField.RegisterValueChangedCallback(evt => {
                if (item.userData != null)
                {
                    int index = (int)item.userData;

                    var keyProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
                    keyProperty.stringValue = evt.newValue;
                    item.Q<Label>("KeyName").text = $"{keyProperty.stringValue} : ";
                }
            });

            var itemFoldout = item.Q<Foldout>("ItemFoldout");
            item.Q<Toggle>("ShowInfo-Toggle").RegisterValueChangedCallback(evt => itemFoldout.value = evt.newValue);

            var baseFieldLayout = item.Q<VisualElement>("BaseFieldLayout");
            var labelArea = item.Q<VisualElement>("LabelArea");
            var inputArea = item.Q<VisualElement>("InputArea");
            NKEditorUtility.ApplyFieldArea(baseFieldLayout, labelArea, inputArea);
            
            
            return item;
        }

        private void BindItem(VisualElement element, int index)
        {
            var keyProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            element.Q<Label>("KeyName").text = $"{keyProperty.stringValue} : ";

            var keyField = element.Q<TextField>("keyField");
            keyField.BindProperty(keyProperty);

            var eventRefProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
            var eventRefField = element.Q<PropertyField>("EventRef-Field");
            eventRefField.BindProperty(eventRefProperty);
            eventRefField.label = "Event";

            var showInfoProperty = clipList.GetArrayElementAtIndex(index).FindPropertyRelative("ShowInfo");
            element.Q<Toggle>("ShowInfo-Toggle").BindProperty(showInfoProperty);
            element.Q<Foldout>("ItemFoldout").value = showInfoProperty.boolValue;

            element.Q<DropdownField>("Add-Button").value = "Add";
            
            element.userData = index;
        }

        private void AddItem(IEnumerable<int> obj)
        {
            _numberLabel.text = clipList.arraySize.ToString();

            int count = 0;

            for (int i = 0; i < clipList.arraySize; i++)
            {
                SerializedProperty key = clipList.GetArrayElementAtIndex(i).FindPropertyRelative(kKey);

                string checkFirstTest;
                if (key.stringValue.Length < kDefaultKey.Length)
                    checkFirstTest = key.stringValue;
                else
                    checkFirstTest = key.stringValue.Substring(0, kDefaultKey.Length);

                if (checkFirstTest == kDefaultKey)
                    count += 1;
            }

            SerializedProperty element = clipList.GetArrayElementAtIndex(clipList.arraySize - 1);

            EventReferenceByKey item = new();
            item.Key = count > 0 ? $"New Key ({count - 1})" : "New Key";
            element.FindPropertyRelative(kKey).stringValue = item.Key;
            element.FindPropertyRelative(kValue).FindPropertyRelative(kPath).stringValue = string.Empty;
            element.FindPropertyRelative(kParams).ClearArray();

            SerializedProperty guid = element.FindPropertyRelative(kValue).FindPropertyRelative("Guid");
            guid.FindPropertyRelative("Data1").intValue = 0;
            guid.FindPropertyRelative("Data2").intValue = 0;
            guid.FindPropertyRelative("Data3").intValue = 0;
            guid.FindPropertyRelative("Data4").intValue = 0;

            element.FindPropertyRelative(kGUID).stringValue = item.GUID;
            serializedObject.ApplyModifiedProperties();
        }
    }
}