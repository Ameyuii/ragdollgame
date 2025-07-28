# 📋 TỔNG QUAN DỰ ÁN UNITY RAGDOLL GAME

## 🎯 MỤC ĐÍCH TÀI LIỆU
Tài liệu này cung cấp cái nhìn tổng quan về toàn bộ logic, chức năng và kiến trúc của dự án Unity Ragdoll Game. Được thiết kế để có thể cập nhật và mở rộng cho các chức năng triển khai trong tương lai.

---

## 🏗️ KIẾN TRÚC TỔNG THỂ

### 📊 Sơ đồ hệ thống chính
```
┌─────────────────────────────────────────────────────────────┐
│                    UNITY RAGDOLL GAME                      │
├─────────────────────────────────────────────────────────────┤
│  🎮 GAME MANAGEMENT LAYER                                   │
│  ├── BattleGameManager (Core Game Controller)              │
│  ├── GameDatabase (Data Management)                        │
│  └── MapStateManager (Scene State)                         │
├─────────────────────────────────────────────────────────────┤
│  👥 CHARACTER SYSTEM LAYER                                  │
│  ├── Character Database System                             │
│  ├── Character Definition System                           │
│  ├── Ragdoll Physics System                               │
│  └── AI Combat System                                      │
├─────────────────────────────────────────────────────────────┤
│  🖥️ UI SYSTEM LAYER                                         │
│  ├── Character Selection UI                                │
│  ├── Team Selection System                                 │
│  ├── Drag & Drop System                                    │
│  └── Battle Control UI                                     │
├─────────────────────────────────────────────────────────────┤
│  ⚡ PHYSICS & ANIMATION LAYER                               │
│  ├── Ragdoll Physics                                       │
│  ├── Animation Controller                                  │
│  ├── Health & Damage System                               │
│  └── Visual Effects                                        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎮 CHỨC NĂNG CHÍNH

### 1. 🎯 GAME FLOW CHÍNH

#### **Setup Phase (Giai đoạn chuẩn bị)**
1. **Character Selection**: Người chơi chọn nhân vật từ danh sách
2. **Team Assignment**: Gán nhân vật vào team (Blue/Red)
3. **Map Placement**: Kéo thả nhân vật vào vị trí trên map
4. **Battle Preparation**: Chuẩn bị cho trận chiến

#### **Battle Phase (Giai đoạn chiến đấu)**
1. **AI Activation**: Kích hoạt AI cho tất cả nhân vật
2. **Combat System**: Nhân vật tự động tìm và tấn công kẻ địch
3. **Physics Simulation**: Ragdoll physics khi nhận damage
4. **Health Management**: Theo dõi máu và trạng thái sống/chết

#### **End Phase (Giai đoạn kết thúc)**
1. **Victory Condition**: Kiểm tra điều kiện thắng/thua
2. **Battle Results**: Hiển thị kết quả trận đấu
3. **Reset Option**: Cho phép reset và chơi lại

### 2. 🏗️ HỆ THỐNG QUẢN LÝ GAME

#### **BattleGameManager.cs** - Trung tâm điều khiển
```csharp
// Chức năng chính:
- Quản lý trạng thái game (Setup/Battle/End)
- Điều khiển UI elements
- Theo dõi số lượng nhân vật còn sống
- Xử lý logic thắng/thua
- Quản lý character spawning
```

**Các trạng thái game:**
- `setupMode = true`: Cho phép drag & drop characters
- `gameStarted = false`: Chưa bắt đầu battle
- `gameStarted = true`: Đang trong battle

**UI Elements được quản lý:**
- Team counters (Team1Counter, Team2Counter)
- Status text (StatusText)
- Control buttons (StartButton, ResetButton)
- Character selection panel

### 3. 👥 HỆ THỐNG NHÂN VẬT

#### **Character Database System**
```
📁 CharacterSystem/
├── 📄 CharacterDatabase.cs - Database chính chứa tất cả characters
├── 📄 CharacterDefinition.cs - Định nghĩa từng character
├── 📄 CharacterStats.cs - Thống kê character
├── 📄 GameDatabase.cs - Singleton quản lý database
└── 📄 CharacterEvents.cs - Event system
```

#### **Character Definition Structure**
```csharp
CharacterDefinition {
    - characterID: string (unique identifier)
    - displayName: string (tên hiển thị)
    - categoryID: string (loại nhân vật)
    - basePrefab: GameObject (prefab gốc)
    - baseStats: CharacterStats (thống kê cơ bản)
    - variants: List<CharacterVariant> (các biến thể)
    - teamMaterials: List<TeamMaterialSet> (materials theo team)
    - unlockCondition: UnlockCondition (điều kiện mở khóa)
}
```

#### **RagdollCharacter.cs** - Character Implementation
```csharp
// Core Components:
- Health System: maxHealth, currentHealth, isDead
- Movement System: moveSpeed, AI navigation
- Combat System: attackDamage, attackRange, attackCooldown
- Team System: teamId (1=Blue, 2=Red)
- Physics System: ragdoll activation/deactivation
- Animation System: Animator controller integration
```

**Ragdoll Physics Logic:**
1. **Normal State**: Character sử dụng Animator và kinematic Rigidbody
2. **Hit State**: Tạm thời activate ragdoll khi nhận damage
3. **Death State**: Hoàn toàn activate ragdoll, disable AI

### 4. 🖥️ HỆ THỐNG UI

#### **Character Selection System**
```
📁 UI Components:
├── 📄 CategoryButtonHandler.cs - Xử lý category buttons
├── 📄 CharacterManager.cs - Quản lý character lists
├── 📄 TeamSelector.cs - Team selection dropdown
└── 📄 CharacterButtonHover.cs - Hover effects
```

#### **Drag & Drop System**
```
📁 Drag & Drop:
├── 📄 CharacterDragSource.cs - Nguồn drag (UI buttons)
├── 📄 CharacterDragDrop.cs - Logic drag & drop
└── 📄 MapDropZone.cs - Vùng drop trên map
```

**Drag & Drop Flow:**
1. **Begin Drag**: Tạo preview object, theo dõi mouse
2. **During Drag**: Update preview position, hiển thị valid/invalid zones
3. **End Drag**: Spawn character tại vị trí drop hoặc cancel

#### **Team Selection System**
```csharp
TeamSelector {
    - availableTeams: TeamData[] (danh sách teams)
    - selectedTeamId: int (team hiện tại được chọn)
    - OnTeamChanged: Action<int> (event khi đổi team)
}

