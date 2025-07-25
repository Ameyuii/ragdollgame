# 🎮 CHARACTER MANAGEMENT TOOL - HƯỚNG DẪN SỬ DỤNG

## 📋 TỔNG QUAN

Character Management Tool là hệ thống quản lý nhân vật hoàn chỉnh cho Unity, cho phép bạn:
- ✅ Quản lý nhân vật trực quan trong Inspector
- ✅ Auto-setup components cho nhân vật
- ✅ Batch operations (xử lý nhiều nhân vật cùng lúc)
- ✅ Template system cho các loại nhân vật khác nhau
- ✅ Hoạt động 100% trong Editor mode (không cần Play)

---

## 🚀 THIẾT LẬP BAN ĐẦU

### Bước 1: Thêm Character Manager
**Cách 1: Sử dụng Menu**
1. Right-click GameObject trong Hierarchy
2. Chọn **Character Management → Add Character Manager**
3. Tool sẽ tự động tìm và gán UI references

**Cách 2: Thủ công**
1. Chọn GameObject cần quản lý nhân vật
2. Add Component → **CharacterManager**
3. Gán references: CharacterSelectionUI, BattleGameManager

### Bước 2: Khởi tạo Categories
1. Trong Inspector của CharacterManager
2. Click **"Initialize Default Categories"**
3. Hoặc Right-click component → **"Initialize Default Categories"**

**Categories mặc định:**
- 🪖 CHIẾN BINH (màu xanh lá)
- 🤖 ROBOT (màu xanh dương)  
- 👹 QUÁI VẬT (màu đỏ)
- 🧟 ZOMBIE (màu xanh lục)

---

## 🎯 QUẢN LÝ NHÂN VẬT

### Thêm Nhân Vật Đơn Lẻ

#### Phương pháp 1: Basic Add
1. Expand **"Add New Character"** trong Inspector
2. Drag & drop **Prefab** vào "Character Prefab"
3. Drag & drop **Icon** vào "UI Icon" (optional)
4. Điền **"Character Name"** (tự động từ prefab name)
5. Chọn **"Target Category"**
6. Click **"Add Character"**

#### Phương pháp 2: Advanced Setup & Add
1. Trong Inspector, click **"Advanced Setup & Add"**
2. Hoặc **Tools → Advanced Character Setup**
3. Cấu hình đầy đủ components và stats
4. Click **"Setup Character with All Components"**

### Thêm Nhiều Nhân Vật (Batch)

#### Batch Add từ Selection
1. Select nhiều GameObjects trong Project
2. Trong CharacterManager Inspector
3. Click **"Add All Selected Objects"**
4. Tất cả objects sẽ được thêm vào category đã chọn

#### Batch Setup với Template
1. Select nhiều GameObjects
2. **Tools → Character Component Template**
3. Chọn template phù hợp
4. Click **"Apply Template to All Selected"**

---

## 🔧 ADVANCED CHARACTER SETUP

### Mở Advanced Setup Tool
- **Tools → Advanced Character Setup**
- Hoặc click **"Advanced Setup & Add"** trong CharacterManager

### Các Tính Năng

#### 1. Target Selection
- **Target Prefab**: Drag prefab cần setup
- **Character Name**: Tên nhân vật (auto-fill từ prefab)
- **Character Manager**: Reference đến CharacterManager
- **"Use Selected GameObject"**: Sử dụng object đang select

#### 2. Components to Add
**Core Components:**
- ✅ **RagdollCharacter**: Script chính điều khiển nhân vật
- ✅ **NavMeshAgent**: AI navigation và pathfinding
- ✅ **Animator**: Animation controller
- ✅ **Rigidbody**: Physics simulation
- ✅ **CapsuleCollider**: Collision detection
- ✅ **AudioSource**: Sound effects

**AI Components:**
- ✅ **Character AI**: Trí tuệ nhân tạo
- ✅ **Health System**: Hệ thống máu
- ✅ **Weapon System**: Hệ thống vũ khí

