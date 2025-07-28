using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ManagedCharacterCategory
{
    [Header("Category Info")]
    public string categoryName = "New Category";
    public Color categoryColor = Color.white;
    public Sprite categoryIcon;
    
    [Header("Characters in this Category")]
    public List<CharacterEntry> characters = new List<CharacterEntry>();
    
    [Header("Settings")]
    public bool isExpanded = true;
}

[System.Serializable]
public class CharacterEntry
{
    [Header("Character Info")]
    public string characterName = "New Character";
    public GameObject prefab;
    public Sprite uiIcon;
    
    [Header("Stats")]
    public int health = 100;
    public float speed = 5f;
    public float attackDamage = 20f;
    public float attackRange = 2f;
    
    [Header("Visual")]
    public Color teamColor = Color.white;
    [TextArea(2, 3)]
    public string description = "";
}

[System.Obsolete("CharacterManager will be replaced by UnifiedGameManager in the new Unified Character System. This script is deprecated and will be removed after migration.")]
public class CharacterManager : MonoBehaviour
{
    [Header("Character Categories")]
    [SerializeField] private List<ManagedCharacterCategory> categories = new List<ManagedCharacterCategory>();
    
    [Header("UI References")]
    public BattleGameManager gameManager;
    
    [Header("Auto Setup")]
    public bool autoRefreshUI = true;
    public bool autoSetupPrefabs = true;
    
    void Start()
    {
        // Find references if not assigned
        // UI setup removed - using AutoUIGenerator instead (CategoryButtonHandler deprecated)
        
        if (gameManager == null)
            gameManager = FindAnyObjectByType<BattleGameManager>();
        
        // Initialize default categories if empty
        if (categories.Count == 0)
        {
            InitializeDefaultCategories();
        }
        
        // Auto refresh UI if enabled
        if (autoRefreshUI)
        {
            RefreshUI();
        }
    }
    
