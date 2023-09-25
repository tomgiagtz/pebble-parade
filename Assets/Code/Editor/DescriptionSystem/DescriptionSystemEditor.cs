using UnityEngine;
using UnityEditor;

public class DescriptionSystemEditor : EditorWindow
{
    private Object targetObject;
    private Editor descriptionObjectEditor;
    private GUIStyle redButtonStyle;

    [MenuItem("Window/Description System/Open Description System Window")]
    public static void OpenCustomWindow()
    {
        var window = GetWindow(typeof(DescriptionSystemEditor));
        window.titleContent = new GUIContent("Description System");
    }

    private void OnGUI()
    {
        if (redButtonStyle == null)
        {
            redButtonStyle = new GUIStyle(GUI.skin.button);
            redButtonStyle.normal.textColor = Color.red;
        }
        // Get the currently selected object
        Object selectedObject = Selection.activeObject;

        // Check if the selected object has changed
        if (selectedObject != targetObject)
        {
            targetObject = selectedObject;
            descriptionObjectEditor = null; // Reset the custom editor
            Repaint();
        }

        if (targetObject == null)
        {
            EditorGUILayout.LabelField("Description System");
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select an object to view/edit its DescriptionObject.");
        }
        else
        {
            DescriptionObject descriptionObject = ((GameObject)targetObject).GetComponent<DescriptionObject>();

            if (descriptionObject == null)
            {
                EditorGUILayout.LabelField("Description System");
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Selected object does not have a DescriptionObject component.");
                if (GUILayout.Button("Attach DescriptionObject"))
                {
                    AttachDescriptionObject();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Description System");
                EditorGUILayout.Separator();

                // Create a custom editor for the DescriptionObject
                if (descriptionObjectEditor == null || descriptionObjectEditor.target != descriptionObject)
                {
                    descriptionObjectEditor = Editor.CreateEditor(descriptionObject);
                }

                // Draw the custom inspector for DescriptionObject
                descriptionObjectEditor.OnInspectorGUI();

                EditorGUILayout.Separator();

                // Add a button to remove the DescriptionObject component
                if (GUILayout.Button("Remove DescriptionObject", redButtonStyle))
                {
                    if (EditorUtility.DisplayDialog("Remove DescriptionObject", "Are you sure you want to remove the DescriptionObject component?", "Yes", "No"))
                    {
                        RemoveDescriptionObject(descriptionObject);
                    }
                }
            }
        }

        // Make sure to call Repaint to update the window
        Repaint();
    }

    private void AttachDescriptionObject()
    {
        GameObject selectedGameObject = (GameObject)targetObject;

        if (selectedGameObject != null)
        {
            DescriptionObject descriptionObject = selectedGameObject.AddComponent<DescriptionObject>();
            // Optionally, you can initialize properties of the DescriptionObject here.
            // For example: descriptionObject.myProperty = "Initial Value";
        }
    }

    private void RemoveDescriptionObject(DescriptionObject descriptionObject)
    {
        if (descriptionObject != null)
        {
            DestroyImmediate(descriptionObject, true);
            descriptionObjectEditor = null; // Reset the custom editor when the component is removed.
            // Optionally, perform any additional cleanup or logic here.
        }
    }
}
