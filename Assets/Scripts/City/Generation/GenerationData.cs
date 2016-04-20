using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{    
    public RoadGraph roadGraph { get; set; }
    public ICollection<CityBlockData> cityBlocks { get; set; }
    public ICollection<SidewalkData> sidewalks { get; set; }
    public ICollection<RoadData> roadMeshes { get; set; }
    public ICollection<ParkData> smallParks { get; set; }
    public ICollection<ParkData> largeParks { get; set; }
    public ICollection<SchoolData> schools { get; set; }
    public ICollection<BuildingPlotData> buildingPlots { get; set; }
    public RiverGraph riverGraph { get; set; }
    public Mesh clickColliderMesh { get; set; }
    public WaterData water { get; set; }
}
