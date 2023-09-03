using BehaviorDesigner.Editor;
using NKStudio.FMODPlus.NMProject.BehaviorDesigner;
using UnityEditor;
using UnityEngine;

[CustomObjectDrawer(typeof(KeyListHandler))]
public class FMODPlayEditor : ObjectDrawer
{
    public override void OnGUI(GUIContent label)
    {
        KeyListHandler keyListHandler = value as KeyListHandler;
        EditorGUILayout.BeginVertical();

        if (keyListHandler != null)
        {
            FieldInspector.DrawFields(task, keyListHandler.IsGlobalKeyList, new GUIContent("Is Global KeyList"));

            if (keyListHandler.IsGlobalKeyList)
                FieldInspector.DrawFields(task, keyListHandler.AudioType, new GUIContent("Audio Type"));
            else
                FieldInspector.DrawFields(task, keyListHandler.LocalKeyList, new GUIContent("Local KeyList"));    
        }
        
        EditorGUILayout.EndVertical();
    }
}