using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrunkMover : MonoBehaviour 
{
    public Transform pathParent = null;
    protected List<PathNode> pathListing = new List<PathNode>();
    protected List<PathNode>.Enumerator pathIter;

    public Banking carBanking = null;
    public Bouncer carBouncing = null;
	
    void Start()
    {
        if (pathParent == null)
        {
            Debug.LogError("Path Parent not defined for trunk mover!  We're going nowhere!", gameObject);
            DebugConsole.SetText("ERROR", "Path Parent not defined for trunk mover!  We're going nowhere!");
            gameObject.SetActive(false);
            return;
        }

        pathListing.AddRange(pathParent.GetComponentsInChildren<PathNode>());
        pathIter = pathListing.GetEnumerator();
        pathIter.MoveNext();
    }

    public float bankMaxAngle = 100f;
    public float bounceDuration = 0.5f;

	// Update is called once per frame
	void Update () 
    {
        if (pathIter.Current == null)
        {
            return;
        }

        pathIter.Current.Process();
        if (!pathIter.Current.IsDone())
        {
            transform.position = pathIter.Current.GetPosition();
            transform.rotation = pathIter.Current.GetRotation();
        }
        else
        {
            pathIter.MoveNext();
            if (carBanking != null)
            {
                var behaviour = pathIter.Current.behaviourToNext;
                if (behaviour == PathNode.Behaviour.TurnLeft)
                {
                    carBanking.startBanking(Vector3.left, pathIter.Current.Duration, bankMaxAngle);
                }
                else if (behaviour == PathNode.Behaviour.TurnRight)
                {
                    carBanking.startBanking(Vector3.right, pathIter.Current.Duration, bankMaxAngle);
                }
                else if (behaviour == PathNode.Behaviour.Straight)
                {
                }
            }

            if (carBouncing != null)
            {
                // I'm lazyyyy
                if (Input.GetKeyUp(KeyCode.B))
                {
                    carBouncing.Bounce(bounceDuration);
                }

            }
        }
	}
}