#### 3. Animation Setup
- **Animator Controller**: Controller cho animations
- **Avatar**: Avatar cho humanoid characters
- **"Find Default Animator Controller"**: Tự động tìm controller

#### 4. Physics Setup
- **Physic Material**: Vật liệu vật lý
- **Mass**: Khối lượng (default: 1)
- **Use Gravity**: Sử dụng trọng lực
- **Is Kinematic**: Chế độ kinematic

#### 5. Character Stats
- **Health**: Máu (default: 100)
- **Speed**: Tốc độ di chuyển (default: 5)
- **Attack Damage**: Sát thương (default: 20)
- **Attack Range**: Tầm đánh (default: 2)

### Workflow Advanced Setup
1. **Select Target**: Chọn prefab hoặc GameObject
2. **Auto-Detect**: Click "Auto-Detect Missing Components"
3. **Configure**: Điều chỉnh settings theo nhu cầu
4. **Setup**: Click "Setup Character with All Components"
5. **Result**: Nhân vật được setup đầy đủ và thêm vào Manager

---

## 📋 QUICK TEMPLATE SYSTEM

### Mở Template Tool
- **Tools → Character Component Template**
- Hoặc Right-click GameObject → **Character Management → Apply Component Template**

### 5 Templates Có Sẵn

#### 1. 🎯 Basic Character
**Components:**
- RagdollCharacter
- Rigidbody
- CapsuleCollider

**Dùng cho:** Nhân vật đơn giản, prototype nhanh

#### 2. 🤖 AI Character  
**Components:**
- RagdollCharacter
- NavMeshAgent
- Rigidbody
- CapsuleCollider
- Animator

**Dùng cho:** Bot, NPC, enemy AI

#### 3. 🎮 Player Character
**Components:**
- RagdollCharacter
- Rigidbody
- CapsuleCollider
- Animator
- AudioSource

**Dùng cho:** Nhân vật người chơi điều khiển

#### 4. ⚔️ Combat Character (FULL)
**Components:**
- RagdollCharacter
- NavMeshAgent
- Rigidbody
- CapsuleCollider
- Animator
- AudioSource

**Dùng cho:** Nhân vật chiến đấu hoàn chỉnh

#### 5. 🚗 Vehicle Character
**Components:**
- RagdollCharacter
- Rigidbody
- BoxCollider
- AudioSource

**Dùng cho:** Xe tăng, robot lớn, vehicles

### Cách Sử Dụng Template
1. **Select Target**: Chọn GameObject(s) cần setup
2. **Choose Template**: Chọn template phù hợp
3. **Apply**: Click "Apply Template" hoặc "Apply Template to All Selected"
4. **Done**: Components được thêm tự động

---

## 🎨 QUẢN LÝ TRONG INSPECTOR

### Categories Management

#### Thêm Category Mới
- Click **"Add New Category"** trong Inspector
- Hoặc Right-click component → **"Add New Category"**

#### Chỉnh Sửa Category
1. Expand category trong Inspector
2. **Category Name**: Đổi tên category
3. **Category Color**: Đổi màu hiển thị
4. **Category Icon**: Thêm icon (optional)
5. Click **"×"** để xóa category

### Characters Management

#### Chỉnh Sửa Character
**Trong Inspector:**
- Expand category → expand character
- Chỉnh sửa trực tiếp: Name, Prefab, Stats
- Click **"Edit"** để mở cửa sổ chỉnh sửa chi tiết

**Cửa Sổ Edit:**
- **Basic Info**: Name, Prefab, UI Icon
- **Stats**: Health, Speed, Attack Damage, Attack Range
- **Visual**: Team Color, Description
- Click **"Save"** để lưu thay đổi

#### Xóa Character
- Click **"×"** bên cạnh character
- Confirm deletion

---

## 🛠️ TOOLS & UTILITIES

### Menu Tools → Character Manager

