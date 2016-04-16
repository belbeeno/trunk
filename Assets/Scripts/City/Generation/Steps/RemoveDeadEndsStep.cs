using System.Linq;

public class RemoveDeadEndsStep : GenerationStepBase
{               
    public override void Run()
    {
        data.roadGraph
            .GetNodes()
            .Where(n => data.roadGraph.GetOutEdges(n).Count() <= 1)
            .ForEach(n => data.roadGraph.RemoveNode(n));
    }
}
