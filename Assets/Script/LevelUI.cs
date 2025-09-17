using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text levelText;
    public Text targetText;
    public Text movesText;
    public Text timeText;
    public Button nextLevelButton;
    public Button restartButton;
    public Button menuButton;
    
    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button gameOverRestartButton;
    
    [Header("Win Panel")]
    public GameObject winPanel;
    public Text winText;
    public Button winNextButton;
    
    private LevelManager levelManager;
    
    void Start()
    {
        levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }
        
        SetupUI();
    }
    
    void SetupUI()
    {
        // Setup buttons
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() => levelManager.NextLevel());
            nextLevelButton.gameObject.SetActive(false);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => levelManager.RestartLevel());
            restartButton.gameObject.SetActive(false);
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() => LoadMainMenu());
        }
        
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(() => {
                levelManager.RestartLevel();
                HideGameOverPanel();
            });
        }
        
        if (winNextButton != null)
        {
            winNextButton.onClick.AddListener(() => {
                levelManager.LoadNextLevel();
                HideWinPanel();
            });
        }
    }
    
    public void ShowGameOverPanel(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = reason;
            }
        }
    }
    
    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
            {
                winText.text = "Level Completed!";
            }
        }
    }
    
    public void HideWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }
    
    void LoadMainMenu()
    {
        // Implement main menu loading
        Debug.Log("Loading Main Menu...");
    }
    
    // Method để update UI từ LevelManager
    public void UpdateLevelInfo(int levelNumber, string levelName)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {levelNumber}: {levelName}";
        }
    }
    
    public void UpdateTarget(int targetValue)
    {
        if (targetText != null)
        {
            targetText.text = $"Target: {targetValue}";
        }
    }
    
    public void UpdateMoves(int currentMoves, int maxMoves)
    {
        if (movesText != null)
        {
            if (maxMoves > 0)
                movesText.text = $"Moves: {currentMoves}/{maxMoves}";
            else
                movesText.text = $"Moves: {currentMoves}";
        }
    }
    
    public void UpdateTime(float currentTime, float timeLimit)
    {
        if (timeText != null)
        {
            if (timeLimit > 0)
            {
                float remainingTime = timeLimit - currentTime;
                timeText.text = $"Time: {remainingTime:F1}s";
                
                // Change color when time is running out
                if (remainingTime < 10f)
                {
                    timeText.color = Color.red;
                }
                else
                {
                    timeText.color = Color.white;
                }
            }
            else
            {
                timeText.text = $"Time: {currentTime:F1}s";
                timeText.color = Color.white;
            }
        }
    }
}
