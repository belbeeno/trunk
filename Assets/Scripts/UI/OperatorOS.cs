using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OperatorOS : MonoBehaviour 
{
    [System.Serializable]
    public class IconToWindowMapping
    {
        public OnDoubleClick icon;
        public AppWindow window;

        public void Init()
        {
            window.OnActiveChanged.AddListener(OnWindowStateChanged);
            icon.OnDoubleClickEvent.AddListener(OnButtonDoubleClick);
        }
            
        public void OnWindowStateChanged(bool newState)
        {
            if (icon.myButton)
            {
                icon.myButton.interactable = !newState;
            }
        }

        public void OnButtonDoubleClick()
        {
            window.gameObject.SetActive(true);
        }
    }

    public IconToWindowMapping[] appMapping = new IconToWindowMapping[0];

	// Use this for initialization
	void Start () {
	    for (int i = 0; i < appMapping.Length; i++)
        {
            appMapping[i].Init();
        }
	}
}
