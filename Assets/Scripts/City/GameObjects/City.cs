using System;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour 
{
    public void GenerateGeometry(GenerationData result)
    {
        ClearChildren();
        AddMeshes(result);
        AddColliders(result);
//      AddDebugObjs(result);
    }
    
    private void ClearChildren()
    {
        while (transform.childCount > 0) 
        {
            GameObject.Destroy(transform.GetChild(0).gameObject);
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
        
        // Parks
        var parksObj = CreateGameObject("Parks");
        foreach (var park in result.smallParks)
        {
            var parkObj = CreateGameObject(parksObj, "Small Park");
            AddMesh(parkObj, park.mesh, park.material);
            var center = park.corners.Average();
            var rotation = Quaternion.Euler(0, 90 * UnityEngine.Random.Range(0, 4), 0);
            var parkPrefab = (GameObject)GameObject.Instantiate(park.prefab, center, rotation);
            parkPrefab.transform.parent = parkObj.transform;
            
            var parkGround = parkPrefab.transform.FindChild("pPlane1");
            var groundRenderer = parkGround.GetComponent<MeshRenderer>();
            var parkSize = parkObj.GetComponent<MeshRenderer>().bounds.extents.x;
            parkPrefab.transform.localScale = Vector3.one * parkSize;
            parkPrefab.transform.position += parkPrefab.transform.forward * parkSize / 3;

            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.schoolAudio);
            audioInstance.transform.SetParent(parkObj.transform, false);
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            audioBounds.points = park.corners;
        }
        foreach (var park in result.largeParks)
        {
            var parkObj = CreateGameObject(parksObj, "Large Park");
            AddMesh(parkObj, park.mesh, park.material);
            var center = park.corners.Average();
            
            var rotation = Quaternion.Euler(0, 90 * UnityEngine.Random.Range(0, 4), 0);
            var parkPrefab = (GameObject)GameObject.Instantiate(park.prefab, center, rotation);
            parkPrefab.transform.parent = parkObj.transform;
            
            var parkGround = parkPrefab.transform.FindChild("pPlane1");
            var groundRenderer = parkGround.GetComponent<MeshRenderer>();
            var parkSize = parkObj.GetComponent<MeshRenderer>().bounds.extents.x;
            parkPrefab.transform.localScale = Vector3.one * parkSize / 4f;
            parkPrefab.transform.position += parkPrefab.transform.forward * parkSize / 3;
            var pos = parkPrefab.transform.position;
            parkPrefab.transform.position = new Vector3(pos.x, 3, pos.z);

            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.schoolAudio);
            audioInstance.transform.SetParent(parkObj.transform, false);
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            audioBounds.points = park.corners;
        }

        // Schools
        var schoolsObj = CreateGameObject("Schools");
        foreach (var school in result.schools)
        {
            var schoolObj = CreateGameObject(parksObj, "School");
            var center = school.corners.Average();
            
            var rotDeg = UnityEngine.Random.Range(0f, 1f) > 0.5 ? 0 : 180;
            var width = school.corners.Max(c => c.x) - school.corners.Min(c => c.x);
            var length = school.corners.Max(c => c.z) - school.corners.Min(c => c.z);
            var rotation = (width < length)
                ? Quaternion.Euler(0, rotDeg, 0)
                : Quaternion.Euler(0, rotDeg + 90, 0);
            
            var schoolPrefab = (GameObject)GameObject.Instantiate(PrefabStore.instance.school, center, rotation);
            schoolPrefab.transform.parent = schoolsObj.transform;
            schoolPrefab.transform.localScale = Vector3.one * Math.Min(width, length) / 3.5f;
            //schoolPrefab.transform.position -= schoolPrefab.transform.right * Math.Min(width, length) / 5;

            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.schoolAudio);
            audioInstance.transform.SetParent(schoolObj.transform, false);
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            audioBounds.points = school.corners;
        }

        // As an optimization, static batch all the unmoving objects with many common materials
        //*
        StaticBatchingUtility.Combine(buildingsObj);
        StaticBatchingUtility.Combine(sidewalksObj);
        StaticBatchingUtility.Combine(roadMeshesObj);
        //*/
        // Water
        var waterObj = CreateGameObject("Water");
        AddMesh(waterObj, result.water.mesh, result.water.material);
    }
    
    private void AddColliders(GenerationData result)
    {   
        // Click collider
        var clickCollider = CreateGameObject("Click Collider");
        var meshCollider = clickCollider.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = result.clickColliderMesh;
        clickCollider.tag = "Networking";
        clickCollider.layer = LayerMask.NameToLayer("HostOnly");
        clickCollider.transform.position = new Vector3(0f, 1f, 0f);
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
