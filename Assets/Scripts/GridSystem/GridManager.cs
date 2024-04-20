using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Runtime.CompilerServices;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Grid<GridTile> _grid;
    private TextMesh[,] debugTextArray;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;


    void Start()
    {
        _grid = new Grid<GridTile>(width, height, 3f, transform.position);
        debugTextArray = new TextMesh[width, height];
        _grid.OnGridChanged += _grid_OnGridChanged;
        _grid.InitGrid((Grid<GridTile> g, int x, int y) => new GridTile("basic", g, x, y ));
        ApplyNeighbors();
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
        Debug.Log(debugTextArray[e.x, e.y]);
        if (debugTextArray[e.x, e.y] == null)
        {
            debugTextArray[e.x, e.y] = UtilsClass.CreateWorldText(e.gridObject.Content, null, grid.GetWorldPosition(e.x, e.y) + new Vector3(grid.CellSize, grid.CellSize) * .5f, 15, Color.white, TextAnchor.MiddleCenter);
            Debug.DrawLine(grid.GetWorldPosition(e.x, e.y), grid.GetWorldPosition(e.x, e.y + 1), Color.white, 100f); ;
            Debug.DrawLine(grid.GetWorldPosition(e.x, e.y), grid.GetWorldPosition(e.x + 1, e.y), Color.white, 100f);
        } else
        {
            debugTextArray[e.x, e.y].text = e.gridObject.Content;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _grid.GetGridObject(UtilsClass.GetMouseWorldPosition()).UpdateContent(Random.Range(0, 1000).ToString());
        }
        if (Input.GetMouseButtonDown(1))
        {
            GridTile selectedGridObject = _grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
            Debug.Log(selectedGridObject.Content);
            int breakout = 0;
            do
            {
                selectedGridObject.UpdateContent("yeah");
                selectedGridObject = selectedGridObject.TopNeighbor;
                breakout++;
                if (breakout > 100)
                {
                    Debug.Log("Breakout");
                    break;
                }
            } while (selectedGridObject != null);

        }
    }

}
