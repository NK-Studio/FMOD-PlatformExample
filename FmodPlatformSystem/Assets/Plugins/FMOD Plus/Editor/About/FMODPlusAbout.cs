using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMODPlus
{
    public class FMODPlusAbout : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset FMODPlusAboutUXML;
        
        [SerializeField]
        private TextAsset packageJson;

        [MenuItem("FMOD/FMOD Plus/About", false, 2000)]
        public static void Init()
        {
            FMODPlusAbout wnd = GetWindow<FMODPlusAbout>();
            wnd.titleContent = new GUIContent("About");
            wnd.minSize = new Vector2(350, 120);
            wnd.maxSize = new Vector2(350, 120);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = FMODPlusAboutUXML;
            VisualElement container = visualTree.Instantiate();

            Label versionLabel = container.Q<Label>("version-label");
            PackageInfo info = JsonUtility.FromJson<PackageInfo>(packageJson.text);
            
            root.Add(container);
            
            if (info == null)
                return;
            
            versionLabel.text = $"Version : {info.version}";
        }
    }

    [System.Serializable]
    public class PackageInfo
    {
        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string description;
        public List<string> keywords;
        public string type;
    }

}
