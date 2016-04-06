using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

    [SerializeField]
    private GameObject currentObject=null;
    public int test = 0;

    [SerializeField]
    private GameObject trunk;

    [SerializeField]
    [Range(0, 1)]
    private float scaleFactor=.5f;

    [SerializeField]
    [Range(0, 10)]
    private float thrust = 6f; 

	// Use this for initialization
	void Start () {
        currentObject = null;
    }
	
	// Update is called once per frame
	void Update () {
    }

    public GameObject GetCurrentObject()
    {
        return currentObject; 
    }

    public void InteractWithItem(GameObject item)
    {            
        // Pick up the item
        if (isEmpty())
        {
            var interactable = item.GetComponent<Interactable>();
            if (!interactable.CanBeHeld())
            {
                return; 
            }
            interactable.ItemSelected();
            HoldItem(item);
        } else
        {
            var itemInteract = item.GetComponent<Interactable>(); 
            var curObjInteract = currentObject.GetComponent<Interactable>();
            if (curObjInteract.CanInteractWith(itemInteract))
            {
                curObjInteract.InteractWith(itemInteract); 
            }
        }
    }

    public void HoldItem(GameObject item)
    {
        currentObject = item;
        currentObject.transform.parent = gameObject.transform; 
        currentObject.transform.localPosition = new Vector3(0, 0, 0);
        var curScale = currentObject.transform.localScale;
        currentObject.transform.localScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        currentObject.transform.localRotation = Quaternion.Euler(-90, 0,  0);
        var rigidBody = currentObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
    }

    public void DropItem()
    {
        if (isEmpty())
        {
            return;
        }
        // Drop the item
        var interactable = currentObject.GetComponent<Interactable>();
        interactable.ItemDropped();

        var curScale = currentObject.transform.localScale;
        currentObject.transform.localScale = new Vector3(curScale.x / scaleFactor, curScale.y / scaleFactor, curScale.z / scaleFactor);
        var rigidBody = currentObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.AddForce(transform.forward * thrust, ForceMode.Impulse);
        currentObject.transform.parent = trunk.transform;
        currentObject = null;
    }

    public bool isEmpty()
    {
        return currentObject == null; 
    }
   
}
