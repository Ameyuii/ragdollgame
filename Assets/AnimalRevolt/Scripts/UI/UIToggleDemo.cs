using UnityEngine;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// Demo script để hướng dẫn sử dụng UIToggleManager
    /// Tạo và cấu hình hệ thống UI toggle ở góc màn hình
    /// </summary>
    public class UIToggleDemo : MonoBehaviour
    {
        [Header("🎮 Demo Settings")]
        [SerializeField, Tooltip("Tự động setup UIToggleManager")]
        private bool autoSetup = true;
        
        [SerializeField, Tooltip("Tạo icon camera mặc định")]
        private bool taoIconCamera = true;
        
        [SerializeField, Tooltip("Vị trí nút toggle")]
        private RectTransform.Edge viTriNut = RectTransform.Edge.Top;

        private UIToggleManager uiToggleManager;

        private void Start()
        {
            if (autoSetup)
            {
                SetupUIToggleSystem();
            }
        }

        /// <summary>
        /// Thiết lập hệ thống UI Toggle
        /// </summary>
        [ContextMenu("🎛️ Setup UI Toggle System")]
        public void SetupUIToggleSystem()
        {
            Debug.Log("🎛️ Bắt đầu thiết lập UI Toggle System...");

            // Tìm hoặc tạo UIToggleManager
            uiToggleManager = FindFirstObjectByType<UIToggleManager>();
            
            if (uiToggleManager == null)
            {
                Debug.Log("📱 Tạo UIToggleManager mới...");
                GameObject managerObj = new GameObject("UIToggleManager");
                uiToggleManager = managerObj.AddComponent<UIToggleManager>();
            }
            else
            {
                Debug.Log("✅ UIToggleManager đã tồn tại");
            }

            // Tạo icon camera nếu cần
            if (taoIconCamera)
            {
                TaoIconCamera();
            }

            // Thiết lập vị trí nút
            uiToggleManager.DatViTriNut(viTriNut, new Vector2(20, 20));

            Debug.Log("🎉 UI Toggle System đã được thiết lập!");
            LogHuongDanSuDung();
        }

        /// <summary>
        /// Tạo icon camera đơn giản
        /// </summary>
        private void TaoIconCamera()
        {
            // Tạo texture đơn giản cho icon camera
            Texture2D iconTexture = TaoIconCameraTexture();
            Sprite iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
            
            // Lưu sprite (optional)
            // AssetDatabase.CreateAsset(iconSprite, "Assets/AnimalRevolt/UI/Icons/CameraIcon.asset");
            
            Debug.Log("📸 Icon camera đã được tạo");
        }

        /// <summary>
        /// Tạo texture đơn giản cho icon camera
        /// </summary>
        private Texture2D TaoIconCameraTexture()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            
            // Vẽ icon camera đơn giản (hình chữ nhật với lens)
            Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            Color cameraColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            Color lensColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            // Fill background
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    // Vẽ body camera (hình chữ nhật)
                    if (x >= 8 && x < size - 8 && y >= 16 && y < size - 8)
                    {
                        texture.SetPixel(x, y, cameraColor);
                    }
                    // Vẽ lens (hình tròn)
                    else if (Vector2.Distance(new Vector2(x, y), new Vector2(size/2, size/2)) < 12)
                    {
                        texture.SetPixel(x, y, lensColor);
                    }
                    // Vẽ flash (hình vuông nhỏ)
                    else if (x >= size - 16 && x < size - 8 && y >= size - 12 && y < size - 4)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Log hướng dẫn sử dụng
        /// </summary>
        private void LogHuongDanSuDung()
        {
            Debug.Log("=== 🎛️ HƯỚNG DẪN SỬ DỤNG UI TOGGLE ===");
            Debug.Log("📱 Tìm nút camera ở góc màn hình (mặc định: góc trên-phải)");
            Debug.Log("🖱️ Click nút camera để bật/tắt Camera Settings UI");
            Debug.Log("⌨️ Phím tắt: F1 để toggle UI");
            Debug.Log("🎯 Khi UI bật: nút sẽ có màu xanh lá");
            Debug.Log("⚫ Khi UI tắt: nút sẽ có màu xám");
            Debug.Log("🎮 UI Camera bao gồm:");
            Debug.Log("  • Main Camera controls (rotation, sensitivity, movement)");
            Debug.Log("  • NPC Camera settings (shared parameters)");
            Debug.Log("  • Camera mode switching (FreeCam, Follow, Overview, Orbital)");
            Debug.Log("  • Camera switching controls (Main/NPC cameras)");
            Debug.Log("  • Save/Load/Reset settings");
            Debug.Log("=====================================");
        }

        /// <summary>
        /// Test toggle UI
        /// </summary>
        [ContextMenu("🧪 Test Toggle UI")]
        public void TestToggleUI()
        {
            if (uiToggleManager != null)
            {
                uiToggleManager.ToggleAllUI();
                Debug.Log($"🧪 Test Toggle: UI hiện tại {(uiToggleManager.DangHienThiUI() ? "BẬT" : "TẮT")}");
            }
            else
            {
                Debug.LogWarning("❌ UIToggleManager không tìm thấy. Hãy chạy Setup UI Toggle System trước.");
            }
        }

        /// <summary>
        /// Thay đổi vị trí nút toggle
        /// </summary>
        [ContextMenu("📍 Change Button Position")]
        public void ThayDoiViTriNut()
        {
            if (uiToggleManager != null)
            {
                // Cycle through different positions
                switch (viTriNut)
                {
                    case RectTransform.Edge.Top:
                        viTriNut = RectTransform.Edge.Right;
                        break;
                    case RectTransform.Edge.Right:
                        viTriNut = RectTransform.Edge.Bottom;
                        break;
                    case RectTransform.Edge.Bottom:
                        viTriNut = RectTransform.Edge.Left;
                        break;
                    case RectTransform.Edge.Left:
                        viTriNut = RectTransform.Edge.Top;
                        break;
                }
                
                uiToggleManager.DatViTriNut(viTriNut, new Vector2(20, 20));
                Debug.Log($"📍 Vị trí nút toggle đã thay đổi thành: {viTriNut}");
            }
        }

        /// <summary>
        /// Hiển thị trạng thái hệ thống
        /// </summary>
        [ContextMenu("📊 Show System Status")]
        public void HienThiTrangThaiHeThong()
        {
            Debug.Log("=== 📊 TRẠNG THÁI UI TOGGLE SYSTEM ===");
            
            // UIToggleManager
            UIToggleManager manager = FindFirstObjectByType<UIToggleManager>();
            Debug.Log($"🎛️ UIToggleManager: {(manager != null ? "✅ Có" : "❌ Không")}");
            if (manager != null)
            {
                Debug.Log($"📱 UI đang hiển thị: {(manager.DangHienThiUI() ? "✅ Có" : "❌ Không")}");
            }

            // CameraSettingsUI
            CameraSettingsUI[] cameraUIs = FindObjectsByType<CameraSettingsUI>(FindObjectsSortMode.None);
            Debug.Log($"📹 CameraSettingsUI components: {cameraUIs.Length}");

            // Camera components
            var cameraController = FindFirstObjectByType<AnimalRevolt.Camera.CameraController>();
            var cameraManager = FindFirstObjectByType<AnimalRevolt.Camera.CameraManager>();
            var npcCameras = FindObjectsByType<AnimalRevolt.Camera.NPCCamera>(FindObjectsSortMode.None);
            
            Debug.Log($"🎮 CameraController: {(cameraController != null ? "✅" : "❌")}");
            Debug.Log($"🎯 CameraManager: {(cameraManager != null ? "✅" : "❌")}");
            Debug.Log($"📸 NPC Cameras: {npcCameras.Length}");
            
            Debug.Log("===================================");
        }

        private void Update()
        {
            // Optional: Hiển thị trạng thái UI trong console với interval
            if (Time.frameCount % 300 == 0) // Mỗi 5 giây (60fps * 5)
            {
                if (uiToggleManager != null && uiToggleManager.DangHienThiUI())
                {
                    Debug.Log($"🎛️ UI Toggle Status: {(uiToggleManager.DangHienThiUI() ? "ACTIVE" : "INACTIVE")}");
                }
            }
        }
    }
}