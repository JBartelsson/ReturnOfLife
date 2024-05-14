using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Runtime.CompilerServices;
using System;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Grid _grid;
    private GridVisualization[,] gridVisualization;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private GameObject gridSpritePrefab;

    public static GridManager Instance { get; private set; }

    public Grid Grid
    {
        get => _grid;
        set => _grid = value;
    }

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
        _grid = new Grid(width, height, cellSize, transform.position);
        gridVisualization = new GridVisualization[width, height];
        _grid.OnGridTileChanged += GridTileOnGridTileChanged;
        _grid.InitGrid((Grid g, int x, int y) => new GridTile(g, x, y));
        _grid.ForEachGridTile((gridTile) =>
        {
            Vector3 spritePrefabPosition = _grid.GetWorldPosition(gridTile.X, gridTile.Y);
            spritePrefabPosition += new Vector3(cellSize * .5f, 0, cellSize * .5f);
            GameObject spawnedObject =
                Instantiate(gridSpritePrefab, spritePrefabPosition, Quaternion.identity, this.transform);
            spawnedObject.name = $"GridTile: {gridTile.X}, {gridTile.Y}";
            gridVisualization[gridTile.X, gridTile.Y] = spawnedObject.GetComponent<GridVisualization>();
        });
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

    private void GridTileOnGridTileChanged(object sender, Grid.OnGridChangedEventArgs e)
    {
        Grid grid = sender as Grid;
        if (gridVisualization[e.x, e.y] == null) return;
        if (e.gridObject.Content.Count > 0)
            gridVisualization[e.x, e.y].SetNewSprite(e.gridObject.Content[0]);

        //mark grid Tile if its used for an editor
        gridVisualization[e.x, e.y].SetMarkedState(e.gridObject.Marked);
    }
}