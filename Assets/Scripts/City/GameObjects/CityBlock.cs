using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CityBlock : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshFilter _filter;
    
    public BuildingPlotData data;
    
    private Color _buildingColor = Color.white;

	void Start () 
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        
        CreateMesh();
        SetMaterial();
	}
    
    private void CreateMesh()
    {
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int floor = 0; floor <= data.numFloors; floor++)
        {
            for (int corner = 0; corner < data.corners.Length; corner++)
            {
                points.Add(data.corners[corner] + Vector3.up * data.floorHeight * floor);
                colors.Add(_buildingColor);
                uvs.Add(new Vector2((float)corner, ((float)floor) / data.numFloors));
                if (floor > 0)
                {
                    tris.Add((corner + 0) + (floor - 1) * data.corners.Length);
                    tris.Add((corner + 0) + (floor - 0) * data.corners.Length);
                    tris.Add((corner + 1) % data.corners.Length + (floor - 1) * data.corners.Length);

                    tris.Add((corner + 0) + (floor - 0) * data.corners.Length);
                    tris.Add((corner + 1) % data.corners.Length + (floor - 0) * data.corners.Length);
                    tris.Add((corner + 1) % data.corners.Length + (floor - 1) * data.corners.Length);
                }
            }
        }
        
        Vector3 averagePointsOnRoof = new Vector3();
        int lastIdx = points.Count;
        for (int i = 0; i < data.corners.Length; ++i)
        {
            averagePointsOnRoof += data.corners[i];
            tris.Add(lastIdx - (data.corners.Length - i));
            tris.Add(lastIdx);
            tris.Add(lastIdx - (data.corners.Length - (i + 1) % data.corners.Length));
        }
        averagePointsOnRoof *= 1f / data.corners.Length;
        averagePointsOnRoof.y = ((float)data.numFloors + 0.5f) * data.floorHeight;
        points.Add(averagePointsOnRoof);
        colors.Add(_buildingColor);
        uvs.Add(new Vector2(1f, Mathf.Max((float)data.numFloors - 1f, 0f)));

        generatedMesh.SetVertices(points);
        generatedMesh.SetColors(colors);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);

        _filter.mesh = generatedMesh;
    }
    
    public void SetMaterial()
    {
        string[] guids = AssetDatabase.FindAssets("BuildingMat");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        else
        {
            Debug.LogError("Couldn't find BuildingMat when creating new CityBlock");
        }
    }
}