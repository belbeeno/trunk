using UnityEngine;
using System.Collections;
using System;

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

    public Transform possessionTarget = null;
    private Transform start;
    [SerializeField]
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
            //currentItem.transform.localScale = Vector3.Lerp(start.localScale, targetScale, t);
            currentItem.transform.localRotation = Quaternion.Slerp(start.localRotation, targetRotation, t);
            if (t >= 1)
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
        if (isAnimating)
        {
            return false; 
        }
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

    protected delegate void OnAnimationComplete(GameObject item);
    protected IEnumerator AnimateIntoPosession(Transform target
                                            , Transform newParent
                                            , float duration
                                            , OnAnimationComplete cb = null)
    {
        Vector3 startPos = target.position;
        Quaternion startRot = target.localRotation;
        Quaternion randoRot = UnityEngine.Random.rotation;
        float timer = 0f;
        while (timer < duration)
        {
            target.position = Vector3.Lerp(target.position, newParent.position, Mathf.Clamp01(timer / duration));
            
            if (timer < duration / 2f)
            {
                target.localRotation = Quaternion.SlerpUnclamped(startRot, randoRot, Ease.CircEaseInOut(timer, 0f, 1f, duration / 2f));
            }
            else
            {
                target.localRotation = Quaternion.SlerpUnclamped(randoRot, newParent.localRotation, Ease.CircEaseOutIn(timer - duration / 2f, 0f, 1f, duration / 2f));
            }
            timer += Time.deltaTime;
            yield return 0;
        }
        target.SetParent(newParent, false);

        if (cb != null) cb.Invoke(target.gameObject);
    }

    private IEnumerator StartAnimateIntoView(Transform start, Transform final, float duration)
    {
        Vector3 startPos = start.position;
        float timer = 0f;
        while (timer < duration)
        {
            start.position = Vector3.Lerp(start.position, final.position, Mathf.Clamp01(timer / duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        
    }


    public void InteractWithItem(GameObject item)
    {            
        if (!hasPhone)
        {
            if (item.GetComponent<CellPhone>())
            {
                Collider col = item.GetComponent<Collider>();
                col.attachedRigidbody.useGravity = false;
                col.enabled = false;
                StopAllCoroutines();
                StartCoroutine(AnimateIntoPosession(item.transform, (possessionTarget != null ? possessionTarget : transform), 2f, PickUpPhone));
            }
            return; 
        }
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
                if (itemInteract.GetType() == typeof(Outside))
                {
                    currentItem = null; 
                }
            }
        }
    }

    public void HoldItem(GameObject item)
    {
        currentItem = item;

        // moves item to left side of the screen, the place for all items being held
       
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        StopAllCoroutines();
        StartCoroutine(AnimateIntoPosession(item.transform, (possessionTarget != null ? possessionTarget : transform), 2f, StartAnimateIntoView));
        // currentItem.transform.parent = gameObject.transform;
        //start = currentItem.transform; 
        //var curScale = currentItem.transform.localScale;
        //targetScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        ////currentItem.transform.localScale = new Vector3(scaleFactor * curScale.x, scaleFactor * curScale.y, scaleFactor * curScale.z);
        //targetRotation = Quaternion.Euler(item.GetComponent<Interactable>().inHandOrientation);
        //isAnimating = true;
        //time = 0f; 
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
        //currentItem.transform.localScale = start.localScale;
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.AddForce(transform.forward * thrust, ForceMode.Impulse);
        currentItem.transform.parent = trunk.transform;
        currentItem = null;
        isAnimating = false; 
    }

    public void StartAnimateIntoView(GameObject item)
    {
        StopAllCoroutines();
        var siobject = item.GetComponent<ScriptableInteractable>(); 
        StartCoroutine(StartAnimateIntoView((possessionTarget != null ? possessionTarget : transform), siobject.transformWhenInInventory, 1f));
    }

    public void PickUpPhone(GameObject phone)
    {
        hasPhone = true;
        phone.SetActive(false);
        phone.GetComponent<CellPhone>().CallOperator(); 
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
