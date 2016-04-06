using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Latch : Interactable
{
    public bool _isOpen { get; set; }

    public GameObject trunkLid;
    private bool _isOpening;
    float _timer = 0f;
    float _duration = 2f;

    [Range(10, 110)]
    public float _maxAmp = 45f;

    void Start()
    {
        _isOpen = false;
        _itemsToInteractWith = new HashSet<Type>();
        _canBeHeld = false;
        addTypeToInteractWith(typeof(Tool));
    }

    // Update is called once per frame
    void Update () {
        if (!_isOpening)
        {
            return;
        }

        _timer += Time.deltaTime;
        var t = _timer / _duration;

        var y = 0f;
        
        y = Ease.ElasticEaseOut(t, 0, 2, 1);
        
        trunkLid.transform.localRotation = Quaternion.Euler(0, 360 - y, 0);

        if (_timer > _duration)
        {
            _isOpening = false;
            gameObject.layer = LayerMask.NameToLayer("ClientOnly");
            Debug.Log("Stopped opening");
        }
    }


    public override void InteractWith(Interactable itemToInteractWith)
    {
        throw new NotImplementedException();
    }

    public void Open()
    {
        trunkLid.transform.localRotation = Quaternion.Euler(0, -2, 0);
        _isOpening = true;
        _isOpen = true; 
    }
}
