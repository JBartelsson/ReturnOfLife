using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpecialFieldsLayout))]
public class SpecialFieldsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var grid = (SpecialFieldsLayout)target;

        EditorGUILayout.BeginVertical();

        GUILayout.Label( "WARNING: Save and commit your prefab/scene OFTEN!");

        for (int y = 0; y < grid.down; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < grid.across; x++)
            {
                int n = grid.GetIndex( x, y);

                var cell = grid.data.Substring( n, 1);

                // hard-coded some cheesy color map - improve it by all means!
                GUI.color = Color.gray;
                if (cell == "1") GUI.color = Color.white;
                if (cell == "2") GUI.color = Color.red;
                if (cell == "3") GUI.color = Color.green;

                if (GUILayout.Button( cell,  GUILayout.Width(20)))
                {
                    grid.ToggleCell(x, y);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.color = Color.yellow;

        GUILayout.Label( "DANGER ZONE BELOW THIS AREA!");

        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

        DrawDefaultInspector();
    }
}
