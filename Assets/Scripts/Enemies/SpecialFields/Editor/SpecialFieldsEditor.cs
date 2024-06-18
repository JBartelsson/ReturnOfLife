using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpecialFieldsLayoutSO), true)]
public class SpecialFieldsEditor : Editor
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        string[] assetNames = AssetDatabase.FindAssets("t:" + typeof(SpecialFieldsLayoutSO).Name,
            new[] { "Assets/ScriptableObject" });
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var layout = AssetDatabase.LoadAssetAtPath<SpecialFieldsLayoutSO>(SOpath);
            ChangeGridSize(layout);
            layout.LoadDataString();
        }
    }

    private static void ChangeGridSize(SpecialFieldsLayoutSO specialFieldsLayoutSO)
    {
        int gridSize = specialFieldsLayoutSO.GridSize;
        if (gridSize <= 0) return;
        if (specialFieldsLayoutSO.Data == null || specialFieldsLayoutSO.Data.Length != (gridSize * gridSize))
        {
            specialFieldsLayoutSO.Data = new SpecialFieldsLayoutSO.Field[gridSize, gridSize];
            string newDataString = "";
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    specialFieldsLayoutSO.Data[x, y] =
                        new SpecialFieldsLayoutSO.Field(new SpecialFieldsLayoutSO.Index(x, y));
                    newDataString += "0";
                }
            }

            specialFieldsLayoutSO.DataString = newDataString;
            EditorUtility.SetDirty(specialFieldsLayoutSO);
        }
    }

    private SpecialFieldType selectedType = SpecialFieldType.NONE;
    private readonly Dictionary<SpecialFieldType, Color> paletteDictionary = new Dictionary<SpecialFieldType, Color>()
    {
        { SpecialFieldType.NONE , Color.black},
        { SpecialFieldType.NORMAL_FIELD , Color.gray},
        { SpecialFieldType.MULTIPLY , Color.red},
    };
    
    public override void OnInspectorGUI()
    {
        SerializedProperty gridSizeProperty = serializedObject.FindProperty("gridSize");
        EditorGUILayout.PropertyField(gridSizeProperty);
        var grid = (SpecialFieldsLayoutSO)target;
        bool isPositions = target.GetType() == typeof(LevelLayoutSO);
        int newGridSize = gridSizeProperty.intValue;
        if (GUILayout.Button("Apply"))
        {
            ChangeGridSize(grid);
        }


        EditorGUILayout.BeginVertical();

        bool changed = false;
        if (grid.Data == null) return;
        foreach (var item in paletteDictionary)
        {
            GUI.color = Color.clear;
            GUI.contentColor = Color.black;
            GUILayout.TextField(item.Key.ToString() + ":");
            GUI.color = item.Value;
            
            if (GUILayout.Button("", GUILayout.Width(20)))
            {
                selectedType = item.Key;
            }
        }
        for (int y = grid.Data.GetLength(1) - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < grid.Data.GetLength(0); x++)
            {
                var cell = grid.Data[x, y];
                
                // choose color according to the dictionary
                if (paletteDictionary.ContainsKey(cell.specialFieldType))
                {
                    GUI.color = paletteDictionary[cell.specialFieldType];
                }
                else
                {
                    GUI.color = Color.magenta;
                }
                if (GUILayout.Button($"", GUILayout.Width(20)))
                {
                    if (Event.current.button == 1)
                    {
                        //Right button
                        grid.ToggleCell(x, y, SpecialFieldType.NONE);
                    }
                    else
                    {
                        //Left button

                        grid.ToggleCell(x, y, selectedType);
                    }


                    changed = true;
                }
            }


            GUILayout.EndHorizontal();
        }

        if (isPositions)
        {
            grid.CenterCellX = Mathf.FloorToInt(grid.Data.GetLength(0) / 2f);
            grid.CenterCellY = Mathf.FloorToInt(grid.Data.GetLength(1) / 2f);
        }

        if (changed)
        {
            grid.SaveDataString();
            EditorUtility.SetDirty(grid);
            Undo.RecordObject(grid, "Changed SpecialFields");
        }

        if (grid.CenterCellX == -1 && !isPositions)
        {
            GUI.contentColor = new Color(90, 0, 0);
            GUILayout.Label("WARNING: No Center Point set! (Use Rightclick!)");
        }

        GUI.color = Color.yellow;


        GUI.color = Color.white;

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}