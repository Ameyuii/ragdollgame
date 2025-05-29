using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý AudioListener để đảm bảo chỉ có 1 AudioListener hoạt động tại một thời điểm
/// AudioListener sẽ luôn được gắn với camera đang hoạt động gần nhất
/// </summary>
public class AudioListenerManager : MonoBehaviour
{
    [Header("Cấu hình AudioListener")]
    [SerializeField, Tooltip("AudioListener chính của scene (thường gắn với Main Camera)")]
    private AudioListener? audioListenerChinh;
    
    [Header("Debug")]
    [SerializeField, Tooltip("Hiển thị thông tin debug")]
    private bool hienThiDebug = true;
    
    // Singleton pattern để đảm bảo chỉ có 1 instance
    private static AudioListenerManager? instance;
    public static AudioListenerManager? Instance => instance;
    
    // Lưu trữ tất cả AudioListener trong scene
    private readonly Dictionary<Camera, AudioListener> audioListenerMap = new Dictionary<Camera, AudioListener>();
    private AudioListener? audioListenerHienTai;
    
    /// <summary>
    /// Khởi tạo singleton và tìm AudioListener chính
    /// </summary>
    private void Awake()
    {
        // Đảm bảo chỉ có 1 instance
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Đã có AudioListenerManager khác tồn tại. Xóa instance mới.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // Tìm AudioListener chính nếu chưa được gán
        if (audioListenerChinh == null)
        {
            Camera? cameraChinh = Camera.main;
            if (cameraChinh != null)
            {
                audioListenerChinh = cameraChinh.GetComponent<AudioListener>();
            }
            
            // Nếu vẫn không tìm thấy, tìm AudioListener đầu tiên trong scene
            if (audioListenerChinh == null)
            {
                audioListenerChinh = FindFirstObjectByType<AudioListener>();
            }
        }
        
        if (hienThiDebug)
        {
            Debug.Log($"AudioListenerManager khởi tạo. AudioListener chính: {(audioListenerChinh != null ? audioListenerChinh.name : "Không tìm thấy")}");
        }
    }
    
    /// <summary>
    /// Khởi tạo hệ thống AudioListener
    /// </summary>
    private void Start()
    {
        // Tìm và đăng ký tất cả AudioListener trong scene
        TimVaDangKyTatCaAudioListener();
        
        // Kích hoạt AudioListener chính ban đầu
        if (audioListenerChinh != null)
        {
            KichHoatAudioListener(audioListenerChinh);
        }
    }
    
    /// <summary>
    /// Tìm và đăng ký tất cả AudioListener trong scene
    /// </summary>
    private void TimVaDangKyTatCaAudioListener()
    {
        // Xóa map cũ
        audioListenerMap.Clear();
        
        // Tìm tất cả AudioListener
        AudioListener[] allAudioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        
        foreach (AudioListener audioListener in allAudioListeners)
        {
            Camera? camera = audioListener.GetComponent<Camera>();
            if (camera != null)
            {
                audioListenerMap[camera] = audioListener;
                
                // Tắt tất cả AudioListener ban đầu
                audioListener.enabled = false;
                
                if (hienThiDebug)
                {
                    Debug.Log($"Đã đăng ký AudioListener cho camera: {camera.name}");
                }
            }
        }
        
        if (hienThiDebug)
        {
            Debug.Log($"Đã tìm thấy và đăng ký {audioListenerMap.Count} AudioListener.");
        }
    }
    
    /// <summary>
    /// Kích hoạt AudioListener cho camera cụ thể và tắt tất cả AudioListener khác
    /// </summary>
    /// <param name="camera">Camera cần kích hoạt AudioListener</param>
    public void KichHoatAudioListenerChoCamera(Camera camera)
    {
        if (camera == null)
        {
            Debug.LogWarning("Camera null! Không thể kích hoạt AudioListener.");
            return;
        }
        
        // Tắt tất cả AudioListener
        TatTatCaAudioListener();
        
        // Tìm AudioListener cho camera này
        if (audioListenerMap.TryGetValue(camera, out AudioListener audioListener))
        {
            KichHoatAudioListener(audioListener);
        }
        else
        {
            // Nếu camera chưa có AudioListener, tạo mới
            AudioListener newAudioListener = camera.GetComponent<AudioListener>();
            if (newAudioListener == null)
            {
                newAudioListener = camera.gameObject.AddComponent<AudioListener>();
                if (hienThiDebug)
                {
                    Debug.Log($"Đã tạo AudioListener mới cho camera: {camera.name}");
                }
            }
            
            // Đăng ký vào map
            audioListenerMap[camera] = newAudioListener;
            KichHoatAudioListener(newAudioListener);
        }
    }
    
    /// <summary>
    /// Kích hoạt AudioListener cụ thể
    /// </summary>
    /// <param name="audioListener">AudioListener cần kích hoạt</param>
    private void KichHoatAudioListener(AudioListener audioListener)
    {
        if (audioListener == null) return;
        
        // Tắt AudioListener hiện tại nếu có
        if (audioListenerHienTai != null && audioListenerHienTai != audioListener)
        {
            audioListenerHienTai.enabled = false;
        }
        
        // Kích hoạt AudioListener mới
        audioListener.enabled = true;
        audioListenerHienTai = audioListener;
        
        if (hienThiDebug)
        {
            Debug.Log($"Đã kích hoạt AudioListener: {audioListener.name}");
        }
    }
    
