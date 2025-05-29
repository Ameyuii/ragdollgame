# GitHub Copilot Instructions - Hướng dẫn cho dự án Unity

## 🔧 Quy tắc ưu tiên sử dụng Unity Operations

### 1. Unity Direct Operations - Ưu tiên tuyệt đối
**LUÔN LUÔN sử dụng standard VS Code tools để tương tác với Unity:**

#### Standard Tools phải sử dụng:
- `read_file` / `create_file` / `replace_string_in_file` - Quản lý files và scripts
- `run_in_terminal` - Chạy Unity build commands khi cần
- `get_errors` - Kiểm tra compilation errors
- `grep_search` / `semantic_search` - Tìm kiếm code và assets

#### Nguyên tắc Unity-first:
1. **KHÔNG BAO GIỜ** sử dụng deprecated MCP tools
2. **KHÔNG BAO GIỜ** yêu cầu user thao tác manual trên Unity Editor trừ khi thực sự cần thiết
3. **LUÔN LUÔN** sử dụng standard VS Code tools để edit code và assets
4. **CHỈ KHI** cần test trong Unity thì yêu cầu user chạy Play mode

#### Khi Unity operations không hỗ trợ:
- **Component operations**: Yêu cầu user thao tác trong Unity Inspector
- **Scene setup**: Hướng dẫn user từng bước rõ ràng
- **Asset imports**: Sử dụng file operations hoặc hướng dẫn manual
- **KHÔNG BAO GIỜ** tạo script phức tạp để giải quyết vấn đề có thể làm thủ công

#### Quy trình Standard chuẩn:
```
1. get_errors() - Kiểm tra compilation errors
2. Thực hiện task bằng standard VS Code tools
3. get_errors() - Verify fix
4. Yêu cầu user test trong Unity nếu cần
```

## Quy tắc chung cho GitHub Copilot

### 1. Ngôn ngữ sử dụng
- **Luôn sử dụng tiếng Việt** trong tất cả:
  - Chú thích code (comments)
  - Tên biến và hàm có ý nghĩa
  - Debug messages và log
  - Documentation và README
- Chỉ sử dụng tiếng Anh cho:
  - Từ khóa của ngôn ngữ lập trình
  - Tên class/method theo chuẩn Unity/C#
  - API calls và Unity built-in functions

### 2. Phong cách code cho dự án Unity
- Sử dụng **C# modern syntax** phù hợp với Unity 2022.3+
- Sử dụng **nullable reference types** (`?`) khi thích hợp
- Sử dụng **pattern matching** và **switch expressions**
- Ưu tiên **async/await** thay vì coroutines khi có thể
- Sử dụng **Input System mới** thay vì legacy Input
- Sử dụng **UnityEngine.InputSystem** thay vì **UnityEngine.Input**

### 3. Quy tắc thiết kế và phát triển
- **KHÔNG được tự động thiết kế chức năng mới** mà không hỏi trước
- **KHÔNG được thay đổi logic hiện có** trừ khi được yêu cầu cụ thể
- **LUÔN hỏi xác nhận** trước khi:
  - Thêm component mới
  - Thay đổi cấu trúc class
  - Thêm dependencies
  - Thay đổi Input System bindings
- **CHỈ sửa lỗi** hoặc **cải thiện code hiện có** khi được yêu cầu

### 4. Cấu trúc comment tiếng Việt
```csharp
/// <summary>
/// Mô tả chi tiết chức năng bằng tiếng Việt
/// </summary>
/// <param name="tenThamSo">Mô tả tham số bằng tiếng Việt</param>
/// <returns>Mô tả giá trị trả về bằng tiếng Việt</returns>
public void TenHam(string tenThamSo)
{
    // Chú thích trong hàm bằng tiếng Việt
    Debug.Log("Log message bằng tiếng Việt");
}
```

### 5. Naming conventions cho dự án
- **Public fields/properties**: PascalCase với tên tiếng Việt có nghĩa
  ```csharp
  public Camera CameraChinh;
  public List<NPCCamera> DanhSachCameraNPC;
  ```
- **Private fields**: camelCase với prefix
  ```csharp
  private float thoiGianChuyenCamera = 0.3f;
  private int chiSoCameraHienTai = -1;
  ```
- **Methods**: PascalCase với động từ tiếng Việt
  ```csharp
  public void ChuyenVeCameraChinh()
  public void CapNhatThongTinCamera()
  ```

### 6. Unity-specific guidelines
- Luôn sử dụng **[SerializeField]** thay vì public fields
- Sử dụng **[Tooltip("Mô tả bằng tiếng Việt")]** cho editor
- Ưu tiên **ScriptableObject** cho data configuration
- Sử dụng **UnityEvent** cho loose coupling
- Implement **IDisposable** khi cần cleanup

