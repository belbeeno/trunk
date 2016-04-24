using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(VolumetricAudioSource))]
public class RiverVolumetricAudio : MonoBehaviour
{
    protected List<Vector3> points = new List<Vector3>();
    protected Vector3[] pointsArray = new Vector3[0];
    protected VolumetricAudioSource volumetricSource = null;

    public float maxDistFromRiver = 50f;

    void Start()
    {
        volumetricSource = GetComponent<VolumetricAudioSource>();
    }

    public void BuildFromRiverGraph(RiverGraph riverGraph)
    {
        points.Clear();
        riverGraph.GetEdges()
            .ForEach(e => 
            {
                points.Add(e.from.pos);
            }
        );
        points.RemoveAt(points.Count - 1);
        pointsArray = points.ToArray();
    }

    void Update()
    {
        if (pointsArray.Length > 0)
        {
            volumetricSource.MoveTowardsWhileCloseTo(Camera.main.transform.position, maxDistFromRiver, pointsArray);
        }
    }

    public readonly Color VolumeWireframeGizmoColor = new Color(0.75f, 0.75f, 0f);
    public void OnDrawGizmosSelected()
    {
        if (pointsArray.Length < 2) return;

        Gizmos.color = VolumeWireframeGizmoColor;
        for (int i = 0; i < pointsArray.Length-1; i++)
        {
            Gizmos.DrawLine(pointsArray[i] + Vector3.up, pointsArray[(i + 1) % pointsArray.Length] + Vector3.up);
        }
    }
}
