# 🛠️ AI Bug Fix Guide - Khắc Phục Lỗi AI NPC System

## 📋 **Tóm Tắt Vấn Đề Đã Sửa**

### ✅ **Vấn đề 1: Animation Walk không hoạt động**
- **Nguyên nhân**: Animator parameter "IsMoving" không tồn tại
- **Giải pháp**: Enhanced animation system với fallback parameters

### ✅ **Vấn đề 2: AI không tìm đến nhau** 
- **Nguyên nhân**: Detection góc hẹp + Line of sight quá strict
- **Giải pháp**: Mở rộng detection 360° + Better enemy detection

## 🔧 **Thay Đổi Đã Thực Hiện**

### 1. **AIMovementController.cs - Animation System**
```csharp
// ✅ NEW: Enhanced animation với multiple fallback parameters
SetAnimatorParameterSafely("IsMoving", isMoving);
SetAnimatorParameterSafely("Moving", isMoving);
SetAnimatorParameterSafely("IsWalking", isWalking);
SetAnimatorParameterSafely("Walking", isWalking);
SetAnimatorParameterSafely("Walk", isWalking);
```

**Lợi ích:**
- ✅ Hỗ trợ nhiều tên parameter khác nhau
- ✅ Không báo lỗi khi parameter không tồn tại
- ✅ Better debugging logs

### 2. **EnemyDetector.cs - Detection System**
```csharp
// ✅ NEW: Relaxed detection - bỏ angle restriction
if (HasLineOfSight(unit)) // Chỉ cần line of sight
{
    detectedEnemies.Add(unit);
}

// ✅ NEW: 360° detection support
if (detectionAngle >= 360f)
    return true;
```

**Lợi ích:**
- ✅ AI detect enemies trong mọi hướng
- ✅ Better debug logs để track detection
- ✅ Relaxed line of sight checking

## 🎯 **Hướng Dẫn Setup & Testing**

### **Bước 1: Kiểm Tra Animator Setup**

1. **Chọn AI character trong scene**
2. **Mở Animator window** (Window > Animation > Animator)
3. **Kiểm tra Parameters tab:**

**Cần có ít nhất một trong các parameters sau:**
- `IsMoving` (Bool)
- `Moving` (Bool) 
- `IsWalking` (Bool)
- `Walking` (Bool)
- `Walk` (Bool)
- `Speed` (Float)

**Nếu không có parameters nào:**
```
📝 Thêm parameter trong Animator:
1. Click [+] trong Parameters tab
2. Chọn "Bool" 
3. Đặt tên "IsWalking"
4. Tạo transition từ Idle -> Walk khi IsWalking = true
```

### **Bước 2: Kiểm Tra Team Setup**

**Trong Inspector của AI characters:**
```
TeamMember component:
✅ Team Type: AI_Team1 (cho team xanh)
✅ Team Type: AI_Team2 (cho team đỏ)
✅ Is Alive: TRUE
✅ Health: > 0
```

### **Bước 3: Kiểm Tra Detection Settings**

**Trong EnemyDetector component:**
```
✅ Detection Radius: 15-20 (tăng nếu cần)
✅ Detection Angle: 360 (để detect mọi hướng)
✅ Detection Mask: Include layer của enemies
✅ Debug Mode: TRUE (để xem logs)
```

### **Bước 4: Kiểm Tra NavMesh**

1. **Mở Navigation window** (Window > AI > Navigation)
2. **Chọn tab "Bake"**
3. **Click "Bake"** để tạo NavMesh
4. **Kiểm tra AI characters có trên NavMesh không:**
   - Chọn AI character
   - Trong NavMeshAgent component: `Is On NavMesh: True`

## 🔍 **Debug & Troubleshooting**

### **Console Logs Để Theo Dõi:**

**✅ Animation Working:**
```
🚶 [AI Name] animating: Speed=1.25, IsWalking=True, State=Moving
```

**✅ Enemy Detection Working:**
```
🎯 [AI Name] detected enemy: [Enemy Name] (Team: AI_Team2) at distance 8.5m
🔥 [AI Name] tracking 1 enemies
```

**✅ AI State Changes:**
```
[AI Name] AI State: Idle -> Seeking
[AI Name] AI State: Seeking -> Moving
```

### **❌ Troubleshooting Common Issues**

**Animation vẫn không hoạt động:**
```
1. Kiểm tra Animator Controller có được assign không
2. Thêm parameter "IsWalking" vào Animator
3. Tạo transition từ Idle to Walk state
4. Set condition: IsWalking = true
```

**AI vẫn không tìm thấy nhau:**
```
1. Tăng Detection Radius lên 20-30
2. Set Detection Angle = 360
3. Kiểm tra Layer Mask include đúng layer
4. Đảm bảo có NavMesh được bake
5. Kiểm tra teams khác nhau (AI_Team1 vs AI_Team2)
```

**NavMesh Issues:**
```
1. Window > AI > Navigation > Bake
2. Kiểm tra Agent Radius/Height phù hợp
3. Đảm bảo ground có layer phù hợp
4. Clear và Bake lại NavMesh
```

## 📊 **Expected Behavior Sau Khi Sửa**

### **Animation System:**
- ✅ Characters sử dụng walk animation khi di chuyển
- ✅ Smooth transition giữa Idle và Walk
- ✅ Không còn "Parameter does not exist" warnings

### **Enemy Detection:**
- ✅ AI teams tự động tìm và chase enemies
- ✅ Detection hoạt động 360° quanh character
- ✅ Better debug logs trong Console

### **Movement Behavior:**
- ✅ AI automatically seek nearby enemies
- ✅ Smooth movement với NavMesh
- ✅ State transitions: Idle → Seeking → Moving → Combat

## 🚀 **Testing Scenarios**

### **Test 1: Animation**
1. Tạo 2 AI characters khác team
2. Đặt cách nhau 10-15 units
3. Play scene
4. **Expected**: Characters walk towards each other

### **Test 2: Detection**
1. Set Detection Radius = 20
2. Set Detection Angle = 360
3. Enable Debug Mode = true
4. **Expected**: Console shows detection logs

### **Test 3: Combat**
1. Để AI characters đến gần nhau
2. **Expected**: State changes to Combat khi gần

## 📝 **Additional Tips**

### **Performance Optimization:**
- Update Interval = 0.1-0.2s (tối ưu performance)
- Max Targets = 3-5 (giới hạn số enemies track)
- Debug Mode = false (trong production)

### **Enhanced Features:**
- AI hỗ trợ multiple animation parameters
- Fallback system cho missing components
- Better error handling và debugging
- 360° enemy detection capability

---

**🎯 Kết quả mong đợi:** Sau khi áp dụng fix này, AI characters sẽ tự động tìm thấy nhau, di chuyển với walk animation, và engage in combat khi đến gần.