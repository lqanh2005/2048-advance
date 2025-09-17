using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [ContextMenu("Create Sample Levels")]
    public void CreateSampleLevels()
    {
        CreateLevel1();
        CreateLevel2();
        CreateLevel3();
        CreateLevel4();
        CreateLevel5();
    }
    
    void CreateLevel1()
    {
        // Level 1: Đơn giản - Đạt 4
        LevelData level = new LevelData
        {
            levelNumber = 1,
            levelName = "Bắt đầu",
            description = "Merge 2 tile giá trị 2 để tạo ra 4",
            targetValue = 4,
            maxMoves = 0,
            timeLimit = 0,
            initialTiles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 0, col = 0, width = 1, height = 1, value = "2", color = Color.cyan, isActive = true },
                new TileData { row = 0, col = 1, width = 1, height = 1, value = "2", color = Color.cyan, isActive = false }
            },
            obstacles = new System.Collections.Generic.List<TileData>()
        };
        
        SaveLevel(level, "Level1");
    }
    
    void CreateLevel2()
    {
        // Level 2: Có chướng ngại - Đạt 8
        LevelData level = new LevelData
        {
            levelNumber = 2,
            levelName = "Chướng ngại đầu tiên",
            description = "Merge để tạo ra 8, cẩn thận với chướng ngại!",
            targetValue = 8,
            maxMoves = 0,
            timeLimit = 0,
            initialTiles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 0, col = 0, width = 1, height = 1, value = "2", color = Color.cyan, isActive = true },
                new TileData { row = 0, col = 1, width = 1, height = 1, value = "2", color = Color.cyan, isActive = false },
                new TileData { row = 1, col = 0, width = 1, height = 1, value = "2", color = Color.green, isActive = false }
            },
            obstacles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 2, col = 2, width = 1, height = 1, value = "X", color = Color.red, isActive = false }
            }
        };
        
        SaveLevel(level, "Level2");
    }
    
    void CreateLevel3()
    {
        // Level 3: Giới hạn lượt - Đạt 16
        LevelData level = new LevelData
        {
            levelNumber = 3,
            levelName = "Chiến thuật",
            description = "Tạo ra 16 trong 10 lượt di chuyển",
            targetValue = 16,
            maxMoves = 10,
            timeLimit = 0,
            initialTiles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 0, col = 0, width = 1, height = 1, value = "2", color = Color.cyan, isActive = true },
                new TileData { row = 0, col = 1, width = 1, height = 1, value = "2", color = Color.cyan, isActive = false },
                new TileData { row = 1, col = 0, width = 1, height = 1, value = "4", color = Color.yellow, isActive = false },
                new TileData { row = 1, col = 1, width = 1, height = 1, value = "4", color = Color.yellow, isActive = false }
            },
            obstacles = new System.Collections.Generic.List<TileData>()
        };
        
        SaveLevel(level, "Level3");
    }
    
    void CreateLevel4()
    {
        // Level 4: Giới hạn thời gian - Đạt 32
        LevelData level = new LevelData
        {
            levelNumber = 4,
            levelName = "Tốc độ",
            description = "Tạo ra 32 trong 30 giây",
            targetValue = 32,
            maxMoves = 0,
            timeLimit = 30f,
            initialTiles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 0, col = 0, width = 1, height = 1, value = "4", color = Color.yellow, isActive = true },
                new TileData { row = 0, col = 1, width = 1, height = 1, value = "4", color = Color.yellow, isActive = false },
                new TileData { row = 1, col = 0, width = 1, height = 1, value = "8", color = Color.magenta, isActive = false },
                new TileData { row = 1, col = 1, width = 1, height = 1, value = "8", color = Color.magenta, isActive = false }
            },
            obstacles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 2, col = 0, width = 1, height = 1, value = "X", color = Color.red, isActive = false },
                new TileData { row = 2, col = 1, width = 1, height = 1, value = "X", color = Color.red, isActive = false }
            }
        };
        
        SaveLevel(level, "Level4");
    }
    
    void CreateLevel5()
    {
        // Level 5: Phức tạp - Đạt 64
        LevelData level = new LevelData
        {
            levelNumber = 5,
            levelName = "Thử thách",
            description = "Tạo ra 64 với nhiều chướng ngại",
            targetValue = 64,
            maxMoves = 15,
            timeLimit = 60f,
            initialTiles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 0, col = 0, width = 1, height = 1, value = "8", color = Color.magenta, isActive = true },
                new TileData { row = 0, col = 1, width = 1, height = 1, value = "8", color = Color.magenta, isActive = false },
                new TileData { row = 1, col = 0, width = 1, height = 1, value = "16", color = Color.blue, isActive = false },
                new TileData { row = 1, col = 1, width = 1, height = 1, value = "16", color = Color.blue, isActive = false }
            },
            obstacles = new System.Collections.Generic.List<TileData>
            {
                new TileData { row = 2, col = 0, width = 1, height = 1, value = "X", color = Color.red, isActive = false },
                new TileData { row = 2, col = 1, width = 1, height = 1, value = "X", color = Color.red, isActive = false },
                new TileData { row = 0, col = 2, width = 1, height = 1, value = "X", color = Color.red, isActive = false },
                new TileData { row = 1, col = 2, width = 1, height = 1, value = "X", color = Color.red, isActive = false }
            }
        };
        
        SaveLevel(level, "Level5");
    }
    
    void SaveLevel(LevelData level, string fileName)
    {
        // Tạo ScriptableObject
        LevelDataAsset asset = ScriptableObject.CreateInstance<LevelDataAsset>();
        asset.levelData = level;
        
        // Lưu file (chỉ trong Editor)
        #if UNITY_EDITOR
        string path = $"Assets/Levels/{fileName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(asset, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Created level: {path}");
        #endif
    }
}
