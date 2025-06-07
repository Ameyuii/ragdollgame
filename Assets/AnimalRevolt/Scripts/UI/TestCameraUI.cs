using UnityEngine;
using AnimalRevolt.Camera;
using AnimalRevolt.UI;

/// <summary>
/// Script test để verify UI Camera Settings hoạt động đúng
/// Tích hợp đầy đủ các chức năng camera testing và validation
/// </summary>
public class TestCameraUI : MonoBehaviour
{
    [Header("🔧 Camera Testing")]
    [SerializeField, Tooltip("Tự động test camera systems khi start")]
    private bool autoTestOnStart = true;
    
    [SerializeField, Tooltip("Hiển thị detailed logs")]
    private bool showDetailedLogs = true;
    
    [SerializeField, Tooltip("Test camera switching performance")]
    private bool testCameraSwitching = true;
    
    [SerializeField, Tooltip("Validate AudioListener management")]
    private bool validateAudioListener = true;

    void Start()
    {
        if (autoTestOnStart)
        {
            TestCameraSystem();
        }
    }

    /// <summary>
    /// Test toàn bộ camera system
    /// </summary>
    public void TestCameraSystem()
    {
        Debug.Log("🔧 Camera UI Test Started - Extended Version");
        
        // Test DieuChinhThongSoCamera
        TestDieuChinhThongSoCamera();
        
        // Test QuanLyCamera
        TestQuanLyCamera();
        
        // Test NPCCamera functionality
        TestNPCCameraFunctionality();
        
        // Test CameraController
        TestCameraController();
        
        // Test AudioListener management nếu enabled
        if (validateAudioListener)
        {
            TestAudioListenerManagement();
        }
        
        // Performance test nếu enabled
        if (testCameraSwitching)
        {
            StartCoroutine(TestCameraSwitchingPerformance());
        }
        
        LogCameraInstructions();
    }

    /// <summary>
    /// Test CameraSettingsUI system
    /// </summary>
    private void TestDieuChinhThongSoCamera()
    {
        CameraSettingsUI cameraUI = FindFirstObjectByType<CameraSettingsUI>();
        if (cameraUI != null)
        {
            Debug.Log("✅ CameraSettingsUI found in scene");
            
            if (showDetailedLogs)
            {
                Debug.Log($"📊 Camera UI Component: {cameraUI.name}");
                Debug.Log("🎛️ Camera settings panel ready for runtime adjustment");
            }
        }
        else
        {
            Debug.LogWarning("❌ CameraSettingsUI NOT found in scene");
            Debug.LogWarning("💡 Add CameraSettingsUI component to enable runtime camera adjustment");
        }
    }

    /// <summary>
    /// Test CameraManager system
    /// </summary>
    private void TestQuanLyCamera()
    {
        CameraManager quanLyCamera = FindFirstObjectByType<CameraManager>();
        if (quanLyCamera != null)
        {
            Debug.Log("✅ CameraManager found in scene");
            
            if (showDetailedLogs)
            {
                Debug.Log($"🎯 Camera Manager: {quanLyCamera.name}");
                Debug.Log("⌨️ Camera switching keys: 0 (Main Camera), 1 (NPC Camera)");
            }
        }
        else
        {
            Debug.LogWarning("❌ CameraManager NOT found in scene");
            Debug.LogWarning("💡 Add CameraManager component to enable camera switching");
        }
    }