TeamData {
    - teamId: int
    - teamName: string
    - teamColor: Color
}
```

### 5. 🤖 HỆ THỐNG AI

#### **⚠️ TRẠNG THÁI HIỆN TẠI: AI BỊ VÔ HIỆU HÓA**
```csharp
// QUAN TRỌNG: AI movement/combat hiện đang bị disable trong RagdollCharacter.cs (dòng 141)
// Cần enable lại để kích hoạt combat system
if (false) // <- Thay đổi thành 'true' để enable AI
{
    // AI logic here
}
```

#### **AutoAIManager.cs** - AI Controller
```csharp
// Chức năng:
- Tự động setup AI cho characters mới spawn
- Quản lý AI behavior trong battle
- Tối ưu performance với update intervals
- Debug và monitoring AI states
// HIỆN TẠI: Cần kiểm tra và kích hoạt lại AI logic
```

**AI Behavior Logic (Khi được enable):**
1. **Target Detection**: Tìm kẻ địch gần nhất trong detection range
2. **Movement**: Di chuyển đến target sử dụng NavMesh
3. **Attack**: Tấn công khi trong attack range
4. **Cooldown Management**: Quản lý thời gian hồi chiêu

#### **SimpleCharacterAI.cs** - Individual AI
```csharp
// AI States (Khi được kích hoạt):
- Idle: Đứng yên, tìm kiếm target
- Moving: Di chuyển đến target
- Attacking: Thực hiện tấn công
- Dead: Không hoạt động
```

### 6. ⚡ HỆ THỐNG PHYSICS

#### **Ragdoll Physics Implementation**
```csharp
// Components Setup:
- Main Rigidbody: Kinematic (normal), Non-kinematic (ragdoll)
- Ragdoll Rigidbodies[]: Tất cả body parts
- Ragdoll Colliders[]: Collision detection cho body parts
- Joints: Kết nối các body parts
```

**Physics States:**
1. **Animated State**: Animator control, kinematic rigidbody
2. **Ragdoll State**: Physics control, non-kinematic rigidbodies
3. **Hybrid State**: Một số parts ragdoll, một số animated

#### **Health & Damage System**
```csharp
// Health Management:
- maxHealth: float (máu tối đa)
- health: float (máu hiện tại)
- healthSlider: UI Slider (thanh máu)
- isDead: bool (trạng thái sống/chết)

