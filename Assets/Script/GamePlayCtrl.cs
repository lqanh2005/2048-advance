using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCtrl : MonoBehaviour
{
    public static GamePlayCtrl Instance;

    public BoardManager boardManager;
    public UIManger uiManger;
    public LevelLoader levelLoader;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        boardManager.Init();
    }
}
