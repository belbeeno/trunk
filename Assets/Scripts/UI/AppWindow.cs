using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AppWindow : MonoBehaviour 
{
    [System.Serializable]
    public class OnActiveChangedCB : UnityEvent<bool> { }
    public OnActiveChangedCB OnActiveChanged;

    private AudioSource _audio = null;
    public AudioSource Audio
    {
        get
        {
            if (_audio == null)
                _audio = transform.parent.GetComponent<AudioSource>();
            return _audio;
        }
    }
    public AudioClip onEnable = null;
    public AudioClip onDisable = null;

    void OnEnable()
    {
        if (Audio && onEnable != null)
        {
            Audio.clip = onEnable;
            Audio.Play();
        }
        OnActiveChanged.Invoke(true);
    }

    void OnDisable()
    {
        if (Audio && onDisable != null)
        {
            Audio.clip = onDisable;
            Audio.Play();
        }
        OnActiveChanged.Invoke(false);
    }

}
