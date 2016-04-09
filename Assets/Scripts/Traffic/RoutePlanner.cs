using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Graph = Graph<RoadNodeData, RoadEdgeData>;
using Node = Node<RoadNodeData>;
using Edge = Edge<RoadNodeData, RoadEdgeData>;
using Random = UnityEngine.Random;

public class RoutePlanner : MonoBehaviour
{
    public Graph graph;
    
    public Edge[] GetRandomPath()
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
    
    public Edge[] GetRandomPath(Edge firstEdge)
    {
        var graphCopy = new Graph<RoadNodeData, RoadEdgeData>(graph);
        graphCopy.RemoveDirectedEdge(firstEdge.to, firstEdge.from);
        var nodes = graphCopy.GetNodes();
        
        while (true)
        {
            var to = nodes[Random.Range(0, nodes.Length)];
            var path = GetPath(graphCopy, firstEdge.to, to);
            
            if (path.Count() > 2)
            {
                path.Insert(0, firstEdge);
                return path.ToArray();
            }
        };
    }
    
    public Edge[] GetPath(Node from, Node to)
    {
        return GetPath(graph, from, to).ToArray();
    }
    
    private IList<Edge> GetPath(Graph roadGraph, Node start, Node goal)
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
    
    private IList<Edge> ReconstructGoal(Graph roadGraph, IDictionary<Node, Node> cameFrom, Node goal)
    {
        var current = goal;
        var path = new List<Edge>();
        while (cameFrom.ContainsKey(current))
        {
            var previous = cameFrom[current];
            var edge = roadGraph.GetEdge(previous, current);
            path.Insert(0, edge);
            current = previous;
        }
        
        return path;
    }
    
    private float HeuristicCostEstimate(Node from, Node to)
    {
        return Vector3.Distance(from.pos, to.pos);
    }
}
