using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public TurnManager TurnManager;
    public UIDocument UIDoc;

    private int _foodAmount = 100;
    private Label _foodLabel;
    private int _currentLevel = 1;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;        

        NewLevel();

        _foodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        _foodLabel.text = "Food " + _foodAmount;
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        _currentLevel++;
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        _foodAmount += amount;
        _foodLabel.text = "Food : " + _foodAmount;
    }
}
