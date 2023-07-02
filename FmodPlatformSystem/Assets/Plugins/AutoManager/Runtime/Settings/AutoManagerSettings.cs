using System;
using UnityEngine;
using NaughtyAttributes;

namespace AutoManager
{
    [CreateAssetMenu(fileName = "AutoManagerSettings", menuName = "Settings/Auto Manager Settings")]
    public class AutoManagerSettings : ScriptableObject
    {
        public string[] ExcludedManagers => excludedManagers;
        
        public bool ShowDebugCustomManager => showDebugCustomManager;
        
        [BoxGroup("Managers")]
        [SerializeField, ReorderableList, TypeDropDown(typeof(Manager))]
        protected string[] excludedManagers;

        [BoxGroup("Debug")]
        [SerializeField]
        protected bool showDebugCustomManager = true;

        private const string KAssetName = "AutoManagerSettings";

        public static AutoManagerSettings CurrentSettings =>
            HasSettingAsset ? Resources.Load<AutoManagerSettings>(KAssetName) : DefaultSettings;

        public static bool HasSettingAsset => Resources.Load<AutoManagerSettings>(KAssetName) != null;
        
        public static AutoManagerSettings DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                    _defaultSettings = CreateDefaultSettings();
                return _defaultSettings;
            }
        }

        private static AutoManagerSettings _defaultSettings;

        private static AutoManagerSettings CreateDefaultSettings()
        {
            AutoManagerSettings defaultAsset = CreateInstance<AutoManagerSettings>();
    
            defaultAsset.excludedManagers = Array.Empty<string>();
            return defaultAsset;
        }
    }
}
