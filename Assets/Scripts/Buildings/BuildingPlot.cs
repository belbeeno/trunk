using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

[RequireComponent(typeof(MeshRenderer)
                , typeof(MeshFilter))]
public class BuildingPlot : MonoBehaviour 
{
    MeshRenderer _renderer = null;
    MeshFilter _filter = null;
    public Vector3[] corners = new Vector3[4];

    [SerializeField]
    protected Color buildingColor = Color.white;

    public int numFloors = 5;

    private void CreateMesh()
    {
        //buildingColor = Random.ColorHSV(0f, 1f, 0.25f, 0.75f, 0f, 1f);
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int floor = 0; floor <= numFloors; ++floor)
        {
            for (int corner = 0; corner < corners.Length; ++corner)
            {
                points.Add(corners[corner] + Vector3.up * Mathf.Max(floor - 0.5f, 0f));
                colors.Add(buildingColor);
                uvs.Add(new Vector2((float)corner
                                    , Mathf.Max((float)floor - 1f, 0f)));
                if (floor > 0)
                {
                    tris.Add((corner + 0) + (floor - 1) * corners.Length);
                    tris.Add((corner + 0) + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 1) * corners.Length);

                    tris.Add((corner + 0) + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 1) * corners.Length);
                }
            }
        }

        Vector3 averagePointsOnRoof = new Vector3();
        int lastIdx = points.Count;
        for (int i = 0; i < corners.Length; ++i)
        {
            averagePointsOnRoof += corners[i];
            tris.Add(lastIdx - (corners.Length - i));
            tris.Add(lastIdx);
            tris.Add(lastIdx - (corners.Length - (i + 1) % corners.Length));
        }
        averagePointsOnRoof *= 1f / corners.Length;
        averagePointsOnRoof.y = (float)numFloors - 0.5f ;
        points.Add(averagePointsOnRoof);
        colors.Add(buildingColor);
        uvs.Add(new Vector2(1f, Mathf.Max((float)numFloors - 1f, 0f)));

        generatedMesh.SetVertices(points);
        generatedMesh.SetColors(colors);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);

        _filter.mesh = generatedMesh;
    }

	void Start () 
    {
        if (!AreCornersValid())
        {
            Debug.LogError("Trying to build a BuildingPlot with not enough corners!", gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            if (_renderer == null) _renderer = GetComponent<MeshRenderer>();
            if (_filter == null) _filter = GetComponent<MeshFilter>();
            CreateMesh();
        }
	}

    private bool AreCornersValid()
    {
        return (corners.Length > 2) && (numFloors >= 1);
    }












#if UNITY_EDITOR
    private Vector3 tl = new Vector3(-0.5f, 0f, 0.5f);
    private Vector3 tr = new Vector3(0.5f, 0f, 0.5f);
    private Vector3 bl = new Vector3(-0.5f, 0f, -0.5f);
    private Vector3 br = new Vector3(0.5f, 0f, -0.5f);

    public void OnDrawGizmos()
    {
        const float drawDistance = 35f;
        if (!AreCornersValid())
        {
            DrawError();
            return;
        }
        Vector2 projPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 projCamPosition = new Vector2(Camera.current.transform.position.x, Camera.current.transform.position.z);
        float relativeDist = Vector2.Distance(projPosition, projCamPosition) / drawDistance;
        if (relativeDist <= 1f)
        {
            Gizmos.color = buildingColor * Mathf.Clamp01(1.5f - relativeDist);
            DrawWireframe();
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (!AreCornersValid())
        {
            DrawError();
            return;
        }

        Gizmos.color = Color.green;
        DrawWireframe();
    }
    private void DrawWireframe()
    {
        for (int i = 0; i < corners.Length; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(corners[i]
                                                                                + (Vector3.up * numFloors * j * 0.33333f))
                    , transform.localToWorldMatrix.MultiplyPoint(corners[(i + 1) % corners.Length]
                                                                                + (Vector3.up * numFloors * j * 0.33333f)));
            }

            Gizmos.DrawRay(transform.localToWorldMatrix.MultiplyPoint(corners[i]), Vector3.up * numFloors * transform.lossyScale.y);
        }
    }
    private void DrawError()
    {
        Gizmos.color = Color.red * (Random.Range(0.75f, 1f));
        Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(tl), transform.localToWorldMatrix.MultiplyPoint(br));
        Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(tr), transform.localToWorldMatrix.MultiplyPoint(bl));
    }

    [MenuItem("GameObject/Trunk/Building Plot", false, 0)]
    public static void CreateBuildingPlot(MenuCommand command)
    {
        GameObject go = CreateBuildingPlot(command.context as GameObject, Vector3.zero);
        Selection.activeObject = go;
    }

    public static GameObject CreateBuildingPlot(GameObject cityBlockGO, Vector3 localPos)
    {
        BuildingPlot[] plots = FindObjectsOfType<BuildingPlot>();
        IEnumerable<BuildingPlot> plotIter = plots.OrderBy(x => x.name);
        string plotName = "New Building Plot";
        int iter = 0;
        bool newPlotNameFound = true;
        do
        {
            newPlotNameFound = true;
            foreach (BuildingPlot plot in plotIter)
            {
                if (plot.name.Equals(plotName + (iter == 0 ? string.Empty : " (" + iter + ")")))
                {
                    iter++;
                    newPlotNameFound = false;
                }
            }
        } while (!newPlotNameFound);
        GameObject go = new GameObject(plotName + (iter == 0 ? string.Empty : " (" + iter + ")"), typeof(MeshRenderer));
        BuildingPlot newPlot = go.AddComponent<BuildingPlot>();
        MeshRenderer rend = go.GetComponent<MeshRenderer>();
        MeshFilter filter = go.GetComponent<MeshFilter>();
        newPlot._renderer = rend;
        newPlot._filter = filter;
        newPlot.numFloors = Random.Range(3, 10);
        newPlot.buildingColor = Color.white * Random.Range(0.25f, 1f);

        string[] guids = AssetDatabase.FindAssets("BuildingMat");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            newPlot._renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        else
        {
            Debug.LogError("Couldn't find BuildingMat when creating new BuildingPlot!", go);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 newCorner = new Vector3(0.9f * (i == 0 || i == 3 ? 1f : -1f)
                                            , 0f
                                            , 0.9f * (i < 2 ? 1f : -1f));
            if (i < newPlot.corners.Length)
            {
                newPlot.corners[i] = newCorner;
            }
        }

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, cityBlockGO);
        go.transform.localPosition = localPos;
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        return go;
    }
#endif
}
