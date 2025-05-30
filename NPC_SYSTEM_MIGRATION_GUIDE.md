# NPC System Architecture - Setup và Migration Guide

## Tổng quan
Hệ thống NPC mới được thiết kế theo kiến trúc modular với inheritance hierarchy, interfaces và ScriptableObjects để dễ dàng mở rộng và maintain.

## Cấu trúc thư mục mới

```
Assets/Scripts/
├── Core/                           # Base classes và interfaces
│   ├── NPCBaseController.cs        # Base controller cho tất cả NPCs
│   ├── AttackSystemBase.cs         # Base attack system
│   ├── Interfaces/
│   │   ├── IAttackable.cs         # Interface cho attack behavior
│   │   ├── ITargetable.cs         # Interface cho target behavior
│   │   └── ISkill.cs              # Interface cho skill system
│   └── Skills/
│       └── SkillBase.cs           # Base class cho skills
├── Characters/                     # Character-specific implementations
│   ├── Warrior/
│   │   ├── WarriorController.cs   # Warrior controller
│   │   ├── WarriorAttackSystem.cs # Warrior attacks
│   │   └── ChargeSkill.cs         # Warrior skill
│   ├── Mage/
│   │   ├── MageController.cs      # Mage controller
│   │   ├── MageAttackSystem.cs    # Mage attacks
│   │   └── FireballSkill.cs       # Mage skill
│   └── Archer/
│       ├── ArcherController.cs    # Archer controller
│       └── ArcherAttackSystem.cs  # Archer attacks
├── Data/                          # Data containers
│   ├── ScriptableObjects/
│   │   └── CharacterData.cs       # Character configuration
│   └── CharacterConfigs/
│       └── CharacterDataFactory.cs # Factory tạo data
└── Tools/                         # Development tools
    └── NPCMigrationTool.cs        # Migration helper
```

## Character Types và Characteristics

### 1. Warrior
- **Đặc điểm**: Tank, tấn công cận chiến, máu nhiều
- **Attack Range**: 2.5f
- **Health**: 120f
- **Attack Types**: attack (40%), attack1 (30%), attack2 (30%)
- **Special**: Charge skill - lao về phía enemy

### 2. Mage
- **Đặc điểm**: Caster, tấn công tầm xa với magic, máu ít
- **Attack Range**: 10f  
- **Health**: 80f
- **Attack Types**: fireball (40%), lightning (35%), ice_shard (25%)
- **Special**: Fireball skill - projectile magic attack

### 3. Archer
- **Đặc điểm**: Ranged, tấn công tầm xa nhất, cần quản lý mũi tên
- **Attack Range**: 15f
- **Health**: 90f
- **Attack Types**: bow_shot (50%), power_shot (30%), rapid_fire (20%)
- **Special**: Arrow management system

## Migration Process

### Bước 1: Backup project
```bash
# Tạo backup trước khi migrate
cp -r "e:\unity\test ai unity" "e:\unity\test ai unity_backup"
```

### Bước 2: Chạy Migration Tool
1. Mở Unity Editor
2. Vào menu: `NPCSystem/Create Character Data Assets`
3. Vào menu: `NPCSystem/Migrate Existing NPCs`
4. Vào menu: `NPCSystem/Setup Scene NPCs`

### Bước 3: Kiểm tra sau Migration
- Tất cả NPCController cũ đã được thay thế
- Character Data assets đã được tạo
- NPCs mới hoạt động bình thường

## Cách sử dụng hệ thống mới

### 1. Tạo NPC mới

```csharp
// Tạo Warrior
GameObject warriorObj = new GameObject("New Warrior");
WarriorController warrior = warriorObj.AddComponent<WarriorController>();

// Load character data
CharacterData warriorData = Resources.Load<CharacterData>("CharacterConfigs/WarriorData");
warrior.SetCharacterData(warriorData);
```

### 2. Tùy chỉnh Character Data
- Mở file `.asset` trong `Assets/Scripts/Data/CharacterConfigs/`
- Chỉnh sửa các thuộc tính như health, damage, range, v.v.
- Thay đổi sẽ apply cho tất cả NPCs dùng data đó

### 3. Thêm Skills cho NPCs

```csharp
// Thêm Charge skill cho Warrior
ChargeSkill chargeSkill = warriorObj.AddComponent<ChargeSkill>();

// Thêm Fireball skill cho Mage
FireballSkill fireballSkill = mageObj.AddComponent<FireballSkill>();
```

### 4. Tạo Character Type mới

1. **Tạo Controller mới**:
```csharp
public class PaladinController : NPCBaseController
{
    // Implementation
}
```

2. **Tạo Attack System**:
```csharp
public class PaladinAttackSystem : AttackSystemBase
{
    // Implementation
}
```

3. **Tạo Character Data**:
- Thêm `CharacterType.Paladin` vào enum
- Tạo factory method trong `CharacterDataFactory`

## Animation Setup

### Animation Triggers cần thiết:

**Warrior**:
- `attack` - Basic attack
- `attack1` - Heavy attack  
- `attack2` - Combo attack
- `charge` - Charge skill

**Mage**:
- `fireball` - Fireball cast
- `lightning` - Lightning cast
- `ice_shard` - Ice shard cast

**Archer**:
- `bow_shot` - Normal bow shot
- `power_shot` - Charged shot
- `rapid_fire` - Multiple shots

### Animation Events:
- `OnAttackHit()` - Gây damage
- `OnAttackEnd()` - Kết thúc attack
- `OnSpellCast()` - Cast spell (Mage)
- `OnArrowShot()` - Bắn mũi tên (Archer)
- `OnChargeHit()` - Charge impact (Warrior)

## Debug và Testing

### Debug Features:
- `showDebugLogs` - Bật/tắt debug logs
- Gizmos hiển thị detection range và target line
- Console logs cho attack events

### Test Checklist:
- [ ] NPCs patrol bình thường
- [ ] Detection range hoạt động đúng
- [ ] Attack animations trigger đúng
- [ ] Damage được apply đúng
- [ ] Skills hoạt động đúng
- [ ] Character data được load đúng

## Performance Considerations

### Optimizations:
- NavMesh Agent pooling
- Attack cooldowns prevent spam
- LOD system cho animations
- Object pooling cho projectiles

### Memory Management:
- ScriptableObjects share data
- Destroy unused GameObjects
- Unload unused assets

## Troubleshooting

### Common Issues:

1. **NPCs không attack**:
   - Kiểm tra animation triggers
   - Kiểm tra attack range
   - Kiểm tra team settings

2. **Animation không chạy**:
   - Kiểm tra Animator Controller
   - Kiểm tra trigger names
   - Kiểm tra animation events

3. **Performance issues**:
   - Giảm số NPCs active
   - Tăng attack cooldowns
   - Bật LOD system

## Next Steps

1. **Implement AI Behaviors**:
   - State Machine cho complex behaviors
   - Group coordination
   - Advanced pathfinding

2. **Expand Skill System**:
   - Passive skills
   - Buff/debuff system
   - Combo skills

3. **Add Visual Effects**:
   - Attack effects
   - Skill effects
   - UI health bars

4. **Audio Integration**:
   - Attack sounds
   - Skill sounds
   - Voice lines

## Maintenance

### Code Review:
- Regular performance profiling
- Code cleanup
- Documentation updates

### Version Control:
- Commit character data separately
- Tag stable versions
- Document breaking changes

---

**Note**: Architecture này được thiết kế để dễ dàng mở rộng. Khi cần thêm character types mới, chỉ cần follow pattern hiện có và implement các abstract methods.
