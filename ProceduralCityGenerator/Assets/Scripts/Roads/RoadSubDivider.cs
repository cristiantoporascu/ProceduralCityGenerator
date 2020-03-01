using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Roads
{
    public static class RoadSubDivider
    {
        public static List<Road> SplitRoad(Road road, RoadGenerator roadGenerator)
        {
            // Generate the new list of roads that will be passed back
            List<Road> subDividedRoads = new List<Road>();

            // Generate a random percentage for division
            float splitPercentage = Random.Range(0.1f, 0.95f);

            // Initiate the positions as vector 3
            Vector3 startPos = road.StartPoint.GetVector3Pos();
            Vector3 endPos = road.EndPoint.GetVector3Pos();
            float splitLength = road.Length();
            splitLength *= splitPercentage;

            Vector3 dir = (startPos - endPos).normalized;
            Vector3 splitPos = endPos + (dir * splitLength);

            // Calculate the perpendicular on the division location
            // Also generate the division points and the adjacent roads
            Vector3 perpendicularDown = Vector3.Cross(startPos - endPos, Vector3.down).normalized;
            float newRoadLength = Random.Range(roadGenerator.PcgEditorRoads.RoadLengthMin, roadGenerator.PcgEditorRoads.RoadLengthMax);
            Vector3 splittedRoadEnd = splitPos + (perpendicularDown * newRoadLength);

            Road FirstSideSplit = new Road(
                new Point(new Vector2(splitPos.x, splitPos.z)), 
                new Point(new Vector2(splittedRoadEnd.x, splittedRoadEnd.z)),
                roadGenerator.PcgEditorRoads.NumberLanes
                );

            Vector3 splittedRoadOtherEnd = splitPos + (perpendicularDown * newRoadLength * -1);

            Road SecondSideSplit = new Road(
                new Point(new Vector2(splitPos.x, splitPos.z)),
                new Point(new Vector2(splittedRoadOtherEnd.x, splittedRoadOtherEnd.z)),
                roadGenerator.PcgEditorRoads.NumberLanes
            );

            // Check if the new generated roads intersect with either of the existing roads
            bool firstSideSplitValid = false;
            bool secondSideSplitValid = false;

            if (!RoadIntersecting(FirstSideSplit, roadGenerator.PcgEditorRoads.RoadSpacingMax, roadGenerator.PcgEditorRoads.RoadSpacingMin))
            {
                RoadGenerator.RoadList.RemoveAll(r => r.Equals(FirstSideSplit));
                subDividedRoads.Add(FirstSideSplit);
                firstSideSplitValid = true;
            }

            if (!RoadIntersecting(SecondSideSplit, roadGenerator.PcgEditorRoads.RoadSpacingMax, roadGenerator.PcgEditorRoads.RoadSpacingMin))
            {
                RoadGenerator.RoadList.RemoveAll(r => r.Equals(SecondSideSplit));
                subDividedRoads.Add(SecondSideSplit);
                secondSideSplitValid = true;
            }

            //
            // Temp disable as altering code functionality
            //
            //if (firstSideSplitValid || secondSideSplitValid)
            //{
            //    RoadGenerator.IntersectionsList.Add(
            //        new Intersection(
            //            new List<Point> {
            //                new Point(
            //                    new Vector2(splitPos.x, splitPos.z)
            //                    )
            //                }
            //            )
            //        );
            //}

            subDividedRoads.Add(road);

            return subDividedRoads;
        }

        private static bool RoadIntersecting(Road currentRoad, float maxAccepted, float minAccepted)
        {
            foreach (var road in RoadGenerator.RoadList)
            {
                bool closeToStart = DistanceFromPointToRoad(road.StartPoint, currentRoad) < maxAccepted;
                bool closeToEnd = DistanceFromPointToRoad(road.EndPoint, currentRoad) < maxAccepted;

                bool minToRoadDistance = MinRoadToRoadDistance(currentRoad, road, minAccepted);

                if (closeToEnd || closeToStart || minToRoadDistance)
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

        private static bool MinRoadToRoadDistance(Road first, Road second, float minAccepted)
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
