using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CardboardAudioSource))]
public class VolumetricAudioSource : MonoBehaviour
{
    public class LineSegment
    {
        public Vector2 start;
        public Vector2 end;

        public Vector2 Vector { get { return (end - start); } }

        public LineSegment()
        {
            start = new Vector2();
            end = new Vector2();
        }
    }
    public enum Winding
    {
        Clockwise = 0,
        CounterClockwise
    }
    public Winding winding = Winding.Clockwise;
    public enum FreezeAxis
    {
        X,
        Y,
        Z,
    }
    public FreezeAxis axisToFreeze = FreezeAxis.Y;

    protected CardboardAudioSource _source = null;
    public CardboardAudioSource Source
    {
        get
        {
            if (_source == null)
            {
                _source = GetComponent<CardboardAudioSource>();
            }
            return _source;
        }
    }

    public void MoveTowardsWhileInside(Vector3 target, params Vector3[] verts)
    {
        float minDistSqrd = float.MaxValue;
        Vector2 minPoint = new Vector2();
        LineSegment segment = new LineSegment();
        //int winningSegment = 0;
        float sqrMag = float.MaxValue;
        Vector2 closest = new Vector2();
        bool isInside = verts.Length >= 3;
        float side = 0f;
        Vector2 frozenTarget = new Vector2();

        switch (axisToFreeze)
        {
            case FreezeAxis.X:
                frozenTarget.Set(target.y, target.z);
                break;
            case FreezeAxis.Y:
                frozenTarget.Set(target.x, target.z);
                break;
            case FreezeAxis.Z:
                frozenTarget.Set(target.x, target.y);
                break;
        }

        for (int i = 0; i < verts.Length; i++)
        {
            switch (axisToFreeze)
            {
                case FreezeAxis.X:
                    segment.start.Set(verts[i].y, verts[i].z);
                    segment.end.Set(verts[(i + 1) % verts.Length].y, verts[(i + 1) % verts.Length].z);
                    break;
                case FreezeAxis.Y:
                    segment.start.Set(verts[i].x, verts[i].z);
                    segment.end.Set(verts[(i + 1) % verts.Length].x, verts[(i + 1) % verts.Length].z);
                    break;
                case FreezeAxis.Z:
                    segment.start.Set(verts[i].x, verts[i].y);
                    segment.end.Set(verts[(i + 1) % verts.Length].x, verts[(i + 1) % verts.Length].y);
                    break;
            }

            if (isInside)
            {
                side = ((segment.Vector.x) * (frozenTarget.y - segment.start.y) - (segment.Vector.y) * (frozenTarget.x - segment.start.x));
                isInside &= (side == 0)
                        || (side < 0 && winding == Winding.CounterClockwise)
                        || (side > 0 && winding == Winding.Clockwise);
            }

            if (Mathf.Approximately(segment.Vector.sqrMagnitude, 0f))
            {
                // Points are coincident
                sqrMag = (frozenTarget - segment.start).sqrMagnitude;
                closest = segment.start;
            }
            else
            {
                float t = ((frozenTarget.x - segment.start.x) * segment.Vector.x
                    + (frozenTarget.y - segment.start.y) * segment.Vector.y)
                    / segment.Vector.sqrMagnitude;

                if (t < 0)
                {
                    closest = segment.start;
                }
                else if (t > 1)
                {
                    closest = segment.end;
                }
                else
                {
                    closest = new Vector2(segment.start.x + t * segment.Vector.x
                                            , segment.start.y + t * segment.Vector.y);
                }
                sqrMag = (frozenTarget - closest).sqrMagnitude;
            }

            if (sqrMag < minDistSqrd)
            {
                minPoint = closest;
                minDistSqrd = sqrMag;
                //winningSegment = i;
            }
        }

        if (isInside)
        {
            minPoint = frozenTarget;
        }
        /*
        Debug.DrawLine(new Vector3(minPoint.x - 1f, minPoint.y - 1f, 0f), new Vector3(minPoint.x + 1f, minPoint.y + 1f, 0f), Color.green);
        Debug.DrawLine(new Vector3(minPoint.x - 1f, minPoint.y + 1f, 0f), new Vector3(minPoint.x + 1f, minPoint.y - 1f, 0f), Color.green);
        Debug.DrawLine(new Vector3(point.x, point.y, 0f), new Vector3(minPoint.x, minPoint.y, 0f), Color.blue);
        Debug.DrawLine(new Vector3(segments[winningSegment].Start.x, segments[winningSegment].Start.y, 0f)
                    , new Vector3(segments[winningSegment].End.x, segments[winningSegment].End.y, 0f)
                    , Color.cyan);
        //*/
        switch (axisToFreeze)
        {
            case FreezeAxis.X:
                transform.position = new Vector3(0f, minPoint.x, minPoint.y);
                break;
            case FreezeAxis.Y:
                transform.position = new Vector3(minPoint.x, 0f, minPoint.y);
                break;
            case FreezeAxis.Z:
                transform.position = new Vector3(minPoint.x, minPoint.y, 0f);
                break;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "sound.png");
    }
}
