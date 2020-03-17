using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PCGEditor
{
    [Serializable]
    public class PCGEditorRoads
    {
        public int LaneDropdownIndex = 0;
        public int NumberLanes = 1;

        public float RoadLengthMin = 1.0f;
        public float RoadLengthMinLimit = 1.0f;

        public float RoadLengthMax = 10.0f;
        public float RoadLengthMaxLimit = 20.0f;

        public float RoadSpacingMin = 1.0f;
        public float RoadSpacingMinLimit = 1.0f;

        public float RoadSpacingMax = 10.0f;
        public float RoadSpacingMaxLimit = 20.0f;

        public int NumberSideWalkVariants;
        public bool PrefabsListFoldout;
        public List<GameObject> PrefabsSidewalks;
    }

    [Serializable]
    public class PCGEditorBuildings
    {
        // ---- Prefabs properties ---- \\
        public bool PrefabGroupFoldout;
        public int NumberPrefabs;

        public bool PrefabsListFoldout;
        public List<BuildingsPrefabsEditor> Prefabs;
    }

    [Serializable]
    public class BuildingsPrefabsEditor
    {
        public GameObject Prefab;
        public float ActiveRange;
    }
}

