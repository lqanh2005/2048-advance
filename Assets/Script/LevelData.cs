using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int row;
    public int col;
    public int width;
    public int height;
    public string value;
    public Color color;
    public bool isActive;
}

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public string levelName;
    public string description;
    public int targetValue; // Giá trị cần đạt để thắng
    public int maxMoves; // Số lượt di chuyển tối đa (0 = không giới hạn)
    public float timeLimit; // Thời gian giới hạn (0 = không giới hạn)
    public List<TileData> initialTiles;
    public List<TileData> obstacles; // Chướng ngại vật cố định
}

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level Data")]
public class LevelDataAsset : ScriptableObject
{
    public LevelData levelData;
}
