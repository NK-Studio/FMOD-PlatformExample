using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    using static ActiveEditorTracker;
    using static EditorGUIUtility;

    public partial class Inspector
    {
        float halfCurrentView
        {
            get => currentViewWidth * 0.5f;
        }

        int selectedCount
        {
            get => selection.Count;
        }

        static int editorStyle
        {
            get => EditorPrefs.GetInt("SI_Style", 0);
            set => EditorPrefs.SetInt("SI_Style", value);
        }

        static int editorButtonStyle
        {
            get => EditorPrefs.GetInt("SI_activeButtonStyle", 0);
            set => EditorPrefs.SetInt("SI_activeButtonStyle", value);
        }

        static bool drawMaterialInspector
        {
            get => EditorPrefs.GetBool("SI_DrawMaterial", true);
            set => EditorPrefs.SetBool("SI_DrawMaterial", value);
        }

        static bool displayReference
        {
            get => EditorPrefs.GetBool("SI_Ref");
            set => EditorPrefs.SetBool("SI_Ref", value);
        }

        static bool displayName
        {
            get => EditorPrefs.GetBool("SI_Name");
            set => EditorPrefs.SetBool("SI_Name", value);
        }

        public static GUIStyle activeButtonStyle
        {
            get
            {
                SmartButtonStyle enumValue = (SmartButtonStyle)editorButtonStyle;

                string value = enumValue.ToString();
                value = value.Replace("_", " ");

                return new GUIStyle(value);
            }
        }

        static GUIStyle activeStyle
        {
            get
            {
                SmartEditorStyles enumValue = (SmartEditorStyles)editorStyle;

                if (enumValue == SmartEditorStyles.None)
                {
                    return GUIStyle.none;
                }

                string value = enumValue.ToString();
                value = value.Replace("_", " ");

                return new GUIStyle(value);
            }
        }

        Event e
        {
            get => Event.current;
        }

        Editor[] editors
        {
            get => sharedTracker.activeEditors;
        }

        GameObject[] gameObjects
        {
            get => Selection.gameObjects;
        }

        Object[] objects
        {
            get => Selection.objects;
        }
    }
}
