using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private Tilemap _tileMap;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;


    void Start()
    {
        _tileMap = GetComponentInChildren<Tilemap>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int tileNumber = Random.Range(0, GroundTiles.Length);
                _tileMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[tileNumber]);
            }
        }
    }


    void Update()
    {
        
    }
}
