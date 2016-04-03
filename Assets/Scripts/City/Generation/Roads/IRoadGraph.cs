using System.Collections.Generic;

public interface IRoadGraph
{
    IEnumerable<RoadData> roads { get; }
}