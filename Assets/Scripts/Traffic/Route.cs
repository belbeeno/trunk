using UnityEngine;
using System.Linq;

public class Route
{
    private RoutePlanner _routePlanner;

    private Node<RoadNodeData>[] _path;
    private float _t = 0f;
    
    public Route()
    {
        var gameObj = GameObject.Find("RoutePlanner");
        _routePlanner = gameObj.GetComponent<RoutePlanner>();
    }
    
    public void Update(Transform transform, float speed, float deltaTime)
    {
        CheckPath();
        
        var segmentDir = (_path[1].pos - _path[0].pos);
        var dist = (_path[1].pos - _path[0].pos).magnitude;
        _t += deltaTime / (dist / speed);
        
        transform.rotation = Quaternion.LookRotation(segmentDir, Vector3.up);
        transform.position = Vector3.Lerp(_path[0].pos, _path[1].pos, _t);
        
        if (_t > 1f)
        {
            _t = 0f;
            _path = _path.Skip(1).ToArray();
        }
    }
    
    private void CheckPath()
    {
        if (_path == null || _path.Count() < 2)
        {
            _path = _routePlanner.GetRandomPath();
            Debug.Log("New random path");
        }
        if (_path.Count() == 2)
        {
            _path = _routePlanner.GetRandomPathStartingWith(_path[0], _path[1]);
            Debug.Log("New extension path");
        }
    }
}
