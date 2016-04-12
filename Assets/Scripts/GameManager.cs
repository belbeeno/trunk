using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationOptions generationOptions;    
    private CityGenerator _generator = new CityGenerator();
    
    public void SetUpGame(bool isHost)
    {
        SetUpGame(isHost, Random.Range(int.MinValue, int.MaxValue));
    }
    
    public void SetUpGame(bool isHost, int seed)
    {
        Random.seed = seed;
        
        var city = _generator.Generate(generationOptions);
        GenerateCity(city);
        InitializeRoutePlanner(city);
        
        if (isHost)
        {
            RepositionCamera();
        }
    }
    
    public void StartGame()
    {
        StartCar();
    }
    
    private void GenerateCity(GenerationResult result)
    {
        var gameObj = GameObject.Find("City");
        var city = gameObj.GetComponent<City>();
        city.GenerateGeometry(result);
    }
    
    private void InitializeRoutePlanner(GenerationResult result)
    {
        var gameObj = GameObject.Find("RoutePlanner");
        var routePlanner = gameObj.GetComponent<RoutePlanner>();
        routePlanner.graph = result.roadGraph;
    }
    
    private void RepositionCamera()
    {
        var cameraX =  generationOptions.cityWidth / 2f;
        var cameraZ = generationOptions.cityHeight / 2f; 
        Camera.main.transform.position = new Vector3(cameraX, 300f, cameraZ);
        Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        Camera.main.orthographicSize = generationOptions.cityHeight / 2f;
    }
    
    private static void StartCar()
    {
        var gameObj = GameObject.Find("Car");
        var car = gameObj.GetComponent<TrunkMover>();
        car.canMove = true;
    }
}