# 🎛️ HƯỚNG DẪN KHÔI PHỤC HỆ THỐNG UI TOGGLE

## 📋 Tóm Tắt
Hệ thống UI Toggle đã được khôi phục thành công trong scene. Chức năng này cho phép ẩn/hiện toàn bộ UI bằng một nút icon ở góc màn hình.

## ✅ Đã Hoàn Thành
- ✅ Thêm prefab `UIToggleSystem` vào scene
- ✅ Script `UIToggleManager` đã sẵn sàng
- ✅ Script `UIToggleDemo` đã cấu hình
- ✅ Prefab có đầy đủ Canvas và Button components

## 🎮 Cách Sử Dụng

### 1. Điều Khiển Cơ Bản
- **Phím tắt**: Nhấn `F1` để toggle UI
- **Nút GUI**: Click vào icon camera ở góc trên-phải màn hình
- **Trạng thái**: 
  - 🟢 Nút màu xanh = UI đang BẬT
  - ⚫ Nút màu xám = UI đang TẮT

### 2. Tính Năng
- Toggle tất cả Camera Settings UI
- Toggle tất cả UI Panel được quản lý
- Tự động tìm và quản lý các CameraSettingsUI component
- Hỗ trợ phím tắt có thể tùy chỉnh
- Vị trí nút có thể điều chỉnh

## 🔧 Cấu Hình Trong Inspector

### UIToggleManager Component
```
🎛️ UI Toggle Settings:
- Toggle UI Action: F1 (có thể thay đổi)
- Vị Trí Nút Toggle: Top (góc trên-phải)
- Kích Thước Nút: 60x60 pixels
- Offset Từ Góc: 20x20 pixels

📱 UI References:
- UI Canvas: Canvas cho nút toggle
- UI Panel: Panel chứa UI cần toggle
- Camera Settings UI: Reference đến CameraSettingsUI
- Icon Camera: Sprite icon cho nút

🎨 UI Styling:
- Màu Nút Bật: Xanh lá (0.2, 0.8, 0.2, 0.8)
- Màu Nút Tắt: Xám (0.5, 0.5, 0.5, 0.6)
```

### UIToggleDemo Component
```
🎮 Demo Settings:
- Auto Setup: ✅ Tự động thiết lập
- Tạo Icon Camera: ✅ Tạo icon mặc định
- Vị Trí Nút: Top (có thể thay đổi)
```

## 🛠️ Context Menu Actions

### UIToggleManager
- **🎛️ Toggle UI**: Test toggle UI ngay lập tức
- **📱 Refresh UI Components**: Tìm lại các UI component
- **🔧 Setup Default Position**: Đặt lại vị trí mặc định

### UIToggleDemo
- **🎛️ Setup UI Toggle System**: Thiết lập lại hệ thống
- **🧪 Test Toggle UI**: Test chức năng toggle
- **📍 Change Button Position**: Thay đổi vị trí nút
- **📊 Show System Status**: Hiển thị trạng thái hệ thống

## 📍 Vị Trí Nút Toggle

Có thể chọn từ 4 vị trí:
- **Top**: Góc trên-phải (mặc định)
- **Right**: Góc trên-phải
- **Bottom**: Góc dưới-phải  
- **Left**: Góc trên-trái

## 🎯 UI Được Quản Lý

Hệ thống tự động quản lý:
- Tất cả `CameraSettingsUI` component trong scene
- UI Panel được gán vào `uiPanel` field
- Các UI khác có thể thêm qua code

## 🐛 Troubleshooting

### Nút Không Hiển Thị
1. Kiểm tra GameObject `UIToggleSystem` có active trong scene
2. Kiểm tra Canvas có render mode = Screen Space Overlay
3. Kiểm tra sorting order của Canvas (mặc định: 1000)

### UI Không Toggle
1. Chạy `📱 Refresh UI Components` trong context menu
2. Kiểm tra có CameraSettingsUI component trong scene
3. Kiểm tra Console log để debug

### Phím Tắt Không Hoạt Động
1. Kiểm tra Input System đã được thiết lập
2. Kiểm tra toggleUIAction có enabled
3. Thử đổi phím tắt khác qua Inspector

## 🔄 Cách Thêm UI Mới Vào Hệ Thống

### Qua Code
```csharp
// Tìm UIToggleManager
UIToggleManager toggleManager = FindFirstObjectByType<UIToggleManager>();

// Thêm UI component
toggleManager.ThemUIComponent(yourUIGameObject);
```

### Qua Inspector
1. Kéo UI GameObject vào field `UI Panel` của UIToggleManager
2. Hoặc kế thừa interface tương tự CameraSettingsUI

## 📝 API Chính

```csharp
// Toggle tất cả UI
toggleManager.ToggleAllUI();

// Bật UI
toggleManager.BatUI();

// Tắt UI  
toggleManager.TatUI();

// Kiểm tra trạng thái
bool isShowing = toggleManager.DangHienThiUI();

// Thay đổi phím tắt
toggleManager.DatPhimTat("<Keyboard>/f2");

// Thay đổi vị trí nút
toggleManager.DatViTriNut(RectTransform.Edge.Bottom, new Vector2(20, 20));
```

## 🎉 Kết Luận

Hệ thống UI Toggle đã được khôi phục hoàn toàn và sẵn sàng sử dụng. Nút icon camera sẽ xuất hiện ở góc trên-phải màn hình, cho phép bạn ẩn/hiện tất cả UI một cách dễ dàng.

### Kiểm Tra Ngay
1. ▶️ Chạy game
2. 👀 Tìm nút camera ở góc trên-phải
3. 🖱️ Click hoặc nhấn F1 để test
4. 🎯 Xem UI Camera Settings ẩn/hiện

**Happy Toggling! 🎛️✨**