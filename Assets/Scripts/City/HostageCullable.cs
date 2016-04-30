using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HostageCullable : MonoBehaviour
{
    private Vector3 centerPos = new Vector3();
    //private bool isInitialized = false;
    private bool wasEnabled = true;
    public Vector3 CenterPos
    {
        get
        {
            return centerPos;
        }
        set
        {
            //isInitialized = true;
            centerPos = value;
        }
    }

    private List<MeshRenderer> meshes = new List<MeshRenderer>();
    List<MeshRenderer> Meshes
    {
        get
        {
            if (meshes.Count == 0)
            {
                meshes.AddRange(GetComponents<MeshRenderer>());
                meshes.AddRange(GetComponentsInChildren<MeshRenderer>());
            }

            return meshes;
        }
    }

    //TrunkMover mover = null;
    ForwardCache camFwd = null;
    ForwardCache trunkFwd = null;
    private readonly float halfCullRad = (GameSettings.HOSTAGE_CULLING_RADIUS / 2f);
    private readonly float sqrCullRad = (GameSettings.HOSTAGE_CULLING_RADIUS * GameSettings.HOSTAGE_CULLING_RADIUS);

    Bounds totalBounds = new Bounds();

    public BoxCollider CreateCollider()
    {
        totalBounds.center = CenterPos;
        for (int i = 0; i < Meshes.Count; i++)
        {
            totalBounds.Encapsulate(Meshes[i].bounds);
        }

        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.center = totalBounds.center;
        col.size = totalBounds.size;
        col.isTrigger = true;

        return col;
    }

    void Start()
    {

        camFwd = Camera.main.GetComponent<ForwardCache>();
        trunkFwd = FindObjectOfType<TrunkMover>().GetComponent<ForwardCache>();

        SetMeshEnabled(false);
    }

    Coroutine thread = null;
    float visTimer = 0f;
    public void SetVisible(float duration)
    {
        visTimer = duration;

        if (thread == null)
        {
            thread = StartCoroutine(SetVisibleCoroutine());
        }
    }
    public IEnumerator SetVisibleCoroutine()
    {
        SetMeshEnabled(true);
        while (visTimer > 0f)
        {
            visTimer -= Time.deltaTime;
            yield return 0;
        }
        SetMeshEnabled(false);
        thread = null;
    }

    void SetMeshEnabled(bool isEnabled)
    {
        if (isEnabled ^ wasEnabled)
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].enabled = isEnabled;
            }
            wasEnabled = isEnabled;
        }
    }

    bool DotTest()
    {
        return Vector3.Dot(trunkFwd.Forward, transform.position - Camera.main.transform.position) > 0f;
    }

    bool DistTest()
    {
        return ((Camera.main.transform.position + camFwd.Forward * halfCullRad) - centerPos).sqrMagnitude < sqrCullRad;
    }
    /*
    void Update()
    {
        if (!isInitialized || !meshs[0].isVisible) return;

        bool dotTest = false;//DotTest();
        bool distTest = DistTest();
    
        if ((dotTest || distTest) && !wasEnabled)
        {
            for (int i = 0; i < meshs.Count; i++)
            {
                meshs[i].enabled = true;
            }
            wasEnabled = true;
        }
        else if (!(dotTest || distTest) && wasEnabled)
        {
            for (int i = 0; i < meshs.Count; i++)
            {
                meshs[i].enabled = false;
            }
            wasEnabled = false;
        }
    }
    */
}