    /// <summary>
    /// Test NPC Camera functionality
    /// </summary>
    private void TestNPCCameraFunctionality()
    {
        NPCCamera[] npcCameras = FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
        
        if (npcCameras.Length > 0)
        {
            Debug.Log($"✅ Found {npcCameras.Length} NPC Camera(s) in scene");
            
            if (showDetailedLogs)
            {
                foreach (NPCCamera npcCam in npcCameras)
                {
                    if (npcCam != null && npcCam.GetCamera() != null)
                    {
                        Debug.Log($"🎯 NPC Camera: {npcCam.name} | Camera: {npcCam.GetCamera().name}");
                        Debug.Log($"   📏 Distance: {npcCam.LayKhoangCach():F1} | Sensitivity: {npcCam.LayDoNhayChuot():F1}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("❌ No NPC Cameras found in scene");
            Debug.LogWarning("💡 Add NPCCamera components to GameObjects to enable NPC camera views");
        }
    }

    /// <summary>
    /// Test CameraController functionality
    /// </summary>
    private void TestCameraController()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraController cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController != null)
            {
                Debug.Log("✅ CameraController found on main camera");
                
                if (showDetailedLogs)
                {
                    Debug.Log($"⚡ Rotation Speed: {cameraController.LayTocDoXoay():F0}°/s");
                    Debug.Log($"🚀 Boost Multiplier: x{cameraController.LayNhanTocDoXoayNhanh():F1}");
                    Debug.Log($"🖱️ Mouse Sensitivity: {cameraController.LayDoNhayChuot():F1}");
                    Debug.Log($"🏃 Movement Speed: {cameraController.LayTocDoChuyenDong():F0}");
                }
            }
            else
            {
                Debug.LogWarning("❌ CameraController NOT found on main camera");
                Debug.LogWarning("💡 Add CameraController component to main camera for free camera movement");
            }
        }
        else
        {
            Debug.LogError("❌ Main Camera not found in scene!");
            Debug.LogWarning("💡 Please ensure there is a Camera with 'MainCamera' tag in the scene");
            
            // Try to find any camera as fallback
            Camera anyCamera = FindFirstObjectByType<Camera>();
            if (anyCamera != null)
            {
                Debug.Log($"🔍 Found alternative camera: {anyCamera.name} - consider tagging it as MainCamera");
            }
        }
    }

    /// <summary>
    /// Test AudioListener management
    /// </summary>
    private void TestAudioListenerManagement()
    {
        AudioListener[] allAudioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int enabledListeners = 0;
        
        foreach (AudioListener listener in allAudioListeners)
        {
            if (listener.enabled)
                enabledListeners++;
        }
        
        if (enabledListeners == 1)
        {
            Debug.Log($"✅ AudioListener management OK: {enabledListeners}/1 enabled");
        }
        else if (enabledListeners > 1)
        {
            Debug.LogWarning($"⚠️ Multiple AudioListeners enabled: {enabledListeners}/{allAudioListeners.Length}");
            Debug.LogWarning("💡 QuanLyCamera should auto-manage AudioListeners to prevent conflicts");
        }
        else
        {
            Debug.LogWarning("❌ No AudioListener enabled - audio may not work");
        }
        
        if (showDetailedLogs)
        {
            Debug.Log($"🔊 Total AudioListeners in scene: {allAudioListeners.Length}");
        }
    }

    /// <summary>
    /// Test camera switching performance
    /// </summary>
    private System.Collections.IEnumerator TestCameraSwitchingPerformance()
    {
        yield return new WaitForSeconds(2f); // Wait for initial setup
        
        CameraManager quanLyCamera = FindFirstObjectByType<CameraManager>();
        if (quanLyCamera == null)
        {
            Debug.LogWarning("❌ Cannot test camera switching - CameraManager not found");
            yield break;
        }
        
        Debug.Log("🔄 Testing camera switching performance...");
        
        float startTime = Time.time;
        
        // Test switching to main camera
        quanLyCamera.BatCameraChinh();
        yield return new WaitForSeconds(0.1f);
        
        // Test switching to NPC camera
        quanLyCamera.ChuyenCameraKeTiep();
        yield return new WaitForSeconds(0.1f);
        
        // Switch back to main
        quanLyCamera.BatCameraChinh();
        
        float endTime = Time.time;
        float switchTime = endTime - startTime;
        
        Debug.Log($"✅ Camera switching test completed in {switchTime:F3}s");
        
        if (switchTime < 0.5f)
        {
            Debug.Log("🚀 Camera switching performance: EXCELLENT");
        }
        else if (switchTime < 1f)
        {
            Debug.Log("👍 Camera switching performance: GOOD");
        }
        else
        {
            Debug.LogWarning("⚠️ Camera switching performance: SLOW - consider optimization");
        }
    }

    /// <summary>
    /// Kiểm tra NPCRagdollManager UI
    /// </summary>
    private void CheckNPCRagdollManager()
    {
        // Tìm NPCRagdollManager nếu có trong project
        var ragdollManagerType = System.Type.GetType("NPCRagdollManager");
        if (ragdollManagerType != null)
        {
            var ragdollManager = FindFirstObjectByType(ragdollManagerType);
            if (ragdollManager != null)
            {
                Debug.Log("✅ NPCRagdollManager found - UI debug should be OFF");
                
                if (showDetailedLogs)
                {
                    Debug.Log($"🎮 Ragdoll Manager: {((MonoBehaviour)ragdollManager).name}");
                }
            }
            else
            {
                Debug.Log("💡 NPCRagdollManager not found in scene (optional)");
            }
        }
        else
        {
            Debug.Log("💡 NPCRagdollManager class not found in project (optional)");
        }
    }

    /// <summary>
    /// Log hướng dẫn camera
    /// </summary>
    private void LogCameraInstructions()
    {
        Debug.Log("=== 🎮 CAMERA INSTRUCTIONS ===");
        Debug.Log("🎛️ Look for Camera UI icon in top-right corner of screen");
        Debug.Log("📹 Click the camera icon to toggle Camera Settings panel");
        Debug.Log("⌨️ Press '0' for Main Camera, '1' for NPC Camera");
        Debug.Log("🖱️ Right-click + drag to rotate camera");
        Debug.Log("🎯 WASD to move camera (Main Camera mode)");
        Debug.Log("⚡ Hold Shift for faster movement/rotation");
        Debug.Log("💫 Scroll wheel to zoom (NPC Camera mode)");
        Debug.Log("=================================");
    }

    /// <summary>
    /// Manual test trigger từ Inspector
    /// </summary>
    [ContextMenu("🔧 Run Camera System Test")]
    public void ManualTest()
    {
        TestCameraSystem();
    }

    /// <summary>
    /// Toggle detailed logs
    /// </summary>
    [ContextMenu("📊 Toggle Detailed Logs")]
    public void ToggleDetailedLogs()
    {
        showDetailedLogs = !showDetailedLogs;
        Debug.Log($"📊 Detailed Logs: {(showDetailedLogs ? "ON" : "OFF")}");
    }

    /// <summary>
    /// Quick camera system validation
    /// </summary>
    [ContextMenu("⚡ Quick Camera Validation")]
    public void QuickValidation()
    {
        Debug.Log("⚡ Quick Camera System Validation:");
        
        bool hasMainCamera = Camera.main != null;
        bool hasQuanLyCamera = FindFirstObjectByType<CameraManager>() != null;
        bool hasCameraUI = FindFirstObjectByType<CameraSettingsUI>() != null;
        bool hasNPCCameras = FindObjectsByType<NPCCamera>(FindObjectsSortMode.None).Length > 0;
        
        Debug.Log($"📷 Main Camera: {(hasMainCamera ? "✅" : "❌")}");
        Debug.Log($"🎯 Camera Manager: {(hasQuanLyCamera ? "✅" : "❌")}");
        Debug.Log($"🎛️ Camera UI: {(hasCameraUI ? "✅" : "❌")}");
        Debug.Log($"📹 NPC Cameras: {(hasNPCCameras ? "✅" : "❌")}");
        
        int score = (hasMainCamera ? 1 : 0) + (hasQuanLyCamera ? 1 : 0) +
                   (hasCameraUI ? 1 : 0) + (hasNPCCameras ? 1 : 0);
        
        Debug.Log($"🏆 Camera System Score: {score}/4");
        
        if (score == 4)
            Debug.Log("🌟 PERFECT! Camera system is fully functional");
        else if (score >= 2)
            Debug.Log("👍 GOOD! Camera system is mostly functional");
        else
            Debug.Log("⚠️ WARNING! Camera system needs setup");
    }
}