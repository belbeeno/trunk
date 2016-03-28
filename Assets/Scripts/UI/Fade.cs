using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour 
{
    public enum State
    {
        IDLE = 0,
        FADE_IN,
        FADE_OUT
    }
    protected Image myImage = null;
    public State state = State.IDLE;

    public float duration = 3f;
    float timer = 0f;

    Color myColor = Color.black;

	// Use this for initialization
	void Start () 
    {
        myImage = GetComponent<Image>();
        myColor = myImage.color;
	}
	
    public void FadeIn()
    {
        state = State.FADE_IN;
        timer = duration;
    }

    public void FadeOut()
    {
        state = State.FADE_OUT;
        timer = 0f;
    }

	// Update is called once per frame
	void Update () 
    {
        if (state == State.IDLE) return;
        else if (state == State.FADE_IN)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                state = State.IDLE;
            }
        }
        else if (state == State.FADE_OUT)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                state = State.IDLE;
            }
        }
        myColor.a = Ease.ExpoEaseInOut(Mathf.Clamp01(timer / duration), 0f, 1f, 1f);
        myImage.color = myColor;
	}
}
