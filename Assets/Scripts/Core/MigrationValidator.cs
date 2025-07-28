using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Migration Validator - Validates the migration from legacy to unified system
/// Provides comprehensive testing and validation capabilities
/// </summary>
public class MigrationValidator : MonoBehaviour
{
    [Header("🎯 System References")]
    [Tooltip("System migration controller")]
    public SystemMigrationController migrationController;
    
    [Tooltip("Character registry")]
    public CharacterRegistry characterRegistry;
    
    [Tooltip("Unified game manager")]
    public UnifiedGameManager unifiedGameManager;
    
    [Tooltip("Auto UI generator")]
    public AutoUIGenerator autoUIGenerator;
    
    [Header("🧪 Test Settings")]
    [Tooltip("Enable automatic testing on start")]
    public bool autoTestOnStart = false;
    
    [Tooltip("Number of characters to spawn for testing")]
    public int testSpawnCount = 3;
    
    [Tooltip("Test positions for character spawning")]
    public Vector3[] testPositions = {
        new Vector3(-2, 0, 0),
        new Vector3(0, 0, 0), 
        new Vector3(2, 0, 0)
    };
    
    [Header("📊 Test Results")]
    [SerializeField] private bool lastTestPassed = false;
    [SerializeField] private string lastTestResults = "";
    [SerializeField] private float lastTestDuration = 0f;
    
    // Test data
    private List<GameObject> testSpawnedCharacters = new List<GameObject>();
    private Dictionary<string, bool> testResults = new Dictionary<string, bool>();
    
    private void Start()
    {
        if (autoTestOnStart)
        {
            Invoke(nameof(RunFullValidation), 1f); // Delay to ensure systems are initialized
        }
    }
    
    #region Public API
    
    [ContextMenu("🧪 Run Full Validation")]
    public void RunFullValidation()
    {
        Debug.Log("🧪 MigrationValidator: Starting full validation...");
        
        float startTime = Time.realtimeSinceStartup;
        testResults.Clear();
        
        try
        {
            // Test system references
            ValidateSystemReferences();
            
            // Test character registry
            ValidateCharacterRegistry();
            
            // Test unified game manager
            ValidateUnifiedGameManager();
            
            // Test UI system
            ValidateUISystem();
            
            // Test migration controller
            ValidateMigrationController();
            
            // Test character spawning
            ValidateCharacterSpawning();
            
            // Calculate results
            lastTestDuration = Time.realtimeSinceStartup - startTime;
            lastTestPassed = testResults.Values.All(result => result);
            lastTestResults = GenerateTestReport();
            
            Debug.Log($"✅ Full validation completed in {lastTestDuration:F2}s - {(lastTestPassed ? "PASSED" : "FAILED")}");
            Debug.Log(lastTestResults);
        }
        catch (System.Exception e)
        {
            lastTestPassed = false;
            lastTestResults = $"❌ Validation failed with exception: {e.Message}";
            Debug.LogError(lastTestResults);
        }
        finally
        {
            CleanupTestObjects();
        }
    }
    
    [ContextMenu("🔄 Test Migration Toggle")]
    public void TestMigrationToggle()
    {
        if (migrationController == null)
        {
            Debug.LogError("❌ Migration controller not assigned!");
            return;
        }
        
        Debug.Log("🔄 Testing migration toggle...");
        
        // Test enabling new systems
        migrationController.useNewCharacterSystem = true;
        migrationController.useNewUISystem = true;
        migrationController.useNewDragSystem = true;
        migrationController.RefreshMigrationSettings();
        
        // Wait and test
        Invoke(nameof(TestNewSystemsActive), 0.5f);
    }
    
