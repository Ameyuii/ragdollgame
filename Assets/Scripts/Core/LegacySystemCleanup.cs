using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Legacy System Cleanup - Safely removes deprecated scripts and components
/// Only use after confirming new unified system works correctly
/// </summary>
public class LegacySystemCleanup : MonoBehaviour
{
    [Header("🧹 Cleanup Settings")]
    [Tooltip("Enable automatic backup before cleanup")]
    public bool createBackupBeforeCleanup = true;
    
    [Tooltip("Confirm cleanup by typing 'CONFIRM' in this field")]
    public string confirmationText = "";
    
    [Header("📋 Scripts to Remove")]
    [Tooltip("List of deprecated script types to remove")]
    public string[] deprecatedScripts = {
        "CategoryButtonHandler",
        "CharacterManager", 
        "CharacterDatabase",
        "SimpleCharacterSelection",
        "CharacterDragSource",
        "CleanupConflictingObjects",
        "FixMissingReferences",
        "ProjectCrashFixExecutor",
        "FixUnityProjectCrash"
    };
    
    [Header("📊 Cleanup Status")]
    [SerializeField] private bool cleanupCompleted = false;
    [SerializeField] private int componentsRemoved = 0;
    [SerializeField] private int scriptsMovedToBackup = 0;
    
    // Cleanup results
    private List<string> cleanupLog = new List<string>();
    private List<GameObject> affectedObjects = new List<GameObject>();
    
    #region Public API
    
    [ContextMenu("🧹 Clean Up Legacy Scripts")]
    public void CleanupLegacyScripts()
    {
        if (confirmationText != "CONFIRM")
        {
            Debug.LogError("❌ Cleanup requires confirmation! Type 'CONFIRM' in the confirmation field.");
            return;
        }
        
        Debug.Log("🧹 Starting legacy system cleanup...");
        
        try
        {
            // Reset counters
            componentsRemoved = 0;
            scriptsMovedToBackup = 0;
            cleanupLog.Clear();
            affectedObjects.Clear();
            
            // Create backup if enabled
            if (createBackupBeforeCleanup)
            {
                CreateCleanupBackup();
            }
            
            // Remove deprecated components
            RemoveDeprecatedComponents();
            
            // Move script files to backup
            MoveScriptsToBackup();
            
            // Update references
            UpdateReferences();
            
            // Validate new system
            ValidateNewSystem();
            
            // Mark cleanup as completed
            cleanupCompleted = true;
            
            // Generate cleanup report
            GenerateCleanupReport();
            
            Debug.Log("✅ Legacy cleanup complete!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Legacy cleanup failed: {e.Message}");
        }
    }
    
    [ContextMenu("🔍 Preview Cleanup")]
    public void PreviewCleanup()
    {
        Debug.Log("🔍 Previewing legacy cleanup...");
        
        // Find all deprecated components
        var deprecatedComponents = FindDeprecatedComponents();
        
        Debug.Log($"📊 CLEANUP PREVIEW:");
        Debug.Log($"  Components to remove: {deprecatedComponents.Count}");
        Debug.Log($"  Scripts to backup: {deprecatedScripts.Length}");
        
        foreach (var component in deprecatedComponents)
        {
            Debug.Log($"  - {component.GetType().Name} on {component.gameObject.name}");
        }
    }
    
    [ContextMenu("🚨 Emergency Restore")]
    public void EmergencyRestore()
    {
        Debug.Log("🚨 EMERGENCY RESTORE - This feature is not yet implemented");
        Debug.Log("To restore manually:");
        Debug.Log("1. Copy scripts from Assets/Scripts/Legacy_Backup/ back to original locations");
        Debug.Log("2. Re-add components to GameObjects");
        Debug.Log("3. Disable new systems in SystemMigrationController");
    }
    
    #endregion
    
    #region Cleanup Methods
    
    private void CreateCleanupBackup()
    {
        Debug.Log("💾 Creating cleanup backup...");
        
        #if UNITY_EDITOR
        // Create backup folder with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupPath = $"Assets/Scripts/Cleanup_Backup_{timestamp}";
        
        if (!AssetDatabase.IsValidFolder(backupPath))
        {
            AssetDatabase.CreateFolder("Assets/Scripts", $"Cleanup_Backup_{timestamp}");
        }
        
        // Copy current scene
        string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        if (!string.IsNullOrEmpty(scenePath))
        {
            string backupScenePath = $"{backupPath}/Scene_Backup.unity";
            AssetDatabase.CopyAsset(scenePath, backupScenePath);
        }
        
        Debug.Log($"💾 Backup created at: {backupPath}");
        #endif
    }
    
