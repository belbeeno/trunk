using UnityEngine;
using System.Collections.Generic;
using System;

public class Tool : Interactable {

   // Use this for initialization
    void Start () {
        _itemsToInteractWith = new HashSet<Type>();
        _canBeHeld = true; 
        addTypeToInteractWith(typeof(Latch));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override bool CanInteractWith(Interactable item)
    {
        if (item.GetType() == typeof(Latch))
        {
            return CanInteractWith((Latch) item);
        }
        return base.CanInteractWith(item);
    }

    public bool CanInteractWith(Latch item)
    {
        return !item._isOpen;
    }

    public override void InteractWith(Interactable itemToInteractWith)
    {
        if (IsSelected())
        {
            if (CanInteractWith(itemToInteractWith))
            {
                if (itemToInteractWith.GetType() == typeof(Latch))
                {
                    var latch = (Latch)itemToInteractWith;
                    if (latch._isOpen)
                    {
                        return;
                    }
                    latch.Open();
                }

                return;
            } else
            {
                Debug.Log("Nope");
                return;
            }
        }
    }
}
