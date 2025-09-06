using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public TurnManager TurnManager;
    public UIDocument UIDoc;

    private int _foodAmount = 10;
    private Label _foodLabel;
    private int _currentLevel = 1;
    private VisualElement _gameOverPanel;
    private Label _gameOverMessage;


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

        _foodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }

    public void StartNewGame()
    {
        _gameOverPanel.style.visibility = Visibility.Hidden;

        _currentLevel = 1;
        _foodAmount = 20;
        _foodLabel.text = "Food : " + _foodAmount;

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
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

        if (_foodAmount <= 0)
        {
            PlayerController.GameOver();
            _gameOverPanel.style.visibility = Visibility.Visible;
            _gameOverMessage.text = "Game Over!\n\nYou traveled through " + _currentLevel + " levels"; 
        }
    }
}
