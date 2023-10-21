using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public static class HierarchyEditorUtility
    {
        internal static IEnumerable<EditorWindow> GetAllWindowsByType(string type) => Resources.FindObjectsOfTypeAll(typeof(EditorWindow)).Where(obj => obj.GetType().ToString() == type).Select(obj => (EditorWindow)obj);
    }
}
