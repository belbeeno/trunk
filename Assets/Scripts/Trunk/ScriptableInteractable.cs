﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Item/Interactable")]
public class ScriptableInteractable : ScriptableObject {

    public bool canBeHeld; // whether item can be held or not
    public string itemName;
    public bool disposeOnUse = false;
    public float rotationWhenInInventory;
    
    public AudioClip itemDroppedSoundClip;
    public AudioClip itemPickedUpSoundClip; 

}
