# Hướng dẫn Test NPCs sau khi sửa Ragdoll System

## 🚀 CÁCH TEST NGAY TRONG UNITY

### 1. Tạo Test Manager GameObject:
1. Trong Unity, tạo Empty GameObject mới
2. Đặt tên: "NPC_TestManager" 
3. Add Component: `NPCTestRunner`
4. Add Component: `NPCMovementFixer`

### 2. Chạy Auto Test:
- Khi vào Play Mode, NPCTestRunner sẽ tự động chạy test
- Xem Console để thấy kết quả chi tiết

### 3. Manual Test:
- Right-click NPCTestRunner → "Start NPC Test"  
- Right-click NPCMovementFixer → "Fix All NPC Movement Issues"
- Right-click NPCMovementFixer → "Create Test Setup"

## 🔍 NHỮNG GÌ ĐÃ ĐƯỢC FIX:

### ✅ Logic Team được khôi phục:
- NPCs chỉ tấn công team khác nhau
- NPCs cùng team sẽ bỏ qua nhau
- Debug log hiển thị team checking

### ✅ Health System Sync:
- NPCHealthComponent đồng bộ với NPCController
- Timing được fix (Start() thay vì Awake())
- Không còn conflict health

### ✅ Debug Logging:
- Chi tiết detection process
- LayerMask auto-fix
- Team checking logs

## 🎯 DẤU HIỆU NPCs HOẠT ĐỘNG ĐÚNG:

### Console Logs nên thấy:
```
🔍 NPC1: Tìm thấy 2 objects trong phạm vi 10m
🎯 NPC1: Phát hiện NPC NPC2 (IsDead: False)  
⚔️ NPC1: NPC2 là địch (Team 1 vs 0)
👁️ NPC1: Có đường nhìn đến NPC2, khoảng cách: 8.5m
🎯 NPC1: Di chuyển đến enemy NPC2
```

### Trong Scene nên thấy:
- NPCs xoay về phía nhau
- Animation đi bộ kích hoạt
- NPCs di chuyển về phía nhau
- Khi gần sẽ tấn công

## ⚠️ NẾU NPCs VẪN KHÔNG DI CHUYỂN:

### Kiểm tra:
1. **NavMesh**: Scene có NavMesh baked chưa?
2. **Team Settings**: NPCs có team khác nhau chưa?
3. **Distance**: NPCs có trong detection range chưa?
4. **Health**: NPCs có bị đánh dấu chết chưa?

### Auto Fix:
- Chạy NPCMovementFixer sẽ tự động fix hầu hết vấn đề
- Tạo test setup với teams khác nhau
- Force NPCs tìm enemies

## 🏗️ SETUP TEST NHANH:

```csharp
// Trong Unity Console, gõ:
GameObject testManager = new GameObject("TestManager");
testManager.AddComponent<NPCTestRunner>();
testManager.AddComponent<NPCMovementFixer>();
```

## 📊 KẾT QUẢ MONG ĐỢI:
- NPCs phát hiện nhau trong 1-2 giây
- Di chuyển về phía nhau  
- Tấn công khi gần
- Health giảm dần qua combat
- Ragdoll kích hoạt khi chết
