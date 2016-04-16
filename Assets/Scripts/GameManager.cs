using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera operatorProxyCamera;
    public GenerationOptions generationOptions;
    private CityGenerator _generator = new CityGenerator();
    
    public void SetUpDebugGame()
    {
        SetUpGame(Random.Range(int.MinValue, int.MaxValue));
        StartGame();
    }
    
    public void SetUpGame(int seed)
    {
        Random.seed = seed;
        
        var city = _generator.Generate(generationOptions);
        GenerateCity(city);
        InitializeRoutePlanner(city);
        RepositionProxyCamera();
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
    
    private void RepositionProxyCamera()
    {
        var cameraX =  generationOptions.cityWidth / 2f;
        var cameraZ = generationOptions.cityHeight / 2f; 
        operatorProxyCamera.transform.position = new Vector3(cameraX, 300f, cameraZ);
        operatorProxyCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        operatorProxyCamera.orthographicSize = generationOptions.cityHeight / 2f;
    }
    
    private static void StartCar()
    {
        var gameObj = GameObject.Find("Car");
        var car = gameObj.GetComponent<TrunkMover>();
        car.canMove = true;
    }
}