    [ContextMenu("🚨 Test Emergency Rollback")]
    public void TestEmergencyRollback()
    {
        if (migrationController == null)
        {
            Debug.LogError("❌ Migration controller not assigned!");
            return;
        }
        
        Debug.Log("🚨 Testing emergency rollback...");
        
        // Enable new systems first
        migrationController.useNewCharacterSystem = true;
        migrationController.useNewUISystem = true;
        migrationController.useNewDragSystem = true;
        
        // Then trigger rollback
        migrationController.EmergencyRollback();
        
        // Verify rollback worked
        bool rollbackSuccess = !migrationController.useNewCharacterSystem && 
                              !migrationController.useNewUISystem && 
                              !migrationController.useNewDragSystem;
        
        Debug.Log($"🚨 Emergency rollback: {(rollbackSuccess ? "✅ SUCCESS" : "❌ FAILED")}");
    }
    
    #endregion
    
    #region Validation Methods
    
    private void ValidateSystemReferences()
    {
        Debug.Log("🔍 Validating system references...");
        
        testResults["SystemReferences_MigrationController"] = migrationController != null;
        testResults["SystemReferences_CharacterRegistry"] = characterRegistry != null;
        testResults["SystemReferences_UnifiedGameManager"] = unifiedGameManager != null;
        testResults["SystemReferences_AutoUIGenerator"] = autoUIGenerator != null;
        
        // Auto-find missing references
        if (migrationController == null)
            migrationController = FindObjectOfType<SystemMigrationController>();
        
        if (characterRegistry == null)
            characterRegistry = Resources.Load<CharacterRegistry>("Data/CharacterRegistry");
        
        if (unifiedGameManager == null)
            unifiedGameManager = FindObjectOfType<UnifiedGameManager>();
        
        if (autoUIGenerator == null)
            autoUIGenerator = FindObjectOfType<AutoUIGenerator>();
        
        Debug.Log($"✅ System references validation complete");
    }
    
    private void ValidateCharacterRegistry()
    {
        Debug.Log("🔍 Validating character registry...");
        
        if (characterRegistry == null)
        {
            testResults["CharacterRegistry_Exists"] = false;
            return;
        }
        
        testResults["CharacterRegistry_Exists"] = true;
        testResults["CharacterRegistry_HasCharacters"] = characterRegistry.characters.Count > 0;
        testResults["CharacterRegistry_AutoDiscovery"] = characterRegistry.enableAutoDiscovery;
        
        // Test character lookup
        var categories = characterRegistry.GetAllCategories();
        testResults["CharacterRegistry_HasCategories"] = categories.Count > 0;
        
        // Test character retrieval
        if (categories.Count > 0)
        {
            var firstCategory = categories[0];
            var charactersInCategory = characterRegistry.GetCharactersByCategory(firstCategory);
            testResults["CharacterRegistry_CategoryLookup"] = charactersInCategory.Count > 0;
        }
        
        Debug.Log($"✅ Character registry validation complete - {characterRegistry.characters.Count} characters, {categories.Count} categories");
    }
    
    private void ValidateUnifiedGameManager()
    {
        Debug.Log("🔍 Validating unified game manager...");
        
        if (unifiedGameManager == null)
        {
            testResults["UnifiedGameManager_Exists"] = false;
            return;
        }
        
        testResults["UnifiedGameManager_Exists"] = true;
        testResults["UnifiedGameManager_HasRegistry"] = unifiedGameManager.characterRegistry != null;
        testResults["UnifiedGameManager_HasSpawnParent"] = unifiedGameManager.spawnParent != null;
        // ✅ DISABLED: Team materials validation - loại bỏ hoàn toàn chức năng màu team
        testResults["UnifiedGameManager_HasTeamMaterials"] = true; // Always pass since we disabled team materials
        
        // Test singleton
        testResults["UnifiedGameManager_Singleton"] = UnifiedGameManager.Instance == unifiedGameManager;
        
        Debug.Log($"✅ Unified game manager validation complete");
    }
    