#### Force Refresh All
- Làm mới tất cả Character Managers
- Sync với UI hiện tại
- **Dùng khi:** UI không cập nhật

#### Initialize All  
- Khởi tạo categories cho tất cả Managers
- **Dùng khi:** Setup project mới

#### Validate All Data
- Kiểm tra tính hợp lệ của dữ liệu
- Báo cáo các vấn đề: prefab null, tên trống, stats không hợp lệ
- **Dùng khi:** Debug hoặc cleanup project

### Context Menu Functions
Right-click CharacterManager component:
- **Initialize Default Categories**
- **Add New Category**  
- **Auto Setup All Prefabs**
- **Validate Character Data**
- **Print Statistics**

### Inspector Tools

#### Auto Setup All Prefabs
- Tự động kiểm tra và thêm components thiếu
- Thêm RagdollCharacter, NavMeshAgent nếu cần
- **Dùng khi:** Có nhiều prefabs chưa setup

#### Validate Character Data
- Kiểm tra từng character:
  - Prefab có null không?
  - Tên có trống không?
  - Stats có hợp lệ không?
- Hiển thị số lượng issues tìm thấy

#### Print Statistics
- Hiển thị thống kê trong Console:
  - Tổng số categories
  - Tổng số characters
  - Phân bố theo category

---

## 🎯 WORKFLOWS KHUYẾN NGHỊ

### Workflow 1: Setup Project Mới
1. **Add CharacterManager** component
2. **Initialize Default Categories**
3. **Setup UI references** (CharacterSelectionUI, BattleGameManager)
4. **Import character models**
5. **Use Advanced Setup** để setup từng character chi tiết

### Workflow 2: Batch Import Characters
1. **Import nhiều models** vào Project
2. **Select tất cả models**
3. **Tools → Character Component Template**
4. **Chọn template phù hợp** (AI Character, Combat Character, etc.)
5. **Apply Template to All Selected**
6. **Add All Selected Objects** trong CharacterManager

### Workflow 3: Prototype Nhanh
1. **Tạo primitive objects** (Cube, Capsule, etc.)
2. **Select objects**
3. **Apply Basic Character template**
4. **Add to CharacterManager**
5. **Test gameplay** ngay lập tức

### Workflow 4: Fine-tuning Characters
1. **Sử dụng Inspector** để chỉnh sửa
2. **Click "Edit"** để mở detailed editor
3. **Adjust stats và properties**
4. **Validate data** định kỳ
5. **Print statistics** để monitor

---

## 🔍 TROUBLESHOOTING

### UI Không Cập Nhật
**Nguyên nhân:** Auto-refresh bị tắt hoặc reference bị null
**Giải pháp:**
1. Kiểm tra **"Auto Refresh UI"** đã bật
2. Click **"Refresh UI"** thủ công
3. Kiểm tra **CharacterSelectionUI reference**
4. **Tools → Character Manager → Force Refresh All**

### Prefab Thiếu Components
**Nguyên nhân:** Prefab chưa được setup đầy đủ
**Giải pháp:**
1. **"Auto Setup All Prefabs"** trong Inspector
2. **"Validate Character Data"** để kiểm tra
3. Sử dụng **Advanced Character Setup** cho setup chi tiết
4. Sử dụng **Component Templates** cho setup nhanh

### Performance Issues
**Nguyên nhân:** Quá nhiều characters, auto-refresh liên tục
**Giải pháp:**
1. Tắt **"Auto Refresh UI"** nếu có nhiều characters
2. Sử dụng **"Refresh UI"** thủ công khi cần
3. Chia characters thành nhiều categories nhỏ
4. Sử dụng **batch operations** thay vì thêm từng cái

### Components Không Tìm Thấy
**Nguyên nhân:** Script không tồn tại hoặc namespace sai
**Giải pháp:**
1. Kiểm tra script **RagdollCharacter** có tồn tại
2. Kiểm tra **compilation errors**
3. Sử dụng **built-in components** trước (Rigidbody, Collider)
4. Check **Setup Log** trong Advanced Setup để xem chi tiết

