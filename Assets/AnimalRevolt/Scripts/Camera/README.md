# 🎥 Animal Revolt Camera System

Hệ thống camera hoàn chỉnh cho game Animal Revolt với nhiều chế độ camera và tính năng nâng cao.

## 📁 Cấu trúc Files

```
Assets/AnimalRevolt/Scripts/Camera/
├── CameraController.cs      # Camera chính với 4 modes
├── CameraManager.cs         # Quản lý chuyển đổi cameras
├── NPCCamera.cs            # Camera quan sát NPC
└── README.md               # Tài liệu này

Assets/AnimalRevolt/Scripts/UI/
└── CameraSettingsUI.cs     # UI điều chỉnh camera
```

## 🎮 Camera Modes

### 1. **FreeCam Mode** (Mặc định)
- **Mô tả**: Camera tự do di chuyển trong không gian 3D
- **Điều khiển**:
  - `WASD`: Di chuyển ngang
  - `Q/E`: Lên/xuống
  - `Right-click + drag`: Xoay camera
  - `Shift`: Tăng tốc độ di chuyển/xoay
- **Sử dụng**: Khám phá scene, debug, chế độ phát triển

### 2. **Follow Mode**
- **Mô tả**: Camera theo dõi mục tiêu tự động
- **Tính năng**:
  - Tự động tìm Player hoặc NPC gần nhất
  - Smooth lerp movement
  - Scroll wheel để điều chỉnh khoảng cách
- **Điều khiển**:
  - `F`: Tìm mục tiêu mới
  - `Scroll wheel`: Zoom in/out
- **Sử dụng**: Theo dõi character, gameplay camera

### 3. **Overview Mode**
- **Mô tả**: Camera nhìn tổng quan tất cả đối tượng
- **Tính năng**:
  - Tự động tính center của tất cả Players/NPCs
  - Camera đặt ở độ cao và khoảng cách phù hợp
  - Dynamic adjustment khi objects di chuyển
- **Điều khiển**:
  - `Scroll wheel`: Điều chỉnh độ cao
- **Sử dụng**: Combat overview, strategic view

### 4. **Orbital Mode**
- **Mô tả**: Camera xoay quanh mục tiêu như vệ tinh
- **Tính năng**:
  - Orbital rotation quanh target
  - Focus lock khi bắt đầu orbital
  - Smooth orbital movement
- **Điều khiển**:
  - `Right-click + drag`: Orbital rotation
  - `Scroll wheel`: Điều chỉnh khoảng cách orbital
  - `Shift`: Boost tốc độ xoay
- **Sử dụng**: Cinematic camera, character inspection

## 🎛️ Camera Controls

### Phím tắt toàn cục:
- **`C`**: Chuyển đổi camera modes (FreeCam → Follow → Overview → Orbital)
- **`Home`**: Reset camera về vị trí ban đầu
- **`F`**: Auto-find follow target (trong Follow/Orbital mode)
- **`F1`**: Toggle Camera Settings UI

### Camera Switching:
- **`0`**: Chuyển về Main Camera
- **`1-9`**: Chuyển sang NPC Camera tương ứng
- **`Tab`**: Chuyển sang camera kế tiếp

### Mouse Controls:
- **Right-click + drag**: Xoay camera (tất cả modes)
- **Scroll wheel**: Zoom/điều chỉnh khoảng cách
- **Left-click**: Interaction (nếu có)

## 🏗️ Kiến trúc System

### CameraController.cs
```csharp
namespace AnimalRevolt.Camera
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraMode { FreeCam, Follow, Overview, Orbital }
        
        // Public API
        public CameraMode LayCameraMode()
        public void DatCameraMode(CameraMode mode)
        public void ResetCamera()
        public void DatMucTieuFollow(Transform target)
    }
}
```

### CameraManager.cs
```csharp
namespace AnimalRevolt.Camera
{
    public class CameraManager : MonoBehaviour
    {
        // Public API
        public void BatCameraChinh()
        public void ChuyenSangNPCCamera(int index)
        public void ChuyenCameraKeTiep()
        public Camera LayCameraHienTai()
    }
}
```

### NPCCamera.cs
```csharp
namespace AnimalRevolt.Camera
{
    public class NPCCamera : MonoBehaviour
    {
        // Public API
        public void BatCamera()
        public void TatCamera()
        public void DatMucTieuNPC(Transform target)
        public void ResetCamera()
    }
}
```

