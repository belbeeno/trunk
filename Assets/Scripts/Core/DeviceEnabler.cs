using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DeviceEnabler : MonoBehaviour
{
    public UnityEvent OnIsNotDesktop;
    public UnityEvent OnIsNotPhone;
	// Use this for initialization
	void Start () 
    {
	    if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            OnIsNotPhone.Invoke();
        }
        if (SystemInfo.deviceType != DeviceType.Desktop)
        {
            OnIsNotDesktop.Invoke();
        }

        enabled = false;
	}
}
