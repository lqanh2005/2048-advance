# 📐 Hướng Dẫn Auto-Size Tile Theo Value

## 🎯 Quy Ước Kích Thước

Khi merge, tile sẽ **TỰ ĐỘNG** thay đổi kích thước theo value mới:

| Value | Kích Thước (W×H) | Diện Tích | Ghi Chú |
|-------|------------------|-----------|---------|
| **2** | 1×1 | 1 | Tile nhỏ nhất |
| **4** | 1×2 | 2 | Dọc |
| **8** | 2×1 | 2 | Ngang |
| **16** | 2×2 | 4 | Vuông |
| **32** | 2×3 | 6 | |
| **64** | 3×2 | 6 | |
| **128** | 3×3 | 9 | |
| **256** | 3×4 | 12 | |
| **512** | 4×3 | 12 | |
| **1024** | 4×4 | 16 | |

---

## 🔄 Merge Tự Động Resize

### Ví Dụ 1: 2 + 2 = 4
```
Before:  [2] (1x1)  +  [2] (1x1)
After:   [4] (1x2)
```
✅ Tile tự động đổi từ 1×1 → 1×2

### Ví Dụ 2: 4 + 4 = 8
```
Before:  [4] (1x2)  +  [4] (1x2)
After:   [8] (2x1)
```
✅ Tile tự động đổi từ 1×2 → 2×1

### Ví Dụ 3: 8 + 8 = 16
```
Before:  [8] (2x1)  +  [8] (2x1)
After:   [16] (2x2)
```
✅ Tile tự động đổi từ 2×1 → 2×2

---

## 📝 Cách Viết File JSON

### Cách 1: Auto-Size (Đơn Giản - Khuyến Nghị)

Chỉ cần set `width: 0` và `height: 0`, hệ thống sẽ tự động tính:

```json
{
    "rows": 6,
    "cols": 6,
    "tiles": [
        {
            "row": 0,
            "col": 0,
            "height": 0,
            "width": 0,
            "value": "2",
            "isActive": true
        },
        {
            "row": 0,
            "col": 2,
            "height": 0,
            "width": 0,
            "value": "4",
            "isActive": false
        }
    ]
}
```

**Kết quả:**
- Tile value `2` → Auto resize thành 1×1
- Tile value `4` → Auto resize thành 1×2

### Cách 2: Manual Size (Tùy Chỉnh)

Nếu muốn kích thước khác quy ước, viết rõ:

```json
{
    "row": 0,
    "col": 0,
    "height": 3,
    "width": 2,
    "value": "16",
    "isActive": false
}
```

**Kết quả:**
- Tile value `16` sẽ có kích thước 2×3 (không theo quy ước 2×2)

---

## 🎮 Xử Lý Khi Không Đủ Chỗ

### Trường Hợp 1: Đủ Chỗ
```
Board: 6x6
Merge tại (2,2): 4+4 → 8 (cần 2x1)

✅ Vị trí (2,2) đủ chỗ
   → Đặt tile tại đó
```

### Trường Hợp 2: Không Đủ Chỗ
```
Board: 6x6
Merge tại (5,5): 4+4 → 8 (cần 2x1)

⚠️ Vị trí (5,5) không đủ chỗ cho 2x1
   → Tìm vị trí gần nhất
   → Di chuyển tile đến vị trí đó
```

### Trường Hợp 3: Không Tìm Được Vị Trí
```
Board: 4x4 (gần đầy)
Merge: 32+32 → 64 (cần 3x2)

❌ Board không đủ lớn
   → Giữ nguyên kích thước cũ (2x3)
   → Chỉ update value thành 64
```

---

## 🔧 Tùy Chỉnh Quy Ước

Bạn có thể thay đổi quy ước trong `Tile.cs`, method `GetSizeFromValue()`:

```csharp
public static (int width, int height) GetSizeFromValue(int value)
{
    switch (value)
    {
        case 2:   return (1, 1);  // ← Thay đổi tại đây
        case 4:   return (1, 2);
        case 8:   return (2, 1);
        case 16:  return (2, 2);
        case 32:  return (2, 3);  // ← Ví dụ: đổi thành (3, 2)
        case 64:  return (3, 2);
        case 128: return (3, 3);
        // Thêm case mới nếu cần...
        default:
            // Công thức tự động cho giá trị khác
            int level = Mathf.FloorToInt(Mathf.Log(value, 2)) - 1;
            int w = 1 + (level / 2);
            int h = 1 + ((level + 1) / 2);
            return (w, h);
    }
}
```

---

## 📊 Debug & Console Log

Khi merge, Console sẽ hiển thị:

```
🔄 Merge: 2 + 2 = 4 | Size: 1x1 -> 1x2
✅ Tile 4 đặt tại (0, 2)
```

```
🔄 Merge: 8 + 8 = 16 | Size: 2x1 -> 2x2
⚠️ Không đủ chỗ tại (5, 5) cho tile 16 (2x2)
✅ Tile 16 di chuyển đến (3, 3)
```

```
🔄 Merge: 32 + 32 = 64 | Size: 2x3 -> 3x2
❌ Không tìm được vị trí cho tile 64! Giữ kích thước cũ.
```

---

## 🎨 Tips

### 1. Test Auto-Size
```csharp
// Trong Unity Console hoặc Inspector
var size = Tile.GetSizeFromValue(16);
Debug.Log($"Value 16: {size.width}x{size.height}");
// Output: Value 16: 2x2
```

### 2. Thiết Kế Board Lớn Hơn
Nếu muốn merge đến giá trị cao (128, 256, ...), đảm bảo board đủ lớn:
- Value 128 (3×3) → Cần board tối thiểu 3×3
- Value 256 (3×4) → Cần board tối thiểu 3×4
- Value 1024 (4×4) → Cần board tối thiểu 4×4

### 3. Load Level Test
```csharp
// Load level test auto-size
GamePlayCtrl.Instance.LoadLevel("level_auto");
```

---

## 📁 File Mẫu

Tôi đã tạo file mẫu: **`Assets/Resources/Levels/level_auto.json`**

File này có:
- 5 tiles với value 2, 2, 4, 4, 8
- Tất cả đều dùng auto-size (width=0, height=0)
- Board 6×6 đủ lớn để test merge

---

## ✅ Checklist

- ✅ Tile tự động resize sau merge
- ✅ Hệ thống tìm vị trí mới nếu không đủ chỗ
- ✅ Fallback giữ nguyên size nếu không tìm được vị trí
- ✅ Hỗ trợ auto-size từ JSON (width=0, height=0)
- ✅ Hỗ trợ manual size từ JSON
- ✅ Console log rõ ràng cho debugging

---

Chúc bạn code vui vẻ! 🎉


