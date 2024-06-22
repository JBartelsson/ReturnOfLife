using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSO), true)]
public class LevelSOEditor : Editor
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        string[] assetNames = AssetDatabase.FindAssets("t:" + typeof(LevelSO).Name,
            new[] { "Assets/ScriptableObject" });
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var layout = AssetDatabase.LoadAssetAtPath<LevelSO>(SOpath);
            ChangeGridSize(layout);
            layout.LoadDataString();
        }
    }

    public static void ChangeGridSize(LevelSO levelSo, bool reset = false)
    {
        int gridSize = levelSo.GridSize;
        if (gridSize <= 0) return;
        if (levelSo.Data == null || levelSo.Data.Length != (gridSize * gridSize) || reset)
        {
            levelSo.Data = new LevelSO.Field[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    levelSo.Data[x, y] =
                        new LevelSO.Field(new LevelSO.Index(x, y));
                }
            }

            if (!reset)
            {
                levelSo.LoadDataString();
            }
            else
            {
                levelSo.SaveDataString();
            }

            EditorUtility.SetDirty(levelSo);
        }
    }

    public static char ConvertFieldToSingleString(SpecialFieldType fieldType)
    {
        //Convert Special Field to Hex for Saving

        string fieldString = fieldType.ToString("X");
        //Get Last character of hex conversion to delete leading zeros
        return fieldString[^1];
    }

    private SpecialFieldType selectedType = SpecialFieldType.NONE;

    private static readonly Dictionary<SpecialFieldType, Color> paletteDictionary =
        new Dictionary<SpecialFieldType, Color>()
        {
            { SpecialFieldType.NONE, Color.black },
            { SpecialFieldType.NORMAL_FIELD, Color.gray },
            { SpecialFieldType.MULTIPLY, Color.red },
        };

    public override void OnInspectorGUI()
    {
        var grid = (LevelSO)target;
        int oldGridSize = grid.GridSize;
        SerializedProperty gridSizeProperty = serializedObject.FindProperty("gridSize");
        SerializedProperty ecoProperty = serializedObject.FindProperty("neededECOPoints");
        EditorGUILayout.PropertyField(gridSizeProperty);
        EditorGUILayout.PropertyField(ecoProperty);
        serializedObject.ApplyModifiedProperties();
        bool changed = false;
        if (GUILayout.Button("Apply Grid Size"))
        {
            ChangeGridSize(grid);
            changed = true;
        }

        GUI.color = Color.green;
        if (GUILayout.Button("Save Asset (So it Shows in Git)"))
        {
            AssetDatabase.SaveAssetIfDirty(grid);
        }

        GUI.color = Color.red;
        if (GUILayout.Button("Reset Grid"))
        {
            ChangeGridSize(grid, true);
            changed = true;
        }

        GUI.color = Color.clear;


        if (grid.Data == null) return;

        foreach (var item in paletteDictionary)
        {
            GUI.color = Color.clear;
            GUI.contentColor = Color.black;
            GUI.color = item.Value;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{item.Key.ToString()}");
            if (GUILayout.Button("", GUILayout.Width(200)))
            {
                selectedType = item.Key;
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginVertical();

        GUI.color = paletteDictionary[selectedType];

        EditorGUILayout.LabelField($"Selected Type: {selectedType.ToString()}");

        for (int y = grid.Data.GetLength(1) - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < grid.Data.GetLength(0); x++)
            {
                var cell = grid.Data[x, y];
                if (cell != null)
                {
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
            }


            GUILayout.EndHorizontal();
        }


        GUI.color = Color.yellow;


        GUI.color = Color.white;

        EditorGUILayout.EndVertical();
        if (changed)
        {
            grid.SaveDataString();
            Undo.RecordObject(grid, "Changed SpecialFields");
        }
    }
}