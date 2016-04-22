using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Networking;

using VoiceChat;

public abstract class TrunkNetworkingBase : MonoBehaviour
{
    public UnityEvent OnSessionEstablished;
    public UnityEvent OnResetImminent;
    public UnityEvent OnGameWin;

    public abstract int VoiceChatID { get; }
    public VoiceChatPlayer voiceChatPlayer = null;

    public abstract void Begin();
    public virtual void SetUpSession(int citySeed, Action callback)
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

        var gameObj = GameObject.Find("GameManager");
        var manager = gameObj.GetComponent<GameManager>();
        
        Log("Setting up game");
        manager.SetUpGame(citySeed, callback);
        
        OnSessionEstablished.Invoke();
    }

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

        SceneManager.LoadScene("Trunk", LoadSceneMode.Single);
    }

    public abstract void OnNewSampleCaptured(VoiceChatPacket packet);
    public void OnVoiceChatMsg(NetworkMessage msg)
    {
        NetMessage.VoiceChatMsg castedMsg = msg.ReadMessage<NetMessage.VoiceChatMsg>();
        //Debug.Log("Voicechat recieved packet id " + castedMsg.payload.PacketId);
        voiceChatPlayer.OnNewSample(castedMsg.payload);
    }
}
