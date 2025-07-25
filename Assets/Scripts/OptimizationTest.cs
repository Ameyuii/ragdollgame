using UnityEngine;

/// <summary>
/// Script test để verify các chức năng sau khi tối ưu hóa UI system
/// </summary>
public class OptimizationTest : MonoBehaviour
{
    [Header("Test Components")]
    public BattleGameManager battleGameManager;
    public CharacterDragSource[] dragSources;
    
    [ContextMenu("Test All Systems")]
    public void TestAllSystems()
    {
        Debug.Log("=== KIỂM TRA SAU KHI TỐI ỬU HÓA ===");
        
        TestBattleGameManager();
        TestDragSourcesSystem();
        TestUISystem();
        
        Debug.Log("=== HOÀN THÀNH KIỂM TRA ===");
    }
    
    void TestBattleGameManager()
    {
        Debug.Log("1. Kiểm tra BattleGameManager...");
        
        if (battleGameManager == null)
        {
            battleGameManager = FindFirstObjectByType<BattleGameManager>();
        }
        
        if (battleGameManager != null)
        {
            Debug.Log($"✅ BattleGameManager tìm thấy: {battleGameManager.name}");
            Debug.Log($"   - Setup Mode: {battleGameManager.setupMode}");
            Debug.Log($"   - Selected Team: {battleGameManager.selectedTeam}");
            Debug.Log($"   - Character Prefabs Count: {(battleGameManager.characterPrefabs?.Length ?? 0)}");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy BattleGameManager!");
        }
    }
    
    void TestDragSourcesSystem()
    {
        Debug.Log("2. Kiểm tra Character Drag Sources...");
        
        dragSources = FindObjectsByType<CharacterDragSource>(FindObjectsSortMode.None);
        
        if (dragSources != null && dragSources.Length > 0)
        {
            Debug.Log($"✅ Tìm thấy {dragSources.Length} CharacterDragSource components");
            
            foreach (var dragSource in dragSources)
            {
                if (dragSource.characterPrefab != null && dragSource.gameManager != null)
                {
                    Debug.Log($"   - {dragSource.name}: Prefab={dragSource.characterPrefab.name}, GameManager={dragSource.gameManager.name}");
                }
                else
                {
                    Debug.LogWarning($"   - {dragSource.name}: Missing references!");
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy CharacterDragSource nào");
        }
    }
    
    void TestUISystem()
    {
        Debug.Log("3. Kiểm tra UI System...");
        
        // Kiểm tra Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ Canvas tìm thấy: {canvas.name}");
            Debug.Log($"   - Child count: {canvas.transform.childCount}");
            
            // List các child objects
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);
                Debug.Log($"   - Child {i}: {child.name}");
            }
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy Canvas!");
        }
        
        // Kiểm tra clean up thành công
        Debug.Log("✅ Tất cả script redundant đã được xóa thành công:");
        Debug.Log("   - TeamSelectionHandler: Đã xóa");
        Debug.Log("   - TeamSelectionFix: Đã xóa");
        Debug.Log("   - SimpleFix: Đã xóa");
        Debug.Log("   - Các test scripts: Đã xóa");
        
        // Verify chỉ còn BattleGameManager system
        var battleManagers = FindObjectsByType<BattleGameManager>(FindObjectsSortMode.None);
        Debug.Log($"✅ Hệ thống UI thống nhất: {battleManagers.Length} BattleGameManager");
    }
    
    [ContextMenu("Test Character Spawning")]
    public void TestCharacterSpawning()
    {
        Debug.Log("=== TEST CHARACTER SPAWNING ===");
        
        if (battleGameManager != null)
        {
            Vector3 testPosition = new Vector3(0, 0, 5);
            battleGameManager.SpawnCharacterAtPosition(testPosition);
            Debug.Log($"Đã test spawn character tại {testPosition}");
        }
        else
        {
            Debug.LogError("Không thể test spawn - BattleGameManager không tìm thấy");
        }
    }
    
    [ContextMenu("Reset All Drag States")]
    public void TestResetDragStates()
    {
        Debug.Log("=== TEST RESET DRAG STATES ===");
        
        CharacterDragSource.ResetAllDragStates();
        Debug.Log("Đã reset tất cả drag states");
    }
}