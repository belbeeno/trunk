using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class OrientationCheck : MonoBehaviour 
{
    CanvasGroup group = null;
    public float duration = 5f;
    float timer = 0f;
    bool updating = false;

    public UnityEvent OnOrientationCheckComplete = new UnityEvent();

	// Use this for initialization
	void Start () {
        if (group == null) group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
	}

    IEnumerator SetAlpha(float a)
    {
        float alphaTimer = Mathf.Clamp01(1f - a);
        while (!Mathf.Approximately(alphaTimer, duration))
        {
            alphaTimer = Mathf.MoveTowards(alphaTimer, a, Time.deltaTime);
            group.alpha = alphaTimer;
            yield return 0;
        }
        OnFadeComplete();
    }

    void Reveal()
    {
        StopAllCoroutines();
        StartCoroutine(SetAlpha(1f));
    }

    void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(SetAlpha(0f));
    }

    void OnFadeComplete()
    {
        StopAllCoroutines();
        if (!updating)
        {
            OnOrientationCheckComplete.Invoke();
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!updating) return;
        timer += Time.deltaTime;
        if (timer > duration)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                updating = false;
                Hide();
            }
        }
    }
}
