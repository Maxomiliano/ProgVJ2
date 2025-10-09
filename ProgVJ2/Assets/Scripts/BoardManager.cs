using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;

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

    public WallObject[] wallPrefabArray;
    public Enemy[] enemyPrefabArray;
    public FoodObject[] FoodPrefabArray;
    public CollectableCellObject[] CollectableCellObjectPrefabArray;

    public ExitCellObject ExitCellPrefab;

    public EnemyBiteCellObject EnemyBitePrefab;
    public int EnemyBiteMaxSpawns = 3;
    public float EnemyBiteSpawnInterval = 3f;

    private List<EnemyBiteCellObject> _enemyBiteCell = new List<EnemyBiteCellObject>();
    private Coroutine _biteCoroutine;


    public ObjectPooler ObjectPooler;
    public float ProjectileSpeed = 5f;
    public int ProjectileDamage = 1;
    public float ProjectileDuration = 4f;
    public float ProjectileInitialDelay = 2f;
    public float ProjectileInterval = 3f;

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
        GenerateEnemy();
        GenerateCollectable();

        if (EnemyBitePrefab != null && EnemyBiteMaxSpawns > 0)
        {
            GenerateEnemyBiteSpawner();
        }

        if (ObjectPooler != null) StartProjectileShooting();
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
            FoodObject newFood = Instantiate(FoodPrefabArray[Random.Range(0, FoodPrefabArray.Length)]);
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

    void GenerateEnemy()
    {
        int enemyCount = Random.Range(1, 2);
        for (int i = 0; i < enemyCount; i++)
        {
            int randomIndex = Random.Range(0, _emptyCellsList.Count);
            Vector2Int coord = _emptyCellsList[randomIndex];
            _emptyCellsList.RemoveAt(randomIndex);

            Enemy newEnemy = Instantiate(enemyPrefabArray[Random.Range(0, enemyPrefabArray.Length)]);
            AddObject(newEnemy, coord);
        }
    }

    void GenerateCollectable()
    {
        int collectableCount = Random.Range(1, 2);
        for (int i = 0; i < collectableCount; i++)
        {
            int randomIndex = Random.Range(0, _emptyCellsList.Count);
            Vector2Int coord = _emptyCellsList[randomIndex];
            _emptyCellsList.RemoveAt(randomIndex);

            CollectableCellObject newCollectableObj = Instantiate(CollectableCellObjectPrefabArray[Random.Range(0, CollectableCellObjectPrefabArray.Length)]);
            AddObject(newCollectableObj, coord);
        }
    }

    public void GenerateEnemyBiteSpawner()
    {
        if (_biteCoroutine != null) return;
        _biteCoroutine = StartCoroutine(EnemyBiteSpawnCoroutine());
    }


    private IEnumerator EnemyBiteSpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(EnemyBiteSpawnInterval);

            List<Vector2Int> validCells = GetValidEmptyCells();

            if (validCells.Count == 0) continue;

            int spawnsToDo = Mathf.Min(EnemyBiteMaxSpawns, validCells.Count);


            for (int i = 0; i < spawnsToDo; i++)
            {
                int randomIndex = Random.Range(0, validCells.Count);
                Vector2Int pos = validCells[randomIndex];
                validCells.RemoveAt(randomIndex);
                SpawnEnemyBite();
            }
        }
    }

    private void SpawnEnemyBite()
    {
        List<Vector2Int> emptyCells = GetValidEmptyCells();

        if (emptyCells.Count == 0) return;

        Vector2Int spawnCell = emptyCells[Random.Range(0, emptyCells.Count)];

        EnemyBiteCellObject newBite = Instantiate(EnemyBitePrefab);
        AddObject(newBite, spawnCell);
        _enemyBiteCell.Add(newBite);
    }

    private List<Vector2Int> GetValidEmptyCells()
    {
        List<Vector2Int> validCells = new List<Vector2Int>();

        for (int y = 1; y < Height - 1; y++)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                var cellData = GetCellData(new Vector2Int(x, y));
                if (cellData != null && cellData.Passable && cellData.ContainedObject == null)
                {
                    validCells.Add(new Vector2Int(x, y));
                }
            }
        }
        return validCells;
    }


    public void StartProjectileShooting()
    {
        if (ObjectPooler == null || ObjectPooler.PooledPrefab == null) return;
        CancelInvoke(nameof(ShootFromBorder));
        InvokeRepeating(nameof(ShootFromBorder), ProjectileInitialDelay, ProjectileInterval);
    }

    public void StopProjectileShooting()
    {
        CancelInvoke(nameof(ShootFromBorder));
    }

    private void ShootFromBorder()
    {
        if (ObjectPooler == null) return;

        GameObject go = ObjectPooler.GetPooledObject();
        if (go == null) return;

        int borderSide = Random.Range(0, 4);
        Vector2Int borderCell;
        Vector3 direction;

        switch (borderSide)
        {
            case 0:
                borderCell = new Vector2Int(0, Random.Range(1, Height - 1));
                direction = Vector3.right;
                break;
            case 1:
                borderCell = new Vector2Int(Width - 1, Random.Range(1, Height - 1));
                direction = Vector3.left;
                break;
            case 2:
                borderCell = new Vector2Int(Random.Range(1, Width - 1), 0);
                direction = Vector3.up;
                break;
            default:
                borderCell = new Vector2Int(Random.Range(1, Width - 1), Height - 1);
                direction = Vector3.down;
                break;
        }

        Vector3 worldPos = CellToWorld(borderCell);
        go.transform.position = worldPos;
        go.transform.SetParent(null);
        go.SetActive(true);

        WallProjectile projectile = go.GetComponent<WallProjectile>();
        if (projectile != null)
        {
            projectile.Init(direction, ProjectileSpeed, ProjectileDamage, ProjectileDuration, ObjectPooler);
        }
        //else
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
        StopProjectileShooting();
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
