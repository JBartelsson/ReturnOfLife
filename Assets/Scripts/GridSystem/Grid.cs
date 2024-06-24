using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using System.Linq;
using System.Net;
using log4net.Core;

public class Grid
{
    private int width;
    private int height;
    private LevelSO currentLevelSO;
    private float cellSize;
    private GridTile[,] gridArray;
    private Vector3 originPosition;
    private List<CardInstance> plantInstances = new();

    public List<CardInstance> PlantInstances
    {
        get => plantInstances;
        set => plantInstances = value;
    }

    private List<SpecialField> specialFields = new();

    public List<SpecialField> SpecialFields
    {
        get => specialFields;
        set => specialFields = value;
    }

    public class SpecialField
    {
        public SpecialFieldType FieldType;
        public List<GridTile> SpecialFieldGridTiles = new();

        public SpecialField(SpecialFieldType fieldType)
        {
            FieldType = fieldType;
        }

        public bool IsFulfilled()
        {
            bool fulfilled = true;
            foreach (GridTile gridTile in SpecialFieldGridTiles)
            {
                if (!gridTile.ContainsPlant())
                {
                    fulfilled = false;
                    break;
                }
            }

            return fulfilled;
        }
    }

    public float CellSize
    {
        get => cellSize;
        set => cellSize = value;
    }

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public event EventHandler<OnGridChangedEventArgs> OnGridTileChanged;

