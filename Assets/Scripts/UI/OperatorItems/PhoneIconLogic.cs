using UnityEngine;
using System.Collections;

public class PhoneIconLogic : MonoBehaviour 
{
    public float shakeDuration = 0.7f;
    protected float shakeTimer = 0f;

    public float waitDuration = 1.4f;
    protected float waitTimer = 0f;

    [MinMaxSlider(-45f, 45f)]
    public Vector2 randoRange = new Vector2(-5f, 5f);

    public UnityEngine.UI.Button myButton = null;

    public void OnRemotePlayerStateChanged(GameManager.PlayerStatus status)
    {
        if (status == GameManager.PlayerStatus.InGameRinging)
        {
            myButton.interactable = true;
        }
    }

    public void Reset()
    {
        transform.localRotation = Quaternion.identity;
        enabled = false;
        TrunkNetworkingBase.GetBase().EnableVoiceChat();
    }

    void Start()
    {
        if (myButton == null) myButton = GetComponent<UnityEngine.UI.Button>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!myButton.IsInteractable()) return;

	    if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(shakeTimer / shakeDuration);
            t = Mathf.Lerp(0f, Random.Range(randoRange.x, randoRange.y), t);
            transform.localRotation = Quaternion.Euler(0f, 0f, t);
            if (shakeTimer < 0f)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            shakeTimer = shakeDuration;
            waitTimer = waitDuration;
        }
	}
}
