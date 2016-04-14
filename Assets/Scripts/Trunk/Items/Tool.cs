using UnityEngine;
using System.Collections.Generic;
using System;

// All tools can be used to open a latch. 
// This is to contain all shared functionalitys of tools
public class Tool : Interactable {

    private ScriptableTools toolData; 

   // Use this for initialization
    void Start () {
        toolData = (ScriptableTools)itemData; 
        canBeHeld = toolData.canBeHeld; 
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public override bool CanInteractWith(Interactable item)
    {
        if (item.GetType() == typeof(Latch))
        {
            return (toolData.canOpenLatch && !((Latch) item).isOpen);
        }
        if (item.GetType() == typeof(Fasteners))
        {
            var fastenerType = ((Fasteners)item).GetFastenerType();
            return (toolData.interactableFastenerList.Contains(fastenerType));
        }
        return base.CanInteractWith(item); 
    }
    
    public override void InteractWith(Interactable itemToInteractWith)
    {
        // Check if it's a tool specific item

        if (toolData.canOpenLatch && itemToInteractWith.GetType() == typeof(Latch))
        {
            var latch = (Latch)itemToInteractWith;
            if (!latch.isOpen)
            {
                latch.Open();
            }
        }
        else if (toolData.canUnfastenFasteners && itemToInteractWith.GetType() == typeof(Fasteners))
        {
            ((Fasteners)itemToInteractWith).Unfasten(); 
            
        }
        else
        {
            base.InteractWith(itemToInteractWith);
        }
    }
    
}
