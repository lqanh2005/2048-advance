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
    
    /// <summary>
    /// Cập nhật value và tự động thay đổi kích thước theo quy ước
    /// </summary>
    public void UpdateValue(int newValue, float cellsize)
    {
        tileText.text = newValue.ToString();
        
        // Lấy kích thước mới theo value
        (int newWidth, int newHeight) = GetSizeFromValue(newValue);
        width = newWidth;
        height = newHeight;
        
        // Cập nhật màu
        backgroundImage.color = GetColor(newValue);
        
        // Refresh vị trí và kích thước
        Refresh(cellsize);
    }
    
    /// <summary>
    /// Quy ước: value -> (width, height)
    /// 2 = 1x1
    /// 4 = 1x2
    /// 8 = 2x1
    /// 16 = 2x2
    /// 32 = 2x3 (hoặc tùy chỉnh)
    /// 64 = 3x2
    /// 128 = 3x3
    /// ...
    /// </summary>
    public static (int width, int height) GetSizeFromValue(int value)
    {
        switch (value)
        {
            case 2:   return (1, 1);  // 1x1
            case 4:   return (1, 2);  // 1x2
            case 8:   return (2, 1);  // 2x1
            case 16:  return (2, 2);  // 2x2
            case 32:  return (2, 3);  // 2x3
            case 64:  return (3, 2);  // 3x2
            case 128: return (3, 3);  // 3x3
            case 256: return (3, 4);  // 3x4
            case 512: return (4, 3);  // 4x3
            case 1024: return (4, 4); // 4x4
            default:
                int level = Mathf.FloorToInt(Mathf.Log(value, 2)) - 1;
                int w = 1 + (level / 2);
                int h = 1 + ((level + 1) / 2);
                return (w, h);
        }
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
            case 32: return Color.blue;
            default: return Color.cyan;
        }
    }
}
