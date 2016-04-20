using System.Linq;

public class CityBlockData
{
    public CityBlockData(RoadEdge[] boundingRoads)
    {
        this.boundingRoads = boundingRoads;
    }
    
    public readonly RoadEdge[] boundingRoads;
    public bool isSmallPark = false;
    public bool isLargePark = false;
    public bool isSchool = false;
    
    public bool ContainsRiver()
    {
        return boundingRoads.Any(r => r.data.isBridge);
    }
    
    public bool IsCityFeature()
    {
        return isSmallPark || isLargePark || isSchool;
    }
}