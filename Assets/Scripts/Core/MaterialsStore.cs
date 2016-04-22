using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MaterialsStore : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/Assign mats on selected to Basic", true)]
    public static bool ValidateSelected()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Renderer>() != null;
    }

    [MenuItem("GameObject/Assign mats on selected to Basic", false)]
    public static void AssignMaterialsToBasic()
    {
        if (Selection.activeGameObject != null)
        {
            Renderer rend = Selection.activeGameObject.GetComponent<Renderer>();
            if (rend)
            {
                Material[] newMats = new Material[rend.sharedMaterials.Length];
                for (int i = 0; i < rend.sharedMaterials.Length; i++ )
                {
                    newMats[i] = MaterialsStore.instance.basic;
                }
                
                rend.sharedMaterials = newMats;
            }
        }
    }
#endif

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
    public Material roads;
    public Material sidewalks;
    public Material grass;
    public Material water;
}