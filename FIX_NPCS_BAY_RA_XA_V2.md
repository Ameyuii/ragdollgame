# 🚀 HƯỚNG DẪN FIX NPCs BAY RA XA

## 🎯 VẤN ĐỀ
2 nhân vật NPC khi chạm vào nhau bị bay ra xa do va chạm vật lý quá mạnh.

## ✅ GIẢI PHÁP ĐÃ THỰC HIỆN

### 1. **Giảm Force Impact Mạnh**
- Giảm `impactForce` từ 200N → **50N** trong NPCController
- Thêm kiểm tra khoảng cách: chỉ đẩy khi distance ≤ 2.5m
- Force chủ yếu theo trục ngang, rất ít lực lên trên (0.1f thay vì 0.5f)

### 2. **Tạo NPCAntiCollisionFix.cs**
Script tự động khắc phục va chạm:
- **Separation Force**: Tự động tách NPCs khi quá gần (< 1.2m)
- **Velocity Limiting**: Giới hạn tốc độ tối đa 3m/s
- **Auto Reset**: Tự động reset NPC về mặt đất nếu bay quá cao
- **Physics Damping**: Giảm tốc độ tự nhiên

### 3. **QuickNPCAntiCollisionFix.cs**
Tool tự động áp dụng fix cho tất cả NPCs:
- Auto-fix Rigidbody settings
- Auto-fix NavMeshAgent settings  
- Tự động thêm NPCAntiCollisionFix component
- Context menu để test và reset

## 🛠️ CÁCH SỬ DỤNG

### Bước 1: Tự Động Fix Tất Cả NPCs
```csharp
// Trong Unity Editor:
1. Tạo Empty GameObject trong scene
2. Add component QuickNPCAntiCollisionFix
3. Tick "Auto Fix On Start" và "Fix All NPCs In Scene"
4. Chạy game - sẽ tự động fix tất cả NPCs
```

### Bước 2: Manual Fix Từng NPC
```csharp
// Right-click trên QuickNPCAntiCollisionFix component:
- "Fix All NPC Collisions" - Fix tất cả
- "Reset All NPCs to Ground" - Reset vị trí về mặt đất
- "Test NPC Collision Settings" - Kiểm tra settings
```

### Bước 3: Fix Thủ Công (Nếu Cần)
```csharp
// Thêm vào từng NPC prefab:
1. Add component NPCAntiCollisionFix
2. Adjust Rigidbody: Mass=60, Drag=3, AngularDrag=8
3. Adjust NavMeshAgent: Radius=0.3, StoppingDistance=2
4. Add RigidbodyConstraints: FreezeRotationX | FreezeRotationZ
```

## ⚙️ CÀI ĐẶT TỐI ưU

### Rigidbody Settings:
- **Mass**: 60f (khối lượng vừa phải)
- **Drag**: 3f (giảm tốc nhanh)
- **Angular Drag**: 8f (giảm xoay)
- **Interpolation**: Interpolate (smooth movement)
- **Collision Detection**: ContinuousDynamic

### NavMeshAgent Settings:
- **Radius**: 0.3f (nhỏ hơn để tránh va chạm)
- **Stopping Distance**: 2f (dừng xa hơn)
- **Obstacle Avoidance**: MedQualityObstacleAvoidance
- **Avoidance Priority**: Random 40-60

### NPCController Physics:
- **Impact Force**: 50N (thay vì 200N)
- **Distance Check**: Chỉ đẩy khi ≤ 2.5m
- **Up Force**: 0.1f (rất nhẹ)
- **Anti-Collision Reduction**: 50% nếu có NPCAntiCollisionFix

## 🧪 TESTING

### Test Collision Fix:
1. Chạy game
2. Để 2 NPCs tấn công nhau
3. Quan sát: NPCs chỉ bị đẩy nhẹ, không bay ra xa
4. Check Console logs để xem force values

### Test Auto Reset:
1. Force NPCs bay lên cao (test)
2. NPCAntiCollisionFix sẽ tự động reset về mặt đất
3. Check logs: "RESET VỊ TRÍ do bay quá xa!"

### Debug Commands:
```csharp
// Trong QuickNPCAntiCollisionFix:
[ContextMenu("Test NPC Collision Settings")] // Kiểm tra settings
[ContextMenu("Reset All NPCs to Ground")]    // Reset về mặt đất
[ContextMenu("Fix All NPC Collisions")]     // Apply fixes
```

## 📊 TRƯỚC VS SAU

### TRƯỚC FIX:
- Impact Force: 200-500N → NPCs bay ra xa
- Không có velocity limiting
- Không có collision separation
- NavMeshAgent overlap gây va chạm

### SAU FIX:
- Impact Force: 50N → Chỉ đẩy nhẹ
- Max velocity: 3m/s
- Auto separation khi < 1.2m distance
- NavMeshAgent optimized với avoidance
- Auto reset nếu bay quá xa

## 🔧 FILE ĐÃ THAY ĐỔI

1. **NPCController.cs**: Giảm force impact và thêm protection
2. **NPCAntiCollisionFix.cs**: NEW - Component chống va chạm
3. **QuickNPCAntiCollisionFix.cs**: NEW - Tool auto-fix
4. **NPCCollisionFixer.cs**: CŨ - Vẫn hoạt động

## 🎮 KẾT QUẢ MONG MUỐN

✅ NPCs tấn công nhau mà không bay ra xa  
✅ Va chạm vật lý tự nhiên nhưng được kiểm soát  
✅ Ragdoll chỉ kích hoạt khi cần (máu thấp/chết)  
✅ NPCs tự động tách ra khi quá gần  
✅ Auto-reset nếu có bug bay lên cao  

## 🚨 LƯU Ý

- **Test trong Unity Editor** để đảm bảo hoạt động tốt
- **Backup scene** trước khi áp dụng fixes
- **Adjust parameters** trong NPCAntiCollisionFix nếu cần
- **Check Console logs** để debug collision issues

---
**Tác giả**: GitHub Copilot  
**Ngày tạo**: Hôm nay  
**Status**: ✅ HOÀN THÀNH - Sẵn sàng test
