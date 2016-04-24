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
        Helicopter
    }

    public OperatorAction action;
    public Toggle toggle = null;

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
    }

    private void UpdateBars()
    {
        bool wasFilled = toggle.interactable;
        toggle.interactable = cooldownClock.fillAmount >= 1f;
        if (wasFilled != toggle.interactable)
        {
            OnFilled.Invoke();
        }
    }

    public void SetInteractable(bool enable)
    {
        toggle.interactable = enable;
        cooldownClock.enabled = !enable;
    }

    public void OnUse()
    {
        toggle.isOn = false;
        toggle.group.SetAllTogglesOff();
        UpdateBars();

        OnOperatorAction.Invoke(action);
    }
}
