using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Pattern))]
public class PatternPropertyDrawer : PropertyDrawer
{
    private SpecialFieldType selectedType = SpecialFieldType.NONE;


    public static void ChangeGridSize(PatternSO patternSO, bool reset = false)
    {
        patternSO.Pattern.LoadGrid(reset);
        EditorUtility.SetDirty(patternSO);
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var patternSO = property.serializedObject.targetObject as PatternSO;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size");
        int newGridSize = EditorGUILayout.IntField(patternSO.Pattern.GridSize);
        GUILayout.EndHorizontal();
        if (newGridSize <= 0)
        {
            GUI.color = Color.red;
            GUILayout.Label("GRID SIZE MUST BE BIGGER THAN 0");
            return;
        }

        if (newGridSize != patternSO.Pattern.GridSize)
        {
            patternSO.Pattern.GridSize = newGridSize;
        }

        property.serializedObject.ApplyModifiedProperties();
        bool changed = false;
        if (GUILayout.Button("Apply Grid Size") || patternSO.Pattern.Data == null)
        {
            ChangeGridSize(patternSO);
            changed = true;
        }

        GUI.color = Color.green;
        if (GUILayout.Button("Save Asset (So it Shows in Git)"))
        {
            AssetDatabase.SaveAssetIfDirty(patternSO);
        }

        GUI.color = Color.red;
        if (GUILayout.Button("Reset Grid"))
        {
            ChangeGridSize(patternSO, true);
            changed = true;
        }

        GUI.color = Color.clear;


        if (patternSO.Pattern.Data == null) return;
        foreach (var item in patternSO.Pattern.PaletteDictionary)
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

        GUI.color = patternSO.Pattern.PaletteDictionary[selectedType];

        EditorGUILayout.LabelField($"Selected Type: {selectedType.ToString()}");

        for (int y = patternSO.Pattern.Data.GetLength(1) - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < patternSO.Pattern.Data.GetLength(0); x++)
            {
                var cell = patternSO.Pattern.Data[x, y];
                if (cell != null)
                {
                    // choose color according to the dictionary
                    if (patternSO.Pattern.PaletteDictionary.ContainsKey(cell.specialFieldType))
                    {
                        GUI.color = patternSO.Pattern.PaletteDictionary[cell.specialFieldType];
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
                            patternSO.Pattern.ToggleCell(x, y, SpecialFieldType.NONE);
                        }
                        else
                        {
                            //Left button

                            patternSO.Pattern.ToggleCell(x, y, selectedType);
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
            patternSO.Pattern.SaveDataString();
            Undo.RecordObject(patternSO, "Changed SpecialFields");
        }


        EditorGUI.EndProperty();
    }
}