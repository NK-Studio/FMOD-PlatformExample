using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NKStudio
{
    [InitializeOnLoad]
    public static class HierarchyWindowAdapter
    {
        private static readonly FieldInfo SceneHierarchyField;
        private static readonly FieldInfo TreeViewField;
        private static readonly PropertyInfo TreeViewDataProperty;
        private static readonly MethodInfo TreeViewItemsMethod;
        private static double _nextWindowsUpdate;
        private static EditorWindow[] _windowsCache;

        static HierarchyWindowAdapter()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(EditorWindow));
            SceneHierarchyField = assembly.GetType("UnityEditor.SceneHierarchyWindow").GetField("m_SceneHierarchy", BindingFlags.Instance | BindingFlags.NonPublic);
            TreeViewField = assembly.GetType("UnityEditor.SceneHierarchy").GetField("m_TreeView", BindingFlags.Instance | BindingFlags.NonPublic);
            TreeViewDataProperty = assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController").GetProperty("data", BindingFlags.Instance | BindingFlags.Public);
            TreeViewItemsMethod = assembly.GetType("UnityEditor.GameObjectTreeViewDataSource").GetMethod("GetRows", BindingFlags.Instance | BindingFlags.Public);
        }

        private static IEnumerable<EditorWindow> GetAllHierarchyWindows(bool forceUpdate = false)
        {
            if (forceUpdate || _nextWindowsUpdate < EditorApplication.timeSinceStartup)
            {
                _nextWindowsUpdate = EditorApplication.timeSinceStartup + 2.0;
                _windowsCache = HierarchyEditorUtility.GetAllWindowsByType("UnityEditor.SceneHierarchyWindow").ToArray();
            }
            return _windowsCache;
        }

        private static IEnumerable<TreeViewItem> GetTreeViewItems(EditorWindow window)
        {
            object obj1 = SceneHierarchyField.GetValue(window);
            object obj2 = TreeViewField.GetValue(obj1);
            object obj3 = TreeViewDataProperty.GetValue(obj2, null);
            return (IEnumerable<TreeViewItem>)TreeViewItemsMethod.Invoke(obj3, null);
        }

        /// <summary>
        /// 인스턴스ID를 이용하여 아이콘을 적용합니다.
        /// </summary>
        /// <param name="instanceId">아이콘을 적용할 게임 오브젝트 InstanceID</param>
        /// <param name="icon">지정할 아이콘 텍스쳐</param>
        internal static void ApplyIconByInstanceId(int instanceId, Texture2D icon)
        {
            foreach (EditorWindow allHierarchyWindow in GetAllHierarchyWindows())
            {
                TreeViewItem treeViewItem = GetTreeViewItems(allHierarchyWindow).FirstOrDefault(item => item.id == instanceId);
                if (treeViewItem != null)
                {
                    treeViewItem.icon = icon;
                }
            }
        }

        /// <summary>
        /// 첫번째 HierarchyWindow를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        internal static object GetFirstHierarchy()
        {
            foreach (EditorWindow allHierarchyWindow in GetAllHierarchyWindows())
            {
                object obj1 = SceneHierarchyField.GetValue(allHierarchyWindow);
                if (obj1 != null)
                    return obj1;
            }

            return null;
        }
    }
}
