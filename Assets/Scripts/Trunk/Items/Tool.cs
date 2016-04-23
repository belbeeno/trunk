using UnityEngine;
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
                StopAllCoroutines(); 
                StartCoroutine(AnimateIntoPosition(latch.transform, latch.toolContactDirection, 1f, GetComponent<Animation>(), toolData.openLatchAnimationClipName, true, latch.Open));
            }
        }
        else if (toolData.canUnfastenFasteners && itemToInteractWith.GetType() == typeof(Fasteners))
        {
            ((Fasteners)itemToInteractWith).Unfasten(); 
            
        } else if (toolData.canWedgeOpenTrunk && itemToInteractWith.GetType() == typeof(WedgeGap))
        {
            var gap = (WedgeGap)itemToInteractWith;
            StartCoroutine(AnimateIntoPosition(gap.transform, gap.gapUpPosition, 1f, GetComponent<Animation>(), toolData.wedgeOpenTrunkAnimationClipName, false, wcb: gap.WedgeOpenCover, offsetStart: gap.positionToolStartAt));
                      
        }
        else
        {
            base.InteractWith(itemToInteractWith);
        }
    }

    protected delegate void latch();
    protected delegate void wedge(float duration);
    protected IEnumerator AnimateIntoPosition(Transform newParent
                                            , Vector3 newParentLocalUpDirection
                                            , float duration
                                            , Animation actionAnimation
                                            , string animationName
                                            , bool animateBack
                                            , latch lcb = null
                                            , wedge wcb = null
                                            , Vector3 offsetStart = new Vector3())
    {
        var curParent = transform.parent; 
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
                
        var isHatchAnimationTriggered = wcb == null;
        if (!isHatchAnimationTriggered)
        {
            timer = 0f;
            var wedgeEndPos = newParent.GetComponent<WedgeGap>().positionToolEndsAt;
            var rotationLayer = transform.GetChild(0); 
            var wedgeEndRot = Quaternion.FromToRotation(rotationLayer.up, wedgeEndPos);
            wcb.Invoke(duration);
            while (timer < duration)
            {
                transform.localPosition = Vector3.Lerp(contactPosition, wedgeEndPos, Ease.QuadEaseInOut(timer, 0f, 1f, duration));
                rotationLayer.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0,90,0), Ease.QuadEaseInOut(timer, 0f, 1f, duration));
                timer += Time.deltaTime;
                yield return 0; 
            }
        }
        

        if (lcb != null) lcb.Invoke();

        if (!animateBack)
        {
            yield break; 
        }

        var curRot = transform.localRotation;
        var curPos = transform.localPosition;
        timer = 0f;
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(curPos, startPos, Mathf.Clamp01(timer / duration));
            transform.localRotation = Quaternion.SlerpUnclamped(curRot, startRot, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        transform.parent = curParent; 
    }

}
