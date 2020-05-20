using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;
    using static EditorGUIUtility;

	public partial class Inspector
	{

        void TYPE_SELECTION()
        {
            if (!multipleTypeSelection) { return; }
            Vector2 oldSize = GetIconSize();

            GL.Space(0);
            BeginVertical("In BigTitle", GL.ExpandWidth(true));

            SetIconSize(new Vector2(32, 32));
            GUIContent content = new GUIContent(AssetPreview.GetMiniTypeThumbnail(typeof(Object)));
            content.text = string.Format(" {0} Objects", objects.Length);
            GL.Label(content, EditorStyles.boldLabel);
            GL.Label("Narrow the Selection:");

            SetIconSize(new Vector2(16, 16));

            TypeSelection typeSelection = new TypeSelection(objects);

            
            foreach (KeyValuePair<Type, List<Object>> ts in typeSelection)
            {
                content = new GUIContent();
                content.image = AssetPreview.GetMiniTypeThumbnail(ts.Key);
                content.text = string.Format(" {0} {1}", ts.Value.Count, ts.Key.Name);

                if (GL.Button(content, "In TypeSelection", GL.ExpandWidth(true)))
                {
                    Selection.objects = ts.Value.ToArray();
                    break;
                }
            }
            EndVertical();

            SetIconSize(oldSize);
        }


        bool MultipleTypeSelection()
        {
            Object[] objects = Selection.objects;

            for (int i = 0; i < objects.Length; i++)
            {
                MultipleTypeSelection(objects[i]);
            }

            if (hasGameObject && hasAssetImporter)
            {
                return true;
            }

            if (hasAssetImporter)
            {
                Type type = null;

                for (int i = 0; i < objects.Length; i++)
                {
                    if(null == objects[i]) { continue; }

                    if (null != type && objects[i].GetType() != type)
                    {
                        return true;
                    }

                    type = objects[i].GetType();
                }
            }

            return false;
        }

        void MultipleTypeSelection(Object value)
        {
            if (value is GameObject)
            {
                hasGameObject = true;

                return;
            }

            hasAssetImporter = true;
        }

        //
        // Classes - Util
        //

        public class TypeSelection : Dictionary<Type, List<Object>>
        {
            public TypeSelection(Object[] objects)
            {
                Type type;
                Object current;

                for(int i = 0; i < objects.Length; i++)
                {
                    current = objects[i];
                    type = current.GetType();

                    Add(current, type);
                }
            }

            void Add(Object o, Type type)
            {
                if (!ContainsKey(type))
                {
                    this[type] = new List<Object>();
                }

                this[type].Add(o);
            }
        }
	}
}
