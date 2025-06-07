# Quick Test Guide - Ragdoll System

## Bước 1: Setup Character
1. **Chọn character** trong scene (ví dụ: SK_Military_Survivalist)
2. **Add Component** → Search "RagdollControllerUI" → Add
3. **Add Component** → Search "SimpleRagdollDemo" → Add (optional)

## Bước 2: Test Ngay

### Cách 1: Inspector UI (Khuyến nghị)
1. **Chạy game** (nhấn Play)
2. **Chọn character** trong Hierarchy
3. **Trong Inspector** sẽ thấy:
   - ✅ **"Enable Ragdoll"** (nút xanh lá)
   - ✅ **"Apply Random Force"** (nút vàng)
   - Status hiển thị real-time

### Cách 2: Context Menu  
1. **Right-click** character trong Hierarchy
2. Chọn **"Toggle Ragdoll"** hoặc **"Apply Random Force"**

### Cách 3: Keyboard (nếu có SimpleRagdollDemo)
1. **Space** - Toggle Ragdoll
2. **F** - Apply Random Force

## Troubleshooting

### Nếu không thấy UI buttons:
- Đảm bảo file `RagdollControllerUIEditor.cs` trong thư mục `Editor`
- Restart Unity nếu cần

### Nếu ragdoll không hoạt động:
- Character cần có **Rigidbody** và **Collider** trên các bones
- Sử dụng **Ragdoll Wizard** (Window → 3D Object → Ragdoll...) để setup

### Nếu force không được áp dụng:
- Phải **enable ragdoll** trước khi apply force
- Kiểm tra character có đang kinematic không

## Expected Result
- Character sẽ chuyển từ animation sang physics simulation
- Có thể áp dụng lực để làm character bay/ngã
- UI buttons sẽ đổi màu theo trạng thái

## Note
- ✅ Hoạt động trong **Play mode** 
- ❌ Context menu có thể dùng ngoài Play mode nhưng không có hiệu ứng
- 🎯 UI buttons chỉ hoạt động khi **đang chạy game**