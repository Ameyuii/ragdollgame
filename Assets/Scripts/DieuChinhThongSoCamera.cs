using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Điều Chỉnh Thông Số Camera - Hệ thống điều chỉnh camera runtime
/// Cung cấp giao diện để điều chỉnh thông số camera chính và tất cả NPC cameras
/// Hỗ trợ shared parameters system cho tất cả NPC cameras
/// </summary>
public class DieuChinhThongSoCamera : MonoBehaviour
{    [Header("Cài đặt Panel")]
    [SerializeField, Tooltip("Hiển thị panel ngay khi start")]
    private bool autoShowOnStart = false;

    [SerializeField, Tooltip("Kích thước icon toggle")]
    private float iconSize = 60f;

    [SerializeField, Tooltip("Vị trí icon (góc màn hình)")]
    private Vector2 iconPosition = new Vector2(10, 10); // Sẽ được set trong Start()

    // UI State
    private bool hienThiPanel = true;
    private bool draggingPanel = false;
    private Vector2 panelPosition = new Vector2(50, 50);
    private Vector2 panelSize = new Vector2(450, 700);
    private Vector2 dragOffset;

    // Camera references
    private QuanLyCamera? quanLyCamera;
    private Camera? cameraChinh;
    private CameraController? cameraController;
    private NPCCamera? npcCameraHienTai;
    private List<NPCCamera> tatCaNPCCameras = new List<NPCCamera>();

    // Runtime adjustable parameters - Camera Chính
    private float runtimeTocDoXoayCamera = 150f;
    private float runtimeNhanTocDoXoay = 2.5f;
    private float runtimeDoNhayChuot = 3f;
    private float runtimeTocDoChuyenDong = 10f;
    private float runtimeTocDoChuyenDongNhanh = 20f;

    // Runtime adjustable parameters - SHARED cho TẤT CẢ NPC Cameras
    private float sharedNPCTocDoXoay = 150f;
    private float sharedNPCNhanTocDoXoay = 2.5f;
    private float sharedNPCDoNhayChuot = 3f;
    private float sharedNPCKhoangCach = 5f;

    // GUI Styles
    private GUIStyle? iconStyle;
    private GUIStyle? titleStyle;
    private GUIStyle? buttonStyle;

    // Save/Load functionality
    [Header("Save/Load Settings")]
    [SerializeField, Tooltip("Tự động load settings khi start")]
    private bool autoLoadOnStart = true;
    
    [SerializeField, Tooltip("Tự động save khi thay đổi")]
    private bool autoSaveOnChange = false;

    // Keys for PlayerPrefs
    private const string PREF_MAIN_XOAY = "CameraSetting_MainXoay";
    private const string PREF_MAIN_NHAN = "CameraSetting_MainNhan";
    private const string PREF_MAIN_NHAY = "CameraSetting_MainNhay";
    private const string PREF_MAIN_TOCDO = "CameraSetting_MainTocDo";
    private const string PREF_MAIN_TOCDO_NHANH = "CameraSetting_MainTocDoNhanh";
    
    private const string PREF_NPC_XOAY = "CameraSetting_NPCXoay";
    private const string PREF_NPC_NHAN = "CameraSetting_NPCNhan";
    private const string PREF_NPC_NHAY = "CameraSetting_NPCNhay";
    private const string PREF_NPC_KHOANGCACH = "CameraSetting_NPCKhoangCach";    /// <summary>
    /// Khởi tạo hệ thống và UI
    /// </summary>
    private void Start()
    {
        InitializeCameraSystem();        
        // Load saved settings if enabled
        if (autoLoadOnStart)
        {
            LoadSavedSettings();
        }
        
        // Auto show panel if enabled
        hienThiPanel = autoShowOnStart;
        
        // Initialize UI positions
        iconPosition = new Vector2(Screen.width - 80, 10); // Góc phải trên
        panelPosition = new Vector2(
            (Screen.width - panelSize.x) * 0.5f,
            (Screen.height - panelSize.y) * 0.5f
        );
    }

