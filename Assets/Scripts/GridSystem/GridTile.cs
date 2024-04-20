using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private string content = "None";
    private int x;
    private int y;
    private Grid<GridTile> grid;

    public GridTile(string content, Grid<GridTile> grid, int x, int y)
    {
        this.content = content;
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public GridTile TopNeighbor { get => topNeighbor; set => topNeighbor = value; }
    public GridTile BottomNeighbor { get => bottomNeighbor; set => bottomNeighbor = value; }
    public GridTile LeftNeighbor { get => leftNeighbor; set => leftNeighbor = value; }
    public GridTile RightNeighbor { get => rightNeighbor; set => rightNeighbor = value; }
    public string Content { get => content; set => content = value; }

    public override string ToString()
    {
        return content;
    }

    public void UpdateContent(string newContent)
    {
        content = newContent;
        grid.UpdateGritContent(x, y, this);
    }
}
