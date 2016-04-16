using UnityEngine;
using System.Collections;

public class PingWave : MonoBehaviour
{
    public enum PingState : int
    {
        Invalid = -1,

        Preview = 0,
        Wave,
        Siren,
    }

    // Why are aniations structured like this lol
    public string[] animNames = new string[3];

    public Animation animController = null;
    public CanvasGroup group = null;

    public PingState currentState = PingState.Invalid;
    private PingState prevState = PingState.Invalid;

    public UnityEngine.UI.ToggleGroup actionToggles = null;

    RectTransform parentRect = null;
    RectTransform rect = null;

    public PID.InitParams pidParams;
    PID pidX = null;
    PID pidY = null;

    // Use this for initialization
    void Start()
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

        pidX = new PID(pidParams, GetVisibleProgressX, GetTargetProgressX, SetVisibleProgressX);
        pidY = new PID(pidParams, GetVisibleProgressY, GetTargetProgressY, SetVisibleProgressY);
    }

    private Vector2 target = new Vector2();
    public float GetVisibleProgressX()
    {
        return rect.anchoredPosition.x;
    }
    public float GetVisibleProgressY()
    {
        return rect.anchoredPosition.y;
    }

    public float GetTargetProgressX() { return target.x; }
    public float GetTargetProgressY() { return target.y; }

    public void SetVisibleProgressX(float val)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x = val;
        rect.anchoredPosition = pos;
    }
    public void SetVisibleProgressY(float val)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.y = val;
        rect.anchoredPosition = pos;
    }

    private void UpdateStatus()
    {
        if (currentState == PingState.Invalid)
        {
            animController.Stop();
            group.alpha = 0f;
        }
        else
        {
            if (prevState == PingState.Invalid)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out target);
                rect.anchoredPosition = target;
            }
            // Why are aniations structured like this lol
            if (animController.isPlaying)
            {
                animController.CrossFade(animNames[(int)currentState],0.25f);
            }
            else
            {
                animController.Play(animNames[(int)currentState], PlayMode.StopAll);
            }
            group.alpha = 1f;
        }
    }

    public void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            currentState = PingState.Preview;
        }
        else
        {
            if (!actionToggles.AnyTogglesOn())
            {
                currentState = PingState.Invalid;
            }
        }
    }

    public void OperatorAction(OperatorToggle.OperatorAction action)
    {
        switch (action)
        {
            case OperatorToggle.OperatorAction.APB:
                currentState = PingState.Wave;
                break;
            case OperatorToggle.OperatorAction.Siren:
                currentState = PingState.Siren;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != currentState)
        {
            UpdateStatus();
            pidX.Reset();
            pidY.Reset();
            prevState = currentState;
        }

        if (currentState != PingState.Invalid)
        {
            if (currentState == PingState.Preview)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out target))
                {
#if UNITY_EDITOR
                    pidX.AssignInit(pidParams);
                    pidY.AssignInit(pidParams);
#endif
                }
            }

            pidX.Compute();
            pidY.Compute();
        }

        if (!animController.isPlaying)
        {
            currentState = PingState.Invalid;
        }
    }
}
