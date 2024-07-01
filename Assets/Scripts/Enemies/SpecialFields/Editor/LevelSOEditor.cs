using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSO), true)]
public class LevelSOEditor : Editor
{
   

    private readonly Dictionary<SpecialFieldType, Color> levelSOPaletteDictionary =
        new Dictionary<SpecialFieldType, Color>()
        {
            { SpecialFieldType.NONE, Color.black },
            { SpecialFieldType.NORMAL_FIELD, Color.gray },
            { SpecialFieldType.MULTIPLY, Color.red },
        };

    private void OnEnable()
    {
        LevelSO levelSo = target as LevelSO;
        levelSo.Pattern.PaletteDictionary = levelSOPaletteDictionary;
    }

    public override void OnInspectorGUI()
    {
      
        base.OnInspectorGUI();

    }

    public static void GenerateGridEditor(LevelLayoutSO.Field[,] grid, SpecialFieldType selectedType,
        Dictionary<SpecialFieldType, Color> paletteDictionary)
    {
        
    }
}