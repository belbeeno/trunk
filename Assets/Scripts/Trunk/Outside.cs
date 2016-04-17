using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outside : Interactable {

    public struct droppedItem
    {
        public string itemName;
        public Vector3 positionDropped;

        public droppedItem(string item, Vector3 position)
        {
            itemName = item;
            positionDropped = position; 
        }
    }

    private List<droppedItem> droppedItems = new List<droppedItem>();
    
	// Use this for initialization
	void Start () {
        canBeHeld = false;
	}
	
	// Update is called once per frame
	void Update () {
	
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
    #if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        for (int i = 0; i < droppedItems.Count; i++)
        {
            DrawAnX(droppedItems[i].positionDropped);
            Gizmos.DrawIcon(droppedItems[i].positionDropped, droppedItems[i].itemName, true);
        }
    }

    Vector3 upRight = new Vector3(1f, 0f, 1f);
    Vector3 downLeft = new Vector3(-1f, 0f, -1f);
    Vector3 upLeft = new Vector3(-1f, 0f, 1f);
    Vector3 downRight = new Vector3(1f, 0f, -1f);
    protected void DrawAnX(Vector3 position)
    {
        Gizmos.DrawLine(position + downRight, position + upLeft);
        Gizmos.DrawLine(position + upRight, position + downLeft);
    }
    #endif // UNITY_EDITOR
}
