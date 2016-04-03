using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class OrientationCheck : MonoBehaviour 
{
    public Fade fader = null;
    public float duration = 5f;
    float timer = 0f;
    bool updating = false;

    public UnityEvent OnOrientationCheckComplete = new UnityEvent();

	// Use this for initialization
	void Start () {
        if (fader == null) fader = GetComponent<Fade>();
	}

    public void OnFadeInComplete()
    {
        updating = true;
        timer = 0f;
    }

    public void OnFadeOutComplete()
    {
        OnOrientationCheckComplete.Invoke();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!updating) return;
        timer += Time.deltaTime;
        if (timer > duration)
        {
            if (SystemInfo.deviceType != DeviceType.Handheld ||
                Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                updating = false;
                fader.FadeOut();
            }
        }
    }
}
