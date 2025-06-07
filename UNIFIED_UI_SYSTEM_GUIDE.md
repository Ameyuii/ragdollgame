# 🎛️ HƯỚNG DẪN HỆ THỐNG UI TỔNG HỢP (UNIFIED UI SYSTEM)

## 📋 Tổng Quan
Hệ thống UI tổng hợp mới cho phép quản lý toàn bộ UI trong game, bao gồm:
- ✅ Bật/tắt toàn bộ UI đồng bộ
- ✅ Bật/tắt UI theo camera hiện tại
- ✅ Quản lý UI theo category (Camera, Ragdoll, Combat, Health, Character, Game)
- ✅ Tự động tìm và đăng ký UI components
- ✅ Migration từ UIToggleManager cũ
- ✅ Theo dõi thay đổi camera tự động

## 🎯 Các Tính Năng Chính

### 1. Quản lý UI tổng hợp
- **UnifiedUIManager**: Script chính quản lý toàn bộ UI
- **UIToggleManager**: Legacy script với chức năng migration
- **Tự động phát hiện**: Tìm tất cả UI components trong scene
- **Phân loại thông minh**: Chia UI theo category và camera

### 2. Control theo Camera
- **F1**: Toggle toàn bộ UI
- **F2**: Toggle UI camera hiện tại
- **Tự động**: Theo dõi thay đổi camera và cập nhật UI tương ứng
- **Đồng bộ**: UI được đồng bộ theo camera đang active

### 3. Phân loại UI
- 📹 **Camera UI**: CameraSettingsUI, NPCCamera controls
- 🎭 **Ragdoll UI**: RagdollControllerUI và debug UI
- ⚔️ **Combat UI**: Combat systems UI
- ❤️ **Health UI**: HealthBar, DamageNumberSpawner
- 👤 **Character UI**: CharacterSelectionUI
- 🎮 **Game UI**: TestCameraUI và các UI khác

## 🚀 Cách Sử Dụng

### Bước 1: Thiết Lập UnifiedUIManager

1. **Tạo UnifiedUIManager mới:**
   ```
   GameObject → Create Empty → Tên: "UnifiedUIManager"
   Add Component → UnifiedUIManager
   ```

2. **Hoặc Migration từ UIToggleManager cũ:**
   - Chọn GameObject có UIToggleManager
   - Trong Inspector → Context Menu → "Migrate to UnifiedUIManager"
   - Hệ thống sẽ tự động chuyển đổi

### Bước 2: Cấu Hình trong Inspector

```
🎛️ UI System Settings:
- Toggle All UI Action: F1 (có thể thay đổi)
- Toggle Camera UI Action: F2 (có thể thay đổi)
- Auto Discover UI: ✅ (khuyến khích)
- Show UI On Start: ❌ (tùy chọn)

📱 UI Toggle Button:
- Button Position: Top (góc trên-phải)
- Button Size: 60x60
- Button Offset: 20x20

🎨 UI Styling:
- Toggle Icon: Sprite icon cho nút
- Active Color: Màu xanh khi UI bật
- Inactive Color: Màu xám khi UI tắt

📹 Camera Integration:
- Camera Manager: Tự động tìm hoặc kéo thả
- Auto Track Camera Changes: ✅ (khuyến khích)
```

### Bước 3: Chạy và Test

1. **▶️ Play** game
2. **🔍 Kiểm tra Console** cho log khởi tạo:
   ```
   🔍 Bắt đầu tìm kiếm tất cả UI components...
   📂 CameraUI: X components
   📂 HealthUI: Y components
   🌍 Global UI: Z components
   🎛️ UnifiedUIManager đã khởi tạo thành công
   ```
3. **🎮 Test controls:**
   - **F1**: Toggle toàn bộ UI
   - **F2**: Toggle UI camera hiện tại
   - **Click nút**: Toggle toàn bộ UI

## 🎮 Controls và Phím Tắt

### Controls Mặc Định
- **F1**: Bật/tắt toàn bộ UI
- **F2**: Bật/tắt UI camera hiện tại
- **Click nút góc màn hình**: Bật/tắt toàn bộ UI

### Tùy Chỉnh Phím Tắt
```csharp
// Trong code
UnifiedUIManager uiManager = FindFirstObjectByType<UnifiedUIManager>();

// Hoặc qua Inspector - thay đổi Input Action bindings
```

### Context Menu Actions
- **🎛️ Toggle All UI**: Test toggle toàn bộ UI
- **📹 Toggle Current Camera UI**: Test toggle UI camera hiện tại
- **🔄 Refresh All UI**: Tìm lại tất cả UI components

## 🎯 API Chính

### Quản lý UI tổng hợp
```csharp
UnifiedUIManager uiManager = FindFirstObjectByType<UnifiedUIManager>();

// Toggle toàn bộ UI
uiManager.ToggleAllUI();

// Bật/tắt toàn bộ UI
uiManager.ShowAllUI();
uiManager.HideAllUI();

// Kiểm tra trạng thái
bool isVisible = uiManager.IsAllUIVisible();
```

### Quản lý UI theo Category
```csharp
// Bật/tắt UI theo category
uiManager.ShowCategoryUI("CameraUI");
uiManager.HideCategoryUI("RagdollUI");

// Bật/tắt UI theo camera
uiManager.ShowCameraUI(0); // Camera chính
uiManager.HideCameraUI(1); // NPC Camera
```

### Đăng ký UI Component mới
```csharp
// Đăng ký UI component vào hệ thống
uiManager.RegisterUIComponent(yourUIComponent, "YourCategory", cameraIndex);
```

## 🔧 Cấu Hình Nâng Cao

