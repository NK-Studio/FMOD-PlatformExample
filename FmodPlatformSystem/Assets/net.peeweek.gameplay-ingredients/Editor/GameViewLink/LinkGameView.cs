using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public static class LinkGameView
    {
        private const string kPreferenceName = "GameplayIngredients.LinkGameView";
        private const string kCinemachinePreferenceName = "GameplayIngredients.LinkGameViewCinemachine";
        private const string kLinkCameraName = "___LINK__SCENE__VIEW__CAMERA___";

        public static bool Active
        {
            get
            {
                // Get preference only when not playing
                if (!Application.isPlaying)
                    m_Active = EditorPrefs.GetBool(kPreferenceName, false);

                return m_Active;
            }

            set
            {
                // Update preference only when not playing
                if(!Application.isPlaying)
                    EditorPrefs.SetBool(kPreferenceName, value);

                m_Active = value;

                if(Camera != null)
                    Camera.SetActive(value);

                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }

        public static bool CinemachineActive
        {
            get
            {
                // Get preference only when not playing
                if (!Application.isPlaying)
                    m_CinemachineActive = EditorPrefs.GetBool(kCinemachinePreferenceName, false);

                return m_CinemachineActive;
            }

            set
            {
                // Update preference only when not playing
                if (!Application.isPlaying)
                    EditorPrefs.SetBool(kCinemachinePreferenceName, value);

                m_CinemachineActive = value;
            }
        }

        private static bool m_Active ;
        private static bool m_CinemachineActive ;
        
        public static SceneView LockedSceneView { get; set; }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            SceneView.duringSceneGui += Update;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            // Reset State when entering editmode or play mode
            if(state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
            {
                Active = Active;

                CinemachineActive = CinemachineActive;
            }
            else // Cleanup before switching state
            {
                if (Camera != null)
                    Object.DestroyImmediate(Camera);
            }
        }

        private const string kMenuPath = "Edit/Link SceneView and GameView %,";
        public const int kMenuPriority = 230;

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = false)]
        private static void Toggle() => Active = !Active;

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = true)]
        private static bool ToggleCheck()
        {
            Menu.SetChecked(kMenuPath, Active);
            return SceneView.sceneViews.Count > 0;
        }

        public static GameObject Camera { get; private set; }

        private static void Update(SceneView sceneView)
        {
            // Check if camera Exists
            if (Camera == null)
            {
                // If disconnected (should not happen, but hey...)
                var result = GameObject.Find(kLinkCameraName);

                Camera = result != null ? result : CreateLinkedCamera();

                if (Application.isPlaying)
                    Active = false;
            }

            if (Active && !CinemachineActive)
            {
                var sv = LockedSceneView == null ? SceneView.lastActiveSceneView : LockedSceneView;
                var sceneCamera = sv.camera;
                var camera = Camera.GetComponent<Camera>();
                var transform = sceneCamera.transform;
                var transform2 = camera.transform;
                bool needRepaint = transform.position != transform2.position
                                   || transform.rotation != transform2.rotation
                                   || sceneCamera.fieldOfView != camera.fieldOfView;

                if(needRepaint)
                {
                    var transform1 = sceneCamera.transform;
                    Camera.transform.position = transform1.position;
                    Camera.transform.rotation = transform1.rotation;
                    camera.orthographic = sceneCamera.orthographic;
                    camera.fieldOfView = sceneCamera.fieldOfView;
                    camera.orthographicSize = sceneCamera.orthographicSize;
                    
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    needRepaint = false;
                }
            }
        }

        private const string kDefaultLinkPrefabName = "LinkGameViewCamera";

        private static GameObject CreateLinkedCamera()
        {
            // Try to find an Asset named as the default name
            var assets = AssetDatabase.FindAssets(kDefaultLinkPrefabName);
            if(assets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                if (obj != null)
                {
                    var instance = Object.Instantiate(obj);
                    if(instance.GetComponent<Camera>() != null)
                    {
                        instance.hideFlags = HideFlags.HideAndDontSave;
                        instance.tag = "MainCamera";
                        instance.name = kLinkCameraName;
                        instance.SetActive(Active);
                        instance.GetComponent<Camera>().depth = int.MaxValue;
                        return instance;
                    }
                    else
                    {
                        Debug.LogWarning("LinkGameView Found default prefab but has no camera!");
                    }
                }
                else
                    Debug.LogWarning("LinkGameView Found default prefab but is not gameobject!");
            }


            // Otherwise ... Create default from code
            var go = new GameObject(kLinkCameraName) {hideFlags = HideFlags.HideAndDontSave, tag = "MainCamera"};
            var camera = go.AddComponent<Camera>();
            camera.depth = int.MaxValue;
            go.SetActive(Active);
            return go;
        }
    }
}

