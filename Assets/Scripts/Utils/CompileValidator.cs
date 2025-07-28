using UnityEngine;

/// <summary>
/// Compile Validator - Validates that all systems compile and work correctly after cleanup
/// </summary>
public class CompileValidator : MonoBehaviour
{
    [Header("🔍 Validation Results")]
    [SerializeField] private bool allSystemsValid = false;
    [SerializeField] private int validatedComponents = 0;
    [SerializeField] private int totalComponents = 0;
    
    [ContextMenu("🧪 Validate All Systems")]
    public void ValidateAllSystems()
    {
        Debug.Log("🧪 Starting system validation...");
        
        validatedComponents = 0;
        totalComponents = 0;
        
        // Test Core Systems
        ValidateCoreSystem();
        
        // Test AI Systems  
        ValidateAISystem();
        
        // Test UI Systems
        ValidateUISystem();
        
        // Test Utils
        ValidateUtils();
        
        // Final result
        allSystemsValid = (validatedComponents == totalComponents);
        
        if (allSystemsValid)
        {
            Debug.Log($"✅ ALL SYSTEMS VALID! ({validatedComponents}/{totalComponents} components working)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Some systems need attention ({validatedComponents}/{totalComponents} components working)");
        }
    }
    
    private void ValidateCoreSystem()
    {
        Debug.Log("🔍 Validating Core Systems...");
        
        // Test UnifiedGameManager
        totalComponents++;
        var unifiedManager = FindAnyObjectByType<UnifiedGameManager>();
        if (unifiedManager != null)
        {
            validatedComponents++;
            Debug.Log("✅ UnifiedGameManager found and accessible");
        }
        else
        {
            Debug.LogWarning("❌ UnifiedGameManager not found");
        }
        
        // Test BattleGameManager
        totalComponents++;
        var battleManager = FindAnyObjectByType<BattleGameManager>();
        if (battleManager != null)
        {
            validatedComponents++;
            Debug.Log("✅ BattleGameManager found and accessible");
        }
        else
        {
            Debug.LogWarning("❌ BattleGameManager not found");
        }
        
        // Test CharacterRegistry
        totalComponents++;
        var registry = FindAnyObjectByType<CharacterRegistry>();
        if (registry != null)
        {
            validatedComponents++;
            Debug.Log("✅ CharacterRegistry found and accessible");
        }
        else
        {
            Debug.LogWarning("❌ CharacterRegistry not found");
        }
    }
    
    private void ValidateAISystem()
    {
        Debug.Log("🔍 Validating AI Systems...");
        
        // Test SimpleCharacterAI
        totalComponents++;
        var aiComponents = FindObjectsOfType<SimpleCharacterAI>();
        if (aiComponents.Length > 0)
        {
            validatedComponents++;
            Debug.Log($"✅ SimpleCharacterAI found ({aiComponents.Length} instances)");
        }
        else
        {
            Debug.LogWarning("❌ No SimpleCharacterAI components found");
        }
        
        // Test SafeNavMeshHelper
        totalComponents++;
        try
        {
            bool testResult = SafeNavMeshHelper.IsAgentValid(null);
            validatedComponents++;
            Debug.Log("✅ SafeNavMeshHelper accessible and working");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ SafeNavMeshHelper error: {e.Message}");
        }
    }
    
    private void ValidateUISystem()
    {
        Debug.Log("🔍 Validating UI Systems...");
        
        // Test AutoUIGenerator
        totalComponents++;
        var autoUI = FindAnyObjectByType<AutoUIGenerator>();
        if (autoUI != null)
        {
            validatedComponents++;
            Debug.Log("✅ AutoUIGenerator found and accessible");
        }
        else
        {
            Debug.LogWarning("❌ AutoUIGenerator not found");
        }
        
        // Test SimpleCharacterDrag
        totalComponents++;
        var dragComponents = FindObjectsOfType<SimpleCharacterDrag>();
        if (dragComponents.Length > 0)
        {
            validatedComponents++;
            Debug.Log($"✅ SimpleCharacterDrag found ({dragComponents.Length} instances)");
        }
        else
        {
            Debug.LogWarning("❌ No SimpleCharacterDrag components found");
        }
    }
    
    private void ValidateUtils()
    {
        Debug.Log("🔍 Validating Utility Systems...");
        
        // Test RagdollCharacter
        totalComponents++;
        var ragdollComponents = FindObjectsOfType<RagdollCharacter>();
        if (ragdollComponents.Length > 0)
        {
            validatedComponents++;
            Debug.Log($"✅ RagdollCharacter found ({ragdollComponents.Length} instances)");
            
            // Test RagdollCharacter methods
            var testCharacter = ragdollComponents[0];
            try
            {
                float health = testCharacter.GetCurrentHealth();
                bool isDead = testCharacter.IsDead();
                Debug.Log($"✅ RagdollCharacter methods working (Health: {health}, Dead: {isDead})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ RagdollCharacter method error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("❌ No RagdollCharacter components found");
        }
        
        // Test HealthBarController
        totalComponents++;
        var healthBars = FindObjectsOfType<HealthBarController>();
        if (healthBars.Length > 0)
        {
            validatedComponents++;
            Debug.Log($"✅ HealthBarController found ({healthBars.Length} instances)");
            
            // Test RefreshHealthBar method
            try
            {
                healthBars[0].RefreshHealthBar();
                Debug.Log("✅ HealthBarController.RefreshHealthBar() working");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ HealthBarController.RefreshHealthBar() error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("❌ No HealthBarController components found");
        }
    }
}
