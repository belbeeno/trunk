using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Fade : MonoBehaviour 
{
    public enum State
    {
        IDLE = 0,
        FADE_IN,
        FADE_OUT
    }
    public Image myImage = null;
    public CanvasGroup group = null;
    public State state = State.IDLE;

    public float duration = 3f;
    float timer = 0f;

    private Color myColor = Color.black;

    public UnityEvent OnFadeComplete = new UnityEvent();

	// Use this for initialization
	void Start () 
    {
        if (group == null && myImage == null)
        {
            Debug.LogError("Fade is missing both a group and a image!", gameObject);
        }

        switch (state)
        {
            case State.FADE_IN:
                timer = 0f;
                break;
            case State.FADE_OUT:
                timer = duration;
                break;
        }
	}
	
    public void FadeIn()
    {
        state = State.FADE_IN;
        timer = 0f;
    }

    public void FadeOut()
    {
        if (group != null)
        {
            group.interactable = false;
        }
        state = State.FADE_OUT;
        timer = duration;
    }

    void OnFadeCompleteInternal()
    {
        if (group != null)
        {
            group.interactable = (state == State.FADE_IN);
            group.blocksRaycasts = (state == State.FADE_IN);
        }
        OnFadeComplete.Invoke();
    }

	// Update is called once per frame
	void Update () 
    {
        if (state == State.IDLE) return;

        else if (state == State.FADE_IN)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                OnFadeCompleteInternal();
                state = State.IDLE;
            }
        }
        else if (state == State.FADE_OUT)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                OnFadeCompleteInternal();
                state = State.IDLE;
            }
        }

        if (group != null)
        {
            group.alpha = Ease.ExpoEaseInOut(Mathf.Clamp01(timer / duration), 0f, 1f, 1f);
        }
        else if (myImage != null)
        {
            myColor = myImage.color;
            myColor.a = Ease.ExpoEaseInOut(Mathf.Clamp01(timer / duration), 0f, 1f, 1f);
            myImage.color = myColor;
        }
	}
}
