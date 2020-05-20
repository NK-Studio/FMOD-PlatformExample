using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients
{
    public class GameplayIngredientsSettings : ScriptableObject
    {
        public string[] excludedeManagers => m_ExcludedManagers;

        public bool disableWelcomeScreenAutoStart => m_DisableWelcomeScreenAutoStart;

        public bool ShowDebugCustomManager => m_ShowDebugCustomManager;
        
        [BoxGroup("Editor")]
        [SerializeField]
        protected bool m_DisableWelcomeScreenAutoStart;

        [BoxGroup("Managers")]
        [SerializeField, ReorderableList, TypeDropDown(typeof(Manager))]
        protected string[] m_ExcludedManagers;

        [BoxGroup("Debug")]
        [SerializeField]
        protected bool m_ShowDebugCustomManager = true;
        
        private const string kAssetName = "GameplayIngredientsSettings";

        public static GameplayIngredientsSettings currentSettings =>
            hasSettingAsset ? Resources.Load<GameplayIngredientsSettings>(kAssetName) : defaultSettings;

        public static bool hasSettingAsset => Resources.Load<GameplayIngredientsSettings>(kAssetName) != null;


        public static GameplayIngredientsSettings defaultSettings
        {
            get
            {
                if (s_DefaultSettings == null)
                    s_DefaultSettings = CreateDefaultSettings();
                return s_DefaultSettings;
            }
        }

        private static GameplayIngredientsSettings s_DefaultSettings;

        private static GameplayIngredientsSettings CreateDefaultSettings()
        {
            var defaultAsset = CreateInstance<GameplayIngredientsSettings>();
    
            defaultAsset.m_ExcludedManagers = new string[0];
            defaultAsset.m_DisableWelcomeScreenAutoStart = false;
            return defaultAsset;
        }
    }
}
