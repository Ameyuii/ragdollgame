# HƯỚNG DẪN CHI TIẾT: CHARACTER DATA SYSTEM

## CharacterData là gì? 🤔

**CharacterData** là một **ScriptableObject** - nó như một "template" hoặc "blueprint" chứa tất cả thông số cấu hình cho NPCs.

### Tại sao cần CharacterData?

#### ❌ **Cách cũ** (không dùng CharacterData):
- Phải cài đặt thông số trực tiếp trên từng NPC trong scene
- Khó maintain khi có nhiều NPCs
- Thay đổi stats phải sửa từng GameObject
- Không thể tái sử dụng configs

#### ✅ **Cách mới** (dùng CharacterData):
- Tạo 1 file config, sử dụng cho nhiều NPCs
- Dễ balance game - chỉnh 1 chỗ, all NPCs cùng loại update
- Có thể tạo variants (Warrior_Strong, Warrior_Fast, etc.)
- Designer có thể tạo configs mà không cần code

## Cách hoạt động 🔧

### 1. CharacterData.cs (ScriptableObject)
```csharp
[CreateAssetMenu(fileName = "New Character Data", menuName = "NPC System/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Stats")]
    public string characterName = "Unknown";
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public int teamId = 0;
    
    [Header("Combat Stats")]
    public float baseDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float detectionRange = 15f;
    
    // ... more configs
}
```

### 2. NPCBaseController sử dụng CharacterData
```csharp
public class NPCBaseController : MonoBehaviour
{
    [Header("Character Configuration")]
    public CharacterData characterData; // Drag file .asset vào đây
    
    protected virtual void Start()
    {
        // Apply data từ ScriptableObject vào NPC
        if (characterData != null)
        {
            characterData.ApplyToCharacter(this);
        }
    }
}
```

## Cách Setup Chi Tiết 📝

### Bước 1: Tạo CharacterData Asset

#### Cách 1: Tạo bằng tay
1. **Right-click** trong Project window
2. **Create** → **NPC System** → **Character Data**
3. Đặt tên file (ví dụ: `Warrior_Heavy.asset`)

#### Cách 2: Sử dụng Factory (Recommended)
```csharp
// Code này đã có sẵn trong CharacterDataFactory.cs
var warriorData = CharacterDataFactory.CreateWarriorData();
var mageData = CharacterDataFactory.CreateMageData();
var archerData = CharacterDataFactory.CreateArcherData();
```

### Bước 2: Cấu hình Stats

Chọn file `.asset` vừa tạo, trong Inspector sẽ thấy:

```
📊 CHARACTER DATA INSPECTOR:

┌─ Basic Stats ─────────────────────┐
│ Character Name: "Heavy Warrior"   │
│ Max Health: 150                   │
│ Move Speed: 3.0                   │
│ Team ID: 1                        │
└───────────────────────────────────┘

┌─ Combat Stats ────────────────────┐
│ Base Damage: 40                   │
│ Attack Range: 2.5                 │
│ Attack Cooldown: 2.0              │
│ Detection Range: 8.0              │
└───────────────────────────────────┘

┌─ Visual & Audio ──────────────────┐
│ Character Prefab: [None]          │
│ Animator Controller: [None]       │
│ Hit Effect: [None]                │
│ Death Effect: [None]              │
└───────────────────────────────────┘

┌─ AI Behavior ─────────────────────┐
│ Patrol Speed: 2.0                 │
│ Patrol Rest Time: (3, 8)          │
│ Can Attack While Patrolling: ✓    │
└───────────────────────────────────┘
```

### Bước 3: Gán vào NPC

1. Chọn NPC GameObject trong scene
2. Trong **NPCBaseController** component
3. Drag file `.asset` vào field **Character Data**

```
🎯 NPC INSPECTOR:

┌─ NPC Base Controller ─────────────┐
│ Character Data: Warrior_Heavy ←── │ Drag .asset file vào đây
│ Max Health: 100 (will be overridden)
│ Team: 0 (will be overridden)
│ Move Speed: 3.5 (will be overridden)
└───────────────────────────────────┘
```

## Ví dụ thực tế 🎮

### Tạo 3 loại Warrior khác nhau:

#### 1. Warrior_Tank.asset
```
Max Health: 200
Base Damage: 25
Move Speed: 2.0
Attack Range: 2.0
```

#### 2. Warrior_DPS.asset  
```
Max Health: 100
Base Damage: 50
Move Speed: 4.0
Attack Range: 2.5
```

#### 3. Warrior_Balanced.asset
```
Max Health: 150
Base Damage: 35
Move Speed: 3.0
Attack Range: 2.2
```

### Sử dụng trong Scene:
- **Boss Warrior** → Gán `Warrior_Tank.asset`
- **Elite Warrior** → Gán `Warrior_DPS.asset`  
- **Normal Warrior** → Gán `Warrior_Balanced.asset`

## Lợi ích của hệ thống này 🎯

### 1. **Easy Balancing**
```
Warrior quá mạnh? 
→ Chỉnh Max Health trong Warrior.asset
→ TẤT CẢ Warriors trong game update ngay!
```

### 2. **Content Creation**
```
Designer muốn tạo boss mới?
→ Duplicate Warrior.asset
→ Rename thành WarriorBoss.asset  
→ Tăng stats → Done!
```

### 3. **A/B Testing**
```
Version A: Mage_Fast.asset (low HP, high speed)
Version B: Mage_Tank.asset (high HP, low speed)
→ Test xem players prefer loại nào
```

### 4. **DLC/Expansion Support**
```
Thêm character type mới?
→ Tạo CharacterData cho nó
→ Existing code vẫn work perfect
```

## Cấu trúc Files 📁

```
Assets/Scripts/Data/
├── ScriptableObjects/
│   └── CharacterData.cs ← Main ScriptableObject class
├── CharacterConfigs/  
│   └── CharacterDataFactory.cs ← Helper để tạo default configs
└── Generated Assets/ (sẽ tạo khi dùng)
    ├── Warrior_Default.asset
    ├── Mage_Default.asset
    ├── Archer_Default.asset
    └── CustomVariants/
        ├── Warrior_Boss.asset
        ├── Mage_Elite.asset
        └── etc...
```

## Code Flow 🔄

```
1. Designer tạo/chỉnh CharacterData.asset
           ↓
2. NPC trong scene references file .asset
           ↓  
3. NPCBaseController.Start() calls characterData.ApplyToCharacter()
           ↓
4. Stats từ .asset được copy vào NPC instance
           ↓
5. NPC hoạt động với stats từ CharacterData
```

## Troubleshooting 🔧

### ❌ **"Character Data is null"**
```
Giải pháp: Drag file .asset vào Character Data field của NPCBaseController
```

### ❌ **"Create menu không thấy NPC System"**
```
Giải pháp: 
1. Build project để Unity nhận CreateAssetMenu
2. Hoặc dùng CharacterDataFactory.CreateXXXData() trong code
```

### ❌ **"Stats không update khi chỉnh .asset"**
```
Giải pháp: Stats chỉ apply lúc Start(). Restart scene để thấy changes.
```

## Summary 📋

**CharacterData = Configuration System cho NPCs**

✅ **Dùng khi nào**: Khi muốn quản lý stats NPCs một cách organized
✅ **Lợi ích**: Easy balancing, content creation, maintenance  
✅ **Setup**: Create .asset → Configure stats → Drag vào NPC
✅ **Workflow**: Designer chỉnh .asset → Code tự động apply stats

**Đây là pattern chuẩn trong game development để separate data khỏi logic!** 🎮
