using System.Collections.Generic;
using Assets.Scripts.Buildings;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject BuildingPrefabSmall;
    public GameObject BuildingPrefabMedium;
    public GameObject BuildingPrefabLarge;

    [HideInInspector] public static GameObject BuildingParent;
    [HideInInspector] public static Transform BuildingParentTransform;

    public static List<GameObject> BuildingList = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        BuildingParent = GameObject.Find("Buildings");
        if (BuildingParent == null)
        {
            BuildingParent = new GameObject("Buildings");
        }

        BuildingParentTransform = BuildingParent.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update() { }

    public void ProcessBuildings()
    {
        ClearBuildings();

        Debug.Log("Bulding processing based on roads");
        foreach (var road in RoadGenerator.RoadList)
        {
            BuildingUtilities.EvaluateBuildingPlacing(road, this);
        }
    }

    public GameObject InstantiateValidProcessedBuilding(GameObject newGameObject)
    {
        if (newGameObject != null)
        {
            GameObject newBuilding = Instantiate(newGameObject);

            if (BuildingUtilities.CheckValidPlacement(newBuilding))
            {
                newBuilding.transform.parent = BuildingParentTransform;
                return newBuilding;
            }
            else
            {
                DestroyImmediate(newBuilding);
            }
        }

        return null;
    }

    public void ClearBuildings()
    {
        foreach (var building in BuildingList)
        {
            DestroyImmediate(building);
        }
        BuildingList.Clear();
    }
}
