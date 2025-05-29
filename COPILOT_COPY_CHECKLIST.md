# ✅ Checklist Copy GitHub Copilot Config

## 🎯 Files cần copy (theo thứ tự ưu tiên)

### 📂 Bắt buộc phải có:
- [ ] `.github/copilot-instructions.md` ⭐ **QUAN TRỌNG NHẤT**

### 📂 Nên có (merge nếu đã tồn tại):
- [ ] `.vscode/settings.json` - Phần GitHub Copilot settings
- [ ] `.editorconfig` - Phần quy tắc tiếng Việt

### 📂 Tùy chọn:
- [ ] `COPILOT_SETUP_README.md` - Documentation
- [ ] `COPILOT_TEMPLATE_GUIDE.md` - Hướng dẫn setup

---

## 🚀 Cách copy nhanh nhất

### Phương án 1: Dùng script (Khuyến nghị)
```cmd
CopyCopilotConfig.bat "C:\path\to\new\project"
```

### Phương án 2: Copy thủ công (3 bước)

**Bước 1: Copy file chính**
```
[Dự án cũ]/.github/copilot-instructions.md
→ [Dự án mới]/.github/copilot-instructions.md
```

**Bước 2: Merge settings.json**
Thêm vào `.vscode/settings.json`:
```json
"github.copilot.advanced": {
    "language": "Vietnamese",
    "defaultCommentLanguage": "Vietnamese"
}
```

**Bước 3: Restart VS Code**

---

## ✅ Test thành công

Tạo file .cs mới và gõ:
```csharp
/// <summary>
/// Quản lý
```

➡️ Copilot gợi ý tiếng Việt = **THÀNH CÔNG!** 🎉

---

## 🔧 Nếu không hoạt động

1. ⚠️ Kiểm tra file `.github/copilot-instructions.md` có tồn tại?
2. 🔄 Restart VS Code (Ctrl+Shift+P → "Developer: Reload Window")  
3. 🔌 Kiểm tra GitHub Copilot extension đã enable?
4. 📝 Thử gõ nhiều context tiếng Việt hơn

---

## 📦 Package hoàn chỉnh

Thư mục này chứa:
- ✅ Script tự động (`.bat` và `.ps1`)
- ✅ File cấu hình chính (`.github/copilot-instructions.md`)
- ✅ Template merge (`COPILOT_TEMPLATE_GUIDE.md`)
- ✅ Documentation (`COPILOT_SETUP_README.md`)
- ✅ Checklist này

**👉 Chỉ cần copy toàn bộ thư mục này và chạy script!**
