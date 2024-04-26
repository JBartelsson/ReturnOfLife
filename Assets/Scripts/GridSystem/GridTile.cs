using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Plantable;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private List<Plantable> content = new();
    private int x;
    private int y;
    private Grid<GridTile> grid;
    private bool marked = false;

    public event EventHandler OnContentUpdated;

    public GridTile(Grid<GridTile> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public GridTile TopNeighbor { get => topNeighbor; set => topNeighbor = value; }
    public GridTile BottomNeighbor { get => bottomNeighbor; set => bottomNeighbor = value; }
    public GridTile LeftNeighbor { get => leftNeighbor; set => leftNeighbor = value; }
    public GridTile RightNeighbor { get => rightNeighbor; set => rightNeighbor = value; }
    public List<Plantable> Content { get => content; set => content = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public bool Marked { get => marked; set => marked = value; }

    public void AddPlantable(CallerArgs callerArgs)
    {
        if (!IsAccessible(callerArgs)) return;
        content.Add(callerArgs.callingPlantable);
        OnContentUpdated?.Invoke(this, EventArgs.Empty);
        grid.UpdateGridContent(x, y, this);
    }

    public void ChangeMarkedStatus(bool status)
    {
        marked = status;
        grid.UpdateGridContent(x, y, this);
    }

    public override string ToString()
    {
         Plantable plant = content.FirstOrDefault((x) => x.type == Plantable.PlantableType.Plant);
        if (plant != null)
        {
            return plant.visualization;
        } else
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
        if (this.Content.Any((x) => x.type == PlantableType.Plant))
        {
            return true;
        }
        return false;
    }

    public void ForEachNeighbor(Action<GridTile> action)
    {
        if (this.TopNeighbor != null)
            action(this.TopNeighbor);
        if (this.BottomNeighbor != null)
            action(this.BottomNeighbor);
        if (this.LeftNeighbor != null)
            action(this.LeftNeighbor);
        if (this.RightNeighbor != null)
            action(this.RightNeighbor);

    }

    public bool IsAccessible(CallerArgs callerArgs)
    {
        //if one Plant of content is not accessible, then the GridTile can not be used
        bool isAccessible = true;
        Debug.Log(isAccessible);
        content.ForEach((x) => {
            isAccessible = isAccessible && x.PlantAccessCheck.IsAccessible(callerArgs);
        });
        Debug.Log(isAccessible);

        return isAccessible;
    }

    public bool HasNeighboredPlant()
    {
        bool hasNeighboredPlant = false;
        this.ForEachNeighbor((x) =>
        {
            if (x.ContainsPlant())
            {
                hasNeighboredPlant = true;
            }
        });
        return hasNeighboredPlant;
    }
}
