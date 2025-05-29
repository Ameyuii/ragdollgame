using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quản lý camera trong scene, cho phép chuyển đổi giữa camera chính và các camera của NPC
/// Sử dụng InputSystem mới để xử lý input từ người dùng
/// Bao gồm quản lý AudioListener để đảm bảo chỉ có 1 AudioListener hoạt động tại một thời điểm
/// </summary>
public class QuanLyCamera : MonoBehaviour
{
    [Header("Cấu hình Camera")]
    [SerializeField, Tooltip("Camera chính của scene")]
    private Camera? cameraChinh;
    
    [SerializeField, Tooltip("Tự động tìm tất cả NPC có camera")]
    private bool tuDongTimNPCCamera = true;
    
    [SerializeField, Tooltip("Danh sách NPC có camera (nếu không tự động tìm)")]
    private List<NPCCamera> danhSachCameraNPC = new List<NPCCamera>();
    
    [Header("Cấu hình AudioListener")]
    [SerializeField, Tooltip("Tự động quản lý AudioListener để tránh xung đột")]
    private bool tuDongQuanLyAudioListener = true;
    
    [SerializeField, Tooltip("Hiển thị debug AudioListener")]
    private bool hienThiDebugAudioListener = true;
    
    // Biến để quản lý camera hiện tại
    private int chiSoCameraHienTai = -1; // -1 = camera chính, 0+ = camera NPC
    
    // Danh sách chứa tất cả camera tìm thấy
    private List<Camera> tatCaCamera = new List<Camera>();
    private Dictionary<Camera, NPCCamera> mapCameraToNPC = new Dictionary<Camera, NPCCamera>();
    
    // Quản lý AudioListener
    private Dictionary<Camera, AudioListener> audioListenerMap = new Dictionary<Camera, AudioListener>();
    private AudioListener? audioListenerChinh;
    
    /// <summary>
    /// Khởi tạo component và tìm các camera
    /// </summary>
    private void Awake()
    {
        // Nếu không chỉ định camera chính, lấy camera tag MainCamera
        if (cameraChinh == null)
        {
            cameraChinh = Camera.main;
            if (cameraChinh == null)
            {
                Debug.LogError("Không tìm thấy camera chính! Vui lòng gán camera chính vào QuanLyCamera hoặc đặt tag MainCamera cho camera chính.");
                return;
            }
        }

        // Thêm camera chính vào danh sách
        if (cameraChinh != null)
        {
            tatCaCamera.Add(cameraChinh);
        }
        
        // Khởi tạo hệ thống AudioListener
        if (tuDongQuanLyAudioListener)
        {
            KhoiTaoAudioListenerSystem();
        }
    }

    /// <summary>
    /// Cấu hình các camera và input khi bắt đầu
    /// </summary>
    private void Start()
    {
        // Tự động thêm CameraController cho camera chính nếu chưa có
        if (cameraChinh != null)
        {
            var controller = cameraChinh.GetComponent("CameraController");
            if (controller == null)
            {
                var controllerType = System.Type.GetType("CameraController");
                if (controllerType != null)
                {
                    cameraChinh.gameObject.AddComponent(controllerType);
                    Debug.Log("Đã tự động thêm CameraController cho camera chính.");
                }
            }
        }

        // Nếu tự động tìm các camera NPC
        if (tuDongTimNPCCamera)
        {
            TimTatCaNPCCamera();
        }
        else if (danhSachCameraNPC.Count > 0)
        {
            foreach (NPCCamera npcCamera in danhSachCameraNPC)
            {
                if (npcCamera != null && npcCamera.GetCamera() != null)
                {
                    ThemCameraNPC(npcCamera);
                }
            }
        }
        
        // Đảm bảo chỉ bật camera chính và AudioListener chính
        BatCameraChinh();
        
        // Kiểm tra tình trạng AudioListener ban đầu
        if (tuDongQuanLyAudioListener)
        {
            KiemTraTinhTrangAudioListener();
        }
    }
    
