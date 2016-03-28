using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif
public class CityBlock : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}






    
#if UNITY_EDITOR
    [MenuItem("GameObject/Trunk/City Block", false, 0)]
    public static void CreateCityBlock(MenuCommand command)
    {
        Selection.activeGameObject = CreateCityBlock(command.context as GameObject, Vector3.zero);
    }

    public static GameObject CreateCityBlock(GameObject target, Vector3 localPos)
    {
        CityBlock[] blocks = FindObjectsOfType<CityBlock>();
        IEnumerable<CityBlock> blockIter = blocks.OrderBy(x => x.name);
        string blockName = "New City Block";
        int iter = 0;
        bool newBlockName = true;
        do
        {
            newBlockName = true;
            foreach (CityBlock block in blockIter)
            {
                if (block.name.Equals(blockName + (iter == 0 ? string.Empty : " (" + iter + ")")))
                {
                    iter++;
                }
            }
        } while (!newBlockName);
        GameObject go = new GameObject(blockName + (iter == 0 ? string.Empty : " (" + iter + ")"), typeof(CityBlock));

        GameObject blockBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blockBase.name = "Base";
        blockBase.transform.SetParent(go.transform, false);
        blockBase.transform.localScale = new Vector3(4f, 0.25f, 4f);

        GameObjectUtility.SetParentAndAlign(go, target);
        go.transform.localPosition = localPos;

        BuildingPlot.CreateBuildingPlot(go, new Vector3(1f, 0f, 1f));
        BuildingPlot.CreateBuildingPlot(go, new Vector3(1f, 0f, -1f));
        BuildingPlot.CreateBuildingPlot(go, new Vector3(-1f, 0f, 1f));
        BuildingPlot.CreateBuildingPlot(go, new Vector3(-1f, 0f, -1f));

        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

        return go;
    }

    [MenuItem("GameObject/Trunk/City Region", false, 0)]
    public static void CreateCityRegion(MenuCommand command)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cityBlockRow = new GameObject("Row " + i);
            Undo.RegisterCreatedObjectUndo(cityBlockRow, "Create " + cityBlockRow.name);

            CreateCityBlock(cityBlockRow, new Vector3(-6f, 0f, 0f));
            CreateCityBlock(cityBlockRow, new Vector3(0f, 0f, 0f));
            CreateCityBlock(cityBlockRow, new Vector3(6f, 0f, 0f));

            GameObjectUtility.SetParentAndAlign(cityBlockRow, command.context as GameObject);
            cityBlockRow.transform.localPosition = new Vector3(0f, 0f, 6f * (i - 1));
        }
        Selection.activeGameObject = command.context as GameObject;
    }
#endif
}
