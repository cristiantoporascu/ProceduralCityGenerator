using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Roads;
using UnityEngine;
using UnityEngine.UIElements;

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
            for (float f = 1.0f; f < length || processRightSide; f += 1.0f)
            {
                // If one side has been completed, follow the other side
                if (f > length && processRightSide)
                {
                    processRightSide = false;
                    f = 1.0f;
                }

                // The offset of which the building has to move on the side of the road
                Vector3 bROffset = new Vector3(-dir.z, 0, dir.x);
                if (processRightSide)
                    bROffset *= -1;

                // Try 3 different types of buildings for higher accuracy
                for (var i = 0; i < 3; i++)
                {
                    // The position of the building before offsetting the road
                    Vector3 prePosCenter = endPos + (dir * f);
                    float perlinVal = Mathf.PerlinNoise(prePosCenter.x / 10f, prePosCenter.z / 10f);

                    GameObject building = null;

                    var descOrderBuildPrefabList =
                        buildingGenerator.PcgEditorBuildings.OrderByDescending(o => o.ActiveRange).ToList();
                    foreach (var prefab in descOrderBuildPrefabList)
                    {
                        building = prefab.ActiveRange < perlinVal ? prefab.Prefab : null;

                        if (building != null)
                        {
                            break;
                        }

                        if (descOrderBuildPrefabList.IndexOf(prefab) == descOrderBuildPrefabList.Count - 1)
                        {
                            building = descOrderBuildPrefabList[0].Prefab;
                        }
                    }

                    if (building != null)
                    {
                        var buildingCollider = building.GetComponent<BoxCollider>();

                        // Road offset based on the length of the building
                        Vector3 roadOffset = bROffset.normalized * (buildingCollider.size.z * 0.5f + current.Lanes * 0.5f + 1.0f /* Sidewalk default width */);
                        Vector3 postPosCenter = prePosCenter + roadOffset;

                        if (f - buildingCollider.size.x < 0 || f + buildingCollider.size.x > length)
                            continue;

                        building.transform.position = postPosCenter;
                        building.transform.LookAt(prePosCenter);

                        // Move the building by the scale of the center of the building
                        building.transform.position =
                            new Vector3(
                                building.transform.position.x,
                                building.transform.position.y + buildingCollider.size.y / 2,
                                building.transform.position.z
                            );

                        // Validate building and set output
                        GameObject output = buildingGenerator.InstantiateValidProcessedBuilding(building);

                        // The building has been validated and is placed in the scene
                        if (output != null)
                            BuildingGenerator.BuildingList.Add(output);
                    }
                }
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

        public static bool IntersectsRoad(GameObject gameObject)
        {
            // Can be made more efficient, currently evaluates all roads, check distance if needed
            foreach (var road in RoadGenerator.RoadList)
            {
                Vector3 startPos = road.StartPoint.GetVector3Pos();
                Vector3 endPos = road.EndPoint.GetVector3Pos();
                Vector3 dir = (startPos - endPos).normalized;

                // Offset from the center of the road with 95% spacing
                Vector3 offset = new Vector3(-dir.z, 0, dir.x).normalized * (road.Lanes * 0.5f) * 0.95f;

                Ray rayCenter = new Ray(startPos, endPos - startPos);
                Ray rayLeftSide = new Ray(startPos + offset, (endPos + offset) - (startPos + offset));
                Ray rayRightSide = new Ray(startPos - offset, (endPos - offset) - (startPos - offset));

                RaycastHit hit = new RaycastHit();
                float distance = Vector2.Distance(road.StartPoint.Position, road.EndPoint.Position);

                if (gameObject.GetComponent<BoxCollider>().Raycast(rayCenter, out hit, distance) 
                    || gameObject.GetComponent<BoxCollider>().Raycast(rayLeftSide, out hit, distance)
                    || gameObject.GetComponent<BoxCollider>().Raycast(rayRightSide, out hit, distance))
                    return true;
            }

            return false;
        }
    }
}

