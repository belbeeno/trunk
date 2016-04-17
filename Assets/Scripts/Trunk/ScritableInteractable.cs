using UnityEngine;
using System.Collections;

public class ScriptableInteractable : ScriptableObject {

    public bool canBeHeld; // whether item can be held or not
    public string itemName;
    public MeshRenderer mesh;
    public bool disposeOnUse = false;
    public Transform transformWhenInInventory; 

}
