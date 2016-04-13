using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class OperatorToggle : MonoBehaviour 
{
    public enum OperatorAction
    {
        APB,
        Siren,
        Helicoptor
    }

    public OperatorAction action;
    public Toggle toggle = null;
    public float cooldown = 5f;
    public bool startAtFull = true;
    float timer = 5f;

    public Image cooldownClock = null;

    [System.Serializable]
    public class OnOperatorActionCB : UnityEvent<OperatorAction> { }

    public OnOperatorActionCB OnOperatorAction = null;
    public UnityEvent OnFilled = null;

    public void Start()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }
        timer = (startAtFull ? cooldown : 0f);
    }

    private void UpdateBars()
    {
        if (cooldownClock)
        {
            cooldownClock.fillAmount = 1f - Mathf.Clamp01(timer / cooldown);
        }
        else
        {
            toggle.image.fillAmount = Mathf.Clamp01(timer / cooldown);
        }
        toggle.interactable = timer >= cooldown;
    }

    public void OnUse()
    {
        timer = 0f;
        toggle.isOn = false;
        toggle.group.SetAllTogglesOff();
        UpdateBars();

        OnOperatorAction.Invoke(action);
    }

    public void Update()
    {
        if (timer >= cooldown) return;

        timer += Time.deltaTime;
        UpdateBars();
        if (timer >= cooldown)
        {
            OnFilled.Invoke();
        }
    }
}
