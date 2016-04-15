using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class EnableCBAudioSources : MonoBehaviour
{
    public List<CardboardAudioSource> allSources = null;
    public bool enableOnStart = true;
    void EnableAllCBAudioSources()
    {
        for (int i = 0; i < allSources.Count; i++)
        {
            allSources[i].gameObject.SetActive(true);
        }
    }

    void Start()
    {
        EnableAllCBAudioSources();
    }

#if UNITY_EDITOR
    public static List<CardboardAudioSource> GetAudioSourcesInScene()
    {
        GameObject[] rootGOs = SceneManager.GetActiveScene().GetRootGameObjects();
        List<CardboardAudioSource> sources = new List<CardboardAudioSource>();

        for (int i = 0; i < rootGOs.Length; i++)
        {
            GetAudioSourcesInScene(rootGOs[i].transform, ref sources);
        }

        return sources;
    }

    protected static void GetAudioSourcesInScene(Transform target, ref List<CardboardAudioSource> sources)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            Transform child = target.GetChild(i);
            CardboardAudioSource cbas = child.GetComponent<CardboardAudioSource>();
            if (cbas != null)
            {
                sources.Add(cbas);
            }
            GetAudioSourcesInScene(child, ref sources);
        }
    }
#endif // UNITY_EDITOR
}
