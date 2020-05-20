using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public static class SceneViewToolbar
    {
        public delegate void SceneViewToolbarDelegate(SceneView sceneView);

        public static event SceneViewToolbarDelegate OnSceneViewToolbarGUI;

        [InitializeOnLoadMethod]
        private static void Initialize() => 
            SceneView.duringSceneGui += OnSceneGUI;

        private static void OnSceneGUI(SceneView sceneView)
        {
            var r = new Rect(Vector2.zero, new Vector2(sceneView.position.width, 24));
            Handles.BeginGUI();
            using (new GUILayout.AreaScope(r))
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    bool play = GUILayout.Toggle(EditorApplication.isPlaying, Contents.playFromHere,
                        EditorStyles.toolbarButton);

                    if (GUI.changed)
                    {
                        if (play)
                            PlayFromHere.Play();
                        else
                            EditorApplication.isPlaying = false;
                    }

                    GUILayout.Space(24);


                    Color backup = GUI.color;

                    bool isLinked = LinkGameView.Active;
                    bool isLocked = LinkGameView.LockedSceneView == sceneView;


                    if (isLinked && isLocked)
                        GUI.color = Styles.lockedLinkColor * 2;
                    else if (isLinked && LinkGameView.CinemachineActive) 
                        GUI.color = Styles.cineColor * 2;

                    isLinked = GUILayout.Toggle(isLinked,
                        LinkGameView.CinemachineActive ? Contents.linkGameViewCinemachine : Contents.linkGameView,
                        EditorStyles.toolbarButton, GUILayout.Width(64));

                    if (GUI.changed)
                    {
                        if (Event.current.shift)
                        {
                            if (!LinkGameView.Active)
                                LinkGameView.Active = true;

                            LinkGameView.CinemachineActive = !LinkGameView.CinemachineActive;
                        }
                        else
                        {
                            LinkGameView.Active = isLinked;
                            LinkGameView.CinemachineActive = false;
                        }
                    }

                    isLocked = GUILayout.Toggle(isLocked, Contents.lockLinkGameView, EditorStyles.toolbarButton);

                    if (GUI.changed)
                    {
                        if (isLocked)
                        {
                            LinkGameView.CinemachineActive = false;
                            LinkGameView.LockedSceneView = sceneView;
                        }
                        else
                            LinkGameView.LockedSceneView = null;
                    }

                    GUI.color = backup;

                    // SceneViewPOV
                    GUILayout.Space(16);
                    if (GUILayout.Button("POV", EditorStyles.toolbarDropDown))
                    {
                        Rect btnrect = GUILayoutUtility.GetLastRect();
                        btnrect.yMax += 17;
                        SceneViewPOV.ShowPopup(btnrect, sceneView);
                    }

                    GUILayout.FlexibleSpace();

                    // Custom Code here
                    OnSceneViewToolbarGUI?.Invoke(sceneView);

                    // Saving Space not to overlap view controls
                    GUILayout.Space(96);
                }
            }

            if (LinkGameView.CinemachineActive)
                DisplayText("CINEMACHINE PREVIEW", Styles.cineColor);
            else if (LinkGameView.Active)
            {
                if (LinkGameView.LockedSceneView == sceneView)
                    DisplayText("GAME VIEW LINKED (LOCKED)", Styles.lockedLinkColor);
                else if (LinkGameView.LockedSceneView == null && SceneView.lastActiveSceneView == sceneView)
                    DisplayText("GAME VIEW LINKED", Color.white);
            }

            Handles.EndGUI();
        }

        private static void DisplayText(string text, Color color)
        {
            Rect r = new Rect(16, 24, 512, 32);
            GUI.color = Color.black;
            GUI.Label(r, text);
            r.x--;
            r.y--;
            GUI.color = color;
            GUI.Label(r, text);
            GUI.color = Color.white;
        }

        private static class Contents
        {
            public static readonly GUIContent playFromHere;
            public static readonly GUIContent lockLinkGameView;
            public static readonly GUIContent linkGameView;
            public static readonly GUIContent linkGameViewCinemachine;

            static Contents()
            {
                lockLinkGameView = new GUIContent(EditorGUIUtility.IconContent("IN LockButton"));
                linkGameView =
                    new GUIContent(
                        EditorGUIUtility.Load("Packages/com.unity.gameplay-ingredient-simple/Icon/GUI/Camera16x16.png") as
                            Texture) {text = " Game"};

                linkGameViewCinemachine =
                    new GUIContent(
                        EditorGUIUtility.Load("Packages/com.unity.gameplay-ingredient-simple/Icon/GUI/Camera16x16.png") as
                            Texture) {text = " Cine"};


                playFromHere = new GUIContent(EditorGUIUtility.IconContent("Animation.Play")) {text = "Play"};
            }
        }

        private static class Styles
        {
            private static GUIStyle toolbar;
            public static readonly Color lockedLinkColor = new Color(0.5f, 1.0f, 0.1f, 1.0f);
            public static readonly Color cineColor = new Color(1.0f, 0.5f, 0.1f, 1.0f);

            static Styles() => toolbar = new GUIStyle(EditorStyles.inspectorFullWidthMargins);
        }
    }
    
    public static class PlayFromHere
    {
        public static void Play() => EditorApplication.isPlaying = true;
    }
}
