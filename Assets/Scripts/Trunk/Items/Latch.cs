using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Latches are used to prevent something with a hinge to open. 
public class Latch : Interactable
{
    public bool isOpen { get; set; }

    private float degreeTrunkLeftOpen = 3f;
    public GameObject cover;
    private bool isOpening;
    float timer = 0f;
    float duration = 2f;

    void Start()
    {
        isOpen = false;
        itemsToInteractWith = new HashSet<Type>() { typeof(Tool) };
        canBeHeld = false;
    }

    // Used to animated the cover opening 
    void Update () {
        if (!isOpening)
        {
            return;
        }

        timer += Time.deltaTime;
        var t = timer / duration;

        var y = 0f;
        
        y = Ease.ElasticEaseOut(t, 0, degreeTrunkLeftOpen, duration);
        
        cover.transform.localRotation = Quaternion.Euler(0, 360 - y, 0);

        if (timer > duration)
        {
            isOpening = false;
            gameObject.layer = LayerMask.NameToLayer("ClientOnly");
            Debug.Log("Stopped opening");
        }
    }

    // Latches aren't an interacter but an interactee

    public void Open()
    {
        isOpening = true;
        isOpen = true; 
    }

}
