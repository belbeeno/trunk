using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperatorStatus : MonoBehaviour 
{
    [SerializeField, ShowOnly]
    protected float checkAreaTimer = GameSettings.APB_COOLDOWN;
    public OperatorToggle checkAreaToggle = null;
    public void ExhaustCheckArea()
    {
        checkAreaTimer = 0f;
        checkAreaToggle.SetInteractable(false);
    }

    [SerializeField, ShowOnly]
    protected float policeTimer = GameSettings.COP_SIREN_PING_COOLDOWN;
    public OperatorToggle policeToggle = null;
    public void ExhaustPoliceTimer()
    {
        policeTimer = 0f;
        policeToggle.SetInteractable(false);
    }

    [SerializeField, ShowOnly]
    protected float helicopterTimer = GameSettings.HELICOPTER_PING_COOLDOWN;
    public OperatorToggle helicopterToggle = null;
    public void ExhaustHelicopterTimer()
    {
        helicopterTimer = 0f;
        helicopterToggle.SetInteractable(false);
    }

    protected void SetTimers(ref float timer, float cooldown, ref OperatorToggle target)
    {
        if (timer < cooldown)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                target.SetInteractable(true);
            }
            target.cooldownClock.fillAmount = Mathf.Clamp01(timer / cooldown);
        }
    }

    public void Update()
    {
        if (GameManager.Get().RemoteStatus == GameManager.PlayerStatus.InGame)
        {
            SetTimers(ref checkAreaTimer, GameSettings.APB_COOLDOWN, ref checkAreaToggle);
            SetTimers(ref policeTimer, GameSettings.COP_SIREN_PING_COOLDOWN, ref policeToggle);
            SetTimers(ref helicopterTimer, GameSettings.HELICOPTER_PING_COOLDOWN, ref helicopterToggle);
        }
        else
        {
            checkAreaToggle.SetInteractable(false);
            policeToggle.SetInteractable(false);
            helicopterToggle.SetInteractable(false);
        }
    }
}
