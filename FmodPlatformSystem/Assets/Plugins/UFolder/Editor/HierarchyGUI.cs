using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NKStudio
{
    [InitializeOnLoad]
    public class HierarchyGUI : Editor
    {
        static HierarchyGUI()
        {
            EditorApplication.hierarchyWindowItemOnGUI += (id, _) => HierarchyWindowItemOnGUI(id);
        }

        /// <summary>
        /// 게임 오브젝트의 태그가 Folder 맞다면 폴더 아이콘을 그려냅니다.
        /// </summary>
        /// <param name="instanceID"></param>
        private static void HierarchyWindowItemOnGUI(int instanceID)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(go == null)
                return;
            
            if (go.CompareTag("Folder"))
                DrawIcon(go);
        }
        
        /// <summary>
        /// 게임 오브젝트의 activeInHierarchy에 따라 아이콘을 그려냅니다.
        /// </summary>
        /// <param name="gameObject">타겟 게임 오브젝트</param>
        private static void DrawIcon(GameObject gameObject)
        {
            if (gameObject.activeInHierarchy)
                ChangeFolderIconActive(gameObject.GetInstanceID(), gameObject);
            else
                ChangeFolderIconDeActive(gameObject.GetInstanceID(), gameObject);
        }

        /// <summary>
        /// 활성 모드에서 폴더 아이콘 변경
        /// </summary>
        private static void ChangeFolderIconActive(int instanceId, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExtended = SceneHierarchyUtility.IsExpanded(obj);
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExtended)
                        iconName = "FolderOpened On Icon";
                    else
                        iconName = "Folder On Icon";
                }
                else
                {
                    if (isExtended)
                        iconName = "FolderOpened Icon";
                    else
                        iconName = "Folder Icon";
                }
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                    iconName = "FolderEmpty On Icon";
                else
                    iconName = "FolderEmpty Icon";
            }

            GUIContent folderIconContent = EditorGUIUtility.IconContent(iconName);
            Texture2D icon = folderIconContent.image as Texture2D;

            HierarchyWindowAdapter.ApplyIconByInstanceId(instanceId, icon);
        }

        /// <summary>
        /// 비활성화 모드에서 폴더 아이콘 변경
        /// </summary>
        private static void ChangeFolderIconDeActive(int instanceId, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExtended = SceneHierarchyUtility.IsExpanded(obj);
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExtended)
                        iconName = "FolderOpened Icon";
                    else
                        iconName = "Folder Icon";
                }
                else
                {
                    if (isExtended)
                        iconName = "FolderOpened On Icon";
                    else
                        iconName = "Folder On Icon";
                }
            }
            else
            {
                //Theme Color
                if (EditorGUIUtility.isProSkin)
                    iconName = "FolderEmpty Icon";
                else
                    iconName = "FolderEmpty On Icon";
            }

            GUIContent folderIconContent = EditorGUIUtility.IconContent(iconName);
            Texture2D icon = folderIconContent.image as Texture2D;

            HierarchyWindowAdapter.ApplyIconByInstanceId(instanceId, icon);
        }
    }
}
