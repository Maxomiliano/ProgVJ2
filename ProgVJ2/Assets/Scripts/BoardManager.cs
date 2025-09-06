using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] _boardData;
    private List<Vector2Int> _emptyCellsList;
    private Tilemap _tileMap;
    private Grid _grid;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public FoodObject[] FoodPrefabArray;


    public void Init()
    {
        _tileMap = GetComponentInChildren<Tilemap>();
        _grid = GetComponentInChildren<Grid>();
        _emptyCellsList = new List<Vector2Int>();

        _boardData = new CellData[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;
                _boardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    _boardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    _boardData[x, y].Passable = true;

                    _emptyCellsList.Add(new Vector2Int(x, y));
                }

                _tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        _emptyCellsList.Remove(new Vector2Int(1, 1));
        GenerateFood();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }
        return _boardData[cellIndex.x, cellIndex.y];
    }

    void GenerateFood()
    {
        int foodCount = Random.Range(4, 7);
        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, _emptyCellsList.Count);
            Vector2Int coord = _emptyCellsList[randomIndex];

            _emptyCellsList.RemoveAt(randomIndex);
            CellData data = _boardData[coord.x, coord.y];
            FoodObject newFood = Instantiate(FoodPrefabArray[Random.Range(0,FoodPrefabArray.Length)]);
            newFood.transform.position = CellToWorld(coord);
            data.ContainedObject = newFood;

        }
    }
}
