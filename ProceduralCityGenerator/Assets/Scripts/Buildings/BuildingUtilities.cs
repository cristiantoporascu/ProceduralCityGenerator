using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.Roads;
using UnityEngine;

namespace Assets.Scripts.Buildings
{
    public static class BuildingUtilities
    {
        public static void EvaluateBuildingPlacing(Road current, BuildingGenerator buildingGenerator)
        {
            // Initiate the positions as vector 3
            Vector3 startPos = current.StartPoint.GetVector3Pos();
            Vector3 endPos = current.EndPoint.GetVector3Pos();
            Vector3 dir = (startPos - endPos).normalized;
            float length = current.Length();

            bool processRightSide = true;
            for (float f = 2.0f; f < length || processRightSide; f += 1.3f)
            {
                // If one side has been completed, follow the other side
                if (f > length && processRightSide)
                {
                    processRightSide = false;
                    f = 2.0f;
                }

                // The offset of which the building has to move on the side of the road
                Vector3 bROffset = new Vector3(-dir.z, 0, dir.x);
                if (processRightSide)
                    bROffset *= -1;

                // The position of the building before offsetting the road
                Vector3 prePosCenter = endPos + (dir * f);
                float perlinVal = Mathf.PerlinNoise(prePosCenter.x / 10f, prePosCenter.z / 10f);

                GameObject building = null;

                if (perlinVal < .25f)
                {
                    building = buildingGenerator.BuildingPrefabSmall;
                }
                else if (perlinVal < .5f)
                {
                    building = buildingGenerator.BuildingPrefabMedium;
                }
                else
                {
                    building = buildingGenerator.BuildingPrefabLarge;
                }

                // Road offset based on the length of the building
                Vector3 roadOffset = bROffset.normalized *  building.GetComponent<Transform>().lossyScale.z;
                Vector3 postPosCenter = prePosCenter + roadOffset;

                if (f - building.GetComponent<Transform>().lossyScale.x < 0 || f + building.GetComponent<Transform>().lossyScale.x > length)
                    continue;

                building.transform.position = postPosCenter;
                building.transform.LookAt(prePosCenter);

                // Move the building by the scale of the center of the building
                building.transform.position = 
                    new Vector3(
                        building.transform.position.x, 
                        building.transform.position.y + building.transform.lossyScale.y / 2, 
                        building.transform.position.z
                    );

                // Validate building and set output
                GameObject output = buildingGenerator.InstantiateValidProcessedBuilding(building);

                // The building has been validated and is placed in the scene
                if(output != null)
                    BuildingGenerator.BuildingList.Add(output);
            }
        }

        public static bool CheckValidPlacement(GameObject building)
        {
            if (BuildingGenerator.BuildingList.Count == 0)
            {
                return true;
            }

            foreach (GameObject other in BuildingGenerator.BuildingList)
                if (Vector3.Distance(building.transform.position, other.transform.position) > 15f)
                    continue;
                else if (IntersectsOther(building, other))
                    return false;
                else if (IntersectsRoad(building))
                    return false;

            return true;
        }

        private static bool IntersectsOther(GameObject main, GameObject other)
        {
            BoxCollider mainCollider = main.GetComponent<BoxCollider>();
            BoxCollider otherCollider = other.GetComponent<BoxCollider>();

            return mainCollider != null && mainCollider.bounds.Intersects(otherCollider.bounds);
        }

        private static bool IntersectsRoad(GameObject building)
        {
            // Can be made more efficient, currently evaluates all roads, check distance if needed
            foreach (var road in RoadGenerator.RoadList)
            {
                Ray ray = new Ray(road.StartPoint.GetVector3Pos(), road.EndPoint.GetVector3Pos() - road.StartPoint.GetVector3Pos());
                RaycastHit hit = new RaycastHit();
                float distance = Vector2.Distance(road.StartPoint.Position, road.EndPoint.Position);

                if (building.GetComponent<BoxCollider>().Raycast(ray, out hit, distance))
                    return true;
            }

            return false;
        }
    }
}

