using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VolumetricAudioSource))]
public class ManualVolumetricAudio : MonoBehaviour
{
    public Vector3[] points = new Vector3[0];
    protected VolumetricAudioSource volumetricSource = null;

    private float farthestDistFromCenterSquared = 0f;
    private Vector3 averagePoint = new Vector3();

    void Start()
    {
        volumetricSource = GetComponent<VolumetricAudioSource>();

        farthestDistFromCenterSquared = 0f;
        averagePoint = points.Average();
        for (int i = 0; i < points.Length; i++)
        {
            farthestDistFromCenterSquared = Mathf.Max((points[i] - averagePoint).sqrMagnitude, farthestDistFromCenterSquared);
        }
        farthestDistFromCenterSquared += volumetricSource.Source.maxDistance * volumetricSource.Source.maxDistance * 2f;
    }

    void Update()
    {
        if (points.Length > 0)
        {
            if ((Camera.main.transform.position - averagePoint).sqrMagnitude < farthestDistFromCenterSquared)
            {
                volumetricSource.MoveTowardsWhileInside(Camera.main.transform.position, points);
            }
        }
    }

    public readonly Color VolumeWireframeGizmoColor = new Color(0.75f, 0f, 0f);
    public void OnDrawGizmosSelected()
    {
        if (points.Length < 2) return;

        Gizmos.color = VolumeWireframeGizmoColor;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i] + Vector3.up, points[(i + 1) % points.Length] + Vector3.up);
        }
    }
}
