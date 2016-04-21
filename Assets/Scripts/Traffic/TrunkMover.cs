using UnityEngine;

[ExecuteInEditMode]
public class TrunkMover : MonoBehaviour 
{   
    public float speed = 1f;
    public float maxTurnAngle = 25f;
    
    private Route _route;
    public bool isMoving = false;
    public Banking banking = null;

    public void Start()
    {
        _route = new Route();
        if (banking == null)
        {
            banking = GetComponentInChildren<Banking>();
        }
    }

    bool wasTurning = false;

	public void Update () 
    {
        if (_route != null && _route.IsAtIntersection() != wasTurning)
        {
            if (_route.IsAtIntersection())
            {
                banking.startBanking(_route.GetTurningDir(), _route.GetTurnDuration(speed), maxTurnAngle);
            }
            wasTurning = _route.IsAtIntersection();
        }

        if (isMoving 
            && _route != null 
            && _route.CanMove())
        {
            _route.Update(transform, speed, Time.deltaTime);
        }
    }
}
