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

    public static void ChangeGridSize(SpecialFieldsLayoutSO specialFieldsLayoutSO, bool reset = false)
    {
        int gridSize = specialFieldsLayoutSO.GridSize;
        if (gridSize <= 0) return;
        if (specialFieldsLayoutSO.Data == null || specialFieldsLayoutSO.Data.Length != (gridSize * gridSize) || reset)
        {
            specialFieldsLayoutSO.Data = new SpecialFieldsLayoutSO.Field[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    specialFieldsLayoutSO.Data[x, y] =
                        new SpecialFieldsLayoutSO.Field(new SpecialFieldsLayoutSO.Index(x, y));
                }
            }

            if (!reset)
            {
                specialFieldsLayoutSO.LoadDataString();
            }
            else
            {
                specialFieldsLayoutSO.SaveDataString();
            }

            EditorUtility.SetDirty(specialFieldsLayoutSO);
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
        var grid = (SpecialFieldsLayoutSO)target;
        int oldGridSize = grid.GridSize;
        SerializedProperty gridSizeProperty = serializedObject.FindProperty("gridSize");
        EditorGUILayout.PropertyField(gridSizeProperty);
        bool changed = false;
        if (GUILayout.Button("Apply Grid Size"))
        {
            serializedObject.ApplyModifiedProperties();
            ChangeGridSize(grid);
            changed = true;
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