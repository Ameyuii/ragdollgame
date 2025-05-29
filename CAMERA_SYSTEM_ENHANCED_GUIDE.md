# HỆ THỐNG CAMERA MỚI - HƯỚNG DẪN THIẾT LẬP

## 🎯 Tính năng mới

### Camera Chính (Main Camera)
- ✅ **Di chuyển tự do 360 độ** trong không gian 3D
- ✅ **WASD**: Di chuyển ngang (trước/sau/trái/phải)
- ✅ **Q/E**: Di chuyển lên cao/xuống thấp
- ✅ **Giữ chuột phải + di chuyển**: Xoay camera tự do
- ✅ **Shift**: Tăng tốc độ di chuyển
- ✅ **Làm mềm chuyển động** để camera di chuyển mượt mà

### Camera NPC
- ✅ **Zoom xa/gần** bằng scroll chuột
- ✅ **Giữ chuột phải + di chuyển**: Xoay xung quanh nhân vật
- ✅ **Theo dõi NPC** tự động
- ✅ **Cấu hình linh hoạt** khoảng cách và góc nhìn
- ✅ **Chuyển động mềm mại** khi theo dõi

## 🔧 Thiết lập

### Bước 1: Kiểm tra Scripts
Đảm bảo các script sau đã được tạo trong thư mục `Assets/Scripts/`:
- ✅ `CameraController.cs` - Điều khiển camera chính
- ✅ `NPCCamera.cs` - Camera cho NPC (đã cập nhật)
- ✅ `QuanLyCamera.cs` - Quản lý tổng thể (đã cập nhật)
- ✅ `TestCameraSystem.cs` - Script test hệ thống

### Bước 2: Thiết lập Camera Chính
1. **Tự động**: `QuanLyCamera` sẽ tự động thêm `CameraController` vào Main Camera
2. **Thủ công** (nếu cần):
   - Chọn Main Camera trong Hierarchy
   - Add Component → `CameraController`
   - Cấu hình các thông số trong Inspector

### Bước 3: Thiết lập NPC Camera
1. **Chọn NPC GameObject** cần có camera
2. **Add Component** → `NPCCamera`
3. **Cấu hình trong Inspector**:
   ```
   🎛️ Cấu hình vị trí:
   - Khoảng cách ban đầu: 5
   - Khoảng cách tối thiểu: 2
   - Khoảng cách tối đa: 15
   - Độ cao camera: 2
   
   🎮 Cấu hình điều khiển:
   - Độ nhạy chuột: 2
   - Tốc độ zoom: 2
   - Tốc độ lerp camera: 5
   - Tốc độ lerp xoay: 10
   ```

### Bước 4: Thiết lập QuanLyCamera
1. **Tạo Empty GameObject** và đặt tên `CameraManager`
2. **Add Component** → `QuanLyCamera`
3. **Cấu hình**:
   - Camera chính: Gán Main Camera
   - Tự động tìm NPC Camera: ✅ (khuyến nghị)
   - Tự động quản lý AudioListener: ✅

### Bước 5: Test Hệ thống
1. **Add `TestCameraSystem`** vào một GameObject trong scene
2. **Chạy game** và kiểm tra GUI debug
3. **Test các chức năng**:
   - Phím `0`: Chuyển về camera chính
   - Phím `1`: Chuyển camera NPC tiếp theo
   - WASD + QE + Chuột: Điều khiển camera chính
   - Chuột + Scroll: Điều khiển camera NPC

## 🎮 Điều khiển

### Camera Chính
| Phím | Chức năng |
|------|-----------|
| `W` | Di chuyển tiến |
| `S` | Di chuyển lùi |
| `A` | Di chuyển trái |
| `D` | Di chuyển phải |
| `Q` | Lên cao |
| `E` | Xuống thấp |
| `Shift` | Tăng tốc |
| `Giữ chuột phải + di chuyển` | Xoay camera |

### Camera NPC
| Điều khiển | Chức năng |
|------------|-----------|
| `Giữ chuột phải + di chuyển` | Xoay xung quanh NPC |
| `Scroll lên` | Zoom vào gần |
| `Scroll xuống` | Zoom ra xa |

### Chuyển đổi Camera
| Phím | Chức năng |
|------|-----------|
| `0` | Camera chính |
| `1` | Camera NPC tiếp theo |

## 🛠️ Cấu hình nâng cao

### CameraController (Camera chính)
```csharp
[Header("Cấu hình di chuyển")]
tocDoChuyenDong = 10f        // Tốc độ di chuyển thường
tocDoChuyenDongNhanh = 20f   // Tốc độ khi giữ Shift
tocDoLenXuong = 5f           // Tốc độ Q/E

[Header("Cấu hình xoay")]
doNhayChuot = 2f             // Độ nhạy chuột
gioiHanGocXoay = 90f         // Giới hạn góc lên/xuống
doDaiLamMem = 0.1f           // Độ mềm chuyển động
```

### NPCCamera
```csharp
[Header("Cấu hình vị trí")]
khoangCachBanDau = 5f        // Khoảng cách mặc định
khoangCachToiThieu = 2f      // Zoom tối đa vào
khoangCachToiDa = 15f        // Zoom tối đa ra
doCaoCamera = 2f             // Độ cao so với NPC

[Header("Cấu hình điều khiển")]
doNhayChuot = 2f             // Độ nhạy xoay
tocDoZoom = 2f               // Tốc độ zoom
tocDoLerpCamera = 5f         // Tốc độ di chuyển mềm
tocDoLerpXoay = 10f          // Tốc độ xoay mềm
```

## 🚨 Xử lý sự cố

### Camera không di chuyển được
- ✅ Kiểm tra `CameraController` đã được thêm vào Main Camera
- ✅ Đảm bảo Main Camera có tag "MainCamera"
- ✅ Kiểm tra `QuanLyCamera` đã được thiết lập đúng

### Camera NPC không hoạt động
- ✅ Kiểm tra `NPCCamera` component đã được thêm
- ✅ Đảm bảo NPC có Collider hoặc visual để camera có thể theo dõi
- ✅ Kiểm tra cấu hình khoảng cách trong Inspector

### Lỗi AudioListener
- ✅ `QuanLyCamera` tự động quản lý AudioListener
- ✅ Bật "Tự động quản lý AudioListener" trong Inspector
- ✅ Kiểm tra Console để xem thông báo debug

### Input không hoạt động
- ✅ Đảm bảo Input System đã được cài đặt
- ✅ Kiểm tra `InputSystem_Actions.inputactions` có chứa Camera actions
- ✅ Regenerate C# Classes nếu cần thiết

## 📝 Lưu ý quan trọng

1. **Performance**: Hệ thống được tối ưu cho nhiều camera
2. **Input System**: Sử dụng Unity Input System mới
3. **AudioListener**: Tự động quản lý để tránh xung đột
4. **Nullable Types**: Code sử dụng nullable reference types
5. **Debug**: Sử dụng `TestCameraSystem` để kiểm tra

## 🎯 Tích hợp với dự án hiện tại

Hệ thống mới **hoàn toàn tương thích** với:
- ✅ NPCController hiện có
- ✅ Input System đã thiết lập
- ✅ AudioListener management
- ✅ Camera switching logic

**Không cần thay đổi** code hiện có, chỉ cần:
1. Add components mới
2. Cấu hình parameters
3. Test và tinh chỉnh
