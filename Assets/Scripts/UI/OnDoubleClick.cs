using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class OnDoubleClick : MonoBehaviour 
{
    public UnityEvent OnDoubleClickEvent;
    public static float timeBetweenClicks = 0.45f;

    public Button myButton;
    private float timer = -1f;

    public void Start()
    {
        if (myButton == null) myButton = GetComponent<Button>();
    }

    public void OnClick()
    {
        if (timer <= 0f)
        {
            timer = timeBetweenClicks;
        }
        else
        {
            OnDoubleClickEvent.Invoke();
            timer = -1f;
        }
    }

    public void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }
}
