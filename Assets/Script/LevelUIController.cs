using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script điều khiển UI cho các nút level (Next, Restart, Previous)
/// Gắn script này vào Canvas hoặc UI Panel
/// </summary>
public class LevelUIController : MonoBehaviour
{
    [Header("References")]
    public GamePlayCtrl gamePlayCtrl;

    [Header("UI Buttons")]
    public Button nextLevelButton;
    public Button restartButton;
    public Button previousLevelButton;

    [Header("Display")]
    public TMP_Text levelText; // Hiển thị "Level 1", "Level 2", ...

    
}


