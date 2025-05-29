# 🎥 Hệ thống Camera Unity - Hướng dẫn khắc phục lỗi

## ✅ Đã khắc phục các lỗi sau:

### 1. **Lỗi Nullable Reference Type**
- **Vấn đề**: Biến `Camera? npcCamera` gây cảnh báo nullable reference
- **Giải pháp**: Đã sửa thành `Camera? npcCamera` với cú pháp nullable chính xác

### 2. **Warning CS0414 trong InputSystem_Actions.cs**
- **Vấn đề**: Các biến scheme index không được sử dụng
- **Giải pháp**: Đã thêm `#pragma warning disable CS0414` để bỏ qua warning

### 3. **File trống gây lỗi compilation**
- **Vấn đề**: `NPCController_new.cs` và `TeamAssignment.cs` trống
- **Giải pháp**: Đã xóa các file trống

## 🛠️ Scripts đã thêm:

### 1. **CameraSystemTester.cs**
- Kiểm tra tự động hệ thống camera
- Tự động sửa các vấn đề về tag và đăng ký camera
- Báo cáo tình trạng hệ thống

### 2. **CameraInstructions.cs**
- Hiển thị hướng dẫn sử dụng trên màn hình
- Hướng dẫn phím điều khiển camera

## 🎮 Cách sử dụng:

### Điều khiển Camera:
- **Phím 0**: Chuyển về camera chính
- **Phím 1**: Chuyển giữa các camera NPC

### Debug và kiểm tra:
- **Phím F1**: Hiển thị danh sách camera (trong CameraManager)
- **Phím F5**: Kiểm tra hệ thống camera (CameraSystemTester)

## 📋 Cách thiết lập cho scene mới:

1. **Thêm CameraManager:**
   ```
   - Tạo GameObject trống tên "Camera Manager"
   - Thêm script CameraManager.cs
   - Gán Main Camera vào trường mainCamera
   ```

2. **Thêm NPCCamera cho NPC:**
   ```
   - Chọn NPC GameObject
   - Thêm script NPCCamera.cs
   - Đảm bảo tag của NPC là "NPC" hoặc bật ignoreTagCheck
   ```

3. **Thêm hỗ trợ debug (tùy chọn):**
   ```
   - Tạo GameObject trống tên "Camera Tester"
   - Thêm script CameraSystemTester.cs
   - Thêm script CameraInstructions.cs
   ```

## 🚨 Các lỗi thường gặp và cách sửa:

### Lỗi: "Không tìm thấy CameraManager"
- **Nguyên nhân**: Chưa có CameraManager trong scene
- **Cách sửa**: Thêm script CameraManager vào một GameObject

### Lỗi: "Camera không được tạo"
- **Nguyên nhân**: NPC không có tag "NPC"
- **Cách sửa**: 
  - Thay đổi tag của NPC thành "NPC", HOẶC
  - Bật `ignoreTagCheck` trong NPCCamera component

### Lỗi: "Không có camera NPC để chuyển đổi"
- **Nguyên nhân**: Không có NPCCamera nào được đăng ký thành công
- **Cách sửa**: Kiểm tra tag của các NPC và đảm bảo NPCCamera được thêm đúng cách

## 📝 Notes:
- Tất cả scripts đã được tối ưu hóa và không còn lỗi compilation
- Hệ thống tự động phát hiện và sửa một số vấn đề thường gặp
- Sử dụng tiếng Việt trong comments và debug messages
