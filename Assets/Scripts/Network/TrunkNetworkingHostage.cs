using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using NetMessage;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingHostage : TrunkNetworkingBase
{
    protected static TrunkNetworkingHostage _instance = null;
    public static TrunkNetworkingHostage Get()
    {
        return _instance;
    }
    public override bool IsHost() { return false; }

    TrunkNetworkDiscovery broadcaster = null;
    NetworkClient network = null;

    [SerializeField]
    TrunkMover mover = null;
    [SerializeField]
    Outside outsideTrunk = null;
    [SerializeField]
    GameObject policeCarInstance = null;
    [SerializeField]
    GameObject helicopterInstance = null;
    
    public override void Begin()
    {
        broadcaster = GetComponent<TrunkNetworkDiscovery>();
        broadcaster.Initialize();
        if (!broadcaster.StartAsClient(Connect))
        {
            Restart("Unable to listen for host on network!");
            return;
        }
        Log("Listening for a host!");

        initParams.Add(new NetHandlerInitParams(MsgType.Connect, OnConnectMsg));
        initParams.Add(new NetHandlerInitParams(ID.InitSession, OnInitSessionMsg));
        initParams.Add(new NetHandlerInitParams(ID.LoadSession, OnLoadSessionMsg));
        initParams.Add(new NetHandlerInitParams(ID.APB, OnAPBRequestMsg));
        initParams.Add(new NetHandlerInitParams(ID.TriggerPoliceCar, OnTriggerPoliceCarMsg));
        initParams.Add(new NetHandlerInitParams(ID.TriggerHelicopter, OnTriggerHelicopterMsg));
        initParams.Add(new NetHandlerInitParams(ID.GameOver, OnGameOverMsg));

        base.Begin();
    }
    public void Start()
    {
        _instance = this;
        _baseInstance = this;

        policeCarInstance.SetActive(false);
        helicopterInstance.SetActive(false);
    }
    public void Update()
    {
        if (GameManager.Get().LocalStatus == GameManager.PlayerStatus.InGameRinging && 
            GameManager.Get().RemoteStatus == GameManager.PlayerStatus.InGame)
        {
            EnableVoiceChat();
        }
        if (Input.GetButtonUp("Back"))
        {
            Application.Quit();
        }
    }
    public override void OnDestroy()
    {
        if (network != null)
        {
            network.Disconnect();
            network = null;
        }

        if (_instance == this)
        {
            _instance = null;
        }

        base.OnDestroy();
    }

    public override void SendMessage(short msgId, MessageBase msg)
    {
        if (network != null)
        {
            network.Send(msgId, msg);
        }
    }

    public override int VoiceChatID
    {
        get { return 1; }
    }
    public override void OnNewSampleCaptured(VoiceChat.VoiceChatPacket packet)
    {
        if (network == null) return;

        //Debug.Log("New sample captured: " + packet.PacketId);
        VoiceChatMsg msg = new VoiceChatMsg(packet);
        network.SendUnreliable(ID.VoiceChatPacket, msg);
    }

    public void Connect(string ip)
    {
        network = new NetworkClient();

        for (int i = 0; i < initParams.Count; i++ )
        {
            network.RegisterHandler(initParams[i].msgId, initParams[i].message);
        }

        network.Connect(ip, TrunkNetworkingOperator.GAME_PORT);
    }
    public void OnConnectMsg(NetworkMessage msg)
    {
        Log("Connected to server!");
        broadcaster.StopBroadcast();
    }
    public void OnGameOverMsg(NetworkMessage msg)
    {
        // If we're getting this at the hostage end, then the operator found us!
        Log("Hostage found!  You win!");
        OnGameWin.Invoke();
        Restart();
    }
    public void OnInitSessionMsg(NetworkMessage msg)
    {
        SeedMsg castedMsg = msg.ReadMessage<SeedMsg>();
        SetUpSession(castedMsg.seed);
    }
    public void OnLoadSessionMsg(NetworkMessage msg)
    {
        SeedMsg castedMsg = msg.ReadMessage<SeedMsg>();
        GameManager.Get().SetUpGame(castedMsg.seed, ValidateSession);
    }

    public void OnAPBRequestMsg(NetworkMessage msg)
    {
        APBRequest castedMsg = msg.ReadMessage<APBRequest>();
        Log("APB requested at position " + castedMsg.position.ToString());

        // Will be needed for hints.
        //Physics.CheckSphere(castedMsg.position, GameSettings.APB_RADIUS, LayerMask.NameToLayer("ClientOnly"));

        APBResponse response = new APBResponse();
        response.origin = castedMsg.position;
        response.origin.y = 0f;

        float distFromOriginSqrd = (Camera.main.transform.position - response.origin).sqrMagnitude;
        if (distFromOriginSqrd <= GameSettings.APB_RADIUS * GameSettings.APB_RADIUS)
        {
            if (GameManager.Get().LocalStatus == GameManager.PlayerStatus.InGame)
            {
                // Can only finish the game if we're still playing
                response.hints.Add(new APBResponse.Hint((mover != null ? mover.transform.position : Camera.main.transform.position), APBResponse.Hint.HintType.Hostage));
            }
        }

        if (outsideTrunk == null)
        {
            Log("Trunk exterior missing!", true);
        }
        else 
        {
            var allDroppedItems = outsideTrunk.GetAllDroppedItems();
            for (int i = 0; i < allDroppedItems.Count; i++)
            {
                response.origin.y = allDroppedItems[i].positionDropped.y;
                distFromOriginSqrd = (allDroppedItems[i].positionDropped - response.origin).sqrMagnitude;
                if (distFromOriginSqrd <= GameSettings.APB_RADIUS * GameSettings.APB_RADIUS)
                {
                    response.hints.Add(new APBResponse.Hint(allDroppedItems[i].positionDropped
                                                                        , allDroppedItems[i].itemName));
                }
            }
        }

#if UNITY_EDITOR
        Debug.DrawLine(Camera.main.transform.position, response.origin, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.forward * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.back * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.left * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.right * GameSettings.APB_RADIUS, Color.red, 5f);

        Vector3 prevPos = new Vector3(Mathf.Cos(0f) * GameSettings.APB_RADIUS, 0f, Mathf.Sin(0f) * GameSettings.APB_RADIUS);
        Vector3 nextPos = new Vector3();
        for (float i = 1f; i < 10f; i += 1f)
        {
            nextPos.x = Mathf.Cos((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;
            nextPos.z = Mathf.Sin((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;

            Debug.DrawLine(response.origin + prevPos, response.origin + nextPos, Color.red, 5f, false);

            prevPos = nextPos;
        }
        nextPos.Set(Mathf.Cos(0f), 0f, Mathf.Sin(0f));
        Debug.DrawLine(prevPos, nextPos * GameSettings.APB_RADIUS, Color.red, 5f);
#endif
        msg.conn.Send(ID.APB, response);
    }

    public void OnTriggerPoliceCarMsg(NetworkMessage msg)
    {
        TriggerPoliceMsg castedMsg = msg.ReadMessage<TriggerPoliceMsg>();
        StartCoroutine(PoliceCarThread(castedMsg.position));
    }
    public IEnumerator PoliceCarThread(Vector2 pos)
    {
        policeCarInstance.transform.position = new Vector3(pos.x, 0f, pos.y);
        policeCarInstance.SetActive(true);
        yield return new WaitForSeconds(GameSettings.COP_SIREN_PING_DURATION);
        policeCarInstance.SetActive(false);
    }

    public void OnTriggerHelicopterMsg(NetworkMessage msg)
    {
        TriggerHelicopterMsg castedMsg = msg.ReadMessage<TriggerHelicopterMsg>();
        StartCoroutine(HelicopterThread(castedMsg.yPos, castedMsg.goingRight));
    }
    public IEnumerator HelicopterThread(float yPos, bool goingRight)
    {
        Vector3 startPos = new Vector3((goingRight ? -500f : GameManager.Get().generationOptions.cityWidth + 500f), 0f, yPos);
        Vector3 endPos = new Vector3((goingRight ? GameManager.Get().generationOptions.cityWidth + 500f : -500f), 0f, yPos);
        helicopterInstance.transform.position = startPos;
        helicopterInstance.SetActive(true);
        float timer = GameSettings.HELICOPTER_PING_DURATION;
        while (timer > 0f)
        {
            helicopterInstance.transform.position = Vector3.Lerp(startPos, endPos, 1f - Mathf.Clamp01(timer / GameSettings.HELICOPTER_PING_DURATION));
            timer -= Time.deltaTime;
            yield return 0;
        }
        helicopterInstance.SetActive(false);
    }

}
