using UnityEngine;

public class PrefabStore : MonoBehaviour
{
    private static PrefabStore _instance;
    
    public static PrefabStore instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("PrefabStore").GetComponent<PrefabStore>();
            }
            return _instance;
        }
    }
    
    public GameObject park;
    public GameObject school;
    public GameObject monument;
}