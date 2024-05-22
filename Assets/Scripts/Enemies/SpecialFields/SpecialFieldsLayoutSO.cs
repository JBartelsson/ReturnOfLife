using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpecialFieldsLayout", menuName = "ScriptableObjects/SpecialFieldLayout")]
public class SpecialFieldsLayoutSO : ScriptableObject
{
    [Serializable]
    public class Field
    {
        public Field(Index index, bool selected = false)
        {
            this.selected = selected;
            this.index = index;
        }

        [FormerlySerializedAs("marked")] public bool selected = false;
        public Index index;
    }
    [SerializeField]
    private Field[,] data;

   
    [SerializeField]
    private int centerCellX = -1;
    [SerializeField]

    private int centerCellY = -1;
    [SerializeField]

    private string dataString = "";

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


    private int across = 9;
    private int down = 9;

    // stretch goals for you:
    // TODO: make an array of colors perhaps?
    // TODO: make a color mapper??
    // TODO: map above characters to graphics??

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
        Debug.Log($"Data string on awake: {dataString}");
        OnValidate();
        foreach (char c in dataString)
        {
            data[localX, localY] = new Field(new Index(localX, localY),c.ToString() == "1");
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
        LoadDataString();
    }

    public void SaveDataString()
    {
        string localDataString = "";
        for (int y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                localDataString += data[x, y].selected ? "1" : "0";
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

    public List<Index> GetSelectedFields()
    {
        List<Index> result = new();
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                if (data[x, y].selected)
                {
                    result.Add(new Index(x,y));
                }
            }
        }

        return result;
    }

    public Index GetCenterField()
    {
        return new Index(centerCellX, centerCellY);
    }

    public void ToggleCell(int x, int y)
    {
        if (!IndexViable(x, y)) return;

        bool cellValue = !data[x, y].selected;
        data[x, y].selected = cellValue;


#if UNITY_EDITOR
        Undo.RecordObject(this, "Toggle Cell");
#endif
        // reassemble
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}