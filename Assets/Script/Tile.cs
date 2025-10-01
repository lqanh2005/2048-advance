using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("---------------------Grid Position-------------")]
    public int row;
    public int col;
    public int height = 1, width = 1;
    public Image backgroundImage;
    public TMP_Text tileText;
    public RectTransform rt;
    
    public bool isSelected = false;

    public void Refresh(float cellsize)
    {
        float x = col * cellsize;
        float y = row * cellsize;
        
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(cellsize * width, cellsize * height);
    }
    public void Setup(int r, int c, int h, int w, float cellsize, string value)
    {
        row = r;
        col = c;
        height = h;
        width = w;
        backgroundImage.color = GetColor(int.Parse(value));
        tileText.text = value;
        Refresh(cellsize);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
    public Color GetColor(int value)
    {
        switch (value)
        {
            case 4: return Color.magenta;
            case 8: return Color.yellow;
            case 16: return Color.green;
            case 32: return Color.clear;
            default: return Color.cyan;
        }
    }
}
