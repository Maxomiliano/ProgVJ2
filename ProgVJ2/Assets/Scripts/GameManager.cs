using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TurnManager _turnManager;

    public BoardManager BoardManager;
    public PlayerController PlayerController;


    void Start()
    {
        _turnManager = new TurnManager();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }
}
