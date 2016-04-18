using System.Linq;

public class CityBlockData
{
    public CityBlockData(RoadEdge[] boundingRoads)
    {
        this.boundingRoads = boundingRoads;
    }
    
    public readonly RoadEdge[] boundingRoads;
    public bool isPark = false;
    
    public bool ContainsRiver()
    {
        return boundingRoads.Any(r => r.data.isBridge);
    }
}