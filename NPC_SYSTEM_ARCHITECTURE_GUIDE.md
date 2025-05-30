# 🏗️ NPC SYSTEM ARCHITECTURE GUIDE

## 📋 TỔNG QUAN THIẾT KẾ

Thiết kế này cho phép tạo nhiều loại nhân vật với skill, đòn đánh và animation khác nhau một cách tối ưu và dễ bảo trì.

## 🎯 NGUYÊN TẮC THIẾT KẾ

### ✅ **DỮ LIỆU CHUNG** (Shared Components)
- Health System (máu, team, chết/sống)
- Movement System (di chuyển, NavMesh, rotation)
- Target Detection (tìm kẻ địch, phạm vi phát hiện)
- Base AI Logic (states cơ bản)

### ⚔️ **DỮ LIỆU RIÊNG** (Character-Specific)
- Attack Patterns (đòn đánh khác nhau)
- Skill Systems (kỹ năng đặc biệt)
- Animation Controllers (hoạt ảnh riêng)
- Character Stats (chỉ số đặc trưng)

## 🏗️ KIẾN TRÚC SCRIPT

### **1. BASE CLASSES (Abstract)**

```
NPCBaseController (Abstract)
├── Health Management
├── Team System
├── Movement Control (NavMesh)
├── Target Detection & AI
├── Death/Respawn Logic
└── Animation Event Handling
```

```
AttackSystemBase (Abstract)
├── Attack Range/Damage Calculation
├── Cooldown Management
├── Target Validation
├── Hit Detection & Physics
└── Animation Synchronization
```

### **2. CHARACTER IMPLEMENTATIONS**

```
WarriorController : NPCBaseController
├── MeleeAttackSystem : AttackSystemBase
├── WarriorSkillSystem
└── Heavy Armor Mechanics

MageController : NPCBaseController
├── RangedMagicSystem : AttackSystemBase
├── ManaSystem
└── Spell Casting Mechanics

ArcherController : NPCBaseController
├── ProjectileSystem : AttackSystemBase
├── ArcherSkillSystem
└── Ranged Combat Mechanics
```

### **3. DATA-DRIVEN DESIGN (ScriptableObjects)**

```
CharacterData.asset
├── Base Stats (HP, Speed, Team, Detection Range)
├── Attack Configuration (Damage, Range, Cooldown)
├── Available Skills List
└── Animation Controller Reference
```

```
SkillData.asset
├── Skill Metadata (Name, Description, Icon)
├── Execution Data (Cooldown, Cost, Duration)
├── Animation Triggers
├── Effect Prefabs
└── Target Requirements
```

```
AttackVariation.asset
├── Attack Identifier
├── Animation Trigger Name
├── Damage Multiplier
├── Special Effects
├── Hit Timing Configuration
└── Combo Potential
```

## 📁 CẤU TRÚC FOLDER

```
Assets/Scripts/
├── Core/
│   ├── NPCBaseController.cs
│   ├── AttackSystemBase.cs
│   ├── Interfaces/
│   │   ├── ISkill.cs
│   │   ├── IAttackable.cs
│   │   └── ITargetable.cs
│   └── Components/
│       ├── HealthComponent.cs
│       ├── MovementComponent.cs
│       ├── TargetingComponent.cs
│       └── AnimationComponent.cs
├── Characters/
│   ├── Warrior/
│   │   ├── WarriorController.cs
│   │   ├── WarriorAttackSystem.cs
│   │   └── WarriorSkills/
│   ├── Mage/
│   │   ├── MageController.cs
│   │   ├── MageAttackSystem.cs
│   │   └── MageSkills/
│   └── Archer/
│       ├── ArcherController.cs
│       ├── ArcherAttackSystem.cs
│       └── ArcherSkills/
├── Skills/
│   ├── SkillSystem.cs
│   ├── SkillManager.cs
│   └── SkillTypes/
│       ├── InstantSkill.cs
│       ├── ProjectileSkill.cs
│       ├── AOESkill.cs
│       └── ChannelSkill.cs
├── Data/
│   ├── ScriptableObjects/
│   │   ├── CharacterData.cs
│   │   ├── SkillData.cs
│   │   └── AttackData.cs
│   └── Enums/
│       ├── CharacterType.cs
│       ├── SkillType.cs
│       └── AttackType.cs
└── Utilities/
    ├── AnimationEventHandler.cs
    ├── EffectManager.cs
    ├── DamageCalculator.cs
    └── AIStateMachine.cs
```

