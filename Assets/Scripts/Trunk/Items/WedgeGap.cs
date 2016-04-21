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

    [SerializeField]
    [Range(0, 5)]
    private float duration;

    public bool isWedging { get; set; }

    public bool isWedged { get; set; }

    public Vector3 localWedgeLocation = new Vector3();

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

    protected IEnumerator AnimateTrunkLidBeingWedgeOpen(Transform item)
    {
        var startRot = cover.transform.localRotation;
        var startPos = item.localPosition; 
        float timer = 0f;
        while (timer < duration)
        {
            item.localPosition = Vector3.Lerp(startPos, localWedgeLocation, Ease.QuartEaseOut(timer, 0f, 1f, duration));
            
            cover.transform.localRotation = Quaternion.SlerpUnclamped(startRot, Quaternion.Euler(0, 360 - wedgeOpenedAngle, 0), Ease.ElasticEaseOut(timer, 0f, 1f, duration));
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

    public void WedgeOpenCover(Transform item)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateTrunkLidBeingWedgeOpen(item));
    }
}