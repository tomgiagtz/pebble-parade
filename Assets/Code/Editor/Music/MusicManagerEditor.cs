using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        MusicManager musicManager = (MusicManager)target;

        if (GUILayout.Button("Menu Theme"))
        {
            musicManager.PlayMainMenuTheme();
        }

        if (GUILayout.Button("World Theme"))
        {
            musicManager.PlayWorldTheme();
        }

        if (GUILayout.Button("Pause"))
        {
            musicManager.StopMusic(); 
        }
    }
}