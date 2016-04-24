using UnityEngine;
using System.Collections;

public class TextSequence3D : MonoBehaviour 
{
    public TextMesh text = null;

    public Transform startPos = null;
    public Transform endPos = null;

    public string[] lines = new string[0];
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
    }

    void UpdateText()
    {
        text.transform.position = Vector3.Lerp(startPos.position, endPos.position, Ease.EnumToFunc(easingIn ? moveFuncIn : moveFuncOut).Invoke(Mathf.Clamp01(timer / tweenTime), 0f, 1f, 1f));
        text.color = Color.Lerp(Color.clear, Color.white, Ease.EnumToFunc(easingIn ? alphaFuncIn : alphaFuncOut).Invoke(Mathf.Clamp01(timer / tweenTime), 0f, 1f, 1f));
    }

    public void Update()
    {
        if (GameManager.Get().LocalStatus != GameManager.PlayerStatus.PreGame && GameManager.Get().RemoteStatus != GameManager.PlayerStatus.NotConnected) return;

        if (linePointer >= lines.Length)
        {
            enabled = false;
            GameManager.Get().LocalStatus = GameManager.PlayerStatus.InGamePreCall;
            return;
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
                easingIn = true;
            }
        }

        UpdateText();
    }
}
