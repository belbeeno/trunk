﻿using UnityEngine;
using System.Collections;

public class ChooseMode : MonoBehaviour 
{
    public Fade screenFader = null;
    
    public TrunkNetworkingOperator hostNetwork = null;
    public TrunkNetworkingHostage clientNetwork = null;

    public Fade networkStatus = null;

    public void OnHostSelected()
    {
        screenFader.OnFadeComplete.AddListener(OnFadeComplete);
        screenFader.FadeOut();
        hostNetwork.enabled = true;
        hostNetwork.Begin();
    }

    public void OnClientSelected()
    {
        screenFader.OnFadeComplete.AddListener(OnFadeComplete);
        screenFader.FadeOut();
        clientNetwork.enabled = true;
        clientNetwork.Begin();
    }

    public void OnFadeComplete()
    {
        screenFader.OnFadeComplete.RemoveListener(OnFadeComplete);
        networkStatus.FadeIn();
    }
}
