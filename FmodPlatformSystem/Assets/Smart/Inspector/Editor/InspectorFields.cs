using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
	public partial class Inspector
    {
        public enum SmartEditorStyles
        {
            None = -1,
            Helpbox = 0,
            Box = 1,
            IN_BigTitle = 2,
            DD_HeaderStyle = 3,
            CN_CountBadge = 4
        }

        public enum SmartButtonStyle
        {
            LargeButton = 0,
            Button = 1,
            MiniButton = 2,
            PreButton = 3,
            ToolbarButton = 4,
            ToolbarButtonFlat = 5,
            Dockarea = 6,
        }

        bool open;
        bool locked;
        bool moveButtons;
        bool stackEditors = true;
        bool displaySettings;
        bool hideUnfiltered;
        bool showSearchBar;
        bool matchWord;
        bool multiEditing;
        bool hasGameObject;
        bool hasAssetImporter;
        bool multipleTypeSelection;
        string filter = "";

        Editor cacheEditor;
        Rect editorRect;
        Rect rect;
        Vector2 scroll;
        SerializedProperty referenceProperty;
        SerializedProperty nameProperty;
        List<Editor> selection = new List<Editor>();
        List<Editor> opened = new List<Editor>();
        List<Object> clipboard = new List<Object>();
        Preview preview;
    }
}
