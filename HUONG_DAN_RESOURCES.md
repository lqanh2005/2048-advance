# Hướng Dẫn Load Level Từ Resources

## 🎯 Ưu Điểm Của Resources

- ✅ Không cần đường dẫn phức tạp
- ✅ Hoạt động trên mọi nền tảng (PC, Mobile, WebGL)
- ✅ File được build vào game, không cần StreamingAssets
- ✅ Load nhanh hơn StreamingAssets

---

## 📁 Cấu Trúc Thư Mục

Đặt file JSON vào:
```
Assets/
  └── Resources/
      └── Levels/
          ├── level_1.json
          ├── level_2.json
          └── level_3.json
```

⚠️ **Lưu ý:** Thư mục phải tên chính xác là **`Resources`** (chữ R viết hoa)

---

## 💻 Cách Sử Dụng

### Cách 1: Load Trong Script

```csharp
public class GamePlayCtrl : MonoBehaviour
{
    public BoardManager boardManager;
    
    void Start()
    {
        // Khởi tạo board trước
        boardManager.Init();
        
        // Load level 1 (KHÔNG CẦN .json extension)
        boardManager.SpawnLevelFromJson("level_1");
        
        // HOẶC có thể để .json cũng được
        // boardManager.SpawnLevelFromJson("level_1.json");
    }
}
```

### Cách 2: Load Level Theo Số Thứ Tự

```csharp
public class LevelManager : MonoBehaviour
{
    public BoardManager boardManager;
    private int currentLevel = 1;
    
    void Start()
    {
        boardManager.Init();
        LoadLevel(currentLevel);
    }
    
    public void LoadLevel(int levelNumber)
    {
        boardManager.SpawnLevelFromJson($"level_{levelNumber}");
    }
    
    public void NextLevel()
    {
        currentLevel++;
        LoadLevel(currentLevel);
    }
    
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }
}
```

### Cách 3: Gọi Trực Tiếp Từ BoardManager

Sửa method `Init()` trong `BoardManager.cs`:

```csharp
public void Init()
{
    InitBoard();
    SpawnLevelFromJson("level_1");  // Load level 1 mặc định
}
```

Rồi trong script khác chỉ cần gọi:

```csharp
void Start()
{
    boardManager.Init();  // Sẽ tự động load level_1
}
```

---

## 🎮 Ví Dụ Hoàn Chỉnh - GamePlayCtrl

Tạo script mới `GamePlayCtrl.cs`:

```csharp
using UnityEngine;

public class GamePlayCtrl : MonoBehaviour
{
    public BoardManager boardManager;
    public LevelLoader levelLoader;
    
    [Header("Level Settings")]
    public int startLevel = 1;
    private int currentLevel;
    
    void Start()
    {
        // Gán LevelLoader vào BoardManager
        if (boardManager.levelLoader == null)
        {
            boardManager.levelLoader = levelLoader;
        }
        
        // Khởi tạo và load level đầu tiên
        boardManager.Init();
        LoadLevel(startLevel);
    }
    
    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        boardManager.SpawnLevelFromJson($"level_{levelNumber}");
        Debug.Log($"Đã load Level {levelNumber}");
    }
    
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }
    
    public void NextLevel()
    {
        LoadLevel(currentLevel + 1);
    }
    
    public void PreviousLevel()
    {
        if (currentLevel > 1)
        {
            LoadLevel(currentLevel - 1);
        }
    }
}
```

---

## 📋 Cấu Trúc File JSON

Tương tự như trước, nhưng giờ đặt trong `Assets/Resources/Levels/`:

```json
{
    "rows": 5,
    "cols": 5,
    "tiles": [
        {
            "row": 0,
            "col": 0,
            "height": 2,
            "width": 2,
            "value": "8",
            "isActive": true
        },
        {
            "row": 0,
            "col": 2,
            "height": 1,
            "width": 2,
            "value": "4",
            "isActive": false
        }
    ]
}
```

---

## 🔄 So Sánh StreamingAssets vs Resources

| Tính năng | StreamingAssets | Resources |
|-----------|-----------------|-----------|
| **Đường dẫn** | Phức tạp (tùy platform) | Đơn giản |
| **Load cách** | File.ReadAllText | Resources.Load |
| **Extension** | Cần `.json` | Không cần `.json` |
| **Sửa file** | Có thể sửa sau khi build | Không thể sửa sau build |
| **Kích thước** | File riêng biệt | Build vào game |
| **Tốc độ** | Chậm hơn một chút | Nhanh hơn |

**Khuyến nghị:**
- Dùng **Resources** cho game release (đơn giản, nhanh)
- Dùng **StreamingAssets** nếu cần sửa file JSON sau khi build

---

## ⚙️ Setup Trong Unity

### 1. Tạo Hierarchy
```
GamePlay (Scene)
├── Canvas
│   └── BoardManager (có script BoardManager)
├── LevelLoader (có script LevelLoader)  
└── GamePlayCtrl (có script GamePlayCtrl)
```

### 2. Gán References

**Trong GamePlayCtrl:**
- Kéo `BoardManager` vào field **Board Manager**
- Kéo `LevelLoader` vào field **Level Loader**

**Trong BoardManager:**
- Kéo `LevelLoader` vào field **Level Loader**

### 3. Đặt File JSON

Đặt tất cả file JSON vào:
```
Assets/Resources/Levels/
```

### 4. Chạy Game

Unity sẽ tự động load level khi Start!

---

## 🐛 Debug

### Lỗi: "Không tìm thấy file trong Resources"

**Nguyên nhân:**
- Thư mục không tên là `Resources`
- File không có trong `Resources/Levels/`
- Đã ghi sai tên file

**Giải pháp:**
```csharp
// Kiểm tra file có tồn tại không
TextAsset test = Resources.Load<TextAsset>("Levels/level_1");
if (test == null)
{
    Debug.LogError("File không tồn tại!");
}
else
{
    Debug.Log("File OK: " + test.text);
}
```

### Lỗi: "LevelLoader chưa được gán"

**Giải pháp:**
- Kiểm tra trong Inspector đã gán `LevelLoader` chưa
- Hoặc tự động tìm trong code:

```csharp
if (levelLoader == null)
{
    levelLoader = FindObjectOfType<LevelLoader>();
}
```

---

## 📝 Ví Dụ File JSON Mẫu

Tôi đã tạo sẵn 2 file mẫu:
- ✅ `Assets/Resources/Levels/level_1.json` - Bảng 5x5 đơn giản
- ✅ `Assets/Resources/Levels/level_2.json` - Bảng 6x6 phức tạp hơn

Bạn có thể copy và tạo thêm `level_3.json`, `level_4.json`, v.v...

---

## 🎨 Tips

### Load Ngẫu Nhiên
```csharp
int randomLevel = Random.Range(1, 10);  // Level 1-9
boardManager.SpawnLevelFromJson($"level_{randomLevel}");
```

### Load Tất Cả Level
```csharp
TextAsset[] allLevels = Resources.LoadAll<TextAsset>("Levels");
Debug.Log($"Có {allLevels.Length} levels");
```

### Kiểm Tra Level Tồn Tại
```csharp
public bool LevelExists(int levelNumber)
{
    TextAsset level = Resources.Load<TextAsset>($"Levels/level_{levelNumber}");
    return level != null;
}
```

---

Chúc bạn code vui vẻ! 🎉


