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
    
    [Header("Visual Settings")]
    public Color originalColor;
    public Color selectedColor = Color.white;
    public bool isSelected = false;

    public void Refresh(float cellsize)
    {
        float x = col * cellsize;
        float y = row * cellsize;
        
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(cellsize * width, cellsize * height);
    }
    public void Setup(int r, int c, int h, int w, float cellsize, string value, Color color)
    {
        row = r;
        col = c;
        height = h;
        width = w;
        originalColor = color; // Lưu màu gốc
        backgroundImage.color = color;
        tileText.text = value;
        Refresh(cellsize);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        //backgroundImage.color = selected ? selectedColor : originalColor;
    }   
}
