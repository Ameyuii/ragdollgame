# Hướng Dẫn Sử Dụng Character Manager

## Tổng Quan
Character Manager là hệ thống quản lý nhân vật trong Inspector của Unity, cho phép bạn dễ dàng thêm/xóa/chỉnh sửa nhân vật mà không cần code.

## Cách Sử Dụng

### 1. Thiết Lập Ban Đầu

1. **Thêm CharacterManager Component**:
   - Chọn GameObject cần quản lý nhân vật
   - Add Component → CharacterManager

2. **Khởi Tạo Categories**:
   - Trong Inspector, click "Initialize Default Categories"
   - Hoặc right-click component → "Initialize Default Categories"

### 2. Quản Lý Categories

#### Tạo Category Mới:
- Click "Add New Category" trong Inspector
- Hoặc right-click component → "Add New Category"

#### Chỉnh Sửa Category:
- Expand category trong Inspector
- Thay đổi tên, màu sắc, icon
- Click nút "×" để xóa category

### 3. Thêm Nhân Vật

#### Phương Pháp 1: Sử Dụng "Add New Character" Section
1. Expand "Add New Character" trong Inspector
2. Kéo thả Prefab vào "Character Prefab"
3. Kéo thả Icon vào "UI Icon" (optional)
4. Điền "Character Name" (tự động từ prefab name)
5. Chọn "Target Category"
6. Click "Add Character"

#### Phương Pháp 2: Batch Add
1. Chọn nhiều GameObjects trong Project
2. Trong Inspector, click "Add All Selected Objects"
3. Tất cả objects sẽ được thêm vào category đã chọn

#### Phương Pháp 3: Programmatic
```csharp
CharacterEntry newCharacter = new CharacterEntry();
newCharacter.characterName = "My Character";
newCharacter.prefab = myPrefab;
newCharacter.health = 100;

characterManager.AddCharacterToCategory("🪖 CHIẾN BINH", newCharacter);
```

### 4. Chỉnh Sửa Nhân Vật

#### Trong Inspector:
- Expand category → expand character
- Chỉnh sửa trực tiếp: Name, Prefab, Stats
- Click "Edit" để mở cửa sổ chỉnh sửa chi tiết
- Click "×" để xóa character

#### Cửa Sổ Edit:
- Chỉnh sửa đầy đủ thông tin character
- Bao gồm: Stats, Visual, Description
- Click "Save" để lưu thay đổi

### 5. Tools & Utilities

#### Auto Setup All Prefabs:
- Tự động kiểm tra và setup components cho prefabs
- Thêm RagdollCharacter, NavMeshAgent nếu thiếu

#### Validate Character Data:
- Kiểm tra tính hợp lệ của dữ liệu
- Báo cáo các vấn đề: prefab null, tên trống, stats không hợp lệ

#### Print Statistics:
- Hiển thị thống kê trong Console
- Tổng số categories, characters, phân bố theo category

#### Open Character Setup Tool:
- Mở Character Setup Tool để setup prefabs nâng cao

### 6. Tự Động Hóa

#### Auto Refresh UI:
- Bật "Auto Refresh UI" để tự động cập nhật UI khi thay đổi
- UI sẽ tự động sync với dữ liệu trong Inspector

#### Auto Setup Prefabs:
- Bật "Auto Setup Prefabs" để tự động kiểm tra components
- Cảnh báo khi prefabs thiếu components cần thiết

## Tính Năng Nổi Bật

### ✅ **Quản Lý Trực Quan**
- Tất cả trong Inspector, không cần code
- Drag & drop dễ dàng
- Visual feedback với màu sắc

### ✅ **Tự Động Sync với UI**
- Thay đổi trong Inspector → UI tự động cập nhật
- Không cần refresh thủ công

### ✅ **Batch Operations**
- Thêm nhiều characters cùng lúc
- Auto setup tất cả prefabs
- Validate toàn bộ dữ liệu

### ✅ **Context Menu Support**
- Right-click component để truy cập nhanh functions
- Không cần mở Inspector

### ✅ **Programmatic Access**
```csharp
// Get all characters
var allCharacters = characterManager.GetAllCharacters();

// Get characters from category
var soldiers = characterManager.GetCharactersFromCategory("🪖 CHIẾN BINH");

// Add character
characterManager.AddCharacterToCategory(categoryName, newCharacter);

// Remove character
characterManager.RemoveCharacter(character);
```

## Workflow Khuyến Nghị

### 1. **Setup Lần Đầu**:
1. Add CharacterManager component
2. Initialize Default Categories
3. Setup UI references (CharacterSelectionUI, BattleGameManager)

### 2. **Thêm Nhân Vật Mới**:
1. Tạo/import model vào Project
2. Sử dụng Character Setup Tool để setup prefab (optional)
3. Drag prefab vào CharacterManager Inspector
4. Điền thông tin và chọn category
5. UI tự động cập nhật

### 3. **Quản Lý Ongoing**:
1. Sử dụng Inspector để chỉnh sửa
2. Validate data định kỳ
3. Auto setup prefabs khi cần
4. Monitor statistics

## Troubleshooting

### UI không cập nhật:
- Kiểm tra "Auto Refresh UI" đã bật
- Click "Refresh UI" thủ công
- Kiểm tra CharacterSelectionUI reference

### Prefab thiếu components:
- Sử dụng "Auto Setup All Prefabs"
- Hoặc "Validate Character Data" để kiểm tra
- Sử dụng Character Setup Tool cho setup chi tiết

### Performance:
- Tắt "Auto Refresh UI" nếu có nhiều characters
- Sử dụng "Refresh UI" thủ công khi cần

## Kết Luận

Character Manager cung cấp:
- ✅ **Quản lý trực quan** trong Inspector
- ✅ **Tự động sync** với UI hiện có
- ✅ **Batch operations** tiết kiệm thời gian
- ✅ **Validation tools** đảm bảo chất lượng
- ✅ **Programmatic access** cho advanced users
- ✅ **Context menu** cho truy cập nhanh

Hệ thống này giúp bạn quản lý nhân vật dễ dàng mà không cần thay đổi UI hiện có!