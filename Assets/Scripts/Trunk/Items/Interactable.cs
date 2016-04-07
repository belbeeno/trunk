
using System;
using System.Collections.Generic;
using UnityEngine;

// This marks a gameobject as something that the player can interact with
public abstract class Interactable : MonoBehaviour
{
    // The type of items this current item can be used with/on
    public HashSet<Type> _itemsToInteractWith;

    // Whether this is an item that can be put into the inventory
    public bool _canBeHeld;

    // Whether the object is currently in the inventory or not
    private bool _isSelected; 
    
    void Start()
    {
    }

    public abstract void InteractWith(Interactable itemToInteractWith);
    
    public virtual bool CanInteractWith(Interactable item)
    {
        return _itemsToInteractWith.Contains(item.GetType()); 
    }
       
    public bool IsSelected()
    {
        return _isSelected; 
    }

    public void ItemSelected()
    {
        _isSelected = true; 
    }

    public void ItemDropped()
    {
        _isSelected = false;
    }

    internal bool CanBeHeld()
    {
        return _canBeHeld; 
    }
}