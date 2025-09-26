using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private ObjectCollector objectCollector = new ObjectCollector();
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public TurnManager TurnManager;
    public UIDocument UIDoc;

    public int CollectedCount = 0;
    private int _foodAmount = 20;
    private int _currentLevel = 1;

    private VisualElement _gameOverPanel;
    private ProgressBar _progressBar;
    private Label _foodLabel;
    private Label _gameOverMessage;
    private Label _levelLabel;
    private Label _stageLabel;
    private Label _collectableLabel;


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
        ObjectCollector objectCollector = FindFirstObjectByType<ObjectCollector>();

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;        

        _foodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

        _levelLabel = UIDoc.rootVisualElement.Q<Label>("LvlLabel");
        _progressBar = UIDoc.rootVisualElement.Q<ProgressBar>("ProgressBar");

        _stageLabel = UIDoc.rootVisualElement.Q<Label>("StageLabel");

        _collectableLabel = UIDoc.rootVisualElement.Q<Label>("CollectableLabel");

        StartNewGame();
    }

    public void StartNewGame()
    {
        _gameOverPanel.style.visibility = Visibility.Hidden;

        _currentLevel = 1;
        _foodAmount = 20;
        _foodLabel.text = "Food : " + _foodAmount;

        UpdateLevelUI(1);
        UpdateExpUI(0, 100);
        UpdateStageUI(_currentLevel);
        UpdateCollectableUI(CollectedCount);

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

        UpdateStageUI(_currentLevel);
        UpdateCollectableUI(CollectedCount);
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

    public void UpdateLevelUI(int level)
    {
        _levelLabel.text = $"Lvl {level}";
    }

    public void UpdateExpUI(int currentExp, int requiredExp)
    {
        _progressBar.highValue = requiredExp;
        _progressBar.value = currentExp;
    }

    public void UpdateStageUI(int stage)
    {
        if (_stageLabel != null)
        {
            _stageLabel.text = $"Stage {stage}";
        }
    }

    public void UpdateCollectableUI(int coll)
    {
        if (_collectableLabel != null)
        {
            _collectableLabel.text = $"Rescued: {CollectedCount}";
        }
    }
}