    private void ValidateUISystem()
    {
        Debug.Log("🔍 Validating UI system...");
        
        if (autoUIGenerator == null)
        {
            testResults["UISystem_AutoUIExists"] = false;
            return;
        }
        
        testResults["UISystem_AutoUIExists"] = true;
        testResults["UISystem_HasRegistry"] = autoUIGenerator.characterRegistry != null;
        testResults["UISystem_HasGameManager"] = autoUIGenerator.unifiedGameManager != null;
        
        // Test UI element finding
        GameObject modelContainer = GameObject.Find("ModelContentArea");
        testResults["UISystem_HasModelContainer"] = modelContainer != null;
        
        GameObject categoryPanel = GameObject.Find("CharacterCategoryPanel");
        testResults["UISystem_HasCategoryPanel"] = categoryPanel != null;
        
        Debug.Log($"✅ UI system validation complete");
    }
    
    private void ValidateMigrationController()
    {
        Debug.Log("🔍 Validating migration controller...");
        
        if (migrationController == null)
        {
            testResults["MigrationController_Exists"] = false;
            return;
        }
        
        testResults["MigrationController_Exists"] = true;
        testResults["MigrationController_HasUnifiedManager"] = migrationController.unifiedGameManager != null;
        testResults["MigrationController_HasAutoUI"] = migrationController.autoUIGenerator != null;
        testResults["MigrationController_LegacyBridge"] = migrationController.enableLegacyBridge;
        
        Debug.Log($"✅ Migration controller validation complete");
    }
    
    private void ValidateCharacterSpawning()
    {
        Debug.Log("🔍 Validating character spawning...");
        
        if (unifiedGameManager == null || characterRegistry == null)
        {
            testResults["CharacterSpawning_CanSpawn"] = false;
            return;
        }
        
        // Get first available character
        var allCharacters = characterRegistry.GetAllActiveCharacters();
        if (allCharacters.Count == 0)
        {
            testResults["CharacterSpawning_HasCharacters"] = false;
            return;
        }
        
        testResults["CharacterSpawning_HasCharacters"] = true;
        
        // Test spawning
        var testCharacter = allCharacters[0];
        Vector3 spawnPos = testPositions.Length > 0 ? testPositions[0] : Vector3.zero;
        
        GameObject spawnedChar = unifiedGameManager.SpawnCharacter(testCharacter.id, spawnPos);
        testResults["CharacterSpawning_CanSpawn"] = spawnedChar != null;
        
        if (spawnedChar != null)
        {
            testSpawnedCharacters.Add(spawnedChar);
            
            // Test character components
            testResults["CharacterSpawning_HasRagdoll"] = spawnedChar.GetComponent<RagdollCharacter>() != null;
            testResults["CharacterSpawning_HasNavMesh"] = spawnedChar.GetComponent<UnityEngine.AI.NavMeshAgent>() != null;
            testResults["CharacterSpawning_HasAI"] = spawnedChar.GetComponent<SimpleCharacterAI>() != null;
        }
        
        Debug.Log($"✅ Character spawning validation complete - Spawned: {spawnedChar != null}");
    }
    
    #endregion

    #region Helper Methods

    private void TestNewSystemsActive()
    {
        Debug.Log("🔍 Testing new systems activation...");

        bool characterSystemActive = migrationController.useNewCharacterSystem;
        bool uiSystemActive = migrationController.useNewUISystem;
        bool dragSystemActive = migrationController.useNewDragSystem;

        Debug.Log($"📊 New Systems Status:");
        Debug.Log($"  Character System: {(characterSystemActive ? "✅ ACTIVE" : "❌ INACTIVE")}");
        Debug.Log($"  UI System: {(uiSystemActive ? "✅ ACTIVE" : "❌ INACTIVE")}");
        Debug.Log($"  Drag System: {(dragSystemActive ? "✅ ACTIVE" : "❌ INACTIVE")}");

        bool allSystemsActive = characterSystemActive && uiSystemActive && dragSystemActive;
        Debug.Log($"🎯 Migration Toggle Test: {(allSystemsActive ? "✅ PASSED" : "❌ FAILED")}");
    }

    private string GenerateTestReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("=== MIGRATION VALIDATION REPORT ===");
        report.AppendLine($"Test Duration: {lastTestDuration:F2}s");
        report.AppendLine($"Overall Result: {(lastTestPassed ? "✅ PASSED" : "❌ FAILED")}");
        report.AppendLine();

