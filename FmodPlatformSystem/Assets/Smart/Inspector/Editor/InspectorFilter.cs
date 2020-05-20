using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using Object = UnityEngine.Object;
    using static EditorGUILayout;
    using static EditorGUISmart;

    public partial class Inspector
    {
        void INIT_FILTERS()
        {
            multiEditing = false;
            hasGameObject = false;
            hasAssetImporter = false;
            multipleTypeSelection = MultipleTypeSelection();
        }

        void FILTER()
        {
            if (open || multipleTypeSelection) { return; }
            
            for(int i = 0; i < editors.Length; i++)
            {
                // Filter Internal
                FilterInternal(editors[i]);
                // Break
                if (open) { break; }
            }

            DRAW_MULTI_EDITING();
        }
        
        void FilterInternal(Editor editor)
        {
            if(!AssertEditor(editor))
            {
                return;
            }

            Object target = editor.target;

            if(null == target)
            {
                return;
            }

            GL.Space(0);

            //GL.Label(editor.target.GetType().ToString(), "PreButton");

            if (target is GameObject)
            {
                FilterGameObject(editor);

                return;
            }
            
            if (target is Component)
            {
                FilterComponent1(editor);

                return;
            }
            
            if (hasGameObject && target is Material)
            {
                FilterMaterial(editor);

                return;
            }
            
            FilterAssetImporter(editor);
        }

        void FilterGameObject(Editor editor)
        {
            BeginBigHeader();
            editor.DrawHeader();
            EndBigHeader();
            SETTINGS(false);
        }

        void FilterComponent1(Editor editor)
        {
            Object target = editor.target;

            if (target is ParticleSystemRenderer)
            {
                return;
            }

            if (MultiEditComponent(target))
            {
                multiEditing = true;

                return;
            }

            FilterComponent2(editor);
        }

        void FilterComponent2(Editor editor)
        {
            if (SearchFilter(editor)) { return; }

            editorRect = BeginVertical(EDITOR_STYLE(editor));
            EDITOR_TITLE(editor);
            EDITOR_FIELDS(editor);
            EDITOR_OPEN(editor);
            EndVertical();

            EDITOR_EVENTS(editor, editorRect);
        }

        void FilterMaterial(Editor editor)
        {
            if (!drawMaterialInspector) { return; }

            BeginVertical(EDITOR_STYLE(editor));
            EDITOR_TITLE(editor);
            EDITOR_FIELDS(editor);
            EDITOR_OPEN(editor);
            EndVertical();
        }

        void FilterAssetImporter(Editor editor)
        {
            string name = editor.GetType().Name;

            if (editor.target is AssetImporter && name == "GenericInspector") { return; }

            BeginVertical();
            BeginBigHeader();
            editor.DrawHeader();
            EndBigHeader();

            editor.OnInspectorGUI();
            EndVertical();
        }

        //
        // Util
        //

        bool MultiEditComponent(Object target)
        {
            for(int i = 0; i < gameObjects.Length; i++)
            {
                if(!gameObjects[i].GetComponent(target.GetType()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
