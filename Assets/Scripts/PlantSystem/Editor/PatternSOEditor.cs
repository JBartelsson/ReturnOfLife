using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatternSO), false)]
public class PatternSOEditor : Editor
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        string[] assetNames = AssetDatabase.FindAssets("t:" + typeof(PatternSO).Name,
            new[] { "Assets/ScriptableObject" });
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var layout = AssetDatabase.LoadAssetAtPath<PatternSO>(SOpath);
            PatternPropertyDrawer.ChangeGridSize(layout);
            layout.Pattern.LoadDataString();
        }
    }

    private readonly Dictionary<SpecialFieldType, Color> patternPaletteDictionary =
        new Dictionary<SpecialFieldType, Color>()
        {
            { SpecialFieldType.NONE, Color.black },
            { SpecialFieldType.NORMAL_FIELD, Color.gray },
            { SpecialFieldType.CENTER, Color.green },
        };

#if UNITY_EDITOR
    private void OnEnable()
    {
        PatternSO patternSO = target as PatternSO;
        patternSO.Pattern.PaletteDictionary = patternPaletteDictionary;
    }
#endif
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    public static void GenerateGridEditor(LevelLayoutSO.Field[,] grid, SpecialFieldType selectedType,
        Dictionary<SpecialFieldType, Color> paletteDictionary)
    {
        
    }
}