## ⚙️ Setup Instructions

### 1. Tạo Main Camera
```csharp
// Attach CameraController.cs vào Main Camera GameObject
// Đặt các thông số cơ bản trong Inspector
```

### 2. Tạo Camera Manager
```csharp
// Tạo empty GameObject "CameraManager"
// Attach CameraManager.cs
// Gán Main Camera reference
```

### 3. Tạo NPC Cameras
```csharp
// Tạo Camera GameObjects cho từng NPC
// Attach NPCCamera.cs
// Đặt ở vị trí quan sát NPC
// Gán NPC target trong Inspector
```

### 4. Tạo Camera Settings UI
```csharp
// Tạo empty GameObject "CameraSettingsUI"
// Attach CameraSettingsUI.cs
// Gán camera references
```

## 🎨 Customization

### Điều chỉnh thông số camera:
1. **Runtime**: Sử dụng Camera Settings UI (F1)
2. **Inspector**: Điều chỉnh serialized fields
3. **Code**: Sử dụng public API methods

### Thêm camera mode mới:
```csharp
// 1. Thêm vào CameraMode enum
public enum CameraMode { FreeCam, Follow, Overview, Orbital, NewMode }

// 2. Thêm case trong Update()
case CameraMode.NewMode:
    XuLyNewMode();
    break;

// 3. Implement method xử lý
private void XuLyNewMode()
{
    // Custom camera behavior
}
```

### Shared Parameters:
- Tất cả NPC cameras sử dụng chung parameters từ CameraSettingsUI
- Có thể override bằng cách tắt `suDungSharedParameters`

## 🐛 Troubleshooting

### Camera không hoạt động:
1. Kiểm tra Input System package đã cài đặt
2. Verify camera references trong Manager
3. Check AudioListener conflicts

### Performance issues:
1. Giảm số lượng NPC cameras active
2. Tối ưu update frequency
3. Sử dụng object pooling cho cameras

### Audio conflicts:
1. CameraManager tự động quản lý AudioListener
2. Chỉ một AudioListener active tại một thời điểm
3. Restore state khi destroy

## 📊 Features Summary

| Feature | Main Camera | NPC Camera | Camera Manager |
|---------|-------------|------------|----------------|
| 4 Camera Modes | ✅ | ❌ | ❌ |
| Mouse Look | ✅ | ✅ | ❌ |
| WASD Movement | ✅ | ❌ | ❌ |
| Follow Target | ✅ | ✅ | ❌ |
| Zoom Control | ✅ | ✅ | ❌ |
| Auto Switch | ❌ | ❌ | ✅ |
| Audio Management | ❌ | ❌ | ✅ |
| Settings UI | ✅ | ✅ | ✅ |

## 🔄 Migration từ old system

### Từ Assets/Scripts/ sang Assets/AnimalRevolt/Scripts/Camera/:
1. **CameraController.cs**: Namespace thay đổi, thêm camera modes
2. **QuanLyCamera.cs** → **CameraManager.cs**: Improved management
3. **NPCCamera.cs**: Enhanced features, shared parameters
4. **DieuChinhThongSoCamera.cs** → **CameraSettingsUI.cs**: Better UI, namespaced

### Breaking Changes:
- Namespace changes: `AnimalRevolt.Camera`, `AnimalRevolt.UI`
- Method naming: Vietnamese naming convention
- Component references cần update

### Backward Compatibility:
- Old scripts vẫn hoạt động trong Assets/Scripts/
- Có thể migrate từng phần hoặc toàn bộ
- Settings được preserve qua PlayerPrefs

## 📝 Best Practices

1. **Sử dụng namespaces** để tránh conflicts
2. **Gán references trong Inspector** thay vì FindObjectOfType
3. **Sử dụng shared parameters** cho consistency
4. **Test trên nhiều platforms** (PC, Mobile)
5. **Backup settings** với PlayerPrefs
6. **Monitor performance** với nhiều cameras
7. **Use Camera Manager** cho switching logic
8. **Implement error handling** cho null references

---

**Created by**: Animal Revolt Development Team  
**Version**: 1.0  
**Last Updated**: 2025  
**Unity Version**: 2022.3+