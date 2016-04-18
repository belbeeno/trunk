using UnityEngine;
using UnityEngine.UI;
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
    public OperatorActionBar actionBar = null;

    protected RectTransform parentRect = null;
    protected RectTransform rect = null;

    protected abstract Vector2 AnchoredPosition { get; set; }
    protected abstract int PreviewState { get; }
    protected abstract void InitPositions();
    protected abstract void ResetPositions();
    protected abstract void UpdatePositions();

    public abstract void OperatorAction(OperatorToggle.OperatorAction action);

    [SerializeField]
    private Image[] _cachedChildren = new Image[0];
    protected Image[] ChildrenImages
    {
        get
        {
            if (_cachedChildren.Length <= 0)
            {
                _cachedChildren = GetComponentsInChildren<Image>();
            }
            return _cachedChildren;
        }
    }

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
    protected bool UpdateTarget()
    {
        Vector2 normalizedPos;
        if (actionBar.GetNormalizedMapPosition(out normalizedPos))
        {
            DebugConsole.SetText("GetNormPos", normalizedPos.ToString());
            target = actionBar.GetProxyMapPosition(normalizedPos);
            DebugConsole.SetText("target", target.ToString());
            return true;
        }

        DebugConsole.SetText("GetNormPos", "OFF SCREEN");
        return false;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, MyCamera, out target);
    }

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
                UpdateTarget();
                AnchoredPosition = target;
            }
            // Why are aniations structured like this lol
            if (animController.isPlaying)
            {
                animController.CrossFade(animNames[currentState], 0.25f);
            }
            else
            {
                animController.Play(animNames[currentState], PlayMode.StopAll);
                for (int i = 0; i < ChildrenImages.Length; i++)
                {
                    // Wow are you fucking serious????
                    // http://forum.unity3d.com/threads/alpha-animation-not-working-with-some-ui-elements.266278/#post-1953828
                    ChildrenImages[i].SetAllDirty();
                }
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
                if (UpdateTarget())
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