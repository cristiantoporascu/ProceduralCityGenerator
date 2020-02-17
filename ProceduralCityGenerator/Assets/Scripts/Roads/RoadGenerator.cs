using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Roads;
using Assets.Scripts.Utility;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roadGameObject;

    public static List<Road> RoadList = new List<Road>();
    public static List<Point> CurrentPoints = new List<Point>();

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
            Debug.Log("RoadAddedMouseEvent");

            Point roadStart = new Point(CurrentPoints[0].Position);
            Point roadEnd = new Point(CurrentPoints[1].Position);

            Road newRoad = new Road(roadStart, roadEnd);

            RoadList.Add(newRoad);

            CurrentPoints.Clear();
        }
    }

    public void CalcRoadSubDivision()
    {
        Debug.Log("RoadAddedSubDivisionEvent");

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
                    LineRenderer roadLineRenderer = _roadGameObject.GetComponent<LineRenderer>();
                    roadLineRenderer.startWidth = 0.3f;
                    roadLineRenderer.endWidth = 0.3f;
                    roadLineRenderer.SetPosition(0, new Vector3(road.StartPoint.Position.x, 0, road.StartPoint.Position.y));
                    roadLineRenderer.SetPosition(1, new Vector3(road.EndPoint.Position.x, 0, road.EndPoint.Position.y));

                    Instantiate(_roadGameObject, GameObject.Find("Roads").transform);
                }
            }
        }
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
