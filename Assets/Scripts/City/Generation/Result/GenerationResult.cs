using System.Collections.Generic;

public class GenerationResult
{
    public RoadGraph roadGraph { get; set; }
    public IEnumerable<CityBlockData> cityBlocks { get; set; }
    public RiverGraph riverGraph { get; set; }
}
