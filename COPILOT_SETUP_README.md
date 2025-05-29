# Hướng dẫn GitHub Copilot cho Dự án Unity

## Tổng quan
Dự án này đã được cấu hình để GitHub Copilot tuân theo các quy tắc cụ thể nhằm đảm bảo tính nhất quán trong code và sử dụng tiếng Việt.

## Các file cấu hình

### 1. `.github/copilot-instructions.md`
- **Mục đích**: Hướng dẫn chính cho GitHub Copilot
- **Nội dung**: Quy tắc ngôn ngữ, phong cách code, naming conventions
- **Quan trọng**: Đây là file chính mà Copilot sẽ tham khảo
- **🚨 CẢNH BÁO**: File này TUYỆT ĐỐI KHÔNG ĐƯỢC XÓA trong bất kỳ trường hợp nào. Nếu bị mất, toàn bộ hệ thống Copilot sẽ mất quy tắc!

### 2. `.vscode/settings.json`
- **Mục đích**: Cấu hình VS Code và GitHub Copilot
- **Nội dung**: Language settings, IntelliSense, file associations
- **Lưu ý**: Đã cấu hình để hiển thị tiếng Việt

### 3. `.editorconfig`
- **Mục đích**: Đảm bảo code style nhất quán
- **Nội dung**: Indent, encoding, line endings
- **Áp dụng**: Tất cả editors và IDEs

## Quy tắc sử dụng

### ✅ ĐƯỢC PHÉP
- Sử dụng tiếng Việt trong comments và tên biến có ý nghĩa
- Đề xuất cải thiện performance
- Sửa lỗi compilation và runtime errors
- Thêm null checks và error handling
- Sử dụng modern C# syntax (nullable types, pattern matching)

### ❌ KHÔNG ĐƯỢC PHÉP
- Tự động thay đổi logic game mà không hỏi
- Thêm features mới mà không được yêu cầu
- Thay đổi Input System bindings tự ý
- Xóa hoặc thay đổi existing components
- **🚨 TUYỆT ĐỐI KHÔNG XÓA** file `.github/copilot-instructions.md` trong bất kỳ trường hợp nào

### 🤔 CẦN XÁC NHẬN TRƯỚC
- Thêm new components hoặc dependencies
- Thay đổi cấu trúc class hoặc architecture
- Refactor large portions of code
- Thay đổi Unity project settings

## Ví dụ code style

### ✅ Đúng
```csharp
/// <summary>
/// Quản lý camera chính và camera của các NPC
/// </summary>
public class QuanLyCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Camera chính của scene")]
    private Camera? cameraChinh;
    
    private int chiSoCameraHienTai = -1; // -1 = camera chính
    
    /// <summary>
    /// Chuyển về camera chính với hiệu ứng mượt mà
    /// </summary>
    public void ChuyenVeCameraChinh()
    {
        Debug.Log("Đang chuyển về camera chính...");
        // Implementation logic
    }
}
```

### ❌ Sai
```csharp
// English comments not allowed
public class CameraManager : MonoBehaviour
{
    public Camera mainCamera; // Should use [SerializeField] instead
    
    private int currentCameraIndex = -1; // Should use Vietnamese names
    
    public void SwitchToMainCamera() // Should use Vietnamese method names
    {
        Debug.Log("Switching to main camera..."); // Should be in Vietnamese
    }
}
```

## Input System Guidelines

### ✅ Sử dụng Input System mới
```csharp
private InputSystem_Actions? inputActions;

void OnEnable()
{
    inputActions ??= new InputSystem_Actions();
    inputActions.Camera.MainCamera.performed += OnChuyenVeCameraChinh;
}

void OnDisable()
{
    if (inputActions != null)
    {
        inputActions.Camera.MainCamera.performed -= OnChuyenVeCameraChinh;
        inputActions.Disable();
    }
}
```

### ❌ KHÔNG sử dụng legacy Input
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Alpha0)) // ❌ Legacy Input - causes errors
    {
        SwitchToMainCamera();
    }
}
```

## Troubleshooting

### GitHub Copilot không tuân theo quy tắc?
1. Restart VS Code
2. Check file `.github/copilot-instructions.md` exists
3. Reload Window (Ctrl+Shift+P → "Developer: Reload Window")
4. Check GitHub Copilot extension is updated

### Copilot suggestions bằng tiếng Anh?
1. Check `.vscode/settings.json` → `github.copilot.advanced.language`
2. Manually type Vietnamese comments to guide Copilot
3. Reference existing Vietnamese code in the project

### IntelliSense không hoạt động đúng?
1. Check OmniSharp is running properly
2. Reload project: Ctrl+Shift+P → "OmniSharp: Restart OmniSharp"
3. Check `.vscode/settings.json` for correct Unity paths

## Bảo vệ File Quan trọng

### 🚨 File KHÔNG ĐƯỢC XÓA
- `.github/copilot-instructions.md` - File cấu hình chính của GitHub Copilot
- `Assets/InputSystem_Actions.inputactions` - Input system bindings
- `ProjectSettings/` folder - Unity project settings

### Khôi phục file bị mất
Nếu file `copilot-instructions.md` bị mất:
1. Tạo lại từ backup hoặc git history
2. Restart VS Code hoàn toàn
3. Test lại Copilot suggestions

## Team Guidelines

- **Luôn review** code suggestions từ Copilot trước khi accept
- **Báo cáo** nếu Copilot đề xuất thay đổi không mong muốn
- **Cập nhật** file instructions khi có quy tắc mới
- **Test thoroughly** after accepting Copilot suggestions

## Liên hệ
Nếu có vấn đề với GitHub Copilot hoặc cần cập nhật quy tắc, liên hệ với team lead.

---
*File này được tạo tự động và cần được cập nhật khi có thay đổi trong dự án.*
