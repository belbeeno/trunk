using System.Linq;
using UnityEngine;

public class AddSchoolsStep : GenerationStepBase
{    
    public override void Run()
    {
        data.schools = data.cityBlocks
            .Where(b => b.isSchool)
            .Select(b => CreateSchool(b, PrefabStore.instance.smallPark))
            .ToArray();
    }
    
    private SchoolData CreateSchool(CityBlockData block, GameObject prefab)
    {        
        var insetAmount = (options.roadWidth / 2f) * options.blockSize;
        var corners = block.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var park = new SchoolData(corners);
        return park;
    }
}
