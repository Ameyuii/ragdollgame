# 🚀 Template Cấu hình GitHub Copilot cho Dự án Unity Mới

## 📁 Files cần copy (theo thứ tự ưu tiên)

### 1. `.github/copilot-instructions.md` ⭐ **BẮT BUỘC**
- File chính chứa tất cả quy tắc
- Copy nguyên xi sang dự án mới
- Đường dẫn: `[dự án mới]/.github/copilot-instructions.md`

### 2. `.vscode/settings.json` ⚠️ **CẦN MERGE**
Copy các settings này vào file settings.json của dự án mới:

```json
{
    "github.copilot.enable": {
        "*": true,
        "plaintext": true,
        "markdown": true,
        "csharp": true
    },
    "github.copilot.advanced": {
        "language": "Vietnamese",
        "defaultCommentLanguage": "Vietnamese",
        "contextLines": 20,
        "inlineSuggestCount": 3
    },
    "github.copilot.chat.welcomeMessage": "Chào bạn! Tôi sẽ hỗ trợ bạn với dự án Unity bằng tiếng Việt theo quy tắc trong copilot-instructions.md",
    "editor.insertSpaces": true,
    "editor.tabSize": 4,
    "editor.detectIndentation": false,
    "editor.suggest.insertMode": "replace",
    "editor.acceptSuggestionOnCommitCharacter": false,
    "editor.acceptSuggestionOnEnter": "on"
}
```

### 3. `.editorconfig` ⚠️ **CẦN MERGE**
Thêm các quy tắc này vào .editorconfig:

```properties
# Quy tắc đặc biệt cho dự án Unity với tiếng Việt
# GitHub Copilot sẽ tuân theo quy tắc này

# Naming conventions cho tiếng Việt
dotnet_naming_rule.vietnamese_naming_allowed.severity = none
dotnet_naming_rule.local_variables_should_be_camel_case.severity = suggestion
dotnet_naming_rule.public_members_should_be_pascal_case.severity = suggestion

# Unity-specific files
[*.{meta,unity,prefab,asset}]
indent_style = space
indent_size = 2

# Shader files
[*.{shader,cginc,hlsl,compute}]
indent_style = space
indent_size = 4
```

## 🎯 Cách setup nhanh (3 bước)

### Bước 1: Copy file chính
```powershell
# Từ dự án hiện tại
Copy-Item ".github\copilot-instructions.md" "C:\path\to\new\project\.github\copilot-instructions.md"
```

### Bước 2: Merge settings.json
- Mở `.vscode/settings.json` của dự án mới
- Thêm các settings GitHub Copilot từ template trên
- Không xóa settings hiện có, chỉ thêm vào

### Bước 3: Restart VS Code
- Mở dự án mới trong VS Code
- Restart VS Code (Ctrl+Shift+P → "Developer: Reload Window")
- Test GitHub Copilot bằng cách tạo comment tiếng Việt

## ✅ Kiểm tra hoạt động

Tạo file test mới và gõ:
```csharp
/// <summary>
/// Quản lý 
```
GitHub Copilot sẽ gợi ý tiếng Việt nếu setup thành công!

## 🔧 Troubleshooting

**Copilot vẫn suggest tiếng Anh?**
- Kiểm tra file `.github/copilot-instructions.md` đã có chưa
- Restart VS Code
- Gõ thêm context tiếng Việt để guide Copilot

**Settings không áp dụng?**
- Kiểm tra JSON syntax trong settings.json
- Reload Window trong VS Code
- Check GitHub Copilot extension đã enable chưa

---

## 📦 Files trong package này:
- `CopyCopilotConfig.ps1` - Script tự động copy
- `.github/copilot-instructions.md` - File hướng dẫn chính  
- `.vscode/settings.json` - VS Code settings (đã merge)
- `.editorconfig` - Code style rules (đã merge)
- `COPILOT_SETUP_README.md` - Documentation chi tiết
