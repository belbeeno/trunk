using UnityEngine;
using System.Collections.Generic;
using System;

// All tools can be used to open a latch. 
// This is to contain all shared functionalitys of tools
public class Tool : Interactable {

   // Use this for initialization
    void Start () {
        _itemsToInteractWith = new HashSet<Type>() { typeof(Latch) };
        _canBeHeld = true; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override bool CanInteractWith(Interactable item)
    {
        if (item.GetType() == typeof(Latch))
        {
            return !((Latch) item)._isOpen;
        }
        return base.CanInteractWith(item);
    }
    
    public override void InteractWith(Interactable itemToInteractWith)
    {
        if (CanInteractWith(itemToInteractWith))
        {
            if (itemToInteractWith.GetType() == typeof(Latch))
            {
                var latch = (Latch)itemToInteractWith;
                if (!latch._isOpen)
                {
                    latch.Open();
                }
            }
        }
        else
        {
            Debug.Log("Nope");
        }
    }
}
