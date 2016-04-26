using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var gameManager = (GameManager)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Start Debug Game"))
        {
            gameManager.SetUpDebugGame(false);
        }
    }
}
