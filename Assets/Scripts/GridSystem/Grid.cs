using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using System.Linq;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private GridTile[,] gridArray;
    private Vector3 originPosition;
    private List<PlantInstance> plantInstances = new();

    public List<PlantInstance> PlantInstances
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

    public float CellSize { get => cellSize; set => cellSize = value; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    public event EventHandler<OnGridChangedEventArgs> OnGridTileChanged;
    public class OnGridChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
        public GridTile gridObject;
    }


    public Grid (int width, int height, float cellSize, Vector3 originPosition, Func<Grid, int, int, GridTile> createGridObject = null)
    {
        this.width = width;
        this.height = height;
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
                UpdateGridContent(x, y, gridArray[x, y]);
            }
        }
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Plane gridPlane = new Plane(originPosition, originPosition + Vector3.left, originPosition + Vector3.forward);
        Debug.DrawLine(worldPosition, worldPosition + Vector3.up, Color.green, 100f);
        Ray clickRay = Camera.main.ScreenPointToRay(worldPosition);
        gridPlane.Raycast(clickRay, out float enter);
        Vector3 intersectionPoint = clickRay.origin + clickRay.direction.normalized * enter;
        Debug.DrawRay(clickRay.origin, clickRay.direction * 100f, Color.yellow, 100f);
        Debug.DrawLine(intersectionPoint, originPosition , Color.red, 100f);
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

    public void SetGridObject(int x, int y, GridTile value)
    {
        if (x >= 0 && y >= 0 && x<= width && y <= height)
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
        } else
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
    
    public void AddSpecialField(SpecialFieldsLayoutSO.Index index, SpecialFieldsLayoutSO.Index offset, SpecialFieldType fieldType, EnemiesSO currentEnemy)
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
                specialFields.First((x) => x.FieldType == gridTile.FieldType).SpecialFieldGridTiles.Remove(gridTile);
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
            specialFields.Add(new SpecialField()
            {
                FieldType = fieldType,
                SpecialFieldGridTiles = new ()
            });
        }
        
    }

    public void ResetGrid()
    {
        ForEachGridTile((x) => x.Reset());
    }

}
