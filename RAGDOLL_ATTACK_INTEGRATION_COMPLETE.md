# HƯỚNG DẪN TEST CHỨC NĂNG RAGDOLL KÍCH HOẠT KHI BỊ TẤNG CÔNG

## Chức năng đã hoàn thành ✅

### 1. **Ragdoll Integration Complete**
- ✅ Đã tích hợp `RagdollController` vào `NPCBaseController`
- ✅ Ragdoll kích hoạt **NGAY KHI BỊ TẤNG CÔNG** thay vì chờ HP dưới 30%
- ✅ Sửa tất cả compilation errors
- ✅ Cập nhật Warrior, Mage, Archer controllers

### 2. **Core Changes Made**

#### NPCBaseController.cs:
```csharp
public virtual void TakeDamage(float damage, GameObject? attacker = null)
{
    // ... existing damage logic ...
    
    // 🔥 CHỨC NĂNG MỚI: Kích hoạt ragdoll NGAY KHI BỊ TẤNG CÔNG
    TriggerRagdollOnHit(attacker);
    
    // ... rest of damage logic ...
}

/// <summary>
/// Kích hoạt ragdoll ngay khi bị tấn công (thay vì chờ HP dưới 30%)
/// </summary>
private void TriggerRagdollOnHit(GameObject? attacker)
{
    if (ragdollController == null) return;
    
    Vector3 hitDirection = Vector3.forward; // Default direction
    if (attacker != null)
    {
        hitDirection = (transform.position - attacker.transform.position).normalized;
    }
    
    // Kích hoạt ragdoll với force từ hướng tấn công
    ragdollController.ActivateRagdoll(hitDirection * knockbackForce);
    
    Debug.Log($"🎯 {gameObject.name}: Ragdoll kích hoạt khi bị tấn công!");
}
```

#### Attack System Integration:
- ✅ Thêm `AttackSystemBase.GetDamage()` method
- ✅ Abstract method `InitializeAttackSystem()` cho derived classes
- ✅ Enhanced attack variations với force calculations

### 3. **Test Setup Created**

#### RagdollAttackTest.cs:
- 🧪 Script test tự động cho chức năng ragdoll
- ⌨️ **Space**: Manual attack test
- ⌨️ **R**: Reset NPC
- 📊 GUI hiển thị realtime status
- 🔄 Auto test mỗi 3 giây

## Cách Test

### Step 1: Setup Scene
1. Tạo scene với NPCs (Warrior/Mage/Archer)
2. Đảm bảo NPCs có `RagdollController` component
3. Add `RagdollAttackTest` script vào một GameObject trong scene

### Step 2: Configure Test
```csharp
[Header("Test Settings")]
public NPCBaseController testNPC;    // Drag NPC vào đây
public float testDamage = 10f;       // Damage amount để test
public float autoTestInterval = 3f;  // Auto test interval
```

### Step 3: Run Test
1. **Play scene**
2. **Quan sát Console logs:**
   ```
   === RAGDOLL ATTACK TEST STARTED ===
   🔥 ATTACK TEST: HP trước = 100.0
   💥 Đã gây 10 damage  
   ❤️ HP sau = 90.0
   🎯 Ragdoll status: Active = True
   ✅ SUCCESS: Ragdoll đã kích hoạt ngay khi bị tấn công!
   ```

3. **Manual controls:**
   - Nhấn **SPACE** để test attack
   - Nhấn **R** để reset NPC
   - Quan sát GUI panel ở góc trái màn hình

### Step 4: Verify Results

#### ✅ Expected Behavior (CHỨC NĂNG MỚI):
- Ragdoll kích hoạt **NGAY** khi NPC nhận damage
- Không cần chờ HP xuống dưới 30%
- NPC ngã xuống với physics realistic
- Force direction từ attacker

#### ❌ Old Behavior (ĐÃ SỬA):
- Ragdoll chỉ kích hoạt khi HP < 30%
- NPC vẫn đứng bình thường khi bị tấn công nhẹ

## Architecture Overview

### Before (NPCController.cs - Old):
```csharp
public void TakeDamage(float damage)
{
    currentHealth -= damage;
    
    // Chỉ kích hoạt ragdoll khi HP < 30%
    if (currentHealth <= maxHealth * 0.3f) 
    {
        TriggerRagdoll();
    }
}
```

### After (NPCBaseController.cs - New):
```csharp  
public virtual void TakeDamage(float damage, GameObject? attacker = null)
{
    currentHealth -= damage;
    
    // 🔥 KÍCH HOẠT RAGDOLL NGAY KHI BỊ TẤNG CÔNG
    TriggerRagdollOnHit(attacker);
    
    // Death logic separate
    if (currentHealth <= 0f)
    {
        Die();
    }
}
```

## Files Modified

### Core System:
- ✅ `Assets/Scripts/Core/NPCBaseController.cs` - Main ragdoll integration
- ✅ `Assets/Scripts/Core/AttackSystemBase.cs` - Added GetDamage() method
- ✅ `Assets/Scripts/Core/Skills/SkillBase.cs` - Fixed interface implementation

### Character Controllers:
- ✅ `Assets/Scripts/Characters/Warrior/WarriorController.cs` - Updated for new architecture
- ✅ `Assets/Scripts/Characters/Mage/MageController.cs` - Updated for new architecture  
- ✅ `Assets/Scripts/Characters/Archer/ArcherController.cs` - Updated for new architecture

### Data & Configuration:
- ✅ `Assets/Scripts/Data/CharacterConfigs/CharacterDataFactory.cs` - Fixed properties

### Test Framework:
- ✅ `Assets/Scripts/RagdollAttackTest.cs` - New test script

## Status Summary

🎯 **CHỨC NĂNG HOÀN THÀNH**: Ragdoll kích hoạt ngay khi bị tấn công thay vì chờ HP dưới 30%

### ✅ Completed:
- Ragdoll integration vào NPCBaseController
- Sửa tất cả compilation errors  
- Attack variation system với force calculations
- Test framework cho verify chức năng
- Update tất cả character controllers

### 🔄 Enhanced Features:
- Advanced attack system với multiple variations
- Knockback effects với impact point calculations
- Animation timing synchronization
- Force-based ragdoll activation

### 🧪 Testing:
- Automated test script với GUI
- Manual controls (Space, R keys)
- Real-time status monitoring
- Console logging cho debugging

**Ready for testing!** 🚀
