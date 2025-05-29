# Hướng Dẫn Test UI Camera Settings

## ✅ Đã Sửa
1. **autoShowOnStart = true** - UI sẽ hiển thị ngay khi start
2. **hienThiPanel = true** - Panel mở mặc định  
3. **NPCRagdollManager hienThiDebug = false** - UI debug ragdoll đã tắt
4. **Icon hiển thị rõ ràng** - "📹 ON" / "📹 OFF"

## 🎮 Cách Test
1. **Chạy game trong Unity**
2. **Tìm icon camera** ở góc trên phải màn hình
3. **Panel Camera Settings** sẽ tự động hiển thị (autoShowOnStart = true)
4. **Click icon** để toggle panel bật/tắt

## 📋 Checklist
- [ ] Icon camera hiển thị ở góc trên phải
- [ ] Panel Camera Settings tự động mở khi start
- [ ] Click icon để toggle panel  
- [ ] KHÔNG thấy UI debug ragdoll
- [ ] Console log hiển thị: "Panel Điều Chỉnh Camera: OPENED"

## 🔧 Nếu vẫn lỗi
1. Kiểm tra GameObject có `DieuChinhThongSoCamera` script
2. Đảm bảo script enabled
3. Check Console logs

## 📝 Script Test
Đã tạo `TestCameraUI.cs` để verify UI hoạt động đúng
