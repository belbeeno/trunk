using UnityEngine;
using System.Collections;

public class InitEnableState : MonoBehaviour 
{
    public bool isEnabled = true;

	void Start () 
    {
        if (gameObject.activeSelf != isEnabled)
        {
            gameObject.SetActive(isEnabled);
        }
	}
}
