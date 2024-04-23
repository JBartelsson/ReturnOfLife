using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;

    public float CellSize { get => cellSize; set => cellSize = value; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    public event EventHandler<OnGridChangedEventArgs> OnGridChanged;
    public class OnGridChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
        public TGridObject gridObject;
    }


    public Grid (int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject = null)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new TGridObject[width, height];
        if (createGridObject != null)
        {
            InitGrid(createGridObject);
        }
       
    }

    public void InitGrid(Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
                UpdateGridContent(x, y, gridArray[x, y]);
            }
        }
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void ForEachGridTile(Action<TGridObject> Action)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Action(gridArray[x, y]);
            }
        }
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x<= width && y <= height)
        {
            gridArray[x, y] = value;
            UpdateGridContent(x, y, gridArray[x, y]);
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            return gridArray[x, y];
        } else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public void UpdateGridContent(int x, int y, TGridObject gridObject)
    {
        OnGridChanged?.Invoke(this, new OnGridChangedEventArgs()
        {
            x = x,
            y = y,
            gridObject = gridObject
        });
    }

}