### 7. Input System guidelines
- Sử dụng **InputSystem_Actions** class đã được generate
- Bind actions theo pattern:
  ```csharp
  inputActions.Camera.MainCamera.performed += OnChuyenVeCameraChinh;
  ```
- Luôn unregister events trong OnDisable/OnDestroy

### 8. Debug và logging với Standard Tools
- **LUÔN LUÔN** sử dụng `get_errors()` để kiểm tra compilation errors
- **LUÔN LUÔN** sử dụng `Debug.Log()` cho Unity console output
- Sử dụng **conditional compilation** cho debug code:
  ```csharp
  #if UNITY_EDITOR
  Debug.Log("Thông tin debug bằng tiếng Việt");
  #endif
  ```
- Log levels:
  - `Debug.Log()`: Thông tin bình thường
  - `Debug.LogWarning()`: Cảnh báo
  - `Debug.LogError()`: Lỗi nghiêm trọng

#### Quy trình debug chuẩn:
```
1. get_errors() - Kiểm tra compilation errors
2. read_file() - Đọc files có vấn đề
3. replace_string_in_file() - Sửa code
4. get_errors() - Verify fix
5. Yêu cầu user test trong Unity Play mode
```

### 9. Performance considerations
- Cache references trong Awake/Start
- Tránh FindObjectOfType trong Update
- Sử dụng object pooling cho frequent instantiation
- Disable components thay vì destroy khi có thể

### 10. Code review checklist
Trước khi đề xuất code, hãy kiểm tra:
- [ ] Tất cả comments đều bằng tiếng Việt
- [ ] Không thay đổi logic hiện có không cần thiết
- [ ] Sử dụng Input System mới
- [ ] Nullable types được sử dụng đúng chỗ
- [ ] Performance được tối ưu
- [ ] Tuân theo Unity best practices

## Ví dụ code mẫu
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quản lý camera trong game với Input System mới
/// </summary>
public class QuanLyCamera : MonoBehaviour
{
    [Header("Cấu hình Camera")]
    [SerializeField, Tooltip("Camera chính của scene")]
    private Camera? cameraChinh;
    
    [SerializeField, Tooltip("Danh sách camera NPC")]
    private List<Camera> danhSachCameraNPC = new List<Camera>();
    
    private InputSystem_Actions? inputActions;
    private int chiSoCameraHienTai = -1; // -1 = camera chính
    
    void OnEnable()
    {
        // Khởi tạo Input System
        inputActions ??= new InputSystem_Actions();
        inputActions.Enable();
        
        // Đăng ký sự kiện
        inputActions.Camera.MainCamera.performed += OnChuyenVeCameraChinh;
        inputActions.Camera.NextCamera.performed += OnChuyenCameraKeTiep;
    }
    
    void OnDisable()
    {
        // Hủy đăng ký sự kiện
        if (inputActions != null)
        {
            inputActions.Camera.MainCamera.performed -= OnChuyenVeCameraChinh;
            inputActions.Camera.NextCamera.performed -= OnChuyenCameraKeTiep;
            inputActions.Disable();
        }
    }
    
    private void OnChuyenVeCameraChinh(InputAction.CallbackContext context)
    {
        Debug.Log("Chuyển về camera chính");
        // Logic xử lý...
    }
}
```

## Lưu ý quan trọng

### Testing và Validation với Standard Tools
- **LUÔN** sử dụng `get_errors()` để check compilation
- **Quy trình test chuẩn:**
  ```
  1. get_errors() - Check compilation
  2. Hướng dẫn user enter Play mode
  3. Yêu cầu user báo cáo kết quả
  4. Debug qua logs và user feedback
  ```
- **KHÔNG BAO GIỜ** giả định Unity đang chạy mà không xác nhận

### Manual Operations Required
**Khi standard tools không hỗ trợ, yêu cầu user thực hiện thủ công:**
- **Xóa component**: "Vui lòng vào Unity Inspector của GameObject [Tên] và xóa component [ComponentName]"
- **Thay đổi Inspector settings**: "Vui lòng vào Inspector và chỉnh sửa [chi tiết cụ thể]"
- **Assign references**: "Vui lòng drag & drop [Asset] vào field [FieldName] trong Inspector"
- **Scene operations**: "Vui lòng select GameObject [Tên] trong Hierarchy và [hành động cụ thể]"

**KHÔNG tạo script để thay thế thao tác thủ công đơn giản!**

### Core Principles
- **LUÔN hỏi trước khi thực hiện thay đổi lớn**
- **Giữ nguyên cấu trúc code hiện có** trừ khi được yêu cầu
- **Ưu tiên sửa lỗi và cải thiện** thay vì thêm feature mới
- **Sử dụng tiếng Việt** trong mọi communication với developer
- **Standard tools first approach** cho mọi operations
