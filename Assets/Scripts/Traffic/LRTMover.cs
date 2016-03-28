using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class LRTMover : MonoBehaviour 
{
    [SerializeField]
    MeshRenderer rend = null;
    public Vector3 travelFrom = new Vector3();
    public Vector3 travelTo = new Vector3();

    public float duration = 10f;
    public float fadeDistance = 0.5f;

    protected float timer = 0f;
    public float timerOffset = 0.5f;

	// Use this for initialization
	void Start () 
    {
        if (rend == null)
            rend = GetComponent<MeshRenderer>();

        transform.position = travelFrom;
        rend.material.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () 
    {
        float offsetTime = timer + timerOffset;
        float t = (offsetTime % duration) / duration;
        transform.position = Vector3.Lerp(travelFrom, travelTo, t);
        if (t < 0.5f)
        {
            rend.material.color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp01(offsetTime / fadeDistance));
        }
        else
        {
            rend.material.color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp01((duration - offsetTime) / fadeDistance));
        }

        timer = (timer + Time.deltaTime) % duration;
	}

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(travelFrom, travelTo);
        Gizmos.DrawCube((travelTo - travelFrom) * (fadeDistance / duration) + travelFrom, Vector3.one);
        Gizmos.DrawCube((travelTo - travelFrom) * ((duration - fadeDistance) / duration) + travelFrom, Vector3.one);
    }

    

}
