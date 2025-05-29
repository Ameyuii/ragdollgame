# Hướng dẫn sử dụng hệ thống camera NPC

## Tổng quan
Hệ thống camera cho phép bạn theo dõi hoạt động của các NPC trong game bằng cách chuyển đổi giữa camera chính và các camera gắn liền với từng NPC.

## Các script đã tạo

### 1. NPCCamera.cs
Script này được gắn vào các NPC để tạo camera riêng theo dõi hoạt động của NPC đó. Camera này sẽ di chuyển theo vị trí của NPC và cung cấp góc nhìn từ phía sau NPC.

### 2. QuanLyCamera.cs
Script quản lý chính của hệ thống camera, cho phép chuyển đổi giữa camera chính và các camera NPC.

**Cách sử dụng:**
1. Gắn script này vào một GameObject trong scene (thường là Main Camera)
2. Cấu hình trong Inspector:
   - Gán Camera chính vào slot "Camera Chính"
   - Chọn "Tự Động Tìm NPC Camera" để tự động phát hiện tất cả NPCCamera trong scene

### 3. NPCController.cs
Controller chính cho NPC, xử lý di chuyển và hành vi cơ bản của NPC.

### 4. CameraController.cs
Controller cho camera chính, cho phép di chuyển và xoay camera tự do.

- Điều chỉnh các thông số trong Inspector nếu cần:
  - `Vị Trí Offset`: Vị trí tương đối của camera so với NPC
  - `Góc Xoay`: Góc quay của camera
  - `Tốc Độ Lerp Camera`: Độ mượt khi camera di chuyển theo NPC

### 5. DieuChinhThongSoCamera.cs (SCRIPT QUAN TRỌNG)
**Script chức năng chính để điều chỉnh thông số camera runtime.**

Script này cung cấp:
- UI panel để điều chỉnh các thông số camera trong lúc chạy game
- Khả năng chuyển đổi giữa camera chính và các camera NPC  
- Hiệu chỉnh tốc độ xoay, zoom và các thông số khác
- Hệ thống shared parameters cho tất cả NPC cameras cùng lúc

**Cách sử dụng:**
1. Gắn script này vào một GameObject trong scene
2. Trong game, nhấn icon ⚙️ ở góc màn hình để mở panel điều chỉnh
3. Sử dụng các slider để điều chỉnh thông số camera
4. Điều chỉnh thông số cho tất cả NPC cameras cùng lúc

**Lưu ý quan trọng:** Script này có thể được sử dụng trong build game để điều chỉnh camera, không chỉ trong Unity Editor.

## Cách sử dụng

### Các phím điều khiển mặc định
- Phím **0**: Chuyển về camera chính
- Phím **1**: Chuyển đến camera NPC tiếp theo

## Lưu ý khi sử dụng
- Component QuanLyCamera có thể tự động tìm tất cả NPCCamera trong scene nếu bạn đặt "Tự Động Tìm NPC Camera" là true
- Nếu bạn muốn chỉ định thủ công các NPC có camera, hãy tắt tùy chọn trên và kéo các NPC vào danh sách "Danh Sách Camera NPC"
- Camera NPC sẽ tự động được tạo khi game chạy, không cần tạo thủ công
- Đảm bảo Input System đã được cài đặt và cấu hình trong dự án
- Script NPCCamera yêu cầu component NPCController được gắn vào cùng GameObject
- **DieuChinhThongSoCamera** là script chính để điều chỉnh camera - luôn thêm vào scene để có thể điều chỉnh camera runtime
