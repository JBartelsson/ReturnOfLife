using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private List<Plantable> content = new();
    private int x;
    private int y;
    private Grid<GridTile> grid;

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

    public void AddPlantable(Plantable plantable)
    {
        if (content.Any((x) => x.type == Plantable.PlantableType.Plant) && plantable.type == Plantable.PlantableType.Plant)
        {
            Debug.LogWarning("Trying to place two plants on another!");
            return;
        }
        content.Add(plantable);
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
}