// Damage Processing:
- TakeDamage(float damage): Nhận damage
- Die(): Xử lý khi chết
- ActivateRagdoll(): Kích hoạt ragdoll physics
```

---

## 🔧 KIẾN TRÚC KỸ THUẬT

### 1. 📁 CẤU TRÚC THƯ MỤC

```
Assets/
├── 📁 Scripts/
│   ├── 📁 CharacterSystem/          # Hệ thống character
│   │   ├── CharacterDatabase.cs
│   │   ├── CharacterDefinition.cs
│   │   ├── CharacterStats.cs
│   │   └── GameDatabase.cs
│   ├── BattleGameManager.cs         # Game controller chính
│   ├── RagdollCharacter.cs          # Character implementation
│   ├── AutoAIManager.cs             # AI management
│   ├── TeamSelector.cs              # Team selection
│   ├── CharacterDragSource.cs       # Drag & drop
│   └── [Other scripts...]
├── 📁 Prefabs/                      # Character prefabs
├── 📁 Materials/                    # Team materials
├── 📁 Animation/                    # Animation assets
├── 📁 Resources/                    # Runtime loadable assets
└── 📁 Scenes/                       # Game scenes
```

### 2. 🎯 DESIGN PATTERNS ĐƯỢC SỬ DỤNG

#### **Singleton Pattern**
- `GameDatabase.Instance`: Truy cập global database
- `BattleGameManager`: Game state management

#### **Observer Pattern**
- `TeamSelector.OnTeamChanged`: Team selection events
- Character death events
- UI update events

#### **Component Pattern**
- Unity's component-based architecture
- Modular character systems
- Reusable UI components

#### **Object Pool Pattern**
- Character spawning optimization
- Effect object reuse
- Performance optimization

### 3. 🔄 DATA FLOW

```
User Input → UI System → Game Manager → Character System → Physics System
     ↑                                                            ↓