    /// <summary>
    /// Tìm và khởi tạo tham chiếu camera
    /// </summary>
    private void InitializeCameraSystem()
    {
        // Tìm QuanLyCamera trong scene
        quanLyCamera = FindFirstObjectByType<QuanLyCamera>();
        if (quanLyCamera == null)
        {
            Debug.LogError("❌ Không tìm thấy QuanLyCamera trong scene!");
            return;
        }

        // Tìm camera chính
        cameraChinh = Camera.main;
        if (cameraChinh == null)
        {
            Debug.LogError("❌ Không tìm thấy camera chính!");
            return;
        }

        // Tìm CameraController
        cameraController = cameraChinh.GetComponent<CameraController>();

        // Load current parameters
        LoadCurrentCameraParameters();
        
        // Find và load shared NPC camera parameters
        LoadAllNPCCameras();

        // Log system info
        Debug.Log("=== 🎮 ĐIỀU CHỈNH THÔNG SỐ CAMERA INITIALIZED ===");
        Debug.Log($"📷 Camera chính: {cameraChinh.name}");
        Debug.Log($"🎛️ QuanLyCamera: {quanLyCamera.name}");
        Debug.Log($"🔧 CameraController: {(cameraController != null ? "✅ Available" : "❌ Missing")}");
        Debug.Log($"🎯 Số lượng NPC Cameras: {tatCaNPCCameras.Count}");
        Debug.Log("💡 Nhấn icon góc màn hình để mở Panel Điều Chỉnh Camera");
        Debug.Log("🔄 Thông số NPC sẽ áp dụng cho TẤT CẢ NPC cameras cùng lúc");
    }

    /// <summary>
    /// Tìm tất cả NPC cameras và load shared parameters
    /// </summary>
    private void LoadAllNPCCameras()
    {
        // Tìm tất cả NPC cameras trong scene
        NPCCamera[] allNPCCameras = FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
        tatCaNPCCameras.Clear();
        tatCaNPCCameras.AddRange(allNPCCameras);
        
        // Tìm NPC camera đang active để làm reference
        npcCameraHienTai = null;
        foreach (NPCCamera npcCam in tatCaNPCCameras)
        {
            Camera? npcCamera = npcCam.GetCamera();
            if (npcCamera != null && npcCamera.enabled)
            {
                npcCameraHienTai = npcCam;
                break;
            }
        }
        
        // Load shared parameters từ NPC camera đầu tiên (nếu có)
        if (tatCaNPCCameras.Count > 0)
        {
            NPCCamera firstNPC = tatCaNPCCameras[0];
            sharedNPCTocDoXoay = firstNPC.LayTocDoXoay();
            sharedNPCNhanTocDoXoay = firstNPC.LayNhanTocDoXoayNhanh();
            sharedNPCDoNhayChuot = firstNPC.LayDoNhayChuot();
            sharedNPCKhoangCach = firstNPC.LayKhoangCach();
            
            Debug.Log($"🔄 Loaded shared NPC parameters từ {firstNPC.name}");
        }
    }

    /// <summary>
    /// Load current camera parameters
    /// </summary>
    private void LoadCurrentCameraParameters()
    {
        // Load camera chính parameters
        if (cameraController != null)
        {
            runtimeTocDoXoayCamera = cameraController.LayTocDoXoay();
            runtimeNhanTocDoXoay = cameraController.LayNhanTocDoXoayNhanh();
            runtimeDoNhayChuot = cameraController.LayDoNhayChuot();
            runtimeTocDoChuyenDong = cameraController.LayTocDoChuyenDong();
            // runtimeTocDoChuyenDongNhanh không có getter method - sử dụng giá trị mặc định
        }
    }

    /// <summary>
    /// Áp dụng tốc độ xoay cho TẤT CẢ NPC cameras
    /// </summary>
    private void ApDungTocDoXoayChoTatCaNPC(float tocDoXoay)
    {
        int soLuongDaApDung = 0;
        foreach (NPCCamera npcCam in tatCaNPCCameras)
        {
            if (npcCam != null)
            {
                npcCam.DatTocDoXoay(tocDoXoay);
                soLuongDaApDung++;
            }
        }
        Debug.Log($"🎯 Đã áp dụng tốc độ xoay {tocDoXoay:F0}°/s cho {soLuongDaApDung} NPC cameras");
    }

