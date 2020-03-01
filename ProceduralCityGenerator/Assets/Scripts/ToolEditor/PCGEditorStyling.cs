using UnityEngine;

namespace Assets.Scripts.PCGEditor
{
    public static class PCGEditorStyling
    {
        public static float SubCompLeftSpacing = 20f;

        private static GUIStyle _style = new GUIStyle();

        public static void InitStyle()
        {
            _style.normal.textColor = Color.white;
            ResetStyles();
        }

        public static void ResetStyles()
        {
            _style.fontSize = 11;
            _style.fontStyle = FontStyle.Normal;
            _style.alignment = TextAnchor.UpperLeft;
        }

        public static void HeadlineStyles()
        {
            _style.fontSize = 15;
            _style.fontStyle = FontStyle.Bold;
            _style.alignment = TextAnchor.UpperCenter;
        }

        public static GUIStyle GetCurrentStyle()
        {
            return _style;
        }
    }
}

