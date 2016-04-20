using System.Linq;

public class RemoveDeadEndsStep : GenerationStepBase
{               
    public override void Run()
    {
        bool removedSome = true;
        while (removedSome)
        {
            var toRemove = data.roadGraph
                .GetNodes()
                .Where(n => data.roadGraph.GetOutEdges(n).Count() <= 1);
                
            removedSome = toRemove.Any();
            
            toRemove.ForEach(n => data.roadGraph.RemoveNode(n));
        }
    }
}
