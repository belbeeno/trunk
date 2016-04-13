using System;
using UnityEngine;

public class Fasteners : Interactable
{
    [SerializeField]
    ScriptableFasteners fastenerData; 

    void Start()
    {
        canBeHeld = false; 
    }

    void Update()
    {
        // animate unfastening behaviour
        // rotate counterclockwise for duration seconds
        // when done, remove item
    }

    public void Unfasten()
    {
        //set flag to animate
    }

    public FastenerType GetFastenerType()
    {
        return fastenerData.type; 
    }
}