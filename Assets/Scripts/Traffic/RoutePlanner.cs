using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class RoutePlanner : MonoBehaviour
{
    public RoadGraph graph;
    
    public RoadEdge[] GetRandomPath()
    {
        var RoadNodes = graph.GetNodes();
        while (true)
        {
            var from = RoadNodes[Random.Range(0, RoadNodes.Length)];
            var to = RoadNodes[Random.Range(0, RoadNodes.Length)];
            IList<RoadEdge> path = null;
            do
            {
                path = GetPath(graph, from, to);
            } while (path == null);
            
            // Getting a null ref here...? That's why the while loop is above.
            if (path.Count() > 2)
            {
                return path.ToArray();
            }
        };
    }
    
    public RoadEdge[] GetRandomPath(RoadEdge firstEdge)
    {
        var graphCopy = new RoadGraph(graph);
        graphCopy.RemoveDirectedEdge(firstEdge.to, firstEdge.from);
        var RoadNodes = graphCopy.GetNodes();
        
        while (true)
        {
            var to = RoadNodes[Random.Range(0, RoadNodes.Length)];
            var path = GetPath(graphCopy, firstEdge.to, to);
            
            if (path.Count() > 2)
            {
                path.Insert(0, firstEdge);
                return path.ToArray();
            }
        };
    }
    
    public RoadEdge[] GetPath(RoadNode from, RoadNode to)
    {
        return GetPath(graph, from, to).ToArray();
    }
    
    private IList<RoadEdge> GetPath(RoadGraph roadGraph, RoadNode start, RoadNode goal)
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
            
            var neighbours = roadGraph.GetAdjacentNodes(current)
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
    
    private IList<RoadEdge> ReconstructGoal(RoadGraph roadGraph, IDictionary<RoadNode, RoadNode> cameFrom, RoadNode goal)
    {
        var current = goal;
        var path = new List<RoadEdge>();
        while (cameFrom.ContainsKey(current))
        {
            var previous = cameFrom[current];
            var edge = roadGraph.GetEdge(previous, current);
            path.Insert(0, edge);
            current = previous;
        }
        
        return path;
    }
    
    private float HeuristicCostEstimate(RoadNode from, RoadNode to)
    {
        return Vector3.Distance(from.pos, to.pos);
    }
}
