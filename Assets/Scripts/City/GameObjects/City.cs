using UnityEngine;

public class City : MonoBehaviour 
{
    public void GenerateGeometry(GenerationResult result)
    {    
        while (transform.childCount > 0) 
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
                 
        // Roads
        var roadsObj = new GameObject("Roads");
        roadsObj.transform.parent = this.transform;
        foreach (var edge in result.roadGraph.GetEdges())
        {
            var roadObj = new GameObject("Road");
            roadObj.transform.parent = roadsObj.transform;
            var roadScript = roadObj.AddComponent<Road>();
            roadScript.road = edge;
        }
        
        // Buildings
        var buildingsObj = new GameObject("Buildings");
        buildingsObj.transform.parent = this.transform;
        foreach (var plotData in result.buildingPlots)
        {
            var plotObj = new GameObject("Building");
            plotObj.transform.parent = buildingsObj.transform;
            plotObj.AddComponent<MeshFilter>();
            plotObj.AddComponent<MeshRenderer>();
            var cityBlockScript = plotObj.AddComponent<CityBlock>();
            cityBlockScript.data = plotData;
        }
        
        // River
        var riverObj = new GameObject("River");
        riverObj.transform.parent = this.transform;
        var riverScript = riverObj.AddComponent<River>();
        riverScript.riverGraph = result.riverGraph;
    }
}
