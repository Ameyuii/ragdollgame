using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AI Coordinator - Manages và đồng bộ giữa AutoAIManager và RagdollCharacter internal AI
/// Đảm bảo không có conflicts và duplicate behaviors
/// </summary>
public class AICoordinator : MonoBehaviour
{
    [Header("AI Coordination Settings")]
    [Tooltip("Chọn AI system chính sẽ được sử dụng")]
    public AISystemType primaryAISystem = AISystemType.AutoAI;
    
    [Header("Performance Settings")]
    public float coordinationUpdateInterval = 1f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    public enum AISystemType
    {
        AutoAI,        // Sử dụng AutoAIManager + SimpleCharacterAI
        InternalAI,    // Sử dụng RagdollCharacter internal AI
        Hybrid         // Kết hợp cả hai (advanced)
    }
    
    private BattleGameManager battleManager;
    private AutoAIManager autoAIManager;
    private List<RagdollCharacter> allCharacters = new List<RagdollCharacter>();
    private float lastCoordinationUpdate = 0f;
    
    void Start()
    {
        // Tìm required components
        battleManager = FindFirstObjectByType<BattleGameManager>();
        autoAIManager = FindFirstObjectByType<AutoAIManager>();
        
        if (battleManager == null)
        {
            Debug.LogError("AICoordinator: Không tìm thấy BattleGameManager!");
            enabled = false;
            return;
        }
        
        // Initialize coordination
        InitializeAICoordination();
        
        Debug.Log($"AICoordinator initialized with {primaryAISystem} as primary AI system");
    }
    
    void InitializeAICoordination()
    {
        // Get all characters
        RefreshCharacterList();
        
        // Configure AI systems based on primary choice
        ConfigureAISystems();
        
        // Start coordination monitoring
        InvokeRepeating(nameof(MonitorAISystems), 1f, coordinationUpdateInterval);
    }
    
    void RefreshCharacterList()
    {
        allCharacters.Clear();
        RagdollCharacter[] characters = FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        allCharacters.AddRange(characters);
        
        if (showDebugInfo)
            Debug.Log($"AICoordinator: Found {allCharacters.Count} characters");
    }
    
    void ConfigureAISystems()
    {
        switch (primaryAISystem)
        {
            case AISystemType.AutoAI:
                ConfigureForAutoAI();
                break;
                
            case AISystemType.InternalAI:
                ConfigureForInternalAI();
                break;
                
            case AISystemType.Hybrid:
                ConfigureForHybridAI();
                break;
        }
    }
    
    void ConfigureForAutoAI()
    {
        if (showDebugInfo)
            Debug.Log("Configuring for AutoAI system");
        
        // Enable AutoAIManager
        if (autoAIManager != null)
        {
            autoAIManager.enableAutoAI = true;
        }
        
        // Clean up existing AI components first
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character == null) continue;
            
