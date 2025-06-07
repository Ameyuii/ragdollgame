using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// Quản lý tổng hợp toàn bộ UI trong game
    /// Bật tắt UI theo camera hiện tại và đồng bộ toàn bộ UI system
    /// </summary>
    public class UnifiedUIManager : MonoBehaviour
    {
        [Header("🎛️ UI System Settings")]
        [SerializeField, Tooltip("Phím tắt để toggle toàn bộ UI")]
        private InputAction toggleAllUIAction = new InputAction("ToggleAllUI", InputActionType.Button, "<Keyboard>/f1");
        
        [SerializeField, Tooltip("Phím tắt để toggle UI camera hiện tại")]
        private InputAction toggleCameraUIAction = new InputAction("ToggleCameraUI", InputActionType.Button, "<Keyboard>/f2");
        
        [SerializeField, Tooltip("Tự động tìm và quản lý tất cả UI")]
        private bool autoDiscoverUI = true;
        
        [SerializeField, Tooltip("Hiển thị UI khi start")]
        private bool showUIOnStart = false;

        [Header("📱 UI Toggle Button")]
        [SerializeField, Tooltip("Vị trí nút toggle")]
        private RectTransform.Edge buttonPosition = RectTransform.Edge.Top;
        
        [SerializeField, Tooltip("Kích thước nút")]
        private Vector2 buttonSize = new Vector2(60, 60);
        
        [SerializeField, Tooltip("Offset từ góc")]
        private Vector2 buttonOffset = new Vector2(20, 20);

        [Header("🎨 UI Styling")]
        [SerializeField, Tooltip("Icon cho nút toggle")]
        private Sprite toggleIcon;
        
        [SerializeField, Tooltip("Màu khi UI bật")]
        private Color activeColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        
        [SerializeField, Tooltip("Màu khi UI tắt")]
        private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);

        [Header("📹 Camera Integration")]
        [SerializeField, Tooltip("Camera Manager reference")]
        private AnimalRevolt.Camera.CameraManager cameraManager;
        
        [SerializeField, Tooltip("Theo dõi thay đổi camera tự động")]
        private bool autoTrackCameraChanges = true;

        // UI State Management
        private bool allUIVisible = false;
        private bool currentCameraUIVisible = false;
        private int lastActiveCameraIndex = -1;

        // UI Components Registry
        private Dictionary<string, List<MonoBehaviour>> uiComponentsByCategory;
        private Dictionary<int, List<MonoBehaviour>> uiComponentsByCamera;
        private List<MonoBehaviour> globalUIComponents;

        // UI Toggle Button
        private Canvas toggleCanvas;
        private Button toggleButton;
        private Image buttonImage;

        // UI Categories
        private const string CAMERA_UI_CATEGORY = "CameraUI";
        private const string RAGDOLL_UI_CATEGORY = "RagdollUI";  
        private const string COMBAT_UI_CATEGORY = "CombatUI";
        private const string HEALTH_UI_CATEGORY = "HealthUI";
        private const string CHARACTER_UI_CATEGORY = "CharacterUI";
        private const string GAME_UI_CATEGORY = "GameUI";

        private void Awake()
        {
            InitializeUISystem();
            CreateToggleButton();
        }

        private void Start()
        {
            if (autoDiscoverUI)
            {
                DiscoverAllUIComponents();
            }

            // Thiết lập trạng thái ban đầu
            SetAllUIVisibility(showUIOnStart);
            
            // Enable input actions
            toggleAllUIAction.Enable();
            toggleCameraUIAction.Enable();

            Debug.Log("🎛️ UnifiedUIManager đã khởi tạo thành công");
        }

        private void Update()
        {
            // Xử lý input
            HandleInputActions();
            
            // Theo dõi thay đổi camera
            if (autoTrackCameraChanges)
            {
                TrackCameraChanges();
            }
        }

        /// <summary>
        /// Khởi tạo hệ thống UI
        /// </summary>
        private void InitializeUISystem()
        {
            uiComponentsByCategory = new Dictionary<string, List<MonoBehaviour>>();
            uiComponentsByCamera = new Dictionary<int, List<MonoBehaviour>>();
            globalUIComponents = new List<MonoBehaviour>();

            // Khởi tạo categories
            string[] categories = { CAMERA_UI_CATEGORY, RAGDOLL_UI_CATEGORY, COMBAT_UI_CATEGORY, 
                                   HEALTH_UI_CATEGORY, CHARACTER_UI_CATEGORY, GAME_UI_CATEGORY };
            
            foreach (string category in categories)
            {
                uiComponentsByCategory[category] = new List<MonoBehaviour>();
            }

            // Tìm CameraManager nếu chưa có
            if (cameraManager == null)
            {
                cameraManager = FindFirstObjectByType<AnimalRevolt.Camera.CameraManager>();
            }
        }

        /// <summary>
        /// Tạo nút toggle UI
        /// </summary>
        private void CreateToggleButton()
        {
            // Tạo Canvas
            GameObject canvasObj = new GameObject("UnifiedUIToggleCanvas");
            toggleCanvas = canvasObj.AddComponent<Canvas>();
            toggleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            toggleCanvas.sortingOrder = 9999; // Trên cùng
            
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Tạo nút toggle
            GameObject buttonObj = new GameObject("UnifiedToggleButton");
            buttonObj.transform.SetParent(toggleCanvas.transform, false);

            // Setup Image
            buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.sprite = toggleIcon;
            buttonImage.color = inactiveColor;

            // Setup Button
            toggleButton = buttonObj.AddComponent<Button>();
            toggleButton.onClick.AddListener(ToggleAllUI);

            // Setup position
            SetupButtonPosition(buttonObj.GetComponent<RectTransform>());
        }

        /// <summary>
        /// Thiết lập vị trí nút
        /// </summary>
        private void SetupButtonPosition(RectTransform rectTransform)
        {
            rectTransform.sizeDelta = buttonSize;

            switch (buttonPosition)
            {
                case RectTransform.Edge.Top:
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.anchoredPosition = new Vector2(-buttonOffset.x, -buttonOffset.y);
                    break;
                case RectTransform.Edge.Bottom:
                    rectTransform.anchorMin = new Vector2(1f, 0f);
                    rectTransform.anchorMax = new Vector2(1f, 0f);
                    rectTransform.anchoredPosition = new Vector2(-buttonOffset.x, buttonOffset.y);
                    break;
                case RectTransform.Edge.Left:
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.anchoredPosition = new Vector2(buttonOffset.x, -buttonOffset.y);
                    break;
                case RectTransform.Edge.Right:
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.anchoredPosition = new Vector2(-buttonOffset.x, -buttonOffset.y);
                    break;
            }
        }

        /// <summary>
        /// Tự động tìm tất cả UI components
        /// </summary>
        private void DiscoverAllUIComponents()
        {
            Debug.Log("🔍 Bắt đầu tìm kiếm tất cả UI components...");

            // Clear existing
            foreach (var category in uiComponentsByCategory.Keys.ToList())
            {
                uiComponentsByCategory[category].Clear();
            }
            uiComponentsByCamera.Clear();
            globalUIComponents.Clear();

            // Tìm Camera UI
            RegisterCameraUIComponents();
            
            // Tìm Ragdoll UI
            RegisterRagdollUIComponents();
            
            // Tìm Combat UI
            RegisterCombatUIComponents();
            
            // Tìm Health UI
            RegisterHealthUIComponents();
            
            // Tìm Character UI
            RegisterCharacterUIComponents();
            
            // Tìm Game UI
            RegisterGameUIComponents();

            LogDiscoveredComponents();
        }

        /// <summary>
        /// Đăng ký Camera UI components
        /// </summary>
        private void RegisterCameraUIComponents()
        {
            // CameraSettingsUI
            CameraSettingsUI[] cameraUIs = FindObjectsByType<CameraSettingsUI>(FindObjectsSortMode.None);
            foreach (var ui in cameraUIs)
            {
                RegisterUIComponent(ui, CAMERA_UI_CATEGORY, -1); // Global camera UI
            }

            // NPCCamera UIs (nếu có)
            // Có thể mở rộng thêm các camera UI khác
        }

        /// <summary>
        /// Đăng ký Ragdoll UI components
        /// </summary>
        private void RegisterRagdollUIComponents()
        {            // RagdollPhysicsController (thường không có UI trực tiếp, nhưng có thể có debug UI)
            RagdollPhysicsController[] ragdollControllers = FindObjectsByType<RagdollPhysicsController>(FindObjectsSortMode.None);
            foreach (var controller in ragdollControllers)
            {
                // Đăng ký làm UI component nếu có UI elements
                RegisterUIComponent(controller, RAGDOLL_UI_CATEGORY);
            }
        }

        /// <summary>
        /// Đăng ký Combat UI components
        /// </summary>
        private void RegisterCombatUIComponents()
        {
            // Có thể có combat UI trong tương lai
            // Placeholder cho combat UI components
        }

        /// <summary>
        /// Đăng ký Health UI components
        /// </summary>
        private void RegisterHealthUIComponents()
        {
            HealthBar[] healthBars = FindObjectsByType<HealthBar>(FindObjectsSortMode.None);
            foreach (var healthBar in healthBars)
            {
                RegisterUIComponent(healthBar, HEALTH_UI_CATEGORY);
            }

            DamageNumberSpawner[] damageSpawners = FindObjectsByType<DamageNumberSpawner>(FindObjectsSortMode.None);
            foreach (var spawner in damageSpawners)
            {
                RegisterUIComponent(spawner, HEALTH_UI_CATEGORY);
            }
        }

        /// <summary>
        /// Đăng ký Character UI components
        /// </summary>
        private void RegisterCharacterUIComponents()
        {
            AnimalRevolt.UI.CharacterSelectionUI[] charSelectionUIs = FindObjectsByType<AnimalRevolt.UI.CharacterSelectionUI>(FindObjectsSortMode.None);
            foreach (var ui in charSelectionUIs)
            {
                RegisterUIComponent(ui, CHARACTER_UI_CATEGORY);
                Debug.Log($"📝 Đã đăng ký CharacterSelectionUI: {ui.name}");
            }
        }

        /// <summary>
        /// Đăng ký Game UI components
        /// </summary>
        private void RegisterGameUIComponents()
        {
            // TestCameraUI và các UI khác
            TestCameraUI[] testUIs = FindObjectsByType<TestCameraUI>(FindObjectsSortMode.None);
            foreach (var ui in testUIs)
            {
                RegisterUIComponent(ui, GAME_UI_CATEGORY);
            }

            // Các UI khác của game
        }

        /// <summary>
        /// Đăng ký UI component vào hệ thống
        /// </summary>
        public void RegisterUIComponent(MonoBehaviour component, string category, int cameraIndex = -1)
        {
            if (component == null) return;

            // Đăng ký theo category
            if (uiComponentsByCategory.ContainsKey(category))
            {
                if (!uiComponentsByCategory[category].Contains(component))
                {
                    uiComponentsByCategory[category].Add(component);
                }
            }

            // Đăng ký theo camera
            if (cameraIndex >= 0)
            {
                if (!uiComponentsByCamera.ContainsKey(cameraIndex))
                {
                    uiComponentsByCamera[cameraIndex] = new List<MonoBehaviour>();
                }
                if (!uiComponentsByCamera[cameraIndex].Contains(component))
                {
                    uiComponentsByCamera[cameraIndex].Add(component);
                }
            }
            else
            {
                // Global UI
                if (!globalUIComponents.Contains(component))
                {
                    globalUIComponents.Add(component);
                }
            }
        }

        /// <summary>
        /// Xử lý input actions
        /// </summary>
        private void HandleInputActions()
        {
            if (toggleAllUIAction.WasPressedThisFrame())
            {
                ToggleAllUI();
            }

            if (toggleCameraUIAction.WasPressedThisFrame())
            {
                ToggleCurrentCameraUI();
            }
        }

        /// <summary>
        /// Theo dõi thay đổi camera
        /// </summary>
        private void TrackCameraChanges()
        {
            if (cameraManager == null) return;

            int currentCameraIndex = cameraManager.LayChiSoCameraHienTai();
            if (currentCameraIndex != lastActiveCameraIndex)
            {
                OnCameraChanged(lastActiveCameraIndex, currentCameraIndex);
                lastActiveCameraIndex = currentCameraIndex;
            }
        }

        /// <summary>
        /// Xử lý khi camera thay đổi
        /// </summary>
        private void OnCameraChanged(int oldCameraIndex, int newCameraIndex)
        {
            Debug.Log($"📹 Camera changed: {oldCameraIndex} → {newCameraIndex}");

            // Ẩn UI của camera cũ
            if (oldCameraIndex >= 0 && uiComponentsByCamera.ContainsKey(oldCameraIndex))
            {
                SetCameraUIVisibility(oldCameraIndex, false);
            }

            // Hiện UI của camera mới nếu đang bật
            if (currentCameraUIVisible && newCameraIndex >= 0 && uiComponentsByCamera.ContainsKey(newCameraIndex))
            {
                SetCameraUIVisibility(newCameraIndex, true);
            }
        }

        /// <summary>
        /// Toggle toàn bộ UI
        /// </summary>
        [ContextMenu("🎛️ Toggle All UI")]
        public void ToggleAllUI()
        {
            allUIVisible = !allUIVisible;
            SetAllUIVisibility(allUIVisible);
            
            Debug.Log($"🎛️ Toggle All UI: {(allUIVisible ? "BẬT" : "TẮT")}");
        }

        /// <summary>
        /// Toggle UI camera hiện tại
        /// </summary>
        [ContextMenu("📹 Toggle Current Camera UI")]
        public void ToggleCurrentCameraUI()
        {
            currentCameraUIVisible = !currentCameraUIVisible;
            
            int currentCamera = cameraManager != null ? cameraManager.LayChiSoCameraHienTai() : 0;
            SetCameraUIVisibility(currentCamera, currentCameraUIVisible);
            
            Debug.Log($"📹 Toggle Camera {currentCamera} UI: {(currentCameraUIVisible ? "BẬT" : "TẮT")}");
        }

        /// <summary>
        /// Đặt hiển thị toàn bộ UI
        /// </summary>
        public void SetAllUIVisibility(bool visible)
        {
            allUIVisible = visible;
            
            // Cập nhật màu nút
            if (buttonImage != null)
            {
                buttonImage.color = visible ? activeColor : inactiveColor;
            }

            // Global UI
            SetGlobalUIVisibility(visible);
            
            // Category UI
            foreach (var category in uiComponentsByCategory.Keys)
            {
                SetCategoryUIVisibility(category, visible);
            }
            
            // Camera UI
            foreach (var cameraIndex in uiComponentsByCamera.Keys)
            {
                SetCameraUIVisibility(cameraIndex, visible);
            }
        }

        /// <summary>
        /// Đặt hiển thị UI theo category
        /// </summary>
        public void SetCategoryUIVisibility(string category, bool visible)
        {
            if (!uiComponentsByCategory.ContainsKey(category)) return;

            foreach (var component in uiComponentsByCategory[category])
            {
                SetComponentVisibility(component, visible);
            }
        }

        /// <summary>
        /// Đặt hiển thị UI theo camera
        /// </summary>
        public void SetCameraUIVisibility(int cameraIndex, bool visible)
        {
            if (!uiComponentsByCamera.ContainsKey(cameraIndex)) return;

            foreach (var component in uiComponentsByCamera[cameraIndex])
            {
                SetComponentVisibility(component, visible);
            }
        }

        /// <summary>
        /// Đặt hiển thị Global UI
        /// </summary>
        public void SetGlobalUIVisibility(bool visible)
        {
            foreach (var component in globalUIComponents)
            {
                SetComponentVisibility(component, visible);
            }
        }

        /// <summary>
        /// Đặt hiển thị cho component cụ thể
        /// </summary>
        private void SetComponentVisibility(MonoBehaviour component, bool visible)
        {
            if (component == null) return;

            // Xử lý các loại UI component khác nhau
            if (component is CameraSettingsUI cameraUI)
            {
                cameraUI.DatHienThiUI(visible);
            }
            else if (component is HealthBar healthBar)
            {
                healthBar.gameObject.SetActive(visible);
            }
            else if (component is AnimalRevolt.UI.CharacterSelectionUI charUI)
            {
                charUI.SetUIVisibility(visible);
            }
            else if (component is RagdollPhysicsController ragdollUI)
            {
                ragdollUI.gameObject.SetActive(visible);
            }
            else if (component is DamageNumberSpawner damageSpawner)
            {
                damageSpawner.gameObject.SetActive(visible);
            }
            else if (component.gameObject != null)
            {
                // Generic GameObject activation
                component.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Log thông tin components đã tìm thấy
        /// </summary>
        private void LogDiscoveredComponents()
        {
            Debug.Log("📊 === UI COMPONENTS DISCOVERED ===");
            
            foreach (var category in uiComponentsByCategory)
            {
                if (category.Value.Count > 0)
                {
                    Debug.Log($"📂 {category.Key}: {category.Value.Count} components");
                }
            }
            
            Debug.Log($"🌍 Global UI: {globalUIComponents.Count} components");
            Debug.Log($"📹 Camera UI: {uiComponentsByCamera.Count} camera groups");
        }

        /// <summary>
        /// Refresh và tìm lại tất cả UI
        /// </summary>
        [ContextMenu("🔄 Refresh All UI")]
        public void RefreshAllUI()
        {
            DiscoverAllUIComponents();
            Debug.Log("🔄 Đã refresh tất cả UI components");
        }

        /// <summary>
        /// Lấy trạng thái hiển thị UI
        /// </summary>
        public bool IsAllUIVisible() => allUIVisible;
        public bool IsCurrentCameraUIVisible() => currentCameraUIVisible;

        /// <summary>
        /// Đặt vị trí nút toggle
        /// </summary>
        public void SetButtonPosition(RectTransform.Edge position, Vector2 offset)
        {
            buttonPosition = position;
            buttonOffset = offset;
            
            if (toggleButton != null)
            {
                SetupButtonPosition(toggleButton.GetComponent<RectTransform>());
            }
        }

        private void OnDestroy()
        {
            toggleAllUIAction?.Dispose();
            toggleCameraUIAction?.Dispose();
        }

        #region Public API

        /// <summary>
        /// Bật toàn bộ UI
        /// </summary>
        public void ShowAllUI() => SetAllUIVisibility(true);

        /// <summary>
        /// Tắt toàn bộ UI
        /// </summary>
        public void HideAllUI() => SetAllUIVisibility(false);

        /// <summary>
        /// Bật UI category cụ thể
        /// </summary>
        public void ShowCategoryUI(string category) => SetCategoryUIVisibility(category, true);

        /// <summary>
        /// Tắt UI category cụ thể
        /// </summary>
        public void HideCategoryUI(string category) => SetCategoryUIVisibility(category, false);

        /// <summary>
        /// Bật UI camera cụ thể
        /// </summary>
        public void ShowCameraUI(int cameraIndex) => SetCameraUIVisibility(cameraIndex, true);

        /// <summary>
        /// Tắt UI camera cụ thể
        /// </summary>
        public void HideCameraUI(int cameraIndex) => SetCameraUIVisibility(cameraIndex, false);

        #endregion
    }
}