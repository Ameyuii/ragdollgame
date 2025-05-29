using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script tạo và quản lý camera riêng cho NPC
/// Camera này sẽ theo dõi hoạt động của NPC từ phía sau và hơi cao hơn một chút
/// </summary>
[RequireComponent(typeof(NPCController))]
public class NPCCamera : MonoBehaviour
{
    [Header("Cấu hình Camera")]
    [SerializeField, Tooltip("Kích hoạt camera ngay khi tạo")]
    private bool kichHoatKhiStart = false;

    [Header("Cấu hình vị trí")]
    [SerializeField, Tooltip("Khoảng cách ban đầu từ camera đến NPC")]
    private float khoangCachBanDau = 5f;

    [SerializeField, Tooltip("Khoảng cách zoom tối thiểu")]
    private float khoangCachToiThieu = 2f;

    [SerializeField, Tooltip("Khoảng cách zoom tối đa")]
    private float khoangCachToiDa = 15f;

    [SerializeField, Tooltip("Độ cao camera so với NPC")]
    private float doCaoCamera = 2f;

    [Header("Cấu hình điều khiển")]
    [SerializeField, Tooltip("Tốc độ xoay camera NPC (độ/giây)")]
    private float tocDoXoayCamera = 150f; // Tăng từ 90f lên 150f để responsive hơn

    [SerializeField, Tooltip("Nhân tốc độ xoay nhanh khi giữ Shift")]
    private float nhanTocDoXoayNhanh = 2.5f; // Tăng từ 2f lên 2.5f để boost mạnh hơn

    [SerializeField, Tooltip("Độ nhạy xoay chuột")]
    private float doNhayChuot = 3f; // Tăng từ 2f lên 3f để nhạy hơn

    [SerializeField, Tooltip("Tốc độ zoom")]
    private float tocDoZoom = 2f;

    [SerializeField, Tooltip("Tốc độ chuyển động mềm của camera")]
    private float tocDoLerpCamera = 5f;

    [SerializeField, Tooltip("Tốc độ xoay mềm của camera")]
    private float tocDoLerpXoay = 10f;

    [SerializeField, Tooltip("Tự động focus vào nhân vật khi bắt đầu xoay")]
    private bool tuDongFocus = true;

    // Biến lưu trữ camera đã tạo
    private Camera? npcCamera;
    private Transform? cameraTransform;
    private bool dangHoatDong = false;

    // Biến quản lý xoay và zoom
    private float gocXoayNgang = 0f; // Góc xoay quanh trục Y
    private float gocXoayDoc = 15f;  // Góc xoay lên xuống
    private float khoangCachHienTai;

    // Biến quản lý input và focus
    private bool daDangKyInput = false;
    private bool dangXoay = false;
    private Vector3 viTriFocus;

    /// <summary>
    /// Khởi tạo camera khi component được thêm vào GameObject
    /// </summary>
    private void Awake()
    {
        // Khởi tạo khoảng cách ban đầu
        khoangCachHienTai = khoangCachBanDau;

        // Tạo GameObject mới cho camera
        GameObject cameraObject = new GameObject(gameObject.name + "_Camera");
        npcCamera = cameraObject.AddComponent<Camera>();
        
        // Cấu hình camera
        npcCamera.fieldOfView = 60f;
        npcCamera.nearClipPlane = 0.3f;
        npcCamera.farClipPlane = 1000f;
        npcCamera.depth = 0; // Đặt depth thấp hơn camera chính
        
        // Tắt camera mặc định
        npcCamera.enabled = kichHoatKhiStart;
        dangHoatDong = kichHoatKhiStart;
        
        // Lưu transform để dễ dàng điều chỉnh vị trí
        cameraTransform = npcCamera.transform;
        
        // Thêm audio listener nhưng tắt mặc định để tránh xung đột
        // AudioListener sẽ được quản lý bởi QuanLyCamera
        var audioListener = cameraObject.GetComponent<AudioListener>();
        if (audioListener == null)
        {
            audioListener = cameraObject.AddComponent<AudioListener>();
        }
        audioListener.enabled = false;

        // Đặt vị trí ban đầu cho camera
        CapNhatViTriCamera();
    }

