using UnityEngine;
using System.Collections;

public class StopItemsFromMoving : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision other)
    {   
        var item = other.gameObject; 
        if (item.GetComponent<Interactable>() != null)
        {
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            item.transform.localScale *= 7;

            item.transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);
        }
    }
}
