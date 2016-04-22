using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using VoiceChat;

public class ChooseMode : MonoBehaviour 
{
    public Fade screenFader = null;
    
    public TrunkNetworkingOperator hostNetwork = null;
    public TrunkNetworkingHostage clientNetwork = null;

    public UnityEngine.UI.Button HostButton = null;
    public UnityEngine.UI.Button ClientButton = null;

    public GameObject AudioEnabledGO = null;
    public GameObject AudioMissingGO = null;
    public UnityEngine.UI.Dropdown AudioDeviceDropdown = null;

    public Fade networkStatus = null;

    private bool waitForDevice = false;

    public void Start()
    {
        if (VoiceChatRecorder.Instance.AvailableDevices.Length > 0)
        {
            AudioEnabledGO.SetActive(true);
            AudioMissingGO.SetActive(false);

            List<string> options = new List<string>(VoiceChatRecorder.Instance.AvailableDevices);
            AudioDeviceDropdown.ClearOptions();
            AudioDeviceDropdown.AddOptions(options);

            waitForDevice = false;
        }
        else
        {
            AudioEnabledGO.SetActive(false);
            AudioMissingGO.SetActive(true);

            HostButton.interactable = false;
            ClientButton.interactable = false;

            waitForDevice = true;
        }
    }

    public void Update()
    {
        if (waitForDevice)
        {
            if (Microphone.devices.Length > 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Trunk", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }

    protected void PrepAudioDevice()
    {
        VoiceChatRecorder.Instance.Device = AudioDeviceDropdown.options[AudioDeviceDropdown.value].text;
    }

    public void OnHostSelected()
    {
        screenFader.OnFadeOutComplete.AddListener(OnFadeComplete);
        screenFader.FadeOut();
        hostNetwork.enabled = true;
        hostNetwork.Begin();
    }

    public void OnClientSelected()
    {
        screenFader.OnFadeOutComplete.AddListener(OnFadeComplete);
        screenFader.FadeOut();
        clientNetwork.enabled = true;
        clientNetwork.Begin();
    }

    public void OnFadeComplete()
    {
        screenFader.OnFadeOutComplete.RemoveListener(OnFadeComplete);
        networkStatus.FadeIn();
    }
}
