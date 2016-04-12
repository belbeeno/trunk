using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(City))]
public class CityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var cityScript = (City)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Regenerate City"))
        {
            cityScript.Generate();
        }
    }
}