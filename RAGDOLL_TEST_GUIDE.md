# Hướng Dẫn Test Ragdoll với SimpleDemo

## Tổng Quan
Đã setup system ragdoll hoàn chỉnh cho việc test với các character model từ prefab. System bao gồm:

1. **RagdollControllerUI** - Component chính quản lý ragdoll state
2. **SimpleRagdollDemo** - Component demo đơn giản cho test
3. **RagdollTestManager** - Manager để spawn và test nhiều character
4. **RagdollSetupHelper** - Helper tự động setup ragdoll cho character

## Scripts Đã Tạo

### 1. RagdollTestManager.cs
- **Chức năng**: Spawn character từ prefab và test ragdoll
- **Vị trí**: `Assets/AnimalRevolt/Scripts/Ragdoll/RagdollTestManager.cs`
- **Controls**:
  - T: Spawn Character
  - C: Clear All Characters
  - R: Toggle All Ragdoll

### 2. RagdollSetupHelper.cs  
- **Chức năng**: Tự động setup ragdoll cho character có sẵn
- **Vị trí**: `Assets/AnimalRevolt/Scripts/Ragdoll/RagdollSetupHelper.cs`
- **Context Menu**:
  - Setup Ragdoll
  - Test Ragdoll Toggle
  - Test Apply Force
  - Reset Ragdoll

## Cách Test

### Phương Pháp 1: Sử dụng Character Có Sẵn

1. **Mở Scene SimpleDemo**
   - Scene đã có character Warrok tại position (0, 2, 0)

2. **Setup Ragdoll cho Character**
   - Select character "TestCharacter_Warrok"
   - Add component `RagdollSetupHelper`
   - Right-click → Context Menu → "Setup Ragdoll"
   - Script sẽ tự động:
     - Thêm Rigidbody, Collider cho các bones
     - Setup CharacterJoint
     - Thêm RagdollControllerUI và SimpleRagdollDemo

3. **Test Ragdoll**
   - Right-click character → "Test Ragdoll Toggle"
   - Right-click character → "Test Apply Force"
   - Hoặc dùng keyboard:
     - Space: Toggle ragdoll
     - Left Click: Apply force

### Phương Pháp 2: Sử dụng RagdollTestManager

1. **Setup Manager**
   - GameObject "RagdollTestManager" đã có trong scene
   - Add component `RagdollTestManager` (sau khi Unity compile)
   - Assign prefabs vào Test Character Prefabs list trong Inspector

2. **Test Multiple Characters**
   - T: Spawn character từ prefab
   - C: Clear all characters
   - R: Toggle ragdoll cho tất cả
   - Context Menu: Apply Explosion Force

## Prefabs Khuyến Nghị Test

### Từ Assets/Prefabs/
- **Warrok W Kurniawan.fbx** - Character chính đã có trong scene

### Từ Assets/Military/Prefab/
- SK_Military_Survivalist (1).prefab
- SK_Military_Survivalist (2).prefab  
- SK_Military_Survivalist (3).prefab
- SK_Military_Survivalist (4).prefab

## Lưu Ý Quan Trọng

### 1. Ragdoll đã có sẵn từ Unity
- Character models đã được bạn add ragdoll từ Unity
- Scripts chỉ cần setup components để control ragdoll
- Không cần tạo ragdoll từ đầu

### 2. Physics Setup
- Default Mass: 1.0
- Default Drag: 0.5  
- Default Angular Drag: 5.0
- Ragdoll Layer: Layer 8
- Continuous Collision Detection: Enabled

### 3. Input Actions
Cần setup Input Actions trong Project Settings:
- Jump Action → Space key (toggle ragdoll)
- Attack Action → Left Click (apply force)
- Custom actions cho RagdollTestManager

## Troubleshooting

### Script không compile
- Kiểm tra tất cả using statements
- Đảm bảo namespace đúng
- Refresh Unity Editor (Ctrl+R)

### Ragdoll không hoạt động
- Kiểm tra character có Rigidbody không
- Kiểm tra Collider đã enabled chưa
- Xem Console log để debug

### Character rơi qua ground
- Kiểm tra Ground có Collider không
- Kiểm tra Ragdoll Layer collision matrix
- Đảm bảo Physics Material phù hợp

## Next Steps

1. **Compile Scripts**: Unity cần compile scripts trước khi dùng
2. **Add Components**: Thêm components vào GameObjects
3. **Setup Input Actions**: Configure input trong Project Settings  
4. **Test và Adjust**: Fine-tune physics parameters
5. **Add More Prefabs**: Test với các character khác

## Demo Controls Summary

**SimpleRagdollDemo (per character):**
- Space: Toggle Ragdoll
- Left Click: Apply Force
- Context Menu: Various ragdoll actions

**RagdollTestManager (global):**
- T: Spawn Character
- C: Clear All  
- R: Toggle All Ragdoll
- Context Menu: Explosion Force

**RagdollSetupHelper (setup):**
- Context Menu: Setup Ragdoll
- Context Menu: Test functions