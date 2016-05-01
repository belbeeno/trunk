using UnityEngine;
using System.Collections;

public class DropItemInTrunkAudioSource : MonoBehaviour {

	void OnCollisionEnter(Collision other)
    {
       
        var audioSource = other.gameObject.GetComponent<CardboardAudioSource>();
        var audioClip = other.gameObject.GetComponent<Interactable>().itemData.itemDroppedSoundClip; 
        if (audioSource != null)
        {
            DebugConsole.SetText("item:", audioSource.name);
            var volume = Mathf.Min(other.relativeVelocity.magnitude, 1);
            audioSource.PlayOneShot(audioClip, volume); 
        }
    }
}
