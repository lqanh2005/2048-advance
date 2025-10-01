using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class TileData
{
    public int row;
    public int col;
    public int height;
    public int width;
    public string value;
    public bool isActive;
}

[Serializable]
public class LevelData
{
    public int rows;
    public int cols;
    public List<TileData> tiles = new List<TileData>();
}

public class LevelLoader : MonoBehaviour
{
    public List<Tile> tileLst = new List<Tile>();
    public int targetValue;
    /// <summary>
    /// Thêm tile vào danh sách theo dõi
    /// </summary>
    public void AddTileToList(Tile tile)
    {
        if (tile != null && !tileLst.Contains(tile))
        {
            tileLst.Add(tile);
        }
    }
    
    /// <summary>
    /// Xóa tile khỏi danh sách theo dõi
    /// </summary>
    public void RemoveTileFromList(Tile tile)
    {
        if (tile != null && tileLst.Contains(tile))
        {
            tileLst.Remove(tile);
            Debug.Log($"🗑️ Đã xóa tile {tile.tileText.text} khỏi danh sách. Còn lại: {tileLst.Count} tiles");
        }
    }
    
    /// <summary>
    /// Xóa tất cả tile khỏi danh sách
    /// </summary>
    public void ClearTileList()
    {
        tileLst.Clear();
        Debug.Log("🧹 Đã xóa tất cả tile khỏi danh sách theo dõi");
    }
    
    /// <summary>
    /// Lấy số lượng tile còn lại
    /// </summary>
    public int GetActiveTilesCount()
    {
        return tileLst.Count;
    }
    
    /// <summary>
    /// Kiểm tra điều kiện thắng (còn 1 tile duy nhất có giá trị lớn nhất)
    /// </summary>
    public bool IsWinCondition()
    {
        if (tileLst.Count != 1) return false;
        var tmp = GetHighestTileValue();
        if(targetValue != tmp) return false;
        return true;
    }
    
    /// <summary>
    /// Lấy giá trị của tile lớn nhất hiện tại
    /// </summary>
    public int GetHighestTileValue()
    {
        int maxValue = 0;
        foreach (Tile tile in tileLst)
        {
            int value = int.Parse(tile.tileText.text);
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }
    
    public LevelData LoadLevelFromResources(string fileName)
    {
        fileName = fileName.Replace(".json", "");
        
        // Load từ Resources/Levels/
        TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/{fileName}");
        
        if (jsonFile == null)
        {
            return null;
        }
        
        try
        {
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
            Debug.Log($"Load level từ Resources thành công: {fileName}");
            return levelData;
        }
        catch
        {
            return null;
        }
    }
}
