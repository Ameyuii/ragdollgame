# 🔄 Hướng dẫn bật Auto Compile trong Unity

## 🎯 Mục tiêu: Unity tự động biên dịch khi sửa code trong VSCode

### ✅ **Cách 1: Bật Auto Refresh trong Unity (Khuyến nghị)**

1. **Mở Unity Editor**
2. **Vào menu: Edit → Preferences** (Windows) hoặc **Unity → Preferences** (Mac)
3. **Chọn tab "General"**
4. **Tìm mục "Auto Refresh"**
5. **✅ Đánh dấu tick vào "Auto Refresh"**
6. **✅ Đánh dấu tick vào "Auto Refresh (Script Changes Only)" nếu có**

### 🔧 **Cách 2: Thiết lập Asset Serialization**

1. **Vào menu: Edit → Project Settings**
2. **Chọn "Editor" trong panel bên trái**
3. **Đặt "Asset Serialization" thành "Force Text"**
4. **Đặt "Version Control" thành "Visible Meta Files"**

### ⚡ **Cách 3: Sử dụng Force Recompile Script**

Tạo script tự động force recompile:
```csharp
// Menu item để force compile
[MenuItem("Tools/Force Recompile")]
static void ForceRecompile()
{
    UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
}
```

### 🚀 **Cách 4: Script theo dõi file thay đổi**

Tạo EditorScript tự động theo dõi thay đổi file:
```csharp
[InitializeOnLoad]
public class AutoRecompiler
{
    static AutoRecompiler()
    {
        EditorApplication.update += CheckForChanges;
    }
    
    static void CheckForChanges()
    {
        if (EditorApplication.isCompiling) return;
        
        // Kiểm tra thay đổi và force recompile nếu cần
        AssetDatabase.Refresh();
    }
}
```

## 🛠️ **Khắc phục sự cố:**

### Nếu Auto Refresh không hoạt động:
1. Restart Unity Editor
2. Kiểm tra Unity không bị focus loss
3. Đảm bảo file .cs được save trong VSCode (Ctrl+S)
4. Kiểm tra Unity không bị pause/lock

### Tăng tốc độ compile:
1. Giảm số lượng file script
2. Sử dụng Assembly Definitions
3. Tắt các package không cần thiết

## ⚙️ **Cài đặt bổ sung:**

### VSCode Extensions hữu ích:
- Unity Tools
- C# for Visual Studio Code
- Unity Code Snippets

### Unity Package Manager:
- Visual Studio Code Editor package
- Input System (nếu dùng)

## 🎯 **Lưu ý quan trọng:**

- Auto Refresh chỉ hoạt động khi Unity Editor đang chạy
- Unity phải có focus hoặc running in background
- File phải được save (Ctrl+S) trong VSCode
- Có thể có delay 1-2 giây trước khi compile

---

**✅ Sau khi thiết lập: VSCode save → Unity auto compile trong vài giây!**
