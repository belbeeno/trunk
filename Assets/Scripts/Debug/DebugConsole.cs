using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour 
{
    private static DebugConsole _instance;

    public static void SetText(string name, string msg)
    {
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

        if ((Input.touchSupported 
                && Input.touchCount > 0 
                && Input.touches[0].phase == TouchPhase.Ended)
            || Input.GetKeyUp(KeyCode.BackQuote))
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
        }
#if UNITY_EDITOR
        if (Input.GetButtonUp("Cancel"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
#endif

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
