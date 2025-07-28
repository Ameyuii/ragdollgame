using UnityEngine;

/// <summary>
/// Controls the migration from legacy character system to unified character system
/// Provides feature flags and safe transition mechanisms
/// </summary>
public class SystemMigrationController : MonoBehaviour
{
    [Header("🔄 Migration Settings")]
    [Tooltip("Enable the new unified character system")]
    public bool useNewCharacterSystem = false;
    
    [Tooltip("Enable the new auto-generated UI system")]
    public bool useNewUISystem = false;
    
    [Tooltip("Enable the new simplified drag system")]
    public bool useNewDragSystem = false;
    
    [Tooltip("Enable legacy bridge for backward compatibility")]
    public bool enableLegacyBridge = true;
    
    [Header("🎯 System References")]
    [Tooltip("Reference to the new UnifiedGameManager")]
    public UnifiedGameManager unifiedGameManager;
    
    [Tooltip("Reference to the new AutoUIGenerator")]
    public AutoUIGenerator autoUIGenerator;
    
    [Header("📊 Migration Status")]
    [SerializeField] private bool migrationInitialized = false;
    [SerializeField] private string migrationPhase = "Not Started";
    
    private void Start()
    {
        InitializeMigration();
    }
    
    private void InitializeMigration()
    {
        Debug.Log("🔄 SystemMigrationController: Initializing migration...");
        
        try
        {
            ConfigureSystems();
            migrationInitialized = true;
            UpdateMigrationPhase();
            
            Debug.Log($"✅ Migration initialized - Phase: {migrationPhase}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Migration initialization failed: {e.Message}");
        }
    }
    
    private void ConfigureSystems()
    {
        // Configure Character System
        ConfigureCharacterSystem();
        
        // Configure UI System
        ConfigureUISystem();
        
        // Configure Drag System
        ConfigureDragSystem();
    }
    
    private void ConfigureCharacterSystem()
    {
        if (useNewCharacterSystem)
        {
            // Enable new unified system
            if (unifiedGameManager != null)
            {
                unifiedGameManager.enabled = true;
                unifiedGameManager.enableLegacyBridge = enableLegacyBridge;
                Debug.Log("✅ New character system enabled");
            }
            else
            {
                Debug.LogWarning("⚠️ UnifiedGameManager reference missing!");
            }
        }
        else
        {
            // Keep legacy system active
            var legacyManager = FindObjectOfType<BattleGameManager>();
            if (legacyManager != null)
            {
                legacyManager.enabled = true;
                Debug.Log("✅ Legacy character system active");
            }
        }
    }
    
    private void ConfigureUISystem()
    {
        // var oldCategoryHandler = FindObjectOfType<CategoryButtonHandler>(); // Legacy - removed
        var oldCategoryHandler = (object)null; // Placeholder

        if (useNewUISystem)
        {
            // Enable new UI system
            if (autoUIGenerator != null)
            {
                autoUIGenerator.enabled = true;
                Debug.Log("✅ New UI system enabled");
            }

            // Disable legacy UI system (Legacy - CategoryButtonHandler removed)
            if (oldCategoryHandler != null)
            {
                // oldCategoryHandler.enabled = false; // Legacy - CategoryButtonHandler removed
                Debug.Log("🔇 Legacy UI system disabled (CategoryButtonHandler removed)");
            }
        }
        else
        {
            // Keep legacy UI system (Legacy - CategoryButtonHandler removed)
            if (oldCategoryHandler != null)
            {
                // oldCategoryHandler.enabled = true; // Legacy - CategoryButtonHandler removed
                Debug.Log("✅ Legacy UI system active (CategoryButtonHandler removed)");
            }

            // Disable new UI system
            if (autoUIGenerator != null)
            {
                autoUIGenerator.enabled = false;
                Debug.Log("🔇 New UI system disabled");
            }
        }
    }
    
    private void ConfigureDragSystem()
    {
        // Drag system configuration will be handled by UI system
        // SimpleCharacterDrag components will be added/removed dynamically
        Debug.Log($"🖱️ Drag system configured for: {(useNewDragSystem ? "New" : "Legacy")} system");
    }
    
    private void UpdateMigrationPhase()
    {
        if (!useNewCharacterSystem && !useNewUISystem && !useNewDragSystem)
        {
            migrationPhase = "Legacy System Active";
        }
        else if (useNewCharacterSystem && useNewUISystem && useNewDragSystem)
        {
            migrationPhase = enableLegacyBridge ? "Full Migration with Bridge" : "Complete Migration";
        }
        else
        {
            migrationPhase = "Partial Migration";
        }
    }
    
    [ContextMenu("🔄 Refresh Migration Settings")]
    public void RefreshMigrationSettings()
    {
        ConfigureSystems();
        UpdateMigrationPhase();
        Debug.Log($"🔄 Migration settings refreshed - Phase: {migrationPhase}");
    }
    
    [ContextMenu("📊 Show Migration Status")]
    public void ShowMigrationStatus()
    {
        Debug.Log("=== MIGRATION STATUS ===");
        Debug.Log($"Character System: {(useNewCharacterSystem ? "NEW" : "LEGACY")}");
        Debug.Log($"UI System: {(useNewUISystem ? "NEW" : "LEGACY")}");
        Debug.Log($"Drag System: {(useNewDragSystem ? "NEW" : "LEGACY")}");
        Debug.Log($"Legacy Bridge: {(enableLegacyBridge ? "ENABLED" : "DISABLED")}");
        Debug.Log($"Migration Phase: {migrationPhase}");
        Debug.Log($"Initialized: {migrationInitialized}");
    }
    
    [ContextMenu("🚨 Emergency Rollback")]
    public void EmergencyRollback()
    {
        Debug.Log("🚨 PERFORMING EMERGENCY ROLLBACK...");
        
        // Disable all new systems
        useNewCharacterSystem = false;
        useNewUISystem = false;
        useNewDragSystem = false;
        enableLegacyBridge = true;
        
        // Reconfigure systems
        ConfigureSystems();
        UpdateMigrationPhase();
        
        Debug.Log("🚨 EMERGENCY ROLLBACK COMPLETE!");
        ShowMigrationStatus();
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying && migrationInitialized)
        {
            // Reconfigure when settings change in inspector
            ConfigureSystems();
            UpdateMigrationPhase();
        }
    }
}
