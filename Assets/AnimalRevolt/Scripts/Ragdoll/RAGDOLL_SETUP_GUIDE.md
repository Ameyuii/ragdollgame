# Hướng Dẫn Setup Ragdoll System

## Bước 1: Chuẩn bị Character

1. **Chọn character** trong scene (ví dụ: Military character từ prefab)
2. **Đảm bảo character có:**
   - Animator component
   - Rigidbody và Collider trên các bones
   - CharacterJoint để nối các bones

## Bước 2: Add Components

### Cách 1: Sử dụng RagdollControllerUI (Có UI trong Inspector)

1. **Add component** `RagdollControllerUI` vào character
2. **Add component** `SimpleRagdollDemo` vào character (optional - để test bằng keyboard)

### Cách 2: Sử dụng RagdollController gốc (Context Menu)

1. **Add component** `RagdollController` vào character từ Assets/Scripts/

## Bước 3: Tạo RagdollSettings

1. **Right-click** trong Project window
2. **Create → AnimalRevolt → Ragdoll Settings**
3. **Đặt tên** file (ví dụ: DefaultRagdollSettings)
4. **Cấu hình** các thông số theo ý muốn:
   - Default Mass: 1.0
   - Default Drag: 0.5
   - Transition Duration: 0.2
   - Initial Force Multiplier: 100

## Bước 4: Assign Settings

1. **Chọn character** trong scene
2. **Trong Inspector** của RagdollControllerUI:
   - Drag RagdollSettings vào field "Settings"
   - Animator sẽ tự động được tìm

## Bước 5: Test Ragdoll

### Với UI Buttons (RagdollControllerUI):

1. **Chạy game** (Play mode)
2. **Chọn character** trong scene
3. **Trong Inspector** sẽ thấy:
   - Nút "Enable Ragdoll" (màu xanh lá)
   - Nút "Apply Random Force" (màu vàng)
   - Status hiển thị real-time

### Với Context Menu:

1. **Right-click** vào character trong scene
2. **Chọn "Toggle Ragdoll"** hoặc **"Apply Random Force"**

### Với Keyboard (SimpleRagdollDemo):

1. **Nhấn Space** để toggle ragdoll
2. **Nhấn F** để apply force
3. **Hướng dẫn** sẽ hiển thị trên màn hình

## Bước 6: Tùy Chỉnh

### Trong RagdollSettings:
- **Physics Settings**: Mass, Drag, Angular Drag
- **Joint Configuration**: Break Force, Break Torque  
- **Transition Settings**: Duration, Force Multiplier
- **Performance**: Max Active Ragdolls, Lifetime
- **Collision Layers**: Ragdoll Layer, Collision Mask

### Trong Inspector:
- **Debug Mode**: Bật để xem log messages
- **Show Inspector Buttons**: Hiển thị UI controls

## Troubleshooting

### Không thấy nút UI:
- Đảm bảo đã add `RagdollControllerUI` (không phải `RagdollController`)
- File `RagdollControllerUIEditor.cs` phải ở trong thư mục Editor
- Restart Unity nếu cần

### Ragdoll không hoạt động:
- Kiểm tra character có Rigidbody trên các bones
- Đảm bảo Collider không bị disable
- Kiểm tra RagdollSettings đã được assign

### Context Menu không xuất hiện:
- Right-click vào GameObject trong Hierarchy (không phải Scene view)
- Đảm bảo đã add đúng component

### Lực không được áp dụng:
- Ragdoll phải được enable trước
- Kiểm tra Initial Force Multiplier trong Settings
- Đảm bảo không có constraint cản trở

## Ví Dụ Setup

```
Character GameObject
├── RagdollControllerUI (script)
├── SimpleRagdollDemo (script - optional)
├── Animator
└── Bones with Rigidbody + Collider + CharacterJoint
```

## Lưu Ý

- **RagdollSettings** nên được lưu trong Resources folder để tự động load
- **Play mode** mới có thể test được ragdoll
- **UI buttons** chỉ hoạt động khi đang chạy game
- **Context menu** có thể dùng cả khi không chạy game (nhưng không có hiệu ứng)