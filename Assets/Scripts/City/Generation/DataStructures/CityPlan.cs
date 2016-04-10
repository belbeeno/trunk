using System.Collections.Generic;
using System.Linq;

public class CityPlan
{
    private IList<CityBlockData> _cityBlocks;
    
    public CityPlan()
    {
        _cityBlocks = new List<CityBlockData>();
    }
    
    public void AddCityBlock(CityBlockData cityBlock)
    {
        _cityBlocks.Add(cityBlock);
    }
    
    public CityBlockData[] GetCityBlocks()
    {
        return _cityBlocks.ToArray();
    }
}
