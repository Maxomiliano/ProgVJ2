using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyBiteSpawner : MonoBehaviour
{
    public EnemyBiteCellObject EnemyBitePrefab;
    public int MaxSpawns = 3;
    public float SpawnInterval = 3f;

    private BoardManager _board;
    private List<EnemyBiteCellObject> _activeEnemies = new List<EnemyBiteCellObject>();

    private void Start()
    {
        _board = GetComponent<BoardManager>();
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);

            _activeEnemies.RemoveAll(b => b == null);

            if (_activeEnemies.Count < MaxSpawns)
            {
                SpawnEnemyBite();
            }
        }
    }

    private void SpawnEnemyBite()
    {
        List<Vector2Int> emptyCells = GetValidEmptyCell();

        if (emptyCells.Count == 0) return;

        Vector2Int spawnCell = emptyCells[Random.Range(0, emptyCells.Count)];
        Vector3 worldPos = _board.CellToWorld(spawnCell);

        EnemyBiteCellObject newBite = Instantiate(EnemyBitePrefab, worldPos, Quaternion.identity);
        _board.GetCellData(spawnCell).ContainedObject = newBite;
        newBite.Init(spawnCell);

        _activeEnemies.Add(newBite);
    }

    private List<Vector2Int> GetValidEmptyCell()
    {
        List<Vector2Int> validCells = new List<Vector2Int>();

        for (int y = 1; y < _board.Height - 1; y++)
        {
            for (int x = 1; x < _board.Width - 1; x++)
            {
                var cellData = _board.GetCellData(new Vector2Int(x, y));
                if (cellData != null && cellData.Passable && cellData.ContainedObject == null)
                {
                    validCells.Add(new Vector2Int(x, y));
                }
            }
        }
        return validCells;
    }
}