UI Updates ← Game State ← AI System ← Character Events ← Physics Events
```

**Luồng dữ liệu chính:**
1. **Input**: User interaction với UI
2. **Processing**: Game manager xử lý logic
3. **Execution**: Character system thực thi
4. **Feedback**: Physics events trigger UI updates

---

## 🚀 CHỨC NĂNG CÓ THỂ MỞ RỘNG

### 1. 🎮 GAMEPLAY FEATURES

#### **Đã có sẵn - Có thể mở rộng:**
- ✅ **Character Variants**: Hỗ trợ multiple variants per character
- ✅ **Team System**: Unlimited teams với custom colors/materials
- ✅ **Unlock System**: Character unlock conditions
- ✅ **Rarity System**: Common, Rare, Epic, Legendary
- ✅ **Stats System**: Customizable character stats

#### **Có thể thêm mới:**
- 🔄 **Skill System**: Special abilities cho characters
- 🔄 **Weapon System**: Trang bị vũ khí khác nhau
- 🔄 **Level System**: Character progression
- 🔄 **Achievement System**: Unlock rewards
- 🔄 **Tournament Mode**: Bracket-style competitions

### 2. 🎨 VISUAL ENHANCEMENTS

#### **Có thể mở rộng:**
- 🔄 **Particle Effects**: Hit, death, spawn effects
- 🔄 **Shader Effects**: Team glow, damage indicators
- 🔄 **Animation System**: More combat animations
- 🔄 **Camera System**: Dynamic camera angles
- 🔄 **Environment**: Interactive map elements

### 3. 🔊 AUDIO SYSTEM

#### **Có thể thêm:**
- 🔄 **Sound Effects**: Combat, UI, ambient sounds
- 🔄 **Music System**: Dynamic background music
- 🔄 **Voice Acting**: Character voices
- 🔄 **Audio Mixing**: Volume controls, audio settings

### 4. 🌐 MULTIPLAYER FEATURES

#### **Có thể mở rộng:**
- 🔄 **Local Multiplayer**: Split-screen hoặc hot-seat
- 🔄 **Online Multiplayer**: Network battles
- 🔄 **Spectator Mode**: Watch other battles
- 🔄 **Replay System**: Record và playback battles

### 5. 📊 DATA & ANALYTICS

#### **Có thể thêm:**
- 🔄 **Save System**: Player progress, unlocks
- 🔄 **Statistics**: Battle history, win rates
- 🔄 **Leaderboards**: Global rankings
- 🔄 **Analytics**: Player behavior tracking

---

## 🛠️ HƯỚNG DẪN PHÁT TRIỂN

### 1. 📝 THÊM CHARACTER MỚI

#### **Bước 1: Tạo Character Prefab**
```csharp
1. Import 3D model vào Unity
2. Setup Animator Controller
3. Add RagdollCharacter component
4. Configure ragdoll physics
5. Setup team materials
6. Save as prefab
```

#### **Bước 2: Tạo Character Definition**
```csharp
1. Right-click → Create → Character System → Character Definition
2. Assign prefab, icon, stats
3. Set category, rarity, unlock conditions
4. Add to CharacterDatabase
```

#### **Bước 3: Update Database**
```csharp
1. Open CharacterDatabase asset
2. Add new character to characters list
3. Ensure category exists
4. Test in game
```

### 2. 🎨 THÊM TEAM MỚI

#### **Tạo Team Configuration:**
```csharp
TeamConfiguration newTeam = new TeamConfiguration {
    teamID = 3,
    teamName = "Green Team",
    primaryColor = Color.green,
    baseMaterial = greenMaterial,
    teamIcon = greenIcon
};
```

#### **Update UI:**
```csharp
// Trong TeamSelector.cs
TeamData[] availableTeams = {
    // ... existing teams
    new TeamData { teamId = 3, teamName = "🟢 TEAM 3", teamColor = Color.green }
};
```

### 3. 🤖 CUSTOM AI BEHAVIORS

#### **Tạo AI Behavior mới:**
```csharp
public class CustomAI : MonoBehaviour {
    // Implement custom AI logic
    // Override target selection
    // Custom movement patterns
    // Special attack behaviors
}
```

#### **Integration với AutoAIManager:**
```csharp
// Trong AutoAIManager.SetupCharacterAI()
if (character.GetComponent<CustomAI>() == null) {
    character.AddComponent<CustomAI>();
}
```

### 4. 🎮 CUSTOM GAME MODES

#### **Tạo Game Mode mới:**
```csharp
public class CustomGameMode : MonoBehaviour {
    // Override victory conditions
    // Custom rules
    // Special mechanics
    // Time limits, objectives, etc.
}
```

---

## 🐛 TROUBLESHOOTING

### 1. ⚠️ COMMON ISSUES

#### **Character không spawn:**
- ✅ Kiểm tra prefab có RagdollCharacter component
- ✅ Verify team materials được assign
- ✅ Check console cho error messages
- ✅ Kiểm tra CharacterDefinition assets trong Resources folder

#### **Ragdoll không hoạt động:**
- ✅ Ensure tất cả body parts có Rigidbody + Collider
- ✅ Check Joint connections
- ✅ Verify layer settings
- ✅ Kiểm tra ragdoll setup trong prefab

#### **AI không hoạt động (QUAN TRỌNG):**
- ⚠️ **KIỂM TRA ĐẦU TIÊN**: AI bị disable trong RagdollCharacter.cs dòng 141
- ✅ Check NavMesh được bake
- ✅ Verify AutoAIManager enabled
- ✅ Ensure characters có NavMeshAgent
- ✅ Kiểm tra SimpleCharacterAI component

#### **Health Bar không hiển thị:**
- ⚠️ **VẤN ĐỀ HIỆN TẠI**: Health bar system đang được debug
- ✅ Kiểm tra Health Bar prefab setup
- ✅ Verify Canvas và UI components
- ✅ Check script FixHealthBarFinal.cs và các debug scripts

#### **UI không responsive:**
- ✅ Check EventSystem trong scene
- ✅ Verify Canvas settings
- ✅ Check button listeners

### 2. 🔧 PERFORMANCE OPTIMIZATION

#### **Character Performance:**
```csharp
// Optimize ragdoll physics
- Reduce rigidbody count
- Use simpler colliders
- Implement LOD system
- Pool character objects
```

#### **UI Performance:**
```csharp
// Optimize UI updates
- Batch UI updates
- Use object pooling for buttons
- Minimize layout rebuilds
- Cache UI references
```

#### **AI Performance:**
```csharp
// Optimize AI calculations
- Increase update intervals
- Use spatial partitioning
- Limit simultaneous pathfinding
- Implement behavior trees
```

---

## 📈 ROADMAP PHÁT TRIỂN

### 🎯 PHASE 1: URGENT FIXES (Ưu tiên cao)
- [ ] **Enable AI System**: Kích hoạt lại AI movement/combat trong RagdollCharacter.cs
- [ ] **Fix Health Bar System**: Hoàn thiện health bar display và functionality
- [ ] **Position Validation**: Enable lại position validation trong drag & drop
- [ ] **Character AI Testing**: Test và debug AI behaviors
- [ ] **Performance Issues**: Fix any performance bottlenecks

### 🎯 PHASE 2: SYSTEM COMPLETION (Trung hạn)
- [ ] **Character System Polish**: Hoàn thiện character variants và definitions
- [ ] **Audio System**: Sound effects, background music
- [ ] **Visual Effects**: Particle systems, hit effects
- [ ] **Save System**: Player progress, character unlocks
- [ ] **UI/UX Improvements**: Polish interface và user experience

### 🎯 PHASE 3: CONTENT EXPANSION (Dài hạn)
- [ ] **More Characters**: Mở rộng character roster (hiện có bear-fish mới)
- [ ] **New Game Modes**: Tournament, survival, objectives
- [ ] **Skill System**: Special abilities, combos
- [ ] **Weapon System**: Equipment, upgrades
- [ ] **Map Variations**: Multiple battle arenas

### 🎯 PHASE 4: ADVANCED FEATURES
- [ ] **Multiplayer**: Local và online multiplayer
- [ ] **Level Editor**: User-generated content
- [ ] **Mod Support**: Community modifications
- [ ] **Mobile Port**: Touch controls, optimization

### 🎯 PHASE 5: POLISH & RELEASE
- [ ] **Complete Testing**: QA, bug fixes, optimization
- [ ] **Balancing**: Character stats, gameplay tuning
- [ ] **Professional Polish**: UI/UX finalization
- [ ] **Marketing**: Trailers, screenshots, store pages

---

## 📚 TÀI LIỆU THAM KHẢO

### 🔗 INTERNAL DOCUMENTATION
- `CHARACTER_MANAGEMENT_SYSTEM_DESIGN.md` - Chi tiết character system
- `CHARACTER_SYSTEM_GUIDE.md` - Hướng dẫn sử dụng character system
- `Unity_Ragdoll_Game_Documentation.md` - Documentation chi tiết
- `UI_SYSTEM_USAGE_GUIDE.md` - Hướng dẫn UI system

### 🔗 EXTERNAL RESOURCES
- [Unity Ragdoll Physics](https://docs.unity3d.com/Manual/wizard-RagdollWizard.html)
- [Unity NavMesh AI](https://docs.unity3d.com/Manual/nav-NavigationSystem.html)
- [Unity UI System](https://docs.unity3d.com/Manual/UISystem.html)
- [Unity Animation System](https://docs.unity3d.com/Manual/AnimationSection.html)

---

## 📝 CHANGELOG

### Version 1.0.0 (Current - Cần hoàn thiện)
- ✅ Core game mechanics implemented
- ✅ Character system with database (ScriptableObject architecture)
- ✅ Drag & drop UI system
- ✅ Ragdoll physics system
- ✅ Unified UI system (BattleGameManager)
- ✅ Character Database với categories và variants
- ⚠️ AI combat system (hiện đang disabled, cần enable)
- ⚠️ Health bar system (đang được debug)
- 🔄 Team management system (cần testing)

### Future Versions
- 🔄 Version 1.0.1: Enable AI system, fix health bars, position validation
- 🔄 Version 1.1.0: Performance optimizations, audio system
- 🔄 Version 1.2.0: Content expansion, bear-fish character completion
- 🔄 Version 2.0.0: Multiplayer support, advanced features

---

## 👥 TEAM & CONTRIBUTORS

### 🏗️ ARCHITECTURE
- **Game Systems**: BattleGameManager, Character Database
- **Physics Systems**: Ragdoll implementation, collision detection
- **AI Systems**: AutoAIManager, character behaviors

### 🎨 UI/UX
- **Interface Design**: Character selection, team management
- **User Experience**: Drag & drop interactions, visual feedback
- **Visual Design**: Team colors, materials, effects

### 🔧 TECHNICAL
- **Performance**: Optimization, memory management
- **Integration**: System interconnections, data flow
- **Testing**: Bug fixes, stability improvements

---

*Tài liệu này sẽ được cập nhật thường xuyên khi có thêm chức năng mới hoặc thay đổi trong dự án.*

**Cập nhật lần cuối:** `27/07/2025`
**Phiên bản:** `1.0.0 (In Development)`
**Tác giả:** `Unity Ragdoll Game Development Team`
**Trạng thái:** `Active Development - Cần hoàn thiện AI system và Health bars`