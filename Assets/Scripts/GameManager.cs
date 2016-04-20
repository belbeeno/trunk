using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Camera operatorProxyCamera;
    public GenerationOptions generationOptions;
    private CityGenerator _generator = new CityGenerator();
    
    private bool _readyToStart;
    private bool _otherReadyToStart;
    private bool _gameHasStarted;
    
    public void Update()
    {
        if (!_gameHasStarted && _otherReadyToStart && _readyToStart)
        {
            _gameHasStarted = true;
            StartGame();
        }
    }
    
    public void MarkOtherReady()
    {
        _otherReadyToStart = true;
    }
    
    public void SetUpGame(int seed, Action callback)
    {
        Random.seed = seed;
        
        var city = _generator.Generate(generationOptions);
        GenerateCity(city);
        InitializeRoutePlanner(city);
        RepositionProxyCamera();
        
        _readyToStart = true;
        callback();
    }
    
    public void StartGame()
    {
        StartCar();
    }
    
    private void GenerateCity(GenerationData result)
    {
        var gameObj = GameObject.Find("City");
        var city = gameObj.GetComponent<City>();
        city.GenerateGeometry(result);
    }
    
    private void InitializeRoutePlanner(GenerationData result)
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
    
    public void SetUpDebugGame()
    {
        SetUpGame(Random.Range(int.MinValue, int.MaxValue), () => {});
        StartGame();
    }
}