        // Group results by category
        var categories = new Dictionary<string, List<KeyValuePair<string, bool>>>();

        foreach (var result in testResults)
        {
            string category = result.Key.Split('_')[0];
            if (!categories.ContainsKey(category))
                categories[category] = new List<KeyValuePair<string, bool>>();

            categories[category].Add(result);
        }

        // Generate report by category
        foreach (var category in categories)
        {
            report.AppendLine($"📋 {category.Key}:");
            foreach (var test in category.Value)
            {
                string testName = test.Key.Substring(category.Key.Length + 1);
                string status = test.Value ? "✅ PASS" : "❌ FAIL";
                report.AppendLine($"  {testName}: {status}");
            }
            report.AppendLine();
        }

        // Summary
        int passedTests = testResults.Values.Count(r => r);
        int totalTests = testResults.Count;
        report.AppendLine($"📊 Summary: {passedTests}/{totalTests} tests passed ({(float)passedTests/totalTests*100:F1}%)");

        return report.ToString();
    }

    private void CleanupTestObjects()
    {
        foreach (GameObject testObj in testSpawnedCharacters)
        {
            if (testObj != null)
            {
                DestroyImmediate(testObj);
            }
        }
        testSpawnedCharacters.Clear();

        Debug.Log("🧹 Test objects cleaned up");
    }

    #endregion

    #region Performance Testing

    [ContextMenu("⚡ Performance Test")]
    public void RunPerformanceTest()
    {
        Debug.Log("⚡ Starting performance test...");

        if (characterRegistry == null || unifiedGameManager == null)
        {
            Debug.LogError("❌ Missing references for performance test");
            return;
        }

        var characters = characterRegistry.GetAllActiveCharacters();
        if (characters.Count == 0)
        {
            Debug.LogError("❌ No characters available for performance test");
            return;
        }

        // Test character spawning performance
        float startTime = Time.realtimeSinceStartup;
        List<GameObject> spawnedChars = new List<GameObject>();

        for (int i = 0; i < testSpawnCount && i < characters.Count; i++)
        {
            Vector3 pos = i < testPositions.Length ? testPositions[i] : new Vector3(i * 2, 0, 0);
            GameObject spawned = unifiedGameManager.SpawnCharacter(characters[i].id, pos);
            if (spawned != null)
                spawnedChars.Add(spawned);
        }

        float spawnTime = Time.realtimeSinceStartup - startTime;

        // Test UI generation performance
        startTime = Time.realtimeSinceStartup;
        if (autoUIGenerator != null)
        {
            autoUIGenerator.GenerateUI();
        }
        float uiTime = Time.realtimeSinceStartup - startTime;

        // Report results
        Debug.Log("⚡ PERFORMANCE TEST RESULTS:");
        Debug.Log($"  Character Spawning: {spawnTime*1000:F2}ms for {spawnedChars.Count} characters");
        Debug.Log($"  UI Generation: {uiTime*1000:F2}ms");
        Debug.Log($"  Average Spawn Time: {(spawnTime/spawnedChars.Count)*1000:F2}ms per character");

        // Cleanup
        foreach (GameObject obj in spawnedChars)
        {
            if (obj != null) DestroyImmediate(obj);
        }
    }

    #endregion

    #region Debug Info

    [ContextMenu("📊 Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("=== SYSTEM STATUS ===");

        if (migrationController != null)
        {
            migrationController.ShowMigrationStatus();
        }

        if (unifiedGameManager != null)
        {
            unifiedGameManager.ShowSystemStatus();
        }

        if (autoUIGenerator != null)
        {
            autoUIGenerator.ShowUIStatus();
        }

        if (characterRegistry != null)
        {
            Debug.Log($"Character Registry: {characterRegistry.characters.Count} characters in {characterRegistry.GetAllCategories().Count} categories");
        }
    }

    #endregion
}
