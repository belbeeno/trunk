using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EnableCBAudioSources))]
public class EnableCBAudioSourcesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnableCBAudioSources script = (EnableCBAudioSources)target;
        if (GUILayout.Button("Gather all CBAudioSources"))
        {
            script.allSources = EnableCBAudioSources.GetAudioSourcesInScene();
        }
        if (GUILayout.Button("Disable all CBAudioSources"))
        {
            script.allSources = EnableCBAudioSources.GetAudioSourcesInScene();
            for (int i = 0; i < script.allSources.Count; i++)
            {
                script.allSources[i].gameObject.SetActive(false);
            }
        }
    }
}
