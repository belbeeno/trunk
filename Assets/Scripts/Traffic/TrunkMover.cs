using UnityEngine;

[ExecuteInEditMode]
public class TrunkMover : MonoBehaviour 
{   
    public float speed = 1f;
    public bool canMove;
    
    private Route _route ;

    public void Start()
    {
        canMove = false;
        _route = new Route();
    }

	public void Update () 
    {
        if (_route != null && canMove)
        {
            _route.Update(transform, speed, Time.deltaTime);
        }
    }
}
