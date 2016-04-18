using UnityEngine;
using System.Collections;
using System;

// Keeps track of what item is currently being held and uses it on other items if possible
public class Inventory : MonoBehaviour {

    [SerializeField]
    private GameObject currentItem;

    [SerializeField]
    private GameObject trunk = null;

    [SerializeField]
    private Transform cameraTransform = null; 

    [SerializeField]
    [Range(0, 10)]
    private float thrust = 6f;

    private bool hasPhone;

    public Transform possessionTarget = null;
    [SerializeField]
    private bool isAnimating;

    [SerializeField]
    [Range(0, 2)]
    private float animationLength = 1f;

    // Use this for initialization
    void Start () {
        currentItem = null;
        hasPhone = false; 
    }
	
	// Update is called once per frame
	void Update () {
        var start = gameObject.transform.localRotation;
        var end = Quaternion.Euler(0, 0, cameraTransform.eulerAngles.y);
        gameObject.transform.localRotation = Quaternion.Slerp(start, end, Time.deltaTime*40);
        //gameObject.transform.localRotation = Quaternion.Euler(0, 0, cameraTransform.eulerAngles.y);
        ////rotation -> camera world position - object world position; 
        //quaternion.lookrotation();
        ////position a
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
        Transform targetParent = target.parent.transform; 
        Vector3 startPos = target.localPosition;
        Quaternion startRot = target.localRotation;
        Quaternion randoRot = UnityEngine.Random.rotation;
        float timer = 0f;
        Vector3 carSpaceTargetPosition = new Vector3();
        while (timer < duration)
        {
            carSpaceTargetPosition = targetParent.localToWorldMatrix.MultiplyPoint(startPos);
            target.position = Vector3.Lerp(carSpaceTargetPosition, newParent.position, Mathf.Clamp01(timer / duration));
            target.localRotation = Quaternion.SlerpUnclamped(startRot, newParent.localRotation, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        target.SetParent(newParent, true);
        Debug.Log("Done Animating");

        if (cb != null) cb.Invoke(target.gameObject);
    }

    private IEnumerator AnimateIntoView(Transform itemToAnimateTransform, Vector3 finalLocalPosition, float duration)
    {
        Vector3 startPos = itemToAnimateTransform.localPosition;
        Debug.Log("animating into view " + itemToAnimateTransform.ToString());
        float timer = 0f;
        while (timer < duration)
        {
            itemToAnimateTransform.localPosition = Vector3.Lerp(startPos, finalLocalPosition, Mathf.Clamp01(timer / duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        isAnimating = false; 
        
    }


    public void InteractWithItem(GameObject item)
    {   
        if (isAnimating)
        {
            return;
        }
        if (!hasPhone)
        {
            if (item.GetComponent<CellPhone>())
            {
                Collider col = item.GetComponent<Collider>();
                col.attachedRigidbody.useGravity = false;
                col.enabled = false;
                StopAllCoroutines();
                StartCoroutine(AnimateIntoPosession(item.transform, (possessionTarget != null ? possessionTarget : transform), animationLength, PickUpPhone));
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
        rigidBody.detectCollisions = false; 
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        isAnimating = true;
        StopAllCoroutines();
        StartCoroutine(AnimateIntoPosession(item.transform, (possessionTarget != null ? possessionTarget : transform), animationLength, StartAnimateIntoView));
    }

    public void DropItem()
    {
        if (!IsHoldingItem())
        {
            return;
        }
        StopAllCoroutines(); 
        var interactable = currentItem.GetComponent<Interactable>();
        interactable.ItemDropped();

        // throws item in the direction currently facing
        //currentItem.transform.localScale = start.localScale;
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true; 
        rigidBody.AddForce(transform.forward * thrust, ForceMode.Impulse);
        currentItem.transform.parent = trunk.transform;
        currentItem = null;
        isAnimating = false; 
    }

    public void StartAnimateIntoView(GameObject item)
    {
        StopAllCoroutines();
        var siobject = item.GetComponent<Interactable>().itemData;
        item.transform.localRotation = Quaternion.Euler(siobject.rotationWhenInInventory);
        StartCoroutine(AnimateIntoView(item.transform, siobject.positionWhenInInventory, animationLength));
    }

    public void PickUpPhone(GameObject phone)
    {
        StartAnimateIntoView(phone);
        hasPhone = true;
        Debug.Log("Hasphone");
        phone.GetComponent<CellPhone>().CallOperator();
        StartCoroutine(AnimateIntoPosession(phone.transform, (possessionTarget != null ? possessionTarget : transform), animationLength, SetPhoneInactive));
        phone.SetActive(false);
    }

    private void SetPhoneInactive(GameObject phone)
    {
        phone.SetActive(false);
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