    void OnValidate()
    {
        // Auto refresh UI when values change in inspector (works in Editor too)
        if (autoRefreshUI)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // In Editor mode, use EditorApplication.delayCall
                UnityEditor.EditorApplication.delayCall += RefreshUI;
            }
            else
            #endif
            {
                RefreshUI();
            }
        }
    }
    
    /// <summary>
    /// Initialize default categories
    /// </summary>
    [ContextMenu("Initialize Default Categories")]
    public void InitializeDefaultCategories()
    {
        categories.Clear();
        
        // Soldier Category
        ManagedCharacterCategory soldierCategory = new ManagedCharacterCategory();
        soldierCategory.categoryName = "🪖 CHIẾN BINH";
        soldierCategory.categoryColor = new Color(0.3f, 1f, 0.3f, 1f);
        categories.Add(soldierCategory);
        
        // Robot Category
        ManagedCharacterCategory robotCategory = new ManagedCharacterCategory();
        robotCategory.categoryName = "🤖 ROBOT";
        robotCategory.categoryColor = new Color(0.3f, 0.6f, 1f, 1f);
        categories.Add(robotCategory);
        
        // Monster Category
        ManagedCharacterCategory monsterCategory = new ManagedCharacterCategory();
        monsterCategory.categoryName = "👹 QUÁI VẬT";
        monsterCategory.categoryColor = new Color(1f, 0.3f, 0.3f, 1f);
        categories.Add(monsterCategory);
        
        // Zombie Category
        ManagedCharacterCategory zombieCategory = new ManagedCharacterCategory();
        zombieCategory.categoryName = "🧟 ZOMBIE";
        zombieCategory.categoryColor = new Color(0.5f, 0.8f, 0.3f, 1f);
        categories.Add(zombieCategory);
        
        Debug.Log("Initialized default categories");
        
        #if UNITY_EDITOR
        // Mark as dirty for editor
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
    
    /// <summary>
    /// Add new category
    /// </summary>
    [ContextMenu("Add New Category")]
    public void AddNewCategory()
    {
        ManagedCharacterCategory newCategory = new ManagedCharacterCategory();
        newCategory.categoryName = $"Category {categories.Count + 1}";
        newCategory.categoryColor = new Color(Random.value, Random.value, Random.value, 1f);
        categories.Add(newCategory);
        
        Debug.Log($"Added new category: {newCategory.categoryName}");
    }
    
    /// <summary>
    /// Auto setup prefabs for characters that don't have components
    /// </summary>
    [ContextMenu("Auto Setup All Prefabs")]
    public void AutoSetupAllPrefabs()
    {
        int setupCount = 0;
        
        foreach (var category in categories)
        {
            foreach (var character in category.characters)
            {
                if (character.prefab != null)
                {
                    if (AutoSetupPrefab(character.prefab, character))
                    {
                        setupCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Auto setup completed for {setupCount} prefabs");
    }
    
    /// <summary>
    /// Auto setup a single prefab
    /// </summary>
    private bool AutoSetupPrefab(GameObject prefab, CharacterEntry character)
    {
        if (!autoSetupPrefabs) return false;
        
        bool needsSetup = false;
        
        // Check if prefab needs RagdollCharacter component
        RagdollCharacter ragdoll = prefab.GetComponent<RagdollCharacter>();
        if (ragdoll == null)
        {
            // Note: In a real scenario, you'd want to modify the prefab asset
            // This is just for demonstration
            Debug.Log($"Prefab {prefab.name} needs RagdollCharacter component");
            needsSetup = true;
        }
        
        // Check if prefab needs NavMeshAgent
        UnityEngine.AI.NavMeshAgent agent = prefab.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.Log($"Prefab {prefab.name} needs NavMeshAgent component");
            needsSetup = true;
        }
        
        return needsSetup;
    }
    
    /// <summary>
    /// Refresh UI with current character data - DISABLED (using AutoUIGenerator instead)
    /// </summary>
    [ContextMenu("Refresh UI")]
    public void RefreshUI()
    {
        Debug.Log("UI refresh disabled - using AutoUIGenerator instead");

        // AutoUIGenerator will handle UI updates automatically
        AutoUIGenerator autoUI = FindAnyObjectByType<AutoUIGenerator>();
        if (autoUI != null)
        {
            Debug.Log("AutoUIGenerator found - UI will be handled automatically");
            autoUI.GenerateUI();
        }
    }
    
    /// <summary>
    /// Get all characters from all categories
    /// </summary>
    public List<CharacterEntry> GetAllCharacters()
    {
        List<CharacterEntry> allCharacters = new List<CharacterEntry>();
        foreach (var category in categories)
        {
            allCharacters.AddRange(category.characters);
        }
        return allCharacters;
    }
    
    /// <summary>
    /// Get characters from specific category
    /// </summary>
    public List<CharacterEntry> GetCharactersFromCategory(string categoryName)
    {
        foreach (var category in categories)
        {
            if (category.categoryName == categoryName)
            {
                return category.characters;
            }
        }
        return new List<CharacterEntry>();
    }
    
    /// <summary>
    /// Add character to specific category
    /// </summary>
    public void AddCharacterToCategory(string categoryName, CharacterEntry character)
    {
        foreach (var category in categories)
        {
            if (category.categoryName == categoryName)
            {
                category.characters.Add(character);
                if (autoRefreshUI) RefreshUI();
                return;
            }
        }
        
        // Category not found, create new one
        ManagedCharacterCategory newCategory = new ManagedCharacterCategory();
        newCategory.categoryName = categoryName;
        newCategory.characters.Add(character);
        categories.Add(newCategory);
        
        if (autoRefreshUI) RefreshUI();
    }
    
    /// <summary>
    /// Remove character from all categories
    /// </summary>
    public bool RemoveCharacter(CharacterEntry character)
    {
        foreach (var category in categories)
        {
            if (category.characters.Remove(character))
            {
                if (autoRefreshUI) RefreshUI();
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Get statistics
    /// </summary>
    [ContextMenu("Print Statistics")]
    public void PrintStatistics()
    {
        Debug.Log("=== CHARACTER MANAGER STATISTICS ===");
        Debug.Log($"Total Categories: {categories.Count}");
        Debug.Log($"Total Characters: {GetAllCharacters().Count}");
        
        foreach (var category in categories)
        {
            Debug.Log($"  {category.categoryName}: {category.characters.Count} characters");
        }
    }
    
    /// <summary>
    /// Validate all character data
    /// </summary>
    [ContextMenu("Validate Character Data")]
    public void ValidateCharacterData()
    {
        int issues = 0;
        
        foreach (var category in categories)
        {
            foreach (var character in category.characters)
            {
                if (character.prefab == null)
                {
                    Debug.LogWarning($"Character '{character.characterName}' in category '{category.categoryName}' has no prefab assigned");
                    issues++;
                }
                
                if (string.IsNullOrEmpty(character.characterName))
                {
                    Debug.LogWarning($"Character in category '{category.categoryName}' has no name");
                    issues++;
                }
                
                if (character.health <= 0)
                {
                    Debug.LogWarning($"Character '{character.characterName}' has invalid health: {character.health}");
                    issues++;
                }
            }
        }
        
        if (issues == 0)
        {
            Debug.Log("✅ All character data is valid!");
        }
        else
        {
            Debug.LogWarning($"⚠️ Found {issues} issues in character data");
        }
    }
    
    // Public properties for external access
    public List<ManagedCharacterCategory> Categories => categories;
    public int CategoryCount => categories.Count;
    public int TotalCharacterCount => GetAllCharacters().Count;
}