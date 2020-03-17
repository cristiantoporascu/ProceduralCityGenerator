using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Buildings;
using Assets.Scripts.PCGEditor;
using Assets.Scripts.Roads;
using Assets.Scripts.Utility;
using UnityEngine;
using Random = System.Random;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roadGameObject;

    private GameObject _roadParent;
    private Transform _roadParentTransform;

    public static List<Intersection> IntersectionsList = new List<Intersection>();
    public static List<Road> RoadList = new List<Road>();
    public static List<Point> CurrentPoints = new List<Point>();

    public static List<GameObject> SidewalkList = new List<GameObject>();

    [HideInInspector] public PCGEditorRoads PcgEditorRoads;


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

    private void CheckForCurrentRoadStructure()
    {
        var roadGenData = GameObject.FindGameObjectWithTag("GeneratedRoadData");

        if (roadGenData != null)
        {
            for(var i = 0; i < roadGenData.transform.childCount; i++)
            {
                var tempGO = roadGenData.transform.GetChild(i).gameObject;

                if (tempGO != null)
                {
                    if (tempGO.tag == "Road" && !CheckRoadExists(tempGO))
                    {
                        RoadList.Add(GenRoadEntryFromLineRenderer(tempGO));
                        DestroyImmediate(tempGO);
                    }
                    else if (tempGO.tag == "Sidewalks")
                    {
                        tempGO.transform.parent = _roadParentTransform;
                        SidewalkList.Add(tempGO);
                    }
                }
            }

            if (roadGenData.transform.childCount <= 0)
                DestroyImmediate(roadGenData);
        }
    }

    private void CreateRoad()
    {
        if (CurrentPoints.Count >= 2)
        {
            Point roadStart = new Point(CurrentPoints[0].Position);
            Point roadEnd = new Point(CurrentPoints[1].Position);

            Road newRoad = new Road(roadStart, roadEnd, PcgEditorRoads.NumberLanes);

            RoadList.Add(newRoad);

            CurrentPoints.Clear();
        }
    }

    private void DrawRoads()
    {
        CheckForCurrentRoadStructure();

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

    private void CalcRoadSubDivision()
    {
        CurrentPoints.Clear();

        List<Road> newRoadStructure = new List<Road>();
        foreach (var road in RoadList)
        {
            newRoadStructure = newRoadStructure.Concat(RoadSubDivider.SplitRoad(road, this)).ToList();
        }

        RoadList = newRoadStructure;
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

        Vector3 meshTL = meshStartPoint + sides * (0.5f * current.Lanes);
        Vector3 meshTR = meshStartPoint - sides * (0.5f * current.Lanes);
        Vector3 meshBL = meshEndPoint + sides * (0.5f * current.Lanes);
        Vector3 meshBR = meshEndPoint - sides * (0.5f * current.Lanes);

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

        // Instantiate the prefab with the specified changes
        var newGameObject = Instantiate(_roadGameObject, _roadParentTransform.transform);

        // Assign the new props to the mesh
        newGameObject.GetComponent<MeshFilter>().mesh = mesh;

        // Change material based on road width
        newGameObject.GetComponent<MeshRenderer>().material =
            Resources.Load<Material>("RoadMaterials/" + current.Lanes);

        newGameObject.GetComponent<SaveRoadProp>().Lanes = current.Lanes;

        // Reference points for easier check
        LineRenderer roadLineRenderer = newGameObject.GetComponent<LineRenderer>();
        roadLineRenderer.SetPosition(0, new Vector3(current.StartPoint.Position.x, 0, current.StartPoint.Position.y));
        roadLineRenderer.SetPosition(1, new Vector3(current.EndPoint.Position.x, 0, current.EndPoint.Position.y));
    }

    private bool CheckRoadExists(Road current)
    {
        return
            GameObject.FindGameObjectsWithTag("Road").ToList()
                .Exists(o =>
                {
                    Road oRoad = GenRoadEntryFromLineRenderer(o);

                    return oRoad.Equals(current);
                });
    }

    private bool CheckRoadExists(GameObject current)
    {
        return
            RoadList
                .Exists(o =>
                {
                    Road currentRoad = GenRoadEntryFromLineRenderer(current);

                    return o.Equals(currentRoad);
                });
    }

    private Road GenRoadEntryFromLineRenderer(GameObject current)
    {
        LineRenderer currentLineRenderer = current.GetComponent<LineRenderer>();

        Vector3 startPos = currentLineRenderer.GetPosition(0);
        Vector3 endPos = currentLineRenderer.GetPosition(1);

        return new Road(
            new Point(new Vector2(startPos.x, startPos.z)), 
            new Point(new Vector2(endPos.x, endPos.z)),
            current.GetComponent<SaveRoadProp>().Lanes
            );
    }

    public void ClearRoads()
    {
        var roadGameObjects = GameObject.FindGameObjectsWithTag("Road");

        foreach (var road in roadGameObjects)
        {
            DestroyImmediate(road);
        }
        RoadList.Clear();
    }

    public void CheckUnusedRoadMeshes(Road removedRoad)
    {
        var roadGameObjects = GameObject.FindGameObjectsWithTag("Road");

        foreach (var roadGO in roadGameObjects)
        {
            if (GenRoadEntryFromLineRenderer(roadGO).Equals(removedRoad))
            {
                DestroyImmediate(roadGO);
            }
        }
    }

    private void GenerateRoadSidewalk()
    {
        foreach (var road in RoadList)
        {
            // Initiate the positions as vector 3
            Vector3 startPos = road.StartPoint.GetVector3Pos();
            Vector3 endPos = road.EndPoint.GetVector3Pos();
            Vector3 dir = (startPos - endPos).normalized;
            float length = road.Length();

            bool processRightSide = true;
            var randomizer = new Random();
            for (float f = 0.5f; f < length || processRightSide; f += 1.0f)
            {
                // If one side has been completed, follow the other side
                if (f > length && processRightSide)
                {
                    processRightSide = false;
                    f = 0.5f;
                }

                // The offset of which the sidewalk has to move on the side of the road
                Vector3 bROffset = new Vector3(-dir.z, 0, dir.x);
                if (processRightSide)
                    bROffset *= -1;

                // The position of the sidewalk before offsetting the road
                Vector3 prePosCenter = endPos + (dir * f);
                GameObject sidewalk = PcgEditorRoads.PrefabsSidewalks[randomizer.Next(0, PcgEditorRoads.NumberSideWalkVariants)];

                if (sidewalk != null)
                {
                    Debug.Log("Sidewalk object");
                    var sidewalkCollider = sidewalk.GetComponent<BoxCollider>();

                    // Road offset based on the length of the building
                    Vector3 roadOffset = bROffset.normalized * (sidewalkCollider.size.z * 0.5f + road.Lanes * 0.5f);
                    Vector3 postPosCenter = prePosCenter + roadOffset;

                    if (f - sidewalkCollider.size.x * 0.5f < 0 || f + sidewalkCollider.size.x * 0.5f > length)
                        continue;

                    sidewalk.transform.position = postPosCenter;
                    sidewalk.transform.LookAt(prePosCenter);

                    // Move the sidewalk by the scale of the center of the building
                    sidewalk.transform.position =
                        new Vector3(
                            sidewalk.transform.position.x,
                            sidewalk.transform.position.y + sidewalkCollider.size.y * 0.5f,
                            sidewalk.transform.position.z
                        );

                    // Validate sidewalk and set output
                    GameObject output = InstantiateValidSidewalk(sidewalk);

                    // The building has been validated and is placed in the scene
                    if (output != null)
                        SidewalkList.Add(output);
                }
            }
        }
    }

    private GameObject InstantiateValidSidewalk(GameObject newGameObject)
    {
        if (newGameObject != null)
        {
            GameObject newSideWalk = Instantiate(newGameObject);

            if (!BuildingUtilities.IntersectsRoad(newSideWalk))
            {
                newSideWalk.transform.parent = _roadParentTransform;
                return newSideWalk;
            }
            else
            {
                DestroyImmediate(newSideWalk);
            }
        }

        return null;
    }

    public void ClearRoadSidewalks()
    {
        foreach (var sideWalk in SidewalkList)
        {
            DestroyImmediate(sideWalk);
        }
        SidewalkList.Clear();
    }

    public void SubDivisionEventListener()
    {
        Debug.Log("Road sub division process started");

        ClearRoadSidewalks();
        gameObject.GetComponent<BuildingGenerator>().ClearBuildings();
        CalcRoadSubDivision();
    }

    public void GenerateRoadSidewalkListener()
    {
        ClearRoadSidewalks();

        Debug.Log("Road sidewalk generator process started");
        GenerateRoadSidewalk();
    }
}
