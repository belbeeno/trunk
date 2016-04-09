using UnityEngine;
using System.Linq;

public class Route
{
    private RoutePlanner _routePlanner;

    private RoadEdge _currentEdge;
    private RoadEdge[] _upcomingEdges;
    
    private float _t = 0f;
    private float _cornerT = 0.1f;
    
    public Route()
    {
        var gameObj = GameObject.Find("RoutePlanner");
        _routePlanner = gameObj.GetComponent<RoutePlanner>();
    }
    
    public void Update(Transform transform, float speed, float deltaTime)
    {
        CheckPath();
                
        if (_t > (1f - _cornerT) && _upcomingEdges.Any())
        {
             _t += deltaTime / (_currentEdge.length / (speed / 2f));
            LerpCorner(transform, _currentEdge, _upcomingEdges.First(), _t);
        }
        else
        {
             _t += deltaTime / (_currentEdge.length / (speed));
            transform.position = Vector3.Lerp(_currentEdge.from.pos, _currentEdge.to.pos, _t);
            transform.rotation = Quaternion.LookRotation(_currentEdge.direction, Vector3.up);
        }
                
        if (_t > 1f)
        {
            _t = _cornerT;
            _currentEdge = _upcomingEdges.First();
            _upcomingEdges = _upcomingEdges.Skip(1).ToArray();
        }
    }
    
    public void LerpCorner(Transform transform, RoadEdge current, RoadEdge next, float t)
    {
        var nextEdgeLerpT = (t + _cornerT) - 1f;
        var transitionLerpT = nextEdgeLerpT / _cornerT;

        var currentPos = Vector3.Lerp(current.from.pos, current.to.pos, t);
        var nextPos = Vector3.Lerp(next.from.pos, next.to.pos, nextEdgeLerpT);
        transform.position = Vector3.Lerp(currentPos, nextPos, transitionLerpT);
        
        var currentDirRot = Quaternion.LookRotation(current.direction, Vector3.up);
        var nextDirRot = Quaternion.LookRotation(next.direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(currentDirRot, nextDirRot, transitionLerpT);
    }
    
    private void CheckPath()
    {
        if (_currentEdge == null)
        {
            var newPath = _routePlanner.GetRandomPath();
            _currentEdge = newPath.First();
            _upcomingEdges = newPath.Skip(1).ToArray();
        }
        else if (_upcomingEdges == null || !_upcomingEdges.Any())
        {
            var newPath = _routePlanner.GetRandomPath(_currentEdge);
            _upcomingEdges = newPath.Skip(1).ToArray();
        }
    }
}
