using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TimeDisplay : MonoBehaviour 
{
    Text text = null;
	// Use this for initialization
	void Start () 
    {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (GameManager.Get() != null 
            && TrunkNetworkingBase.GetBase() != null
            && TrunkNetworkingBase.GetBase().IsHost())
        {
            text.text = "2:0" + Mathf.FloorToInt((GameSettings.GAME_SESSION_LENGTH - GameManager.Get().GetElapsedTime()) / 60f).ToString() + " PM";
            
        }
	}
}
