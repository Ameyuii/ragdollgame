# ⚡ CHARACTER MANAGEMENT TOOL - QUICK REFERENCE

## 🚀 SETUP NHANH (5 PHÚT)

### 1. Thêm Manager
```
Right-click GameObject → Character Management → Add Character Manager
```

### 2. Khởi tạo
```
Inspector → "Initialize Default Categories"
```

### 3. Thêm nhân vật
```
Tools → Advanced Character Setup → Setup Character
```

---

## 📋 MENU SHORTCUTS

### Tools Menu
- **Tools → Advanced Character Setup** - Setup chi tiết nhân vật
- **Tools → Character Component Template** - Apply templates nhanh
- **Tools → Character Manager → Force Refresh All** - Refresh tất cả

### Right-click GameObject
- **Character Management → Add Character Manager** - Thêm manager
- **Character Management → Setup Selected as Character** - Setup selection
- **Character Management → Apply Component Template** - Apply template

### Right-click Assets
- **Character Management → Auto Setup Character Prefab** - Auto setup prefab

---

## 🎯 TEMPLATES NHANH

| Template | Components | Dùng cho |
|----------|------------|----------|
| **Basic** | RagdollCharacter + Physics | Prototype nhanh |
| **AI** | + NavMeshAgent + Animator | Bot, NPC, Enemy |
| **Player** | + Animator + AudioSource | Player character |
| **Combat** | Full components | Character chiến đấu |
| **Vehicle** | + BoxCollider | Xe tăng, robot lớn |

---

## 🔧 COMPONENTS CHÍNH

### Core Components
- ✅ **RagdollCharacter** - Script chính
- ✅ **NavMeshAgent** - AI navigation  
- ✅ **Animator** - Animation
- ✅ **Rigidbody** - Physics
- ✅ **CapsuleCollider** - Collision
- ✅ **AudioSource** - Sound

### AI Components  
- ✅ **Character AI** - Trí tuệ nhân tạo
- ✅ **Health System** - Hệ thống máu
- ✅ **Weapon System** - Hệ thống vũ khí

---

## ⚡ WORKFLOW NHANH

### Single Character
```
1. Tools → Advanced Character Setup
2. Drag prefab → Auto-detect components
3. Setup Character with All Components
```

### Batch Characters
```
1. Select multiple objects
2. Tools → Character Component Template  
3. Choose template → Apply to All Selected
4. CharacterManager → Add All Selected Objects
```

### Quick Prototype
```
1. Create Cube/Capsule
2. Right-click → Apply Component Template
3. Choose "Basic Character"
```

---

## 🛠️ TROUBLESHOOTING NHANH

| Problem | Quick Fix |
|---------|-----------|
| UI không cập nhật | Inspector → "Refresh UI" |
| Prefab thiếu components | "Auto Setup All Prefabs" |
| Performance chậm | Tắt "Auto Refresh UI" |
| Template không work | Check script tồn tại |
| Data không save | Right-click → "Validate Character Data" |

---

## 🎯 HOTKEYS & SHORTCUTS

### Inspector Shortcuts
- **Initialize Default Categories** - Tạo categories mặc định
- **Add New Category** - Thêm category mới
- **Refresh UI** - Làm mới UI
- **Auto Setup All Prefabs** - Setup tất cả prefabs
- **Validate Character Data** - Kiểm tra dữ liệu

### Context Menu (Right-click component)
- **Initialize Default Categories**
- **Add New Category**  
- **Auto Setup All Prefabs**
- **Validate Character Data**
- **Print Statistics**

---

## 📊 STATS & VALIDATION

### Print Statistics
```
Right-click CharacterManager → Print Statistics
→ Console hiển thị: Categories, Characters, Distribution
```

### Validate Data
```
Inspector → "Validate Character Data"
→ Kiểm tra: Null prefabs, Empty names, Invalid stats
```

### Force Refresh
```
Tools → Character Manager → Force Refresh All
→ Refresh tất cả managers trong scene
```

---

## 🎮 PROGRAMMATIC ACCESS

### Get Manager
```csharp
CharacterManager manager = FindObjectOfType<CharacterManager>();
```

### Get Characters
```csharp
// All characters
var all = manager.GetAllCharacters();

// By category
var soldiers = manager.GetCharactersFromCategory("🪖 CHIẾN BINH");
```

### Add Character
```csharp
CharacterEntry character = new CharacterEntry();
character.characterName = "New Character";
character.prefab = myPrefab;
manager.AddCharacterToCategory("🪖 CHIẾN BINH", character);
```

---

## 🔍 DEBUG COMMANDS

### Console Commands
```csharp
// In any script
CharacterManager manager = FindObjectOfType<CharacterManager>();

// Print all characters
manager.PrintStatistics();

// Validate all data  
manager.ValidateCharacterData();

// Refresh UI
manager.RefreshUI();

// Get count
Debug.Log($"Total: {manager.TotalCharacterCount}");
```

---

## ⚙️ SETTINGS QUAN TRỌNG

### CharacterManager Settings
- ✅ **Auto Refresh UI** - Tự động refresh (tắt nếu lag)
- ✅ **Auto Setup Prefabs** - Tự động kiểm tra components
- 🔗 **Character Selection UI** - Reference đến UI
- 🔗 **Game Manager** - Reference đến GameManager

### Advanced Setup Settings
- 🎯 **Target Prefab** - Prefab cần setup
- 📝 **Character Name** - Tên nhân vật
- 🎨 **Animator Controller** - Controller cho animation
- 👤 **Avatar** - Avatar cho humanoid
- ⚖️ **Mass** - Khối lượng physics
- ❤️ **Health** - Máu nhân vật
- 🏃 **Speed** - Tốc độ di chuyển

---

## 🎯 BEST PRACTICES

### ✅ DO
- Sử dụng templates cho consistency
- Batch operations cho efficiency  
- Validate data định kỳ
- Backup trước khi batch operations
- Đặt tên categories rõ ràng

### ❌ DON'T
- Không tắt Auto Refresh khi có ít characters
- Không skip validation khi có nhiều characters
- Không modify prefabs trực tiếp mà không backup
- Không mix manual setup với tool setup

---

## 📞 SUPPORT

### Khi Cần Giúp Đỡ
1. **Check Console** - Xem error messages
2. **Validate Data** - Tìm issues
3. **Print Statistics** - Hiểu current state  
4. **Force Refresh** - Reset system

### Common Fixes
- **UI issues** → Force Refresh All
- **Component issues** → Auto Setup All Prefabs
- **Performance issues** → Disable Auto Refresh
- **Data issues** → Validate Character Data

---

*Quick Reference v1.0 - Character Management Tool*