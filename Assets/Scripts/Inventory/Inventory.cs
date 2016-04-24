using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

// Keeps track of what item is currently being held and uses it on other items if possible
public class Inventory : MonoBehaviour {

    [SerializeField]
    private GameObject currentItem;

    private Interactable currentInteractable; 

    [SerializeField]
    private GameObject trunk = null;

    [SerializeField]
    private Transform cameraTransform = null; 

    [SerializeField]
    [Range(0, 10)]
    private float thrust = 6f;

    [SerializeField]
    private bool hasPhone;

    public Transform leftTarget = null;
    public Transform rightTarget = null;

    [SerializeField]
    private bool isAnimating;

    [SerializeField]
    [Range(0, 2)]
    private float toOutsideAnimationLength = 5f;

    private float toInventoryAnimationLength = 0.75f;

    // Use this for initialization
    void Start () {
        currentItem = null;
        hasPhone = false; 
    }
	
	// Update is called once per frame
	void Update () {
        var start = gameObject.transform.localRotation;
        var lstart = leftTarget.transform.localRotation;
        var rstart = rightTarget.transform.localRotation; 
        var camRotY = cameraTransform.localRotation.eulerAngles.y;

        var moveRight = (camRotY >= 0 && camRotY < 60) || (camRotY > 320 && camRotY <= 360);
        var moveLeft = (camRotY > 300 && camRotY < 360) || (camRotY >= 0 && camRotY < 20);

        var end = Quaternion.Euler(0, 0, camRotY);
        transform.localRotation = Quaternion.Slerp(start, end, Time.deltaTime*40);

        if (moveRight)
        {
            rightTarget.transform.localRotation = Quaternion.Slerp(rstart, end, Time.deltaTime * 40);
            rightTarget.GetChild(0).transform.LookAt(cameraTransform);
        }
        if (moveLeft)
        {
            leftTarget.transform.localRotation = Quaternion.Slerp(lstart, end, Time.deltaTime * 40);
            leftTarget.GetChild(0).transform.LookAt(cameraTransform);
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

            var targetInteractable = GetInteractable(item);
            if (IsHoldingItem() && targetInteractable != null)
            {
                // check if what we're holding can interact with what we're looking at
               return currentInteractable.CanInteractWith(targetInteractable);
            }

            return targetInteractable == null ? false : targetInteractable.CanBeHeld();

        }
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
                StartCoroutine(AnimateIntoPosession(item.transform, (leftTarget.GetChild(0) ?? transform), toInventoryAnimationLength, PickUpPhone));
            }
            return; 
        }
        // Pick up the item
        if (!IsHoldingItem())
        {
            var otherItem = GetInteractable(item);
            if (!otherItem.CanBeHeld())
            {
                return; 
            }
            otherItem.ItemSelected();
            HoldItem(item);
        } else
        {
            var itemInteract = GetInteractable(item);
            var curObjInteract = GetInteractable(currentItem);
            if (curObjInteract.CanInteractWith(itemInteract))
            {
                curObjInteract.InteractWith(itemInteract);
                if (itemInteract.GetType() == typeof(WedgeGap))
                {
                    currentItem = null; 
                }
            }
        }
    }

    public Interactable GetInteractable(GameObject item)
    {
       return item.GetComponent<Interactable>() ?? item.transform.parent.GetComponent<Interactable>();
    }

    public void GetOutsidePosition(BaseEventData data)
    {
        if (currentItem == null) return;

        //DebugConsole.SetText("object", data.ToString());
        var pointerData = data as PointerEventData;
        var outside = pointerData.pointerCurrentRaycast.gameObject;
        var outsidePosition = outside.transform.InverseTransformPoint(pointerData.pointerCurrentRaycast.worldPosition);
        currentItem.transform.SetParent(outside.transform,true);
        StartCoroutine(AnimateToOutside(outsidePosition, toOutsideAnimationLength, outside));
    }

    public void HoldItem(GameObject item)
    {
        currentItem = item;
        currentInteractable = GetInteractable(item);
        // moves item to left side of the screen, the place for all items being held

        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.detectCollisions = false; 
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        isAnimating = true;
        StopAllCoroutines();
        GetComponent<CardboardAudioSource>().Play(); 
        StartCoroutine(AnimateIntoPosession(item.transform, (rightTarget.GetChild(0) ?? transform), toInventoryAnimationLength));
    }

    public void DropItem()
    {
        if (!IsHoldingItem())
        {
            return;
        }
        StopAllCoroutines();
        currentInteractable.ItemDropped();

        // throws item in the direction currently facing
        //currentItem.transform.localScale = start.localScale;
        currentItem.transform.parent = trunk.transform;
        ThrowCurrentItem(cameraTransform.forward);
        isAnimating = false; 
    }

    private void ThrowCurrentItem(Vector3 direction)
    {
        var rigidBody = currentItem.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true;
        rigidBody.AddForce(direction * thrust, ForceMode.Impulse);
        currentItem = null;
        currentInteractable = null;
    }

    public void PickUpPhone(GameObject phone)
    {
        hasPhone = true;
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

    #region Animation Coroutines

    protected delegate void OnAnimationComplete(GameObject item);
    protected IEnumerator AnimateIntoPosession(Transform target
                                            , Transform newParent
                                            , float duration
                                            , OnAnimationComplete cb = null)
    {
        target.parent = newParent;
        Vector3 startPos = target.localPosition;
        Quaternion startRot = target.localRotation;
        var endRot = Quaternion.AngleAxis(target.GetComponent<Interactable>().itemData.rotationWhenInInventory, Vector3.forward);
        float timer = 0f;
        while (timer < duration)
        {
            target.localPosition = Vector3.Lerp(startPos, Vector3.zero, Ease.CircEaseInOut(timer, 0f, 1f, duration)); 
            target.localRotation = Quaternion.SlerpUnclamped(startRot, endRot, Ease.CircEaseInOut(timer, 0f, 1f, duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        isAnimating = false;
        if (cb != null) cb.Invoke(target.gameObject);
        
    }

    private IEnumerator AnimateToOutside(Vector3 finalLocalPosition, float duration, GameObject outside)
    {
        Vector3 startPos = currentItem.transform.localPosition;
        float timer = 0f;
        while (timer < duration)
        {
            currentItem.transform.localPosition = Vector3.Lerp(startPos, finalLocalPosition, Mathf.Clamp01(timer / duration));
            timer += Time.deltaTime;
            yield return 0;
        }
        isAnimating = false;
        currentInteractable.InteractWith(GetInteractable(outside));

        currentItem.transform.SetParent(null, true);
        ThrowCurrentItem(transform.parent.TransformDirection(Vector3.right));
    }
    
    #endregion
}
