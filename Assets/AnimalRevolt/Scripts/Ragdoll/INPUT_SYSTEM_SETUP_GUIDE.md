# Hướng dẫn Setup Input System cho SimpleRagdollDemo

## Vấn đề đã được sửa

Lỗi `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings.` đã được sửa thành công.

## Những thay đổi đã thực hiện

### 1. Cập nhật SimpleRagdollDemo.cs
- Thay thế `UnityEngine.Input.GetKeyDown()` bằng Input System mới
- Sử dụng `InputActionReference` thay vì `KeyCode`
- Thêm `OnEnable()/OnDisable()` để quản lý lifecycle của Input Actions
- Thêm public properties để Editor có thể truy cập

### 2. Tạo SimpleRagdollInputSetup.cs
- ScriptableObject utility để setup Input Actions
- Có thể tự động setup cho tất cả SimpleRagdollDemo trong scene
- Sử dụng reflection để set private fields

### 3. Tạo SimpleRagdollDemoEditor.cs  
- Custom Editor với UI thân thiện
- Button "Auto Setup Input Actions" để setup tự động
- Test controls trong Editor khi runtime
- Hỗ trợ manual assignment của Input Actions

## Cách sử dụng

### Phương pháp 1: Auto Setup (Khuyến nghị)
1. Chọn GameObject có SimpleRagdollDemo component
2. Trong Inspector, click button **"Auto Setup Input Actions"**
3. Input Actions sẽ tự động được map:
   - **Space key** → Toggle Ragdoll (Jump action)
   - **Left Click/Enter** → Apply Force (Attack action)

### Phương pháp 2: Manual Setup
1. Trong Inspector của SimpleRagdollDemo:
2. Assign **Toggle Ragdoll Action** → Player/Jump action reference
3. Assign **Apply Force Action** → Player/Attack action reference

### Phương pháp 3: ScriptableObject Setup
1. Tạo SimpleRagdollInputSetup asset: Right click → Create → AnimalRevolt → Ragdoll → Input Setup
2. Assign Jump và Attack action references
3. Gọi `SetupInputActions()` hoặc `AutoSetupAllInScene()`

## Input Mapping hiện tại

| Action | Key | Chức năng |
|--------|-----|-----------|
| Jump | Space | Toggle Ragdoll on/off |
| Attack | Left Click, Enter | Apply test force |

## Test Controls

### Runtime Controls:
- **Space**: Toggle ragdoll
- **Left Click/Enter**: Apply force
- **Right Click GameObject**: Context menu với các options

### Editor Controls (khi đang chạy):
- Button "Toggle Ragdoll" trong Inspector
- Button "Apply Force" trong Inspector  
- Button "Apply Random Force" trong Inspector
- Button "Reset Ragdoll" trong Inspector

## Troubleshooting

### Nếu Input không hoạt động:
1. Kiểm tra Input Action References đã được assign chưa
2. Kiểm tra Input Actions có enabled không
3. Sử dụng "Auto Setup Input Actions" button
4. Kiểm tra Console có lỗi gì không

### Nếu cần custom key bindings:
1. Mở InputSystem_Actions.inputactions
2. Sửa key bindings cho Jump và Attack actions
3. Input sẽ tự động cập nhật

## Lưu ý quan trọng

- Script này chỉ hoạt động với Unity Input System package
- Không được sử dụng UnityEngine.Input class cũ
- Input Actions phải được enabled trong OnEnable()
- Input Actions phải được disabled trong OnDisable() để tránh memory leaks

## File structure

```
Assets/AnimalRevolt/Scripts/Ragdoll/
├── SimpleRagdollDemo.cs (đã cập nhật)
├── SimpleRagdollInputSetup.cs (mới)
├── Editor/
│   └── SimpleRagdollDemoEditor.cs (mới)
└── INPUT_SYSTEM_SETUP_GUIDE.md (file này)
```

## Kết luận

Lỗi Input System đã được sửa hoàn toàn. SimpleRagdollDemo giờ đây:
- ✅ Tương thích với Unity Input System mới
- ✅ Có Editor tools thân thiện  
- ✅ Hỗ trợ auto setup
- ✅ Dễ dàng customize và mở rộng
- ✅ Không có memory leaks