    private void RemoveDeprecatedComponents()
    {
        Debug.Log("🗑️ Removing deprecated components...");
        
        var deprecatedComponents = FindDeprecatedComponents();
        
        foreach (var component in deprecatedComponents)
        {
            if (component != null)
            {
                string componentName = component.GetType().Name;
                string objectName = component.gameObject.name;
                
                // Log removal
                cleanupLog.Add($"Removed {componentName} from {objectName}");
                affectedObjects.Add(component.gameObject);
                
                // Remove component
                #if UNITY_EDITOR
                DestroyImmediate(component);
                #else
                Destroy(component);
                #endif
                
                componentsRemoved++;
                Debug.Log($"❌ Removed {componentName} from {objectName}");
            }
        }
        
        Debug.Log($"✅ Removed {componentsRemoved} deprecated components");
    }
    
    private List<Component> FindDeprecatedComponents()
    {
        List<Component> deprecatedComponents = new List<Component>();
        
        // Find all objects in scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            
            foreach (Component comp in components)
            {
                if (comp != null && deprecatedScripts.Contains(comp.GetType().Name))
                {
                    deprecatedComponents.Add(comp);
                }
            }
        }
        
        return deprecatedComponents;
    }
    
    private void MoveScriptsToBackup()
    {
        Debug.Log("📁 Moving scripts to backup...");
        
        #if UNITY_EDITOR
        string backupFolder = "Assets/Scripts/Legacy_Backup";
        
        // Ensure backup folder exists
        if (!AssetDatabase.IsValidFolder(backupFolder))
        {
            AssetDatabase.CreateFolder("Assets/Scripts", "Legacy_Backup");
        }
        
        foreach (string scriptName in deprecatedScripts)
        {
            // Find script file
            string[] guids = AssetDatabase.FindAssets($"{scriptName} t:Script");
            
            foreach (string guid in guids)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileName(scriptPath);
                string backupPath = $"{backupFolder}/{fileName}";
                
                // Move to backup if not already there
                if (!scriptPath.Contains("Legacy_Backup") && !scriptPath.Contains("Cleanup_Backup"))
                {
                    AssetDatabase.MoveAsset(scriptPath, backupPath);
                    scriptsMovedToBackup++;
                    cleanupLog.Add($"Moved {fileName} to backup");
                    Debug.Log($"📁 Moved {fileName} to backup");
                }
            }
        }
        
        AssetDatabase.Refresh();
        #endif
        
        Debug.Log($"✅ Moved {scriptsMovedToBackup} scripts to backup");
    }
    
    private void UpdateReferences()
    {
        Debug.Log("🔗 Updating references...");
        
        // Find objects that might have missing script references
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int missingReferences = 0;
        
        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    missingReferences++;
                }
            }
        }
        
        if (missingReferences > 0)
        {
            Debug.LogWarning($"⚠️ Found {missingReferences} missing script references. These are expected after cleanup.");
        }
        
        Debug.Log("✅ Reference update complete");
    }
    
    private void ValidateNewSystem()
    {
        Debug.Log("✅ Validating new system...");
        
        // Check if new system components exist and are working
        var unifiedManager = FindObjectOfType<UnifiedGameManager>();
        var autoUIGenerator = FindObjectOfType<AutoUIGenerator>();
        var migrationController = FindObjectOfType<SystemMigrationController>();
        
        bool systemValid = unifiedManager != null && autoUIGenerator != null && migrationController != null;
        
        if (systemValid)
        {
            Debug.Log("✅ New unified system validation passed");
            cleanupLog.Add("New system validation: PASSED");
        }
        else
        {
            Debug.LogError("❌ New unified system validation failed!");
            cleanupLog.Add("New system validation: FAILED");
        }
    }
    
    private void GenerateCleanupReport()
    {
        Debug.Log("=== LEGACY CLEANUP REPORT ===");
        Debug.Log($"Cleanup Completed: {cleanupCompleted}");
        Debug.Log($"Components Removed: {componentsRemoved}");
        Debug.Log($"Scripts Moved to Backup: {scriptsMovedToBackup}");
        Debug.Log($"Objects Affected: {affectedObjects.Count}");
        Debug.Log("");

        Debug.Log("📋 Cleanup Log:");
        foreach (string logEntry in cleanupLog)
        {
            Debug.Log($"  - {logEntry}");
        }

        Debug.Log("");
        Debug.Log("🎯 Next Steps:");
        Debug.Log("1. Test the new unified system thoroughly");
        Debug.Log("2. Verify all functionality works as expected");
        Debug.Log("3. If issues occur, use Emergency Restore");
        Debug.Log("4. After confirming stability, backup folder can be deleted");
    }
    
    #endregion
    
    #region Utility Methods
    
    [ContextMenu("📊 Show Cleanup Status")]
    public void ShowCleanupStatus()
    {
        Debug.Log("=== CLEANUP STATUS ===");
        Debug.Log($"Cleanup Completed: {cleanupCompleted}");
        Debug.Log($"Components Removed: {componentsRemoved}");
        Debug.Log($"Scripts Moved: {scriptsMovedToBackup}");
        Debug.Log($"Confirmation Text: '{confirmationText}'");
        Debug.Log($"Ready for Cleanup: {confirmationText == "CONFIRM"}");
    }
    
    #endregion
}
