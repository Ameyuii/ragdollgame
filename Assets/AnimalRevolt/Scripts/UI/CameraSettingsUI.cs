using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// UI để điều chỉnh thông số camera trong game
    /// Hỗ trợ runtime adjustment cho tất cả camera systems
    /// </summary>
    public class CameraSettingsUI : MonoBehaviour
    {
        [Header("🎛️ UI Settings")]
        [SerializeField, Tooltip("Hiển thị UI khi start")]
        private bool hienThiKhiStart = false;
        
        [SerializeField, Tooltip("Vị trí UI trên màn hình")]
        private Rect viTriUI = new Rect(20, 20, 300, 400);
        
        [SerializeField, Tooltip("UIToggleManager để quản lý UI")]
        private UIToggleManager uiToggleManager;

        [Header("📹 Camera References")]
        [SerializeField, Tooltip("Camera Controller chính")]
        private AnimalRevolt.Camera.CameraController cameraController;
        
        [SerializeField, Tooltip("Camera Manager")]
        private AnimalRevolt.Camera.CameraManager cameraManager;

        [Header("⚙️ Shared Parameters")]
        [SerializeField, Tooltip("Tốc độ xoay NPC cameras")]
        private float tocDoXoayNPC = 120f;
        
        [SerializeField, Tooltip("Độ nhạy chuột NPC cameras")]
        private float doNhayChuotNPC = 2f;
        
        [SerializeField, Tooltip("Nhân tốc độ xoay nhanh NPC")]
        private float nhanTocDoXoayNhanhNPC = 2f;
        
        [SerializeField, Tooltip("Khoảng cách mặc định NPC")]
        private float khoangCachNPC = 5f;

        // UI State
        private bool hienThiUI;
        private Vector2 viTriCuon = Vector2.zero;
        private GUIStyle titleStyle, labelStyle, buttonStyle, boxStyle;

        private void Start()
        {
            hienThiUI = hienThiKhiStart;
            
            // Tự động tìm references nếu chưa gán
            if (cameraController == null)
            {
                cameraController = FindFirstObjectByType<AnimalRevolt.Camera.CameraController>();
            }
            
            if (cameraManager == null)
            {
                cameraManager = FindFirstObjectByType<AnimalRevolt.Camera.CameraManager>();
            }
            
            // Tự động tìm UIToggleManager nếu chưa gán
            if (uiToggleManager == null)
            {
                uiToggleManager = FindFirstObjectByType<UIToggleManager>();
            }
            
            // Load settings từ PlayerPrefs
            LoadSettings();
            
            Debug.Log("🎛️ CameraSettingsUI khởi tạo - quản lý bởi UIToggleManager");
        }

        private void Update()
        {
            // UI toggle hiện được quản lý bởi UIToggleManager
            // Không cần xử lý input trực tiếp ở đây nữa
        }

        private void OnGUI()
        {
            if (!hienThiUI) return;

            // Khởi tạo styles
            KhoiTaoGUIStyles();

            // Vẽ UI window
            viTriUI = GUI.Window(12345, viTriUI, VeNoiDungUI, "🎥 Camera Settings", boxStyle);
        }

        /// <summary>
        /// Vẽ nội dung UI
        /// </summary>
        private void VeNoiDungUI(int windowID)
        {
            viTriCuon = GUILayout.BeginScrollView(viTriCuon, GUILayout.Width(280), GUILayout.Height(360));
            
            // Main Camera Settings
            VeMainCameraSettings();
            
            GUILayout.Space(10);
            
            // NPC Camera Shared Settings
            VeNPCCameraSettings();
            
            GUILayout.Space(10);
            
            // Camera Mode Controls
            VeCameraModeControls();
            
            GUILayout.Space(10);
            
            // Utility Buttons
            VeUtilityButtons();
            
            GUILayout.EndScrollView();
            
            // Cho phép kéo window
            GUI.DragWindow();
        }

        /// <summary>
        /// Vẽ settings cho Main Camera
        /// </summary>
        private void VeMainCameraSettings()
        {
            GUILayout.Label("🎮 MAIN CAMERA", titleStyle);
            
            if (cameraController != null)
            {
                // Tốc độ xoay
                GUILayout.BeginHorizontal();
                GUILayout.Label("Tốc độ xoay:", GUILayout.Width(120));
                float tocDoXoay = GUILayout.HorizontalSlider(cameraController.LayTocDoXoay(), 50f, 300f, GUILayout.Width(100));
                GUILayout.Label($"{tocDoXoay:F0}°/s", GUILayout.Width(50));
                GUILayout.EndHorizontal();
                cameraController.DatTocDoXoay(tocDoXoay);

                // Độ nhạy chuột
                GUILayout.BeginHorizontal();
                GUILayout.Label("Độ nhạy chuột:", GUILayout.Width(120));
                float doNhayChuot = GUILayout.HorizontalSlider(cameraController.LayDoNhayChuot(), 0.5f, 10f, GUILayout.Width(100));
                GUILayout.Label($"{doNhayChuot:F1}", GUILayout.Width(50));
                GUILayout.EndHorizontal();
                cameraController.DatDoNhayChuot(doNhayChuot);

                // Tốc độ di chuyển
                GUILayout.BeginHorizontal();
                GUILayout.Label("Tốc độ di chuyển:", GUILayout.Width(120));
                float tocDoDiChuyen = GUILayout.HorizontalSlider(cameraController.LayTocDoChuyenDong(), 1f, 50f, GUILayout.Width(100));
                GUILayout.Label($"{tocDoDiChuyen:F0}", GUILayout.Width(50));
                GUILayout.EndHorizontal();
                cameraController.DatTocDoChuyenDong(tocDoDiChuyen);

                // Camera Mode hiện tại
                GUILayout.BeginHorizontal();
                GUILayout.Label("Mode hiện tại:", GUILayout.Width(120));
                GUILayout.Label($"{cameraController.LayCameraMode()}", labelStyle, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("❌ CameraController không tìm thấy", labelStyle);
            }
        }

        /// <summary>
        /// Vẽ settings cho NPC Cameras (shared parameters)
        /// </summary>
        private void VeNPCCameraSettings()
        {
            GUILayout.Label("📹 NPC CAMERAS (Shared)", titleStyle);
            
            // Tốc độ xoay NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tốc độ xoay:", GUILayout.Width(120));
            tocDoXoayNPC = GUILayout.HorizontalSlider(tocDoXoayNPC, 50f, 300f, GUILayout.Width(100));
            GUILayout.Label($"{tocDoXoayNPC:F0}°/s", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Độ nhạy chuột NPC
            GUILayout.BeginHorizontal();
            GUILayout.Label("Độ nhạy chuột:", GUILayout.Width(120));
            doNhayChuotNPC = GUILayout.HorizontalSlider(doNhayChuotNPC, 0.5f, 10f, GUILayout.Width(100));
            GUILayout.Label($"{doNhayChuotNPC:F1}", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Nhân tốc độ nhanh
            GUILayout.BeginHorizontal();
            GUILayout.Label("Boost tốc độ:", GUILayout.Width(120));
            nhanTocDoXoayNhanhNPC = GUILayout.HorizontalSlider(nhanTocDoXoayNhanhNPC, 1f, 5f, GUILayout.Width(100));
            GUILayout.Label($"x{nhanTocDoXoayNhanhNPC:F1}", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Khoảng cách mặc định
            GUILayout.BeginHorizontal();
            GUILayout.Label("Khoảng cách:", GUILayout.Width(120));
            khoangCachNPC = GUILayout.HorizontalSlider(khoangCachNPC, 2f, 20f, GUILayout.Width(100));
            GUILayout.Label($"{khoangCachNPC:F1}m", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Hiển thị số lượng NPC cameras
            AnimalRevolt.Camera.NPCCamera[] npcCameras = FindObjectsByType<AnimalRevolt.Camera.NPCCamera>(FindObjectsSortMode.None);
            GUILayout.Label($"📊 NPC Cameras: {npcCameras.Length}", labelStyle);
        }

        /// <summary>
        /// Vẽ controls cho Camera Modes
        /// </summary>
        private void VeCameraModeControls()
        {
            GUILayout.Label("🎥 CAMERA MODES", titleStyle);
            
            if (cameraController != null)
            {
                // Buttons để chuyển mode
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("🎮 FreeCam", buttonStyle))
                {
                    cameraController.DatCameraMode(AnimalRevolt.Camera.CameraController.CameraMode.FreeCam);
                }
                if (GUILayout.Button("🎯 Follow", buttonStyle))
                {
                    cameraController.DatCameraMode(AnimalRevolt.Camera.CameraController.CameraMode.Follow);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("🌍 Overview", buttonStyle))
                {
                    cameraController.DatCameraMode(AnimalRevolt.Camera.CameraController.CameraMode.Overview);
                }
                if (GUILayout.Button("🔄 Orbital", buttonStyle))
                {
                    cameraController.DatCameraMode(AnimalRevolt.Camera.CameraController.CameraMode.Orbital);
                }
                GUILayout.EndHorizontal();
                
                // Reset camera button
                if (GUILayout.Button("🏠 Reset Camera", buttonStyle))
                {
                    cameraController.ResetCamera();
                }
            }

            // Camera Manager controls
            if (cameraManager != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("📹 Camera Switching:", labelStyle);
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("0: Main", buttonStyle))
                {
                    cameraManager.BatCameraChinh();
                }
                if (GUILayout.Button("1: NPC", buttonStyle))
                {
                    cameraManager.ChuyenSangNPCCamera(0);
                }
                if (GUILayout.Button("Tab: Next", buttonStyle))
                {
                    cameraManager.ChuyenCameraKeTiep();
                }
                GUILayout.EndHorizontal();
                
                // Hiển thị camera hiện tại
                GUILayout.Label($"📊 Camera: {cameraManager.LayChiSoCameraHienTai()}", labelStyle);
            }
        }

        /// <summary>
        /// Vẽ utility buttons
        /// </summary>
        private void VeUtilityButtons()
        {
            GUILayout.Label("🔧 UTILITIES", titleStyle);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Save", buttonStyle))
            {
                SaveSettings();
                Debug.Log("💾 Camera settings saved");
            }
            if (GUILayout.Button("📁 Load", buttonStyle))
            {
                LoadSettings();
                Debug.Log("📁 Camera settings loaded");
            }
            if (GUILayout.Button("🔄 Reset", buttonStyle))
            {
                ResetToDefaults();
                Debug.Log("🔄 Camera settings reset to defaults");
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.Label("💡 Phím tắt:", labelStyle);
            GUILayout.Label("• F1: Toggle UI", labelStyle);
            GUILayout.Label("• C: Chuyển camera mode", labelStyle);
            GUILayout.Label("• 0-9: Chuyển camera", labelStyle);
            GUILayout.Label("• Home: Reset camera", labelStyle);
        }

        /// <summary>
        /// Khởi tạo GUI styles
        /// </summary>
        private void KhoiTaoGUIStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12,
                    normal = { textColor = Color.yellow }
                };
            }

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 10,
                    normal = { textColor = Color.white }
                };
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 9
                };
            }

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.window)
                {
                    normal = { background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.8f)) }
                };
            }
        }

        /// <summary>
        /// Tạo texture cho background
        /// </summary>
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        #region Settings Management

        /// <summary>
        /// Lưu settings vào PlayerPrefs
        /// </summary>
        private void SaveSettings()
        {
            if (cameraController != null)
            {
                PlayerPrefs.SetFloat("CameraController_TocDoXoay", cameraController.LayTocDoXoay());
                PlayerPrefs.SetFloat("CameraController_DoNhayChuot", cameraController.LayDoNhayChuot());
                PlayerPrefs.SetFloat("CameraController_TocDoDiChuyen", cameraController.LayTocDoChuyenDong());
            }
            
            PlayerPrefs.SetFloat("NPCCamera_TocDoXoay", tocDoXoayNPC);
            PlayerPrefs.SetFloat("NPCCamera_DoNhayChuot", doNhayChuotNPC);
            PlayerPrefs.SetFloat("NPCCamera_NhanTocDoNhanh", nhanTocDoXoayNhanhNPC);
            PlayerPrefs.SetFloat("NPCCamera_KhoangCach", khoangCachNPC);
            
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load settings từ PlayerPrefs
        /// </summary>
        private void LoadSettings()
        {
            if (cameraController != null)
            {
                cameraController.DatTocDoXoay(PlayerPrefs.GetFloat("CameraController_TocDoXoay", 150f));
                cameraController.DatDoNhayChuot(PlayerPrefs.GetFloat("CameraController_DoNhayChuot", 3f));
                cameraController.DatTocDoChuyenDong(PlayerPrefs.GetFloat("CameraController_TocDoDiChuyen", 10f));
            }
            
            tocDoXoayNPC = PlayerPrefs.GetFloat("NPCCamera_TocDoXoay", 120f);
            doNhayChuotNPC = PlayerPrefs.GetFloat("NPCCamera_DoNhayChuot", 2f);
            nhanTocDoXoayNhanhNPC = PlayerPrefs.GetFloat("NPCCamera_NhanTocDoNhanh", 2f);
            khoangCachNPC = PlayerPrefs.GetFloat("NPCCamera_KhoangCach", 5f);
        }

        /// <summary>
        /// Reset về giá trị mặc định
        /// </summary>
        private void ResetToDefaults()
        {
            if (cameraController != null)
            {
                cameraController.DatTocDoXoay(150f);
                cameraController.DatDoNhayChuot(3f);
                cameraController.DatTocDoChuyenDong(10f);
            }
            
            tocDoXoayNPC = 120f;
            doNhayChuotNPC = 2f;
            nhanTocDoXoayNhanhNPC = 2f;
            khoangCachNPC = 5f;
        }

        #endregion

        #region Public API for Shared Parameters

        public float LayTocDoXoayNPC() => tocDoXoayNPC;
        public float LayDoNhayChuotNPC() => doNhayChuotNPC;
        public float LayNhanTocDoXoayNhanhNPC() => nhanTocDoXoayNhanhNPC;
        public float LayKhoangCachNPC() => khoangCachNPC;

        public void DatTocDoXoayNPC(float tocDo) => tocDoXoayNPC = Mathf.Max(10f, tocDo);
        public void DatDoNhayChuotNPC(float doNhay) => doNhayChuotNPC = Mathf.Max(0.1f, doNhay);
        public void DatNhanTocDoXoayNhanhNPC(float nhan) => nhanTocDoXoayNhanhNPC = Mathf.Max(1f, nhan);
        public void DatKhoangCachNPC(float khoangCach) => khoangCachNPC = Mathf.Max(1f, khoangCach);

        /// <summary>
        /// Toggle hiển thị UI
        /// </summary>
        public void ToggleUI()
        {
            hienThiUI = !hienThiUI;
        }

        /// <summary>
        /// Đặt hiển thị UI
        /// </summary>
        public void DatHienThiUI(bool hienThi)
        {
            hienThiUI = hienThi;
        }

        #endregion

        private void OnDestroy()
        {
            // Input handling hiện được quản lý bởi UIToggleManager
            // Không cần dispose input action ở đây nữa
        }
    }
}