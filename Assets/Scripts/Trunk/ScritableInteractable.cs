using UnityEngine;
using System.Collections;

public class ScritableInteractable : ScriptableObject {

    public bool canBeHeld; // whether item can be held or not
    public string itemName;
    public MeshRenderer mesh;
    public bool disposeOnUse = false;
    public Vector3 inventoryPosition;
    public Vector3 inventoryRotation; 

}
