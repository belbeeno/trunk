
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public HashSet<Type> _itemsToInteractWith;
    public bool _canBeHeld;
    private bool _isSelected; 
    
    void Start()
    {
    }

    public abstract void InteractWith(Interactable itemToInteractWith);

    public void addTypeToInteractWith(Type type)
    {
        _itemsToInteractWith.Add(type);
    }


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