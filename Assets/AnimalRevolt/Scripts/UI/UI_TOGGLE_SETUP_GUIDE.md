# 🎛️ Hướng Dẫn Thiết Lập UI Toggle System

Hệ thống UI Toggle cho phép bạn bật/tắt tất cả UI camera thông qua một nút ở góc màn hình và phím tắt.

## 📋 Tính Năng Chính

### ✨ UIToggleManager
- **Nút toggle ở góc màn hình**: Có thể đặt ở 4 góc (trên, dưới, trái, phải)
- **Phím tắt F1**: Bật/tắt UI nhanh chóng
- **Quản lý tập trung**: Điều khiển tất cả UI camera từ một nơi
- **Visual feedback**: Màu nút thay đổi theo trạng thái (xanh = bật, xám = tắt)

### 🎮 Tích Hợp Camera UI
- **CameraSettingsUI**: Điều chỉnh thông số camera runtime
- **Hỗ trợ multiple cameras**: Main camera, NPC cameras
- **Camera mode switching**: FreeCam, Follow, Overview, Orbital
- **Save/Load settings**: Lưu cài đặt vào PlayerPrefs

## 🚀 Cách Thiết Lập

### Bước 1: Thêm UIToggleManager vào Scene

```csharp
// Cách 1: Tự động (khuyến nghị)
// Thêm UIToggleDemo component vào bất kỳ GameObject nào
// Nó sẽ tự động tạo và cấu hình UIToggleManager

// Cách 2: Thủ công
GameObject managerObj = new GameObject("UIToggleManager");
UIToggleManager manager = managerObj.AddComponent<UIToggleManager>();
```

### Bước 2: Cấu hình CameraSettingsUI

```csharp
// CameraSettingsUI sẽ tự động tìm và tích hợp với UIToggleManager
// Không cần cấu hình thêm, chỉ cần đảm bảo có component này trong scene
```

### Bước 3: Tùy Chỉnh Vị Trí và Phím Tắt

```csharp
UIToggleManager manager = FindFirstObjectByType<UIToggleManager>();

// Thay đổi vị trí nút (góc trên-phải là mặc định)
manager.DatViTriNut(RectTransform.Edge.Top, new Vector2(20, 20));

// Thay đổi phím tắt (F1 là mặc định)
manager.DatPhimTat("<Keyboard>/f2"); // Chuyển sang F2
```

## 🎯 Cách Sử Dụng

### Trong Game

1. **Nhìn góc màn hình** → Tìm icon camera
2. **Click nút camera** → Toggle UI on/off
3. **Hoặc nhấn F1** → Toggle UI bằng phím tắt
4. **Màu nút**:
   - 🟢 **Xanh lá**: UI đang bật
   - ⚫ **Xám**: UI đang tắt

### Trong Inspector

**UIToggleDemo Component:**
- `Auto Setup`: Tự động thiết lập khi start
- `Tạo Icon Camera`: Tạo icon camera mặc định
- `Vị Trí Nút`: Chọn góc màn hình

**Context Menu Actions:**
- `🎛️ Setup UI Toggle System`: Thiết lập hệ thống
- `🧪 Test Toggle UI`: Test toggle UI
- `📍 Change Button Position`: Thay đổi vị trí nút
- `📊 Show System Status`: Hiển thị trạng thái hệ thống

## ⚡ Quick Start

### Thiết Lập Nhanh (1 phút)

1. **Tạo GameObject mới** trong scene
2. **Add Component** → `UIToggleDemo`
3. **Play scene** → Hệ thống sẽ tự động thiết lập
4. **Tìm nút camera** ở góc trên-phải màn hình
5. **Click hoặc nhấn F1** để test

### Thiết Lập Thủ Công

```csharp
// 1. Tạo UIToggleManager
GameObject managerObj = new GameObject("UIToggleManager");
UIToggleManager manager = managerObj.AddComponent<UIToggleManager>();

// 2. Tùy chỉnh (optional)
manager.DatViTriNut(RectTransform.Edge.Bottom, new Vector2(30, 30));
manager.DatPhimTat("<Keyboard>/grave"); // Phím ~ (tilde)

// 3. Hệ thống sẽ tự động tìm và quản lý CameraSettingsUI
```

## 🔧 Tùy Chỉnh Nâng Cao

### Thêm UI Component Khác

```csharp
UIToggleManager manager = FindFirstObjectByType<UIToggleManager>();

// Thêm UI component khác để được quản lý
GameObject customUI = /* your UI component */;
manager.ThemUIComponent(customUI);
```

### Tạo Icon Camera Tùy Chỉnh

```csharp
// Trong UIToggleManager Inspector:
// - Icon Camera: Kéo sprite của bạn vào đây
// - Màu Nút Bật: Tùy chỉnh màu khi UI bật
// - Màu Nút Tắt: Tùy chỉnh màu khi UI tắt
```

### Event Handling

```csharp
UIToggleManager manager = FindFirstObjectByType<UIToggleManager>();

// Listen to UI state changes
manager.OnUIToggled += (bool isOn) => {
    Debug.Log($"UI toggled: {isOn}");
    // Xử lý logic tùy chỉnh
};
```

## 🐛 Troubleshooting

### Lỗi Thường Gặp

**❌ Nút không hiển thị**
- Kiểm tra Canvas được tạo đúng cách
- Đảm bảo sortingOrder > 0
- Kiểm tra icon sprite được gán

**❌ Phím tắt không hoạt động**
- Kiểm tra Input Action được Enable
- Đảm bảo không có conflict với input khác
- Verify binding string đúng format

**❌ CameraSettingsUI không toggle**
- Đảm bảo CameraSettingsUI có trong scene
- Kiểm tra reference trong UIToggleManager
- Gọi TimCacUIComponent() để refresh

### Debug Commands

```csharp
// Kiểm tra trạng thái hệ thống
UIToggleDemo demo = FindFirstObjectByType<UIToggleDemo>();
demo.HienThiTrangThaiHeThong();

// Test toggle
demo.TestToggleUI();

// Thay đổi vị trí để test
demo.ThayDoiViTriNut();
```

## 📝 Best Practices

### Performance
- UI Toggle chỉ tốn ít tài nguyên
- Sử dụng OnGUI cho Camera UI (đã optimize)
- Toggle thay vì destroy/create UI

### UX Design
- Đặt nút ở vị trí dễ nhìn nhưng không cản trở gameplay
- Sử dụng màu sắc rõ ràng để phân biệt trạng thái
- Giữ phím tắt đơn giản và dễ nhớ

### Code Organization
- Một UIToggleManager cho toàn bộ scene
- Tách riêng logic toggle và UI rendering
- Sử dụng events cho loose coupling

## 🔗 Liên Kết

**Related Scripts:**
- [`UIToggleManager.cs`](UIToggleManager.cs) - Core toggle system
- [`CameraSettingsUI.cs`](CameraSettingsUI.cs) - Camera UI component
- [`UIToggleDemo.cs`](UIToggleDemo.cs) - Demo and setup helper

**Dependencies:**
- Unity Input System Package
- UnityEngine.UI
- AnimalRevolt.Camera namespace (cho camera components)

---

**💡 Tip**: Bắt đầu với UIToggleDemo để thiết lập nhanh, sau đó tùy chỉnh theo nhu cầu của bạn!