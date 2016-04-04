using UnityEngine;

public class City : MonoBehaviour 
{
    public GenerationOptions generationOptions;
    
    private CityGenerator _generator = new CityGenerator();
    
	void Start () 
    {    
    }
	
	void Update () 
    {
	}
    
    public void Generate()
    {
        // Clear children
        while (transform.childCount > 0) 
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        // Data generation
        var result = _generator.Generate(generationOptions);
        
        // Roads
        var roadsObj = new GameObject("Roads");
        roadsObj.transform.parent = this.transform;
        foreach (var roadData in result.roads)
        {
            var roadObj = new GameObject("Road");
            roadObj.transform.parent = roadsObj.transform;
            var roadScript = roadObj.AddComponent<Road>();
            roadScript.data = roadData;
        }
        
        // City blocks
        var cityBlocksObj = new GameObject("City Blocks");
        cityBlocksObj.transform.parent = this.transform;
        foreach (var plotData in result.cityBlocks)
        {
            var plotObj = new GameObject("City Block");
            plotObj.transform.parent = cityBlocksObj.transform;
            plotObj.AddComponent<MeshFilter>();
            plotObj.AddComponent<MeshRenderer>();
            var cityBlockScript = plotObj.AddComponent<CityBlock>();
            cityBlockScript.data = plotData;
        }
        
        // River
        var riverObj = new GameObject("River");
        riverObj.transform.parent = this.transform;
        var riverScript = riverObj.AddComponent<River>();
        riverScript.data = result.river;
    }
}