    /// <summary>
    /// Cập nhật vị trí của camera theo NPC và xử lý input
    /// </summary>
    private void LateUpdate()
    {
        if (dangHoatDong && cameraTransform != null)
        {
            // Xử lý input chuột khi camera này đang hoạt động
            XuLyInputChuot();
            
            // Cập nhật vị trí camera
            CapNhatViTriCamera();
        }
    }

    /// <summary>
    /// Xử lý input chuột để xoay camera và zoom (chỉ khi giữ chuột phải để xoay)
    /// </summary>
    private void XuLyInputChuot()
    {
        if (Mouse.current == null) return;

        // Kiểm tra trạng thái chuột phải
        bool chuotPhaiPressed = Mouse.current.rightButton.isPressed;
        bool batDauXoay = Mouse.current.rightButton.wasPressedThisFrame;
        bool ketThucXoay = Mouse.current.rightButton.wasReleasedThisFrame;

        // Auto-focus khi bắt đầu xoay
        if (batDauXoay && tuDongFocus)
        {
            dangXoay = true;
            viTriFocus = transform.position + Vector3.up * doCaoCamera;
            Debug.Log("🎯 Auto-focus vào nhân vật - Bắt đầu orbital camera mode");
        }

        if (ketThucXoay)
        {
            dangXoay = false;
            Debug.Log("⭕ Kết thúc orbital camera mode");
        }

        // Xử lý xoay camera bằng chuột (chỉ khi giữ chuột phải)
        if (chuotPhaiPressed)
        {
            Vector2 deltaXoayChuot = Mouse.current.delta.ReadValue();

            // Kiểm tra tăng tốc xoay bằng Shift
            float tocDoXoayHienTai = tocDoXoayCamera;
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            {
                tocDoXoayHienTai *= nhanTocDoXoayNhanh;
                Debug.Log($"🚀 Boost tốc độ xoay: {tocDoXoayHienTai}°/s");
            }

            // Cải thiện xoay quanh nhân vật - tăng độ nhạy và precision
            float multiplierXoay = dangXoay ? 0.035f : 0.025f; // Tăng khi đang focus
            gocXoayNgang += deltaXoayChuot.x * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * multiplierXoay;
            gocXoayDoc -= deltaXoayChuot.y * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * multiplierXoay;

            // Giới hạn góc xoay lên xuống (mở rộng range để dễ xoay hơn)
            gocXoayDoc = Mathf.Clamp(gocXoayDoc, -89f, 89f); // Mở rộng từ -85f đến -89f
        }

        // Xử lý zoom bằng scroll chuột (luôn hoạt động)
        float scrollInput = Mouse.current.scroll.ReadValue().y;
        if (scrollInput != 0f)
        {
            khoangCachHienTai -= scrollInput * tocDoZoom * Time.deltaTime;
            khoangCachHienTai = Mathf.Clamp(khoangCachHienTai, khoangCachToiThieu, khoangCachToiDa);
        }
    }

    /// <summary>
    /// Cập nhật vị trí và xoay của camera
    /// </summary>
    private void CapNhatViTriCamera()
    {
        if (cameraTransform == null) return;

        // Tính toán vị trí camera dựa trên góc xoay và khoảng cách
        Vector3 huongCamera = new Vector3(
            Mathf.Sin(gocXoayNgang * Mathf.Deg2Rad) * Mathf.Cos(gocXoayDoc * Mathf.Deg2Rad),
            Mathf.Sin(gocXoayDoc * Mathf.Deg2Rad),
            Mathf.Cos(gocXoayNgang * Mathf.Deg2Rad) * Mathf.Cos(gocXoayDoc * Mathf.Deg2Rad)
        );

        // Chọn điểm focus: dùng viTriFocus khi đang xoay, hoặc vị trí NPC hiện tại
        Vector3 diemFocus;
        if (dangXoay && tuDongFocus)
        {
            diemFocus = viTriFocus; // Giữ nguyên focus point khi đang xoay
        }
        else
        {
            diemFocus = transform.position + Vector3.up * doCaoCamera;
            viTriFocus = diemFocus; // Cập nhật focus point khi không xoay
        }

        // Vị trí mục tiêu camera
        Vector3 viTriMoi = diemFocus + huongCamera * khoangCachHienTai;

        // Di chuyển camera mềm mại đến vị trí mới (nhanh hơn khi đang xoay)
        float tocDoLerpHienTai = dangXoay ? tocDoLerpCamera * 3f : tocDoLerpCamera; // Tăng từ 2f lên 3f
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, viTriMoi, Time.deltaTime * tocDoLerpHienTai);
        
