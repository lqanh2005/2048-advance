using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCtrl : MonoBehaviour
{
    public static GamePlayCtrl Instance;

    public BoardManager boardManager;
    public UIManager uiManger;
    public LevelLoader levelLoader;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        uiManger.Init();
        boardManager.Init();
    }
}
