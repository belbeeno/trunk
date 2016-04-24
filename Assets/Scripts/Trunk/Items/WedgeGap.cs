using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WedgeGap : Interactable
{
    [SerializeField]
    [Range(5, 15)]
    private float wedgeOpenedAngle = 10f;

    [SerializeField]
    private GameObject cover=null;
    
    public bool isWedging { get; set; }

    public bool isWedged { get; set; }

    public Vector3 positionToolStartAt = new Vector3();
    public Vector3 positionToolEndsAt = new Vector3();
    public Vector3 gapUpPosition = new Vector3(); 

    [SerializeField]
    private List<GameObject> otherGaps = null; 

    void Start()
    {
        canBeHeld = false;
        isWedging = false;
        isWedged = false;  
    }

    void Update()
    {
    }

    protected IEnumerator AnimateTrunkLidBeingWedgeOpen(float duration)
    {
        var startRot = cover.transform.localRotation;
        //var startPos = item.localPosition; 
        float timer = 0f;
        while (timer < duration)
        {
            //item.localPosition = Vector3.Lerp(startPos, positionToolStartAt, Ease.QuartEaseOut(timer, 0f, 1f, duration));
            
            cover.transform.localRotation = Quaternion.SlerpUnclamped(startRot, Quaternion.Euler(0, 360 - wedgeOpenedAngle, 0), Ease.QuadEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }

        GetComponent<BoxCollider>().enabled = false; 
        foreach (var gap in otherGaps)
        {
            gap.GetComponent<BoxCollider>().enabled = false; 
        }
        Debug.Log("Done Animating");
        
    }

    public void WedgeOpenCover(float duration)
    {
        StartCoroutine(AnimateTrunkLidBeingWedgeOpen(duration));
    }

    public void WedgeOpenCoverNoAnimation()
    {
        GetComponent<BoxCollider>().enabled = false;
        foreach (var gap in otherGaps)
        {
            gap.GetComponent<BoxCollider>().enabled = false;
        }
        
    }
}