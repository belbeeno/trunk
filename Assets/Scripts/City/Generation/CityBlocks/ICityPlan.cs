using System.Collections.Generic;

public interface ICityPlan
{
    IEnumerable<CityBlockData> plots { get; }
}