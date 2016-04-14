using System;
using UnityEngine;

public class Fasteners : Interactable
{
    private ScriptableFasteners fastenerData;
    private bool isAnimating;
    private float time;

    void Start()
    {
        fastenerData = (ScriptableFasteners)itemData; 
        canBeHeld = false; 
    }

    void Update()
    {
        // animate unfastening behaviour
        // rotate counterclockwise for duration seconds
        // when done, remove item
        if (isAnimating)
        {
            time += Time.deltaTime;
            if (time > fastenerData.unfastenDuration)
            {
                isAnimating = false;
                GetComponent<Rigidbody>().useGravity = true;
                return; 
            }
            transform.Rotate(Vector3.up, 1);
            var final = fastenerData.startPosition - fastenerData.fastenerLength * Vector3.right; 
            transform.localPosition = Vector3.Lerp(fastenerData.startPosition, final, time/fastenerData.unfastenDuration);
        }
    }

    public void Unfasten()
    {
        //set flag to animate
        isAnimating = true;
        time = 0f; 
    }

    public FastenerType GetFastenerType()
    {
        return fastenerData.type; 
    }

}