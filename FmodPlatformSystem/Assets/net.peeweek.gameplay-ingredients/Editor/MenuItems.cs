using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameplayIngredients.Editor
{
    public static class MenuItems
    {
        public const int kWindowMenuPriority = 100;
 
        #region GROUP_UNGROUP

        private const int kGroupMenuIndex = 500;
        private const string kGroupMenuString = "Edit/Group Selected %G";
        private const string kUnGroupMenuString = "Edit/Un-Group Selected %#G";

        [MenuItem(kGroupMenuString, priority = kGroupMenuIndex, validate = false)]
        private static void Group()
        {
            if (Selection.gameObjects.Length <= 1)
                return;

            var selected = Selection.gameObjects;
            Transform parent = selected[0].transform.parent;
            Scene scene = selected[0].scene;

            bool sparseParents = selected.Any(obj => obj.transform.parent != parent || obj.scene != scene);

            if (sparseParents) parent = null;

            Vector3 posSum = selected.Aggregate(Vector3.zero, (current, go) => current + go.transform.position);

            GameObject groupObj = new GameObject("Group");
            groupObj.transform.position = posSum / selected.Length;
            groupObj.transform.parent = parent;
            groupObj.isStatic = true;

            foreach (var go in selected)
                go.transform.parent = groupObj.transform;

            // Expand by pinging the first object
            EditorGUIUtility.PingObject(selected[0]);
            
        }

        [MenuItem(kGroupMenuString, priority = kGroupMenuIndex, validate = true)]
        private static bool GroupCheck() => 
            Selection.gameObjects.Length > 1;


        [MenuItem(kUnGroupMenuString, priority = kGroupMenuIndex+1, validate = false)]
        private static void UnGroup()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var selected = Selection.gameObjects;
            var oldParents = new List<Transform>();
            foreach(var go in selected)
            {
                if(go.transform.parent != null)
                {
                    if(!oldParents.Contains(go.transform.parent))
                        oldParents.Add(go.transform.parent);

                    go.transform.parent = go.transform.parent.parent;
                }
            }
            
            // Cleanup old parents
            var toDelete = (from parent in oldParents let go = parent.gameObject where parent.childCount == 0 && parent.GetComponents<Component>().Length == 1 select go).ToList();

            foreach (var trash in toDelete)
                Object.DestroyImmediate(trash);
            
        }

        [MenuItem(kUnGroupMenuString, priority = kGroupMenuIndex+1, validate = true)]
        private static bool UnGroupCheck() => 
            (Selection.gameObjects.Length > 0);

        #endregion
    }
}