    /// <summary>
    /// Xử lý input trong mỗi frame
    /// </summary>
    private void Update()
    {
        // Kiểm tra input để chuyển camera
        if (Keyboard.current != null)
        {
            // Phím 0 để chuyển về camera chính
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                BatCameraChinh();
            }
            
            // Phím 1 để chuyển sang camera NPC tiếp theo
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChuyenCameraKeTiep();
            }
        }
    }
    
    /// <summary>
    /// Tự động tìm tất cả NPC có component NPCCamera
    /// </summary>
    private void TimTatCaNPCCamera()
    {
        // Tìm tất cả NPCCamera trong scene
        NPCCamera[] allNPCCameras = FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
        foreach (NPCCamera npcCamera in allNPCCameras)
        {
            if (npcCamera != null && npcCamera.GetCamera() != null)
            {
                ThemCameraNPC(npcCamera);
            }
        }
        
        Debug.Log($"Đã tìm thấy {tatCaCamera.Count - 1} camera NPC.");
    }
    
    /// <summary>
    /// Thêm camera NPC vào danh sách quản lý
    /// </summary>
    /// <param name="npcCamera">Component NPCCamera</param>
    public void ThemCameraNPC(NPCCamera npcCamera)
    {
        if (npcCamera == null) return;
        
        Camera? camera = npcCamera.GetCamera();
        if (camera == null) return;
        
        // Kiểm tra xem camera đã tồn tại trong danh sách chưa
        if (!tatCaCamera.Contains(camera))
        {
            tatCaCamera.Add(camera);
            mapCameraToNPC[camera] = npcCamera;
            
            // Đăng ký AudioListener cho camera NPC
            if (tuDongQuanLyAudioListener)
            {
                DangKyAudioListenerChoNPCCamera(camera);
            }
            
            // Tắt camera mặc định
            npcCamera.BatTatCamera(false);
        }
    }
    
    // Các phương thức xử lý input được cập nhật trong Update()
    
    /// <summary>
    /// Bật camera chính và tắt tất cả camera khác
    /// </summary>
    public void BatCameraChinh()
    {
        if (cameraChinh == null) return;
        
        // Tắt tất cả camera
        TatTatCaCamera();
        
        // Bật camera chính
        cameraChinh.enabled = true;
        
        // Bật CameraController cho camera chính
        var cameraController = cameraChinh.GetComponent("CameraController") as MonoBehaviour;
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
        
        // Kích hoạt AudioListener chính
        if (tuDongQuanLyAudioListener)
        {
            KichHoatAudioListenerChinh();
        }
        
        // Cập nhật chỉ số camera hiện tại
        chiSoCameraHienTai = -1;
        
        Debug.Log("Đã chuyển về camera chính");
    }
    
    /// <summary>
    /// Chuyển sang camera kế tiếp
    /// </summary>
    public void ChuyenCameraKeTiep()
    {
        // Nếu không có camera NPC nào
        if (tatCaCamera.Count <= 1)
        {
            Debug.Log("Không tìm thấy camera NPC nào để chuyển!");
            return;
        }
        
        // Tăng chỉ số camera hiện tại
        chiSoCameraHienTai++;
        
        // Nếu vượt quá số lượng camera, reset về camera đầu tiên trong danh sách NPC
        if (chiSoCameraHienTai >= tatCaCamera.Count - 1)
        {
            chiSoCameraHienTai = 0;
        }
        
        // Kích hoạt camera tương ứng
        ChuyenSangCamera(chiSoCameraHienTai);
        
        Debug.Log($"Đã chuyển sang camera NPC thứ {chiSoCameraHienTai + 1}");
    }
    
    /// <summary>
    /// Chuyển đến camera với chỉ số cụ thể
    /// </summary>
    /// <param name="chiSoCamera">Chỉ số camera (0 để bắt đầu)</param>
    public void ChuyenSangCamera(int chiSoCamera)
    {
        // Nếu chỉ số không hợp lệ
        if (chiSoCamera < 0 || chiSoCamera >= tatCaCamera.Count - 1)
        {
            Debug.LogWarning($"Chỉ số camera không hợp lệ: {chiSoCamera}");
            return;
        }
        
        // Tắt tất cả camera
        TatTatCaCamera();
        
        // Lấy camera thực tế (bỏ qua camera chính ở vị trí 0)
        Camera targetCamera = tatCaCamera[chiSoCamera + 1]; // +1 vì camera chính ở vị trí 0
        
        // Bật camera mục tiêu
        if (mapCameraToNPC.TryGetValue(targetCamera, out NPCCamera npcCamera))
        {
            npcCamera.BatTatCamera(true);
        }
        else
        {
            // Trường hợp camera không liên kết với NPCCamera
            targetCamera.enabled = true;
        }
        
        // Kích hoạt AudioListener cho camera mục tiêu
        if (tuDongQuanLyAudioListener)
        {
            KichHoatAudioListenerChoCamera(targetCamera);
        }
        
        // Cập nhật chỉ số camera hiện tại
        chiSoCameraHienTai = chiSoCamera;
    }
    
    /// <summary>
    /// Tắt tất cả camera
    /// </summary>
    private void TatTatCaCamera()
    {
        // Tắt camera chính
        if (cameraChinh != null)
        {
            cameraChinh.enabled = false;
            
            // Tắt CameraController của camera chính
            var cameraController = cameraChinh.GetComponent("CameraController") as MonoBehaviour;
            if (cameraController != null)
            {
                cameraController.enabled = false;
            }
        }
        
        // Tắt tất cả các camera NPC
        foreach (KeyValuePair<Camera, NPCCamera> entry in mapCameraToNPC)
        {
            entry.Value.BatTatCamera(false);
        }
    }
    
    /// <summary>
    /// Xóa camera NPC khỏi danh sách quản lý
    /// </summary>
    /// <param name="npcCamera">Component NPCCamera</param>
    public void XoaCameraNPC(NPCCamera npcCamera)
    {
        if (npcCamera == null) return;
        
        Camera? camera = npcCamera.GetCamera();
        if (camera == null) return;
        
        // Nếu camera hiện tại đang là camera bị xóa, chuyển về camera chính
        if (chiSoCameraHienTai >= 0 && tatCaCamera.Count > 1)
        {
            if (tatCaCamera[chiSoCameraHienTai + 1] == camera)
            {
                BatCameraChinh();
            }
        }
        
        // Xóa khỏi danh sách
        if (tatCaCamera.Contains(camera))
        {
            tatCaCamera.Remove(camera);
        }
        
        if (mapCameraToNPC.ContainsKey(camera))
        {
            mapCameraToNPC.Remove(camera);
        }
    }
    
    // =================== QUẢN LÝ AUDIOLISTENER ===================
    
    /// <summary>
    /// Khởi tạo hệ thống quản lý AudioListener
    /// </summary>
    private void KhoiTaoAudioListenerSystem()
    {
        // Tìm AudioListener chính
        if (cameraChinh != null)
        {
            audioListenerChinh = cameraChinh.GetComponent<AudioListener>();
            if (audioListenerChinh == null)
            {
                audioListenerChinh = cameraChinh.gameObject.AddComponent<AudioListener>();
                if (hienThiDebugAudioListener)
                {
                    Debug.Log("Đã tạo AudioListener cho camera chính.");
                }
            }
            audioListenerMap[cameraChinh] = audioListenerChinh;
        }
        
        // Tắt tất cả AudioListener khác trong scene để tránh xung đột
        TatTatCaAudioListenerKhac();
        
        if (hienThiDebugAudioListener)
        {
            Debug.Log("Đã khởi tạo hệ thống quản lý AudioListener.");
        }
    }
    
    /// <summary>
    /// Tắt tất cả AudioListener khác ngoài AudioListener chính
    /// </summary>
    private void TatTatCaAudioListenerKhac()
    {
        AudioListener[] allAudioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int soLuongTat = 0;
        
        foreach (AudioListener audioListener in allAudioListeners)
        {
            if (audioListener != audioListenerChinh && audioListener.enabled)
            {
                audioListener.enabled = false;
                soLuongTat++;
            }
        }
        
        if (hienThiDebugAudioListener && soLuongTat > 0)
        {
            Debug.Log($"Đã tắt {soLuongTat} AudioListener khác để tránh xung đột.");
        }
    }
    
    /// <summary>
    /// Đăng ký AudioListener cho camera NPC
    /// </summary>
    /// <param name="camera">Camera NPC</param>
    private void DangKyAudioListenerChoNPCCamera(Camera camera)
    {
        if (camera == null) return;
        
        AudioListener audioListener = camera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListenerMap[camera] = audioListener;
            // Tắt AudioListener mặc định
            audioListener.enabled = false;
            
            if (hienThiDebugAudioListener)
            {
                Debug.Log($"Đã đăng ký AudioListener cho camera NPC: {camera.name}");
            }
        }
    }
    
    /// <summary>
    /// Kích hoạt AudioListener cho camera cụ thể
    /// </summary>
    /// <param name="camera">Camera cần kích hoạt AudioListener</param>
    private void KichHoatAudioListenerChoCamera(Camera camera)
    {
        if (!tuDongQuanLyAudioListener || camera == null) return;
        
        // Tắt tất cả AudioListener
        foreach (var kvp in audioListenerMap)
        {
            if (kvp.Value != null)
            {
                kvp.Value.enabled = false;
            }
        }
        
        // Kích hoạt AudioListener cho camera hiện tại
        if (audioListenerMap.TryGetValue(camera, out AudioListener? audioListener) && audioListener != null)
        {
            audioListener.enabled = true;
            
            if (hienThiDebugAudioListener)
            {
                Debug.Log($"Đã kích hoạt AudioListener cho camera: {camera.name}");
            }
        }
        else
        {
            // Nếu không tìm thấy AudioListener, kích hoạt AudioListener chính
            KichHoatAudioListenerChinh();
        }
        
        // Kiểm tra và báo cáo tình trạng AudioListener
        KiemTraTinhTrangAudioListener();
    }
    
    /// <summary>
    /// Kích hoạt AudioListener chính
    /// </summary>
    private void KichHoatAudioListenerChinh()
    {
        if (!tuDongQuanLyAudioListener) return;
        
        // Tắt tất cả AudioListener khác
        foreach (var kvp in audioListenerMap)
        {
            if (kvp.Value != null)
            {
                kvp.Value.enabled = false;
            }
        }
        
        // Kích hoạt AudioListener chính
        if (audioListenerChinh != null)
        {
            audioListenerChinh.enabled = true;
            
            if (hienThiDebugAudioListener)
            {
                Debug.Log("Đã kích hoạt AudioListener chính.");
            }
        }
    }
    
    /// <summary>
    /// Kiểm tra tình trạng AudioListener và sửa lỗi nếu cần
    /// </summary>
    private void KiemTraTinhTrangAudioListener()
    {
        if (!tuDongQuanLyAudioListener) return;
        
        AudioListener[] allAudioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int soLuongDangBat = 0;
        AudioListener? audioListenerDangBat = null;
        
        foreach (AudioListener audioListener in allAudioListeners)
        {
            if (audioListener.enabled)
            {
                soLuongDangBat++;
                audioListenerDangBat = audioListener;
            }
        }
        
        if (soLuongDangBat > 1)
        {
            Debug.LogWarning($"Phát hiện {soLuongDangBat} AudioListener đang hoạt động! Tự động sửa lỗi...");
            
            // Tắt tất cả AudioListener
            foreach (AudioListener listener in allAudioListeners)
            {
                listener.enabled = false;
            }
            
            // Chỉ bật AudioListener của camera hiện tại
            Camera? cameraHienTai = LayCameraHienTai();
            if (cameraHienTai != null && audioListenerMap.TryGetValue(cameraHienTai, out AudioListener? audioListener) && audioListener != null)
            {
                audioListener.enabled = true;
            }
            else if (audioListenerChinh != null)
            {
                audioListenerChinh.enabled = true;
            }
            
            Debug.Log("Đã sửa lỗi AudioListener trùng lặp.");
        }
        else if (soLuongDangBat == 0)
        {
            Debug.LogWarning("Không có AudioListener nào đang hoạt động! Kích hoạt AudioListener chính...");
            KichHoatAudioListenerChinh();
        }
    }
    
    /// <summary>
    /// Lấy camera đang hoạt động hiện tại
    /// </summary>
    /// <returns>Camera đang hoạt động</returns>
    private Camera? LayCameraHienTai()
    {
        if (chiSoCameraHienTai == -1)
        {
            return cameraChinh;
        }
        else if (chiSoCameraHienTai >= 0 && chiSoCameraHienTai < tatCaCamera.Count - 1)
        {
            return tatCaCamera[chiSoCameraHienTai + 1]; // +1 vì camera chính ở vị trí 0
        }
        return null;
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Debug method để kiểm tra tình trạng AudioListener (chỉ trong Editor)
    /// </summary>
    [ContextMenu("Kiểm tra tình trạng AudioListener")]
    private void DebugAudioListenerStatus()
    {
        Debug.Log("=== TÌNH TRẠNG AUDIOLISTENER ===");
        Debug.Log($"Tự động quản lý AudioListener: {tuDongQuanLyAudioListener}");
        Debug.Log($"AudioListener chính: {(audioListenerChinh != null ? audioListenerChinh.name : "Null")}");
        Debug.Log($"Camera hiện tại: {(LayCameraHienTai() != null ? LayCameraHienTai()?.name : "Null")}");
        Debug.Log($"Chỉ số camera hiện tại: {chiSoCameraHienTai}");
        
        foreach (var kvp in audioListenerMap)
        {
            Debug.Log($"- Camera: {kvp.Key.name}, AudioListener: {(kvp.Value != null ? kvp.Value.name : "Null")}, Enabled: {(kvp.Value != null ? kvp.Value.enabled : false)}");
        }
        
        AudioListener[] allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int enabledCount = 0;
        foreach (AudioListener listener in allListeners)
        {
            if (listener.enabled) enabledCount++;
        }
        Debug.Log($"Tổng số AudioListener trong scene: {allListeners.Length}, Đang bật: {enabledCount}");
    }
    #endif
}