using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "toolData", menuName = "Item/Tool")]
public class ScriptableTools : ScriptableInteractable
{
    public List<FastenerType> interactableFastenerList;
    public bool canUnfastenFasteners = false;
    public string unfastenAnimationClipName; 
    public bool canOpenLatch = true;
    public string openLatchAnimationClipName; 
    public bool canWedgeOpenTrunk = true;
    public string wedgeOpenTrunkAnimationClipName;
    
    public float toolTipOffset;

    // Moving in this direction would move the tool forward tip first
    public Vector3 toolForwardDirection; 
}