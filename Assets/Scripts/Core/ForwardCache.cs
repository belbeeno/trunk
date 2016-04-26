using System;
using UnityEngine;

class ForwardCache : MonoBehaviour
{
    float timestamp = 0f;
    Vector3 cachedForward = new Vector3();
    public Vector3 Forward
    {
        get
        {
            if (Time.time - timestamp < Mathf.Epsilon)
            {
                timestamp = Time.time;
                cachedForward = transform.forward;
            }
            return cachedForward;
        }
    }

    void Start()
    {
        timestamp = Time.time;
    }
}