using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct GameManagerFlowStep
    {
        public GameObject target;
        public bool isEnabled;
    }

    public GameManagerFlowStep[] initSteps = new GameManagerFlowStep[0];

    public Camera operatorProxyCamera;
    public RectTransform operatorMapCanvasRect;
    public GenerationOptions generationOptions;
    private CityGenerator _generator = new CityGenerator();
    
    private bool _readyToStart;
    private bool _otherReadyToStart;
    private bool _gameHasStarted;

	public void Start()
	{
		initSteps.ForEach(x => x.target.SetActive(x.isEnabled));
	}
    
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
        RefreshMapSizeAndPositions();
        
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
    
    private void RefreshMapSizeAndPositions()
    {
        var cameraX =  generationOptions.cityWidth / 2f;
        var cameraZ = generationOptions.cityHeight / 2f; 
        operatorProxyCamera.transform.position = new Vector3(cameraX, 400f, cameraZ);
        operatorProxyCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        operatorProxyCamera.orthographicSize = generationOptions.cityHeight / 2f;

        operatorMapCanvasRect.anchoredPosition3D = new Vector3(0f, 300f, 0f);
        operatorMapCanvasRect.sizeDelta = Vector2.one * Mathf.Min(generationOptions.cityHeight, generationOptions.cityWidth);
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