    /// <summary>
    /// Áp dụng nhân tốc độ xoay nhanh cho TẤT CẢ NPC cameras
    /// </summary>
    private void ApDungNhanTocDoXoayChoTatCaNPC(float nhanTocDo)
    {
        int soLuongDaApDung = 0;
        foreach (NPCCamera npcCam in tatCaNPCCameras)
        {
            if (npcCam != null)
            {
                npcCam.DatNhanTocDoXoayNhanh(nhanTocDo);
                soLuongDaApDung++;
            }
        }
        Debug.Log($"🚀 Đã áp dụng nhân tốc độ x{nhanTocDo:F1} cho {soLuongDaApDung} NPC cameras");
    }

    /// <summary>
    /// Áp dụng độ nhạy chuột cho TẤT CẢ NPC cameras
    /// </summary>
    private void ApDungDoNhayChuotChoTatCaNPC(float doNhay)
    {
        int soLuongDaApDung = 0;
        foreach (NPCCamera npcCam in tatCaNPCCameras)
        {
            if (npcCam != null)
            {
                npcCam.DatDoNhayChuot(doNhay);
                soLuongDaApDung++;
            }
        }
        Debug.Log($"🖱️ Đã áp dụng độ nhạy chuột {doNhay:F1} cho {soLuongDaApDung} NPC cameras");
    }

    /// <summary>
    /// Áp dụng khoảng cách cho TẤT CẢ NPC cameras
    /// </summary>
    private void ApDungKhoangCachChoTatCaNPC(float khoangCach)
    {
        int soLuongDaApDung = 0;
        foreach (NPCCamera npcCam in tatCaNPCCameras)
        {
            if (npcCam != null)
            {
                npcCam.DatKhoangCach(khoangCach);
                soLuongDaApDung++;
            }
        }
        Debug.Log($"📏 Đã áp dụng khoảng cách {khoangCach:F1} cho {soLuongDaApDung} NPC cameras");
    }

