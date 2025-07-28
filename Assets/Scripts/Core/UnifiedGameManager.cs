using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// Unified Game Manager - Replaces multiple legacy managers with a single, simplified system
/// Handles character spawning, team management, and system integration
/// </summary>
public class UnifiedGameManager : MonoBehaviour
{
    [Header("🎯 Core References")]
    [Tooltip("Character registry containing all character data")]
    public CharacterRegistry characterRegistry;
    
    [Tooltip("Parent transform for spawned characters")]
    public Transform spawnParent;
    
    [Header("👥 Team Settings")]
    // ✅ DISABLED: Team materials and colors - loại bỏ hoàn toàn chức năng màu team
    /*
    [Tooltip("Materials for each team (Team 1-4)")]
    public Material[] teamMaterials = new Material[4];

    [Tooltip("Colors for each team (used for previews)")]
    public Color[] teamColors = { Color.blue, Color.red, Color.green, Color.yellow };
    */
    
    [Header("⚙️ Current State")]
    [Tooltip("Currently selected team")]
    public int selectedTeam = 1;
    
    [Tooltip("Currently selected category")]
    public string selectedCategory = "ROBOT";
    
    [Header("🔄 Legacy Integration")]
    [Tooltip("Enable bridge to legacy BattleGameManager")]
    public bool enableLegacyBridge = true;
    
    [Tooltip("Reference to legacy BattleGameManager")]
    public BattleGameManager legacyBattleManager;
    
    [Header("📊 Runtime Stats")]
    [SerializeField] private int charactersSpawned = 0;
    [SerializeField] private bool systemInitialized = false;
    
    // Singleton pattern
    private static UnifiedGameManager _instance;
    public static UnifiedGameManager Instance => _instance;
    
