using Assets.Scripts.PCGEditor;
using UnityEngine;

public class PCGEditorProps : ScriptableObject
{
    [SerializeField] [HideInInspector] public Vector2 WindowScrollViewPos;

    [SerializeField] [HideInInspector] public PCGEditorRoads PcgEditorRoads;
    [SerializeField] [HideInInspector] public PCGEditorBuildings PcgEditorBuildings;
}
