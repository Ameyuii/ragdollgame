# FIX NHANH: NPCs Bay Ra Xa Khi Va Chạm

## Vấn đề
2 nhân vật khi chạm vào nhau bị "bay ra xa" do:
- Lực physics impact quá mạnh (500N)
- NavMeshAgent collision settings không phù hợp
- Ragdoll được kích hoạt không đúng lúc

## Giải pháp nhanh

### 1. Sử dụng QuickNPCPhysicsFix (Khuyến nghị)

```
1. Tạo Empty GameObject trong scene
2. Attach script "QuickNPCPhysicsFix" 
3. Click Play hoặc dùng Context Menu "Fix All NPCs"
```

Script này sẽ tự động:
- ✅ Giảm lực tấn công từ 500N xuống 150N
- ✅ Tăng Rigidbody drag lên 3.0 (giảm trượt)
- ✅ Giảm mass xuống 50kg (nhẹ hơn)
- ✅ Điều chỉnh NavMeshAgent radius và stopping distance
- ✅ Thêm NPCCollisionFixer component

### 2. Điều chỉnh thủ công

#### A. Physics Settings:
```
Rigidbody:
- Mass: 50 (thay vì 70)
- Drag: 3.0 (thay vì 0.0)
- Angular Drag: 5.0
- Constraints: Freeze Rotation X,Z

NavMeshAgent:
- Radius: 0.3 (thay vì 0.5)
- Stopping Distance: 2.0
- Obstacle Avoidance: High Quality
```

#### B. Code Changes (nếu có thể sửa NPCController):
```csharp
// Trong AddPhysicsImpact(), giảm force:
float impactForce = 150f; // Thay vì 500f

// Chỉ kích hoạt ragdoll khi cần thiết:
if (target.currentHealth <= target.maxHealth * 0.3f)
{
    // Kích hoạt ragdoll
}
```

### 3. Emergency Reset

Nếu NPCs đã bay quá xa:
```
1. Select QuickNPCPhysicsFix GameObject
2. Right-click → Context Menu → "Reset All NPCs"
```

## Kiểm tra kết quả

Sau khi áp dụng fix:
- ✅ NPCs vẫn tấn công nhau bình thường
- ✅ Có knockback nhẹ khi bị đánh
- ✅ Không bị bay ra xa
- ✅ Ragdoll chỉ kích hoạt khi máu thấp hoặc chết

## Debug Tips

### Console Logs để theo dõi:
```
💥 [NPC]: Đẩy nhẹ [Target] với lực 150
🛑 [NPC]: Giảm tốc độ từ va chạm (X.XX)
🔧 [NPC]: Fixed Rigidbody settings
```

### Kiểm tra trong Inspector:
```
Rigidbody Velocity < 5.0 (bình thường)
NavMeshAgent.velocity < 3.5 (tốc độ di chuyển)
```

## Lưu ý quan trọng

1. **Backup project** trước khi thay đổi
2. **Test với 2 NPCs** trong scene nhỏ trước
3. **Kiểm tra layerMask** - NPCs phải ở layer phù hợp
4. **Unity Physics Settings** - Time.fixedDeltaTime = 0.02 (50Hz)

## Troubleshooting

### NPCs vẫn bay xa:
- Tăng Rigidbody drag lên 5.0
- Giảm impact force xuống 100N
- Kiểm tra có Collider nào khác can thiệp không

### NPCs không tấn công:
- Kiểm tra team settings (khác team mới tấn công)
- Kiểm tra enemyLayerMask
- Xem debug logs trong Console

### NPCs không di chuyển:
- Kiểm tra NavMeshAgent enabled = true
- Kiểm tra isStopped = false
- Ensure có NavMesh trên ground

Áp dụng QuickNPCPhysicsFix sẽ fix nhanh nhất! 🚀
