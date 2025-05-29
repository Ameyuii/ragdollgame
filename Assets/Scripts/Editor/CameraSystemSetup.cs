using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script để thiết lập hệ thống camera nâng cao một cách tự động
/// Menu: Tools/Camera System/Setup Enhanced Camera System
/// </summary>
public static class CameraSystemSetup
{
    [MenuItem("Tools/Camera System/Setup Enhanced Camera System")]
    public static void SetupEnhancedCameraSystem()
    {
        Debug.Log("🚀 Bắt đầu thiết lập hệ thống camera nâng cao...");
        
        bool hasChanges = false;
        
        // 1. Thiết lập Main Camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("❌ Không tìm thấy Main Camera! Vui lòng tạo camera với tag MainCamera.");
            return;
        }
        
        // Thêm CameraController nếu chưa có
        var cameraController = mainCamera.GetComponent<CameraController>();
        if (cameraController == null)
        {
            mainCamera.gameObject.AddComponent<CameraController>();
            Debug.Log("✅ Đã thêm CameraController vào Main Camera");
            hasChanges = true;
        }
        
        // 2. Thiết lập QuanLyCamera
        QuanLyCamera quanLyCamera = Object.FindFirstObjectByType<QuanLyCamera>();
        if (quanLyCamera == null)
        {
            GameObject cameraManager = new GameObject("CameraManager");
            quanLyCamera = cameraManager.AddComponent<QuanLyCamera>();
            Debug.Log("✅ Đã tạo CameraManager với QuanLyCamera");
            hasChanges = true;
        }
          // 3. Thiết lập TestCameraSystem
        var testSystemType = System.Type.GetType("TestCameraSystem");
        if (testSystemType != null)
        {
            var testSystem = Object.FindFirstObjectByType(testSystemType) as MonoBehaviour;
            if (testSystem == null)
            {
                GameObject testObject = new GameObject("CameraSystemTester");
                testObject.AddComponent(testSystemType);
                Debug.Log("✅ Đã tạo CameraSystemTester");
                hasChanges = true;
            }
        }
        
        // 4. Kiểm tra Input System
        CheckInputSystem();
        
        // 5. Tạo NPC test nếu không có NPC nào
        NPCCamera[] existingNPCs = Object.FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
        if (existingNPCs.Length == 0)
        {
            if (EditorUtility.DisplayDialog("Tạo NPC Test", 
                "Không tìm thấy NPC nào có NPCCamera. Bạn có muốn tạo NPC test không?", 
                "Có", "Không"))
            {
                CreateTestNPC();
                hasChanges = true;
            }
        }
        
        if (hasChanges)
        {
            EditorUtility.SetDirty(quanLyCamera);
            Debug.Log("🎉 Thiết lập hệ thống camera hoàn tất!");
            Debug.Log("📚 Xem file CAMERA_SYSTEM_ENHANCED_GUIDE.md để biết thêm chi tiết");
        }
        else
        {
            Debug.Log("ℹ️ Hệ thống camera đã được thiết lập sẵn");
        }
        
        // Hiển thị hướng dẫn
        ShowSetupGuide();
    }
    
    [MenuItem("Tools/Camera System/Create Test NPC")]
    public static void CreateTestNPC()
    {
        // Tạo NPC test
        GameObject npc = new GameObject("NPC_Test");
        npc.transform.position = new Vector3(5, 0, 5);
        
        // Thêm visual
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(npc.transform);
        visual.transform.localPosition = Vector3.up * 0.5f;
        visual.name = "Visual";
        
        // Thêm NPCCamera
        npc.AddComponent<NPCCamera>();
        
        // Thêm NPCController nếu có
        var npcControllerType = System.Type.GetType("NPCController");
        if (npcControllerType != null)
        {
            npc.AddComponent(npcControllerType);
        }
        
        Selection.activeGameObject = npc;
        Debug.Log("✅ Đã tạo NPC test tại vị trí (5, 0, 5)");
    }
    
    [MenuItem("Tools/Camera System/Validate Camera System")]
    public static void ValidateCameraSystem()
    {
        Debug.Log("🔍 Kiểm tra hệ thống camera...");
        
        // Kiểm tra Main Camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("❌ Không tìm thấy Main Camera");
            return;
        }
        
        var cameraController = mainCamera.GetComponent<CameraController>();
        if (cameraController == null)
        {
            Debug.LogWarning("⚠️ Main Camera chưa có CameraController");
        }
        else
        {
            Debug.Log("✅ Main Camera có CameraController");
        }
        
        // Kiểm tra QuanLyCamera
        QuanLyCamera quanLyCamera = Object.FindFirstObjectByType<QuanLyCamera>();
        if (quanLyCamera == null)
        {
            Debug.LogWarning("⚠️ Chưa có QuanLyCamera trong scene");
        }
        else
        {
            Debug.Log("✅ QuanLyCamera đã được thiết lập");
        }
        
        // Kiểm tra NPCCamera
        NPCCamera[] npcCameras = Object.FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
        if (npcCameras.Length == 0)
        {
            Debug.LogWarning("⚠️ Không có NPCCamera nào trong scene");
        }
        else
        {
            Debug.Log($"✅ Tìm thấy {npcCameras.Length} NPCCamera");
        }
        
        // Kiểm tra AudioListener
        AudioListener[] audioListeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        int enabledCount = 0;
        foreach (var listener in audioListeners)
        {
            if (listener.enabled) enabledCount++;
        }
        
        if (enabledCount > 1)
        {
            Debug.LogWarning($"⚠️ Có {enabledCount} AudioListener đang hoạt động (nên chỉ có 1)");
        }
        else if (enabledCount == 1)
        {
            Debug.Log("✅ AudioListener đang hoạt động bình thường");
        }
        else
        {
            Debug.LogWarning("⚠️ Không có AudioListener nào hoạt động");
        }
        
        Debug.Log("🔍 Kiểm tra hoàn tất");
    }
    
    private static void CheckInputSystem()
    {
        // Kiểm tra Input System package
        var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.InputSystem, Unity.InputSystem");
        if (inputSystemType == null)
        {
            Debug.LogWarning("⚠️ Unity Input System chưa được cài đặt");
            Debug.Log("💡 Cài đặt qua Package Manager: Window > Package Manager > Unity Registry > Input System");
        }
        else
        {
            Debug.Log("✅ Unity Input System đã được cài đặt");
        }
        
        // Kiểm tra Input Actions file
        string[] guids = AssetDatabase.FindAssets("InputSystem_Actions t:InputActionAsset");
        if (guids.Length == 0)
        {
            Debug.LogWarning("⚠️ Không tìm thấy InputSystem_Actions.inputactions");
        }
        else
        {
            Debug.Log("✅ InputSystem_Actions.inputactions đã tồn tại");
        }
    }
    
    private static void ShowSetupGuide()
    {
        bool showGuide = EditorUtility.DisplayDialog(
            "Hệ thống Camera đã sẵn sàng!", 
            "Thiết lập hoàn tất! Bạn có muốn xem hướng dẫn sử dụng không?",
            "Xem hướng dẫn", 
            "Đóng"
        );
        
        if (showGuide)
        {
            string guidePath = "Assets/CAMERA_SYSTEM_ENHANCED_GUIDE.md";
            var guide = AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath);
            if (guide != null)
            {
                Selection.activeObject = guide;
                EditorGUIUtility.PingObject(guide);
            }
            else
            {
                Debug.Log("📚 Xem file CAMERA_SYSTEM_ENHANCED_GUIDE.md để biết hướng dẫn chi tiết");
            }
        }
    }
}