    private void Awake()
    {
        // Singleton setup
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning("⚠️ Multiple UnifiedGameManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        InitializeSystem();
    }
    
    private void InitializeSystem()
    {
        Debug.Log("🚀 UnifiedGameManager: Initializing system...");
        
        try
        {
            // Validate references
            if (characterRegistry == null)
            {
                Debug.LogError("❌ CharacterRegistry not assigned!");
                return;
            }
            
            // Setup spawn parent if not assigned
            if (spawnParent == null)
            {
                GameObject spawnContainer = GameObject.Find("SpawnedCharacters");
                if (spawnContainer == null)
                {
                    spawnContainer = new GameObject("SpawnedCharacters");
                }
                spawnParent = spawnContainer.transform;
                Debug.Log("📦 Created spawn parent container");
            }
            
            // Setup legacy bridge if enabled
            if (enableLegacyBridge)
            {
                SetupLegacyBridge();
            }
            
            // Initialize character registry
            if (characterRegistry.enableAutoDiscovery)
            {
                characterRegistry.AutoDiscoverCharacters();
            }
            
            systemInitialized = true;
            Debug.Log($"✅ UnifiedGameManager initialized with {characterRegistry.characters.Count} characters");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ UnifiedGameManager initialization failed: {e.Message}");
        }
    }
    
    #region Public API
    
    /// <summary>
    /// Main method to spawn character - replaces all existing spawn methods
    /// </summary>
    public GameObject SpawnCharacter(string characterId, Vector3 position)
    {
        if (!systemInitialized)
        {
            Debug.LogError("❌ System not initialized!");
            return null;
        }
        
        var character = characterRegistry.GetCharacter(characterId);
        if (character?.prefab == null)
        {
            Debug.LogError($"❌ Character not found: {characterId}");
            return null;
        }
        
        try
        {
            // Instantiate character
            GameObject instance = Instantiate(character.prefab, position, Quaternion.identity, spawnParent);
            instance.name = $"{character.displayName} (Team {selectedTeam})";
            
            // Setup character
            SetupCharacterTeam(instance, selectedTeam);
            SetupCharacterAI(instance);
            SetupCharacterHealth(instance);
            
            charactersSpawned++;
            Debug.Log($"✅ Spawned {character.displayName} for team {selectedTeam} at {position}");
            
            return instance;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to spawn character {characterId}: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Get all characters in a specific category
    /// </summary>
    public List<CharacterRegistry.CharacterEntry> GetCharactersByCategory(string category)
    {
        if (characterRegistry == null) return new List<CharacterRegistry.CharacterEntry>();
        return characterRegistry.GetCharactersByCategory(category);
    }
    
    /// <summary>
    /// Get all available categories
    /// </summary>
    public List<string> GetAllCategories()
    {
        if (characterRegistry == null) return new List<string>();
        return characterRegistry.GetAllCategories();
    }
    
    /// <summary>
    /// Set the currently selected team
    /// </summary>
    public void SetSelectedTeam(int teamId)
    {
        selectedTeam = Mathf.Clamp(teamId, 1, 4);
        Debug.Log($"🎯 Selected team: {selectedTeam}");
        
        // Sync with legacy system if enabled
        if (enableLegacyBridge && legacyBattleManager != null)
        {
            legacyBattleManager.selectedTeam = selectedTeam;
        }
    }
    
    /// <summary>
    /// Set the currently selected category
    /// </summary>
    public void SetSelectedCategory(string category)
    {
        selectedCategory = category;
        Debug.Log($"📂 Selected category: {category}");
    }
    
    /// <summary>
    /// DISABLED: Get team color for UI previews - loại bỏ hoàn toàn chức năng màu team
    /// </summary>
    /*
    public Color GetTeamColor(int teamId)
    {
        if (teamId > 0 && teamId <= teamColors.Length)
        {
            return teamColors[teamId - 1];
        }
        return Color.white;
    }
    */

    /// <summary>
    /// DISABLED: Get team material for character rendering - loại bỏ hoàn toàn chức năng màu team
    /// </summary>
    /*
    public Material GetTeamMaterial(int teamId)
    {
        if (teamId > 0 && teamId <= teamMaterials.Length)
        {
            return teamMaterials[teamId - 1];
        }
        return null;
    }
    */

    /// <summary>
    /// Enable AI for all spawned characters (called when battle starts)
    /// </summary>
    public void EnableAllAI()
    {
        SimpleCharacterAI[] allAI = FindObjectsOfType<SimpleCharacterAI>();
        int enabledCount = 0;

        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null)
            {
                ai.enabled = true;
                enabledCount++;
            }
        }

        Debug.Log($"✅ Enabled AI for {enabledCount} characters - Battle started!");
    }

    /// <summary>
    /// Disable AI for all spawned characters (called when battle ends/resets)
    /// </summary>
    public void DisableAllAI()
    {
        SimpleCharacterAI[] allAI = FindObjectsOfType<SimpleCharacterAI>();
        int disabledCount = 0;

        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null)
            {
                ai.enabled = false;
                disabledCount++;
            }
        }

