# 🚀 QUICK START - Load Level Từ Resources

## Các Bước Nhanh

### 1️⃣ Đặt File JSON
Đặt file JSON vào: **`Assets/Resources/Levels/`**

Ví dụ:
- `Assets/Resources/Levels/level_1.json` ✅
- `Assets/Resources/Levels/level_2.json` ✅

### 2️⃣ Setup Trong Unity

**Tạo GameObject:**
1. Tạo Empty GameObject tên "LevelLoader"
2. Add Component → `LevelLoader`

**Gán References trong GamePlayCtrl:**
1. Chọn GameObject có `GamePlayCtrl`
2. Kéo `LevelLoader` vào field **Level Loader**
3. Kéo `BoardManager` vào field **Board Manager**

**Gán References trong BoardManager:**
1. Chọn GameObject có `BoardManager`
2. Kéo `LevelLoader` vào field **Level Loader**

### 3️⃣ Chạy Game

Game sẽ **TỰ ĐỘNG** load `level_1.json` khi Start!

---

## 📝 Cấu Trúc File JSON

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
        }
    ]
}
```

---

## 🎮 Các Methods Có Sẵn

```csharp
// Load level cụ thể
GamePlayCtrl.Instance.LoadLevel(2);  // Load level_2

// Restart level hiện tại
GamePlayCtrl.Instance.RestartLevel();

// Next level
GamePlayCtrl.Instance.NextLevel();

// Previous level
GamePlayCtrl.Instance.PreviousLevel();

// Load random level
GamePlayCtrl.Instance.LoadRandomLevel(1, 5);  // Random từ level 1-5

// Lấy level hiện tại
int current = GamePlayCtrl.Instance.GetCurrentLevel();
```

---

## ✅ File Đã Tạo Sẵn

- ✅ `Assets/Resources/Levels/level_1.json` (Bảng 5x5)
- ✅ `Assets/Resources/Levels/level_2.json` (Bảng 6x6)

Copy 2 file này để tạo `level_3.json`, `level_4.json`, ...

---

## ⚙️ Thay Đổi Level Bắt Đầu

Trong Inspector của `GamePlayCtrl`:
- **Start Level** = 1 → Bắt đầu từ level 1
- **Start Level** = 3 → Bắt đầu từ level 3

---

## 🔍 Kiểm Tra Console

Khi chạy game, Console sẽ hiển thị:
- ✅ `"Load level từ Resources thành công: level_1"`
- ✅ `"Đã spawn 4 tiles từ level_1"`
- ✅ `"✅ Đã load Level 1"`

Nếu thấy thông báo này → **Thành công!** 🎉

---

## 📚 Xem Thêm

- `HUONG_DAN_RESOURCES.md` - Hướng dẫn chi tiết đầy đủ


