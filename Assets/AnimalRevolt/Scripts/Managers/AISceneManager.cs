using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manager chính cho toàn bộ AI system trong scene
/// Quản lý spawn, team setup, performance monitoring
/// </summary>
public class AISceneManager : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private bool autoInitializeOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Team Configuration")]
    [SerializeField] private Transform[] blueTeamSpawnPoints;
    [SerializeField] private Transform[] redTeamSpawnPoints;
    [SerializeField] private GameObject[] characterPrefabs;
    
    [Header("AI Settings")]
    [SerializeField] private AISceneSettings sceneSettings;
    
    [Header("Performance")]
    [SerializeField] private int maxAICharacters = 20;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private bool enablePerformanceMonitoring = true;
    
    // Runtime data
    private List<AICharacterSetup> allAICharacters = new List<AICharacterSetup>();
    private List<AICharacterSetup> blueTeamCharacters = new List<AICharacterSetup>();
    private List<AICharacterSetup> redTeamCharacters = new List<AICharacterSetup>();
    
    private AIPerformanceManager performanceManager;
    
    // Events
    public System.Action<AICharacterSetup> OnCharacterSpawned;
    public System.Action<AICharacterSetup> OnCharacterDestroyed;
    public System.Action<TeamType> OnTeamEliminated;
    public System.Action<TeamType> OnTeamVictory;
    
    // Properties
    public int TotalCharacters => allAICharacters.Count;
    public int BlueTeamCount => blueTeamCharacters.Count(c => c != null && c.TeamMember.IsAlive);
    public int RedTeamCount => redTeamCharacters.Count(c => c != null && c.TeamMember.IsAlive);
    public bool IsBattleActive => BlueTeamCount > 0 && RedTeamCount > 0;
    
    private void Start()
    {
        if (autoInitializeOnStart)
        {
            InitializeScene();
        }
    }
    
    /// <summary>
    /// Initialize toàn bộ scene
    /// </summary>
    [ContextMenu("Initialize Scene")]
    public void InitializeScene()
    {
        if (debugMode)
            Debug.Log("=== AISceneManager: Initializing Scene ===");
        
        // Setup performance manager
        SetupPerformanceManager();
        
        // Find existing AI characters
        FindExistingCharacters();
        
        // Setup scene settings
        ApplySceneSettings();
        
        // Validate NavMesh
        ValidateNavMesh();
        
        if (debugMode)
            Debug.Log($"Scene initialized with {TotalCharacters} AI characters");
    }
    
    /// <summary>
    /// Setup performance manager
    /// </summary>
    private void SetupPerformanceManager()
    {
        if (enablePerformanceMonitoring)
        {
            performanceManager = gameObject.GetComponent<AIPerformanceManager>();
            if (performanceManager == null)
                performanceManager = gameObject.AddComponent<AIPerformanceManager>();
        }
    }
    
    /// <summary>
    /// Tìm và register tất cả AI characters có sẵn trong scene
    /// </summary>
    private void FindExistingCharacters()
    {
        allAICharacters.Clear();
        blueTeamCharacters.Clear();
        redTeamCharacters.Clear();
        
        // Tìm tất cả AICharacterSetup trong scene
        AICharacterSetup[] foundCharacters = FindObjectsOfType<AICharacterSetup>();
        
        foreach (AICharacterSetup character in foundCharacters)
        {
            RegisterCharacter(character);
        }
        
        if (debugMode)
            Debug.Log($"Found {foundCharacters.Length} existing AI characters");
    }
    
    /// <summary>
    /// Register một character vào manager
    /// </summary>
    public void RegisterCharacter(AICharacterSetup character)
    {
        if (character == null) return;
        
        // Setup character nếu chưa setup
        if (!character.IsSetupComplete)
        {
            character.SetupAICharacter();
        }
        
        // Add to lists
        if (!allAICharacters.Contains(character))
        {
            allAICharacters.Add(character);
            
            // Add to team lists
            if (character.TeamMember != null)
            {
                switch (character.TeamMember.TeamType)
                {
                    case TeamType.AI_Team1:
                        blueTeamCharacters.Add(character);
                        break;
                    case TeamType.AI_Team2:
                        redTeamCharacters.Add(character);
                        break;
                }
            }
            
            // Subscribe to death event
            if (character.TeamMember != null)
            {
                character.TeamMember.OnDeath += OnCharacterDeath;
            }
            
            OnCharacterSpawned?.Invoke(character);
            
            if (debugMode)
                Debug.Log($"Registered character: {character.name}");
        }
    }
    
    /// <summary>
    /// Spawn AI character tại vị trí cụ thể
    /// </summary>
    public AICharacterSetup SpawnCharacter(GameObject prefab, Vector3 position, TeamType team)
    {
        if (TotalCharacters >= maxAICharacters)
        {
            if (debugMode)
                Debug.LogWarning("Max AI characters reached!");
            return null;
        }
        
        if (prefab == null)
        {
            Debug.LogError("Cannot spawn character: prefab is null");
            return null;
        }
        
        // Instantiate
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.name = $"{prefab.name}_{team}_{TotalCharacters + 1}";
        
        // Setup AI Character
        AICharacterSetup setup = instance.GetComponent<AICharacterSetup>();
        if (setup == null)
        {
            setup = instance.AddComponent<AICharacterSetup>();
        }
        
        // Configure team using setter method
        setup.SetTeamType(team);
        
        // Setup character
        setup.SetupAICharacter();
        
        // Register
        RegisterCharacter(setup);
        
        if (debugMode)
            Debug.Log($"Spawned character: {instance.name} at {position}");
        
        return setup;
    }
    
    /// <summary>
    /// Spawn team hoàn chỉnh
    /// </summary>
    public void SpawnTeam(TeamType team, int count, GameObject[] prefabs = null)
    {
        Transform[] spawnPoints = team == TeamType.AI_Team1 ? blueTeamSpawnPoints : redTeamSpawnPoints;
        GameObject[] usePrefabs = prefabs ?? characterPrefabs;
        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError($"No spawn points defined for {team} team!");
            return;
        }
        
        if (usePrefabs == null || usePrefabs.Length == 0)
        {
            Debug.LogError("No character prefabs available!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            // Chọn spawn point
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            
            // Chọn prefab
            GameObject prefab = usePrefabs[Random.Range(0, usePrefabs.Length)];
            
            // Spawn
            SpawnCharacter(prefab, spawnPoint.position, team);
        }
        
        if (debugMode)
            Debug.Log($"Spawned {team} team with {count} characters");
    }
    
    /// <summary>
    /// Apply scene settings cho tất cả characters
    /// </summary>
    private void ApplySceneSettings()
    {
        if (sceneSettings == null) return;
        
        foreach (AICharacterSetup character in allAICharacters)
        {
            if (character == null) continue;
            
            // Apply settings through reflection để access private fields
            ApplySettingsToCharacter(character, sceneSettings);
        }
        
        if (debugMode)
            Debug.Log("Applied scene settings to all characters");
    }
    
    /// <summary>
    /// Apply settings cho một character cụ thể
    /// </summary>
    private void ApplySettingsToCharacter(AICharacterSetup character, AISceneSettings settings)
    {
        if (character.AIMovement != null)
        {
            // Update movement settings
            var walkSpeedField = typeof(AIMovementController).GetField("walkSpeed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var runSpeedField = typeof(AIMovementController).GetField("runSpeed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            walkSpeedField?.SetValue(character.AIMovement, settings.walkSpeed);
            runSpeedField?.SetValue(character.AIMovement, settings.runSpeed);
        }
        
        if (character.CombatController != null)
        {
            // Update combat settings
            var damageField = typeof(CombatController).GetField("attackDamage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var rangeField = typeof(CombatController).GetField("attackRange", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            damageField?.SetValue(character.CombatController, settings.attackDamage);
            rangeField?.SetValue(character.CombatController, settings.attackRange);
        }
    }
    
    /// <summary>
    /// Validate NavMesh trong scene
    /// </summary>
    private void ValidateNavMesh()
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        
        if (triangulation.vertices.Length == 0)
        {
            Debug.LogError("No NavMesh found in scene! Please bake NavMesh.");
            return;
        }
        
        if (debugMode)
            Debug.Log($"NavMesh validation OK - {triangulation.vertices.Length} vertices");
    }
    
    /// <summary>
    /// Handle character death
    /// </summary>
    private void OnCharacterDeath(TeamMember deadCharacter)
    {
        if (debugMode)
            Debug.Log($"Character died: {deadCharacter.gameObject.name}");
        
        // Check for team elimination
        if (BlueTeamCount == 0)
        {
            OnTeamEliminated?.Invoke(TeamType.AI_Team1);
            OnTeamVictory?.Invoke(TeamType.AI_Team2);
            
            if (debugMode)
                Debug.Log("Red Team Victory!");
        }
        else if (RedTeamCount == 0)
        {
            OnTeamEliminated?.Invoke(TeamType.AI_Team2);
            OnTeamVictory?.Invoke(TeamType.AI_Team1);
            
            if (debugMode)
                Debug.Log("Blue Team Victory!");
        }
    }
    
    /// <summary>
    /// Clean up destroyed characters
    /// </summary>
    public void CleanupDestroyedCharacters()
    {
        allAICharacters.RemoveAll(c => c == null);
        blueTeamCharacters.RemoveAll(c => c == null);
        redTeamCharacters.RemoveAll(c => c == null);
    }
    
    /// <summary>
    /// Reset scene - respawn all characters
    /// </summary>
    [ContextMenu("Reset Scene")]
    public void ResetScene()
    {
        // Destroy existing characters
        foreach (AICharacterSetup character in allAICharacters.ToArray())
        {
            if (character != null)
            {
                DestroyImmediate(character.gameObject);
            }
        }
        
        // Clear lists
        allAICharacters.Clear();
        blueTeamCharacters.Clear();
        redTeamCharacters.Clear();
        
        // Reinitialize
        InitializeScene();
        
        if (debugMode)
            Debug.Log("Scene reset complete");
    }
    
    /// <summary>
    /// Get performance stats
    /// </summary>
    public string GetPerformanceStats()
    {
        var stats = new System.Text.StringBuilder();
        stats.AppendLine("=== AI Scene Performance Stats ===");
        stats.AppendLine($"Total Characters: {TotalCharacters}");
        stats.AppendLine($"Blue Team Alive: {BlueTeamCount}");
        stats.AppendLine($"Red Team Alive: {RedTeamCount}");
        stats.AppendLine($"Battle Active: {IsBattleActive}");
        
        if (performanceManager != null)
        {
            // Add performance manager stats if available
            stats.AppendLine("Performance Manager: Active");
        }
        
        return stats.ToString();
    }
    
    /// <summary>
    /// Debug info
    /// </summary>
    [ContextMenu("Show Debug Info")]
    public void ShowDebugInfo()
    {
        Debug.Log(GetPerformanceStats());
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!debugMode) return;
        
        // Draw spawn points
        if (blueTeamSpawnPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in blueTeamSpawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 1f);
                    Gizmos.DrawLine(point.position, point.position + Vector3.up * 2f);
                }
            }
        }
        
        if (redTeamSpawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform point in redTeamSpawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 1f);
                    Gizmos.DrawLine(point.position, point.position + Vector3.up * 2f);
                }
            }
        }
    }
}

/// <summary>
/// ScriptableObject chứa settings cho scene
/// </summary>
[System.Serializable]
public class AISceneSettings
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 5f;
    
    [Header("Combat")]
    public float attackDamage = 25f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    
    [Header("Detection")]
    public float detectionRadius = 10f;
    public float engageDistance = 8f;
    
    [Header("Performance")]
    public float updateInterval = 0.1f;
    public int maxActiveCharacters = 20;
}