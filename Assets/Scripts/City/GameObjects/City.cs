using UnityEngine;

public class City : MonoBehaviour 
{
    public GenerationOptions generationOptions;
    
    private CityGenerator _generator = new CityGenerator();
    
	void Start () 
    {    
    }
	
	void Update () 
    {
	}
    
    public void Generate()
    {
        while (transform.childCount > 0) 
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        var result = _generator.Generate(generationOptions);
        InitializeRoutePlanner(result);
        CreateGameObjects(result);
        RepositionCamera();
        StartCar();
    }
    
    private void InitializeRoutePlanner(GenerationResult result)
    {
        var gameObj = GameObject.Find("RoutePlanner");
        var routePlanner = gameObj.GetComponent<RoutePlanner>();
        routePlanner.graph = result.roadGraph;
    }
    
    private void CreateGameObjects(GenerationResult result)
    {     
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
        
        // City blocks
        var cityBlocksObj = new GameObject("City Blocks");
        cityBlocksObj.transform.parent = this.transform;
        foreach (var plotData in result.cityBlocks)
        {
            var plotObj = new GameObject("City Block");
            plotObj.transform.parent = cityBlocksObj.transform;
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
    
    private void RepositionCamera()
    {
        var cameraX =  generationOptions.cityWidth / 2f;
        var cameraZ = generationOptions.cityHeight / 2f; 
        Camera.main.transform.position = new Vector3(cameraX, 300f, cameraZ);
        Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        Camera.main.orthographicSize = generationOptions.cityHeight / 2f;
    }
    
    private void StartCar()
    {
        var gameObj = GameObject.Find("Car");
        var car = gameObj.GetComponent<TrunkMover>();
        car.canMove = true;
    }
}
