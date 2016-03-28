using UnityEngine;
using System.Collections;

public class DebugLabel : MonoBehaviour 
{
    void Start()
    {
        if (!Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
	}
}
