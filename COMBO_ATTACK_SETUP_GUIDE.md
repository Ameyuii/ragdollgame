# ⚔️ HƯỚNG DẪN SETUP COMBO ATTACK SYSTEM

## 🎯 Đã thêm vào NPCController.cs:

### ✅ **Tham số mới trong Inspector:**
- **Max Combo Hits**: Số lượng hits tối đa (1=single, 2=double, 3=triple combo)
- **Combo Window**: Thời gian để thực hiện combo tiếp theo (0.5s mặc định)

### ✅ **Logic Combo:**
- Tự động detect combo opportunity
- Support unlimited combo hits
- Auto reset khi hết thời gian

## 🎬 SETUP TRONG UNITY ANIMATOR:

### **Bước 1: Tạo Animation States**
1. Mở **Animator Controller** của NPC
2. Tạo các States:
   ```
   - Idle (có sẵn)
   - Attack1 (animation hiện tại)
   - Attack2 (animation combo thứ 2)  
   - Attack3 (animation combo thứ 3) - tùy chọn
   ```

### **Bước 2: Tạo Parameters**
1. Trong Animator, tab **Parameters**:
   ```
   - Attack (Trigger) - có sẵn
   - ComboNext (Trigger) - mới thêm
   ```

### **Bước 3: Setup Transitions**
```
Idle → Attack1
  - Condition: Attack (trigger)
  - Has Exit Time: false
  - Transition Duration: 0.1

Attack1 → Attack2  
  - Condition: ComboNext (trigger)
  - Has Exit Time: true
  - Exit Time: 0.8 (80% animation)
  - Transition Duration: 0.1

Attack2 → Attack3
  - Condition: ComboNext (trigger)  
  - Has Exit Time: true
  - Exit Time: 0.8
  - Transition Duration: 0.1

Attack1/2/3 → Idle
  - Không có condition
  - Has Exit Time: true
  - Exit Time: 1.0 (kết thúc animation)
```

### **Bước 4: Animation Events (Quan trọng!)**
Cho mỗi Attack animation:
1. **Hit Event**: `OnAttackHit` tại frame damage
2. **End Event**: `OnComboEnd` tại frame cuối (chỉ cho Attack cuối cùng)

## ⚙️ THIẾT LẬP TRONG INSPECTOR:

### **Single Attack (hiện tại):**
```
Max Combo Hits: 1
Combo Window: 0.5
Attack Animation Duration: 1.0
Attack Hit Timing: 0.65
```

### **Double Combo:**
```
Max Combo Hits: 2
Combo Window: 0.8
Attack Animation Duration: 1.2 (tổng thời gian cả 2 hits)
```

### **Triple Combo:**
```
Max Combo Hits: 3
Combo Window: 1.0
Attack Animation Duration: 1.8 (tổng thời gian cả 3 hits)
```

## 🧪 **DEBUG LOGS:**

### **Single Attack:**
```
🎯 NPC_Soldier bắt đầu combo attack Enemy
⚔️ NPC_Soldier gây 20 sát thương cho Enemy!
🏁 NPC_Soldier combo sequence completed
```

### **Combo Attack:**
```
🎯 NPC_Soldier bắt đầu combo attack Enemy
⚔️ NPC_Soldier gây 20 sát thương cho Enemy!
🔥 NPC_Soldier combo hit 2/3 → Enemy
⚔️ NPC_Soldier gây 20 sát thương cho Enemy!
🔥 NPC_Soldier combo hit 3/3 → Enemy
⚔️ NPC_Soldier gây 20 sát thương cho Enemy!
🏁 NPC_Soldier combo sequence completed
```

## ❓ **TROUBLESHOOTING:**

### **Combo không kích hoạt:**
- Kiểm tra `Max Combo Hits > 1`
- Kiểm tra Animation Transition có đúng trigger `ComboNext`
- Kiểm tra `Combo Window` có đủ lớn

### **Combo bị stuck:**
- Thêm Animation Event `OnComboEnd` ở cuối animation
- Kiểm tra Exit Time của transitions

### **Damage không sync với animation:**
- Điều chỉnh `Attack Hit Timing`
- Kiểm tra Animation Event `OnAttackHit`

## 📂 **Files cần chỉnh sửa:**
1. **NPCController.cs** ✅ (Đã sửa xong)
2. **Animator Controller** ⚠️ (Cần setup thêm states + transitions)
3. **Animation Assets** ⚠️ (Cần thêm animation events)

---
**Lưu ý**: Setup Animator là bước quan trọng nhất để combo hoạt động!
