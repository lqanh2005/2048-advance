using UnityEngine;
using UnityEngine.UI;

public class GridDrawer : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 5;
    public int cols = 5;
    public Color lineColor = Color.red;
    public float lineThickness = 2f;

    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        DrawGrid();
    }

    void DrawGrid()
    {
        float cellW = rt.rect.width / cols;
        float cellH = rt.rect.height / rows;

        // Vẽ các đường dọc
        for (int c = 0; c <= cols; c++)
        {
            CreateLine(new Vector2(c * cellW, 0), new Vector2(c * cellW, rt.rect.height));
        }

        // Vẽ các đường ngang
        for (int r = 0; r <= rows; r++)
        {
            CreateLine(new Vector2(0, -r * cellH), new Vector2(rt.rect.width, -r * cellH));
        }
    }

    void CreateLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(transform, false);
        
        var img = lineObj.GetComponent<Image>();
        img.color = lineColor;

        RectTransform lrt = lineObj.GetComponent<RectTransform>();
        lrt.anchorMin = lrt.anchorMax = new Vector2(0, 1); // top-left
        lrt.pivot = new Vector2(0, 0);

        Vector2 dir = end - start;
        float len = dir.magnitude;

        lrt.sizeDelta = new Vector2(len, lineThickness);
        lrt.anchoredPosition = start;
        lrt.localEulerAngles = new Vector3(0, 0, -Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}
