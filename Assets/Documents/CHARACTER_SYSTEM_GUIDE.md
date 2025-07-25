# 📋 Character System Guide - Cập nhật sau tối ưu hóa

## 🎯 Tình trạng hiện tại (Đã tối ưu)

**Hệ thống UI đã được tối ưu hóa hoàn toàn với BattleGameManager làm trung tâm!**

### ✅ Hệ thống hiện tại:

1. **🎮 BattleGameManager System (Active)**
   - **Chức năng chính**: Character selection + Team selection + Drag & drop
   - **Status**: ✅ Hoạt động ổn định
   - **File**: `Assets/Scripts/BattleGameManager.cs`

2. **🗑️ Removed Systems (Đã xóa)**
   - ❌ TeamSelectionHandler (Đã xóa)
   - ❌ TeamSelectionFix (Đã xóa) 
   - ❌ SimpleFix (Đã xóa)
   - ❌ 6 test/debug scripts khác (Đã xóa)

### 🚀 Tính năng hoạt động:

- ✅ **Character Selection** - Click để chọn character từ danh sách
- ✅ **Team Selection** - Click team buttons (Blue/Red)
- ✅ **Drag & Drop Placement** - Kéo character vào map
- ✅ **Real-time Preview** - Preview với team colors
- ✅ **Unlimited Spawning** - Không giới hạn số lượng character
- ✅ **Battle Management** - Start/Reset battle functionality

### 🔧 Cách sử dụng nhanh:

1. **Setup Mode**: Game tự động vào setup mode khi start
2. **Chọn Team**: Click "Team 1 (Blue)" hoặc "Team 2 (Red)"
3. **Chọn Character**: Click character từ danh sách bên trái
4. **Đặt Character**: Drag character từ UI vào map
5. **Start Battle**: Click "Start" để bắt đầu chiến đấu

---

## ⚠️ Legacy Information (Deprecated)

**The following information is for reference only. Please use the new system.**

### 3. Enhanced Character Selection UI
- **Component**: `EnhancedCharacterSelectionUI`
- **Chức năng**: Tạo UI drag-and-drop tự động từ database
- **Tính năng**:
  - Hiển thị categories ở panel trái
  - Hiển thị characters ở panel dưới
  - Team selection
  - Tự động refresh khi database thay đổi

## Cách Sử Dụng

### Thêm Nhân Vật Mới

#### Phương Pháp 1: Sử dụng Character Setup Tool
1. Mở `Tools > Character Setup Tool`
2. Chọn model trong Project hoặc Scene
3. Điền thông tin: Name, Type, Icon, Stats
4. Chọn components cần thêm
5. Chọn Character Database
6. Click "Setup Character"

#### Phương Pháp 2: Thêm trực tiếp vào Database
1. Chọn `CharacterDatabase.asset` trong Project
2. Trong Inspector, sử dụng "Add New Character" section
3. Kéo thả Prefab và Icon
4. Điền Name và chọn Type
5. Click "Add Character to Database"

#### Phương Pháp 3: Batch Add
1. Chọn nhiều GameObjects trong Project
2. Trong Character Setup Tool, click "Setup Selected Objects"
3. Hoặc trong Database Inspector, click "Add All Selected Objects"

### Quản Lý Categories

#### Tạo Category Mới
1. Trong Database Inspector, click "Add New Category"
2. Hoặc sử dụng "Initialize Default Categories" để tạo categories mặc định

#### Chỉnh Sửa Category
1. Mở Database Inspector
2. Expand category cần chỉnh sửa
3. Thay đổi Name, Type, Color, Icon
4. Thêm/xóa characters trong category

### Chỉnh Sửa Nhân Vật
1. Trong Database Inspector, tìm character cần chỉnh sửa
2. Click button "Edit" bên cạnh character
3. Cửa sổ Character Edit sẽ mở
4. Chỉnh sửa thông tin và click "Save"

### Sử Dụng UI trong Game
1. Đảm bảo `EnhancedCharacterSelectionUI` component đã được gán `CharacterDatabase`
2. UI sẽ tự động tạo categories và character buttons
3. Click category để xem characters
4. Click team dropdown để chọn team
5. Click character để spawn hoặc drag-and-drop

## Cấu Trúc File

```
Assets/
├── CharacterDatabase.asset          # Database chính
├── Scripts/
│   ├── CharacterDatabase.cs         # ScriptableObject definition
│   ├── EnhancedCharacterSelectionUI.cs  # UI Manager
│   ├── CreateCharacterDatabase.cs   # Utility script
│   └── Editor/
│       ├── CharacterSetupTool.cs    # Setup tool window
│       └── CharacterDatabaseEditor.cs # Custom inspector
└── Prefabs/                         # Character prefabs
    ├── Character1.prefab
    ├── Character2.prefab
    └── ...
```

## Tính Năng Nâng Cao

### Auto-Setup Components
Tool tự động thêm các components cần thiết:
- **RagdollCharacter**: Script điều khiển nhân vật
- **NavMeshAgent**: AI pathfinding
- **Animator**: Animation controller
- **Colliders**: Collision detection
- **Rigidbody**: Physics

### Reflection-Based Integration
Hệ thống sử dụng reflection để tương thích với các script hiện có:
- Tự động detect và sử dụng `CharacterDragSource` nếu có
- Tự động gọi `SpawnCharacter` method nếu có
- Không bị lỗi nếu script không tồn tại

### Database Statistics
Inspector hiển thị thống kê:
- Tổng số categories
- Tổng số characters
- Phân bố theo type

## Mở Rộng

### Thêm Character Type Mới
1. Mở `CharacterDatabase.cs`
2. Thêm type mới vào enum `CharacterType`
3. Rebuild project

### Tùy Chỉnh UI Layout
1. Mở `EnhancedCharacterSelectionUI.cs`
2. Chỉnh sửa các method `CreateLeftPanel`, `CreateBottomPanel`
3. Thay đổi `charactersPerRow`, `buttonSpacing`, etc.

### Thêm Properties Mới
1. Thêm field vào `CharacterInfo` class
2. Cập nhật `CharacterEditWindow` để hiển thị field mới
3. Cập nhật `CharacterSetupTool` nếu cần

## Troubleshooting

### Database không hiển thị
- Đảm bảo `CharacterDatabase.asset` tồn tại
- Gán database vào `EnhancedCharacterSelectionUI` component
- Click "Find Database" trong Setup Tool

### UI không tự động refresh
- Click "Refresh UI" trong Database Inspector
- Hoặc gọi `RefreshDatabase()` method từ code

### Character không spawn
- Đảm bảo prefab có các component cần thiết
- Kiểm tra `BattleGameManager` có method `SpawnCharacter`
- Kiểm tra console log để debug

## Kết Luận
Hệ thống này giúp bạn:
- ✅ Dễ dàng thêm nhân vật mới chỉ bằng drag-and-drop
- ✅ Quản lý nhân vật theo categories
- ✅ Tự động setup components và scripts
- ✅ UI tự động cập nhật khi thêm nhân vật
- ✅ Tương thích với code hiện có
- ✅ Dễ mở rộng và tùy chỉnh