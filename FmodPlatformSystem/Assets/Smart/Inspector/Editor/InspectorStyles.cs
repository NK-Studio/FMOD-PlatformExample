using UnityEngine;

namespace Smart
{
    public partial class Inspector
    {
        public static class Styles
        {
            public static RectOffset zero
            {
                get => new RectOffset(0, 0, 0, 0);
            }

            public static GUIStyle radio
            {
                get
                {
                    GUIStyle style = new GUIStyle("radio");
                    style.overflow = zero;
                    return style;
                }
            }

            public static GUIStyle progressBar
            {
                get
                {
                    GUIStyle style = new GUIStyle("ProgressBarBar");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.padding = new RectOffset(0, 0, 0, 0);

                    return style;
                }
            }

            public static GUIStyle InBigTitlePost
            {
                get
                {
                    GUIStyle style = new GUIStyle("IN BigTitle Post");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.padding = new RectOffset(0, 0, 0, 0);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.stretchWidth = true;
                    //style.stretchHeight = true;
                    style.fontSize = 12;

                    return style;
                }
            }

            public static GUIStyle InBigTitlePostGrey
            {
                get
                {
                    GUIStyle style = new GUIStyle(InBigTitlePost);
                    style.normal.textColor = Color.gray;

                    return style;
                }
            }

            public static GUIStyle boldlabel
            {
                get
                {
                    GUIStyle style = new GUIStyle("boldlabel");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.padding = new RectOffset(0, 0, 0, 0);
                    style.alignment = TextAnchor.MiddleLeft;

                    return style;
                }
            }

            public static GUIStyle textField
            {
                get
                {
                    GUIStyle style = new GUIStyle("textField");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.stretchHeight = true;
                    style.alignment = TextAnchor.MiddleLeft;

                    return style;
                }
            }
        }

        void INIT_BIG_STYLE()
        {
            GUIStyle style = "IN BigTitle";
            style.overflow = Styles.zero;
            style.margin = Styles.zero;
        }
    }
}
