using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpecialFieldsLayoutSO), true)]
public class SpecialFieldsEditor : Editor
{
    
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        string[] assetNames = AssetDatabase.FindAssets("t:" + typeof(SpecialFieldsLayoutSO).Name, new[] { "Assets/ScriptableObject" });
        foreach (string SOName in assetNames)
        {
            var SOpath    = AssetDatabase.GUIDToAssetPath(SOName);
            var layout = AssetDatabase.LoadAssetAtPath<SpecialFieldsLayoutSO>(SOpath);
            layout.LoadDataString();
        }
    }
    public override void OnInspectorGUI()
    {
        var grid = (SpecialFieldsLayoutSO)target;
        bool isPositions = target.GetType() == typeof(SpecialFieldsPositionsSO);
        Debug.Log("");

        EditorGUILayout.BeginVertical();

        bool changed = false;
        for (int y = grid.Data.GetLength(1) - 1; y >= 0 ; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0 ; x < grid.Data.GetLength(0); x++)
            {
                var cell = grid.Data[x, y];

                // hard-coded some cheesy color map - improve it by all means!
                GUI.color = Color.gray;
                Debug.Log($"{x}, {y}");
                if (!cell.selected) GUI.color = Color.gray;
                if (cell.selected)
                {
                    GUI.color = Color.green;
                    if (x == grid.CenterCellX && y == grid.CenterCellY && !isPositions) GUI.color = Color.red;
                }

                if (GUILayout.Button($"", GUILayout.Width(20)))
                {
                    if (Event.current.button == 1)
                    {
                        if (isPositions) return;
                        // Right button action.
                        if (!grid.Data[x, y].selected)
                            grid.ToggleCell(x, y);

                        if (grid.Data[x, y].selected)
                        {
                            grid.CenterCellX = x;
                            grid.CenterCellY = y;
                        }
                        if (!grid.Data[x, y].selected)
                        {
                            grid.CenterCellX = -1;
                            grid.CenterCellY = -1;
                        }
                    }
                    else
                    {
                        grid.ToggleCell(x, y);
                        if (!grid.Data[x, y].selected && x == grid.CenterCellX && y == grid.CenterCellY)
                        {
                            grid.CenterCellX = -1;
                            grid.CenterCellY = -1;
                        }
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
    }
}