    /// <summary>
    /// Initialize GUI styles
    /// </summary>
    private void InitializeGUIStyles()
    {
        if (iconStyle == null)
        {
            iconStyle = new GUIStyle(GUI.skin.button);
            iconStyle.fontSize = 24;
            iconStyle.fontStyle = FontStyle.Bold;
        }

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 16;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
        }
    }

    /// <summary>
    /// Render GUI
    /// </summary>
    private void OnGUI()
    {
        InitializeGUIStyles();

        // Toggle icon (always visible)
        DrawToggleIcon();

        // Panel (only when enabled)
        if (hienThiPanel)
        {
            DrawMainPanel();
        }
    }    /// <summary>
    /// Draw toggle icon
    /// </summary>
    private void DrawToggleIcon()
    {
        Rect iconRect = new Rect(iconPosition.x, iconPosition.y, iconSize, iconSize);
        
        // Style for prominent display
        GUIStyle iconStyle = new GUIStyle(GUI.skin.button);
        iconStyle.fontSize = 14;
        iconStyle.fontStyle = FontStyle.Bold;
        iconStyle.normal.textColor = Color.white;
        iconStyle.hover.textColor = Color.yellow;
        
        // Background color based on state
        if (hienThiPanel)
        {
            GUI.backgroundColor = Color.green;
        }
        else
        {
            GUI.backgroundColor = Color.cyan;
        }
          // Sử dụng text thay vì emoji để đảm bảo hiển thị
        string iconText = hienThiPanel ? "📹 ON" : "📹 OFF";
        
        if (GUI.Button(iconRect, iconText, iconStyle))
        {
            hienThiPanel = !hienThiPanel;
            Debug.Log($"🎛️ Panel Điều Chỉnh Camera: {(hienThiPanel ? "OPENED" : "CLOSED")}");
        }
        
        // Reset background color
        GUI.backgroundColor = Color.white;
        
        // Tooltip khi hover
        if (iconRect.Contains(Event.current.mousePosition))
        {
            Vector2 tooltipPos = new Vector2(iconRect.x, iconRect.y + iconRect.height + 5);
            GUI.Label(new Rect(tooltipPos.x, tooltipPos.y, 120, 20), "Click để mở Camera Settings");
        }
    }

    /// <summary>
    /// Draw main panel with dragging support
    /// </summary>
    private void DrawMainPanel()
    {
        // Panel background
        Rect panelRect = new Rect(panelPosition.x, panelPosition.y, panelSize.x, panelSize.y);
        
        // Handle dragging
        HandlePanelDragging(panelRect);
        
        // Draw panel
        GUI.Box(panelRect, "");
        
        GUILayout.BeginArea(panelRect);
        GUILayout.BeginVertical();
        
        // Title bar with close button
        GUILayout.BeginHorizontal();
        GUILayout.Label("⚙️ ĐIỀU CHỈNH THÔNG SỐ CAMERA", titleStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("❌", GUILayout.Width(30), GUILayout.Height(25)))
        {
            hienThiPanel = false;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Camera info
        DrawCameraInfo();
        
        GUILayout.Space(10);
        
        // Camera controls
        DrawCameraControls();
        
        GUILayout.Space(10);
        
        // Parameter adjustments
        DrawParameterAdjustments();
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Handle panel dragging
    /// </summary>
    private void HandlePanelDragging(Rect panelRect)
    {
        Rect titleBarRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 30);
        
        Event currentEvent = Event.current;
        
        if (currentEvent.type == EventType.MouseDown && titleBarRect.Contains(currentEvent.mousePosition))
        {
            draggingPanel = true;
            dragOffset = currentEvent.mousePosition - panelPosition;
        }
        
        if (draggingPanel)
        {
            if (currentEvent.type == EventType.MouseDrag)
            {
                panelPosition = currentEvent.mousePosition - dragOffset;
                
                // Keep panel on screen
                panelPosition.x = Mathf.Clamp(panelPosition.x, 0, Screen.width - panelSize.x);
                panelPosition.y = Mathf.Clamp(panelPosition.y, 0, Screen.height - panelSize.y);
            }
            else if (currentEvent.type == EventType.MouseUp)
            {
                draggingPanel = false;
            }
        }
    }

    /// <summary>
    /// Draw camera information
    /// </summary>
    private void DrawCameraInfo()
    {
        GUILayout.Label("📊 THÔNG TIN CAMERA", titleStyle);
        
        if (cameraChinh != null)
        {
            bool isCameraChinhActive = cameraChinh.enabled;
            GUILayout.Label($"📷 Camera hiện tại: {(isCameraChinhActive ? "Camera chính" : "Camera NPC")}");
            GUILayout.Label($"🎯 Số lượng NPC Cameras: {tatCaNPCCameras.Count}");
            
            if (isCameraChinhActive)
            {
                GUILayout.Label($"📍 Vị trí: {cameraChinh.transform.position:F1}");
                GUILayout.Label($"🔄 Góc xoay: {cameraChinh.transform.eulerAngles:F1}");
                
                if (cameraController != null)
                {
                    GUILayout.Label($"⚙️ Tốc độ xoay: {cameraController.LayTocDoXoay():F0}°/s");
                    GUILayout.Label($"🚀 Boost nhân: x{cameraController.LayNhanTocDoXoayNhanh():F1}");
                    GUILayout.Label($"🖱️ Độ nhạy chuột: {cameraController.LayDoNhayChuot():F1}");
                }
            }
            else if (npcCameraHienTai != null)
            {
                Camera? npcCam = npcCameraHienTai.GetCamera();
                if (npcCam != null)
                {
                    GUILayout.Label($"📍 Vị trí NPC: {npcCam.transform.position:F1}");
                    GUILayout.Label($"🔄 Góc xoay NPC: {npcCam.transform.eulerAngles:F1}");
                }
            }
            
            // Shared NPC parameters info
            GUILayout.Label($"🔗 SHARED NPC: Tốc độ {sharedNPCTocDoXoay:F0}°/s | Boost x{sharedNPCNhanTocDoXoay:F1} | Nhạy {sharedNPCDoNhayChuot:F1} | KC {sharedNPCKhoangCach:F1}");
        }
    }

    /// <summary>
    /// Draw camera control buttons
    /// </summary>
    private void DrawCameraControls()
    {
        GUILayout.Label("🕹️ ĐIỀU KHIỂN CAMERA", titleStyle);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Camera chính", buttonStyle))
        {
            ChuyenVeCameraChinh();
        }
        if (GUILayout.Button("🎯 Camera NPC", buttonStyle))
        {
            ChuyenCameraNPC();
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("🔄 Refresh NPC List", buttonStyle))
        {
            LoadAllNPCCameras();
        }
    }

    /// <summary>
    /// Draw parameter adjustment sliders
    /// </summary>
    private void DrawParameterAdjustments()
    {
        GUILayout.Label("⚙️ ĐIỀU CHỈNH CAMERA CHÍNH", titleStyle);
        
        if (cameraController != null)
        {
            // Tốc độ xoay
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tốc độ xoay:", GUILayout.Width(120));
            float newTocDoXoay = GUILayout.HorizontalSlider(runtimeTocDoXoayCamera, 50f, 300f);
            GUILayout.Label($"{newTocDoXoay:F0}°/s", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newTocDoXoay != runtimeTocDoXoayCamera)
            {
                runtimeTocDoXoayCamera = newTocDoXoay;                cameraController.DatTocDoXoay(runtimeTocDoXoayCamera);
                Debug.Log($"🔄 Đã đặt tốc độ xoay camera chính: {runtimeTocDoXoayCamera:F0}°/s");
                
                // Auto save if enabled
                if (autoSaveOnChange)
                {
                    TryAutoSave();
                }
            }
            
            // Nhân boost
            GUILayout.BeginHorizontal();
            GUILayout.Label("Nhân boost:", GUILayout.Width(120));
            float newNhanBoost = GUILayout.HorizontalSlider(runtimeNhanTocDoXoay, 1f, 5f);
            GUILayout.Label($"x{newNhanBoost:F1}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newNhanBoost != runtimeNhanTocDoXoay)
            {
                runtimeNhanTocDoXoay = newNhanBoost;
                cameraController.DatNhanTocDoXoayNhanh(runtimeNhanTocDoXoay);                Debug.Log($"🚀 Đã đặt nhân boost camera chính: x{runtimeNhanTocDoXoay:F1}");
                
                // Auto save if enabled
                if (autoSaveOnChange)
                {
                    TryAutoSave();
                }
            }
            
            // Độ nhạy chuột
            GUILayout.BeginHorizontal();
            GUILayout.Label("Độ nhạy chuột:", GUILayout.Width(120));
            float newDoNhay = GUILayout.HorizontalSlider(runtimeDoNhayChuot, 0.5f, 10f);
            GUILayout.Label($"{newDoNhay:F1}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
              if (newDoNhay != runtimeDoNhayChuot)
            {
                runtimeDoNhayChuot = newDoNhay;
                cameraController.DatDoNhayChuot(runtimeDoNhayChuot);
                Debug.Log($"🖱️ Đã đặt độ nhạy chuột camera chính: {runtimeDoNhayChuot:F1}");
                
                // Auto save if enabled
                if (autoSaveOnChange)
                {
                    TryAutoSave();
                }
            }
            
            // Tốc độ di chuyển
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tốc độ di chuyển:", GUILayout.Width(120));
            float newTocDoChuyenDong = GUILayout.HorizontalSlider(runtimeTocDoChuyenDong, 1f, 50f);
            GUILayout.Label($"{newTocDoChuyenDong:F0}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
              if (newTocDoChuyenDong != runtimeTocDoChuyenDong)
            {
                runtimeTocDoChuyenDong = newTocDoChuyenDong;
                cameraController.DatTocDoChuyenDong(runtimeTocDoChuyenDong);
                Debug.Log($"🏃 Đã đặt tốc độ di chuyển camera chính: {runtimeTocDoChuyenDong:F0}");
                
                // Auto save if enabled
                if (autoSaveOnChange)
                {
                    TryAutoSave();
                }
            }
        }
        else
        {
            GUILayout.Label("❌ CameraController không tìm thấy");
        }

        // === SHARED NPC CAMERA CONTROLS ===
        GUILayout.Space(10);
        GUILayout.Label("🎯 SHARED NPC PARAMETERS (TẤT CẢ NPC)", titleStyle);
        
        if (tatCaNPCCameras.Count > 0)
        {
            // Tốc độ xoay SHARED NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Tốc độ xoay ALL ({tatCaNPCCameras.Count}):", GUILayout.Width(150));
            float newSharedTocDoXoay = GUILayout.HorizontalSlider(sharedNPCTocDoXoay, 50f, 300f);
            GUILayout.Label($"{newSharedTocDoXoay:F0}°/s", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newSharedTocDoXoay != sharedNPCTocDoXoay)
            {
                sharedNPCTocDoXoay = newSharedTocDoXoay;
                ApDungTocDoXoayChoTatCaNPC(sharedNPCTocDoXoay);
            }
            
            // Nhân boost SHARED NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Nhân boost ALL ({tatCaNPCCameras.Count}):", GUILayout.Width(150));
            float newSharedNhanBoost = GUILayout.HorizontalSlider(sharedNPCNhanTocDoXoay, 1f, 5f);
            GUILayout.Label($"x{newSharedNhanBoost:F1}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newSharedNhanBoost != sharedNPCNhanTocDoXoay)
            {
                sharedNPCNhanTocDoXoay = newSharedNhanBoost;
                ApDungNhanTocDoXoayChoTatCaNPC(sharedNPCNhanTocDoXoay);
            }
            
            // Độ nhạy chuột SHARED NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Độ nhạy ALL ({tatCaNPCCameras.Count}):", GUILayout.Width(150));
            float newSharedDoNhay = GUILayout.HorizontalSlider(sharedNPCDoNhayChuot, 0.5f, 10f);
            GUILayout.Label($"{newSharedDoNhay:F1}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newSharedDoNhay != sharedNPCDoNhayChuot)
            {
                sharedNPCDoNhayChuot = newSharedDoNhay;
                ApDungDoNhayChuotChoTatCaNPC(sharedNPCDoNhayChuot);
            }
            
            // Khoảng cách SHARED NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Khoảng cách ALL ({tatCaNPCCameras.Count}):", GUILayout.Width(150));
            float newSharedKhoangCach = GUILayout.HorizontalSlider(sharedNPCKhoangCach, 2f, 15f);
            GUILayout.Label($"{newSharedKhoangCach:F1}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            
            if (newSharedKhoangCach != sharedNPCKhoangCach)
            {
                sharedNPCKhoangCach = newSharedKhoangCach;
                ApDungKhoangCachChoTatCaNPC(sharedNPCKhoangCach);
            }
              // Reset buttons
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🔄 Reset Camera Chính", buttonStyle))
            {
                ResetCameraChinhToDefaults();
            }
            if (GUILayout.Button("🎯 Reset ALL NPC", buttonStyle))
            {
                ResetAllNPCToDefaults();
            }
            GUILayout.EndHorizontal();
            
            // Save/Load buttons
            GUILayout.Space(15);
            GUILayout.Label("💾 SAVE/LOAD SETTINGS", titleStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Save Settings", buttonStyle))
            {
                SaveCurrentSettings();
            }
            if (GUILayout.Button("📂 Load Settings", buttonStyle))
            {
                LoadSavedSettings();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🗑️ Clear Saved & Reset", buttonStyle))
            {
                if (GUI.changed || Event.current.type == EventType.Used)
                {
                    ResetAndClearSavedSettings();
                }
            }
            GUILayout.Label($"Auto-save: {(autoSaveOnChange ? "ON" : "OFF")}", GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("❌ Không có NPC Camera nào trong scene");
            if (GUILayout.Button("🔄 Refresh NPC List", buttonStyle))
            {
                LoadAllNPCCameras();
            }
        }
    }

    /// <summary>
    /// Chuyển về camera chính
    /// </summary>
    private void ChuyenVeCameraChinh()
    {
        if (quanLyCamera != null)
        {
            quanLyCamera.BatCameraChinh();
            Debug.Log("🔄 Chuyển về camera chính");
        }
    }

    /// <summary>
    /// Chuyển camera NPC
    /// </summary>
    private void ChuyenCameraNPC()
    {
        if (quanLyCamera != null)
        {
            quanLyCamera.ChuyenCameraKeTiep();
            Debug.Log("🎯 Chuyển camera NPC");
            
            // Update current NPC camera reference
            LoadAllNPCCameras();
        }
    }

    /// <summary>
    /// Reset camera chính về giá trị mặc định
    /// </summary>
    private void ResetCameraChinhToDefaults()
    {
        runtimeTocDoXoayCamera = 150f;
        runtimeNhanTocDoXoay = 2.5f;
        runtimeDoNhayChuot = 3f;
        runtimeTocDoChuyenDong = 10f;
        runtimeTocDoChuyenDongNhanh = 20f;
        
        if (cameraController != null)
        {
            cameraController.DatTocDoXoay(runtimeTocDoXoayCamera);
            cameraController.DatNhanTocDoXoayNhanh(runtimeNhanTocDoXoay);
            cameraController.DatDoNhayChuot(runtimeDoNhayChuot);
            cameraController.DatTocDoChuyenDong(runtimeTocDoChuyenDong);
        }
        
        Debug.Log("🔄 Đã reset Camera chính về giá trị mặc định");
    }

    /// <summary>
    /// Reset TẤT CẢ NPC cameras về giá trị mặc định
    /// </summary>
    private void ResetAllNPCToDefaults()
    {
        sharedNPCTocDoXoay = 150f;
        sharedNPCNhanTocDoXoay = 2.5f;
        sharedNPCDoNhayChuot = 3f;
        sharedNPCKhoangCach = 5f;
        
        // Áp dụng cho tất cả NPC cameras
        ApDungTocDoXoayChoTatCaNPC(sharedNPCTocDoXoay);
        ApDungNhanTocDoXoayChoTatCaNPC(sharedNPCNhanTocDoXoay);
        ApDungDoNhayChuotChoTatCaNPC(sharedNPCDoNhayChuot);
        ApDungKhoangCachChoTatCaNPC(sharedNPCKhoangCach);
        
        Debug.Log($"🎯 Đã reset TẤT CẢ {tatCaNPCCameras.Count} NPC cameras về giá trị mặc định");
    }

    /// <summary>
    /// Toggle panel visibility từ code
    /// </summary>
    public void TogglePanel()
    {
        hienThiPanel = !hienThiPanel;
    }

    /// <summary>
    /// Show/hide panel từ code
    /// </summary>
    public void ShowPanel(bool show)
    {
        hienThiPanel = show;
    }

    /// <summary>
    /// Load saved settings từ PlayerPrefs
    /// </summary>
    private void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey(PREF_MAIN_XOAY))
        {
            // Load Main Camera settings
            runtimeTocDoXoayCamera = PlayerPrefs.GetFloat(PREF_MAIN_XOAY, 150f);
            runtimeNhanTocDoXoay = PlayerPrefs.GetFloat(PREF_MAIN_NHAN, 2.5f);
            runtimeDoNhayChuot = PlayerPrefs.GetFloat(PREF_MAIN_NHAY, 3f);
            runtimeTocDoChuyenDong = PlayerPrefs.GetFloat(PREF_MAIN_TOCDO, 10f);
            runtimeTocDoChuyenDongNhanh = PlayerPrefs.GetFloat(PREF_MAIN_TOCDO_NHANH, 20f);
            
            // Load NPC Camera settings
            sharedNPCTocDoXoay = PlayerPrefs.GetFloat(PREF_NPC_XOAY, 150f);
            sharedNPCNhanTocDoXoay = PlayerPrefs.GetFloat(PREF_NPC_NHAN, 2.5f);
            sharedNPCDoNhayChuot = PlayerPrefs.GetFloat(PREF_NPC_NHAY, 3f);
            sharedNPCKhoangCach = PlayerPrefs.GetFloat(PREF_NPC_KHOANGCACH, 5f);
            
            Debug.Log("💾 Đã load camera settings từ PlayerPrefs");
            
            // Apply loaded settings to cameras
            ApplyCurrentSettingsToAllCameras();
        }
        else
        {
            Debug.Log("💾 Không tìm thấy saved settings, sử dụng default values");
        }
    }

    /// <summary>
    /// Save current settings vào PlayerPrefs
    /// </summary>
    private void SaveCurrentSettings()
    {
        // Save Main Camera settings
        PlayerPrefs.SetFloat(PREF_MAIN_XOAY, runtimeTocDoXoayCamera);
        PlayerPrefs.SetFloat(PREF_MAIN_NHAN, runtimeNhanTocDoXoay);
        PlayerPrefs.SetFloat(PREF_MAIN_NHAY, runtimeDoNhayChuot);
        PlayerPrefs.SetFloat(PREF_MAIN_TOCDO, runtimeTocDoChuyenDong);
        PlayerPrefs.SetFloat(PREF_MAIN_TOCDO_NHANH, runtimeTocDoChuyenDongNhanh);
        
        // Save NPC Camera settings
        PlayerPrefs.SetFloat(PREF_NPC_XOAY, sharedNPCTocDoXoay);
        PlayerPrefs.SetFloat(PREF_NPC_NHAN, sharedNPCNhanTocDoXoay);
        PlayerPrefs.SetFloat(PREF_NPC_NHAY, sharedNPCDoNhayChuot);
        PlayerPrefs.SetFloat(PREF_NPC_KHOANGCACH, sharedNPCKhoangCach);
        
        PlayerPrefs.Save();
        Debug.Log("💾 Đã save camera settings vào PlayerPrefs");
    }

    /// <summary>
    /// Apply current settings to all cameras
    /// </summary>
    private void ApplyCurrentSettingsToAllCameras()
    {
        // Apply to main camera if exists
        if (cameraController != null)
        {
            cameraController.DatTocDoXoay(runtimeTocDoXoayCamera);
            cameraController.DatNhanTocDoXoayNhanh(runtimeNhanTocDoXoay);
            cameraController.DatDoNhayChuot(runtimeDoNhayChuot);
            cameraController.DatTocDoChuyenDong(runtimeTocDoChuyenDong);
        }
        
        // Apply to all NPC cameras
        ApDungTocDoXoayChoTatCaNPC(sharedNPCTocDoXoay);
        ApDungNhanTocDoXoayChoTatCaNPC(sharedNPCNhanTocDoXoay);
        ApDungDoNhayChuotChoTatCaNPC(sharedNPCDoNhayChuot);
        ApDungKhoangCachChoTatCaNPC(sharedNPCKhoangCach);
    }    /// <summary>
    /// Reset all settings to defaults và xóa saved data
    /// </summary>
    private void ResetAndClearSavedSettings()
    {
        // Reset to defaults
        ResetCameraChinhToDefaults();
        ResetAllNPCToDefaults();
        
        // Clear saved data
        PlayerPrefs.DeleteKey(PREF_MAIN_XOAY);
        PlayerPrefs.DeleteKey(PREF_MAIN_NHAN);
        PlayerPrefs.DeleteKey(PREF_MAIN_NHAY);
        PlayerPrefs.DeleteKey(PREF_MAIN_TOCDO);
        PlayerPrefs.DeleteKey(PREF_MAIN_TOCDO_NHANH);
        PlayerPrefs.DeleteKey(PREF_NPC_XOAY);
        PlayerPrefs.DeleteKey(PREF_NPC_NHAN);
        PlayerPrefs.DeleteKey(PREF_NPC_NHAY);
        PlayerPrefs.DeleteKey(PREF_NPC_KHOANGCACH);
        PlayerPrefs.Save();
        
        Debug.Log("🗑️ Đã reset tất cả settings và xóa saved data");
    }

    /// <summary>
    /// Auto save nếu enabled
    /// </summary>
    private void TryAutoSave()
    {
        if (autoSaveOnChange)
        {
            SaveCurrentSettings();
        }
    }
}
