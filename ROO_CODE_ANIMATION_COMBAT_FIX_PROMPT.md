# ROO CODE - ANIMATION COMBAT FIX PROMPT

## 🎯 NHIỆM VỤ CHÍNH
Kiểm tra và sửa hệ thống animation combat - hiện tại logic combat đã hoạt động (trừ HP) nhưng chưa có animation tấn công.

## 📋 YÊU CẦU CỤ THỂ

### 1. KIỂM TRA ANIMATOR CONTROLLER
- Mở Animator Controller của NPC characters
- Xác nhận có 3 animation attack triggers:
  - **Attack** (Trigger)
  - **Attack1** (Trigger) 
  - **Attack2** (Trigger)
- Kiểm tra các parameters bổ sung:
  - AttackSpeed (float - tốc độ animation)
  - IsInCombat (bool)
  - AttackIndex (int - để track animation hiện tại)

### 2. PHÂN TÍCH CODE HIỆN TẠI
Kiểm tra các file:
- `Assets/AnimalRevolt/Scripts/Combat/CombatController.cs`
- `Assets/AnimalRevolt/Scripts/AI/AIMovementController.cs`  
- `Assets/AnimalRevolt/Scripts/Animation/AnimationController.cs` (nếu có)

### 3. XÁC ĐỊNH VẤN ĐỀ
- Logic combat đã hoạt động (damage đã được tính)
- NPC đã tiến lại gần nhau trong combat
- Nhưng chưa thấy animation attack được trigger

### 4. GIẢI PHÁP CẦN THỰC HIỆN

#### A. Sửa CombatController.cs - RANDOM ATTACK SYSTEM:
```csharp
// Thêm biến để track attack patterns
private string[] attackTriggers = {"Attack", "Attack1", "Attack2"};
private string lastAttackUsed = "";
private int consecutiveAttackCount = 0;
private const int maxConsecutiveAttacks = 2; // Có thể lặp lại 1 animation 2 lần

// Hàm chọn attack animation ngẫu nhiên
private string GetRandomAttackTrigger()
{
    string selectedAttack;
    
    // Nếu đã dùng cùng 1 attack 2 lần liên tiếp, buộc phải đổi
    if (consecutiveAttackCount >= maxConsecutiveAttacks)
    {
        // Chọn attack khác (loại trừ attack hiện tại)
        var availableAttacks = attackTriggers.Where(a => a != lastAttackUsed).ToArray();
        selectedAttack = availableAttacks[Random.Range(0, availableAttacks.Length)];
        consecutiveAttackCount = 1;
    }
    else
    {
        // Random hoàn toàn
        selectedAttack = attackTriggers[Random.Range(0, attackTriggers.Length)];
        
        // Track consecutive count
        if (selectedAttack == lastAttackUsed)
        {
            consecutiveAttackCount++;
        }
        else
        {
            consecutiveAttackCount = 1;
        }
    }
    
    lastAttackUsed = selectedAttack;
    return selectedAttack;
}

// Trigger animation attack với random selection
private void TriggerAttackAnimation()
{
    if (animator != null)
    {
        string attackTrigger = GetRandomAttackTrigger();
        animator.SetTrigger(attackTrigger);
        
        Debug.Log($"🎬 [ANIMATION] {gameObject.name} TRIGGER {attackTrigger} (Consecutive: {consecutiveAttackCount})");
    }
}

// Gọi trong hàm PerformAttack() hoặc tương tự
private void PerformAttack()
{
    TriggerAttackAnimation(); // Random attack animation
    // ...logic damage hiện tại...
}
```

#### B. Kiểm tra Animator Parameters:
- Đảm bảo có 3 triggers: **Attack**, **Attack1**, **Attack2**
- Thiết lập transition từ Idle/Walk → Attack/Attack1/Attack2 → Idle
- Đặt duration phù hợp cho từng animation attack
- **Quan trọng**: Đảm bảo cả 3 animation có cùng timing để damage sync đúng

