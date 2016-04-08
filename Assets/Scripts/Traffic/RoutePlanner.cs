using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Node = Node<RoadNodeData>;

public class RoutePlanner : MonoBehaviour
{
    public Graph<RoadNodeData, RoadEdgeData> graph;
        
    public Node[] GetRandomPath()
    {
        var nodes = graph.GetNodes();
        var from = nodes[Random.Range(0, nodes.Length)];
        var to = nodes[Random.Range(0, nodes.Length)];
        var path = GetPath(from, to);
        
        return path;
    }
    
    public Node[] GetPath(Node start, Node goal)
    {
        var closedSet = new List<Node>();
        var openSet = new List<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var gScores = new Dictionary<Node, float>();
        var fScores = new Dictionary<Node, float>();
        
        openSet.Add(start);
        gScores[start] = 0f;
        fScores[start] = HeuristicCostEstimate(start, goal);
        
        while (openSet.Any())
        {
            var current = openSet.OrderBy(n => fScores[n]).First();
            if (current == goal)
            {
                ReconstructGoal(cameFrom, goal);
            }
            
            openSet.Remove(current);
            closedSet.Add(current);
            
            var neighbours = graph.GetAdjacentNodes(current)
                .Where(n => !closedSet.Contains(n));
            
            foreach (var neighbour in neighbours)
            {
                var gScore = gScores[current] + Vector3.Distance(current.pos, neighbour.pos);
                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
                else if (gScore < gScores[neighbour])
                {
                    cameFrom[neighbour] = current;
                    gScores[neighbour] = gScore;
                    fScores[neighbour] = gScore = HeuristicCostEstimate(neighbour, goal);
                }
            }
        }
        
        return null;
    }
    
    private Node[] ReconstructGoal(IDictionary<Node, Node> cameFrom, Node goal)
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
