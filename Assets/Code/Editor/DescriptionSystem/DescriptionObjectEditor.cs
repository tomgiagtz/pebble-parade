using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DescriptionObject))]
public class DescriptionObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DescriptionObject obj = (DescriptionObject)target;

        EditorGUILayout.LabelField("Description Details");
        DrawDefaultInspector();
        // Create a button that sets the 'name' variable to 'gameObject.name'
        if (GUILayout.Button("Set Name As Object Name"))
        {
            obj.AssignName();
        }
    }
}
