using UnityEngine;
using System.Collections;

public class StopItemsFromMoving : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {   
        var item = other.gameObject; 
        if (item.GetComponent<Interactable>() != null)
        {
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            item.transform.localScale *= 2;

            item.transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);
        }
    }
}