### Tự động tìm UI Components
Hệ thống tự động tìm các loại UI sau:
- **CameraSettingsUI**: UI điều chỉnh camera
- **HealthBar**: Thanh máu nhân vật
- **DamageNumberSpawner**: Spawn số damage
- **CharacterSelectionUI**: UI chọn nhân vật
- **RagdollControllerUI**: Ragdoll debug UI
- **TestCameraUI**: UI test camera

### Thêm UI Category mới
```csharp
// Trong UnifiedUIManager, thêm category mới:
private const string YOUR_CATEGORY = "YourCategoryUI";

// Trong InitializeUISystem(), thêm:
uiComponentsByCategory[YOUR_CATEGORY] = new List<MonoBehaviour>();

// Trong DiscoverAllUIComponents(), thêm:
RegisterYourCategoryUIComponents();
```

### Custom UI Component Registration
```csharp
// Cho UI component đặc biệt
public void RegisterCustomUI(MonoBehaviour component, string category, int cameraIndex = -1)
{
    RegisterUIComponent(component, category, cameraIndex);
    
    // Refresh để cập nhật hệ thống
    RefreshAllUI();
}
```

## 📱 UI theo Camera

### Cách hoạt động
1. **Theo dõi camera**: Hệ thống tự động theo dõi camera đang active
2. **UI mapping**: Mỗi camera có thể có UI riêng
3. **Tự động chuyển**: Khi đổi camera, UI cũ ẩn, UI mới hiện

### Đăng ký UI cho Camera cụ thể
```csharp
// Camera 0 (Main Camera)
uiManager.RegisterUIComponent(mainCameraUI, "CameraUI", 0);

// Camera 1 (NPC Camera)
uiManager.RegisterUIComponent(npcCameraUI, "CameraUI", 1);

// Global UI (không theo camera)
uiManager.RegisterUIComponent(globalUI, "GameUI", -1);
```

## 🔄 Migration từ UIToggleManager cũ

### Tự động Migration
1. **Enable Auto Migrate**: Trong UIToggleManager → `Auto Migrate To Unified = true`
2. **Restart scene**: Hệ thống sẽ tự động tạo UnifiedUIManager
3. **Legacy UI**: UIToggleManager cũ sẽ delegate calls sang UnifiedUIManager

### Manual Migration
1. **Context Menu**: UIToggleManager → "Migrate to UnifiedUIManager"
2. **Check Status**: "Show Migration Status" để kiểm tra
3. **Remove Legacy**: Xóa UIToggleManager component cũ nếu muốn

### Backward Compatibility
- UIToggleManager cũ vẫn hoạt động
- Calls được delegate sang UnifiedUIManager nếu có
- Không cần thay đổi code hiện tại

## 🐛 Troubleshooting

### UI không hiển thị
1. **Check Console**: Xem log khởi tạo UnifiedUIManager
2. **Refresh UI**: Context Menu → "Refresh All UI"
3. **Auto Discover**: Đảm bảo `Auto Discover UI = true`

### Phím tắt không hoạt động
1. **Check Input System**: Đảm bảo Input System được thiết lập
2. **Input Actions**: Kiểm tra `Toggle All UI Action` và `Toggle Camera UI Action`
3. **Enable state**: Các Input Action phải được enable

### UI Component không được tìm thấy
1. **Manual Register**: Sử dụng `RegisterUIComponent()` để đăng ký thủ công
2. **Correct Type**: Đảm bảo component có đúng type mong đợi
3. **Scene Active**: Component phải ở trong scene đang active

### Camera UI không chuyển đổi
1. **Camera Manager**: Đảm bảo có CameraManager trong scene
2. **Auto Track**: `Auto Track Camera Changes = true`
3. **Camera Index**: Kiểm tra index camera có đúng không

## 📊 Monitoring và Debug

### Console Logs
```
🔍 Bắt đầu tìm kiếm tất cả UI components...
📂 CameraUI: 2 components
📂 HealthUI: 3 components  
🌍 Global UI: 1 components
📹 Camera UI: 2 camera groups
🎛️ UnifiedUIManager đã khởi tạo thành công
```

### Runtime Debug
```csharp
// Check trạng thái UI
Debug.Log($"All UI Visible: {uiManager.IsAllUIVisible()}");
Debug.Log($"Camera UI Visible: {uiManager.IsCurrentCameraUIVisible()}");

// Refresh UI components
uiManager.RefreshAllUI();
```

## 🎉 Best Practices

### 1. Setup
- ✅ Sử dụng Auto Discover UI
- ✅ Enable Auto Track Camera Changes
- ✅ Đặt Button ở vị trí dễ nhấn
- ✅ Customize phím tắt phù hợp với game

### 2. Development
- ✅ Đăng ký UI mới qua `RegisterUIComponent()`
- ✅ Sử dụng categories phù hợp
- ✅ Test với nhiều camera khác nhau
- ✅ Check console logs để debug

### 3. Performance
- ✅ Refresh UI chỉ khi cần thiết
- ✅ Avoid frequent manual registration
- ✅ Sử dụng camera index mapping hiệu quả

## 🔗 Liên Quan

- **CameraSettingsUI**: UI chính cho camera settings
- **CameraManager**: Quản lý switching camera
- **UIToggleManager**: Legacy UI toggle system
- **HealthBar**: Health UI component
- **CharacterSelectionUI**: Character selection system

---

**🎛️ Happy UI Managing! ✨**

Với UnifiedUIManager, bạn có thể quản lý toàn bộ UI một cách thống nhất, linh hoạt và mạnh mẽ hơn bao giờ hết!