        Debug.Log($"⏸️ Disabled AI for {disabledCount} characters - Battle stopped!");
    }

    #endregion
    
    #region Character Setup
    
    private void SetupCharacterTeam(GameObject character, int teamId)
    {
        // Setup RagdollCharacter team
        RagdollCharacter ragdoll = character.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            ragdoll.teamId = teamId;
        }
        else
        {
            // Add RagdollCharacter if missing
            ragdoll = character.AddComponent<RagdollCharacter>();
            ragdoll.teamId = teamId;
            ragdoll.maxHealth = 100f; // Default values
        }

        // ✅ Loại bỏ việc áp dụng team material - giữ nguyên texture gốc
        Debug.Log($"✅ Set team {teamId} for character: {character.name} (keeping original texture)");
    }
    
    private void SetupCharacterAI(GameObject character)
    {
        // Ensure NavMeshAgent exists
        NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = character.AddComponent<NavMeshAgent>();
            agent.speed = 3f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.stoppingDistance = 1.5f;
        }
        
        // Ensure AI component exists but keep it disabled initially
        SimpleCharacterAI ai = character.GetComponent<SimpleCharacterAI>();
        if (ai == null)
        {
            ai = character.AddComponent<SimpleCharacterAI>();
        }

        // AI should only be enabled when battle starts
        bool battleStarted = (legacyBattleManager != null && legacyBattleManager.gameStarted);
        ai.enabled = battleStarted;

        if (battleStarted)
        {
            Debug.Log($"✅ AI enabled for {character.name} - Battle is active");
        }
        else
        {
            Debug.Log($"⏸️ AI disabled for {character.name} - Waiting for battle start");
        }
    }
    
    private void SetupCharacterHealth(GameObject character)
    {
        HealthBarController healthController = character.GetComponent<HealthBarController>();
        if (healthController != null)
        {
            healthController.RefreshHealthBar();
        }
    }
    
    #endregion

    #region Legacy Bridge

    private void SetupLegacyBridge()
    {
        Debug.Log("🔗 Setting up legacy bridge...");

        // Find legacy BattleGameManager if not assigned
        if (legacyBattleManager == null)
        {
            legacyBattleManager = FindObjectOfType<BattleGameManager>();
        }

        if (legacyBattleManager != null)
        {
            // Sync character prefabs with registry
            SyncCharacterPrefabsWithLegacy();

            // Sync team selection
            selectedTeam = legacyBattleManager.selectedTeam;

            Debug.Log("✅ Legacy bridge established");
        }
        else
        {
            Debug.LogWarning("⚠️ Legacy BattleGameManager not found");
        }
    }

    private void SyncCharacterPrefabsWithLegacy()
    {
        if (legacyBattleManager == null || characterRegistry == null) return;

        // Update legacy manager's character prefabs array with registry data
        List<GameObject> allPrefabs = new List<GameObject>();
        foreach (var character in characterRegistry.characters)
        {
            if (character.IsValid && character.isActive)
            {
                allPrefabs.Add(character.prefab);
            }
        }

        legacyBattleManager.characterPrefabs = allPrefabs.ToArray();
        Debug.Log($"🔗 Synced {allPrefabs.Count} prefabs with legacy system");
    }

    /// <summary>
    /// Bridge method for legacy spawn calls
    /// </summary>
    public GameObject LegacySpawnCharacter(GameObject prefab, Vector3 position)
    {
        // Find character by prefab
        var character = characterRegistry.characters.Find(c => c.prefab == prefab);
        if (character != null)
        {
            return SpawnCharacter(character.id, position);
        }

        // Fallback to direct instantiation
        Debug.LogWarning($"⚠️ Legacy spawn fallback for {prefab.name}");
        GameObject instance = Instantiate(prefab, position, Quaternion.identity, spawnParent);
        SetupCharacterTeam(instance, selectedTeam);
        SetupCharacterAI(instance);
        return instance;
    }

    #endregion

    #region Debug & Maintenance

    [ContextMenu("📊 Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("=== UNIFIED GAME MANAGER STATUS ===");
        Debug.Log($"System Initialized: {systemInitialized}");
        Debug.Log($"Characters Spawned: {charactersSpawned}");
        Debug.Log($"Selected Team: {selectedTeam}");
        Debug.Log($"Selected Category: {selectedCategory}");
        Debug.Log($"Legacy Bridge: {(enableLegacyBridge ? "ENABLED" : "DISABLED")}");

        if (characterRegistry != null)
        {
            Debug.Log($"Registry Characters: {characterRegistry.characters.Count}");
            Debug.Log($"Registry Categories: {characterRegistry.GetAllCategories().Count}");
        }
        else
        {
            Debug.Log("Registry: NOT ASSIGNED");
        }
    }

    [ContextMenu("🔄 Refresh Character Registry")]
    public void RefreshCharacterRegistry()
    {
        if (characterRegistry != null)
        {
            characterRegistry.AutoDiscoverCharacters();

            if (enableLegacyBridge)
            {
                SyncCharacterPrefabsWithLegacy();
            }

            Debug.Log("🔄 Character registry refreshed");
        }
    }

    [ContextMenu("🧹 Clear Spawned Characters")]
    public void ClearSpawnedCharacters()
    {
        if (spawnParent != null)
        {
            int childCount = spawnParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(spawnParent.GetChild(i).gameObject);
            }

            charactersSpawned = 0;
            Debug.Log($"🧹 Cleared {childCount} spawned characters");
        }
    }

    #endregion

    private void OnValidate()
    {
        // Clamp team selection
        selectedTeam = Mathf.Clamp(selectedTeam, 1, 4);

        // Sync with legacy system if enabled and playing
        if (Application.isPlaying && enableLegacyBridge && legacyBattleManager != null)
        {
            legacyBattleManager.selectedTeam = selectedTeam;
        }
    }
}
