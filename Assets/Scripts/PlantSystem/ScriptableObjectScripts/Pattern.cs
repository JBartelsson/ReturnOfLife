using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Pattern
{
    [Serializable]
    public class Field
    {
        public Field(Index index, SpecialFieldType specialFieldType = SpecialFieldType.NORMAL_FIELD)
        {
            this.specialFieldType = specialFieldType;
            this.index = index;
        }

        public SpecialFieldType specialFieldType = SpecialFieldType.NORMAL_FIELD;
        public Index index;
    }

    [SerializeField] private Field[,] data;
    [SerializeField] private string dataString = "";

    [SerializeField] private int gridSize = 7;

    private Dictionary<SpecialFieldType, Color> paletteDictionary;

    public Dictionary<SpecialFieldType, Color> PaletteDictionary
    {
        get => paletteDictionary;
        set => paletteDictionary = value;
    }

    public static char ConvertFieldToSingleString(SpecialFieldType fieldType)
    {
        //Convert Special Field to Hex for Saving

        string fieldString = fieldType.ToString("X");
        //Get Last character of hex conversion to delete leading zeros
        return fieldString[^1];
    }

    public void SaveDataString()
    {
        string localDataString = "";
        for (int y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                localDataString += Pattern.ConvertFieldToSingleString(data[x, y].specialFieldType).ToString();
            }
        }

        dataString = localDataString;
        Debug.Log($"Data String saved: {dataString}");
    }

    public bool IndexViable(int x, int y)
    {
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= gridSize) return false;
        if (y >= gridSize) return false;
        return true;
    }

    // public List<Index> GetRandomlyMirroredSelectedFields()
    // {
    //     List<Index> result = new();
    //     Index origin = new Index(centerCellX, centerCellY);
    //     //Apply Random mirroring
    //     int mirroring = Random.Range(0, 4);
    //     for (int x = 0; x < data.GetLength(0); x++)
    //     {
    //         for (int y = 0; y < data.GetLength(1); y++)
    //         {
    //             if (data[x, y].selected)
    //             {
    //                 Index relativeIndex = new Index(x, y) - origin;
    //                 Index transformedIndex;
    //                 switch (mirroring)
    //                 {
    //                     // No mirroring
    //                     case 0:
    //                         transformedIndex = new Index(relativeIndex.X, relativeIndex.Y);
    //                         break;
    //                     // X Mirroring
    //                     case 1:
    //                         transformedIndex = new Index(-relativeIndex.X, relativeIndex.Y);
    //                         break;
    //                     //Y Mirroring
    //                     case 2:
    //                         transformedIndex = new Index(relativeIndex.X, -relativeIndex.Y);
    //                         break;
    //                     //X and Y Mirroring
    //                     case 3:
    //                         transformedIndex = new Index(-relativeIndex.X, -relativeIndex.Y);
    //                         break;
    //                     default:
    //                         transformedIndex = new Index(0, 0);
    //                         break;
    //                 }
    //                 result.Add(transformedIndex + origin);
    //             }
    //         }
    //     }
    //
    //     return result;
    // }


    public void ToggleCell(int x, int y, SpecialFieldType newType)
    {
        if (!IndexViable(x, y)) return;

        data[x, y].specialFieldType = newType;
    }

    public string DataString
    {
        get => dataString;
        set => dataString = value;
    }

    public int GridSize
    {
        get => gridSize;
        set => gridSize = value;
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

    // for you to get stuff out of the grid to use in your game
    public Field GetCellState(int x, int y)
    {
        return data[x, y];
    }


    public void LoadDataString()
    {
        int localX = 0;
        int localY = 0;
        int oldGridSize = Mathf.FloorToInt(Mathf.Sqrt(dataString.Length));
        // if (oldGridSize != gridSize) LevelSOEditor.ChangeGridSize(this, true);
        foreach (char c in dataString)
        {
            data[localX, localY] =
                new Field(new Index(localX, localY), (SpecialFieldType)Convert.ToInt32(c.ToString(), 16));
            localX++;
            if (localX > gridSize - 1)
            {
                localX = 0;
                localY++;
            }
        }
    }

    public void LoadGrid(bool reset = false)
    {
        if (gridSize <= 0) return;
        if (Data == null || Data.Length != (gridSize * gridSize) || reset)
        {
            Data = new Field[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Data[x, y] =
                        new Field(new Index(x, y));
                }
            }

            if (!reset)
            {
                LoadDataString();
            }
            else
            {
                SaveDataString();
            }
        }
    }
}