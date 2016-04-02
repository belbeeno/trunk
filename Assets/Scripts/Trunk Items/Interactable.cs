
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public HashSet<Interactable> _itemsToInteractWith;
    public bool _isStationary;
    public bool _isSelected;

    public Color activeColor = Color.red;
    public MeshRenderer _renderer = null;

    public abstract void InteractWith(Interactable itemToInteractWith);

    public bool CanInteractWith(Interactable item)
    {
        return _itemsToInteractWith.Contains(item); 
    }
    
    public void ItemSelected()
    {
        _isSelected = true; 
    }

    public void ItemDropped()
    {
        _isSelected = false;
    }
}