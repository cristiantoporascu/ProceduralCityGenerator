using UnityEngine;

public class BuildingGenerationUI : MonoBehaviour
{
    private GameObject _manager;

    // Start is called before the first frame update
    private void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("Manager");
    }

    // Update is called once per frame
    private void Update()
    {
        BuildingPlacementEvent();
    }

    private void BuildingPlacementEvent()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _manager.GetComponent<BuildingGenerator>().ProcessBuildings();
        }
    }
}
