using System.Linq;
using UnityEngine;

public class CityBlockData
{
    public CityBlockData(RoadEdge[] boundingRoads)
    {
        this.boundingRoads = boundingRoads;
    }
    
    public readonly RoadEdge[] boundingRoads;
    
    public bool ContainsRiver()
    {
        return boundingRoads.Any(r => r.data.isBridge);
    }
}