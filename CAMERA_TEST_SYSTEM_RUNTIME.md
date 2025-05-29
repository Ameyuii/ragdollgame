# 🎮 Camera Test System - Runtime Debug Panel

## Tổng quan
**TestCameraSystem** là một công cụ debug runtime cho phép điều chỉnh thông số camera ngay trong build game. Hệ thống này giữ nguyên chức năng test camera ban đầu nhưng được cải tiến để hoạt động trong cả Editor và Build game.

## ✨ Tính năng chính

### 🎛️ Runtime Debug Panel
- **Icon toggle** ở góc màn hình để bật/tắt panel
- **Draggable panel** có thể kéo thả di chuyển
- **Real-time parameter adjustment** với sliders
- **Hoạt động trong build game** - không chỉ trong Editor

### 📊 Thông tin Camera Real-time
- Camera hiện tại đang active
- Vị trí và góc xoay camera
- Tốc độ xoay, boost multiplier, độ nhạy chuột

### 🕹️ Điều khiển Camera
- **Chuyển camera chính**: Quay về camera chính
- **Chuyển camera NPC**: Chuyển sang camera NPC tiếp theo
- **Đặt vị trí test**: Reset camera chính về vị trí test
- **Tạo NPC test**: Tạo NPC mẫu với camera để test

### ⚙️ Điều chỉnh thông số Runtime
#### 📷 Camera Chính:
- **Tốc độ xoay**: 50° - 300°/s
- **Nhân boost**: x1.0 - x5.0 (khi giữ Shift)
- **Độ nhạy chuột**: 0.5 - 10.0
- **Tốc độ di chuyển**: 1 - 50

#### 🎯 NPC Camera:
- **NPC Tốc độ xoay**: 50° - 300°/s
- **NPC Nhân boost**: x1.0 - x5.0 (khi giữ Shift)
- **NPC Độ nhạy chuột**: 0.5 - 10.0
- **NPC Khoảng cách**: 2 - 15 (zoom in/out)

## 🚀 Cách sử dụng

### Setup trong Scene
1. **Thêm TestCameraSystem vào scene**:
   ```
   GameObject → Create Empty → Đặt tên "CameraTestSystem"
   Add Component → TestCameraSystem
   ```

2. **Cấu hình trong Inspector**:
   - `Auto Show On Start`: Tự động hiện panel khi start
   - `Vi Tri Test Camera Chinh`: Vị trí test cho camera chính
   - `Icon Size`: Kích thước icon toggle (mặc định 60px)
   - `Icon Position`: Vị trí icon trên màn hình

### Sử dụng trong Game
1. **Mở Debug Panel**:
   - Click vào icon 🎮 ở góc màn hình
   - Hoặc gọi `testCameraSystem.TogglePanel()` từ code

2. **Di chuyển Panel**:
   - Kéo thanh title bar để di chuyển panel
   - Panel tự động giới hạn trong màn hình

3. **Điều chỉnh thông số**:
   - **Camera Chính**: Sử dụng sliders đầu để thay đổi camera chính
   - **NPC Camera**: Sử dụng sliders "NPC" để thay đổi camera NPC hiện tại
   - Thay đổi được áp dụng ngay lập tức cho camera đang active
   - Logs hiển thị giá trị mới trong Console

4. **Test Camera**:
   - Dùng buttons để chuyển đổi camera
   - Tạo NPC test để thử nghiệm

## 🔧 API cho Developer

### Public Methods
```csharp
public class TestCameraSystem : MonoBehaviour
{
    // Toggle panel hiển thị
    public void TogglePanel()
    
    // Hiện/ẩn panel
    public void ShowPanel(bool show)
    
    // Reset về giá trị mặc định
    public void ResetToDefaults()
}
```

### Sử dụng từ Code
```csharp
// Tìm TestCameraSystem trong scene
TestCameraSystem testSystem = FindFirstObjectByType<TestCameraSystem>();

// Mở panel debug
testSystem.ShowPanel(true);

// Reset tất cả thông số về mặc định
testSystem.ResetToDefaults();
```

## 🎯 Build Game Integration

### Debug trong Build
- **TestCameraSystem hoạt động đầy đủ trong build game**
- Không cần Unity Editor để sử dụng
- Ideal cho beta testing và tuning parameters

### Production Build
Nếu muốn loại bỏ trong production:
```csharp
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    // TestCameraSystem code here
#endif
```

## 📱 UI Layout

### Icon Toggle
- **Vị trí**: Góc trên trái màn hình (customizable)
- **Kích thước**: 60x60px (customizable)  
- **Icon**: 🎮 emoji
- **Click**: Toggle panel on/off

### Debug Panel
- **Kích thước**: 450x500px
- **Draggable**: Có thể kéo bằng title bar
- **Sections**:
  1. **Header**: Title + Close button
  2. **Camera Info**: Thông tin real-time
  3. **Controls**: Buttons điều khiển
  4. **Parameters**: Sliders điều chỉnh

## 🔍 Troubleshooting

### Common Issues

**Icon không hiển thị**:
- Kiểm tra TestCameraSystem đã được add vào scene
- Verify icon position không bị che khuất

**Panel không responsive**:
- Đảm bảo GUI calls trong OnGUI()
- Kiểm tra dragging states

**Parameters không áp dụng**:
- Verify CameraController references
- Kiểm tra logs trong Console

**Không tìm thấy QuanLyCamera**:
- Đảm bảo QuanLyCamera có trong scene
- Check initialization order trong Awake/Start

### Debug Tips
- **Console logs**: Tất cả actions đều có logs chi tiết
- **Real-time info**: Panel hiển thị thông tin camera current
- **Parameter validation**: Sliders có min/max bounds safe

## 📋 Dependencies

### Required Components
- `QuanLyCamera`: Quản lý camera system
- `CameraController`: Điều khiển camera chính
- `NPCCamera`: Camera cho NPC (optional)

### Required Packages
- **Input System**: Cho camera controls
- **Unity UI**: Cho GUI rendering (built-in)

## 🔄 Version History

### V1.0 (Current)
- ✅ Runtime debug panel với icon toggle
- ✅ Real-time parameter adjustment
- ✅ Build game compatibility
- ✅ Draggable UI panel
- ✅ Complete camera test functions

### Features đã implement:
- Camera switching (chính ↔ NPC)
- Position testing và NPC creation
- Real-time parameter sliders
- Full GUI với proper styling
- Error handling và logging

## 💡 Best Practices

### Performance
- Panel chỉ render khi `hienThiDebugPanel = true`
- GUI styles được cache để tránh recreation
- Minimal overhead khi panel đóng

### User Experience
- Icon nhỏ gọn, không che gameplay
- Panel có thể di chuyển tránh vướng UI game
- Clear feedback qua logs và visual indicators

### Development
- Sử dụng trong development builds để tuning
- Keep references đến core camera components
- Validate tất cả actions trước khi execute

---

**🎮 TestCameraSystem V1.0 - Runtime Camera Debug Panel**  
*Công cụ debug camera mạnh mẽ cho Unity game development*
