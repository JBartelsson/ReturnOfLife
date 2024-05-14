using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Plantable;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private List<PlantInstance> content = new();
    private int x;
    private int y;
    private Grid grid;
    private bool marked = false;

    public event EventHandler OnContentUpdated;

    public GridTile(Grid grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public GridTile TopNeighbor
    {
        get => topNeighbor;
        set => topNeighbor = value;
    }

    public GridTile BottomNeighbor
    {
        get => bottomNeighbor;
        set => bottomNeighbor = value;
    }

    public GridTile LeftNeighbor
    {
        get => leftNeighbor;
        set => leftNeighbor = value;
    }

    public GridTile RightNeighbor
    {
        get => rightNeighbor;
        set => rightNeighbor = value;
    }

    public List<PlantInstance> Content
    {
        get => content;
        set => content = value;
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public bool Marked
    {
        get => marked;
        set => marked = value;
    }

    public void AddPlantable(CallerArgs callerArgs)
    {
        if (!IsAccessible(callerArgs)) return;
        content.Add(callerArgs.callingPlantInstance);
        Debug.Log($"CONTENT OF {x}, {y} is {content.Count} long");
        grid.UpdateGridContent(x, y, this);
        OnContentUpdated?.Invoke(this, EventArgs.Empty);
        GameManager.Instance.AddFieldScore(1);
    }

    public void ChangeMarkedStatus(bool status)
    {
        marked = status;
        grid.UpdateGridContent(x, y, this);
    }

    public override string ToString()
    {
        PlantInstance plant = content.FirstOrDefault();
        if (plant != null)
        {
            return plant.DebugVisualization();
        }
        else
        {
            return "-";
        }
    }

    public bool OnSameAxisAs(GridTile other)
    {
        if (this.x == other.x || this.y == other.y)
        {
            return true;
        }

        return false;
    }

    public float DistanceTo(GridTile other)
    {
        float xDistanceSqr = Mathf.Pow(other.x - this.x, 2);
        float yDistanceSqr = Mathf.Pow(other.y - this.y, 2);
        return Mathf.Sqrt(xDistanceSqr + yDistanceSqr);
    }

    public bool ContainsPlant()
    {
        if (this.Content.Count != 0)
        {
            return true;
        }

        return false;
    }

    public void ForEachNeighbor(Action<GridTile> action)
    {
        ForTopAndBottomNeighbor(action);
        ForLeftAndRightNeighbor(action);
    }

    public void ForTopAndBottomNeighbor(Action<GridTile> action)
    {
        if (this.TopNeighbor != null)
            action(this.TopNeighbor);
        if (this.BottomNeighbor != null)
            action(this.BottomNeighbor);
    }

    public void ForLeftAndRightNeighbor(Action<GridTile> action)
    {
        if (this.LeftNeighbor != null)
            action(this.LeftNeighbor);
        if (this.RightNeighbor != null)
            action(this.RightNeighbor);
    }

    public void ForEachAdjacentTile(Action<GridTile> action)
    {
        ForEachNeighbor(action);
        RightNeighbor?.ForTopAndBottomNeighbor(action);
        LeftNeighbor?.ForTopAndBottomNeighbor(action);
    }

    public bool IsAccessible(CallerArgs callerArgs)
    {
        //if one Plant of content is not accessible, then the GridTile can not be used
        bool isAccessible = true;
        content.ForEach((plantInstance) => { isAccessible = isAccessible && plantInstance.IsAccessible(callerArgs); });

        return isAccessible;
    }

    public bool HasNeighboredPlant()
    {
        bool hasNeighboredPlant = false;
        this.ForEachNeighbor((gridTile) =>
        {
            Debug.Log($"{gridTile.x}, {gridTile.y}");
            if (gridTile.ContainsPlant())
            {
                hasNeighboredPlant = true;
            }
        });
        return hasNeighboredPlant;
    }
}