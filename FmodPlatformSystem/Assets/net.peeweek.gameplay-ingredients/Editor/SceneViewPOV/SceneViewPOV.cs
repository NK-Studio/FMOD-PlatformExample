using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace GameplayIngredients.Editor
{
    public class SceneViewPOV : PopupWindowContent
    {
        private static ScenePOVRoot POVRoot;

        private const string kPOVObjectName = "__SceneView__POV__";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            SceneManager.activeSceneChanged -= EditorSceneManager_activeSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;

            SceneManager.activeSceneChanged += EditorSceneManager_activeSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private static void SceneManager_sceneUnloaded(Scene arg0) =>
            CheckPOVGameObjects();

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) =>
            CheckPOVGameObjects();

        private static void EditorSceneManager_activeSceneChanged(Scene arg0, Scene arg1) =>
            CheckPOVGameObjects();

        private static void CheckPOVGameObjects()
        {
            if (Application.isPlaying)
                return;

            var activeScene = SceneManager.GetActiveScene();

            var allRoots = GameObject.FindObjectsOfType<ScenePOVRoot>();
            ScenePOVRoot activePOV = null;
            foreach (var povRoot in allRoots)
                if (povRoot.Scene == activeScene)
                    activePOV = povRoot;

            if (activePOV == null)
            {
                activePOV = CreatePOVRootObject();
                MarkDirtyPOVScene(activePOV);
            }

            POVRoot = activePOV;
        }

        private static ScenePOVRoot CreatePOVRootObject()
        {
            var povRoot = new GameObject(kPOVObjectName) {isStatic = true, hideFlags = HideFlags.HideInHierarchy};
            return povRoot.AddComponent<ScenePOVRoot>();
        }

        public static void ShowPopup(Rect buttonRect, SceneView sceneView) => 
            PopupWindow.Show(buttonRect, new SceneViewPOV(sceneView));

        private readonly SceneView m_SceneView;

        public SceneViewPOV(SceneView sceneView) => 
            m_SceneView = sceneView;

        public override Vector2 GetWindowSize()
        {
            CheckPOVGameObjects();
            return new Vector2(256.0f, 80.0f + POVRoot.AllPOV.Length * 20);
        }

        private static void MarkDirtyPOVScene(ScenePOVRoot root) => 
            EditorSceneManager.MarkSceneDirty(root.gameObject.scene);

        private string m_NewPOVName = "New POV";

        public override void OnGUI(Rect rect)
        {
            if (POVRoot == null)
                CheckPOVGameObjects();

            if (m_SceneView != null)
            {
                Styles.Header("Add Point Of View");
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Name", GUILayout.Width(64));
                    m_NewPOVName = GUILayout.TextField(m_NewPOVName);

                    if (GUILayout.Button("+", GUILayout.Width(32)))
                    {
                        POVRoot.AddPOV(m_SceneView.camera.transform, m_NewPOVName);
                        MarkDirtyPOVScene(POVRoot);
                        m_NewPOVName = "New POV";
                    }
                }

                GUILayout.Space(8);

                var povs = POVRoot.AllPOV;

                Styles.Header("Go to Point Of View");

                foreach (var pov in povs.OrderBy(o => o.name))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(pov.name, EditorStyles.foldout))
                            m_SceneView.AlignViewToObject(pov.transform);

                        if (GUILayout.Button("X", GUILayout.Width(32)))
                        {
                            if (EditorUtility.DisplayDialog("Destroy POV?",
                                "Do you want to destroy this POV: " + pov.gameObject.name + " ?", "Yes", "No"))
                            {
                                GameObject.DestroyImmediate(pov.gameObject);
                                MarkDirtyPOVScene(POVRoot);
                            }
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No POV Root found (Create an Empty Game Object named 'POV_ROOT') or no SceneView currently active",
                    MessageType.Warning);
            }
        }

        private static class Styles
        {
            public static void Header(string name)
            {
                var content = new GUIContent(name);
                var rect = GUILayoutUtility.GetRect(content, EditorStyles.boldLabel);
                rect.xMin = 0;
                rect.xMax = EditorGUIUtility.currentViewWidth;

                EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.1f));
                EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);
            }
        }
    }
}
