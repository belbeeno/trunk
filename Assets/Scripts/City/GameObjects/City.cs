using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour 
{
    public IEnumerator GenerateGeometry(GenerationData result)
    {
        ClearChildren();
        yield return StartCoroutine(AddMeshes(result));
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
    
    private IEnumerator AddMeshes(GenerationData result)
    {                         
        // Buildings
        var buildingsObj = CreateGameObject("Buildings");
        foreach (var plot in result.buildingPlots)
        {
            var plotObj = CreateGameObject(buildingsObj, "Building");
            AddMesh(plotObj, plot.mesh, plot.material);

            if (result.isHostage)
            {
                HostageCullable culler = plotObj.AddComponent<HostageCullable>();
                culler.CenterPos = plot.corners.Average();
                culler.CreateCollider();
            }
        }
        yield return 0;
        
        // Sidewalks
        var sidewalksObj = CreateGameObject("Sidewalks");
        foreach (var sidewalk in result.sidewalks)
        {
            var sidewalkObj = CreateGameObject(sidewalksObj, "Sidewalk");
            AddMesh(sidewalkObj, sidewalk.mesh, sidewalk.material);

            if (result.isHostage)
            {
                HostageCullable culler = sidewalkObj.AddComponent<HostageCullable>();
                culler.CenterPos = sidewalk.corners.Average();
                BoxCollider collider = culler.CreateCollider();
                Vector3 yPaddedSize = collider.size;
                yPaddedSize.y = 10f;
                collider.size = yPaddedSize;
            }
        }
        yield return 0;
        
        // Road meshes
        var roadMeshesObj = CreateGameObject("Road Meshes");
        foreach (var roadMesh in result.roadMeshes)
        {
            var roadMeshObj = CreateGameObject(roadMeshesObj, "Road Mesh");
            AddMesh(roadMeshObj, roadMesh.mesh, roadMesh.material);

            if (result.isHostage)
            {
                HostageCullable culler = roadMeshObj.AddComponent<HostageCullable>();
                culler.CenterPos = roadMesh.corners.Average();
                culler.CreateCollider();
            }
        }
        yield return 0;
        
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
            
            var parkSize = parkObj.GetComponent<MeshRenderer>().bounds.extents.x;
            parkPrefab.transform.localScale = Vector3.one * parkSize;
            parkPrefab.transform.position += parkPrefab.transform.forward * parkSize / 3;
            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.parkAudio);
            audioInstance.transform.SetParent(parkObj.transform, false);
            audioInstance.transform.position = center;
            CardboardAudio.ActivatePostInitialization(audioInstance); 
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            audioBounds.points = park.corners;

            if (result.isHostage)
            {
                HostageCullable culler = parkObj.AddComponent<HostageCullable>();
                culler.CenterPos = park.corners.Average();
                culler.CreateCollider();
            }
        }
        yield return 0;

        foreach (var park in result.largeParks)
        {
            var parkObj = CreateGameObject(parksObj, "Large Park");
            AddMesh(parkObj, park.mesh, park.material);
            var center = park.corners.Average();
            
            var rotation = Quaternion.Euler(0, 90 * UnityEngine.Random.Range(0, 4), 0);
            var parkPrefab = (GameObject)GameObject.Instantiate(park.prefab, center, rotation);
            parkPrefab.transform.parent = parkObj.transform;
            
            var parkSize = parkObj.GetComponent<MeshRenderer>().bounds.extents.x;
            parkPrefab.transform.localScale = Vector3.one * parkSize / 4f;
            parkPrefab.transform.position += parkPrefab.transform.forward * parkSize / 3;
            var pos = parkPrefab.transform.position;
            parkPrefab.transform.position = new Vector3(pos.x, 3, pos.z);

            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.bigParkAudio);
            audioInstance.transform.SetParent(parkObj.transform, false);
            audioInstance.transform.position = center;
            CardboardAudio.ActivatePostInitialization(audioInstance); 
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            if (audioBounds == null) Debug.LogError("audioBounds is null!");
            if (audioBounds.points == null) Debug.LogError("points is null!");
            if (park == null) Debug.LogError("park is null!");
            if (park.corners == null) Debug.LogError("corners is null!");
            audioBounds.points = park.corners;

            if (result.isHostage)
            {
                HostageCullable culler = parkObj.AddComponent<HostageCullable>();
                culler.CenterPos = park.corners.Average();
                culler.CreateCollider();
            }
        }
        yield return 0;

        // Schools
        var schoolsObj = CreateGameObject("Schools");
        foreach (var school in result.schools)
        {
            var schoolObj = CreateGameObject(schoolsObj, "School");
            var center = school.corners.Average();
            
            var rotDeg = UnityEngine.Random.Range(0f, 1f) > 0.5 ? 0 : 180;
            var width = school.corners.Max(c => c.x) - school.corners.Min(c => c.x);
            var length = school.corners.Max(c => c.z) - school.corners.Min(c => c.z);
            var rotation = (width < length)
                ? Quaternion.Euler(0, rotDeg, 0)
                : Quaternion.Euler(0, rotDeg + 90, 0);
            
            var schoolPrefab = (GameObject)GameObject.Instantiate(PrefabStore.instance.school, center, rotation);
            schoolPrefab.transform.parent = schoolObj.transform;
            schoolPrefab.transform.localScale = Vector3.one * Math.Min(width, length) / 3.5f;
            //schoolPrefab.transform.position -= schoolPrefab.transform.right * Math.Min(width, length) / 5;

            GameObject audioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.schoolAudio);
            audioInstance.transform.SetParent(schoolPrefab.transform, false);
            audioInstance.transform.position = center;
            CardboardAudio.ActivatePostInitialization(audioInstance); 
            ManualVolumetricAudio audioBounds = audioInstance.GetComponent<ManualVolumetricAudio>();
            audioBounds.points = school.corners;

            if (result.isHostage)
            {
                HostageCullable culler = schoolObj.AddComponent<HostageCullable>();
                culler.CenterPos = center;
                culler.CreateCollider();
            }
        }
        yield return 0;

        // As an optimization, static batch all the unmoving objects with many common materials
        //*
        StaticBatchingUtility.Combine(buildingsObj);
        StaticBatchingUtility.Combine(sidewalksObj);
        StaticBatchingUtility.Combine(roadMeshesObj);
        yield return 0;
        //*/

        // Water
        var waterObj = CreateGameObject("Water");
        AddMesh(waterObj, result.water.mesh, result.water.material);
        
        GameObject waterAudioInstance = GameObject.Instantiate<GameObject>(PrefabStore.instance.waterAudio);
        waterAudioInstance.transform.SetParent(waterObj.transform, false);
        waterAudioInstance.transform.position = result.water.corners[0];
        RiverVolumetricAudio waterAudioBounds = waterAudioInstance.GetComponent<RiverVolumetricAudio>();
        waterAudioBounds.BuildFromRiverGraph(result.riverGraph);
        CardboardAudio.ActivatePostInitialization(waterAudioInstance);
    }
    
    private void AddColliders(GenerationData result)
    {   
        // Click collider
        var clickCollider = CreateGameObject("Click Collider");
        var meshCollider = clickCollider.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = result.clickColliderMesh;
        clickCollider.tag = "Networking";
        clickCollider.layer = LayerMask.NameToLayer("HostOnly");
        clickCollider.transform.position = new Vector3(0f, 0f, 0f);
        clickCollider.AddComponent<StopItemsFromMoving>(); 
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
