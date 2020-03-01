using Assets.Scripts.PCGEditor;
using UnityEngine;

public class PCGEditorProps : ScriptableObject
{
    [SerializeField] [HideInInspector] public PCGEditorRoads PcgEditorRoads;
    [SerializeField] [HideInInspector] public PCGEditorBuildings PcgEditorBuildings;
}
