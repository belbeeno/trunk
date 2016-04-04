using System.Collections.Generic;

public class GenerationResult
{
    public IEnumerable<RoadData> roads { get; set; }
    public IEnumerable<CityBlockData> cityBlocks { get; set; }
    public RiverData river { get; set; }
}
