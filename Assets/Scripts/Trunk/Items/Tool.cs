﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

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
        if (item.GetType() == typeof(WedgeGap))
        {
            return (toolData.canWedgeOpenTrunk);
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
                StartCoroutine(OpenLatchAnimation(latch, 1f));
            }
        }
        else if (toolData.canUnfastenFasteners && itemToInteractWith.GetType() == typeof(Fasteners))
        {
            ((Fasteners)itemToInteractWith).Unfasten(); 
            
        } else if (toolData.canWedgeOpenTrunk && itemToInteractWith.GetType() == typeof(WedgeGap))
        {
            var gap = (WedgeGap)itemToInteractWith;
            StartCoroutine(WedgeOpenTrunk(gap, 1f));
        }
        else
        {
            base.InteractWith(itemToInteractWith);
        }
    }


    protected IEnumerator OpenLatchAnimation(Latch latch, float duration)
    {
        var curParent = transform.parent;

        var startPos = transform.localPosition;
        var startRot = transform.localRotation;
        
        yield return StartCoroutine(AnimateIntoPosition(latch.transform, latch.toolContactDirection, 1f));
        
        var actionAnimation = GetComponent<Animation>();
        actionAnimation.Play(toolData.openLatchAnimationClipName);
        var audioSource = GetComponent<CardboardAudioSource>();
        audioSource.PlayOneShot(toolData.itemLatchSoundClip);
        while (actionAnimation.isPlaying)
        {
            yield return 0;
        }

        latch.Open();

        transform.parent = curParent;

        var curPos = transform.localPosition;
        var rotationLayer = transform.GetChild(0);
        var rotLayerRot = rotationLayer.localRotation;
        var curRot = transform.localRotation; 

        var timer = 0f;
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(curPos, startPos, Mathf.Clamp01(timer / duration));
            transform.localRotation = Quaternion.Slerp(curRot, startRot, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            rotationLayer.localRotation = Quaternion.SlerpUnclamped(rotLayerRot, Quaternion.identity, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            
            timer += Time.deltaTime;
            yield return 0;
        }
        
    }

    protected IEnumerator WedgeOpenTrunk(WedgeGap gap, float duration)
    {
        yield return StartCoroutine(AnimateIntoPosition(gap.transform, gap.gapUpPosition, 1f));

        var timer = 0f;
        var contactPosition = transform.localPosition;
        var wedgeEndPos = gap.GetComponent<WedgeGap>().positionToolEndsAt;
        var rotationLayer = transform.GetChild(0);
        var wedgeEndRot = Quaternion.AngleAxis(-90, Vector3.right);
        
        gap.WedgeOpenCover(duration);
        gap.GetComponent<CardboardAudioSource>().Play(); 
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(contactPosition, wedgeEndPos, Ease.QuadEaseInOut(timer, 0f, 1f, duration));
            rotationLayer.localRotation = Quaternion.Slerp(Quaternion.identity, wedgeEndRot, Ease.QuadEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }
    }
    
    protected IEnumerator AnimateIntoPosition(Transform newParent
                                            , Vector3 newParentLocalUpDirection
                                            , float duration
                                            , Vector3 offsetStart = new Vector3())
    {
        transform.parent = newParent;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        var endRot = Quaternion.FromToRotation(toolData.frontDirection, -1 * newParentLocalUpDirection);
        var contactPosition = (newParentLocalUpDirection * toolData.toolTipOffset) + offsetStart;
        float timer = 0f;
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, contactPosition, Mathf.Clamp01(timer / duration));
            transform.localRotation = Quaternion.SlerpUnclamped(startRot, endRot, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        
    }

}