    /// <summary>
    /// Tắt tất cả AudioListener trong scene
    /// </summary>
    private void TatTatCaAudioListener()
    {
        foreach (AudioListener audioListener in audioListenerMap.Values)
        {
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
        
        // Tắt cả AudioListener chính nếu nó không trong map
        if (audioListenerChinh != null && !audioListenerMap.ContainsValue(audioListenerChinh))
        {
            audioListenerChinh.enabled = false;
        }
        
        audioListenerHienTai = null;
    }
    
    /// <summary>
    /// Kích hoạt AudioListener chính
    /// </summary>
    public void KichHoatAudioListenerChinh()
    {
        if (audioListenerChinh != null)
        {
            TatTatCaAudioListener();
            KichHoatAudioListener(audioListenerChinh);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioListener chính!");
        }
    }
    
    /// <summary>
    /// Đăng ký AudioListener mới cho camera
    /// </summary>
    /// <param name="camera">Camera</param>
    /// <param name="audioListener">AudioListener</param>
    public void DangKyAudioListener(Camera camera, AudioListener audioListener)
    {
        if (camera == null || audioListener == null) return;
        
        audioListenerMap[camera] = audioListener;
        
        // Tắt AudioListener mới đăng ký để tránh xung đột
        audioListener.enabled = false;
        
        if (hienThiDebug)
        {
            Debug.Log($"Đã đăng ký AudioListener mới cho camera: {camera.name}");
        }
    }
    
    /// <summary>
    /// Hủy đăng ký AudioListener
    /// </summary>
    /// <param name="camera">Camera</param>
    public void HuyDangKyAudioListener(Camera camera)
    {
        if (camera == null) return;
        
        if (audioListenerMap.TryGetValue(camera, out AudioListener audioListener))
        {
            // Nếu AudioListener đang được kích hoạt, chuyển về AudioListener chính
            if (audioListenerHienTai == audioListener)
            {
                KichHoatAudioListenerChinh();
            }
            
            audioListenerMap.Remove(camera);
            
            if (hienThiDebug)
            {
                Debug.Log($"Đã hủy đăng ký AudioListener cho camera: {camera.name}");
            }
        }
    }
    
    /// <summary>
    /// Kiểm tra và sửa lỗi AudioListener trùng lặp
    /// </summary>
    public void KiemTraVaSuaLoiAudioListener()
    {
        // Tìm tất cả AudioListener đang bật
        AudioListener[] allActiveListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int soLuongDangBat = 0;
        
        foreach (AudioListener listener in allActiveListeners)
        {
            if (listener.enabled)
            {
                soLuongDangBat++;
            }
        }
        
        if (soLuongDangBat > 1)
        {
            Debug.LogWarning($"Phát hiện {soLuongDangBat} AudioListener đang hoạt động! Sửa lỗi...");
            
            // Tắt tất cả và chỉ bật AudioListener hiện tại
            foreach (AudioListener listener in allActiveListeners)
            {
                listener.enabled = false;
            }
            
            if (audioListenerHienTai != null)
            {
                audioListenerHienTai.enabled = true;
            }
            else if (audioListenerChinh != null)
            {
                audioListenerChinh.enabled = true;
                audioListenerHienTai = audioListenerChinh;
            }
            
            Debug.Log("Đã sửa lỗi AudioListener trùng lặp.");
        }
        else if (soLuongDangBat == 0)
        {
            Debug.LogWarning("Không có AudioListener nào đang hoạt động! Kích hoạt AudioListener chính...");
            KichHoatAudioListenerChinh();
        }
        else
        {
            if (hienThiDebug)
            {
                Debug.Log("AudioListener hoạt động bình thường.");
            }
        }
    }
    
    /// <summary>
    /// Cleanup khi object bị destroy
    /// </summary>
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Debug method để kiểm tra tình trạng AudioListener (chỉ trong Editor)
    /// </summary>
    [ContextMenu("Kiểm tra tình trạng AudioListener")]
    private void DebugAudioListenerStatus()
    {
        Debug.Log("=== TÌNH TRẠNG AUDIOLISTENER ===");
        Debug.Log($"AudioListener chính: {(audioListenerChinh != null ? audioListenerChinh.name : "Null")}");
        Debug.Log($"AudioListener hiện tại: {(audioListenerHienTai != null ? audioListenerHienTai.name : "Null")}");
        Debug.Log($"Số lượng AudioListener đã đăng ký: {audioListenerMap.Count}");
        
        foreach (var kvp in audioListenerMap)
        {
            Debug.Log($"- Camera: {kvp.Key.name}, AudioListener: {kvp.Value.name}, Enabled: {kvp.Value.enabled}");
        }
        
        // Kiểm tra tổng số AudioListener trong scene
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
