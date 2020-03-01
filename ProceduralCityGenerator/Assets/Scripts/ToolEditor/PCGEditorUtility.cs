using Assets.Scripts.PCGEditor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.PCGEditor
{
    public static class PCGEditorUtility
    {
        public static void InputLabel(string labelName)
        {
            PCGEditorStyling.ResetStyles();
            EditorGUILayout.LabelField(labelName, PCGEditorStyling.GetCurrentStyle(), new []{GUILayout.Width(110)});
        }

        public static void HeadlineLabel(string labelName)
        {
            PCGEditorStyling.HeadlineStyles();
            EditorGUILayout.LabelField(labelName, PCGEditorStyling.GetCurrentStyle());
            PCGEditorStyling.ResetStyles();
        }
    }
}
