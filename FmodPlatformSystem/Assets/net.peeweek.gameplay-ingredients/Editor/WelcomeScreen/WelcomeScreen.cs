using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    internal partial class WelcomeScreen : EditorWindow
    {
        private const string kShowOnStartupPreference = "GameplayIngredients.Welcome.ShowAtStartup";
        private const int WindowWidth = 640;
        private const int WindowHeight = 520;

        private static bool showOnStartup
        {
            get => EditorPrefs.GetBool(kShowOnStartupPreference, true);
            set { if (value != showOnStartup) EditorPrefs.SetBool(kShowOnStartupPreference, value); }
        }

        private static Texture2D header
        {
            get
            {
                if (s_Header == null)
                    s_Header = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.gameplay-ingredient-simple/Editor/WelcomeScreen/welcome-title.png");

                return s_Header;
            }
        }

        private static Texture2D s_Header;

        public static void Reload()
        {
            EditorApplication.update -= ShowAtStartup;
            InitShowAtStartup();
        }

        [InitializeOnLoadMethod]
        private static void InitShowAtStartup()
        {
            if (showOnStartup && !GameplayIngredientsSettings.currentSettings.disableWelcomeScreenAutoStart)
                EditorApplication.update += ShowAtStartup;
        }

        private static void ShowAtStartup()
        {
            if (!Application.isPlaying) ShowFromMenu();
            EditorApplication.update -= ShowAtStartup;
        }

        [MenuItem("Window/Gameplay Ingredients/Welcome Screen", priority = MenuItems.kWindowMenuPriority)]
        private static void ShowFromMenu() => 
            GetWindow<WelcomeScreen>(true, "Gameplay Ingredients");

        private void OnEnable()
        {
            position = new Rect((Screen.width / 2.0f) - WindowWidth / 2, (Screen.height / 2.0f) - WindowHeight / 2, WindowWidth, WindowHeight);
            minSize = new Vector2(WindowWidth, WindowHeight);
            maxSize = new Vector2(WindowWidth, WindowHeight);

            if (!GameplayIngredientsSettings.hasSettingAsset)
                wizardMode = WizardMode.FirstTimeSetup;

            InitTips();
        }

        private void OnDestroy() => 
            EditorApplication.update -= ShowAtStartup;

        private enum WizardMode
        {
            TipOfTheDay = 0,
            FirstTimeSetup = 1,
            About = 2,
        }

        [SerializeField]
        private WizardMode wizardMode = WizardMode.TipOfTheDay;

        private void OnGUI()
        {
            Rect headerRect = GUILayoutUtility.GetRect(640, 215);
            GUI.DrawTexture(headerRect, header);
            using (new GUILayout.AreaScope(new Rect(160, 180, 320, 32)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope(EditorStyles.miniButton))
                    {
                        bool value;

                        value = wizardMode == WizardMode.TipOfTheDay;
                        value = GUILayout.Toggle(value, "  Tips  ", Styles.buttonLeft); 
                        if(value)
                            wizardMode = WizardMode.TipOfTheDay;

                        value = wizardMode == WizardMode.FirstTimeSetup;
                        value = GUILayout.Toggle(value, "  Setup  ", Styles.buttonMid);
                        if (value)
                            wizardMode = WizardMode.FirstTimeSetup;

                        value = wizardMode == WizardMode.About;
                        value = GUILayout.Toggle(value, "  About  ", Styles.buttonRight);
                        if(value)
                            wizardMode = WizardMode.About;
                    }

                    GUILayout.FlexibleSpace();
                }
            }
            GUILayout.Space(8);

            switch (wizardMode)
            {
                case WizardMode.TipOfTheDay:
                    OnTipsGUI();
                    break;
                case WizardMode.FirstTimeSetup:
                    OnSetupGUI();
                    break;
                case WizardMode.About:
                    OnAboutGUI();
                    break;
            }

            Rect line = GUILayoutUtility.GetRect(640, 1);
            EditorGUI.DrawRect(line, Color.black);
            using (new GUILayout.HorizontalScope())
            {
                if(!GameplayIngredientsSettings.currentSettings.disableWelcomeScreenAutoStart)
                    showOnStartup = GUILayout.Toggle(showOnStartup, "다음엔 열지 않기");
                
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close")) 
                    Close();
            }
        }

        private void OnAboutGUI()
        {
            GUILayout.Label("About", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {

                GUILayout.Label("Gameplay Ingredients", Styles.centeredTitle);
                GUILayout.Label(@"(and cooking ustensils)

Unity 프로토 타입 및 게임을 위한 오픈 소스 런타임 및 에디터 툴 세트. 이 스크립트는 Thomas Iche가 유지 보수하며 MIT 라이센스에 따라 단일 패키지로 릴리스됩니다.

<b>This package also makes use of the following third party components:</b>
- <i>Naughty Attributes</i> by Denis Rizov (https://github.com/dbrizov) 
- <i>Fugue Icons</i> by Yusuke Kamiyamane (https://p.yusukekamiyamane.com/).
- <i>Header art background 'Chef's Station'</i> by Todd Quackenbush (https://unsplash.com/photos/x5SRhkFajrA).
", Styles.centeredBody);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("  Github Page  ", Styles.buttonLeft))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients");
                    if (GUILayout.Button("  Report a Bug  ", Styles.buttonMid))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients/issues");
                    if (GUILayout.Button("  LICENSE  ", Styles.buttonRight))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients/blob/master/LICENSE");

                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();
            }
        }

        private static class Styles
        {
            public static readonly GUIStyle buttonLeft;
            public static readonly GUIStyle buttonMid;
            public static readonly GUIStyle buttonRight;
            public static readonly GUIStyle title;
            public static readonly GUIStyle body;

            public static readonly GUIStyle centeredTitle;
            public static readonly GUIStyle centeredBody;
            public static readonly GUIStyle helpBox;

            static Styles()
            {
                buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                buttonMid = new GUIStyle(EditorStyles.miniButtonMid);
                buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
                buttonLeft.fontSize = 12;
                buttonMid.fontSize = 12;
                buttonRight.fontSize = 12;

                title = new GUIStyle(EditorStyles.label) {fontSize = 22};

                centeredTitle = new GUIStyle(title) {alignment = TextAnchor.UpperCenter};

                body = new GUIStyle(EditorStyles.label) {fontSize = 12, wordWrap = true, richText = true};

                centeredBody = new GUIStyle(body) {alignment = TextAnchor.UpperCenter};

                helpBox = new GUIStyle(EditorStyles.helpBox) {padding = new RectOffset(12, 12, 12, 12)};
            }
        }
    }
}
