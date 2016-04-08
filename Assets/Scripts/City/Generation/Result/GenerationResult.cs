using System.Collections.Generic;

public class GenerationResult
{
    public Graph<RoadNodeData, RoadEdgeData> roadGraph { get; set; }
    public IEnumerable<RoadData> roads { get; set; }
    public IEnumerable<CityBlockData> cityBlocks { get; set; }
    public RiverData river { get; set; }
}
