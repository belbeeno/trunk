using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Networking;

using VoiceChat;
using NetMessage;

using Random = UnityEngine.Random;

public abstract class TrunkNetworkingBase : MonoBehaviour
{
    protected static TrunkNetworkingBase _baseInstance = null;
    public static TrunkNetworkingBase GetBase()
    {
        return _baseInstance;
    }
    public abstract bool IsHost();

    public struct NetHandlerInitParams
    {
        public NetHandlerInitParams(short _msgId, NetworkMessageDelegate _msg)
        {
            msgId = _msgId;
            message = _msg;
        }
        public short msgId;
        public NetworkMessageDelegate message;
    }

    protected List<NetHandlerInitParams> initParams = new List<NetHandlerInitParams>();

    public UnityEvent OnSessionEstablished;
    public UnityEvent OnResetImminent;
    public UnityEvent OnGameWin;

    public abstract int VoiceChatID { get; }
    public VoiceChatPlayer voiceChatPlayer = null;

    public virtual void Begin()
    {
        initParams.Add(new NetHandlerInitParams(MsgType.Disconnect, OnDisconnectMsg));
        initParams.Add(new NetHandlerInitParams(ID.VoiceChatPacket, OnVoiceChatMsg));
        initParams.Add(new NetHandlerInitParams(ID.PlayerStatusChange, OnPlayerStatusChanged));
        initParams.Add(new NetHandlerInitParams(ID.ValidateSession, OnValidateSessionMsg));
    }

    public abstract void SendMessage(short msgId, MessageBase msg);

    public void OnDisconnectMsg(NetworkMessage msg)
    {
        Restart("Disconnect detected!");
    }

    public void SetUpSession(int citySeed)
    {
        VoiceChat.VoiceChatRecorder.Instance.NetworkId = VoiceChatID;
        if (!VoiceChat.VoiceChatRecorder.Instance.StartRecording())
        {
            Log("VoiceChat Recording couldn't start!", true);
            return;
        }
        else
        {
            //Debug.Log("Voicechat Initialized, starting...");
            VoiceChatRecorder.Instance.NewSample += OnNewSampleCaptured;
            voiceChatPlayer.gameObject.SetActive(true);
        }

        GameManager.Get().SetUpGame(citySeed, ValidateSession);
    }
    public void ValidateSession()
    {
        SeedMsg msg = new SeedMsg();
        msg.seed = Random.seed;
        SendMessage(ID.ValidateSession, msg);
    }
    public void OnValidateSessionMsg(NetworkMessage msg)
    {
        SeedMsg castedMsg = msg.ReadMessage<SeedMsg>();
        GameManager.Get().remoteValidationSeed = castedMsg.seed;
    }
    public void SetGameIsValid()
    {
        OnSessionEstablished.Invoke();
    }

    public abstract void OnNewSampleCaptured(VoiceChatPacket packet);
    public void OnVoiceChatMsg(NetworkMessage msg)
    {
        VoiceChatMsg castedMsg = msg.ReadMessage<VoiceChatMsg>();
        //Debug.Log("Voicechat recieved packet id " + castedMsg.payload.PacketId);
        voiceChatPlayer.OnNewSample(castedMsg.payload);
    }
    public void EnableVoiceChat()
    {
        VoiceChat.VoiceChatRecorder.Instance.AutoDetectSpeech = true;
        GameManager.Get().LocalStatus = GameManager.PlayerStatus.InGame;
    }

    public void LocalPlayerStatusChanged(GameManager.PlayerStatus newStatus)
    {
        UpdateStatusMsg castedMsg = new UpdateStatusMsg();
        castedMsg.status = (int)newStatus;
        SendMessage(ID.PlayerStatusChange, castedMsg);
    }
    public void OnPlayerStatusChanged(NetworkMessage msg)
    {
        UpdateStatusMsg castedMsg = msg.ReadMessage<UpdateStatusMsg>();
        GameManager.Get().RemoteStatus = (GameManager.PlayerStatus)castedMsg.status;
    }

    public virtual void OnDestroy()
    {
        if (_baseInstance == this)
        {
            _baseInstance = null;
        }

        if (VoiceChat.VoiceChatRecorder.Instance != null)
        {
            Destroy(VoiceChat.VoiceChatRecorder.Instance);
        }
    }

    #region Logging and Scene Management
    public UnityEngine.UI.Text statusText = null;
    public void Log(string msg, bool asError = false)
    {
        statusText.text += "\n" + (asError ? "<color=\"red\">ERROR:</color> " : string.Empty) + msg;
        DebugConsole.SetText("NetworkStatus", msg);
    }
    public void Restart(string msg = null)
    {
        OnResetImminent.Invoke();

        StopAllCoroutines();
        StartCoroutine(RestartingIn(msg));
    }
    private IEnumerator RestartingIn(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            Log(msg + "  Resetting in...", true);
        }
        else
        {
            Log("Resetting in...");
        }

        for (int sec = 5; sec > 0; sec--)
        {
            Log(sec + "...");
            yield return new WaitForSeconds(1f);
        }

        Destroy(this);
        SceneManager.LoadScene("Trunk", LoadSceneMode.Single);
    }
    #endregion
}
