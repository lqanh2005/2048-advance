using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public SettingPanel settingPanel;
    public GameObject winBox;
    public Button nextBtn;
    public Button settingBtn;
    public TMP_Text levelTxt;
    public void Init()
    {
        settingPanel.Init();
        nextBtn.onClick.AddListener(HandheldNextLevel);
        settingBtn.onClick.AddListener(() => settingPanel.gameObject.SetActive(true));
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        levelTxt.text = $"Level {currentLevel}";
    }

    private void HandheldNextLevel()
    {
        var tmp = PlayerPrefs.GetInt("CurrentLevel", 1) + 1;
        PlayerPrefs.SetInt("CurrentLevel", tmp);
        GameManager.Instance.adsController.ShowInterstitialAd(() => Initiate.Fade("GamePlayScene", Color.black, 2f));

    }
}
