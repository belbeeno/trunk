using UnityEngine;
using System.Collections;

public class ScritableInteractable : ScriptableObject {

    public bool canBeHeld; // whether item can be held or not
    public string name;
    public MeshRenderer mesh;
    public bool disposeOnUse = false; 

}
