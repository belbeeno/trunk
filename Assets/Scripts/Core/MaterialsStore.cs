using UnityEngine;

public class MaterialsStore : MonoBehaviour
{
    private static MaterialsStore _instance;
    
    public static MaterialsStore instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("MaterialsStore").GetComponent<MaterialsStore>();
            }
            return _instance;
        }
    }
    
    public Material basic;
    public Material buildings;
}