## 🎮 ANIMATION SYSTEM

### **Animator Structure**
- **Base Controller**: States chung (Idle, Walk, Hit, Die)
- **Override Controllers**: Character-specific animations
- **Sub-State Machines**: Attack patterns cho từng loại character

### **Animation Events System**
```
AnimationEventHandler.cs
├── OnAttackStart() - Bắt đầu attack
├── OnAttackHit() - Thời điểm gây damage
├── OnAttackEnd() - Kết thúc attack
├── OnSkillCast() - Cast skill
├── OnEffectTrigger() - Trigger effects
└── OnMovementChange() - Thay đổi di chuyển
```

## ⚡ SKILL SYSTEM DESIGN

### **Interface Pattern**
```csharp
public interface ISkill
{
    bool CanUse(NPCBaseController caster);
    void Execute(NPCBaseController caster, Vector3 targetPosition);
    float GetCooldown();
    float GetManaCost();
    SkillType GetSkillType();
}
```

### **Skill Categories**
- **Instant Skills**: Buff bản thân, Heal, Teleport
- **Projectile Skills**: Fireball, Magic Missile, Arrow
- **AOE Skills**: Earthquake, Explosion, Area Heal
- **Channel Skills**: Charging attacks, Continuous damage

## 🌟 LỢI ÍCH CỦA ARCHITECTURE

### ✅ **Tái sử dụng Code**
- Base classes xử lý tất cả logic chung
- Components có thể mix & match freely
- ScriptableObjects cho phép tạo variants nhanh chóng

### ✅ **Dễ Mở Rộng**
- Thêm character type mới chỉ cần inherit base classes
- Thêm skill mới qua ScriptableObject creation
- Animation override controllers cho variety

### ✅ **Maintainability**
- Separation of concerns rõ ràng
- Data và logic tách biệt hoàn toàn
- Component-based debugging dễ dàng

### ✅ **Designer Friendly**
- Non-programmers có thể tạo characters
- Visual parameter tweaking trong Inspector
- Drag & drop skill assignment

## 🎯 EXAMPLE SETUP

### **Warrior Character**
```
Assets/Characters/Warrior/
├── WarriorController.cs (Inherit NPCBaseController)
├── WarriorData.asset (ScriptableObject)
├── WarriorAnimator.controller (Animator)
├── Skills/
│   ├── SwordSlash.asset (SkillData)
│   ├── BerserkerRage.asset (SkillData)
│   └── ShieldBlock.asset (SkillData)
└── Attacks/
    ├── BasicSwordAttack.asset (AttackData)
    ├── HeavySlash.asset (AttackData)
    └── ComboFinisher.asset (AttackData)
```

### **Mage Character**
```
Assets/Characters/Mage/
├── MageController.cs (Inherit NPCBaseController)
├── MageData.asset (ScriptableObject)
├── MageAnimator.controller (Animator)
├── Skills/
│   ├── Fireball.asset (SkillData)
│   ├── Teleport.asset (SkillData)
│   └── ManaShield.asset (SkillData)
└── Attacks/
    ├── StaffHit.asset (AttackData)
    ├── MagicBolt.asset (AttackData)
    └── AreaBlast.asset (AttackData)
```

## 🚀 IMPLEMENTATION ROADMAP

### **Phase 1: Core Foundation**
1. Tái cấu trúc NPCController thành NPCBaseController
2. Tạo AttackSystemBase abstract class
3. Thiết lập ScriptableObject data structures
4. Implement component system

### **Phase 2: Character Specialization**
1. Tạo WarriorController với melee combat
2. Implement basic skill system
3. Animation integration và events
4. Testing và debugging

### **Phase 3: Expansion**
1. Thêm MageController với magic system
2. ArcherController với projectile system
3. Advanced skill combinations
4. Polish và optimization

## 📝 NOTES & BEST PRACTICES

- **Luôn sử dụng interfaces** cho loose coupling
- **ScriptableObjects cho tất cả data** configuration
- **Component pattern** cho functionality modules
- **Animation Events** cho precise timing
- **Object Pooling** cho projectiles và effects
- **State Machine** cho complex AI behaviors

---
**📅 Tạo**: {DateTime.Now:dd/MM/yyyy}
**🎯 Mục đích**: Hướng dẫn thiết kế architecture cho NPC system có thể scale
**👤 Tác giả**: GitHub Copilot AI Assistant
