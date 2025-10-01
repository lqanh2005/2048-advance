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
