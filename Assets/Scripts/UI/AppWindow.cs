using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AppWindow : MonoBehaviour 
{
    [System.Serializable]
    public class OnActiveChangedCB : UnityEvent<bool> { }
    public OnActiveChangedCB OnActiveChanged;
	
    void OnEnable()
    {
        OnActiveChanged.Invoke(true);
    }

    void OnDisable()
    {
        OnActiveChanged.Invoke(false);
    }

}
