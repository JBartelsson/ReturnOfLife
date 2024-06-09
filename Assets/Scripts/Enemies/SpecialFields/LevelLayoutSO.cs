using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SpecialFieldsPositions", menuName = "ScriptableObjects/Enemies/SpecialFieldsPositions")]
public class LevelLayoutSO : ScriptableObject
{
    public enum FieldType
    {
        None, Field, SpecialField
    }
    [Serializable]
    public class Field
    {
        public Field(Index index, FieldType fieldType)
        {
            this.FieldType = fieldType;
            this.Index = index;
        }

        public FieldType FieldType;
        public Index Index;
    }

    [SerializeField] private Field[,] data;


    [SerializeField] private int centerCellX = -1;
    [SerializeField] private int centerCellY = -1;
    [SerializeField] private string dataString = "";

    public int CenterCellX
    {
        get => centerCellX;
        set => centerCellX = value;
    }

    public int CenterCellY
    {
        get => centerCellY;
        set => centerCellY = value;
    }

    public Field[,] Data
    {
        get => data;
        set => data = value;
    }


    public class Index
    {
        public Index(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X = 0;
        public int Y = 0;

        public static Index operator +(Index a, Index b)
            => new Index(a.X + b.X, a.Y + b.Y);

        public static Index operator -(Index a, Index b)
            => new Index(a.X - b.X, a.Y - b.Y);

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

[Header("Basic Layout")]
    [SerializeField] private int across = 9;
    [SerializeField] private int down = 9;

    // for you to get stuff out of the grid to use in your game
    public Field GetCellState(int x, int y)
    {
        return data[x, y];
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (data == null || data.Length != (across * down))
        {
            data = new Field[across, down];
            string newDataString = "";
            for (int i = 0; i < across * down; i++)
            {
                newDataString += "0";
            }

            dataString = newDataString;
            LoadDataString();
            EditorUtility.SetDirty(this);
        }
    }

    void Reset()
    {
        OnValidate();
    }
    
    
#endif
    public void LoadDataString()
    {
        int localX = 0;
        int localY = 0;
        OnValidate();
        foreach (char c in dataString)
        {
            data[localX, localY] = new Field(new Index(localX, localY), (FieldType)Mathf.RoundToInt((float)char.GetNumericValue(c)));
            localX++;
            if (localX > across - 1)
            {
                localX = 0;
                localY++;
            }
        }
    }

    private void Awake()
    {
        OnValidate();
        LoadDataString();
    }

    public void SaveDataString()
    {
        string localDataString = "";
        for (int y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                localDataString += data[x, y].FieldType;
            }
        }

        dataString = localDataString;
        Debug.Log($"Data String saved: {dataString}");
    }

    public bool IndexViable(int x, int y)
    {
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= across) return false;
        if (y >= down) return false;
        return true;
    }

    public Index GetCenterField()
    {
        return new Index(centerCellX, centerCellY);
    }

    public void ToggleCell(int x, int y, FieldType fieldType)
    {
        if (!IndexViable(x, y)) return;

        data[x, y].FieldType = fieldType;

    }
    
}