using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SpecialFieldsLayout", menuName = "ScriptableObjects/Enemies/SpecialFieldLayout")]
public class SpecialFieldsLayoutSO : ScriptableObject
{
    [Serializable]
    public class Field
    {
        public Field(Index index, SpecialFieldType specialFieldType = SpecialFieldType.NONE)
        {
            this.specialFieldType = specialFieldType;
            this.index = index;
        }

        [FormerlySerializedAs("marked")] public SpecialFieldType specialFieldType = SpecialFieldType.NONE;
        public Index index;
    }

    [SerializeField] private Field[,] data;


    [SerializeField] private int centerCellX = -1;
    [SerializeField] private int centerCellY = -1;
    [SerializeField] private string dataString = "";

    [SerializeField] private int gridSize = 7;

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
        if (oldGridSize != gridSize) SpecialFieldsEditor.ChangeGridSize(this, true);
        foreach (char c in dataString)
        {
            data[localX, localY] = new Field(new Index(localX, localY), (SpecialFieldType)Convert.ToInt32(c.ToString(), 16));
            localX++;
            if (localX > gridSize - 1)
            {
                localX = 0;
                localY++;
            }
        }
    }

    private void Awake()
    {
        
        LoadDataString();
    }

    public void SaveDataString()
    {
        string localDataString = "";
        for (int y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                
                localDataString += SpecialFieldsEditor.ConvertFieldToSingleString(data[x, y].specialFieldType).ToString();
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

    public Index GetCenterField()
    {
        return new Index(centerCellX, centerCellY);
    }

    public void ToggleCell(int x, int y, SpecialFieldType newType)
    {
        if (!IndexViable(x, y)) return;

        data[x, y].specialFieldType = newType;


#if UNITY_EDITOR
        Undo.RecordObject(this, "Toggle Cell");
#endif
        // reassemble
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}