using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    protected static GameManager _instance;
    public static GameManager Get()
    {
        if (_instance == null) _instance = FindObjectOfType<GameManager>();
        return _instance;
    }

    public enum PlayerStatus : int
    {
        NotConnected,

        Loading,        // Being loading
        LoadingReady,   // Loading is complete; waiting for remote player
        LoadingFailed,  // Random seed at the end of generation is divergent!  Retry...

        PreGame,        // Mostly for showing UI to the hostage
        InGamePreCall,  // Haven't picked up the phone yet
        InGameRinging,  // It's ringing...
        InGame,
    }

    [System.Serializable]
    public class OnPlayerStatusChangedCB : UnityEvent<PlayerStatus> { }

    [SerializeField, ShowOnly]
    private PlayerStatus localStatus = PlayerStatus.NotConnected;
    public PlayerStatus LocalStatus
    {
        get { return localStatus; }
        set
        {
            if (localStatus != value)
            {
                localStatus = value;
                if (TrunkNetworkingBase.GetBase() != null)
                {
                    TrunkNetworkingBase.GetBase().LocalPlayerStatusChanged(localStatus);
                }
                OnLocalStatusChanged.Invoke(localStatus);
            }
        }
    }
    public OnPlayerStatusChangedCB OnLocalStatusChanged;

    [SerializeField, ShowOnly]
    private PlayerStatus remoteStatus = PlayerStatus.NotConnected;
    public PlayerStatus RemoteStatus 
    { 
        get { return remoteStatus; } 
        set
        {
            if (remoteStatus != value)
            {
                remoteStatus = value;
                OnRemoteStatusChanged.Invoke(remoteStatus);
            }
        }
    }
    public OnPlayerStatusChangedCB OnRemoteStatusChanged;

    public bool IsReadyToPlay()
    {
        // Eh it's the last day who cares.
        return localStatus != PlayerStatus.NotConnected
            && localStatus != PlayerStatus.Loading
            && localStatus != PlayerStatus.LoadingFailed
            && remoteStatus != PlayerStatus.NotConnected
            && remoteStatus != PlayerStatus.Loading
            && remoteStatus != PlayerStatus.LoadingFailed;
    }

    public int remoteValidationSeed = -1;

    [System.Serializable]
    public struct GameManagerFlowStep
    {
        public GameObject target;
        public bool isEnabled;
    }

    public GameManagerFlowStep[] initSteps = new GameManagerFlowStep[0];

    public Camera operatorProxyCamera;
    public RectTransform operatorMapCanvasRect;
    public OperatorPanAndZoom operatorControls = null;
    public GenerationOptions generationOptions;
    private CityGenerator _generator = new CityGenerator();
    
    private bool _gameHasStarted;
    public bool HasGameStarted() { return _gameHasStarted; }

	public void Start()
	{
		initSteps.ForEach(x => x.target.SetActive(x.isEnabled));
        _instance = this;
	}
    
    public void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public void Update()
    {
        if (!_gameHasStarted )
        {
            if (IsReadyToPlay())
            {
                if (Random.seed == remoteValidationSeed)
                {
                    StartGame();

                    LocalStatus = (TrunkNetworkingBase.GetBase().IsHost() ? PlayerStatus.InGamePreCall : PlayerStatus.PreGame);
                    TrunkNetworkingBase.GetBase().SetGameIsValid();
                }
                else
                {
                    remoteValidationSeed = -1;
                    LocalStatus = PlayerStatus.LoadingFailed;
                }
            }
        }
    }
    
    public void SetUpGame(int seed, Action callback)
    {
        StartCoroutine(SetUpGameCoroutine(seed, callback));
    }

    public IEnumerator SetUpGameCoroutine(int seed, Action callback)
    {
        Random.seed = seed;
        LocalStatus = PlayerStatus.Loading;
        
        var city = _generator.Generate(generationOptions);
        yield return 0;
        GenerateCity(city);
        yield return 0;
        InitializeRoutePlanner(city);
        yield return 0;
        RefreshMapSizeAndPositions();
        
        LocalStatus = PlayerStatus.LoadingReady;
        callback();
    }
    
    public void StartGame()
    {
        _gameHasStarted = true;
        var gameObj = GameObject.Find("Car");
        var car = gameObj.GetComponent<TrunkMover>();
        car.isMoving = true;
    }
    
    private void GenerateCity(GenerationData result)
    {
        var gameObj = GameObject.Find("City");
        Transform xform = gameObj.transform;
        for (int i = 0; i < xform.childCount; i++)
        {
            Destroy(xform.GetChild(i));
        }
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
        
        if (operatorControls != null)
        {
            operatorControls.Init();
        }
    }
    
    public void SetUpDebugGame()
    {
        SetUpGame(Random.Range(int.MinValue, int.MaxValue), () => { });
        StartGame();
    }
}
