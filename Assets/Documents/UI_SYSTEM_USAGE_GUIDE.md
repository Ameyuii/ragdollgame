# 🎮 Hướng dẫn sử dụng UI System - Sau tối ưu hóa

## 📋 Tổng quan

Sau khi tối ưu hóa, project sử dụng **hệ thống UI thống nhất** với BattleGameManager làm trung tâm. Không còn xung đột giữa các hệ thống UI khác nhau.

## 🎯 Hệ thống hiện tại

### **BattleGameManager - Hệ thống chính**
- **File**: `Assets/Scripts/BattleGameManager.cs`
- **Chức năng**: Character selection + Team selection + Drag & drop + Battle management
- **Trạng thái**: ✅ Hoạt động ổn định

### **Đã loại bỏ (9 scripts)**
- ❌ TeamSelectionHandler.cs
- ❌ TeamSelectionFix.cs  
- ❌ SimpleFix.cs
- ❌ DebugTeamSelection.cs
- ❌ FinalTeamTest.cs
- ❌ SimpleDebugTeam.cs
- ❌ TestTeamSelection.cs
- ❌ TestTeamUI.cs
- ❌ TestDragDropWithTeam.cs

## 🚀 Cách sử dụng từng bước

### **Bước 1: Khởi động game**
1. Mở Unity Editor
2. Load scene `backup.unity`
3. Click Play
4. Game tự động vào **Setup Mode**

### **Bước 2: Chọn team**
1. Nhìn vào panel bên trái màn hình
2. Click **"Team 1 (Blue)"** hoặc **"Team 2 (Red)"**
3. Button sẽ highlight để cho biết team đã chọn
4. Text sẽ hiển thị team hiện tại

### **Bước 3: Chọn character**
1. Scroll xuống để xem danh sách character
2. Mỗi character có:
   - **Preview image** (bên trái)
   - **Tên character** (bên phải trên)
   - **Thông tin stats** (bên phải dưới)
3. Click vào character muốn chọn
4. Character sẽ được highlight

### **Bước 4: Đặt character vào map**
1. **Drag** character button từ panel UI
2. **Kéo** ra map (khu vực 3D)
3. Trong lúc drag:
   - Sẽ xuất hiện **preview 3D** với màu team
   - Preview sẽ follow chuột
4. **Thả** tại vị trí muốn đặt character
5. Character sẽ spawn với:
   - ✅ Đúng model
   - ✅ Đúng màu team  
   - ✅ Đúng vị trí

### **Bước 5: Lặp lại để tạo army**
1. Chọn team khác (nếu muốn)
2. Chọn character khác (nếu muốn) 
3. Drag & drop thêm character
4. **Không giới hạn** số lượng character

### **Bước 6: Bắt đầu battle**
1. Click **"Start Battle"** button
2. Game chuyển sang **Battle Mode**
3. Các character sẽ tự động chiến đấu
4. UI sẽ hiển thị:
   - Team counters
   - Battle status
   - Victory condition

### **Bước 7: Reset (nếu cần)**
1. Click **"Reset Battle"** button
2. Tất cả character sẽ bị xóa
3. Game quay về **Setup Mode**
4. Có thể setup lại từ đầu

## 🔧 Troubleshooting

### **❌ Character không spawn khi drag**
**Nguyên nhân**: Chưa chọn character hoặc team
**Giải pháp**: 
1. Đảm bảo đã chọn team (button highlight)
2. Đảm bảo đã chọn character (button highlight)
3. Drag từ chính button character đó

### **❌ Character spawn sai màu team**
**Nguyên nhân**: Team selection không sync
**Giải pháp**:
1. Click lại team button để refresh
2. Chọn character lại
3. Nếu vẫn lỗi, reset battle và thử lại

### **❌ UI không hiển thị character**
**Nguyên nhân**: Character prefabs không được assign
**Giải pháp**:
1. Check BattleGameManager component trong scene
2. Assign character prefabs vào `characterPrefabs` array
3. Hoặc để game tự động load từ Resources

### **❌ Drag không hoạt động**
**Nguyên nhân**: Drag state bị stuck
**Giải pháp**:
1. Sử dụng OptimizationTest: Right-click → "Reset All Drag States"
2. Hoặc reload scene

## 🧪 Testing & Debug

### **OptimizationTest.cs** - Script test tổng thể
**Location**: `Assets/Scripts/OptimizationTest.cs`

**Context Menu Options:**
1. **"Test All Systems"** - Kiểm tra toàn bộ hệ thống
2. **"Test Character Spawning"** - Test spawn character
3. **"Reset All Drag States"** - Reset drag system

### **Debug Console Messages**
Game sẽ log thông tin chi tiết:
```
[DRAG DEBUG] OnBeginDrag called on CharacterButton_0
[DRAG DEBUG] Successfully started dragging Character_Name
[DRAG DEBUG] Drop position: (0.0, 0.1, 5.0)
[DRAG DEBUG] Using team from BattleGameManager: 1
[DRAG DEBUG] Successfully dropped character at position for team 1
```

### **Visual Indicators**
- **Team buttons**: Highlight khi chọn
- **Character buttons**: Highlight khi chọn
- **Drag preview**: 3D capsule với màu team
- **Status text**: Hiển thị current mode

## ⚙️ Advanced Usage

### **Custom Character Setup**
1. Tạo character prefab mới
2. Assign vào `BattleGameManager.characterPrefabs[]`
3. Game sẽ tự động tạo UI button

### **Team Colors Customization**
1. Tạo material mới trong `Assets/Resources/`
2. Đặt tên `Team1_Blue.mat`, `Team2_Red.mat`, etc.
3. Game sẽ tự động apply khi spawn

### **Performance Optimization**
- Game sử dụng **preview generation** cache
- **Drag system** tối ưu với object pooling
- **UI elements** được reuse thay vì tạo mới

## 📞 Support

**Nếu gặp vấn đề:**
1. Check Unity Console để xem error messages
2. Sử dụng OptimizationTest để diagnose
3. Xem CLAUDE.md để hiểu architecture
4. Reset scene và thử lại

**File quan trọng:**
- `BattleGameManager.cs` - Main UI logic
- `CharacterDragSource.cs` - Drag & drop logic  
- `OptimizationTest.cs` - Testing utilities
- `CLAUDE.md` - Architecture documentation