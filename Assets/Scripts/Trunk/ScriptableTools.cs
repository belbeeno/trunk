using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "toolData", menuName = "Item/Tool")]
public class ScriptableTools : ScritableInteractable
{
    public List<FastenerType> interactableFastenerList;
    public bool canUnfastenFasteners = false;

    public bool canOpenLatch = true;
    public bool canWedgeOpenTrunk = true;
    
    public AnimationType currentAnimation = AnimationType.NONE; 
}

[SerializeField]
public enum AnimationType
{
    NONE, 
    PRYING, 
    UNFASTENING 
}