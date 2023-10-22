using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NKStudio
{
    [CustomEditor(typeof(Folder))]
    [CanEditMultipleObjects]
    public class FolderEditor : Editor
    {
        private void ApplyIcon()
        {
            string iconName = Prefix + "Folder Icon";
            Texture2D icon = EditorGUIUtility.FindTexture(iconName);
            EditorGUIUtility.SetIconForObject(target, icon);
        }

        public override VisualElement CreateInspectorGUI()
        {
            ApplyIcon();
            VisualElement root = new VisualElement();
            
            PropertyField behaviourField = new();
            behaviourField.bindingPath = "behaviour";
            behaviourField.tooltip = "PlayOnDestroy로 설정되면 Runtime시 폴더를 파괴하고, 자식들을 외부로 노출합니다.";
            root.Add(behaviourField);
            
            HelpBox helpBox = new HelpBox();
            helpBox.messageType = HelpBoxMessageType.Info;
            string msg = Application.systemLanguage == SystemLanguage.Korean
                ? "프레임 최적화를 위해 빌드 및 런타임 시 폴더 구조를 깨뜨리고, 자식들을 외부로 빼냅니다."
                : "To optimize the frame, the folder structure is broken at build and runtime, and the children are taken out.";
            helpBox.text = msg;
            root.Add(helpBox);
            return root;
        }

        private static string Prefix => EditorGUIUtility.isProSkin ? "d_" : "";
    }

}
