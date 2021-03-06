﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Latches are used to prevent something with a hinge to open. 
public class Latch : Interactable
{
    [SerializeField]
    public bool isOpen;
    private float degreeTrunkLeftOpen = 3f;
    public GameObject cover;
    public List<GameObject> sideOpenings; 
    private bool isOpening;
    float timer = 0f;
    float duration = 2f;

    public Vector3 toolContactDirection; 

    void Start()
    {
        isOpen = false;
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
            foreach (var side in sideOpenings)
            {
                side.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    // Latches aren't an interacter but an interactee

    public void Open()
    {
        isOpening = true;
        isOpen = true;
        var openLatchSoundClip = GetComponent<CardboardAudioSource>();
        openLatchSoundClip.Play();
        var lidOpeningSoundClip = cover.GetComponent<CardboardAudioSource>();
        lidOpeningSoundClip.Play(); 
    }

}
