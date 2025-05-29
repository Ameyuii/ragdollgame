# Hệ Thống Camera NPC

## 1. Tổng Quan

Hệ thống camera NPC cung cấp các tính năng sau:
- Tạo camera riêng cho mỗi NPC để theo dõi hoạt động
- Quản lý chuyển đổi giữa camera chính và các camera NPC
- Điều khiển camera thông qua phím tắt

## 2. Các Script

### 2.1. NPCCamera.cs
Script này được gắn vào từng NPC mà bạn muốn tạo camera riêng.

#### Chức năng chính:
- Tự động tạo camera gắn liền với NPC
- Theo dõi vị trí và hướng nhìn của NPC
- Cung cấp các tham số tùy chỉnh như offset, góc nhìn và độ mượt

#### Các tham số có thể tùy chỉnh:
- **Kích Hoạt Khi Start**: Camera có được bật ngay từ đầu hay không
- **Vị Trí Offset**: Vị trí tương đối của camera so với NPC (mặc định là phía sau và cao hơn NPC)
- **Góc Xoay**: Góc nhìn của camera
- **Tốc Độ Lerp Camera**: Điều chỉnh độ mượt khi camera di chuyển theo NPC

### 2.2. QuanLyCamera.cs
Script này quản lý việc chuyển đổi giữa các camera trong scene.

#### Chức năng chính:
- Quản lý danh sách camera (camera chính và các camera NPC)
- Xử lý input để chuyển đổi giữa các camera
- Tự động tìm và đăng ký các NPCCamera trong scene

#### Các tham số có thể tùy chỉnh:
- **Camera Chính**: Tham chiếu đến camera chính trong scene
- **Thời Gian Chuyển Camera**: Độ trễ khi chuyển đổi giữa các camera
- **Tự Động Tìm NPC Camera**: Tự động tìm tất cả NPCCamera trong scene
- **Danh Sách NPC Camera**: Danh sách thủ công các camera NPC (nếu không tự động tìm)

## 3. Cách Cài Đặt

### 3.1. Thiết lập Camera Manager:
1. Tạo một GameObject mới và đặt tên là "CameraManager"
2. Gắn component `QuanLyCamera` vào GameObject này
3. Kéo Camera chính của scene vào trường "Camera Chinh" trong Inspector
4. Nếu muốn chỉ định thủ công các NPC có camera:
   - Tắt tùy chọn "Tự Động Tìm NPC Camera"
   - Kéo các NPC có NPCCamera vào danh sách "Danh Sách NPC Camera"

### 3.2. Thiết lập Camera cho từng NPC:
1. Chọn GameObject NPC cần theo dõi
2. Đảm bảo NPC đã có component NPCController
3. Thêm component `NPCCamera` vào NPC
4. Điều chỉnh các thông số trong Inspector theo nhu cầu

## 4. Cách Sử Dụng

### 4.1. Các phím tắt:
- **Phím 0**: Chuyển về camera chính
- **Phím 1**: Chuyển đến camera NPC tiếp theo
  
### 4.2. API:
- `QuanLyCamera.BatCameraChinh()`: Chuyển về camera chính
- `QuanLyCamera.ChuyenCameraKeTiep()`: Chuyển đến camera NPC tiếp theo
- `QuanLyCamera.ChuyenSangCamera(int chiSoCamera)`: Chuyển đến camera có chỉ số cụ thể
- `QuanLyCamera.ThemCameraNPC(NPCCamera npcCamera)`: Thêm camera NPC vào danh sách quản lý
- `QuanLyCamera.XoaCameraNPC(NPCCamera npcCamera)`: Xóa camera NPC khỏi danh sách quản lý

## 5. Lưu Ý Khi Sử Dụng

- Đảm bảo Input System đã được cài đặt trong project
- Mỗi NPC chỉ nên có một component NPCCamera
- Component NPCCamera yêu cầu component NPCController
- Camera NPC sẽ tự động được tạo khi game chạy, không cần tạo thủ công
- Nếu bạn gặp vấn đề với hiệu năng khi có nhiều camera, hãy giảm "Tốc Độ Lerp Camera" trong NPCCamera
