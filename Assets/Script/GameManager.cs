using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MusicController musicController;
    public AdsController adsController;
    public AlalyticController alalyticController;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
