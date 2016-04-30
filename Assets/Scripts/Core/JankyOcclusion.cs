using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JankyOcclusion : MonoBehaviour
{
    public bool makeRaysVisible = false;
    public int maxDistance = 200;
    /*
    private readonly float[] rayGridY = new float[] {
          -(Mathf.PI / 2f) *  5f / 15f
        , -(Mathf.PI / 2f) *  4f / 15f
        , -(Mathf.PI / 2f) *  3f / 15f
        , -(Mathf.PI / 2f) *  2f / 15f
        , -(Mathf.PI / 2f) *  1f / 15f
        , 0f
        , (Mathf.PI / 2f) *  1f / 15f
        , (Mathf.PI / 2f) *  2f / 15f
        , (Mathf.PI / 2f) *  3f / 15f
        , (Mathf.PI / 2f) *  4f / 15f
        , (Mathf.PI / 2f) *  5f / 15f
    };
    private readonly float[] rayGridX = new float[] { 
          -(Mathf.PI / 2f)
        , -(Mathf.PI / 2f) * 14f / 15f
        , -(Mathf.PI / 2f) * 13f / 15f
        , -(Mathf.PI / 2f) * 12f / 15f
        , -(Mathf.PI / 2f) * 11f / 15f
        , -(Mathf.PI / 2f) * 10f / 15f
        , -(Mathf.PI / 2f) *  9f / 15f
        , -(Mathf.PI / 2f) *  8f / 15f
        , -(Mathf.PI / 2f) *  7f / 15f
        , -(Mathf.PI / 2f) *  6f / 15f
        , -(Mathf.PI / 2f) *  5f / 15f
        , -(Mathf.PI / 2f) *  4f / 15f
        , -(Mathf.PI / 2f) *  3f / 15f
        , -(Mathf.PI / 2f) *  2f / 15f
        , -(Mathf.PI / 2f) *  1f / 15f
        , 0f
        , (Mathf.PI / 2f) *  1f / 15f
        , (Mathf.PI / 2f) *  2f / 15f
        , (Mathf.PI / 2f) *  3f / 15f
        , (Mathf.PI / 2f) *  4f / 15f
        , (Mathf.PI / 2f) *  5f / 15f
        , (Mathf.PI / 2f) *  6f / 15f
        , (Mathf.PI / 2f) *  7f / 15f
        , (Mathf.PI / 2f) *  8f / 15f
        , (Mathf.PI / 2f) *  9f / 15f
        , (Mathf.PI / 2f) * 10f / 15f
        , (Mathf.PI / 2f) * 11f / 15f
        , (Mathf.PI / 2f) * 12f / 15f
        , (Mathf.PI / 2f) * 13f / 15f
        , (Mathf.PI / 2f) * 14f / 15f
        , (Mathf.PI / 2f)};
    */
    private readonly float[] rayGridY = new float[] {
          -(90f) *  15f / 15f
        , -(90f) *  12f / 15f
        , -(90f) *  9f / 15f
        , -(90f) *  6f / 15f
        , -(90f) *  3f / 15f
        , 0f
        , (90f) *  3f / 15f
        , (90f) *  6f / 15f
        , (90f) *  9f / 15f
        , (90f) *  12f / 15f
        , (90f) *  15f / 15f
    };
    private readonly float[] rayGridX = new float[] { 
          -(90f)
        , -(90f) * 14f / 15f
        , -(90f) * 13f / 15f
        , -(90f) * 12f / 15f
        , -(90f) * 11f / 15f
        , -(90f) * 10f / 15f
        , -(90f) *  9f / 15f
        , -(90f) *  8f / 15f
        , -(90f) *  7f / 15f
        , -(90f) *  6f / 15f
        , -(90f) *  5f / 15f
        , -(90f) *  4f / 15f
        , -(90f) *  3f / 15f
        , -(90f) *  2f / 15f
        , -(90f) *  1f / 15f
        , 0f
        , (90f) *  1f / 15f
        , (90f) *  2f / 15f
        , (90f) *  3f / 15f
        , (90f) *  4f / 15f
        , (90f) *  5f / 15f
        , (90f) *  6f / 15f
        , (90f) *  7f / 15f
        , (90f) *  8f / 15f
        , (90f) *  9f / 15f
        , (90f) * 10f / 15f
        , (90f) * 11f / 15f
        , (90f) * 12f / 15f
        , (90f) * 13f / 15f
        , (90f) * 14f / 15f
        , (90f)};
    public LayerMask mask;

    private Camera cachedCamera = null;
    public Camera Cam
    {
        get
        {
            if (cachedCamera == null) cachedCamera = GetComponent<Camera>();
            return cachedCamera;
        }
    }

    void OnEnable()
    {
        StartCoroutine(SweepCast());
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator SweepCast()
    {
        WaitForSeconds pause = new WaitForSeconds(0.1f);
        List<GameObject> hits = new List<GameObject>();
        int y = 0;
        Vector3 forward;
        while (true)
        {
            y = ++y % rayGridY.Length;
            hits.Clear();
            forward = transform.forward;
            for (int x = 0; x < rayGridX.Length; x++) //(float x in rayGridX)
            {
                GameObject go = CastOcclusionRay(forward, rayGridX[x], rayGridY[(x + y) % rayGridY.Length]);
                if (go != null) hits.Add(go);
            }

            yield return 0;

            for (int i = 0; i < hits.Count; i++ )
            {
                hits[i].SendMessage("SetVisible", 3f);
            }

            yield return 0;
        }
    }

    GameObject CastOcclusionRay(Vector3 centerDir, float angleX, float angleY)
    {
        //Ray ray = Cam.ViewportPointToRay(new Vector3(graphX, graphY, 0));
        Quaternion rot = Quaternion.Euler(angleY, angleX, 0f);
        Vector3 dir = rot * centerDir;
        /*
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        dir.Set(centerDir.x * cos - centerDir.z * sin
            ,   0f
            ,   centerDir.x * sin + centerDir.z * cos);
        */
        Ray ray = new Ray(transform.position, dir);

        if (makeRaysVisible == true) Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, mask))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }
}