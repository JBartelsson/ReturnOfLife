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
    private float cellSize = 1f;
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

    public void BuildGrid(LevelSO levelSO)
    {
        Debug.Log($"Trying to build grid with {levelSO?.name}");
        InitGrid(levelSO);
    }

    private void InitGrid(LevelSO levelSO)
    {
        //Create a new Grid, unsubscribing to previous events must happen at some time
        if (_grid != null) _grid.ResetSubscriptions();
        Debug.Log($"GRID{levelSO == null}");
        _grid = new Grid(levelSO, cellSize, transform.position);
        
        //Delete all previous GridVisualizations
        if (gridVisualization != null)
        {
            for (int x = 0; x < gridVisualization.GetLength(0); x++)
            {
                for (int y = 0; y < gridVisualization.GetLength(0); y++)
                {
                    Destroy(gridVisualization[x, y].gameObject);
                }
            }
        }
        gridVisualization = new GridVisualization[levelSO.Pattern.GridSize, levelSO.Pattern.GridSize];
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
        _grid.UpdateWholeGrid();
        OnGridReady?.Invoke(this, EventArgs.Empty);
    }

    

    private void GridTileOnGridTileChanged(object sender, Grid.OnGridChangedEventArgs e)
    {
        Grid grid = sender as Grid;
        if (gridVisualization[e.x, e.y] == null) return;
        gridVisualization[e.x, e.y].UpdateContent(e.gridObject);
       
    }
}