using UnityEngine;

// Keeps track of what item is currently being held and uses it on other items if possible
public class Inventory : MonoBehaviour {

    [SerializeField]
    private GameObject currentItem;

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
        currentItem = null;
    }
	
	// Update is called once per frame
	void Update () {
    }

    public GameObject GetCurrentItem()
    {
        return currentItem; 
    }

    public void InteractWithItem(GameObject item)
    {            
        // Pick up the item
        if (isEmpty())
        {
            var otherItem = item.GetComponent<Interactable>();
            if (!otherItem.CanBeHeld())
            {
                return; 
            }
            otherItem.ItemSelected();
            HoldItem(item);
        } else
        {
            var itemInteract = item.GetComponent<Interactable>(); 
            var curObjInteract = currentItem.GetComponent<Interactable>();
            if (curObjInteract.CanInteractWith(itemInteract))
            {
                curObjInteract.InteractWith(itemInteract); 
            }
        }
    }

    public void HoldItem(GameObject item)
    {
        currentItem = item;

        // moves item to left side of the screen, the place for all items being held
        currentItem.transform.parent = gameObject.transform; 
        currentItem.transform.localPosition = new Vector3(0, 0, 0);
        var curScale = currentItem.transform.localScale;
        currentItem.transform.localScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        currentItem.transform.localRotation = Quaternion.Euler(-90, 0,  0);
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
    }

    public void DropItem()
    {
        if (isEmpty())
        {
            return;
        }
        
        var interactable = currentItem.GetComponent<Interactable>();
        interactable.ItemDropped();

        // throws item in the direction currently facing
        var curScale = currentItem.transform.localScale;
        currentItem.transform.localScale = new Vector3(curScale.x / scaleFactor, curScale.y / scaleFactor, curScale.z / scaleFactor);
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.AddForce(transform.forward * thrust, ForceMode.Impulse);
        currentItem.transform.parent = trunk.transform;
        currentItem = null;
    }

    public bool isEmpty()
    {
        return currentItem == null; 
    }
   
}
