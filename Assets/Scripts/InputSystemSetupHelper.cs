using UnityEngine;

/// <summary>
/// Helper để hướng dẫn setup Input System cho Unity 6.2
/// </summary>
public class InputSystemSetupHelper : MonoBehaviour
{
    [Header("🎮 Input System Setup")]
    [SerializeField, Tooltip("Hiển thị hướng dẫn khi start")]
    private bool showInstructionsOnStart = true;
    
    [SerializeField, Tooltip("Hiển thị warning mỗi 10 giây")]
    private bool showPeriodicWarning = true;
    
    private float nextWarningTime = 10f;

    void Start()
    {
        if (showInstructionsOnStart)
        {
            ShowSetupInstructions();
        }
    }

    void Update()
    {
        if (showPeriodicWarning && Time.time >= nextWarningTime)
        {
            ShowSetupInstructions();
            nextWarningTime = Time.time + 10f; // Mỗi 10 giây
        }
    }

    [ContextMenu("📖 Show Setup Instructions")]
    public void ShowSetupInstructions()
    {
        Debug.Log("=== 🎮 INPUT SYSTEM SETUP FOR UNITY 6.2 ===");
        Debug.Log("");
        Debug.Log("🔧 CÁCH 1: Cài Input System Package (Khuyên dùng)");
        Debug.Log("1. Window → Package Manager");
        Debug.Log("2. Tìm 'Input System' → Install");
        Debug.Log("3. Restart Unity khi được hỏi");
        Debug.Log("");
        Debug.Log("🔧 CÁCH 2: Enable cả Legacy và Input System");
        Debug.Log("1. Edit → Project Settings");
        Debug.Log("2. XR Plug-in Management → Input System Package");
        Debug.Log("3. Chọn 'Both' thay vì 'Input System Package (New)'");
        Debug.Log("4. Restart Unity");
        Debug.Log("");
        Debug.Log("🔧 CÁCH 3: Quay về Legacy Input (Tạm thời)");
        Debug.Log("1. Edit → Project Settings");
        Debug.Log("2. XR Plug-in Management → Input System Package");
        Debug.Log("3. Chọn 'Input Manager (Old)' ");
        Debug.Log("4. Restart Unity");
        Debug.Log("");
        Debug.Log("📋 SAU KHI SETUP:");
        Debug.Log("- Input controls sẽ hoạt động bình thường");
        Debug.Log("- WASD: Di chuyển camera");
        Debug.Log("- Mouse: Xoay camera");
        Debug.Log("- Space: Toggle ragdoll");
        Debug.Log("- F1-F5: Debug functions");
        Debug.Log("========================================");
    }

    [ContextMenu("🎮 Test Input System")]
    public void TestInputSystem()
    {
        Debug.Log("🧪 Testing Input System...");
        
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool wPressed = Input.GetKey(KeyCode.W);
        Vector3 mousePos = Input.mousePosition;
        
        Debug.Log($"Space Pressed: {spacePressed}");
        Debug.Log($"W Pressed: {wPressed}");
        Debug.Log($"Mouse Position: {mousePos}");
        
        if (!spacePressed && !wPressed && mousePos == Vector3.zero)
        {
            Debug.LogWarning("❌ Input System không hoạt động - Cần setup theo hướng dẫn!");
            ShowSetupInstructions();
        }
        else
        {
            Debug.Log("✅ Input System hoạt động tốt!");
        }
    }

    void OnGUI()
    {
        // Hiển thị warning ở góc màn hình
        GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 110));
        
        GUI.color = Color.yellow;
        GUILayout.Box("⚠️ INPUT DISABLED");
        GUI.color = Color.white;
        
        GUILayout.Label("Input controls không hoạt động");
        GUILayout.Label("Cần setup Input System");
        
        if (GUILayout.Button("📖 Xem hướng dẫn"))
        {
            ShowSetupInstructions();
        }
        
        GUILayout.EndArea();
    }
} 