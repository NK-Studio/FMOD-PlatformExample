using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Smart
{
    using static GUILayout;
    using GLE = EditorGUILayout;
    using static EditorGUIUtility;
    using GLU = EditorGUIUtility;

    public static class EditorGUISmart
    {
        public static Color bg;
        public static Color blue1 = new Color(0.6f, 0.9f, 1);
        public static Color blue2 = new Color(0.5f, 0.8f, 1, 0.5f);
        public static Color blue3 = new Color(0.6f, 0.9f, 1, 0.5f);
        public static Color green = new Color(0.6f, 1, 0.4f);

        public static float VerticalSpace
        {
            get => standardVerticalSpacing;
        }
        public static float LineHeight
        {
            get => singleLineHeight;
        }

        public static GUILayoutOption ExpandWidth
        {
            get => ExpandWidth(true);
        }
        public static GUILayoutOption DontExpandWidth
        {
            get => ExpandWidth(false);
        }

        public static GUILayoutOption DontExpandHeight
        {
            get => ExpandHeight(false);
        }

        public static void BeginCenterArea()
        {
            GLE.BeginHorizontal();
            FlexibleSpace();
        }

        public static void EndCenterArea()
        {
            FlexibleSpace();
            GLE.EndHorizontal();
        }

        public static void BeginCenterArea(float width)
        {
            GLE.BeginHorizontal();
            FlexibleSpace();
            GLE.BeginHorizontal(Width(width));
        }

        public static void EndCenterAreaWidth()
        {
            GLE.EndHorizontal();
            FlexibleSpace();
            GLE.EndHorizontal();
        }

        public static void BeginBackgroundColor(Color color)
        {
            bg = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        public static void EndBackgroundColor()
        {
            GUI.backgroundColor = bg;
        }

        private static float labelWidth;
        private static float fieldWidth;


        public static void BeginBigHeader()
        {
            labelWidth = GLU.labelWidth;
            fieldWidth = GLU.fieldWidth;
        }

        public static void EndBigHeader()
        {
            GLU.labelWidth = labelWidth;
            GLU.fieldWidth = fieldWidth;
        }
    }
}