using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("---------------------Grid Position-------------")]
    public int row;
    public int col;
    public int height = 1, width = 1;
    public Image backgroundImage;
    public TMP_Text tileText;
    public RectTransform rt;

    private Vector2 startPos;
    private Vector2 endPos;
    private float swipeThreshold = 50f;

    public void Refresh(float cellsize)
    {
        float x = col * cellsize;
        float y = -row * cellsize;
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(cellsize * width, cellsize * height);
    }
    public void Setup(int r, int c, int h, int w, float cellsize, string value, Color color)
    {
        row = r;
        col = c;
        height = h;
        width = w;
        backgroundImage.color = color;
        tileText.text = value;
        Refresh(cellsize);
    }   

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endPos = eventData.position;
        Vector2 delta = endPos - startPos;
        if (delta.magnitude < swipeThreshold) return;
        Vector2Int dir;
        if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            dir = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            dir = delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

    }
}
