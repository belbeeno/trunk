using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Graph = Graph<RoadNodeData, RoadRoadEdgeData>;
using Random = UnityEngine.Random;

public class RoutePlanner : MonoBehaviour
{
    public Graph graph;
    
    public RoadEdge[] GetRandomPath()
    {
        var RoadNodes = graph.GetRoadNodes();
        while (true)
        {
            var from = RoadNodes[Random.Range(0, RoadNodes.Length)];
            var to = RoadNodes[Random.Range(0, RoadNodes.Length)];
            var path = GetPath(graph, from, to);
            
            if (path.Count() > 2)
            {
                return path.ToArray();
            }
        };
    }
    
    public RoadEdge[] GetRandomPath(RoadEdge firstRoadEdge)
    {
        var graphCopy = new Graph<RoadRoadNodeData, RoadRoadEdgeData>(graph);
        graphCopy.RemoveDirectedRoadEdge(firstRoadEdge.to, firstRoadEdge.from);
        var RoadNodes = graphCopy.GetRoadNodes();
        
        while (true)
        {
            var to = RoadNodes[Random.Range(0, RoadNodes.Length)];
            var path = GetPath(graphCopy, firstRoadEdge.to, to);
            
            if (path.Count() > 2)
            {
                path.Insert(0, firstRoadEdge);
                return path.ToArray();
            }
        };
    }
    
    public RoadEdge[] GetPath(RoadNode from, RoadNode to)
    {
        return GetPath(graph, from, to).ToArray();
    }
    
    private IList<RoadEdge> GetPath(Graph roadGraph, RoadNode start, RoadNode goal)
    {
        var closedSet = new List<RoadNode>();
        var openSet = new List<RoadNode>();
        var cameFrom = new Dictionary<RoadNode, RoadNode>();
        var gScores = new Dictionary<RoadNode, float>();
        var fScores = new Dictionary<RoadNode, float>();
        
        openSet.Add(start);
        gScores[start] = 0f;
        Func<RoadNode, float> getGScore = (n) => gScores.ContainsKey(n) ? gScores[n] : float.MaxValue;
        fScores[start] = HeuristicCostEstimate(start, goal);
        Func<RoadNode, float> getFScore = (n) => fScores.ContainsKey(n) ? fScores[n] : float.MaxValue;
        
        while (openSet.Any())
        {
            var current = openSet.OrderBy(n => getFScore(n)).First();
            if (current == goal)
            {
                return ReconstructGoal(roadGraph, cameFrom, goal);
            }
            
            openSet.Remove(current);
            closedSet.Add(current);
            
            var neighbours = roadGraph.GetAdjacentRoadNodes(current)
                .Where(n => !closedSet.Contains(n));
            
            foreach (var neighbour in neighbours)
            {
                var gScore = getGScore(current) + Vector3.Distance(current.pos, neighbour.pos);
                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
                else if (gScore >= getGScore(neighbour))
                {
                    continue;
                }
                
                cameFrom[neighbour] = current;
                gScores[neighbour] = gScore;
                fScores[neighbour] = gScore = HeuristicCostEstimate(neighbour, goal);
            }
        }
                
        return null;
    }
    
    private IList<RoadEdge> ReconstructGoal(Graph roadGraph, IDictionary<RoadNode, RoadNode> cameFrom, RoadNode goal)
    {
        var current = goal;
        var path = new List<RoadEdge>();
        while (cameFrom.ContainsKey(current))
        {
            var previous = cameFrom[current];
            var RoadEdge = roadGraph.GetEdge(previous, current);
            path.Insert(0, RoadEdge);
            current = previous;
        }
        
        return path;
    }
    
    private float HeuristicCostEstimate(RoadNode from, RoadNode to)
    {
        return Vector3.Distance(from.pos, to.pos);
    }
}
