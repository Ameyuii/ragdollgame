# 🎯 TestCameraSystem V3.0 - Shared NPC Parameters System

## 📖 Tổng quan

TestCameraSystem đã được nâng cấp lên V3.0 với hệ thống **Shared Parameters** cho tất cả NPC cameras. Thay vì điều chỉnh từng NPC camera riêng lẻ, giờ đây bạn có thể điều chỉnh thông số cho TẤT CẢ NPC cameras cùng một lúc.

## 🔧 Tính năng mới

### ✅ Shared NPC Parameters
- **Tốc độ xoay**: Áp dụng cho tất cả NPC cameras
- **Nhân boost**: Áp dụng cho tất cả NPC cameras  
- **Độ nhạy chuột**: Áp dụng cho tất cả NPC cameras
- **Khoảng cách**: Áp dụng cho tất cả NPC cameras

### ✅ Real-time Updates
- Khi thay đổi slider, thông số được áp dụng ngay lập tức cho tất cả NPC cameras
- Hiển thị số lượng NPC cameras được áp dụng trong UI
- Log chi tiết số lượng cameras đã được update

### ✅ Auto-detection System
- Tự động tìm và load tất cả NPC cameras trong scene
- Refresh button để cập nhật danh sách khi có NPC mới
- Load shared parameters từ NPC camera đầu tiên làm default

## 🎮 Cách sử dụng

### 1. Mở Camera Debug Panel
- Nhấn icon 🎮 ở góc màn hình để mở panel
- Panel hiển thị "V3.0 (SHARED NPC)" trong title

### 2. Kiểm tra thông tin
- **📊 THÔNG TIN CAMERA**: Hiển thị camera hiện tại và số lượng NPC cameras
- **🔗 SHARED NPC**: Hiển thị thông số chung đang áp dụng

### 3. Điều chỉnh Camera Chính
- **⚙️ ĐIỀU CHỈNH CAMERA CHÍNH**: Chỉ áp dụng cho camera chính
- Tốc độ xoay, nhân boost, độ nhạy chuột, tốc độ di chuyển

### 4. Điều chỉnh TẤT CẢ NPC Cameras
- **🎯 SHARED NPC PARAMETERS (TẤT CẢ NPC)**: Áp dụng cho tất cả NPC
- Mỗi slider hiển thị `ALL (X)` với X là số lượng NPC cameras
- Thay đổi slider → tự động áp dụng cho tất cả NPC ngay lập tức

### 5. Reset và Refresh
- **🔄 Reset Camera Chính**: Reset camera chính về mặc định
- **🎯 Reset ALL NPC**: Reset tất cả NPC cameras về mặc định  
- **🔄 Refresh NPC List**: Cập nhật danh sách NPC cameras

## 📝 Thông số mặc định

### Camera Chính
```
Tốc độ xoay: 150°/s
Nhân boost: x2.5
Độ nhạy chuột: 3.0
Tốc độ di chuyển: 10.0
```

### Shared NPC Parameters
```
Tốc độ xoay: 150°/s
Nhân boost: x2.5
Độ nhạy chuột: 3.0
Khoảng cách: 5.0
```

## 🔄 Workflow Test

### 1. Test cơ bản
```
1. Mở panel → Kiểm tra số lượng NPC cameras
2. Điều chỉnh slider "Tốc độ xoay ALL" 
3. Kiểm tra Console logs → "Đã áp dụng tốc độ xoay XXX°/s cho X NPC cameras"
4. Chuyển sang NPC camera và test xoay
```

### 2. Test với nhiều NPC
```
1. Nhấn "🔧 Tạo NPC test" để tạo thêm NPC cameras
2. Nhấn "🔄 Refresh NPC List" để cập nhật danh sách
3. Điều chỉnh shared parameters
4. Kiểm tra tất cả NPC cameras đều có cùng thông số
```

### 3. Test Reset
```
1. Điều chỉnh shared parameters thành giá trị khác
2. Nhấn "🎯 Reset ALL NPC" 
3. Kiểm tra tất cả NPC cameras về giá trị mặc định
```

## 🚀 Tính năng nâng cao

### Auto-apply cho NPC mới
- Khi tạo NPC test mới, shared parameters được áp dụng ngay lập tức
- NPC mới tự động có cùng thông số với các NPC khác

### Smart logging
- Mỗi thay đổi đều có log chi tiết
- Hiển thị số lượng cameras được áp dụng
- Emoji icons để dễ nhận diện loại thay đổi

### Panel dragging
- Kéo thả panel bằng title bar
- Panel tự động giữ trong màn hình
- Kích thước panel tự động điều chỉnh (700px height)

## 🛠️ Troubleshooting

### Không thấy NPC cameras
```
1. Kiểm tra scene có NPCCamera components không
2. Nhấn "🔄 Refresh NPC List"
3. Kiểm tra Console logs xem có lỗi init không
```

### Shared parameters không apply
```
1. Kiểm tra NPCCamera có methods setter không
2. Kiểm tra Console logs cho error messages
3. Thử "🎯 Reset ALL NPC" và test lại
```

### Panel không hiển thị
```
1. Kiểm tra TestCameraSystem component trong scene
2. Thử gọi TogglePanel() từ code
3. Kiểm tra autoShowOnStart setting
```

## 📋 Changelog V3.0

### ✅ New Features
- Shared parameters system cho tất cả NPC cameras
- Real-time batch updates
- Smart NPC detection và auto-refresh
- Enhanced UI với số lượng cameras
- Separate reset buttons cho camera chính và NPC

### ✅ Improvements  
- Better logging với emoji và số lượng
- Auto-apply shared parameters cho NPC mới
- Larger panel size để chứa thêm controls
- Code structure được refactor để maintainable hơn

### ✅ Bug Fixes
- Fixed missing getter/setter methods
- Proper error handling cho missing components
- Consistent parameter loading và saving

## 🎯 Use Cases

### Development Testing
- Quickly tune all NPC cameras cùng lúc
- A/B test different parameter sets
- Consistent behavior across all NPCs

### Build Game Debugging  
- Runtime parameter adjustment trong build
- Performance testing với different settings
- User preference testing

### Content Creation
- Easy setup cho multiple NPCs
- Consistent camera feel across scenes
- Quick iteration on camera behavior

---

**📌 Lưu ý**: Hệ thống này được thiết kế để tối ưu workflow development và ensure consistency across tất cả NPC cameras trong project Unity.
