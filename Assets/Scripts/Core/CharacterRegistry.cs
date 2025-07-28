using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Single source of truth for all character data in the unified character system
/// Provides auto-discovery, validation, and fast lookup capabilities
/// </summary>
[CreateAssetMenu(fileName = "CharacterRegistry", menuName = "Game/Character Registry")]
public class CharacterRegistry : ScriptableObject
{
    [Header("🔍 Auto-Discovery Settings")]
    [Tooltip("Enable automatic character discovery from folder structure")]
    public bool enableAutoDiscovery = true;
    
    [Tooltip("Base path for character discovery")]
    public string basePath = "Assets/Resources/Characters";
    
    [Tooltip("Automatically generate icons from prefabs if missing")]
    public bool autoGenerateIcons = true;
    
    [Header("📋 Character Data")]
    [Tooltip("List of all characters in the system")]
    public List<CharacterEntry> characters = new List<CharacterEntry>();
    
    [Header("📊 Runtime Cache")]
    [SerializeField] private bool cacheInitialized = false;
    [SerializeField] private int lastCacheUpdate = 0;
    
    // Runtime lookup tables for performance
    private Dictionary<string, CharacterEntry> characterLookup;
    private Dictionary<string, List<CharacterEntry>> categoryLookup;
    
    [System.Serializable]
    public class CharacterEntry
    {
        [Header("🆔 Identity")]
        [Tooltip("Unique identifier for this character")]
        public string id;
        
        [Tooltip("Display name shown in UI")]
        public string displayName;
        
        [Tooltip("Category this character belongs to")]
        public string category;
        
        [Header("🎮 Assets")]
        [Tooltip("Character prefab")]
        public GameObject prefab;
        
        [Tooltip("Character icon for UI")]
        public Sprite icon;
        
        [Header("📊 Stats")]
        [Tooltip("Character health points")]
        public float health = 100f;
        
        [Tooltip("Movement speed")]
        public float speed = 3f;
        
        [Tooltip("Attack damage")]
        public float damage = 20f;
        
        [Header("⚙️ Settings")]
        [Tooltip("Is this character active and available")]
        public bool isActive = true;
        
        [Tooltip("Sort order in UI")]
        public int sortOrder = 0;
        
        [Header("🔍 Discovery Info")]
        [Tooltip("Was this entry auto-discovered")]
        public bool autoDiscovered = false;
        
        [Tooltip("Last discovery timestamp")]
        public string lastDiscovered;
        
        // Validation
        public bool IsValid => prefab != null && !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(category);
        
        // Display info for inspector
        public string GetDisplayInfo()
        {
            return $"{displayName} ({category}) - {(IsValid ? "✅" : "❌")}";
        }
    }
    
    #region Auto-Discovery
    
    [ContextMenu("🔍 Auto Discover Characters")]
    public void AutoDiscoverCharacters()
    {
        if (!enableAutoDiscovery)
        {
            Debug.LogWarning("⚠️ Auto-discovery is disabled in settings");
            return;
        }
        
        Debug.Log("🔍 Starting character auto-discovery...");
        
        try
        {
            // Clear existing auto-discovered entries
            characters.RemoveAll(c => c.autoDiscovered);
            
            // Discover characters
            int discoveredCount = DiscoverCharactersFromFolders();
            
            // Rebuild lookup tables
            RebuildLookupTables();
            
            Debug.Log($"✅ Auto-discovery complete! Found {discoveredCount} characters");
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Auto-discovery failed: {e.Message}");
        }
    }
    
    private int DiscoverCharactersFromFolders()
    {
        int discoveredCount = 0;
        
        if (!Directory.Exists(basePath))
        {
            Debug.LogWarning($"⚠️ Base path does not exist: {basePath}");
            return 0;
        }
        
        // Get all category folders
        string[] categoryFolders = Directory.GetDirectories(basePath);
        
        foreach (string categoryFolder in categoryFolders)
        {
            string categoryName = Path.GetFileName(categoryFolder).ToUpper();
            discoveredCount += DiscoverCharactersInCategory(categoryFolder, categoryName);
        }
        
        return discoveredCount;
    }
    
    private int DiscoverCharactersInCategory(string categoryPath, string categoryName)
    {
        int discoveredCount = 0;
        
        // Get all prefab files in category folder
        string[] prefabFiles = Directory.GetFiles(categoryPath, "*.prefab", SearchOption.AllDirectories);
        
        foreach (string prefabPath in prefabFiles)
        {
            #if UNITY_EDITOR
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) continue;
            
            // Parse naming convention: Category_Name.prefab
            string fileName = Path.GetFileNameWithoutExtension(prefabPath);
            string[] parts = fileName.Split('_');
            
            if (parts.Length < 2)
            {
                Debug.LogWarning($"⚠️ Prefab {fileName} doesn't follow naming convention Category_Name");
                continue;
            }
            
            // Create character entry
            CharacterEntry entry = new CharacterEntry();
            entry.id = $"auto_{fileName.ToLower()}";
            entry.category = categoryName;
            entry.displayName = string.Join(" ", parts.Skip(1)); // Join all parts after category
            entry.prefab = prefab;
            entry.autoDiscovered = true;
            entry.lastDiscovered = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // Auto-discover icon
            entry.icon = FindIconForPrefab(prefabPath);
            
            // Auto-extract stats from prefab
            ExtractStatsFromPrefab(entry, prefab);
            
            characters.Add(entry);
            discoveredCount++;
            
            Debug.Log($"📦 Discovered: {entry.displayName} in {categoryName}");
            #endif
        }
        
