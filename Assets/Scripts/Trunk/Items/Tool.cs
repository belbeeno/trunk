using UnityEngine;
using System.Collections.Generic;
using System;

public class Tool : Interactable {

   // Use this for initialization
    void Start () {
        _itemsToInteractWith = new HashSet<Interactable>(); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void InteractWith(Interactable itemToInteractWith)
    {
        if (_isSelected)
        {
            if (CanInteractWith(itemToInteractWith))
            {
                Debug.Log("Sure");
                return;
            } else
            {
                Debug.Log("Nope");
                return;
            }
        }
    }
}
