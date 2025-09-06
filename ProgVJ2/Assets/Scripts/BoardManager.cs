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
    public WallObject[] wallPrefabArray;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public FoodObject[] FoodPrefabArray;
    public ExitCellObject ExitCellPrefab;


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
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        _emptyCellsList.Remove(endCoord);

        GenerateWall();
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
            FoodObject newFood = Instantiate(FoodPrefabArray[Random.Range(0,FoodPrefabArray.Length)]);
            AddObject(newFood, coord);

        }
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; i++)
        {
            int randomIndex = Random.Range(0, _emptyCellsList.Count);
            Vector2Int coord = _emptyCellsList[randomIndex];

            _emptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(wallPrefabArray[Random.Range(0, wallPrefabArray.Length)]);
            AddObject(newWall, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        _tileMap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = _boardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return _tileMap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void Clean()
    {
        if (_boardData == null) return;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                CellData cellData = _boardData[x, y];
                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }
                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}
