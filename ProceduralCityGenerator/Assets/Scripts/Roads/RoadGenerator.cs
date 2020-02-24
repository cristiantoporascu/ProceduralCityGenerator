using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Roads;
using Assets.Scripts.Utility;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roadGameObject;

    private GameObject _roadParent;
    private Transform _roadParentTransform;

    public static List<Intersection> IntersectionsList = new List<Intersection>();
    public static List<Road> RoadList = new List<Road>();
    public static List<Point> CurrentPoints = new List<Point>();

    // Start is called before the first frame update
    private void Start()
    {
        _roadParent = GameObject.Find("Roads");
        if (_roadParent == null)
        {
            _roadParent = new GameObject("Roads");
        }

        _roadParentTransform = _roadParent.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        DrawRoads();
    }

    public void AddPoints(Vector3 newPoint)
    {
        CurrentPoints.Add(new Point(new Vector2(newPoint.x, newPoint.z)));
    }

    private void CreateRoad()
    {
        if (CurrentPoints.Count >= 2)
        {
            Point roadStart = new Point(CurrentPoints[0].Position);
            Point roadEnd = new Point(CurrentPoints[1].Position);

            Road newRoad = new Road(roadStart, roadEnd);

            RoadList.Add(newRoad);

            CurrentPoints.Clear();
        }
    }

    public void CalcRoadSubDivision()
    {
        CurrentPoints.Clear();

        List<Road> newRoadStructure = new List<Road>();
        foreach (var road in RoadList)
        {
            newRoadStructure = newRoadStructure.Concat(RoadSubDivider.SplitRoad(road)).ToList();
        }

        RoadList = newRoadStructure;
    }

    public void DrawRoads()
    {
        Debug.Log("Current road number: " + RoadList.Count);

        CreateRoad();

        if (RoadList.Count > GameObject.FindGameObjectsWithTag("Road").Length)
        {
            foreach (var road in RoadList)
            {
                if (!CheckRoadExists(road) && _roadGameObject != null)
                {
                    CreateRoadMesh(road);
                }
            }
        }
    }

    private void CreateRoadMesh(Road current)
    {
        // Initialise a new mesh for the prefab
        Mesh mesh = new Mesh();

        // Initialise properties of the mesh
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        // Initialise mesh points as Vector3
        Vector3 meshStartPoint = current.StartPoint.GetVector3Pos();
        Vector3 meshEndPoint = current.EndPoint.GetVector3Pos();
        
        // Get perpendicular on road to calculate the width of the mesh
        Vector3 sides = Vector3.Cross(meshStartPoint - meshEndPoint, Vector3.down).normalized;

        Vector3 meshTL = meshStartPoint + sides * (0.5f * 2.0f); // TODO: Set based on lane number, default 1
        Vector3 meshTR = meshStartPoint - sides * (0.5f * 2.0f);
        Vector3 meshBL = meshEndPoint + sides * (0.5f * 2.0f);
        Vector3 meshBR = meshEndPoint - sides * (0.5f * 2.0f);

        // Set the new vertices of the mesh
        vertices.AddRange(new Vector3[] { meshTL, meshTR, meshBL, meshBR });

        // Set the new indices of the mesh
        triangles.AddRange(new int[] { 0, 2, 1 });
        triangles.AddRange(new int[] { 1, 2, 3 });

        // Set the normals of the mesh
        normals.AddRange(new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up });

        // Setup the properties on the newly created mesh
        float length = Vector3.Distance(meshStartPoint, meshEndPoint) * 0.5f; // TODO: Set Tilling as variable
        uvs.AddRange(new Vector2[] { new Vector2(0, length), new Vector2(1, length), new Vector2(0, 0), new Vector2(1, 0) });

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        // Assign the new props to the mesh
        _roadGameObject.GetComponent<MeshFilter>().mesh = mesh;

        // Reference points for easier check
        LineRenderer roadLineRenderer = _roadGameObject.GetComponent<LineRenderer>();
        roadLineRenderer.SetPosition(0, new Vector3(current.StartPoint.Position.x, 0, current.StartPoint.Position.y));
        roadLineRenderer.SetPosition(1, new Vector3(current.EndPoint.Position.x, 0, current.EndPoint.Position.y));

        // Instantiate the prefab with the specified changes
        Instantiate(_roadGameObject, _roadParentTransform.transform);
    }

    private void CreateRoadFromLineRenderer(Road current)
    {
        LineRenderer roadLineRenderer = _roadGameObject.GetComponent<LineRenderer>();
        roadLineRenderer.startWidth = 0.3f;
        roadLineRenderer.endWidth = 0.3f;
        roadLineRenderer.SetPosition(0, new Vector3(current.StartPoint.Position.x, 0, current.StartPoint.Position.y));
        roadLineRenderer.SetPosition(1, new Vector3(current.EndPoint.Position.x, 0, current.EndPoint.Position.y));

        Instantiate(_roadGameObject, _roadParentTransform.transform);
    }

    private bool CheckRoadExists(Road current)
    {
        return
            GameObject.FindGameObjectsWithTag("Road").ToList()
                .Exists(o =>
                {
                    LineRenderer currentLineRenderer = o.GetComponent<LineRenderer>();

                    Vector3 startPos = currentLineRenderer.GetPosition(0);
                    Vector3 endPos = currentLineRenderer.GetPosition(1);
                    Road oRoad = new Road(new Point(new Vector2(startPos.x, startPos.z)), new Point(new Vector2(endPos.x, endPos.z)));

                    return oRoad.Equals(current);
                });
    }
}
