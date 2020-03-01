using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PCGEditorLoader
{
    public static void PcgEditorManagerLoader()
    {
        Debug.Log("[PCGToolEditor]: Starting up!");

        if (AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            Debug.Log("[PCGToolEditor]: Resource folder already exists!");
        }
        else
        {
            Debug.Log("[PCGToolEditor]: Creating resource folder...");
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        var toolConfig = Resources.Load<PCGEditorProps>("PCGToolConfig");

        if (toolConfig == null)
        {
            Debug.Log("[PCGToolEditor]: Creating new configuration!");
            toolConfig = ScriptableObject.CreateInstance<PCGEditorProps>();
            AssetDatabase.CreateAsset(toolConfig, "Assets/Resources/PCGToolConfig.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("[PCGToolEditor]: Config already exists!");
        }
    }
}
