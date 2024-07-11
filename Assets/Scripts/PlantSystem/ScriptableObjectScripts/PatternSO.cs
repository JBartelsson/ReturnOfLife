using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "ScriptableObjects/PlantSystem/Pattern")]
public class PatternSO : ScriptableObject
{
    [SerializeField] private Pattern pattern;

    public Pattern Pattern
    {
        get => pattern;
        set => pattern = value;
    }

    public void ForEachNormalFieldRelative(Action<Pattern.Field, int, int> function)
    {
        Pattern.Field centerField = null;
        for (int x = 0; x < pattern.Data.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.Data.GetLength(1); y++)
            {
                if (pattern.Data[x, y].specialFieldType == SpecialFieldType.CENTER)
                {
                    centerField = pattern.Data[x, y];
                }
            }
        }

        if (centerField == null)
        {
            Debug.LogError($"NO CENTER FIELD SET ON {name}");
            return;
        }
        for (int x = 0; x < pattern.Data.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.Data.GetLength(1); y++)
            {
                Pattern.Field currentField = pattern.Data[x, y];
                if (currentField.specialFieldType == SpecialFieldType.NORMAL_FIELD)
                {
                    int relativeX = currentField.index.X - centerField.index.X;
                    int relativeY = currentField.index.Y - centerField.index.Y;
                    function(currentField, relativeX, relativeY);
                }
            }
        }
    }

    public bool IsTileInPattern(GridTile centerTile, GridTile checkingTile)
    {
        int incomingRelativeX = checkingTile.X - centerTile.X;
        int incomingRelativeY = checkingTile.Y - centerTile.Y;
        bool isInPattern = false;
        ForEachNormalFieldRelative(((field, relativeX, relativeY) =>
        {
            if (incomingRelativeX == relativeX && incomingRelativeY == relativeY)
                isInPattern = true;
        } ));
        return isInPattern;
    }

    private void Awake()
    {
        pattern.LoadGrid();
    }
}
