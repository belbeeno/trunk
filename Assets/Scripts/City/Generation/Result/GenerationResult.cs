public class GenerationResult
{
    public RoadGraph roadGraph { get; set; }
    
    public CityBlockData[] cityBlocks { get; set; }
    public BuildingPlotData[] buildingPlots { get; set; }
    public RiverGraph riverGraph { get; set; }
    public SidewalkData[] sidewalks { get; set; }
    public RoadData[] roadMeshes { get; set; }
}