            // Remove any existing SimpleCharacterAI components
            SimpleCharacterAI existingAI = character.GetComponent<SimpleCharacterAI>();
            if (existingAI != null)
            {
                DestroyImmediate(existingAI);
            }
        }
        
        // Let AutoAIManager setup AI for all characters
        if (autoAIManager != null)
        {
            // Force setup for all characters
            foreach (RagdollCharacter character in allCharacters)
            {
                if (character != null)
                {
                    autoAIManager.ForceSetupCharacterAI(character.gameObject);
                }
            }
            
            // Enable AI if battle is active
            if (battleManager != null && battleManager.gameStarted)
            {
                autoAIManager.EnableAllAI();
            }
        }
        
        if (showDebugInfo)
        {
            // Check results
            SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
            Debug.Log($"AutoAI configuration complete - {allAI.Length} SimpleCharacterAI components found");
        }
    }
    
    void ConfigureForInternalAI()
    {
        if (showDebugInfo)
            Debug.Log("Configuring for Internal AI system");
        
        // Disable AutoAIManager
        if (autoAIManager != null)
        {
            autoAIManager.enableAutoAI = false;
        }
        
        // Remove all SimpleCharacterAI components to prevent conflicts
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character == null) continue;
            
            SimpleCharacterAI externalAI = character.GetComponent<SimpleCharacterAI>();
            if (externalAI != null)
            {
                DestroyImmediate(externalAI);
            }
        }
        
        // RagdollCharacter internal AI will automatically activate
        // due to the coordination logic we added
    }
    
    void ConfigureForHybridAI()
    {
        if (showDebugInfo)
            Debug.Log("Configuring for Hybrid AI system");
        
        // Advanced hybrid logic - some characters use AutoAI, others use internal AI
        // For now, use AutoAI for all characters but allow fallback to internal AI
        
        if (autoAIManager != null)
        {
            autoAIManager.enableAutoAI = true;
        }
        
        // Let each character decide based on its setup
    }
    
    void MonitorAISystems()
    {
        if (Time.time - lastCoordinationUpdate < coordinationUpdateInterval)
            return;
        
        lastCoordinationUpdate = Time.time;
        
        // Check for new characters
        RefreshCharacterList();
        
        // Validate AI coordination
        ValidateAICoordination();
        
        // Handle battle state changes
        HandleBattleStateChanges();
    }
    
    void ValidateAICoordination()
    {
        if (!showDebugInfo) return;
        
        int charactersWithSimpleAI = 0;
        int charactersWithInternalAI = 0;
        int charactersWithConflicts = 0;
        
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character == null) continue;
            
            SimpleCharacterAI simpleAI = character.GetComponent<SimpleCharacterAI>();
            bool hasSimpleAI = simpleAI != null && simpleAI.enabled;
            
            // Check if RagdollCharacter internal AI would be active
            BattleGameManager gameManager = FindFirstObjectByType<BattleGameManager>();
            AutoAIManager autoAI = FindFirstObjectByType<AutoAIManager>();
            bool wouldUseInternalAI = gameManager != null && gameManager.gameStarted && 
                                     simpleAI == null && 
                                     (autoAI == null || !autoAI.enableAutoAI);
            
            if (hasSimpleAI) charactersWithSimpleAI++;
            if (wouldUseInternalAI) charactersWithInternalAI++;
            if (hasSimpleAI && wouldUseInternalAI) charactersWithConflicts++;
        }
        
        if (charactersWithConflicts > 0)
        {
            Debug.LogWarning($"AICoordinator: Detected {charactersWithConflicts} characters with AI conflicts!");
        }
        
        Debug.Log($"AI Status - SimpleAI: {charactersWithSimpleAI}, InternalAI: {charactersWithInternalAI}, Conflicts: {charactersWithConflicts}");
    }
    
    void HandleBattleStateChanges()
    {
        if (battleManager == null) return;
        
        if (battleManager.gameStarted)
        {
            // Battle started - ensure AI is active
            switch (primaryAISystem)
            {
                case AISystemType.AutoAI:
                    if (autoAIManager != null)
                    {
                        autoAIManager.EnableAllAI();
                    }
                    break;
                    
                case AISystemType.InternalAI:
                    // Internal AI will activate automatically through RagdollCharacter logic
                    break;
                    
                case AISystemType.Hybrid:
                    if (autoAIManager != null)
                    {
                        autoAIManager.EnableAllAI();
                    }
                    break;
            }
        }
        else
        {
            // Battle not started - ensure AI is disabled
            if (autoAIManager != null)
            {
                autoAIManager.DisableAllAI();
            }
        }
    }
    
    /// <summary>
    /// Chuyển đổi AI system runtime
    /// </summary>
    public void SwitchAISystem(AISystemType newSystem)
    {
        if (primaryAISystem == newSystem) return;
        
        Debug.Log($"Switching AI system from {primaryAISystem} to {newSystem}");
        
        primaryAISystem = newSystem;
        ConfigureAISystems();
    }
    
    /// <summary>
    /// Force sync tất cả AI systems
    /// </summary>
    public void ForceSyncAISystems()
    {
        Debug.Log("Force syncing AI systems...");
        
        RefreshCharacterList();
        ConfigureAISystems();
        ValidateAICoordination();
        
        Debug.Log("AI systems sync completed");
    }
    
    /// <summary>
    /// Kiểm tra AI status cho debugging
    /// </summary>
    public void CheckAIStatus()
    {
        Debug.Log("=== AI COORDINATION STATUS ===");
        Debug.Log($"Primary AI System: {primaryAISystem}");
        Debug.Log($"Battle Started: {(battleManager != null ? battleManager.gameStarted : false)}");
        Debug.Log($"AutoAI Enabled: {(autoAIManager != null ? autoAIManager.enableAutoAI : false)}");
        
        ValidateAICoordination();
        
        if (autoAIManager != null)
        {
            autoAIManager.CheckAIStatus();
        }
    }
    
    void Update()
    {
        // Runtime monitoring và debugging - disabled old Input system
        // Use Unity Input System or UI buttons instead
        /*
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CheckAIStatus();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ForceSyncAISystems();
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Cycle through AI systems for testing
            AISystemType nextSystem = (AISystemType)(((int)primaryAISystem + 1) % 3);
            SwitchAISystem(nextSystem);
        }
        */
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo) return;
        
        // Draw coordinator status in scene view
        Vector3 pos = transform.position + Vector3.up * 5f;
        UnityEditor.Handles.Label(pos, $"AI Coordinator\nPrimary: {primaryAISystem}\nCharacters: {allCharacters.Count}");
    }
}