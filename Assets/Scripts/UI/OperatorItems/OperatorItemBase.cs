using UnityEngine;
using System.Collections;

public abstract class OperatorItemBase : MonoBehaviour
{
    // Why are aniations structured like this lol
    public string[] animNames = new string[3];

    public Animation animController = null;
    public CanvasGroup group = null;

    public int currentState = -1;
    private int prevState = -1;

    public OperatorToggle[] togglesToTest = new OperatorToggle[2];

    protected RectTransform parentRect = null;
    protected RectTransform rect = null;

    protected abstract Vector2 AnchoredPosition { get; set; }
    protected abstract int PreviewState { get; }
    protected abstract void InitPositions();
    protected abstract void ResetPositions();
    protected abstract void UpdatePositions();

    public abstract void OperatorAction(OperatorToggle.OperatorAction action);

    // Use this for initialization
    protected virtual void Start()
    {
        if (animController == null)
        {
            Debug.LogWarning("Potentially optimization in go " + gameObject.name, gameObject);
            animController = GetComponentInChildren<Animation>();
        }
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
        }
        
        parentRect = transform.parent.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();

        UpdateStatus();
        InitPositions();
    }

    protected Vector2 target = new Vector2();

    private void UpdateStatus()
    {
        if (currentState == -1)
        {
            animController.Stop();
            group.alpha = 0f;
        }
        else
        {
            if (prevState == -1)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out target);
                AnchoredPosition = target;
            }
            // Why are aniations structured like this lol
            if (animController.isPlaying)
            {
                animController.CrossFade(animNames[currentState],0.25f);
            }
            else
            {
                animController.Play(animNames[currentState], PlayMode.StopAll);
            }
            group.alpha = 1f;
        }
    }

    public void OnToggleChanged(bool isOn)
    {
        bool hasOnToggle = false;
        for (int i = 0; i < togglesToTest.Length && !hasOnToggle; i++)
        {
            if (togglesToTest[i].toggle.isOn)
            {
                hasOnToggle = true;
            }
        }

        currentState = (hasOnToggle ? PreviewState : -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != currentState)
        {
            UpdateStatus();
            ResetPositions();
            prevState = currentState;
        }

        if (currentState != -1)
        {
            if (currentState == PreviewState)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out target))
                {
#if UNITY_EDITOR
                    InitPositions();
#endif
                }
            }

            UpdatePositions();
        }

        if (!animController.isPlaying)
        {
            currentState = -1;
        }
    }
}