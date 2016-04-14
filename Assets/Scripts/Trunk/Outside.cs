﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

}