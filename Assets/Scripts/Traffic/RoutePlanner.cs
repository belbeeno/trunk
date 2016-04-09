using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Node = Node<RoadNodeData>;
using Random = UnityEngine.Random;

public class RoutePlanner : MonoBehaviour
{
    public Graph<RoadNodeData, RoadEdgeData> graph;
    
    public Node[] GetRandomPath()
    {
        var nodes = graph.GetNodes();
        while (true)
        {
            var from = nodes[Random.Range(0, nodes.Length)];
            var to = nodes[Random.Range(0, nodes.Length)];
            var path = GetPath(graph, from, to);
            
            if (path.Count() > 2)
            {
                return path.ToArray();
            }
        };
    }
    
    public Node[] GetRandomPathStartingWith(Node first, Node second)
    {
        var graphCopy = new Graph<RoadNodeData, RoadEdgeData>(graph);
        graphCopy.RemoveDirectedEdge(second, first);
        var nodes = graphCopy.GetNodes();
        
        while (true)
        {
            var to = nodes[Random.Range(0, nodes.Length)];
            var path = new List<Node>(GetPath(graphCopy, second, to));
            
            if (path.Count() > 2)
            {
                path.Insert(0, first);
                return path.ToArray();
            }
        };
    }
    
    public Node[] GetPath(Node from, Node to)
    {
        return GetPath(graph, from, to).ToArray();
    }
    
    private IList<Node> GetPath(Graph<RoadNodeData, RoadEdgeData> roadGraph, Node start, Node goal)
    {
        var closedSet = new List<Node>();
        var openSet = new List<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var gScores = new Dictionary<Node, float>();
        var fScores = new Dictionary<Node, float>();
        
        openSet.Add(start);
        gScores[start] = 0f;
        Func<Node, float> getGScore = (n) => gScores.ContainsKey(n) ? gScores[n] : float.MaxValue;
        fScores[start] = HeuristicCostEstimate(start, goal);
        Func<Node, float> getFScore = (n) => fScores.ContainsKey(n) ? fScores[n] : float.MaxValue;
        
        while (openSet.Any())
        {
            var current = openSet.OrderBy(n => getFScore(n)).First();
            if (current == goal)
            {
                return ReconstructGoal(cameFrom, goal);
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
    
    private IList<Node> ReconstructGoal(IDictionary<Node, Node> cameFrom, Node goal)
    {
        var current = goal;
        var path = new List<Node>() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        
        return path.ToArray();
    }
    
    private float HeuristicCostEstimate(Node from, Node to)
    {
        return Vector3.Distance(from.pos, to.pos);
    }
}
