using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Runtime.CompilerServices;
using System;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Grid<GridTile> _grid;
    private TextMesh[,] debugTextArray;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    public static GridManager Instance { get; private set; }
    public Grid<GridTile> Grid { get => _grid; set => _grid = value; }

    public event EventHandler OnGridReady;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Grid Manager already exists!");
        }
    }
    void Start()
    {
        _grid = new Grid<GridTile>(width, height, 3f, transform.position);
        debugTextArray = new TextMesh[width, height];
        _grid.OnGridChanged += _grid_OnGridChanged;
        _grid.InitGrid((Grid<GridTile> g, int x, int y) => new GridTile(g, x, y ));
        ApplyNeighbors();
        OnGridReady?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyNeighbors()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                //If not on right edge, set right Neighbor
                if (x >= 0 && x < _grid.Width - 1)
                {
                    _grid.GetGridObject(x, y).RightNeighbor = _grid.GetGridObject(x + 1, y);
                }
                //If not on left edge, set left Neighbor

                if (x > 0 && x < _grid.Width)
                {
                    _grid.GetGridObject(x, y).LeftNeighbor = _grid.GetGridObject(x - 1, y);
                }
                //If not on top edge, set top Neighbor

                if (y >= 0 && y < _grid.Height - 1)
                {
                    _grid.GetGridObject(x, y).TopNeighbor = _grid.GetGridObject(x, y + 1);
                }
                //If not on bottom edge, set top Neighbor

                if (y > 0 && y < _grid.Height)
                {
                    _grid.GetGridObject(x, y).BottomNeighbor = _grid.GetGridObject(x, y - 1);
                }
            }
        }
    }

    private void _grid_OnGridChanged(object sender, Grid<GridTile>.OnGridChangedEventArgs e)
    {
        Grid<GridTile> grid = sender as Grid<GridTile>;
        if (debugTextArray[e.x, e.y] == null)
        {
            debugTextArray[e.x, e.y] = UtilsClass.CreateWorldText(e.gridObject.ToString(), null, grid.GetWorldPosition(e.x, e.y) + new Vector3(grid.CellSize, grid.CellSize) * .5f, 15, Color.white, TextAnchor.MiddleCenter);
            Debug.DrawLine(grid.GetWorldPosition(e.x, e.y), grid.GetWorldPosition(e.x, e.y + 1), Color.white, 100f); ;
            Debug.DrawLine(grid.GetWorldPosition(e.x, e.y), grid.GetWorldPosition(e.x + 1, e.y), Color.white, 100f);
        } else
        {
            debugTextArray[e.x, e.y].text = e.gridObject.ToString();
        }
        if (grid.GetGridObject(e.x, e.y).Marked)
        {
            debugTextArray[e.x, e.y].color = Color.red;
        } else
        {
            debugTextArray[e.x, e.y].color = Color.white;

        }
    }

    private void Update()
    {
        
    }

}
