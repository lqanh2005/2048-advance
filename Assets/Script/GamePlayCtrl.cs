using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCtrl : MonoBehaviour
{
    public static GamePlayCtrl Instance;

    public BoardManager boardManager;
    public SwipCtrl swipCtrl;
    public ActiveTileMove tileMove;
    public UIManger uiManger;

    private void Awake()
    {
        Instance = this;
    }
}
