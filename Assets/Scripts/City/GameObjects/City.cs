using UnityEngine;

public class City : MonoBehaviour 
{
    public void GenerateGeometry(GenerationData result)
    {
        ClearChildren();
        AddMeshes(result);
        AddColliders(result);
        AddDebugObjs(result);
    }
    
    private void ClearChildren()
    {
        while (transform.childCount > 0) 
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    
    private void AddMeshes(GenerationData result)
    {                         
        // Buildings
        var buildingsObj = CreateGameObject("Buildings");
        foreach (var plot in result.buildingPlots)
        {
            var plotObj = CreateGameObject(buildingsObj, "Building");
            AddMesh(plotObj, plot.mesh, plot.material);
        }
        
        // Sidewalks
        var sidewalksObj = CreateGameObject("Sidewalks");
        foreach (var sidewalk in result.sidewalks)
        {
            var sidewalkObj = CreateGameObject(sidewalksObj, "Sidewalk");
            AddMesh(sidewalkObj, sidewalk.mesh, sidewalk.material);
        }
        
        // Road meshes
        var roadMeshesObj = CreateGameObject("Road Meshes");
        foreach (var roadMesh in result.roadMeshes)
        {
            var roadMeshObj = CreateGameObject(roadMeshesObj, "Road Mesh");
            AddMesh(roadMeshObj, roadMesh.mesh, roadMesh.material);
        }
    }
    
    private void AddColliders(GenerationData result)
    {   
        // Click collider
        var clickCollider = CreateGameObject("Click Collider");
        var meshCollider = clickCollider.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = result.clickColliderMesh;
    }
    
    private void AddDebugObjs(GenerationData result)
    {
        // Roads
        var roadsObj = CreateGameObject("Roads");
        foreach (var edge in result.roadGraph.GetEdges())
        {
            var roadObj = CreateGameObject(roadsObj, "Road");
            var roadScript = roadObj.AddComponent<Road>();
            roadScript.road = edge;
        }
        
        // River
        var riverObj = CreateGameObject("River");
        var riverScript = riverObj.AddComponent<River>();
        riverScript.riverGraph = result.riverGraph;
    }
    
    private GameObject CreateGameObject(string name)
    {
        return CreateGameObject(this.gameObject, name);
    }
    
    private GameObject CreateGameObject(GameObject parent, string name)
    {
        var newObj = new GameObject(name);
        newObj.transform.parent = parent.transform;
        
        return newObj;
    }
    
    private void AddMesh(GameObject gameObj, Mesh mesh, Material material)
    {
        var filter = gameObj.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        
        var renderer = gameObj.AddComponent<MeshRenderer>();
        renderer.material = material;
    }
}
