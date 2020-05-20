using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameplayIngredients.Editor
{
    public class GameplayIngredientsAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Contains(WelcomeScreen.kSettingsAssetPath))
            {
                Debug.Log("Imported GameplayIngredientsSettings");
                WelcomeScreen.Reload();
            }
        }
    }
}
