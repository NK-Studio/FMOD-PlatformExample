using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;
    using static EditorGUISmart;
    

    public partial class Inspector
    {
        void SETTINGS(bool drawSelectedSettings)
        {
            BeginVertical();
            if (drawSelectedSettings)
            {
                DrawSelectedSettings();
            }

            SETTINGS_INTERNTAL();
            DrawSearchBar();
            //displaySettings = GL.Toggle(displaySettings, "Settings", "Foldout");
            DrawSettingsContent();

            EndVertical();
        }

        bool smartView = true;

        void SETTINGS_INTERNTAL()
        {
            BeginHorizontal();
            smartView = GL.Toggle(smartView, "SmartView", "Radio");
            displaySettings = GL.Toggle(displaySettings, "Settings", "Radio", DontExpandWidth);
            EndHorizontal();
        }

        void DrawSelectedSettings()
        {
            BeginHorizontal(GL.ExpandWidth(true));
            if (GL.Button("Close", "ButtonLeft"))
            {
                CLOSE();

                return;
            }
            stackEditors = GL.Toggle(stackEditors, "Stack", "ButtonMid");
            locked = GL.Toggle(locked, "Lock", "ButtonRight");
            EndHorizontal();
        }

        void DrawSearchBar()
        {
            if (!showSearchBar) { return; }

            BeginHorizontal();
            filter = TextField(GUIContent.none, filter, "SearchTextField");
            if (GL.Button(GUIContent.none, "SearchCancelButton"))
            { filter = ""; EditorGUI.FocusTextInControl(null); }
            EndHorizontal();
        }

        void DrawSettingsContent()
        {
            if (!displaySettings) { return; }

            BeginVertical("DD HeaderStyle");
#if NAME_FIELD
            displayName = Toggle(displayName, "Display Names", "radio");
#endif
            showSearchBar = GL.Toggle(showSearchBar, "Search Bar", "Radio");
            displayReference = GL.Toggle(displayReference, "Display References", "radio");
            drawMaterialInspector = GL.Toggle(drawMaterialInspector, "Display Materials", "Radio");
            moveButtons = GL.Toggle(moveButtons, "Move Buttons", "Radio");
            matchWord = GL.Toggle(matchWord, "Match Word", "Radio");
            hideUnfiltered = GL.Toggle(hideUnfiltered, "Hide Unfiltered", "Radio");
            editorStyle = (int)(SmartEditorStyles)EnumPopup("Style", (SmartEditorStyles)editorStyle);
            editorButtonStyle = (int)(SmartButtonStyle)EnumPopup("Button Style", (SmartButtonStyle)editorButtonStyle);
#if DEV_SETTINGS
            DeleteEditors();
#endif
            EndVertical();
        }
    }
}