    public class OnGridChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
        public GridTile gridObject;
    }


    public Grid(LevelSO levelSO, float cellSize, Vector3 originPosition,
        Func<Grid, int, int, GridTile> createGridObject = null)
    {
        this.currentLevelSO = levelSO;
        this.width = levelSO.GridSize;
        this.height = levelSO.GridSize;
        this.cellSize = cellSize;
        this.originPosition = originPosition - new Vector3(width * cellSize * .5f, 0, height * cellSize * .5f);
        gridArray = new GridTile[width, height];
        if (createGridObject != null)
        {
            InitGrid(createGridObject);
        }
    }

    public void InitGrid(Func<Grid, int, int, GridTile> createGridObject)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
                gridArray[x, y].ChangeFieldType(currentLevelSO.Data[x, y].specialFieldType, true);
            }
        }

        ApplyNeighbors();
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Plane gridPlane = new Plane(originPosition, originPosition + Vector3.left, originPosition + Vector3.forward);
        Debug.DrawLine(worldPosition, worldPosition + Vector3.up, Color.green, 100f);
        Ray clickRay = Camera.main.ScreenPointToRay(worldPosition);
        gridPlane.Raycast(clickRay, out float enter);
        Vector3 intersectionPoint = clickRay.origin + clickRay.direction.normalized * enter;
        Debug.DrawRay(clickRay.origin, clickRay.direction * 100f, Color.yellow, 100f);
        Debug.DrawLine(intersectionPoint, originPosition, Color.red, 100f);
        x = Mathf.FloorToInt((intersectionPoint - originPosition).x / cellSize);
        y = Mathf.FloorToInt((intersectionPoint - originPosition).z / cellSize);
    }

    public void ForEachGridTile(Action<GridTile> Action)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Action(gridArray[x, y]);
            }
        }
    }

    public void ApplyNeighbors()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                //If not on right edge, set right Neighbor
                if (x >= 0 && x < Width - 1)
                {
                    GetGridObject(x, y).RightNeighbor = GetGridObject(x + 1, y);
                }
                //If not on left edge, set left Neighbor

                if (x > 0 && x < Width)
                {
                    GetGridObject(x, y).LeftNeighbor = GetGridObject(x - 1, y);
                }
                //If not on top edge, set top Neighbor

                if (y >= 0 && y < Height - 1)
                {
                    GetGridObject(x, y).TopNeighbor = GetGridObject(x, y + 1);
                }
                //If not on bottom edge, set top Neighbor

                if (y > 0 && y < Height)
                {
                    GetGridObject(x, y).BottomNeighbor = GetGridObject(x, y - 1);
                }
            }
        }
    }

    public void SetGridObject(int x, int y, GridTile value)
    {
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            gridArray[x, y] = value;
            UpdateGridContent(x, y, gridArray[x, y]);
        }
    }

    public void SetGridObject(Vector3 worldPosition, GridTile value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public GridTile GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(GridTile);
        }
    }

    public GridTile GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public void UpdateGridContent(int x, int y, GridTile gridObject)
    {
        OnGridTileChanged?.Invoke(this, new OnGridChangedEventArgs()
        {
            x = x,
            y = y,
            gridObject = gridObject
        });
    }

    public void UpdateWholeGrid()
    {
        ForEachGridTile((gridTile) => { UpdateGridContent(gridTile.X, gridTile.Y, gridTile); });
    }

    private List<SpecialField> FindConnectedFields()
    {
        List<SpecialField> specialFieldsList = new();
        List<GridTile> visitedTiles = new();
        ForEachGridTile((gridTile) =>
        {
            if (visitedTiles.Contains(gridTile))
            {
                return;
            }

            if (gridTile.FieldType == SpecialFieldType.NONE || gridTile.FieldType == SpecialFieldType.NORMAL_FIELD)
            {
                visitedTiles.Add(gridTile);
                return;
            }
            if (gridTile.RightNeighbor != null)
            {
                if (gridTile.RightNeighbor.FieldType == gridTile.FieldType)
                {
                    {
                        SpecialField newField = new(gridTile.FieldType);
                        PopulateFieldGroup(ref newField, ref visitedTiles, gridTile);
                        specialFieldsList.Add(newField);
                        return;
                    }
                }
            }

            if (gridTile.BottomNeighbor != null)
            {
                if (gridTile.BottomNeighbor.FieldType == gridTile.FieldType)
                {
                    {
                        SpecialField newField = new(gridTile.FieldType);
                        PopulateFieldGroup(ref newField, ref visitedTiles, gridTile);
                        specialFieldsList.Add(newField);
                        return;
                    }
                }
            }
        });
        return specialFieldsList;
    }

    private void PopulateFieldGroup(ref SpecialField newSpecialField, ref List<GridTile> visitedTiles,
        GridTile gridTile)
    {
        newSpecialField.SpecialFieldGridTiles.Add(gridTile);
        visitedTiles.Add(gridTile);

        if (gridTile.RightNeighbor != null)
        {
            if (gridTile.RightNeighbor.FieldType == gridTile.FieldType && !visitedTiles.Contains(gridTile.RightNeighbor))
            {
                PopulateFieldGroup(ref newSpecialField, ref visitedTiles, gridTile.RightNeighbor);
            }
        }

        if (gridTile.TopNeighbor != null)
        {
            if (gridTile.TopNeighbor.FieldType == gridTile.FieldType && !visitedTiles.Contains(gridTile.TopNeighbor))
            {
                PopulateFieldGroup(ref newSpecialField, ref visitedTiles, gridTile.TopNeighbor);
            }
        }
        
        if (gridTile.LeftNeighbor != null)
        {
            if (gridTile.LeftNeighbor.FieldType == gridTile.FieldType && !visitedTiles.Contains(gridTile.LeftNeighbor))
            {
                PopulateFieldGroup(ref newSpecialField, ref visitedTiles, gridTile.LeftNeighbor);
            }
        }
        if (gridTile.BottomNeighbor != null)
        {
            if (gridTile.BottomNeighbor.FieldType == gridTile.FieldType && !visitedTiles.Contains(gridTile.BottomNeighbor))
            {
                PopulateFieldGroup(ref newSpecialField, ref visitedTiles, gridTile.BottomNeighbor);
            }
        }
    }

    public void AddSpecialField(LevelSO.Index index, LevelSO.Index offset, SpecialFieldType fieldType,
        EnemiesSO currentEnemy)
    {
        int x = index.X - offset.X;
        int y = index.Y - offset.Y;
        GridTile gridTile = GetGridObject(x, y);
        if (gridTile == null) return;
        //If no Special Field has been rendered, just add it
        if (gridTile.FieldType == SpecialFieldType.NONE)
        {
            gridTile.ChangeFieldType(fieldType);
        }
        else
        {
            //If there are two Special Fields overlapping, check for the priority
            if (currentEnemy.SpecialFieldPriority.Priority.IndexOf(gridTile.FieldType) >
                currentEnemy.SpecialFieldPriority.Priority.IndexOf(fieldType))
            {
                specialFields.First((x) => x.FieldType == gridTile.FieldType).SpecialFieldGridTiles
                    .Remove(gridTile);
                gridTile.ChangeFieldType(fieldType);
            }
        }

        //In the end, still add the new field to the List of SpecialFields
        if (specialFields.Any((x) => x.FieldType == fieldType))
        {
            specialFields.First((x) => x.FieldType == fieldType).SpecialFieldGridTiles.Add(gridTile);
        }
        else
        {
            specialFields.Add(new SpecialField(fieldType)
            {
                SpecialFieldGridTiles = new()
            });
        }
    }

    public void ResetGrid()
    {
        ForEachGridTile((x) => x.Reset());
        SpecialFields.Clear();
        plantInstances.Clear();
    }

    public void BuildGrid(LevelSO levelSO)
    {
    }

//Reset all events so no memory leaks are existing
    public void ResetSubscriptions()
    {
        OnGridTileChanged = null;
        ForEachGridTile((gridTile) => { gridTile.ResetSubscriptions(); });
    }
}