#### C. Synchronize với Combat Logic:
```csharp
// Trong CombatController, đảm bảo animation sync với damage
private void OnAttackAnimationComplete()
{
    // Reset consecutive count nếu cần
    if (consecutiveAttackCount >= maxConsecutiveAttacks)
    {
        Debug.Log($"🎬 [ANIMATION] {gameObject.name} Reset attack pattern");
    }
    
    // Continue combat logic
    CheckForNextAttack();
}

// Optional: Thêm variety cho attack patterns
private void ResetAttackPattern()
{
    lastAttackUsed = "";
    consecutiveAttackCount = 0;
    Debug.Log($"🎬 [ANIMATION] {gameObject.name} Attack pattern reset");
}
```

### 5. KIỂM TRA VÀ TEST

#### A. Verify Animation Setup:
1. Kiểm tra Animator Controller có cả 3 transition: Attack, Attack1, Attack2
2. Test từng animation trong Scene view
3. Đảm bảo cả 3 animation có duration tương tự để combat timing nhất quán
4. Kiểm tra transition conditions đúng cho cả 3 triggers

#### B. Debug Animation Triggers:
```csharp
// Thêm debug log để kiểm tra random system
Debug.Log($"🎬 [ANIMATION] Triggering {attackTrigger} for {gameObject.name}");
Debug.Log($"🎬 [ANIMATION] Consecutive count: {consecutiveAttackCount}/{maxConsecutiveAttacks}");
Debug.Log($"🎬 [ANIMATION] Last attack used: {lastAttackUsed}");
Debug.Log($"🎬 [ANIMATION] Available triggers: {string.Join(", ", attackTriggers)}");
```

#### C. Test Combat Scenario:
1. Đặt 2 NPC khác team gần nhau
2. Quan sát combat logic hoạt động
3. Xác nhận cả 3 animation attack được random trigger
4. Kiểm tra pattern: cùng 1 animation có thể lặp tối đa 2 lần
5. Verify animation sync với damage timing
6. Test visual variety trong combat

### 6. BỔ SUNG NẾU CẦN

#### Nếu thiếu Animation Files:
- Đã có 3 animation attacks với triggers: Attack, Attack1, Attack2
- Kiểm tra animation clips được assign đúng trong Animator Controller
- Đặt Animation Events nếu cần timing chính xác cho damage
- Đảm bảo cả 3 animation có visual variety rõ ràng

#### Nếu cần Animation Events:
```csharp
// Trong animation, thêm Animation Event tại frame damage
public void OnAttackHit()
{
    // Gọi damage logic tại đúng thời điểm animation
    DealDamage();
}
```

## 🎯 KẾT QUẢ MONG MUỐN
- NPC có **3 loại animation attack** random khi combat (Attack, Attack1, Attack2)
- **Hệ thống random thông minh**: cùng 1 animation có thể lặp tối đa 2 lần liên tiếp
- **Visual variety**: Combat trông đa dạng và không nhàm chán
- Animation sync với damage logic
- Không làm hỏng logic combat hiện tại

## 📝 GHI CHÚ QUAN TRỌNG
- **THỰC HIỆN CÁC YÊU CẦU TRONG FILE RULE VÀ SETTING CỦA ROO**
- Giữ nguyên logic combat đã hoạt động
- Chỉ thêm animation triggers, không thay đổi damage calculation
- Test kỹ để đảm bảo animation không làm chậm combat
- Comment code bằng tiếng Việt
- Sử dụng Unity coding conventions

## 🔧 TOOLS ĐỀ XUẤT
1. Mở Window → Animation → Animator để check parameters
2. Sử dụng Debug.Log để trace animation triggers
3. Test trong Scene view với Timeline window
4. Sử dụng Animation window để check timing

---
**Lưu ý**: Prompt này thiết kế hệ thống random attack với 3 animation có sẵn (Attack, Attack1, Attack2). System cho phép lặp lại cùng 1 animation tối đa 2 lần để tạo variety nhưng không quá repetitive.
