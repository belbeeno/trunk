using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Outside : Interactable {

    public struct droppedItem
    {
        string itemName;
        Vector3 positionDropped;

        public droppedItem(string item, Vector3 position)
        {
            itemName = item;
            positionDropped = position; 
        }
    }

    private List<droppedItem> droppedItems; 

    
    
	// Use this for initialization
	void Start () {
        canBeHeld = false;
        droppedItems = new List<droppedItem>(); 
	}

    public void DropItem(string name, Vector3 position)
    {
        droppedItems.Add(new droppedItem(name, position));
    }

    public void DropItem(string name)
    {
        droppedItems.Add(new droppedItem(name, transform.position));

        Debug.Log(string.Format("itemName {0}, position {1}", name, transform.position));
    }

    public List<droppedItem> GetAllDroppedItems()
    {
        return droppedItems; 
    }

    public void Test(BaseEventData data)
    {
        //DebugConsole.SetText("object", data.ToString());
        var pointerData = data as PointerEventData;
        DebugConsole.SetText("gameobject", pointerData.pointerCurrentRaycast.gameObject.ToString());
        var outsidePosition = transform.InverseTransformPoint(pointerData.pointerCurrentRaycast.worldPosition); 
       
    }
}
