using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Item/Interactable")]
public class ScriptableInteractable : ScriptableObject {

    public bool canBeHeld; // whether item can be held or not
    public string itemName;
    public MeshRenderer mesh;
    public bool disposeOnUse = false;
    public Vector3 positionWhenInInventory;
    public Vector3 rotationWhenInInventory;  

}
