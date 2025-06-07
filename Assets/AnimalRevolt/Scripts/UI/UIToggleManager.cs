using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// Quản lý toggle UI cho toàn bộ hệ thống - Legacy Version
    /// Được thay thế bởi UnifiedUIManager cho chức năng mở rộng
    /// Vẫn giữ để backward compatibility
    /// </summary>
    [System.Obsolete("Sử dụng UnifiedUIManager thay vì UIToggleManager để có chức năng mở rộng hơn")]
    public class UIToggleManager : MonoBehaviour
    {
        [Header("⚠️ LEGACY WARNING")]
        [SerializeField, Tooltip("Script này đã được thay thế bởi UnifiedUIManager")]
        private bool showLegacyWarning = true;
        
        [Header("🎛️ UI Toggle Settings")]
        [SerializeField, Tooltip("Phím tắt để toggle UI")]
        private InputAction toggleUIAction = new InputAction("ToggleUI", InputActionType.Button, "<Keyboard>/f1");
        
        [SerializeField, Tooltip("Vị trí nút toggle (góc màn hình)")]
        private RectTransform.Edge viTriNutToggle = RectTransform.Edge.Top;
        
        [SerializeField, Tooltip("Kích thước nút toggle")]
        private Vector2 kichThuocNut = new Vector2(60, 60);
        
        [SerializeField, Tooltip("Khoảng cách từ góc màn hình")]
        private Vector2 offsetTuGoc = new Vector2(20, 20);

        [Header("📱 UI References")]
        [SerializeField, Tooltip("Canvas chứa UI toggle")]
        private Canvas uiCanvas;
        
        [SerializeField, Tooltip("Panel chứa tất cả UI có thể toggle")]
        private GameObject uiPanel;
        
        [SerializeField, Tooltip("CameraSettingsUI component")]
        private CameraSettingsUI cameraSettingsUI;
        
        [Header("🆕 Unified UI Integration")]
        [SerializeField, Tooltip("UnifiedUIManager để quản lý UI tổng hợp")]
        private UnifiedUIManager unifiedUIManager;
        
        [SerializeField, Tooltip("Tự động chuyển sang UnifiedUIManager")]
        private bool autoMigrateToUnified = true;

        [Header("🎨 UI Styling")]
        [SerializeField, Tooltip("Icon cho nút camera")]
        private Sprite iconCamera;
        
        [SerializeField, Tooltip("Màu nút khi UI đang bật")]
        private Color mauNutBat = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        
        [SerializeField, Tooltip("Màu nút khi UI đang tắt")]
        private Color mauNutTat = new Color(0.5f, 0.5f, 0.5f, 0.6f);

        // UI Components
        private Button nutToggle;
        private Image hinhAnhNut;
        private bool dangHienThiUI = false;
        
        // UI Panel List
        private CameraSettingsUI[] danhSachCameraUI;
        
        // Migration flag
        private bool hasMigrated = false;

        private void Awake()
        {
            // Hiển thị cảnh báo legacy
            if (showLegacyWarning)
            {
                Debug.LogWarning("⚠️ UIToggleManager is LEGACY! Use UnifiedUIManager for enhanced functionality");
            }
            
            // Kiểm tra migration
            if (autoMigrateToUnified)
            {
                TryMigrateToUnified();
            }
            
            if (!hasMigrated)
            {
                TaoUIToggle();
                TimCacUIComponent();
            }
        }

        private void Start()
        {
            if (hasMigrated) return;
            
            // Enable input action
            toggleUIAction.Enable();
            
            // Thiết lập ban đầu
            CapNhatTrangThaiUI(false);
            
            Debug.Log("🎛️ UIToggleManager đã khởi tạo (Legacy Mode)");
        }
private void Update()
{
    if (hasMigrated) return;
    
    // Xử lý phím tắt
    if (toggleUIAction.WasPressedThisFrame())
    {
        ToggleAllUI();
    }
}

/// <summary>
/// Thử migration sang UnifiedUIManager
/// </summary>
private void TryMigrateToUnified()
{
    // Tìm UnifiedUIManager trong scene
    if (unifiedUIManager == null)
    {
        unifiedUIManager = FindFirstObjectByType<UnifiedUIManager>();
    }
    
    // Nếu chưa có, tạo mới
    if (unifiedUIManager == null && autoMigrateToUnified)
    {
        GameObject unifiedObj = new GameObject("UnifiedUIManager");
        unifiedUIManager = unifiedObj.AddComponent<UnifiedUIManager>();
        
        Debug.Log("🆕 Đã tạo UnifiedUIManager mới để thay thế UIToggleManager");
    }
    
    if (unifiedUIManager != null)
    {
        hasMigrated = true;
        Debug.Log("✅ Migration thành công sang UnifiedUIManager");
        
        // Disable legacy components
        if (nutToggle != null)
            nutToggle.gameObject.SetActive(false);
    }
}

        /// <summary>
        /// Tạo nút toggle UI ở góc màn hình
        /// </summary>
        private void TaoUIToggle()
        {
            // Tạo Canvas nếu chưa có
            if (uiCanvas == null)
            {
                GameObject canvasObj = new GameObject("UIToggleCanvas");
                uiCanvas = canvasObj.AddComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                uiCanvas.sortingOrder = 1000; // Hiển thị trên cùng
                
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Tạo nút toggle
            GameObject nutObj = new GameObject("ToggleUIButton");
            nutObj.transform.SetParent(uiCanvas.transform, false);
            
            // Thêm Image component
            hinhAnhNut = nutObj.AddComponent<Image>();
            hinhAnhNut.sprite = iconCamera;
            hinhAnhNut.color = mauNutTat;
            
            // Thêm Button component
            nutToggle = nutObj.AddComponent<Button>();
            nutToggle.onClick.AddListener(ToggleAllUI);
            
            // Thiết lập RectTransform
            RectTransform rectTransform = nutObj.GetComponent<RectTransform>();
            ThietLapViTriNut(rectTransform);
        }

        /// <summary>
        /// Thiết lập vị trí nút theo góc màn hình
        /// </summary>
        private void ThietLapViTriNut(RectTransform rectTransform)
        {
            rectTransform.sizeDelta = kichThuocNut;
            
            switch (viTriNutToggle)
            {
                case RectTransform.Edge.Top:
                    // Góc trên-phải
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.anchoredPosition = new Vector2(-offsetTuGoc.x, -offsetTuGoc.y);
                    break;
                    
                case RectTransform.Edge.Bottom:
                    // Góc dưới-phải
                    rectTransform.anchorMin = new Vector2(1f, 0f);
                    rectTransform.anchorMax = new Vector2(1f, 0f);
                    rectTransform.anchoredPosition = new Vector2(-offsetTuGoc.x, offsetTuGoc.y);
                    break;
                    
                case RectTransform.Edge.Left:
                    // Góc trên-trái
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.anchoredPosition = new Vector2(offsetTuGoc.x, -offsetTuGoc.y);
                    break;
                    
                case RectTransform.Edge.Right:
                    // Góc trên-phải (mặc định)
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.anchoredPosition = new Vector2(-offsetTuGoc.x, -offsetTuGoc.y);
                    break;
            }
        }

        /// <summary>
        /// Tìm các UI component trong scene
        /// </summary>
        private void TimCacUIComponent()
        {
            // Tìm CameraSettingsUI
            if (cameraSettingsUI == null)
            {
                cameraSettingsUI = FindFirstObjectByType<CameraSettingsUI>();
            }
            
            // Tìm tất cả Camera UI trong scene
            danhSachCameraUI = FindObjectsByType<CameraSettingsUI>(FindObjectsSortMode.None);
            
            Debug.Log($"📱 Tìm thấy {danhSachCameraUI.Length} CameraSettingsUI component(s)");
        }

        /// <summary>
        /// Toggle tất cả UI - Delegate sang UnifiedUIManager nếu có
        /// </summary>
        public void ToggleAllUI()
        {
            if (hasMigrated && unifiedUIManager != null)
            {
                unifiedUIManager.ToggleAllUI();
                return;
            }
            
            dangHienThiUI = !dangHienThiUI;
            CapNhatTrangThaiUI(dangHienThiUI);
            
            Debug.Log($"🎛️ Toggle All UI (Legacy): {(dangHienThiUI ? "BẬT" : "TẮT")}");
        }

        /// <summary>
        /// Cập nhật trạng thái hiển thị của tất cả UI
        /// </summary>
        private void CapNhatTrangThaiUI(bool hienThi)
        {
            // Cập nhật màu nút
            if (hinhAnhNut != null)
            {
                hinhAnhNut.color = hienThi ? mauNutBat : mauNutTat;
            }

            // Toggle UI Panel nếu có
            if (uiPanel != null)
            {
                uiPanel.SetActive(hienThi);
            }

            // Toggle Camera Settings UI
            if (cameraSettingsUI != null)
            {
                cameraSettingsUI.DatHienThiUI(hienThi);
            }

            // Toggle tất cả Camera UI khác
            foreach (var cameraUI in danhSachCameraUI)
            {
                if (cameraUI != null && cameraUI != cameraSettingsUI)
                {
                    cameraUI.DatHienThiUI(hienThi);
                }
            }
        }

        /// <summary>
        /// Bật UI
        /// </summary>
        public void BatUI()
        {
            dangHienThiUI = true;
            CapNhatTrangThaiUI(true);
        }

        /// <summary>
        /// Tắt UI
        /// </summary>
        public void TatUI()
        {
            dangHienThiUI = false;
            CapNhatTrangThaiUI(false);
        }

        /// <summary>
        /// Thêm UI component vào danh sách quản lý
        /// </summary>
        public void ThemUIComponent(GameObject uiComponent)
        {
            if (uiPanel == null)
            {
                // Tạo UI Panel để chứa các UI component
                GameObject panelObj = new GameObject("ManagedUIPanel");
                panelObj.transform.SetParent(uiCanvas.transform, false);
                uiPanel = panelObj;
            }
            
            if (uiComponent != null)
            {
                uiComponent.transform.SetParent(uiPanel.transform, false);
            }
        }

        /// <summary>
        /// Thiết lập phím tắt mới
        /// </summary>
        public void DatPhimTat(string keyBinding)
        {
            toggleUIAction.Dispose();
            toggleUIAction = new InputAction("ToggleUI", InputActionType.Button, keyBinding);
            toggleUIAction.Enable();
        }

        /// <summary>
        /// Thiết lập vị trí nút
        /// </summary>
        public void DatViTriNut(RectTransform.Edge viTri, Vector2 offset)
        {
            viTriNutToggle = viTri;
            offsetTuGoc = offset;
            
            if (nutToggle != null)
            {
                ThietLapViTriNut(nutToggle.GetComponent<RectTransform>());
            }
        }

        /// <summary>
        /// Lấy trạng thái hiển thị UI
        /// </summary>
        public bool DangHienThiUI()
        {
            return dangHienThiUI;
        }

        private void OnDestroy()
        {
            // Dispose Input Action
            toggleUIAction?.Dispose();
        }

        #region Context Menu Actions

        [ContextMenu("🎛️ Toggle UI")]
        public void DebugToggleUI()
        {
            ToggleAllUI();
        }

        [ContextMenu("📱 Refresh UI Components")]
        public void DebugRefreshComponents()
        {
            TimCacUIComponent();
        }

        [ContextMenu("🔧 Setup Default Position")]
        public void DebugSetupDefaultPosition()
        {
            DatViTriNut(RectTransform.Edge.Top, new Vector2(20, 20));
        }
        
        [ContextMenu("🆕 Migrate to UnifiedUIManager")]
        public void DebugMigrateToUnified()
        {
            autoMigrateToUnified = true;
            TryMigrateToUnified();
        }
        
        [ContextMenu("📊 Show Migration Status")]
        public void DebugShowMigrationStatus()
        {
            Debug.Log($"=== UIToggleManager Migration Status ===");
            Debug.Log($"🔄 Has Migrated: {hasMigrated}");
            Debug.Log($"🆕 UnifiedUIManager: {(unifiedUIManager != null ? "✅ Found" : "❌ Not Found")}");
            Debug.Log($"⚙️ Auto Migrate: {autoMigrateToUnified}");
            
            if (!hasMigrated)
            {
                Debug.Log("💡 Tip: Enable 'Auto Migrate To Unified' hoặc chạy 'Migrate to UnifiedUIManager'");
            }
        }

        #endregion
    }
}