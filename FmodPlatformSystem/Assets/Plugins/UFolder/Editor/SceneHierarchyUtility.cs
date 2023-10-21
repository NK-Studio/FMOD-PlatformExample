using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NKStudio
{
    public static class SceneHierarchyUtility
    {
        /// <summary>
        /// Hierarchy 뷰에서 대상 GameObject가 열려있는지 확인합니다.
        /// </summary>
        /// <param name="gameObject">열려있는지 확인할 게임 오브젝트</param>
        internal static bool IsExpanded(GameObject gameObject)
        {
            var expandedGameObjects = GetExpandedGameObjects();

            if (expandedGameObjects != null)
                return expandedGameObjects.Contains(gameObject);

            return false;
        }

        /// <summary>
        /// Hierarchy 뷰에서 확장된(즉, 펼쳐진) 모든 GameObject의 목록을 가져옵니다.
        /// </summary>
        private static List<GameObject> GetExpandedGameObjects()
        {
            object sceneHierarchy = HierarchyWindowAdapter.GetFirstHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            if (methodInfo != null)
            {
                object result = methodInfo.Invoke(sceneHierarchy, Array.Empty<object>());

                return (List<GameObject>)result;
            }

            return null;
        }
    }
}
