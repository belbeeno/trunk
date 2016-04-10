using System.Collections.Generic;
using UnityEngine;

public class GenerationResult
{
    public RoadGraph roadGraph { get; set; }
    public IEnumerable<CityBlockData> cityBlocks { get; set; }
    public IList<Vector3> river { get; set; }
}