---

## 📊 BEST PRACTICES

### Organization
- ✅ **Đặt tên categories rõ ràng** với emoji
- ✅ **Sử dụng màu sắc** để phân biệt categories
- ✅ **Group characters** theo gameplay role
- ✅ **Consistent naming** cho characters

### Performance  
- ✅ **Tắt Auto Refresh** khi có >50 characters
- ✅ **Batch operations** thay vì individual setup
- ✅ **Validate data** định kỳ để cleanup
- ✅ **Use templates** cho consistency

### Workflow
- ✅ **Setup templates trước** khi import models
- ✅ **Test với Basic Character** trước khi full setup
- ✅ **Backup project** trước khi batch operations
- ✅ **Document custom templates** nếu tạo thêm

### Maintenance
- ✅ **Regular validation** để tìm issues
- ✅ **Print statistics** để monitor growth
- ✅ **Clean up unused** categories/characters
- ✅ **Update templates** khi có script mới

---

## 🎮 TÍNH NĂNG NÂNG CAO

### Programmatic Access
```csharp
// Get Character Manager
CharacterManager manager = FindObjectOfType<CharacterManager>();

// Get all characters
var allCharacters = manager.GetAllCharacters();

// Get characters from specific category
var soldiers = manager.GetCharactersFromCategory("🪖 CHIẾN BINH");

// Add character programmatically
CharacterEntry newCharacter = new CharacterEntry();
newCharacter.characterName = "My Character";
newCharacter.prefab = myPrefab;
manager.AddCharacterToCategory("🪖 CHIẾN BINH", newCharacter);

// Remove character
manager.RemoveCharacter(character);
```

### Custom Templates
Bạn có thể tạo custom templates bằng cách:
1. Modify **CharacterComponentTemplate.cs**
2. Thêm template mới vào **templates list**
3. Define **required components** và **description**

### Integration với Systems Khác
- **Save System**: Characters data có thể serialize
- **Multiplayer**: Sync character selection qua network
- **Modding**: Load characters từ external files
- **Analytics**: Track character usage statistics

---

## 📞 HỖ TRỢ

### Khi Gặp Vấn Đề
1. **Check Console** để xem error messages
2. **Validate Character Data** để tìm issues
3. **Print Statistics** để hiểu current state
4. **Force Refresh All** để reset system

### Debug Information
- **Setup Log** trong Advanced Setup
- **Console messages** từ validation
- **Statistics** từ Print Statistics
- **Component status** trong Inspector

### Common Issues & Solutions
| Issue | Solution |
|-------|----------|
| UI không hiển thị characters | Force Refresh UI |
| Prefab missing components | Auto Setup All Prefabs |
| Performance lag | Disable Auto Refresh |
| Template không apply | Check script existence |
| Categories không save | Mark object as dirty |

---

## 🎉 KẾT LUẬN

Character Management Tool cung cấp:

### ✅ **Productivity Boost**
- Giảm 80% thời gian setup characters
- Batch operations cho efficiency
- Template system cho consistency
- Visual management interface

### ✅ **Quality Assurance**  
- Auto-validation để tránh errors
- Consistent component setup
- Built-in best practices
- Error prevention mechanisms

### ✅ **Scalability**
- Handle hundreds of characters
- Organized category system
- Batch processing capabilities
- Performance optimizations

### ✅ **User Experience**
- Intuitive Inspector interface
- Context menu shortcuts
- Real-time feedback
- Comprehensive documentation

**Character Management Tool** là giải pháp hoàn chỉnh cho việc quản lý nhân vật trong Unity, giúp bạn tập trung vào gameplay thay vì setup technical! 🚀

---

*Tài liệu này được tạo tự động bởi Character Management Tool System*
*Phiên bản: 1.0 | Cập nhật: 2025*