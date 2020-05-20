using UnityEditor;

public static class EditorExtensions
{
    public static SerializedProperty GetProperty(this Editor editor, string path)
    {
        return editor.serializedObject.FindProperty(path);
    }

    public static SerializedProperty Get(this SerializedObject so, string path)
    {
        return so.FindProperty(path);
    }

    public static SerializedProperty Get(this SerializedProperty p, string path)
    {
        return p.FindPropertyRelative(path);
    }

    public static SerializedProperty Child(this SerializedProperty p, int i)
    {
        return p.GetArrayElementAtIndex(i);
    }
}