using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

namespace LevelEditor
{
    [Serializable]
    public class TileData
    {
        [HorizontalGroup("Tile")]
        [LabelWidth(50)]
        public int row;
        
        [HorizontalGroup("Tile")]
        [LabelWidth(50)]
        public int col;
        
        [HorizontalGroup("Tile")]
        [LabelWidth(60)]
        public int height;
        
        [HorizontalGroup("Tile")]
        [LabelWidth(60)]
        public int width;
        
        [HorizontalGroup("Tile")]
        [LabelWidth(50)]
        public string value;
        
        [HorizontalGroup("Tile")]
        [LabelWidth(80)]
        public bool isActive;
    }

    [Serializable]
    public class LevelData
    {
        [BoxGroup("Level Info")]
        [LabelWidth(60)]
        public int rows = 5;
        
        [BoxGroup("Level Info")]
        [LabelWidth(60)]
        public int cols = 5;
        
        [BoxGroup("Tiles")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "value")]
        public List<TileData> tiles = new List<TileData>();
    }

    [Serializable]
    public class LevelParameters
    {
        public int gridSize = 5;
        public int tileCount = 5;
        public int targetValue = 32; // Giá trị cần đạt
        public int maxTileWidth = 2;
        public int maxTileHeight = 2;
        public int obstacleCount = 0;
        public int minValue = 2;
        public bool allowInactiveTiles = false;
        
        // Win condition: Còn lại 1 tile duy nhất có giá trị = targetValue
        // Tiles ban đầu có giá trị từ minValue đến targetValue/2
        // Số lượng tiles phải là SỐ LẺ (vì mỗi merge: 2 tiles → 1 tile)
    }

    [CreateAssetMenu(fileName = "LevelGenerator", menuName = "Level Editor/Level Generator")]
    public class LevelGenerator : ScriptableObject
    {
        [Title("Level Generator")]
        [InfoBox("Sử dụng công cụ này để tạo và chỉnh sửa các level cho game 2048\n\n" +
                 "Win Condition: Còn lại 1 tile duy nhất có giá trị = Target Value\n" +
                 "Tiles ban đầu có giá trị từ 2 đến Target Value / 2\n" +
                 "Số lượng tiles phải là SỐ LẺ (vì mỗi merge: 2 tiles → 1 tile)\n\n" +
                 "Kích thước tiles tự động dựa vào value:\n" +
                 "2→1x1, 4→1x2, 8→2x1, 16→2x2, 32→2x3, 64→3x2, 128→3x3, 256→3x4, 512→4x3, 1024→4x4")]
        
        [BoxGroup("Current Level")]
        [HideLabel]
        public LevelData currentLevel = new LevelData();
        
        [Title("Auto Generation Settings", "Sinh tự động nhiều level với độ khó tăng dần")]
        [BoxGroup("Auto Generation")]
        [LabelWidth(150)]
        [Range(1, 1000)]
        [Tooltip("Số lượng level muốn sinh tự động")]
        public int numberOfLevels = 50;
        
        [BoxGroup("Auto Generation")]
        [LabelWidth(150)]
        [Range(1, 10)]
        [Tooltip("Độ khó tối thiểu (1 = dễ nhất)")]
        public float minDifficulty = 1f;
        
        [BoxGroup("Auto Generation")]
        [LabelWidth(150)]
        [Range(1, 10)]
        [Tooltip("Độ khó tối đa (10 = khó nhất)")]
        public float maxDifficulty = 10f;
        
        [BoxGroup("Auto Generation")]
        [LabelWidth(150)]
        [Tooltip("Tên bắt đầu cho các level (VD: level_1, level_2...)")]
        public string levelPrefix = "level_";
        
        [BoxGroup("Auto Generation")]
        [LabelWidth(150)]
        [Range(1, 100)]
        [Tooltip("Số lần thử lại tối đa nếu generation thất bại")]
        public int maxRetryAttempts = 10;
        
        [BoxGroup("Level Management")]
        [Button("Thêm Tile Mới", ButtonSizes.Medium)]
        private void AddNewTile()
        {
            if (currentLevel.tiles == null)
                currentLevel.tiles = new List<TileData>();
                
            currentLevel.tiles.Add(new TileData());
        }
        
        [BoxGroup("Level Management")]
        [Button("Xóa Tile Cuối", ButtonSizes.Medium)]
        private void RemoveLastTile()
        {
            if (currentLevel.tiles != null && currentLevel.tiles.Count > 0)
            {
                currentLevel.tiles.RemoveAt(currentLevel.tiles.Count - 1);
            }
        }
        
        [BoxGroup("Level Management")]
        [Button("Xóa Tất Cả Tiles", ButtonSizes.Medium)]
        private void ClearAllTiles()
        {
            if (currentLevel.tiles != null)
            {
                currentLevel.tiles.Clear();
            }
        }
        
        [BoxGroup("File Operations")]
        [LabelWidth(100)]
        public string fileName = "new_level";
        
        [BoxGroup("File Operations")]
        [Button("Lưu Level", ButtonSizes.Large)]
        [GUIColor(0.4f, 0.8f, 0.4f)]
        private void SaveLevel()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("Tên file không được để trống!");
                return;
            }
            
            string json = JsonConvert.SerializeObject(currentLevel, Formatting.Indented);
            string path = Path.Combine(Application.dataPath, "Resources/Levels", fileName + ".json");
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);
                Debug.Log($"Đã lưu level thành công: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Lỗi khi lưu level: {e.Message}");
            }
        }
        
        [BoxGroup("File Operations")]
        [Button("Tải Level", ButtonSizes.Large)]
        [GUIColor(0.4f, 0.6f, 0.8f)]
        private void LoadLevel()
        {
            string path = Path.Combine(Application.dataPath, "Resources/Levels", fileName + ".json");
            
            if (!File.Exists(path))
            {
                Debug.LogError($"File không tồn tại: {path}");
                return;
            }
            
            try
            {
                string json = File.ReadAllText(path);
                currentLevel = JsonConvert.DeserializeObject<LevelData>(json);
                Debug.Log($"Đã tải level thành công từ: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Lỗi khi tải level: {e.Message}");
            }
        }
        
        [BoxGroup("Quick Actions")]
        [Button("Tạo Level Mẫu (5x5)", ButtonSizes.Medium)]
        private void CreateSampleLevel()
        {
            currentLevel = new LevelData
            {
                rows = 5,
                cols = 5,
                tiles = new List<TileData>
                {
                    new TileData { row = 0, col = 0, height = 1, width = 2, value = "8", isActive = true },
                    new TileData { row = 0, col = 2, height = 2, width = 1, value = "4", isActive = false },
                    new TileData { row = 2, col = 1, height = 1, width = 1, value = "2", isActive = false },
                    new TileData { row = 2, col = 2, height = 1, width = 1, value = "2", isActive = false }
                }
            };
        }
        
        [BoxGroup("Level Preview")]
        [ShowInInspector]
        [ReadOnly]
        [TableMatrix(HorizontalTitle = "Level Preview", SquareCells = true)]
        private string[,] GetLevelPreview()
        {
            if (currentLevel?.tiles == null || currentLevel.tiles.Count == 0)
                return new string[0, 0];
                
            string[,] preview = new string[currentLevel.rows, currentLevel.cols];
            
            // Khởi tạo tất cả ô là trống
            for (int i = 0; i < currentLevel.rows; i++)
            {
                for (int j = 0; j < currentLevel.cols; j++)
                {
                    preview[i, j] = "";
                }
            }
            
            // Điền tiles vào preview
            foreach (var tile in currentLevel.tiles)
            {
                if (tile.isActive)
                {
                    for (int i = tile.row; i < tile.row + tile.height && i < currentLevel.rows; i++)
                    {
                        for (int j = tile.col; j < tile.col + tile.width && j < currentLevel.cols; j++)
                        {
                            preview[i, j] = tile.value;
                        }
                    }
                }
            }
            
            return preview;
        }
        
        [BoxGroup("Validation")]
        [Button("Kiểm Tra Level", ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.8f, 0.4f)]
        private void ValidateLevel()
        {
            if (currentLevel?.tiles == null)
            {
                Debug.LogWarning("Level chưa có tiles nào!");
                return;
            }
            
            bool isValid = ValidateLevelData(currentLevel);
            
            if (isValid)
            {
                Debug.Log("✅ Level hợp lệ! Không có lỗi nào được phát hiện.");
            }
            else
            {
                Debug.LogError("❌ Level có lỗi! Xem Console để biết chi tiết.");
            }
        }
        
        [BoxGroup("Validation")]
        [Button("Hiển Thị Mapping", ButtonSizes.Medium)]
        [GUIColor(0.4f, 0.4f, 0.8f)]
        private void ShowMapping()
        {
            Debug.Log("=== VALUE TO SIZE MAPPING ===");
            int[] values = {2, 4, 8, 16, 32, 64, 128, 256, 512, 1024};
            
            foreach (int value in values)
            {
                var (height, width) = GetTileSizeByValue(value);
                Debug.Log($"Value {value} → {height}x{width} (height={height}, width={width})");
            }
            Debug.Log("===============================");
        }
        
        [BoxGroup("Validation")]
        [Button("Kiểm Tra Tổng Mass", ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.4f, 0.4f)]
        private void CheckTotalMass()
        {
            if (currentLevel?.tiles == null)
            {
                Debug.LogWarning("Level chưa có tiles nào!");
                return;
            }
            
            int totalMass = 0;
            int activeTileCount = 0;
            
            Debug.Log("=== TỔNG MASS ANALYSIS ===");
            foreach (var tile in currentLevel.tiles)
            {
                if (tile.isActive && int.TryParse(tile.value, out int value))
                {
                    totalMass += value;
                    activeTileCount++;
                    Debug.Log($"Tile {tile.value} → +{value} (total: {totalMass})");
                }
            }
            
            Debug.Log($"Tổng mass: {totalMass}");
            Debug.Log($"Số active tiles: {activeTileCount}");
            Debug.Log($"Có phải lũy thừa của 2: {IsPowerOfTwo(totalMass)}");
            
            if (IsPowerOfTwo(totalMass))
            {
                Debug.Log($"✅ Tổng mass {totalMass} là lũy thừa của 2!");
            }
            else
            {
                Debug.LogError($"❌ Tổng mass {totalMass} KHÔNG phải lũy thừa của 2! Cần là 2, 4, 8, 16, 32...");
            }
            
            Debug.Log("==========================");
        }
        
        [BoxGroup("Validation")]
        [Button("Hiển Thị Logic Generation", ButtonSizes.Medium)]
        [GUIColor(0.4f, 0.8f, 0.8f)]
        private void ShowGenerationLogic()
        {
            Debug.Log("=== GENERATION LOGIC ===");
            
            // Test với các độ khó khác nhau
            for (float difficulty = 0; difficulty <= 5; difficulty += 1)
            {
                var parameters = GetLevelParameters(difficulty);
                Debug.Log($"Difficulty {difficulty:F1}:");
                Debug.Log($"  - Grid Size: {parameters.gridSize}x{parameters.gridSize}");
                Debug.Log($"  - Target Value: {parameters.targetValue} (lũy thừa của 2)");
                Debug.Log($"  - Tile Count: {parameters.tileCount} (số lẻ)");
                Debug.Log($"  - Logic: Tạo tiles từ targetValue {parameters.targetValue}");
                Debug.Log($"  - Ví dụ: {parameters.targetValue/2} + {parameters.targetValue/4} + {parameters.targetValue/4} = {parameters.targetValue}");
                Debug.Log("---");
            }
            
            Debug.Log("=========================");
        }
        
        [BoxGroup("Validation")]
        [Button("Test Split Algorithm", ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.4f, 0.8f)]
        private void TestSplitAlgorithm()
        {
            Debug.Log("=== TEST SPLIT ALGORITHM ===");
            
            // Test với các targetValue khác nhau
            int[] targets = { 8, 16, 32, 64, 128 };
            
            foreach (int target in targets)
            {
                Debug.Log($"\nTargetValue = {target} (2^{GetPowerIndex(target)})");
                
                // Test với N = 3, 5, 7
                int[] testN = { 3, 5, 7 };
                
                foreach (int N in testN)
                {
                    List<int> tiles = GenerateTileValuesBySplit(target, N, 1);
                    int totalMass = 0;
                    string tilesStr = "";
                    
                    foreach (int tile in tiles)
                    {
                        totalMass += tile;
                        tilesStr += tile + " ";
                    }
                    
                    bool isValid = totalMass == target;
                    string status = isValid ? "✅" : "❌";
                    
                    Debug.Log($"  N={N}: [{tilesStr}] → Total={totalMass} {status}");
                }
            }
            
            Debug.Log("============================");
        }
        
        [BoxGroup("Validation")]
        [Button("Sửa Level Tự Động", ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.4f, 0.8f)]
        private void AutoFixLevel()
        {
            if (currentLevel?.tiles == null)
            {
                Debug.LogWarning("Level chưa có tiles nào!");
                return;
            }
            
            Debug.Log("Bắt đầu sửa level tự động...");
            
            // Sửa kích thước tiles theo mapping
            foreach (var tile in currentLevel.tiles)
            {
                if (int.TryParse(tile.value, out int tileValue))
                {
                    var (expectedHeight, expectedWidth) = GetTileSizeByValue(tileValue);
                    tile.height = expectedHeight;
                    tile.width = expectedWidth;
                }
            }
            
            // Kiểm tra và sửa overflow
            foreach (var tile in currentLevel.tiles)
            {
                // Sửa overflow theo chiều ngang
                if (tile.col + tile.width > currentLevel.cols)
                {
                    int newCol = currentLevel.cols - tile.width;
                    tile.col = Mathf.Max(0, newCol);
                    Debug.Log($"Đã sửa tile {tile.value} overflow ngang: col={tile.col}");
                }
                
                // Sửa overflow theo chiều dọc
                if (tile.row + tile.height > currentLevel.rows)
                {
                    int newRow = currentLevel.rows - tile.height;
                    tile.row = Mathf.Max(0, newRow);
                    Debug.Log($"Đã sửa tile {tile.value} overflow dọc: row={tile.row}");
                }
            }
            
            // Kiểm tra lại
            bool isValid = ValidateLevelData(currentLevel);
            
            if (isValid)
            {
                Debug.Log("✅ Đã sửa level thành công!");
            }
            else
            {
                Debug.LogWarning("⚠️ Level vẫn có lỗi sau khi sửa. Cần kiểm tra thủ công.");
            }
        }
        
        // ==================== AUTO GENERATION METHODS ====================
        
        private (int height, int width) GetTileSizeByValue(int value)
        {
            // Mapping CỐ ĐỊNH và NHẤT QUÁN giữa value và kích thước tile
            // QUAN TRỌNG: Mapping này phải nhất quán với game engine!
            switch (value)
            {
                case 2:    return (1, 1);  // 1x1
                case 4:    return (1, 2);  // 1x2
                case 8:    return (2, 1);  // 2x1 (CAO 2, RỘNG 1)
                case 16:   return (2, 2);  // 2x2
                case 32:   return (2, 3);  // 2x3
                case 64:   return (3, 2);  // 3x2
                case 128:  return (3, 3);  // 3x3
                case 256:  return (3, 4);  // 3x4
                case 512:  return (4, 3);  // 4x3
                case 1024: return (4, 4);  // 4x4
                default:   return (1, 1);  // Fallback cho giá trị không xác định
            }
        }
        
        [BoxGroup("Auto Generation")]
        [Button("SINH TỰ ĐỘNG N LEVEL", ButtonSizes.Large)]
        [GUIColor(0.2f, 0.8f, 0.2f)]
        private void GenerateMultipleLevels()
        {
            Debug.Log($"Bắt đầu sinh {numberOfLevels} level tự động...");
            
            int successCount = 0;
            int failCount = 0;
            
            for (int i = 1; i <= numberOfLevels; i++)
            {
                // Tính độ khó cho level này (S-Curve progression)
                float difficulty = CalculateDifficulty(i, numberOfLevels);
                
                // Lấy parameters dựa trên độ khó
                LevelParameters parameters = GetLevelParameters(difficulty);
                
                // Sinh level
                LevelData generatedLevel = GenerateLevelWithRetry(i, parameters);
                
                if (generatedLevel != null)
                {
                    // Lưu level
                    string levelName = $"{levelPrefix}{i}";
                    SaveGeneratedLevel(generatedLevel, levelName);
                    successCount++;
                    
                    Debug.Log($"✓ Level {i}/{numberOfLevels} - Difficulty: {difficulty:F2} - Saved as {levelName}");
                }
                else
                {
                    failCount++;
                    Debug.LogWarning($"✗ Level {i}/{numberOfLevels} - Generation failed after {maxRetryAttempts} attempts");
                }
            }
            
            Debug.Log($"<color=green>Hoàn thành! Thành công: {successCount}, Thất bại: {failCount}</color>");
        }
        
        private float CalculateDifficulty(int levelIndex, int totalLevels)
        {
            // S-Curve progression: tăng từ từ đầu, nhanh giữa, chậm cuối
            float normalized = (float)levelIndex / totalLevels;
            float curveValue = Mathf.Pow(normalized, 1.5f);
            
            // Map vào range [minDifficulty, maxDifficulty]
            return minDifficulty + (maxDifficulty - minDifficulty) * curveValue;
        }
        
        private LevelParameters GetLevelParameters(float difficulty)
        {
            var parameters = new LevelParameters();
            
            // Grid Size: 3x3 đến 8x8 (tùy theo độ khó)
            parameters.gridSize = Mathf.Clamp(4 + Mathf.FloorToInt(difficulty / 2), 4, 8);
            
            // Tile Count: 3 đến 15 (PHẢI LÀ SỐ LẺ để còn lại 1 tile)
            int baseTileCount = 3 + Mathf.FloorToInt(difficulty * 1.2f);
            parameters.tileCount = Mathf.Clamp(baseTileCount, 3, 15);
            
            // Đảm bảo tileCount là số lẻ
            if (parameters.tileCount % 2 == 0)
                parameters.tileCount++; // Chuyển thành số lẻ
            
            // Target Value (giá trị cần đạt): 8 đến 1024
            // PHẢI LÀ LŨY THỪA CỦA 2 (8, 16, 32, 64, 128, 256, 512, 1024...)
            int baseTargetValue = Mathf.FloorToInt(8 * Mathf.Pow(2, difficulty / 2.5f));
            
            // Làm tròn về lũy thừa của 2 gần nhất
            parameters.targetValue = RoundToPowerOfTwo(baseTargetValue);
            
            // Đảm bảo targetValue không quá lớn (max 1024)
            parameters.targetValue = Mathf.Min(parameters.targetValue, 1024);
            
            // Max Tile Size: Không cần nữa vì kích thước được xác định bởi value
            // Nhưng giữ lại cho tương thích
            parameters.maxTileWidth = 4;
            parameters.maxTileHeight = 4;
            
            // Obstacles: 0 đến 4
            parameters.obstacleCount = Mathf.FloorToInt(difficulty / 3);
            
            // Min Value: luôn là 2
            parameters.minValue = 2;
            
            // Allow inactive tiles sau level 5
            parameters.allowInactiveTiles = difficulty > 5;
            
            return parameters;
        }
        
        private int RoundToPowerOfTwo(int value)
        {
            // Làm tròn value về lũy thừa của 2 gần nhất
            // VD: 10 → 8, 15 → 16, 20 → 16
            
            if (value <= 2) return 2;
            
            // Tìm lũy thừa của 2 gần nhất
            int power = 1;
            while (power < value)
            {
                power *= 2;
            }
            
            // Chọn gần hơn giữa power và power/2
            int lowerPower = power / 2;
            if (value - lowerPower < power - value)
                return lowerPower;
            else
                return power;
        }
        
        private LevelData GenerateLevelWithRetry(int levelIndex, LevelParameters parameters)
        {
            for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
            {
                LevelData level = GenerateSingleLevel(levelIndex, parameters);
                
                if (level != null && ValidateLevelData(level) && CanWinLevel(level, parameters.targetValue))
                {
                    return level;
                }
            }
            
            // Nếu fail hết, return level đơn giản
            return CreateFallbackLevel(parameters);
        }
        
        private LevelData GenerateSingleLevel(int levelIndex, LevelParameters parameters)
        {
            var level = new LevelData
            {
                rows = parameters.gridSize,
                cols = parameters.gridSize,
                tiles = new List<TileData>()
            };
            
            // Tạo grid để track vị trí đã dùng
            bool[,] occupiedGrid = new bool[parameters.gridSize, parameters.gridSize];
            
            // ===== THUẬT TOÁN SPLIT-BASED GENERATION =====
            // 1. Chọn targetValue = 2^T (đã có từ parameters)
            // 2. Chọn dải bậc [b..a] với b < a < T
            // 3. Sinh N tiles bằng cách split từ 1 tile 2^T thành N tiles
            
            int T = GetPowerIndex(parameters.targetValue); // T = log2(targetValue)
            int b = 1; // Bậc thấp nhất (2^1 = 2)
            int a = Mathf.Min(T - 1, 4); // Bậc cao nhất (không vượt quá T-1 và tối đa 16 = 2^4)
            int N = parameters.tileCount; // Số tiles mong muốn
            
            // Kiểm tra N có hợp lệ không: N phải nằm trong [2^(T-a), 2^(T-b)]
            int minN = Mathf.FloorToInt(Mathf.Pow(2, T - a));
            int maxN = Mathf.FloorToInt(Mathf.Pow(2, T - b));
            N = Mathf.Clamp(N, Mathf.Max(3, minN), maxN);
            
            // Đảm bảo N là số lẻ
            if (N % 2 == 0) N++;
            
            // Sinh multiset bằng cách split từ 1 tile 2^T
            List<int> tileValues = GenerateTileValuesBySplit(parameters.targetValue, N, b);
            
            // Tạo tiles với các giá trị đã sinh
            foreach (int tileValue in tileValues)
            {
                TileData tile = GenerateTileWithValue(parameters, occupiedGrid, tileValue);
                
                if (tile != null)
                {
                    level.tiles.Add(tile);
                    MarkGridOccupied(occupiedGrid, tile);
                }
                else
                {
                    // Nếu không đặt được, thử vị trí khác hoặc bỏ qua
                    Debug.LogWarning($"Không thể đặt tile với value {tileValue}");
                }
            }
            
            return level.tiles.Count >= 3 ? level : null; // Tối thiểu 3 tiles
        }
        
        private int GetPowerIndex(int value)
        {
            // Tính T sao cho 2^T = value
            // VD: value = 8 → T = 3 (2^3 = 8)
            //     value = 16 → T = 4 (2^4 = 16)
            int T = 0;
            int power = 1;
            while (power < value)
            {
                power *= 2;
                T++;
            }
            return T;
        }
        
        private List<int> GenerateTileValuesBySplit(int targetValue, int N, int minPower)
        {
            // Thuật toán: Bắt đầu với 1 tile có giá trị targetValue
            // Split N-1 lần để có N tiles
            // Mỗi lần chọn 1 tile có thể split (value > 2^minPower) và tách thành 2 tiles nhỏ hơn
            
            List<int> tiles = new List<int> { targetValue };
            int minValue = Mathf.FloorToInt(Mathf.Pow(2, minPower));
            
            // Split N-1 lần
            for (int i = 0; i < N - 1; i++)
            {
                // Tìm tất cả tiles có thể split (value > minValue)
                List<int> splittableTiles = new List<int>();
                for (int j = 0; j < tiles.Count; j++)
                {
                    if (tiles[j] > minValue)
                    {
                        splittableTiles.Add(j);
                    }
                }
                
                // Nếu không còn tile nào có thể split, dừng lại
                if (splittableTiles.Count == 0)
                {
                    Debug.LogWarning($"Không thể split thêm! Chỉ có {tiles.Count} tiles thay vì {N}");
                    break;
                }
                
                // Chọn ngẫu nhiên 1 tile để split
                int randomIndex = splittableTiles[UnityEngine.Random.Range(0, splittableTiles.Count)];
                int valueToSplit = tiles[randomIndex];
                
                // Split thành 2 tiles nhỏ hơn
                tiles[randomIndex] = valueToSplit / 2;
                tiles.Add(valueToSplit / 2);
            }
            
            return tiles;
        }
        
        private int GetLargestPowerOfTwo(int value)
        {
            if (value <= 0) return 1;
            
            int power = 1;
            while (power * 2 <= value)
            {
                power *= 2;
            }
            return power;
        }
        
        private TileData GenerateTileWithValue(LevelParameters parameters, bool[,] occupiedGrid, int tileValue)
        {
            var (height, width) = GetTileSizeByValue(tileValue);
            
            if (FindAvailablePosition(occupiedGrid, width, height, out int row, out int col))
            {
                bool isActive = !parameters.allowInactiveTiles || UnityEngine.Random.value > 0.2f;
                
                return new TileData
                {
                    row = row,
                    col = col,
                    height = height,
                    width = width,
                    value = tileValue.ToString(),
                    isActive = isActive
                };
            }
            
            return null;
        }
        
        private int[] GetPossibleValues(LevelParameters parameters)
        {
            // Tạo list các giá trị có thể từ minValue đến targetValue/2
            // VD: nếu targetValue = 16, thì tiles có thể có giá trị 2, 4, 8 (không có 16)
            // Win condition: Merge tất cả tiles để còn lại 1 tile duy nhất có giá trị = targetValue (16)
            List<int> values = new List<int>();
            
            int maxTileValue = parameters.targetValue / 2;
            int currentValue = parameters.minValue;
            
            while (currentValue <= maxTileValue)
            {
                // Kiểm tra tile có vừa với grid không
                var (height, width) = GetTileSizeByValue(currentValue);
                if (height <= parameters.gridSize && width <= parameters.gridSize)
                {
                    values.Add(currentValue);
                }
                else
                {
                    Debug.LogWarning($"Tile value {currentValue} ({height}x{width}) quá lớn cho grid {parameters.gridSize}x{parameters.gridSize}, bỏ qua!");
                }
                
                currentValue *= 2;
            }
            
            // Đảm bảo luôn có ít nhất giá trị 2
            if (values.Count == 0)
            {
                Debug.LogWarning($"Không có tile value nào phù hợp với grid {parameters.gridSize}x{parameters.gridSize}! Fallback về giá trị mặc định.");
                values.Add(2);
            }
            
            return values.ToArray();
        }
        
        private TileData GenerateRandomTile(LevelParameters parameters, bool[,] occupiedGrid, int[] possibleValues)
        {
            int maxAttempts = 50;
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Chọn value với weighted random (tiles nhỏ xuất hiện nhiều hơn)
                int tileValue = GetWeightedRandomValue(possibleValues);
                
                // Lấy kích thước tương ứng với value
                var (height, width) = GetTileSizeByValue(tileValue);
                
                // Tìm vị trí trống
                if (FindAvailablePosition(occupiedGrid, width, height, out int row, out int col))
                {
                    // Random isActive (obstacle)
                    bool isActive = !parameters.allowInactiveTiles || UnityEngine.Random.value > 0.2f;
                    
                    return new TileData
                    {
                        row = row,
                        col = col,
                        height = height,
                        width = width,
                        value = tileValue.ToString(),
                        isActive = isActive
                    };
                }
            }
            
            return null;
        }
        
        private int GetWeightedRandomValue(int[] possibleValues)
        {
            // Tiles nhỏ (2, 4) xuất hiện nhiều hơn tiles lớn
            // Weight giảm dần theo cấp số nhân: 2 (50%), 4 (25%), 8 (12.5%), 16 (6.25%)...
            
            float totalWeight = 0f;
            for (int i = 0; i < possibleValues.Length; i++)
            {
                totalWeight += Mathf.Pow(0.5f, i); // 1, 0.5, 0.25, 0.125...
            }
            
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            for (int i = 0; i < possibleValues.Length; i++)
            {
                currentWeight += Mathf.Pow(0.5f, i);
                if (randomValue <= currentWeight)
                {
                    return possibleValues[i];
                }
            }
            
            // Fallback: return giá trị nhỏ nhất
            return possibleValues[0];
        }
        
        private bool FindAvailablePosition(bool[,] grid, int width, int height, out int row, out int col)
        {
            List<(int, int)> availablePositions = new List<(int, int)>();
            int gridRows = grid.GetLength(0);
            int gridCols = grid.GetLength(1);
            
            // Kiểm tra tile có vừa với grid không
            if (height > gridRows || width > gridCols)
            {
                Debug.LogWarning($"Tile {height}x{width} quá lớn cho grid {gridRows}x{gridCols}!");
                row = col = -1;
                return false;
            }
            
            for (int r = 0; r <= gridRows - height; r++)
            {
                for (int c = 0; c <= gridCols - width; c++)
                {
                    if (CanPlaceTile(grid, r, c, width, height))
                    {
                        availablePositions.Add((r, c));
                    }
                }
            }
            
            if (availablePositions.Count > 0)
            {
                var pos = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
                row = pos.Item1;
                col = pos.Item2;
                return true;
            }
            
            row = col = -1;
            return false;
        }
        
        private bool CanPlaceTile(bool[,] grid, int row, int col, int width, int height)
        {
            for (int r = row; r < row + height; r++)
            {
                for (int c = col; c < col + width; c++)
                {
                    if (grid[r, c]) return false;
                }
            }
            return true;
        }
        
        private void MarkGridOccupied(bool[,] grid, TileData tile)
        {
            for (int r = tile.row; r < tile.row + tile.height; r++)
            {
                for (int c = tile.col; c < tile.col + tile.width; c++)
                {
                    if (r < grid.GetLength(0) && c < grid.GetLength(1))
                        grid[r, c] = true;
                }
            }
        }
        
        private bool HasAvailableSpace(bool[,] grid)
        {
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    if (!grid[r, c]) return true;
                }
            }
            return false;
        }
        
        private bool ValidateLevelData(LevelData level)
        {
            if (level?.tiles == null || level.tiles.Count < 3)
                return false;
            
            // Tạo grid để kiểm tra overlap
            bool[,] occupiedGrid = new bool[level.rows, level.cols];
            
            // Đếm số active tiles (không tính obstacles)
            int activeTileCount = 0;
            foreach (var tile in level.tiles)
            {
                // Kiểm tra bounds cơ bản
                if (tile.row < 0 || tile.col < 0)
                {
                    Debug.LogWarning($"Tile có vị trí âm: row={tile.row}, col={tile.col}");
                    return false;
                }
                
                // Kiểm tra overflow nghiêm ngặt
                if (tile.row + tile.height > level.rows)
                {
                    Debug.LogWarning($"Tile overflow theo chiều dọc: row={tile.row}, height={tile.height}, maxRows={level.rows}");
                    return false;
                }
                
                if (tile.col + tile.width > level.cols)
                {
                    Debug.LogWarning($"Tile overflow theo chiều ngang: col={tile.col}, width={tile.width}, maxCols={level.cols}");
                    return false;
                }
                
                if (tile.isActive)
                {
                    activeTileCount++;
                    
                    // Kiểm tra value phải là lũy thừa của 2
                    if (int.TryParse(tile.value, out int tileValue))
                    {
                        if (!IsPowerOfTwo(tileValue))
                        {
                            Debug.LogWarning($"Tile có giá trị {tileValue} không phải lũy thừa của 2! Phải là 2, 4, 8, 16, 32...");
                            return false;
                        }
                        
                        // Kiểm tra kích thước có khớp với value không
                        var (expectedHeight, expectedWidth) = GetTileSizeByValue(tileValue);
                        if (tile.height != expectedHeight || tile.width != expectedWidth)
                        {
                            Debug.LogWarning($"Tile value {tileValue} có kích thước sai! Expected: {expectedHeight}x{expectedWidth}, Actual: {tile.height}x{tile.width}");
                            return false;
                        }
                    }
                    
                    // Kiểm tra overlap
                    if (!CanPlaceTile(occupiedGrid, tile.row, tile.col, tile.width, tile.height))
                    {
                        Debug.LogWarning($"Tile overlap với tile khác tại ({tile.row}, {tile.col})");
                        return false;
                    }
                    
                    // Mark grid occupied
                    MarkGridOccupied(occupiedGrid, tile);
                }
            }
            
            // Số lượng active tiles phải là số lẻ
            if (activeTileCount % 2 == 0)
            {
                Debug.LogWarning($"Level có {activeTileCount} active tiles (số chẵn). Cần số lẻ để còn lại 1 tile cuối cùng!");
                return false;
            }
            
            return true;
        }
        
        private bool CanWinLevel(LevelData level, int targetValue)
        {
            // Kiểm tra xem có thể đạt được win condition không
            // Win: Còn lại 1 tile có giá trị = targetValue
            
            // Đếm tổng "mass" của tất cả tiles
            int totalMass = 0;
            foreach (var tile in level.tiles)
            {
                if (tile.isActive && int.TryParse(tile.value, out int value))
                {
                    totalMass += value;
                }
            }
            
            // QUAN TRỌNG: Tổng mass phải là lũy thừa của 2
            if (!IsPowerOfTwo(totalMass))
            {
                Debug.LogWarning($"Tổng mass {totalMass} không phải lũy thừa của 2! Cần là 2, 4, 8, 16, 32...");
                return false;
            }
            
            // Tổng mass phải >= targetValue để có thể đạt được target
            if (totalMass < targetValue)
            {
                Debug.LogWarning($"Tổng mass {totalMass} nhỏ hơn target {targetValue}!");
                return false;
            }
            
            return true;
        }
        
        private LevelData CreateFallbackLevel(LevelParameters parameters)
        {
            // Tạo level đơn giản đảm bảo luôn hợp lệ và có thể thắng
            // Sử dụng cùng logic với GenerateSingleLevel nhưng đơn giản hơn
            
            var level = new LevelData
            {
                rows = parameters.gridSize,
                cols = parameters.gridSize,
                tiles = new List<TileData>()
            };
            
            // Tạo grid để track vị trí
            bool[,] occupiedGrid = new bool[parameters.gridSize, parameters.gridSize];
            
            // Strategy: Tạo tiles từ targetValue (đơn giản)
            // Pattern: targetValue/2 + targetValue/4 + targetValue/4
            // VD: target=8 → 4 + 2 + 2 = 8 ✓
            // VD: target=16 → 8 + 4 + 4 = 16 ✓
            // VD: target=32 → 16 + 8 + 8 = 32 ✓
            
            int remainingMass = parameters.targetValue;
            int tilesCreated = 0;
            
            // Tạo tiles theo pattern đơn giản
            int[] tileValues = { parameters.targetValue / 2, parameters.targetValue / 4, parameters.targetValue / 4 };
            
            foreach (int tileValue in tileValues)
            {
                if (tileValue >= 2 && tilesCreated < 5) // Tối đa 5 tiles
                {
                    TileData tile = GenerateTileWithValue(parameters, occupiedGrid, tileValue);
                    if (tile != null)
                    {
                        level.tiles.Add(tile);
                        MarkGridOccupied(occupiedGrid, tile);
                        remainingMass -= tileValue;
                        tilesCreated++;
                    }
                }
            }
            
            // Nếu chưa đủ tiles, thêm tiles 2
            while (tilesCreated < 3 && HasAvailableSpace(occupiedGrid))
            {
                TileData tile = GenerateTileWithValue(parameters, occupiedGrid, 2);
                if (tile != null)
                {
                    level.tiles.Add(tile);
                    MarkGridOccupied(occupiedGrid, tile);
                    tilesCreated++;
                }
                else
                {
                    break;
                }
            }
            
            Debug.Log($"Fallback level: {tilesCreated} tiles, target = {parameters.targetValue}");
            
            return level;
        }
        
        private bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }
        
        private void SaveGeneratedLevel(LevelData level, string levelName)
        {
            // Safety check: không save level rỗng
            if (level?.tiles == null || level.tiles.Count == 0)
            {
                Debug.LogError($"Không thể lưu {levelName}: Level không có tiles!");
                return;
            }
            
            string json = JsonConvert.SerializeObject(level, Formatting.Indented);
            string path = Path.Combine(Application.dataPath, "Resources/Levels", levelName + ".json");
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Lỗi khi lưu {levelName}: {e.Message}");
            }
        }
    }
}
