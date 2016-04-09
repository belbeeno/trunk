using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Latches are used to prevent something with a hinge to open. 
public class Latch : Interactable
{
    public bool _isOpen { get; set; }

    private float _final = 2;
    public GameObject cover;
    private bool _isOpening;
    float _timer = 0f;
    float _duration = 2f;

    void Start()
    {
        _isOpen = false;
        _itemsToInteractWith = new HashSet<Type>() { typeof(Tool) };
        _canBeHeld = false;
    }

    // Used to animated the cover opening 
    void Update () {
        if (!_isOpening)
        {
            return;
        }

        _timer += Time.deltaTime;
        var t = _timer / _duration;

        var y = 0f;
        
        y = Ease.ElasticEaseOut(t, 0, _final, _duration);
        
        cover.transform.localRotation = Quaternion.Euler(0, 360 - y, 0);

        if (_timer > _duration)
        {
            _isOpening = false;
            gameObject.layer = LayerMask.NameToLayer("ClientOnly");
            Debug.Log("Stopped opening");
        }
    }

    // Latches aren't an interacter but an interactee
    public override void InteractWith(Interactable itemToInteractWith)
    {
        return;
    }

    public void Open()
    {
        _isOpening = true;
        _isOpen = true; 
    }
}
