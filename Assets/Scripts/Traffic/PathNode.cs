using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif
public class PathNode : MonoBehaviour
{
    public enum Behaviour
    {
        Straight,
        TurnLeft,
        TurnRight,
        End,
    }

    public Behaviour behaviourToNext = Behaviour.Straight;

    [SerializeField]
    private PathNode _nextNode = null;
    public PathNode NextNode
    {
        get
        {
            if (behaviourToNext != Behaviour.End && _nextNode == null)
            {
                int myIdx = transform.GetSiblingIndex();
                if (transform.parent.childCount > myIdx + 1)
                {
                    _nextNode = transform.parent.GetChild(myIdx + 1).GetComponent<PathNode>();
                }
            }
            return _nextNode;
        }
    }
    public Vector3 NextNodePos
    {
        get
        {
            if (NextNode != null)
            {
                return NextNode.transform.position;
            }
            return transform.position;
        }
    }
    private PathNode _prevNode = null;
    public PathNode PrevNode
    {
        get
        {
            if (_prevNode == null)
            {
                int myIdx = transform.GetSiblingIndex();
                if (myIdx >= 0)
                {
                    _prevNode = transform.parent.GetChild(myIdx - 1).GetComponent<PathNode>();
                }
            }
            return _prevNode;
        }
    }
    public Vector3 PrevNodePos
    {
        get
        {
            if (PrevNode != null)
            {
                return PrevNode.transform.position;
            }
            return transform.position;
        }
    }

    public void Start()
    {
        // Manhattan distance is good enough for our game!
        duration = (Mathf.Abs(NextNodePos.x - transform.position.x) + Mathf.Abs(NextNodePos.z - transform.position.z)) / speedLimitUPS;
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Trunk/Create Path Node", false, 100)]
    public static void CreatePathNode()
    {
        GameObject parentGO = Selection.activeGameObject;
        Transform parent = null;
        int pathID = 0;
        string pathName = "Path Node";
        if (parentGO != null)
        {
            if (parentGO.GetComponent<PathNode>() != null)
            {
                parent = parentGO.transform.parent;
                parentGO = parent.gameObject;
            }
            else
            {
                parent = parentGO.transform;
            }
            List<PathNode> nodes = new List<PathNode>(parent.GetComponentsInChildren<PathNode>());
            IEnumerable<PathNode> pathIter = nodes.OrderBy(x => x.name);
            bool newPathName = true;
            do
            {
                newPathName = true;
                foreach (PathNode path in pathIter)
                {
                    if (path.name.Equals(pathName + (pathID == 0 ? string.Empty : " (" + pathID + ")")))
                    {
                        pathID++;
                    }
                }
            } while (!newPathName);
        }
        GameObject go = new GameObject(pathName + (pathID == 0 ? string.Empty : " (" + pathID + ")"));
        GameObjectUtility.SetParentAndAlign(go, parentGO);
        if (Selection.activeTransform != null)
        {
            go.transform.position = Selection.activeTransform.position;
        }
        go.AddComponent<PathNode>();
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

        Selection.activeGameObject = go;
    }
#endif

    public float speedLimitUPS = 2f;
    public float stopAfterFinish = 0.5f;
    protected float duration = 2f;
    public float Duration
    {
        get
        {
            if (behaviourToNext == Behaviour.Straight)
            {
                return duration;
            }
            return 1f;
        }
    }
    protected float timer = 0f;
    public void Process()
    {
        timer += Time.deltaTime;
    }
    public Vector3 GetPosition()
    {
        float t = Mathf.Clamp01(timer / Duration);
        return Vector3.Lerp(transform.position, NextNodePos, t);
    }
    public Quaternion GetRotation()
    {
        float t = Mathf.Clamp01(timer / Duration);
        if (behaviourToNext == Behaviour.Straight)
        {
            return Quaternion.LookRotation(NextNodePos - transform.position, Vector3.up);
        }
        else if (behaviourToNext == Behaviour.End)
        {
            return Quaternion.LookRotation(transform.position - PrevNodePos);
        }
        else
        {
            Quaternion prevDir = Quaternion.LookRotation(transform.position - PrevNodePos, Vector3.up);
            Quaternion nextDir = Quaternion.LookRotation(NextNode.NextNodePos - NextNodePos, Vector3.up);

            return Quaternion.Lerp(prevDir, nextDir, t);
        }
    }
    public bool IsBraking()
    {
        return timer > Duration && !IsDone();
    }
    public bool IsDone()
    {
        if (behaviourToNext == Behaviour.End) return false;
        return timer > Duration + (behaviourToNext == Behaviour.Straight ? stopAfterFinish : 0f); 
    }

    public void OnDrawGizmos()
    {
        if (Vector3.SqrMagnitude(transform.position - NextNodePos) <= Mathf.Epsilon) return; 
        switch (behaviourToNext)
        {
            case Behaviour.Straight:
                Gizmos.color = Color.yellow;
                break;
            case Behaviour.TurnLeft:
                Gizmos.color = Color.blue;
                break;
            case Behaviour.TurnRight:
                Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawLine(transform.position, NextNodePos);
    }
}
