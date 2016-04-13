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

    private bool hasPhone;

    private Transform start;
    private bool isAnimating;
    private float animationLength = 1f;
    private float time;
    private Vector3 targetScale;
    private Quaternion targetRotation; 

    // Use this for initialization
    void Start () {
        currentItem = null;
        hasPhone = false; 
    }
	
	// Update is called once per frame
	void Update () {
        if (isAnimating)
        {
            time += Time.deltaTime;
            var t = Mathf.Min(time / animationLength, 1);
            currentItem.transform.localPosition = Vector3.Lerp(start.localPosition, new Vector3(0, 0, 0), t);
            currentItem.transform.localScale = Vector3.Lerp(start.localScale, targetScale, t);
            currentItem.transform.localRotation = Quaternion.Slerp(start.localRotation, targetRotation, t);
            if (t == 1)
            {
                isAnimating = false; 
            }
        }

    }

    public GameObject GetCurrentItem()
    {
        return currentItem; 
    }

    // returns true if: 
    // currently not holding anything and item can be held
    // currently holding something and the two items can interact 
    public bool CanInteractWith(GameObject item)
    {
        if (!HasPhone())
        {
            return item.GetComponent<CellPhone>() != null;
        }
        else {

            var targetInteractable = item.GetComponent<Interactable>();
            if (IsHoldingItem() && targetInteractable != null)
            {
                // check if what we're holding can interact with what we're looking at
                var currentInteractable = currentItem.GetComponent<Interactable>();
                Debug.Log("currentItem " + currentItem);
                return currentInteractable.CanInteractWith(targetInteractable);
            }

            return targetInteractable == null ? false : targetInteractable.CanBeHeld();

        }
    }

    public void InteractWithItem(GameObject item)
    {            
        // Pick up the item
        if (!IsHoldingItem())
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
        start = currentItem.transform; 
        var curScale = currentItem.transform.localScale;
        targetScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        //currentItem.transform.localScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        targetRotation = Quaternion.Euler(item.GetComponent<Interactable>().inHandOrientation);
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        isAnimating = true;
        time = 0f; 
    }

    public void DropItem()
    {
        if (!IsHoldingItem())
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

    public void PickUpPhone(GameObject item)
    {
        hasPhone = true; 
    }

    public bool IsHoldingItem()
    {
        return currentItem != null; 
    }
    
    public bool HasPhone()
    {
        return hasPhone; 
    }
}