        return discoveredCount;
    }
    
    private Sprite FindIconForPrefab(string prefabPath)
    {
        #if UNITY_EDITOR
        // Look for icon with same name as prefab
        string iconPath = prefabPath.Replace(".prefab", ".png");
        Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        
        if (icon == null)
        {
            // Try .jpg extension
            iconPath = prefabPath.Replace(".prefab", ".jpg");
            icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        }
        
        if (icon == null && autoGenerateIcons)
        {
            // TODO: Generate icon from prefab (placeholder for now)
            Debug.Log($"🎨 Icon generation needed for: {Path.GetFileNameWithoutExtension(prefabPath)}");
        }
        
        return icon;
        #else
        return null;
        #endif
    }
    
    private void ExtractStatsFromPrefab(CharacterEntry entry, GameObject prefab)
    {
        // Extract stats from RagdollCharacter component
        RagdollCharacter ragdoll = prefab.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            entry.health = ragdoll.maxHealth;
            // entry.speed = ragdoll.moveSpeed; // If available
            // entry.damage = ragdoll.attackDamage; // If available
        }
        
        // Extract speed from NavMeshAgent
        UnityEngine.AI.NavMeshAgent agent = prefab.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            entry.speed = agent.speed;
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Get character by unique ID
    /// </summary>
    public CharacterEntry GetCharacter(string id)
    {
        if (characterLookup == null) RebuildLookupTables();
        characterLookup.TryGetValue(id, out CharacterEntry character);
        return character;
    }
    
    /// <summary>
    /// Get all characters in a specific category
    /// </summary>
    public List<CharacterEntry> GetCharactersByCategory(string category)
    {
        if (categoryLookup == null) RebuildLookupTables();
        categoryLookup.TryGetValue(category, out List<CharacterEntry> characters);
        return characters ?? new List<CharacterEntry>();
    }
    
    /// <summary>
    /// Get all unique categories
    /// </summary>
    public List<string> GetAllCategories()
    {
        return characters.Where(c => c.IsValid && c.isActive)
                        .Select(c => c.category)
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();
    }
    
    /// <summary>
    /// Get all valid and active characters
    /// </summary>
    public List<CharacterEntry> GetAllActiveCharacters()
    {
        return characters.Where(c => c.IsValid && c.isActive)
                        .OrderBy(c => c.category)
                        .ThenBy(c => c.sortOrder)
                        .ThenBy(c => c.displayName)
                        .ToList();
    }
    
    /// <summary>
    /// Add a new character entry
    /// </summary>
    public void AddCharacter(CharacterEntry character)
    {
        if (character == null || !character.IsValid)
        {
            Debug.LogError("❌ Cannot add invalid character");
            return;
        }
        
        // Check for duplicate IDs
        if (GetCharacter(character.id) != null)
        {
            Debug.LogError($"❌ Character with ID '{character.id}' already exists");
            return;
        }
        
        characters.Add(character);
        RebuildLookupTables();
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        
        Debug.Log($"✅ Added character: {character.displayName}");
    }
    
    #endregion
    
    #region Validation & Maintenance
    
    [ContextMenu("✅ Validate All Characters")]
    public void ValidateAllCharacters()
    {
        Debug.Log("🔍 Validating all characters...");
        
        int validCount = 0;
        int invalidCount = 0;
        
        foreach (var character in characters)
        {
            if (character.IsValid)
            {
                validCount++;
            }
            else
            {
                invalidCount++;
                Debug.LogWarning($"⚠️ Invalid character: {character.displayName} - Missing: {GetMissingFields(character)}");
            }
        }
        
        Debug.Log($"✅ Validation complete - Valid: {validCount}, Invalid: {invalidCount}");
    }
    
    private string GetMissingFields(CharacterEntry character)
    {
        List<string> missing = new List<string>();
        
        if (character.prefab == null) missing.Add("Prefab");
        if (string.IsNullOrEmpty(character.displayName)) missing.Add("Display Name");
        if (string.IsNullOrEmpty(character.category)) missing.Add("Category");
        
        return string.Join(", ", missing);
    }
    
    [ContextMenu("🔄 Rebuild Lookup Tables")]
    public void RebuildLookupTables()
    {
        characterLookup = characters.Where(c => c.IsValid)
                                  .ToDictionary(c => c.id, c => c);
        
        categoryLookup = characters.Where(c => c.IsValid)
                                 .GroupBy(c => c.category)
                                 .ToDictionary(g => g.Key, g => g.ToList());
        
        cacheInitialized = true;
        lastCacheUpdate = System.DateTime.Now.GetHashCode();
        
        Debug.Log($"🔄 Lookup tables rebuilt - {characterLookup.Count} characters, {categoryLookup.Count} categories");
    }
    
    #endregion
    
    private void OnValidate()
    {
        // Rebuild lookup tables when data changes in inspector
        if (Application.isPlaying && cacheInitialized)
        {
            RebuildLookupTables();
        }
    }
}
