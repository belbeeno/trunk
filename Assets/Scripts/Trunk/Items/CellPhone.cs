using UnityEngine;
using System.Collections;
using System;

public class CellPhone : Interactable 
{
    public void CallOperator()
    {
        // Trigger animation to show up in inventory
        GameManager.Get().LocalStatus = GameManager.PlayerStatus.InGameRinging;
    }
}
