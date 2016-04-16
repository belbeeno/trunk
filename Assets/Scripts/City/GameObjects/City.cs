using UnityEngine;

public class City : MonoBehaviour 
{
    public void GenerateGeometry(GenerationResult result)
    {    
        while (transform.childCount > 0) 
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
                         
        // Buildings
        var buildingsObj = new GameObject("Buildings");
        buildingsObj.transform.parent = this.transform;
        foreach (var plot in result.buildingPlots)
        {
            var plotObj = new GameObject("Building");
            plotObj.transform.parent = buildingsObj.transform;
            var meshFilter = plotObj.AddComponent<MeshFilter>();
            meshFilter.mesh = plot.mesh;
            var meshRenderer = plotObj.AddComponent<MeshRenderer>();
            meshRenderer.material = plot.material;
        }
        
        // Sidewalks
        var sidewalksObj = new GameObject("Sidewalks");
        sidewalksObj.transform.parent = this.transform;
        foreach (var sidewalk in result.sidewalks)
        {
            var sidewalkObj = new GameObject("Sidewalk");
            sidewalkObj.transform.parent = sidewalksObj.transform;
            var meshFilter = sidewalkObj.AddComponent<MeshFilter>();
            meshFilter.mesh = sidewalk.mesh;
            var meshRenderer = sidewalkObj.AddComponent<MeshRenderer>();
            meshRenderer.material = sidewalk.material;
        }
        
        // Road meshes
        var roadMeshesObj = new GameObject("Road Meshes");
        roadMeshesObj.transform.parent = this.transform;
        foreach (var roadMesh in result.roadMeshes)
        {
            var roadMeshObj = new GameObject("Road Mesh");
            roadMeshObj.transform.parent = roadMeshesObj.transform;
            var meshFilter = roadMeshObj.AddComponent<MeshFilter>();
            meshFilter.mesh = roadMesh.mesh;
            var meshRenderer = roadMeshObj.AddComponent<MeshRenderer>();
            meshRenderer.material = roadMesh.material;
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
        
        // River
        var riverObj = new GameObject("River");
        riverObj.transform.parent = this.transform;
        var riverScript = riverObj.AddComponent<River>();
        riverScript.riverGraph = result.riverGraph;
    }
}
