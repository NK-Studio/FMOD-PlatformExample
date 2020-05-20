using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    internal partial class WelcomeScreen : EditorWindow
    {
        private void OnSetupGUI()
        {
            GUILayout.Label("First Time Setup", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                pages[currentPage].Invoke();

                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(currentPage == 0);
                    if (GUILayout.Button("Back")) currentPage--;
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(currentPage == pages.Length - 1);
                    if (GUILayout.Button("Next")) currentPage++;
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        public int currentPage;

        public Action[] pages = new Action[]
        {
            WelcomePage, SettingAssetPage
        };

        private static void WelcomePage()
        {
            GUILayout.Label("Welcome to Gameplay Ingredients !", Styles.title);
            GUILayout.Space(12);
            GUILayout.Label(@"해당 Setup은 프로젝트를 설정하여 스크립트를 사용하고 사용자 정의 할 수 있도록 도와줍니다.", Styles.body);
        }

        public const string kSettingsAssetPath = "Assets/Resources/GameplayIngredientsSettings.asset";

        private static void SettingAssetPage()
        {
            GUILayout.Label("Creating a Settings Asset", Styles.title);
            GUILayout.Space(12);
            GUILayout.Label(@"GameplayIngredients는 다양한 기능을 제공하는 프레임 워크입니다.

이 에셋은 Resources 폴더에 저장해야합니다. 필수 사항은 아니지만 프로젝트 요구에 맞게 수정할 수 있도록 생성하는 것이 좋습니다.
", Styles.body);
            GUILayout.Space(16);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("GameplayIngredientsSettings 에셋 생성"))
                {
                    bool create = true;
                    if(System.IO.File.Exists(Application.dataPath +"/../"+ kSettingsAssetPath))
                    {
                        if (!EditorUtility.DisplayDialog("GameplayIngredientsSettings Asset Overwrite", "A GameplayIngredientsSettings Asset already exists, do you want to overwrite it?", "Yes", "No"))
                            create = false;
                    }

                    if(create)
                    {
                        if(!System.IO.Directory.Exists(Application.dataPath+"/Resources"))
                            AssetDatabase.CreateFolder("Assets", "Resources");

                        GameplayIngredientsSettings asset = Instantiate(GameplayIngredientsSettings.defaultSettings);
                        AssetDatabase.CreateAsset(asset, kSettingsAssetPath);
                        Selection.activeObject = asset;
                    }
                }
            }
        }
    }
}

