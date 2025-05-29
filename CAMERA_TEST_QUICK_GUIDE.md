# 🎮 TestCameraSystem - Setup và Sử dụng

## 🚀 Quick Setup

### 1. Thêm vào Scene
```
1. Tạo Empty GameObject → Đặt tên "CameraTestSystem"
2. Add Component → TestCameraSystem
3. Đảm bảo có QuanLyCamera trong scene
```

### 2. Cấu hình cơ bản
- ✅ `Auto Show On Start`: false (khuyến nghị)
- ✅ `Icon Position`: (10, 10) - góc trên trái
- ✅ `Icon Size`: 60 - vừa đủ để click

## 🎯 Sử dụng trong Game

### Mở Debug Panel
1. **Click icon 🎮** ở góc màn hình
2. Panel sẽ hiện với tất cả controls

### Điều chỉnh Camera
- **Sliders**: Kéo để thay đổi thông số real-time
- **Buttons**: Test chuyển đổi camera
- **Drag**: Kéo title bar để di chuyển panel

## ⚡ Thông số có thể điều chỉnh

| Thông số | Phạm vi | Mặc định | Tác dụng |
|----------|---------|----------|----------|
| Tốc độ xoay | 50° - 300°/s | 150°/s | Tốc độ xoay camera |
| Nhân boost | x1.0 - x5.0 | x2.5 | Tăng tốc khi giữ Shift |
| Độ nhạy chuột | 0.5 - 10.0 | 3.0 | Sensitivity chuột |
| Tốc độ di chuyển | 1 - 50 | 10 | Tốc độ WASD |

## 🔧 Hoạt động trong Build

### Development Build
- ✅ **Full functionality** - tất cả features hoạt động
- ✅ **Real-time tuning** - điều chỉnh parameters live
- ✅ **No Unity Editor required** - chạy standalone

### Production Build
Nếu cần tắt trong production:
```csharp
// Trong TestCameraSystem.cs
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    // Tất cả TestCameraSystem code
#endif
```

## 🎮 Controls

### Panel Controls
- **🔄 Camera chính**: Chuyển về camera chính
- **🎯 Camera NPC**: Chuyển camera NPC kế tiếp  
- **📍 Đặt vị trí test**: Reset camera về vị trí test
- **🔧 Tạo NPC test**: Tạo NPC mẫu để test

### Panel UI
- **❌ Close**: Đóng panel (hoặc click icon lại)
- **Drag title bar**: Di chuyển panel
- **Sliders**: Điều chỉnh thông số real-time

## ✅ Checklist Setup

- [ ] TestCameraSystem đã add vào scene
- [ ] QuanLyCamera có trong scene  
- [ ] CameraController attach vào camera chính
- [ ] Icon position không che UI game
- [ ] Test trong Play mode trước khi build

## 🔍 Debug

### Console Logs
Tất cả actions đều có logs:
```
🔄 Test: Chuyển về camera chính
🎯 Test: Chuyển camera NPC  
🖱️ Đã đặt độ nhạy chuột: 4.0
🚀 Đã đặt nhân boost: x3.0
```

### Common Issues
- **Icon không hiện**: Check TestCameraSystem active
- **Panel không responsive**: Verify trong OnGUI()
- **Không tìm thấy camera**: Check camera references

---

**💡 TestCameraSystem giúp debug và tune camera parameters ngay trong build game!**
