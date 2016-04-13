
using System;
using System.Collections.Generic;
using UnityEngine;

// This marks a gameobject as something that the player can interact with
public abstract class Interactable : MonoBehaviour
{
    // The type of items this current item can be used with/on
    public HashSet<Type> itemsToInteractWith;

    // Whether this is an item that can be put into the inventory
    protected bool canBeHeld;

    // Whether the object is currently in the inventory or not
    private bool isSelected;

    public Vector3 inHandOrientation;

    void Start()
    {
    }

    public virtual void InteractWith(Interactable itemToInteractWith)
    {
        if (!canBeHeld)
        {
            return; 
        }
    }
    
    public virtual bool CanInteractWith(Interactable item)
    {
        return canBeHeld;
    }
       
    public bool IsSelected()
    {
        return isSelected; 
    }

    public void ItemSelected()
    {
        isSelected = true; 
    }

    public void ItemDropped()
    {
        isSelected = false;
    }

    internal bool CanBeHeld()
    {
        return canBeHeld; 
    }
}