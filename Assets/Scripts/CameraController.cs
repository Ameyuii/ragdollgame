using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller cho camera chính có thể di chuyển tự do trong không gian 3D
/// Hỗ trợ di chuyển bằng WASD + QE và xoay bằng chuột
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Cấu hình di chuyển")]
    [SerializeField, Tooltip("Tốc độ di chuyển camera")]
    private float tocDoChuyenDong = 10f;

    [SerializeField, Tooltip("Tốc độ di chuyển nhanh (khi giữ Shift)")]
    private float tocDoChuyenDongNhanh = 20f;    [SerializeField, Tooltip("Tốc độ lên xuống (Q/E)")]
    private float tocDoLenXuong = 5f;

    [Header("Cấu hình xoay")]
    [SerializeField, Tooltip("Tốc độ xoay camera chính (độ/giây)")]
    private float tocDoXoayCamera = 150f; // Tăng từ 120f để consistent với NPC camera

    [SerializeField, Tooltip("Nhân tốc độ xoay nhanh khi giữ Shift")]
    private float nhanTocDoXoayNhanh = 2.5f; // Tăng từ 2f để consistent với NPC camera

    [SerializeField, Tooltip("Độ nhạy xoay chuột")]
    private float doNhayChuot = 3f; // Tăng từ 2f để consistent

    [SerializeField, Tooltip("Giới hạn góc xoay lên xuống")]
    private float gioiHanGocXoay = 90f;

    [SerializeField, Tooltip("Làm mềm chuyển động")]
    private float doDaiLamMem = 0.1f;

    // Biến quản lý input
    private Vector2 inputDiChuyen;
    private Vector2 inputXoayChuot;
    private bool inputLenCao;
    private bool inputXuongThap;
    private bool inputChuyenDongNhanh;

    // Biến quản lý xoay
    private float gocXoayX = 0f;
    private float gocXoayY = 0f;

    // Biến làm mềm chuyển động
    private Vector3 vanTocHienTai;
    private Vector3 vanTocLamMem;

    /// <summary>
    /// Khởi tạo góc xoay ban đầu
    /// </summary>
    private void Start()
    {
        // Lấy góc xoay hiện tại của camera
        Vector3 gocXoayHienTai = transform.eulerAngles;
        gocXoayY = gocXoayHienTai.y;
        gocXoayX = gocXoayHienTai.x;

        // Chuyển đổi góc X về khoảng -90 đến 90
        if (gocXoayX > 180f)
            gocXoayX -= 360f;
    }

    /// <summary>
    /// Cập nhật chuyển động và xoay camera
    /// </summary>
    private void Update()
    {
        // Chỉ xử lý khi component này được kích hoạt
        if (!enabled) return;

        XuLyXoayCamera();
        XuLyDiChuyenCamera();
    }    /// <summary>
    /// Xử lý xoay camera bằng chuột (chỉ khi giữ chuột phải)
    /// </summary>
    private void XuLyXoayCamera()
    {
        if (Mouse.current == null) return;        // Chỉ xoay khi giữ chuột phải
        if (Mouse.current.rightButton.isPressed)
        {
            // Lấy input xoay chuột
            Vector2 deltaXoayChuot = Mouse.current.delta.ReadValue();

            // Kiểm tra tăng tốc xoay bằng Shift
            float tocDoXoayHienTai = tocDoXoayCamera;
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            {
                tocDoXoayHienTai *= nhanTocDoXoayNhanh;
                Debug.Log($"🚀 Boost tốc độ xoay camera chính: {tocDoXoayHienTai}°/s");
            }

            // Cập nhật góc xoay với tốc độ tùy chỉnh (tăng multiplier cho responsive hơn)
            gocXoayX -= deltaXoayChuot.y * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.015f; // Tăng từ 0.01f
            gocXoayY += deltaXoayChuot.x * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.015f; // Tăng từ 0.01f

            // Giới hạn góc xoay lên xuống
            gocXoayX = Mathf.Clamp(gocXoayX, -gioiHanGocXoay, gioiHanGocXoay);

            // Áp dụng xoay
            transform.rotation = Quaternion.Euler(gocXoayX, gocXoayY, 0f);
        }
    }

    /// <summary>
    /// Xử lý di chuyển camera
    /// </summary>
    private void XuLyDiChuyenCamera()
    {
        if (Keyboard.current == null) return;

        // Lấy input di chuyển
        Vector2 inputWASD = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) inputWASD.y += 1f;
        if (Keyboard.current.sKey.isPressed) inputWASD.y -= 1f;
        if (Keyboard.current.aKey.isPressed) inputWASD.x -= 1f;
        if (Keyboard.current.dKey.isPressed) inputWASD.x += 1f;

        // Lấy input lên xuống
        float inputLenXuong = 0f;
        if (Keyboard.current.qKey.isPressed) inputLenXuong += 1f;
        if (Keyboard.current.eKey.isPressed) inputLenXuong -= 1f;

        // Kiểm tra chuyển động nhanh
        bool chuyenDongNhanh = Keyboard.current.leftShiftKey.isPressed;

        // Tính toán hướng di chuyển
        Vector3 huongDiChuyen = Vector3.zero;

        // Di chuyển theo hướng camera nhìn (không bị ảnh hưởng bởi góc lên xuống)
        Vector3 huongTruoc = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 huongPhai = transform.right;

        huongDiChuyen += huongTruoc * inputWASD.y;
        huongDiChuyen += huongPhai * inputWASD.x;
        huongDiChuyen += Vector3.up * inputLenXuong;

        // Tính tốc độ di chuyển
        float tocDoHienTai = chuyenDongNhanh ? tocDoChuyenDongNhanh : tocDoChuyenDong;
        if (inputLenXuong != 0f)
        {
            // Sử dụng tốc độ riêng cho lên xuống
            Vector3 vanTocLenXuong = Vector3.up * inputLenXuong * tocDoLenXuong;
            Vector3 vanTocNgang = new Vector3(huongDiChuyen.x, 0, huongDiChuyen.z) * tocDoHienTai;
            huongDiChuyen = vanTocNgang + vanTocLenXuong;
        }
        else
        {
            huongDiChuyen *= tocDoHienTai;
        }

        // Làm mềm chuyển động
        vanTocHienTai = Vector3.SmoothDamp(vanTocHienTai, huongDiChuyen, ref vanTocLamMem, doDaiLamMem);

        // Áp dụng di chuyển
        transform.position += vanTocHienTai * Time.deltaTime;
    }

    /// <summary>
    /// Đặt lại vị trí và góc xoay camera
    /// </summary>
    /// <param name="viTriMoi">Vị trí mới</param>
    /// <param name="gocXoayMoi">Góc xoay mới</param>
    public void DatLaiViTriCamera(Vector3 viTriMoi, Vector3 gocXoayMoi)
    {
        transform.position = viTriMoi;
        transform.rotation = Quaternion.Euler(gocXoayMoi);
        
        // Cập nhật biến góc xoay nội bộ
        gocXoayY = gocXoayMoi.y;
        gocXoayX = gocXoayMoi.x;
        
        // Reset velocity
        vanTocHienTai = Vector3.zero;
        vanTocLamMem = Vector3.zero;
    }

    /// <summary>
    /// Bật/tắt điều khiển camera
    /// </summary>
    /// <param name="kichHoat">Trạng thái kích hoạt</param>
    public void BatTatDieuKhien(bool kichHoat)
    {
        enabled = kichHoat;
        
        // Reset input khi tắt
        if (!kichHoat)
        {
            vanTocHienTai = Vector3.zero;
            vanTocLamMem = Vector3.zero;
        }
    }

    /// <summary>
    /// Đặt tốc độ xoay camera
    /// </summary>
    /// <param name="tocDoMoi">Tốc độ xoay mới (độ/giây)</param>
    public void DatTocDoXoay(float tocDoMoi)
    {
        tocDoXoayCamera = Mathf.Max(0f, tocDoMoi);
    }

    /// <summary>
    /// Lấy tốc độ xoay camera hiện tại
    /// </summary>
    /// <returns>Tốc độ xoay hiện tại</returns>
    public float LayTocDoXoay()
    {
        return tocDoXoayCamera;
    }

    /// <summary>
    /// Đặt độ nhạy chuột
    /// </summary>
    /// <param name="doNhayMoi">Độ nhạy mới</param>
    public void DatDoNhayChuot(float doNhayMoi)
    {
        doNhayChuot = Mathf.Max(0f, doNhayMoi);
    }

    /// <summary>
    /// Lấy độ nhạy chuột hiện tại
    /// </summary>
    /// <returns>Độ nhạy chuột hiện tại</returns>
    public float LayDoNhayChuot()
    {
        return doNhayChuot;
    }

    /// <summary>
    /// Đặt tốc độ di chuyển
    /// </summary>
    /// <param name="tocDoMoi">Tốc độ di chuyển mới</param>
    public void DatTocDoChuyenDong(float tocDoMoi)
    {
        tocDoChuyenDong = Mathf.Max(0f, tocDoMoi);
    }

    /// <summary>
    /// Lấy tốc độ di chuyển hiện tại
    /// </summary>
    /// <returns>Tốc độ di chuyển hiện tại</returns>
    public float LayTocDoChuyenDong()
    {
        return tocDoChuyenDong;
    }

    /// <summary>
    /// Đặt nhân tốc độ xoay nhanh (khi giữ Shift)
    /// </summary>
    /// <param name="nhanMoi">Nhân tốc độ mới</param>
    public void DatNhanTocDoXoayNhanh(float nhanMoi)
    {
        nhanTocDoXoayNhanh = Mathf.Max(1f, nhanMoi);
    }

    /// <summary>
    /// Lấy nhân tốc độ xoay nhanh hiện tại
    /// </summary>
    /// <returns>Nhân tốc độ xoay nhanh</returns>
    public float LayNhanTocDoXoayNhanh()
    {
        return nhanTocDoXoayNhanh;
    }
}
