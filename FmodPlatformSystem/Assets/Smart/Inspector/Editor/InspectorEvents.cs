using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Smart
{
    using Object = UnityEngine.Object;

	public partial class Inspector
	{
        //
        // EDITORS
        //

        void EDITOR_EVENTS(Editor editor, Rect rect)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    EditorMouseDown(editor, rect);
                    break;
                case EventType.KeyDown:
                    EditorKeyDown(editor);
                    break;
            }
        }

        void EditorMouseDown(Editor editor, Rect rect)
        {
            // E x i t
            if (!rect.Contains(e.mousePosition)) { return; }
            // LEFT (0)
            if (e.button == 0) { EditorMouseDownLeft(editor); }
            // RIGHT (1)
            if (e.button == 1) { EditorMouseDownRight(editor); }
        }

        void EditorMouseDownLeft(Editor editor)
        {
            if (e.control)
            {
                SelectMultiple(editor);
            }
            else
            {
                SelectSingle(editor);
            }

            Repaint();
        }

        void EditorMouseDownRight(Editor editor)
        {
            if (!smartView) { return; }

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Move Up"), false, () => MoveComponentUp(editor));
            menu.AddItem(new GUIContent("Move Down"), false, () => MoveComponentDown(editor));
            menu.AddItem(new GUIContent("Delete"), false, () => Delete(editor));
            menu.ShowAsContext();
        }

        void Delete(Editor editor)
        {
            if (selection.Contains(editor)) { selection.Remove(editor); }

            Undo.DestroyObjectImmediate(editor.target);
        }

        void MoveComponentUp(Editor editor)
        {
            Component component;

            component = editor.target as Component;
            // Continue
            if (!component) { return; }

            ComponentUtility.MoveComponentUp(component);
        }

        void MoveComponentDown(Editor editor)
        {
            Component component;

            component = editor.target as Component;
            // Exit
            if (!component) { return; }

            ComponentUtility.MoveComponentDown(component);
        }

        void EditorKeyDown(Editor editor)
        {
            if (!IsSelected(editor)) { return; }

            switch (e.keyCode)
            {
                case KeyCode.Delete:
                    DeleteComponent();
                    e.Use();
                    break;
                case KeyCode.C:
                    if (!e.control) { return; }
                    CopyComponents();
                    return;

            }
            Repaint();
        }

        void DeleteComponent()
        {
            for (int i = selectedCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < selection[i].targets.Length; j++)
                {
                    if (selection[i].targets[j] == null) { continue; }

                    Undo.DestroyObjectImmediate(selection[i].targets[j]);
                }
                selection.RemoveAt(i);
            }
        }

        void CopyComponents()
        {
            string message;

            clipboard.Clear();

            for (int i = 0; i < selection.Count; i++)
            {
                clipboard.AddRange(selection[i].targets);
            }

            message = string.Format("Copy: {0} Components", selection.Count);

            ShowNotification(new GUIContent(message));
        }

        void PasteComponents()
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                for (int j = 0; j < clipboard.Count; j++)
                {
                    ComponentUtility.CopyComponent(clipboard[j] as Component);
                    ComponentUtility.PasteComponentAsNew(gameObjects[i]);
                }

                Undo.RecordObject(gameObjects[i], "PasteComponent");
            }

            string text = string.Format("Paste: {0} Components", clipboard.Count);

            ShowNotification(new GUIContent(text));

            Repaint();
        }

        //
        // INSPECTOR
        //

        void INSPECTOR_EVENT()
        {
            switch(e.type)
            {
                case EventType.KeyDown:
                    InspectorKeyDown();
                    break;
                case EventType.DragUpdated:
                    InspectorDragUpdated();
                    break;
                case EventType.DragPerform:
                    InspectorDragPerform();
                    break;
            }
        }

        void InspectorKeyDown()
        {
            switch (e.keyCode)
            {
                case KeyCode.V:
                    if (!e.control) { return; }
                    PasteComponents();
                    return;

            }
            Repaint();
        }

        void InspectorDragUpdated()
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }

        void InspectorDragPerform()
        {
            // E x i t
            if (!rect.Contains(e.mousePosition)) { return; }

            Object[] others = DragAndDrop.objectReferences;
            MonoScript script;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                for (int j = 0; j < others.Length; j++)
                {
                    script = others[j] as MonoScript;

                    if (!script) { continue; }

                    gameObjects[i].AddComponent(script.GetClass());
                }
            }

            e.Use();
        }
    }
}
