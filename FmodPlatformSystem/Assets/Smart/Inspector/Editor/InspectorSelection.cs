using UnityEditor;

namespace Smart
{
    using UnityEngine;
    using static EditorGUILayout;
    using static EditorGUISmart;

    public partial class Inspector
    {
        void SELECTION()
        {
            if (!open) { return; }

            if (0 == opened.Count)
            {
                CLOSE();

                return;
            }

            SETTINGS(true);

            for (int i = 0; i < opened.Count; i++)
            {
                // Scene Reload can delete our open editors
                if (!AssertEditor(opened[i])) { opened.RemoveAt(i); break; }

                DrawOpened(opened[i]);
            }
        }

        void DrawOpened(Editor editor)
        {
            editor.serializedObject.Update();
            if (stackEditors)
            {
                DrawOpenedStacked(editor);
            }
            else
            {
                DrawOpenedNonStacked(editor);
            }
            editor.serializedObject.Update();

            if (!preview) { return; }

            if (!(editor.target is ParticleSystem)) { return; }
            // Auto update preview if target is ParticleSystem
            preview.Repaint();
        }
        
        void DrawOpenedStacked(Editor editor)
        {
            bool expanded = true;

            if(editor.target is Component)
            {
                expanded = DRAW_EDITOR_TITLE(editor);
            }

            if (searching)
            {
                DrawOpenedManually(editor);
            }
            else
            {
                if(editor.target is Material)
                {
                    if(expanded)
                        DrawOpenedMaterial(editor);
                }
                else
                {
                    if(expanded)
                        editor.OnInspectorGUI();
                }
            }
        }

        void DrawOpenedNonStacked(Editor editor)
        {
            for (int i = 0; i < editor.targets.Length; i++)
            {
                Editor.CreateCachedEditor(editor.targets[i], editor.GetType(), ref cacheEditor);
                cacheEditor.name = "Non-Stacked";

                if (editor.target is Component)
                {
                    InspectorTitlebar(true, cacheEditor);
                }

                Component component = cacheEditor.target as Component;
                GUI.enabled = false;
                ObjectField("GameObject", component.gameObject, typeof(GameObject), true);
                GUI.enabled = true;

                if (searching)
                {
                    DrawOpenedManually(cacheEditor);
                }
                else
                {
                    if (cacheEditor.target is Material)
                    {
                        DrawOpenedMaterial(editor);
                    }
                    else
                    {
                        cacheEditor.OnInspectorGUI();
                    }
                }
            }
        }

        void DrawOpenedManually(Editor editor)
        {
            editor.serializedObject.Update();
            SerializedProperty itr = editor.serializedObject.GetIterator();
            while (itr.NextVisible(true))
            {
                GUI.enabled = itr.name != "m_Script";

                if (SearchProperty(filter, itr.displayName))
                {
                    if (!hideUnfiltered)
                    {
                        BeginHorizontal(Styles.progressBar, DontExpandHeight);
                        PropertyField(itr, true, DontExpandHeight);
                        EndHorizontal();
                    }
                    else
                    {
                        PropertyField(itr, true, DontExpandHeight);
                    }
                }
                else
                {
                    if (!hideUnfiltered)
                    {
                        PropertyField(itr, true);
                    }
                }
                GUI.enabled = true;
            }

            editor.serializedObject.ApplyModifiedProperties();
        }

        void DrawOpenedMaterial(Editor editor)
        {
            editor = editor as MaterialEditor;
            editor.DrawHeader();
            editor.OnInspectorGUI();
        }

        //
        // Select & Delete
        //

        void OPEN()
        {
            CLOSE();

            for (int i = 0; i < selectedCount; i++)
            {   // Create an instance of each editor selected, so we can lock down the inspector
                opened.Add(Editor.CreateEditor(selection[i].targets));
            }

            selection.Clear();

            open = true;
        }

        void CLOSE()
        {
            for (int i = 0; i < opened.Count; i++)
            {
                DestroyImmediate(opened[i]);

                opened[i] = null;
            }

            if(cacheEditor)
            {
                DestroyImmediate(cacheEditor);

                cacheEditor = null;
            }

            opened.Clear();

            open = false;
        }

        bool IsSelected(Editor editor)
        {
            return selection.Contains(editor);
        }

        void SelectMultiple(Editor editor)
        {
            if (IsSelected(editor))
            {
                selection.Remove(editor);
            }
            else
            {
                selection.Add(editor);
            }
        }

        void SelectSingle(Editor editor)
        {
            if (IsSelected(editor))
            {
                selection.Clear();
                selection.Remove(editor);
            }
            else
            {
                selection.Clear();
                selection.Add(editor);
            }
        }
    }
}
