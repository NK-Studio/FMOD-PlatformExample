using System;
using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients
{
    [CreateAssetMenu(fileName = "GameplayIngredientsSettings", menuName = "Settings/Auto Manager Settings")]
    public class GameplayIngredientsSettings : ScriptableObject
    {
        public string[] ExcludedManagers => excludedManagers;
        
        public bool ShowDebugCustomManager => showDebugCustomManager;
        
        [BoxGroup("Managers")]
        [SerializeField, ReorderableList, TypeDropDown(typeof(Manager))]
        protected string[] excludedManagers;

        [BoxGroup("Debug")]
        [SerializeField]
        protected bool showDebugCustomManager = true;

        private const string KAssetName = "GameplayIngredientsSettings";

        public static GameplayIngredientsSettings CurrentSettings =>
            HasSettingAsset ? Resources.Load<GameplayIngredientsSettings>(KAssetName) : DefaultSettings;

        public static bool HasSettingAsset => Resources.Load<GameplayIngredientsSettings>(KAssetName) != null;
        
        public static GameplayIngredientsSettings DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                    _defaultSettings = CreateDefaultSettings();
                return _defaultSettings;
            }
        }

        private static GameplayIngredientsSettings _defaultSettings;

        private static GameplayIngredientsSettings CreateDefaultSettings()
        {
            GameplayIngredientsSettings defaultAsset = CreateInstance<GameplayIngredientsSettings>();
    
            defaultAsset.excludedManagers = Array.Empty<string>();
            return defaultAsset;
        }
    }
}
