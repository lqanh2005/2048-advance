using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelDataAsset[] levels;
    public int currentLevel = 0;
    
    [Header("UI References")]
    public Text levelText;
    public Text targetText;
    public Text movesText;
    public Text timeText;
    public Button nextLevelButton;
    public Button restartButton;
    
    [Header("Game References")]
    public BoardManager boardManager;
    
    private int currentMoves = 0;
    private float currentTime = 0;
    private bool isLevelActive = false;
    private bool isLevelCompleted = false;
    
    public static LevelManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadLevel(currentLevel);
    }
    
    void Update()
    {
        if (isLevelActive && !isLevelCompleted)
        {
            UpdateTime();
            CheckWinCondition();
        }
    }
    
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= levels.Length)
        {
            Debug.Log("No more levels!");
            return;
        }
        
        currentLevel = levelIndex;
        LevelData level = levels[levelIndex].levelData;
        
        // Reset level state
        currentMoves = 0;
        currentTime = 0;
        isLevelActive = true;
        isLevelCompleted = false;
        
        // Clear existing tiles
        ClearAllTiles();
        
        // Setup level
        SetupLevel(level);
        
        // Update UI
        UpdateUI();
        
        Debug.Log($"Loaded Level {level.levelNumber}: {level.levelName}");
    }
    
    void SetupLevel(LevelData level)
    {
        // Create initial tiles
        //foreach (TileData tileData in level.initialTiles)
        //{
        //    GameObject tileObj = Instantiate(GamePlayCtrl.Instance.boardManager.tilePrefab.gameObject, GamePlayCtrl.Instance.boardManager.tilesParent);
        //    Tile tile = tileObj.GetComponent<Tile>();
        //    tile.Setup(tileData.row, tileData.col, tileData.width, tileData.height, 
        //              boardManager.GetCellSize(), tileData.value, tileData.color);
        //    GamePlayCtrl.Instance.boardManager.MarkOccupancy(tile, true);
            
        //    if (tileData.isActive)
        //    {
        //        GamePlayCtrl.Instance.boardManager.SetActiveTile(tile);
        //    }
        //}
        
        //// Create obstacles
        //foreach (TileData obstacleData in level.obstacles)
        //{
        //    GameObject tileObj = Instantiate(GamePlayCtrl.Instance.boardManager.tilePrefab.gameObject, GamePlayCtrl.Instance.boardManager.tilesParent);
        //    Tile obstacle = obstacleObj.GetComponent<Tile>();
        //    obstacle.Setup(obstacleData.row, obstacleData.col, obstacleData.width, obstacleData.height,
        //                  boardManager.GetCellSize(), obstacleData.value, obstacleData.color);
        //    boardManager.MarkOccupancy(obstacle, true);
        //}
    }
    
    void ClearAllTiles()
    {
        for (int i = boardManager.tilesParent.childCount - 1; i >= 0; i--)
        {
            Destroy(boardManager.tilesParent.GetChild(i).gameObject);
        }
    }
    
    public void OnTileMoved()
    {
        if (!isLevelActive || isLevelCompleted) return;
        
        currentMoves++;
        UpdateUI();
        
        // Check if moves exceeded
        LevelData level = levels[currentLevel].levelData;
        if (level.maxMoves > 0 && currentMoves >= level.maxMoves)
        {
            GameOver("Hết lượt di chuyển!");
        }
    }
    
    void UpdateTime()
    {
        LevelData level = levels[currentLevel].levelData;
        if (level.timeLimit > 0)
        {
            currentTime += Time.deltaTime;
            UpdateUI();
            
            if (currentTime >= level.timeLimit)
            {
                GameOver("Hết thời gian!");
            }
        }
    }
    
    void CheckWinCondition()
    {
        LevelData level = levels[currentLevel].levelData;
        
        // Check if any tile reached target value
        List<Tile> allTiles = GamePlayCtrl.Instance.boardManager.GetAllTiles();
        foreach (Tile tile in allTiles)
        {
            int tileValue = int.Parse(tile.tileText.text);
            if (tileValue >= level.targetValue)
            {
                LevelCompleted();
                return;
            }
        }
    }
    
    void LevelCompleted()
    {
        isLevelCompleted = true;
        isLevelActive = false;
        
        Debug.Log($"Level {currentLevel + 1} Completed!");
        
        // Show win UI
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
        
        // Save progress
        SaveProgress();
    }
    
    void GameOver(string reason)
    {
        isLevelActive = false;
        Debug.Log($"Game Over: {reason}");
        
        // Show restart button
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
    }
    
    void UpdateUI()
    {
        LevelData level = levels[currentLevel].levelData;
        
        if (levelText != null)
            levelText.text = $"Level {level.levelNumber}";
            
        if (targetText != null)
            targetText.text = $"Target: {level.targetValue}";
            
        if (movesText != null)
        {
            if (level.maxMoves > 0)
                movesText.text = $"Moves: {currentMoves}/{level.maxMoves}";
            else
                movesText.text = $"Moves: {currentMoves}";
        }
        
        if (timeText != null)
        {
            if (level.timeLimit > 0)
            {
                float remainingTime = level.timeLimit - currentTime;
                timeText.text = $"Time: {remainingTime:F1}s";
            }
            else
            {
                timeText.text = $"Time: {currentTime:F1}s";
            }
        }
    }
    
    void SaveProgress()
    {
        int highestLevel = PlayerPrefs.GetInt("HighestLevel", 0);
        if (currentLevel + 1 > highestLevel)
        {
            PlayerPrefs.SetInt("HighestLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
    
    public void NextLevel()
    {
        LoadLevel(currentLevel + 1);
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);
    }
    
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
    }
    
    public void LoadNextLevel()
    {
        if (currentLevel + 1 < levels.Length)
        {
            NextLevel();
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
}
