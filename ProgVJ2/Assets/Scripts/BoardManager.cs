using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData 
    {
        public bool Passable;
    }

    private CellData[,] _boardData;
    private Tilemap _tileMap;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;


    void Start()
    {
        _tileMap = GetComponentInChildren<Tilemap>();
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
                }

                _tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }


    void Update()
    {
        
    }
}
