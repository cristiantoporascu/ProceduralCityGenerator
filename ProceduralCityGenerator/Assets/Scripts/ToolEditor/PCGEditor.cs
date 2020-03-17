using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.PCGEditor;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PCGEditor : EditorWindow
{
    private GameObject _manager;
    private PCGEditorProps _pcgEditorProps;

    [MenuItem("Window/PCG Editor")]
    public static void Init()
    {
        GetWindow<PCGEditor>("PCG Editor");
    }

    private void OnEnable()
    {
        PCGEditorLoader.PcgEditorManagerLoader();

        _manager = GameObject.FindGameObjectWithTag("Manager");

        var toolConfig = Resources.Load<PCGEditorProps>("PCGToolConfig");

        if (toolConfig != null)
        {
            _pcgEditorProps = toolConfig;
        }
        else
        {
            _pcgEditorProps = CreateInstance<PCGEditorProps>();
        }
    }

    private void RoadSettingsGui()
    {
        PCGEditorUtility.HeadlineLabel("Road settings");

        // -------- LANE NUMBER -------- \\
        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Lane number");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);

        _pcgEditorProps.PcgEditorRoads.LaneDropdownIndex = 
            EditorGUILayout.Popup(_pcgEditorProps.PcgEditorRoads.LaneDropdownIndex, new[] {"One lane", "Two lanes", "Four lanes"});

        if (_pcgEditorProps.PcgEditorRoads.LaneDropdownIndex == 0)
        {
            _pcgEditorProps.PcgEditorRoads.NumberLanes = 1;
        }
        else
        {
            _pcgEditorProps.PcgEditorRoads.NumberLanes = 2 * _pcgEditorProps.PcgEditorRoads.LaneDropdownIndex;
        }

        EditorGUILayout.EndHorizontal();


        // -------- ROAD LENGTH -------- \\
        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Road Length");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Min Val:", _pcgEditorProps.PcgEditorRoads.RoadLengthMin.ToString());
        EditorGUILayout.LabelField("Max Val:", _pcgEditorProps.PcgEditorRoads.RoadLengthMax.ToString());
        EditorGUILayout.MinMaxSlider(
            ref _pcgEditorProps.PcgEditorRoads.RoadLengthMin, 
            ref _pcgEditorProps.PcgEditorRoads.RoadLengthMax,
            _pcgEditorProps.PcgEditorRoads.RoadLengthMinLimit,
            _pcgEditorProps.PcgEditorRoads.RoadLengthMaxLimit);

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        // -------- ROAD SPACING -------- \\
        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Road Spacing");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Min Val:", _pcgEditorProps.PcgEditorRoads.RoadSpacingMin.ToString());
        EditorGUILayout.LabelField("Max Val:", _pcgEditorProps.PcgEditorRoads.RoadSpacingMax.ToString());
        EditorGUILayout.MinMaxSlider(
            ref _pcgEditorProps.PcgEditorRoads.RoadSpacingMin,
            ref _pcgEditorProps.PcgEditorRoads.RoadSpacingMax,
            _pcgEditorProps.PcgEditorRoads.RoadSpacingMinLimit,
            _pcgEditorProps.PcgEditorRoads.RoadSpacingMaxLimit);

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15f);
        if (GUILayout.Button("Run Road Subdivision"))
        {
            _manager.GetComponent<RoadGenerator>().SubDivisionEventListener();
        }

        if (GUILayout.Button("Run Road L-System division"))
        {
            _manager.GetComponent<RoadGenerator>().SubDivisionEventListener();
        }
        if (GUILayout.Button("Clear Roads"))
        {
            _manager.GetComponent<RoadGenerator>().ClearRoads();
        }

        // -------- ROAD SIDEWALK VARIANTS -------- \\
        GUILayout.Space(15f);

        _pcgEditorProps.PcgEditorRoads.SidewalkGroupFoldout =
            EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorRoads.SidewalkGroupFoldout, "Road sidewalk");

        if (_pcgEditorProps.PcgEditorRoads.SidewalkGroupFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);

            EditorGUILayout.LabelField("Number variants");
            _pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants =
                EditorGUILayout.IntField(_pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants);

            if (_pcgEditorProps.PcgEditorRoads.PrefabsSidewalks != null)
            {
                if (_pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants > _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Count)
                {
                    for (var i = 0; i < _pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants - _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Count; i++)
                    {
                        _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Add(null);
                    }
                }
                else if (_pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants < _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Count)
                {
                    _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks
                        .RemoveRange(_pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants, _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Count - _pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants);
                }
            }
            else
            {
                _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks = new List<GameObject>();
            }
            EditorGUILayout.EndHorizontal();

            if (_pcgEditorProps.PcgEditorRoads.NumberSideWalkVariants > 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                _pcgEditorProps.PcgEditorRoads.PrefabsListFoldout =
                    EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorRoads.PrefabsListFoldout,
                        new GUIContent("Prefabs", "GameObject => " +
                                                  "The game object which you want to be placed in the scene up which allows multiple variants of sidewalks."));

                if (_pcgEditorProps.PcgEditorRoads.PrefabsListFoldout)
                {
                    GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                    EditorGUILayout.BeginVertical();

                    for (var i = 0; i < _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks.Count; i++)
                    {
                        _pcgEditorProps.PcgEditorRoads.PrefabsSidewalks[i] = EditorGUILayout
                            .ObjectField(_pcgEditorProps.PcgEditorRoads.PrefabsSidewalks[i], typeof(GameObject), true) as GameObject;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10f);
        if (GUILayout.Button("Run Road Sidewalk Generator"))
        {
            _manager.GetComponent<RoadGenerator>().GenerateRoadSidewalkListener();
        }
        if (GUILayout.Button("Clear Sidewalks"))
        {
            _manager.GetComponent<RoadGenerator>().ClearRoadSidewalks();
        }
    }

    private void BuildingSettingsGui()
    {
        PCGEditorUtility.HeadlineLabel("Building settings");
        GUILayout.Space(15f);

        // -------- BUILDING VARIANTS ON PERLIN -------- \\

        _pcgEditorProps.PcgEditorBuildings.PrefabGroupFoldout =
            EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorBuildings.PrefabGroupFoldout, "Building prefabs");
        if (_pcgEditorProps.PcgEditorBuildings.PrefabGroupFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);

            EditorGUILayout.LabelField("Number");
            _pcgEditorProps.PcgEditorBuildings.NumberPrefabs =
                EditorGUILayout.IntField(_pcgEditorProps.PcgEditorBuildings.NumberPrefabs);

            if (_pcgEditorProps.PcgEditorBuildings.PrefabsBuildings != null)
            {
                if (_pcgEditorProps.PcgEditorBuildings.NumberPrefabs > _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Count)
                {
                    for (var i = 0; i < _pcgEditorProps.PcgEditorBuildings.NumberPrefabs - _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Count; i++)
                    {
                        _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Add(new BuildingsPrefabsEditor());
                    }
                }
                else if (_pcgEditorProps.PcgEditorBuildings.NumberPrefabs < _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Count)
                {
                    _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings
                        .RemoveRange(_pcgEditorProps.PcgEditorBuildings.NumberPrefabs, _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Count - _pcgEditorProps.PcgEditorBuildings.NumberPrefabs);
                }
            }
            else
            {
                _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings = new List<BuildingsPrefabsEditor>();
            }
            EditorGUILayout.EndHorizontal();

            if (_pcgEditorProps.PcgEditorBuildings.NumberPrefabs > 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                _pcgEditorProps.PcgEditorBuildings.PrefabsListFoldout =
                    EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorBuildings.PrefabsListFoldout,
                        new GUIContent("Prefabs", "GameObject | Perlin Range => " +
                                                  "The game object which you want to be placed in the scene up to the specified range based on a perlin noise map"));

                if (_pcgEditorProps.PcgEditorBuildings.PrefabsListFoldout)
                {
                    GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                    EditorGUILayout.BeginVertical();

                    for (var i = 0; i < _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings[i].Prefab = EditorGUILayout
                            .ObjectField(_pcgEditorProps.PcgEditorBuildings.PrefabsBuildings[i].Prefab, typeof(GameObject), true) as GameObject;
                        _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings[i].ActiveRange = EditorGUILayout.FloatField(_pcgEditorProps.PcgEditorBuildings.PrefabsBuildings[i].ActiveRange);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        // -------- BUILDING VARIANTS ON AREA TYPE -------- \\
        GUILayout.Space(15f);

        _pcgEditorProps.PcgEditorBuildings.AreasGroupFoldout =
            EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorBuildings.AreasGroupFoldout, "Building area");
        if (_pcgEditorProps.PcgEditorBuildings.AreasGroupFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);

            EditorGUILayout.LabelField("Number");
            _pcgEditorProps.PcgEditorBuildings.AreasNumber =
                EditorGUILayout.IntField(_pcgEditorProps.PcgEditorBuildings.AreasNumber);

            if (_pcgEditorProps.PcgEditorBuildings.AreasProperties != null)
            {
                if (_pcgEditorProps.PcgEditorBuildings.AreasNumber > _pcgEditorProps.PcgEditorBuildings.AreasProperties.Count)
                {
                    for (var i = 0; i < _pcgEditorProps.PcgEditorBuildings.AreasNumber - _pcgEditorProps.PcgEditorBuildings.AreasProperties.Count; i++)
                    {
                        _pcgEditorProps.PcgEditorBuildings.AreasProperties.Add(new BuildingsAreasEditor());
                    }
                }
                else if (_pcgEditorProps.PcgEditorBuildings.AreasNumber < _pcgEditorProps.PcgEditorBuildings.AreasProperties.Count)
                {
                    _pcgEditorProps.PcgEditorBuildings.AreasProperties
                        .RemoveRange(_pcgEditorProps.PcgEditorBuildings.AreasNumber, _pcgEditorProps.PcgEditorBuildings.AreasProperties.Count - _pcgEditorProps.PcgEditorBuildings.AreasNumber);
                }
            }
            else
            {
                _pcgEditorProps.PcgEditorBuildings.AreasProperties = new List<BuildingsAreasEditor>();
            }
            EditorGUILayout.EndHorizontal();

            if (_pcgEditorProps.PcgEditorBuildings.AreasNumber > 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                _pcgEditorProps.PcgEditorBuildings.AreasListFoldout =
                    EditorGUILayout.Foldout(_pcgEditorProps.PcgEditorBuildings.AreasListFoldout,
                        new GUIContent("Prefabs", "GameObject | Area name | Range => " +
                                                  "The game object which you want to be placed in the scene up to the specified range based on a specific area name"));

                if (_pcgEditorProps.PcgEditorBuildings.AreasListFoldout)
                {
                    GUILayout.Space(PCGEditorStyling.SubCompLeftSpacing);
                    EditorGUILayout.BeginVertical();

                    for (var i = 0; i < _pcgEditorProps.PcgEditorBuildings.AreasProperties.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        _pcgEditorProps.PcgEditorBuildings.AreasProperties[i].Prefab = EditorGUILayout
                            .ObjectField(_pcgEditorProps.PcgEditorBuildings.AreasProperties[i].Prefab, typeof(GameObject), true) as GameObject;
                        _pcgEditorProps.PcgEditorBuildings.AreasProperties[i].AreaName = EditorGUILayout.TextField(_pcgEditorProps.PcgEditorBuildings.AreasProperties[i].AreaName);
                        _pcgEditorProps.PcgEditorBuildings.AreasProperties[i].AreaRange = EditorGUILayout.FloatField(_pcgEditorProps.PcgEditorBuildings.AreasProperties[i].AreaRange);
                        if (GUILayout.Button("Create"))
                        {
                            _pcgEditorProps.PcgEditorBuildings.AreasProperties[i].Gizmo = new GameObject(_pcgEditorProps.PcgEditorBuildings.AreasProperties[i].AreaName);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10f);
        if (GUILayout.Button("Run Building Placement"))
        {
            _manager.GetComponent<BuildingGenerator>().BuildingPlacementEventListener();
        }
        if (GUILayout.Button("Clear Buildings"))
        {
            _manager.GetComponent<BuildingGenerator>().ClearBuildings();
        }
    }

    private void SaveGeneratedMap()
    {
        if (GUILayout.Button("Save Generated Map"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/SavedData"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "SavedData");
            }

            var savedPath = EditorUtility.SaveFilePanel(
                "Save data into folder",
                "Assets/Resources/SavedData",
                "SaveFolder",
                "");

            if (savedPath.Length != 0)
            {
                if (!AssetDatabase.IsValidFolder(savedPath))
                {
                    AssetDatabase.CreateFolder("Assets/Resources/SavedData", savedPath.Split('/').LastOrDefault());
                }

                var roadParent = GameObject.Find("Roads");
                if (roadParent != null)
                {
                    var newPrefab = Instantiate(roadParent);
                    newPrefab.tag = "GeneratedRoadData";
                    PrefabUtility.SaveAsPrefabAsset(newPrefab, savedPath + "/RoadGeneratedData.prefab");
                }

                var buildingParent = GameObject.Find("Buildings");
                if (buildingParent != null)
                {
                    buildingParent.tag = "GeneratedBuildingData";
                    PrefabUtility.SaveAsPrefabAsset(buildingParent, savedPath + "/BuildingGeneratedData.prefab");
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        RenderGui();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_pcgEditorProps);
        }
    }

    private void RenderGui()
    {
        _pcgEditorProps.WindowScrollViewPos = 
            GUILayout.BeginScrollView(_pcgEditorProps.WindowScrollViewPos, false, false);

        PCGEditorStyling.InitStyle();

        GUILayout.Space(20f);

        RoadSettingsGui();

        GUILayout.Space(30f);

        BuildingSettingsGui();

        GUILayout.Space(30f);

        SaveGeneratedMap();

        GUILayout.Space(30f);
        GUILayout.EndScrollView();

        SaveDataInComponent();
    }

    private void SaveDataInComponent()
    {
        _manager.GetComponent<RoadGenerator>().PcgEditorRoads =
            _pcgEditorProps.PcgEditorRoads;


        var buildingGenerator = _manager.GetComponent<BuildingGenerator>();

        buildingGenerator.PcgEditorBuildings =
            _pcgEditorProps.PcgEditorBuildings.PrefabsBuildings;
        buildingGenerator.PcgEditorAreas =
            _pcgEditorProps.PcgEditorBuildings.AreasProperties;
    }
}
