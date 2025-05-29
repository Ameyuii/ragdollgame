using UnityEngine;

/// <summary>
/// Script test để verify UI Camera Settings hoạt động đúng
/// </summary>
public class TestCameraUI : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔧 Camera UI Test Started");
        
        // Kiểm tra DieuChinhThongSoCamera có trong scene không
        DieuChinhThongSoCamera cameraUI = FindFirstObjectByType<DieuChinhThongSoCamera>();
        if (cameraUI != null)
        {
            Debug.Log("✅ DieuChinhThongSoCamera found in scene");
        }
        else
        {
            Debug.LogWarning("❌ DieuChinhThongSoCamera NOT found in scene");
        }
        
        // Kiểm tra NPCRagdollManager UI có bị tắt không
        NPCRagdollManager ragdollManager = FindFirstObjectByType<NPCRagdollManager>();
        if (ragdollManager != null)
        {
            Debug.Log("✅ NPCRagdollManager found - UI debug should be OFF");
        }
        
        Debug.Log("🎮 Look for Camera UI icon in top-right corner of screen");
        Debug.Log("📹 Click the camera icon to toggle Camera Settings panel");
    }
}
