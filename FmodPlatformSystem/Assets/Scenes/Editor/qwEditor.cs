using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(qw))]
public class qwEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(new IntegerField());

        //root.schedule.Execute(() => );
        
        return root;
    }
}
