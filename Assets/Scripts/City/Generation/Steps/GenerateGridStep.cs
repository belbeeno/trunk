using UnityEngine;

public class GenerateGridStep : GenerationStepBase
{    
    public override void Run()
    {
        data.roadGraph = new RoadGraph();
        
        for (var x = 0; x <= options.blocksWidth; x++)
        for (var y = 0; y <= options.blocksHeight; y++)
        {
            AddIntersection(x, y);
        }
    }
    
    private void AddIntersection(int x, int y)
    {
        var pos = ToVector3(x, y);
        data.roadGraph.AddNode(pos);
        data.roadGraph.AddUndirectedEdge(pos, ToVector3(x, y - 1), isBridge: false);
        data.roadGraph.AddUndirectedEdge(pos, ToVector3(x - 1, y), isBridge: false);
    }
    
    private Vector3 ToVector3(int x, int y)
    {
        return new Vector3(x, 0f, y) * options.blockSize;
    }
}