using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanOutCityStep : GenerationStepBase
{           
    private IList<Vector3> _smallParkLocs;
    private IList<Vector3> _largeParkLocs;
    private IList<Vector3> _schoolLocs;
      
    public override void Run()
    {
        _smallParkLocs = new List<Vector3>();
        _largeParkLocs = new List<Vector3>();
        _schoolLocs = new List<Vector3>();
        
        TryCreate(() => _largeParkLocs, options.numLargeParks, AddLargePark);
        TryCreate(() => _schoolLocs, options.numSchools, AddSchool);
        TryCreate(() => _smallParkLocs, options.numSmallParks, AddSmallPark);
    }
    
    private void TryCreate(Func<IList<Vector3>> locs, int numToCreate, Action createFunc)
    {
        var tries = 0;
        while (locs().Count() < numToCreate && tries < 5 * numToCreate)
        {
            createFunc();
            tries++;
        }
    }
    
    private void AddSmallPark()
    {
        var block = data.cityBlocks
            .Where(b => !b.ContainsRiver())
            .Where(b => !b.IsCityFeature())
            .RandomMember();
        var center = block.boundingRoads.Select(b => b.from.pos).Average();
        var isOkay = _smallParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.smallParkDist)
            && _largeParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist)
            && _schoolLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist);
        
        if (isOkay)
        {
            _smallParkLocs.Add(center);
            block.isSmallPark = true;
        }
    }
    
    private void AddLargePark()
    {
        var intersection = data.roadGraph.GetNodes().RandomMember();
        var outEdges = data.roadGraph.GetOutEdges(intersection);   
        var blocksToMerge = data.cityBlocks.Where(b => b.boundingRoads.Any(r => outEdges.Contains(r))).ToArray();      
        var invalid = blocksToMerge.Any(b => b.ContainsRiver() || b.IsCityFeature()) || blocksToMerge.Count() != 4;
        if (invalid)
        {
            return;
        }
        
        var reverseEdges = outEdges.Select(e => data.roadGraph.GetEdge(e.to, e.from));
        var edgesToRemove = outEdges.Union(reverseEdges).ToArray();
        var allEdges = blocksToMerge.SelectMany(b => b.boundingRoads);
        var simplifiedEdges = SimplifyEdges(allEdges.Except(edgesToRemove));
        var newBlock = new CityBlockData(simplifiedEdges);

        var center = newBlock.boundingRoads.Select(r => r.from.pos).Average();
        var isOkay = _largeParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.largeParkDist)
            && _smallParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist)
            && _schoolLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist);
        if (isOkay)
        {
            newBlock.isLargePark = true;
            blocksToMerge.ForEach(b => data.cityBlocks.Remove(b));
            data.cityBlocks.Add(newBlock);
            edgesToRemove.ForEach(e => data.roadGraph.RemoveDirectedEdge(e));
            _largeParkLocs.Add(simplifiedEdges.Select(e => e.from.pos).Average());
        }
    }
    
    private void AddSchool()
    {
        var block = data.cityBlocks.Where(b => !b.ContainsRiver() && !b.IsCityFeature()).RandomMember();        
        foreach (var edge in block.boundingRoads.Shuffle())
        {
            var reverse = data.roadGraph.GetEdge(edge.to, edge.from);
            var neighbour = data.cityBlocks
                .Where(b => b.boundingRoads.Contains(reverse))
                .Where(b => !b.ContainsRiver())
                .Where(b => !b.IsCityFeature())
                .SingleOrDefault();
            
            if (neighbour == null)
            {
                continue;
            }
            
            var allEdges = block.boundingRoads.Union(neighbour.boundingRoads).ToList();
            allEdges.Remove(edge);
            allEdges.Remove(reverse);
            
            var simplifiedEdges = SimplifyEdges(allEdges);
            var newBlock = new CityBlockData(simplifiedEdges.ToArray());
            
            var center = newBlock.boundingRoads.Select(r => r.from.pos).Average();
            var isOkay = _schoolLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.schoolDist)
                && _smallParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist)
                && _largeParkLocs.All(l => Vector3.Distance(center, l) > options.blockSize * options.differentFeatureDist);
            
            if (isOkay)
            {
                newBlock.isSchool = true;
                data.cityBlocks.Remove(block);
                data.cityBlocks.Remove(neighbour);
                data.cityBlocks.Add(newBlock);
                data.roadGraph.RemoveDirectedEdge(edge);
                data.roadGraph.RemoveDirectedEdge(reverse);
                _schoolLocs.Add(simplifiedEdges.Select(e => e.from.pos).Average());
                break;
            }
        }
    }
    
    private RoadEdge[] SimplifyEdges(IEnumerable<RoadEdge> input)
    {
        var inputEdges = new List<RoadEdge>(input);
        var simplifiedEdges = new List<RoadEdge> { inputEdges.First() };
        inputEdges.RemoveAt(0);
        while (inputEdges.Any())
        {                
            var toAdd = inputEdges.SingleOrDefault(e => e.from == simplifiedEdges.Last().to);
            if (toAdd == null)
            {
                Debug.Log(string.Join("\n", input.Select(r => string.Format("({0},{1})", r.from.pos, r.to.pos)).ToArray()));
                Debug.Log(string.Join("\n", simplifiedEdges.Select(r => string.Format("({0},{1})", r.from.pos, r.to.pos)).ToArray()));
                throw new ArgumentException();
            }
            simplifiedEdges.Add(toAdd);
            inputEdges.Remove(toAdd);
        }

        var prev = simplifiedEdges.Last();
        foreach (var cur in new List<RoadEdge>(simplifiedEdges))
        {
            var ang = Vector3.Angle(prev.direction, cur.direction);
            if (ang < 5 || ang > 175)
            {
                var index = simplifiedEdges.IndexOf(prev);
                simplifiedEdges.Insert(index, new RoadEdge(prev.from, cur.to, prev.data));
                simplifiedEdges.Remove(prev);
                simplifiedEdges.Remove(cur);
            }
            prev = cur;
        }
        
        return simplifiedEdges.ToArray();
    }
}
