using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// All tools can be used to open a latch. 
// This is to contain all shared functionalitys of tools
public class Tool : Interactable {

    private ScriptableTools toolData;

    private Vector3 localPositionToAmnimateTo; 

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
                StartCoroutine(AnimateIntoPosition(latch.transform, latch.upDirection, 1f, GetComponent<Animation>(), latch.Open));
            }
        }
        else if (toolData.canUnfastenFasteners && itemToInteractWith.GetType() == typeof(Fasteners))
        {
            ((Fasteners)itemToInteractWith).Unfasten(); 
            
        } else if (toolData.canWedgeOpenTrunk && itemToInteractWith.GetType() == typeof(WedgeGap))
        {
            var gap = (WedgeGap)itemToInteractWith;
            gameObject.transform.parent = gap.transform;
            gap.WedgeOpenCover(gameObject.transform);
           
        }
        else
        {
            base.InteractWith(itemToInteractWith);
        }
    }

    protected delegate void OnAnimationComplete();
    protected IEnumerator AnimateIntoPosition(Transform newParent
                                            , Vector3 newParentLocalUpDirection
                                            , float duration
                                            , Animation actionAnimation
                                            , OnAnimationComplete cb = null)
    {
        DebugConsole.SetText("before position", transform.localPosition.ToString());
        DebugConsole.SetText("calculated position", newParent.InverseTransformPoint(transform.TransformPoint(transform.localPosition)).ToString());
        transform.parent = newParent;
        DebugConsole.SetText("after position", transform.localPosition.ToString());

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        var endRot = Quaternion.FromToRotation(toolData.toolForwardDirection, newParentLocalUpDirection);
        float timer = 0f;
        while (timer < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, newParentLocalUpDirection * toolData.toolTipOffset, Mathf.Clamp01(timer / duration));
            transform.localRotation = Quaternion.SlerpUnclamped(startRot, endRot, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }

        //Trigger animation script here
        actionAnimation.Play(toolData.openLatchAnimationClipName);
        if (cb != null) cb.Invoke();
    }

}
