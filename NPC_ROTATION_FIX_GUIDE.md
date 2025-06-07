# Fix NPC Rotation Loop - Quick Test Guide

## Vấn đề gốc
2 NPC sau khi bắt đầu game quay vào nhau và không di chuyển

## Các fix đã thực hiện

### 1. CombatController.cs - HandleEngaging()
- ✅ Thêm `effectiveStoppingDistance` để tránh infinite approach
- ✅ Offset destination position để tránh collision
- ✅ Set `navAgent.stoppingDistance` để control approach distance

### 2. CombatController.cs - HandleRotation()  
- ✅ Thêm anti-loop logic khi 2 NPC target nhau
- ✅ Chỉ cho NPC có instanceID thấp hơn được rotate khi quá gần

### 3. CombatController.cs - HandleNavMeshMovement()
- ✅ Thêm anti-stuck detection
- ✅ Smart target position calculation
- ✅ Path validation và fallback logic

### 4. CombatController.cs - ValidateNavMeshAgentSettings()
- ✅ Auto-fix NavMeshAgent settings trong Awake()
- ✅ Đảm bảo speed, acceleration, stoppingDistance hợp lệ
- ✅ Set avoidance priority để tránh conflict

### 5. CombatDebugHelper.cs
- ✅ Monitor stuck movement
- ✅ Detect rotation loops  
- ✅ Auto-fix capabilities

## Testing Steps

1. **Start Scene**: Play scene và quan sát 2 NPC
2. **Check Console**: Xem debug logs để verify logic
3. **Observe Behavior**: 
   - NPCs sẽ approach nhau
   - Dừng ở khoảng cách an toàn (~1.5-2m)
   - Không quay vòng vô tận
   - Combat animation sẽ trigger khi đủ gần

## Debug Logs để tìm

```
🎯 [COMBAT ENGAGING] ... CẦN DI CHUYỂN đến target
🔧 [COMBAT ENGAGING] ... SET NavAgent.stoppingDistance = X.XX
🔄 [ROTATION ANTI-LOOP] ... shouldRotate = true/false
🛑 [COMBAT ENGAGING] ... KHÔNG CẦN DI CHUYỂN - đã đủ gần target
```

## NavMesh Requirements

- NavMesh phải được bake trong scene
- Ground objects cần có "Navigation Static" flag  
- NavMesh settings: agentRadius=0.5, agentHeight=2

## Nếu vẫn có vấn đề

1. Check NavMesh bake status
2. Verify TeamMember settings (different teams)
3. Check console cho error messages
4. Attach CombatDebugHelper vào empty GameObject

## Expected Result
✅ NPCs sẽ approach, combat, và có movement behavior bình thường
❌ Không còn infinite rotation loop
❌ Không còn stuck movement
