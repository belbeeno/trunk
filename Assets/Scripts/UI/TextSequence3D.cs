using UnityEngine;
using System.Collections;

public class TextSequence3D : MonoBehaviour 
{
    public TextMesh text = null;

    public Transform startPos = null;
    public Transform endPos = null;

    public string[] lines = new string[0];
    public string winMessage = "You were rescued by the police.";
    public string loseMessage = "You were never heard from again...";
    protected int linePointer = 0;
    protected float timer = 0f;
    protected bool easingIn = true;

    public float tweenTime = 1f;
    public Ease.EaseTypes moveFuncIn = Ease.EaseTypes.CircEaseOut;
    public Ease.EaseTypes alphaFuncIn = Ease.EaseTypes.Linear;
    public Ease.EaseTypes moveFuncOut = Ease.EaseTypes.CircEaseOut;
    public Ease.EaseTypes alphaFuncOut = Ease.EaseTypes.Linear;

    public void Start()
    {
        linePointer = 0;
        if (linePointer < lines.Length)
        {
            text.text = lines[linePointer];
        }
        TrunkNetworkingBase.GetBase().OnGameWin.AddListener(OnWin);
        TrunkNetworkingBase.GetBase().OnGameLost.AddListener(OnLost);
    }

    void UpdateText()
    {
        text.transform.position = Vector3.Lerp(startPos.position, endPos.position, Ease.EnumToFunc(easingIn ? moveFuncIn : moveFuncOut).Invoke(Mathf.Clamp01(timer / tweenTime), 0f, 1f, 1f));
        text.color = Color.Lerp(Color.clear, Color.white, Ease.EnumToFunc(easingIn ? alphaFuncIn : alphaFuncOut).Invoke(Mathf.Clamp01(timer / tweenTime), 0f, 1f, 1f));
    }

    public void OnWin()
    {
        lines = new string[1];
        lines[0] = winMessage;
        linePointer = 0;
    }

    public void OnLost()
    {
        lines = new string[1];
        lines[0] = loseMessage;
        linePointer = 0;
    }

    public void Update()
    {
        if (GameManager.Get().LocalStatus != GameManager.PlayerStatus.PreGame
            && GameManager.Get().RemoteStatus != GameManager.PlayerStatus.NotConnected)
        {
            return;
        }
        else if (GameManager.Get().IsGameOver())
        {
            // Getting lazyyyy
            if (linePointer >= lines.Length) Application.Quit();
        }

        if (easingIn)
        {
            if (timer < tweenTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (Cardboard.SDK.Triggered || Input.GetMouseButtonUp(0))
                {
                    easingIn = false;
                }
            }
        }
        
        if (!easingIn)
        {
            if (timer >= 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                linePointer++;
                if (linePointer < lines.Length)
                {
                    text.text = lines[linePointer];
                }
                else if (GameManager.Get().IsGameOver())
                {
                    Application.Quit();
                }
                else
                {
                    GameManager.Get().LocalStatus = GameManager.PlayerStatus.InGamePreCall;
                }
                easingIn = true;
            }
        }

        UpdateText();

    }
}