        // Xoay camera nhìn về phía điểm focus (mượt mà hơn khi orbital)
        Vector3 huongNhin = (diemFocus - cameraTransform.position).normalized;
        Quaternion xoayMoi = Quaternion.LookRotation(huongNhin);
        float tocDoXoayLerpHienTai = dangXoay ? tocDoLerpXoay * 2.5f : tocDoLerpXoay; // Tăng từ 1.5f lên 2.5f
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, xoayMoi, Time.deltaTime * tocDoXoayLerpHienTai);
    }

    /// <summary>
    /// Kích hoạt hoặc tắt camera
    /// AudioListener sẽ được quản lý bởi QuanLyCamera
    /// </summary>
    /// <param name="kichHoat">Trạng thái kích hoạt (true = bật, false = tắt)</param>
    public void BatTatCamera(bool kichHoat)
    {
        if (npcCamera != null)
        {
            npcCamera.enabled = kichHoat;
            dangHoatDong = kichHoat;
            
            // Nếu camera được kích hoạt, đặt lại vị trí ban đầu
            if (kichHoat)
            {
                DatLaiViTriBanDau();
            }
            
            // AudioListener sẽ được quản lý bởi QuanLyCamera, không cần xử lý ở đây
        }
    }

    /// <summary>
    /// Đặt lại vị trí ban đầu của camera
    /// </summary>
    public void DatLaiViTriBanDau()
    {
        gocXoayNgang = 0f;
        gocXoayDoc = 15f;
        khoangCachHienTai = khoangCachBanDau;
        
        if (cameraTransform != null)
        {
            CapNhatViTriCamera();
        }
    }

    /// <summary>
    /// Đặt góc xoay của camera
    /// </summary>
    /// <param name="gocNgang">Góc xoay ngang (Y)</param>
    /// <param name="gocDoc">Góc xoay dọc (X)</param>
    public void DatGocXoay(float gocNgang, float gocDoc)
    {
        gocXoayNgang = gocNgang;
        gocXoayDoc = Mathf.Clamp(gocDoc, -80f, 80f);
    }

    /// <summary>
    /// Đặt khoảng cách camera
    /// </summary>
    /// <param name="khoangCach">Khoảng cách mới</param>
    public void DatKhoangCach(float khoangCach)
    {
        khoangCachHienTai = Mathf.Clamp(khoangCach, khoangCachToiThieu, khoangCachToiDa);
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
    /// Đặt độ nhạy chuột
    /// </summary>
    /// <param name="doNhayMoi">Độ nhạy mới</param>
    public void DatDoNhayChuot(float doNhayMoi)
    {
        doNhayChuot = Mathf.Max(0f, doNhayMoi);
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
    /// Bật/tắt auto-focus khi xoay
    /// </summary>
    /// <param name="batTat">True để bật, false để tắt</param>
    public void BatTatAutoFocus(bool batTat)
    {
        tuDongFocus = batTat;
    }

    /// <summary>
    /// Kiểm tra trạng thái auto-focus
    /// </summary>
    /// <returns>True nếu auto-focus đang bật</returns>
    public bool KiemTraAutoFocus()
    {
        return tuDongFocus;
    }

    /// <summary>
    /// Lấy component camera
    /// </summary>
    /// <returns>Camera component</returns>
    public Camera? GetCamera()
    {
        return npcCamera;
    }

    /// <summary>
    /// Lấy tốc độ xoay hiện tại
    /// </summary>
    public float LayTocDoXoay()
    {
        return tocDoXoayCamera;
    }

    /// <summary>
    /// Lấy nhân tốc độ xoay nhanh
    /// </summary>
    public float LayNhanTocDoXoayNhanh()
    {
        return nhanTocDoXoayNhanh;
    }

    /// <summary>
    /// Lấy độ nhạy chuột
    /// </summary>
    public float LayDoNhayChuot()
    {
        return doNhayChuot;
    }

    /// <summary>
    /// Lấy khoảng cách hiện tại
    /// </summary>
    public float LayKhoangCach()
    {
        return khoangCachHienTai;
    }

    /// <summary>
    /// Xóa camera khi component bị destroy
    /// </summary>
    private void OnDestroy()
    {
        if (npcCamera != null)
        {
            Destroy(npcCamera.gameObject);
        }
    }
}