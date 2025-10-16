using UnityEngine;

public class Enemy : CellObject
{
    public int Health = 3;
    private int _currentHealth;
    public int experienceValue = 5;

    private Animator _animator;

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerProgression progression = player.GetComponent<PlayerProgression>();
            if (progression != null)
            {
                progression.GainExperience(experienceValue);
            }
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        _currentHealth = Health;
        transform.position = GameManager.Instance.BoardManager.CellToWorld(coord);
        _animator = GetComponent<Animator>();

        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    public override bool PlayerWantsToEnter()
    {
        _currentHealth -= 1;

        if(_currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        return false;
    }

    private bool MoveTo(Vector2Int coord)
    {
        BoardManager board = GameManager.Instance.BoardManager;
        BoardManager.CellData targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        //remove enemy from current cell
        BoardManager.CellData currentCell = board.GetCellData(_cell);
        currentCell.ContainedObject = null;

        //add it to the next cell
        targetCell.ContainedObject = this;
        _cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    private void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - _cell.x;
        int yDist = playerCell.y - _cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
        {
            if (_animator != null)
            {
                _animator.SetTrigger("EnemyAttack");
            }

            PlayerController player = GameManager.Instance.PlayerController;
            if (player != null)
            {
                player.PlayerHitAnimation();
            }

            GameManager.Instance.ChangeFood(-1);
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    private bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(_cell + Vector2Int.right);
        }

        return MoveTo(_cell + Vector2Int.left);
    }

    private bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo (_cell + Vector2Int.up);
        }

        return MoveTo(_cell + Vector2Int.down);
    }
}
