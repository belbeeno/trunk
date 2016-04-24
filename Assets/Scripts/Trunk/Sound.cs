using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	void OnCollisionEnter(Collision other)
    {
        var audioSource = other.gameObject.GetComponent<CardboardAudioSource>();
        if (audioSource != null)
        {
            audioSource.Play(); 
        }
    }
}
