using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Roads
{
    public static class RoadSubDivider
    {
        public static List<Road> SplitRoad(Road road)
        {
            List<Road> subDividedRoads = new List<Road>();

            float splitPercentage = Random.Range(0.1f, 0.95f);

            Vector3 startPos = road.StartPoint.GetVector3Pos();
            Vector3 endPos = road.EndPoint.GetVector3Pos();
            float splitLength = road.Length();
            splitLength *= splitPercentage;

            Vector3 dir = (startPos - endPos).normalized;
            Vector3 splitPos = endPos + (dir * splitLength);

            Vector3 perpendicularDown = Vector3.Cross(startPos - endPos, Vector3.down).normalized;
            float newRoadLength = Random.Range(0.5f, 9.0f); // TODO: Sliders for values 
            Vector3 splittedRoadEnd = splitPos + (perpendicularDown * newRoadLength);

            Road FirstSideSplit = new Road(
                new Point(new Vector2(splitPos.x, splitPos.z)), 
                new Point(new Vector2(splittedRoadEnd.x, splittedRoadEnd.z))
                );

            Vector3 splittedRoadOtherEnd = splitPos + (perpendicularDown * newRoadLength * -1);

            Road SecondSideSplit = new Road(
                new Point(new Vector2(splitPos.x, splitPos.z)),
                new Point(new Vector2(splittedRoadOtherEnd.x, splittedRoadOtherEnd.z))
            );

            bool firstSideSplitValid = false;
            bool secondSideSplitValid = false;

            if (!RoadIntersecting(FirstSideSplit, 0.5f))
            {
                RoadGenerator.RoadList.RemoveAll(r => r.Equals(FirstSideSplit));
                subDividedRoads.Add(FirstSideSplit);
            }

            if (!RoadIntersecting(SecondSideSplit, 0.5f))
            {
                RoadGenerator.RoadList.RemoveAll(r => r.Equals(SecondSideSplit));
                subDividedRoads.Add(SecondSideSplit);
            }

            subDividedRoads.Add(road);

            return subDividedRoads;
        }

        private static bool RoadIntersecting(Road currentRoad, float maxAccepted) // TODO: Slider for maxAccepted
        {
            foreach (var road in RoadGenerator.RoadList)
            {
                bool closeToStart = DistanceFromPointToRoad(road.StartPoint, currentRoad) < maxAccepted;
                bool closeToEnd = DistanceFromPointToRoad(road.EndPoint, currentRoad) < maxAccepted;

                bool minRoadDistance = MinRoadToRoadDistance(currentRoad, road, 0.5f);

                if (closeToEnd || closeToStart || minRoadDistance)
                    return true;
            }

            return false;
        }

        private static float DistanceFromPointToRoad(Point point, Road road)
        {
            Vector3 toProject = point.Position - road.StartPoint.Position;
            Vector3 projectOn = road.EndPoint.Position - road.StartPoint.Position;

            float scalarPojection = Vector3.Dot(projectOn, toProject) / projectOn.magnitude;

            return Mathf.Sqrt(toProject.magnitude - scalarPojection);
        }

        private static bool MinRoadToRoadDistance(Road first, Road second, float minAccepted) // TODO: Slider for minAccepted
        {
            if (Vector2.Distance(first.StartPoint.Position, second.StartPoint.Position) < minAccepted)
                return true;
            if (Vector2.Distance(first.StartPoint.Position, second.EndPoint.Position) < minAccepted)
                return true;
            if (Vector2.Distance(first.EndPoint.Position, second.StartPoint.Position) < minAccepted)
                return true;
            if (Vector2.Distance(first.EndPoint.Position, second.EndPoint.Position) < minAccepted)
                return true;

            return false;
        }
    }
}
