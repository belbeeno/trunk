using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour 
{
    private static DebugConsole _instance;

    public static void SetText(string name, string msg)
    {
#if UNITY_EDITOR
        Debug.Log("[" + name + "] - " + msg);
#endif
        if (Debug.isDebugBuild && _instance != null)
        {
            if (_instance.messages.ContainsKey(name))
            {
                if (string.IsNullOrEmpty(msg))
                {
                    _instance.messages.Remove(name);
                }
                else
                {
                    _instance.messages[name] = msg;
                }
            }
            else if (!string.IsNullOrEmpty(msg))
            {
                _instance.messages.Add(name, msg);
            }
        }
    }

    public static string GetText(string name)
    {
        string retVal = string.Empty;
        if (Debug.isDebugBuild && _instance != null)
        {
            _instance.messages.TryGetValue(name, out retVal);
        }
        return retVal;
    }

    public Dictionary<string, string> messages = new Dictionary<string,string>();
    [SerializeField, Header("CONSOLE - Press ~ in game to show/hide")]
    protected Text consoleText = null;
    [SerializeField]
    protected GameObject consolePanel = null;

	// Use this for initialization
	void Start () 
    {
        _instance = this;
	}

    private float touchTimer = 1f;

	// Update is called once per frame
	void Update ()
    {
        if (!Debug.isDebugBuild)
        {
            enabled = false;
            if (consolePanel)
            {
                consolePanel.SetActive(false);
            }
        }

        if (consoleText == null || consolePanel == null)
        {
            Debug.LogWarning("DebugConsole could not be initialized; missing a reference to some objects!", gameObject);
            enabled = false;
            if (consolePanel)
            {
                consolePanel.SetActive(false);
            }
            return;
        }
        //*
        if (Input.touchSupported 
                && Input.touchCount == 1 
                && Input.touches[0].phase == TouchPhase.Ended)
        {
            float prevTimer = touchTimer;
            touchTimer -= Time.deltaTime;
            if (prevTimer > 0f && touchTimer <= 0f)
            {
                consolePanel.SetActive(!consolePanel.activeSelf);
            }
        }
        else if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
        }
        else
        {
            touchTimer = 1f;
        }
        //*/
        if (consolePanel.activeSelf)
        {
            consoleText.text = "";
            foreach (KeyValuePair<string, string> iter in messages)
            {
                consoleText.text += "<color=\"green\">" + iter.Key + "</color>: " + iter.Value + "\n";
            }
        }
	}
}
