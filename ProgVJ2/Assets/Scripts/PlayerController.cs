using System;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager _board;
    private Animator _animator;
    private Vector2Int _cellPosition;
    private Vector3 _moveTarget;

    private bool hasMoved = false;

    public float MoveSpeed = 5f;
    public Vector2Int Cell => _cellPosition;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        _board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        _cellPosition = cell;

        if (immediate)
        {
            hasMoved = false;
            transform.position = _board.CellToWorld(_cellPosition);
            _moveTarget = transform.position;
        }
        else
        {
            hasMoved = true;
            _moveTarget = _board.CellToWorld(_cellPosition);
        }

        _animator.SetBool("Moving", hasMoved);
    }

    private void PlayerAttackAnimation()
    {
        _animator.SetTrigger("Attack");
    }

    public void PlayerHitAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("PlayerHit");
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        Vector2Int newCellTarget = _cellPosition;
        bool inputDetected = false;


        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            inputDetected = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            inputDetected = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            inputDetected = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            inputDetected = true;
        }

        if (inputDetected && newCellTarget != _cellPosition)
        {
            BoardManager.CellData cellData = _board.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget, false);
                }
                else
                {
                    if (cellData.ContainedObject.PlayerWantsToEnter())
                    {
                        MoveTo(newCellTarget, false);
                    }
                    else
                    {
                        PlayerAttackAnimation();
                    }
                }
            }
        }

        if (hasMoved)
        {
            transform.position = Vector3.MoveTowards(transform.position, _moveTarget, MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _moveTarget) < 0.001f)
            {
                hasMoved = false;
                _animator.SetBool("Moving", false);

                BoardManager.CellData cellData = _board.GetCellData(_cellPosition);
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }
            return;
        }
    }
}
