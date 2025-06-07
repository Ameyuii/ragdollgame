using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manager tối ưu performance cho multiple AI agents
/// Quản lý update intervals, culling, và resource pooling
/// </summary>
public class AIPerformanceManager : MonoBehaviour
{
    [Header("Performance Settings")]
    [SerializeField] private int maxActiveAI = 20;
    [SerializeField] private float updateDistance = 50f;
    [SerializeField] private float cullingDistance = 100f;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private bool enableLOD = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("LOD Settings")]
    [SerializeField] private float closeDistance = 15f;
    [SerializeField] private float mediumDistance = 30f;
    [SerializeField] private float farDistance = 50f;
    
    // Singleton
    public static AIPerformanceManager Instance { get; private set; }
    
    // AI Management
    private List<AIAgent> allAIAgents = new List<AIAgent>();
    private List<AIAgent> activeAIAgents = new List<AIAgent>();
    private Camera playerCamera;
    private Transform playerTransform;
    
    // Performance tracking
    private float lastUpdateTime;
    private int frameCounter = 0;
    private float avgFrameTime = 0f;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Find player camera
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();
    }
    
    private void Start()
    {
        // Register all existing AI agents
        RegisterExistingAI();
        
        if (debugMode)
            Debug.Log($"AIPerformanceManager initialized with {allAIAgents.Count} AI agents");
    }
    
    private void Update()
    {
        // Update với interval để tối ưu performance
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAIManagement();
            lastUpdateTime = Time.time;
        }
        
        // Performance tracking
        TrackPerformance();
    }
    
    /// <summary>
    /// Register AI agent với manager
    /// </summary>
    public void RegisterAI(AIAgent aiAgent)
    {
        if (!allAIAgents.Contains(aiAgent))
        {
            allAIAgents.Add(aiAgent);
            
            if (debugMode)
                Debug.Log($"Registered AI agent: {aiAgent.name}");
        }
    }
    
    /// <summary>
    /// Unregister AI agent
    /// </summary>
    public void UnregisterAI(AIAgent aiAgent)
    {
        allAIAgents.Remove(aiAgent);
        activeAIAgents.Remove(aiAgent);
        
        if (debugMode)
            Debug.Log($"Unregistered AI agent: {aiAgent.name}");
    }
    
    /// <summary>
    /// Main AI management update
    /// </summary>
    private void UpdateAIManagement()
    {
        if (playerCamera == null) return;
        
        Vector3 playerPosition = playerCamera.transform.position;
        
        // Clear active list
        activeAIAgents.Clear();
        
        // Evaluate all AI agents
        foreach (var aiAgent in allAIAgents)
        {
            if (aiAgent == null || !aiAgent.gameObject.activeInHierarchy) continue;
            
            float distance = Vector3.Distance(playerPosition, aiAgent.transform.position);
            
            // Culling - disable AI quá xa
            if (distance > cullingDistance)
            {
                aiAgent.SetActive(false);
                continue;
            }
            
            // Enable AI trong range
            if (distance <= updateDistance)
            {
                aiAgent.SetActive(true);
                activeAIAgents.Add(aiAgent);
                
                // Set LOD level
                if (enableLOD)
                {
                    AILODLevel lodLevel = GetLODLevel(distance);
                    aiAgent.SetLODLevel(lodLevel);
                }
            }
            else
            {
                aiAgent.SetActive(false);
            }
        }
        
        // Limit số lượng active AI
        if (activeAIAgents.Count > maxActiveAI)
        {
            // Sort by distance và disable những cái xa nhất
            activeAIAgents.Sort((a, b) => 
            {
                float distA = Vector3.Distance(playerPosition, a.transform.position);
                float distB = Vector3.Distance(playerPosition, b.transform.position);
                return distA.CompareTo(distB);
            });
            
            for (int i = maxActiveAI; i < activeAIAgents.Count; i++)
            {
                activeAIAgents[i].SetActive(false);
            }
            
            activeAIAgents.RemoveRange(maxActiveAI, activeAIAgents.Count - maxActiveAI);
        }
    }
    
    /// <summary>
    /// Get LOD level based on distance
    /// </summary>
    private AILODLevel GetLODLevel(float distance)
    {
        if (distance <= closeDistance)
            return AILODLevel.High;
        else if (distance <= mediumDistance)
            return AILODLevel.Medium;
        else if (distance <= farDistance)
            return AILODLevel.Low;
        else
            return AILODLevel.Disabled;
    }
    
    /// <summary>
    /// Register existing AI trong scene
    /// </summary>
    private void RegisterExistingAI()
    {
        // Tìm tất cả GameObject có AI components
        var aiMovementControllers = FindObjectsOfType<MonoBehaviour>();
        
        foreach (var component in aiMovementControllers)
        {
            if (component.GetType().Name == "AIMovementController")
            {
                AIAgent aiAgent = component.GetComponent<AIAgent>();
                if (aiAgent == null)
                {
                    aiAgent = component.gameObject.AddComponent<AIAgent>();
                }
                RegisterAI(aiAgent);
            }
        }
    }
    
    /// <summary>
    /// Track performance metrics
    /// </summary>
    private void TrackPerformance()
    {
        frameCounter++;
        
        if (frameCounter >= 60) // Update every 60 frames
        {
            avgFrameTime = Time.deltaTime;
            frameCounter = 0;
              if (debugMode)
            {
                // Debug.Log($"AI Performance - Active: {activeAIAgents.Count}/{allAIAgents.Count}, " +
                //          $"Frame Time: {avgFrameTime * 1000f:F1}ms");
            }
        }
    }
    
    /// <summary>
    /// Get current performance stats
    /// </summary>
    public AIPerformanceStats GetPerformanceStats()
    {
        return new AIPerformanceStats
        {
            totalAI = allAIAgents.Count,
            activeAI = activeAIAgents.Count,
            averageFrameTime = avgFrameTime,
            frameRate = 1f / avgFrameTime
        };
    }
    
    /// <summary>
    /// Emergency performance mode - disable tất cả AI non-essential
    /// </summary>
    public void EnableEmergencyMode()
    {
        maxActiveAI = Mathf.Max(5, maxActiveAI / 2);
        updateDistance = updateDistance * 0.5f;
        updateInterval = updateInterval * 2f;
        
        Debug.LogWarning("AI Emergency Performance Mode activated!");
    }
    
    /// <summary>
    /// Disable emergency mode
    /// </summary>
    public void DisableEmergencyMode()
    {
        maxActiveAI = 20;
        updateDistance = 50f;
        updateInterval = 0.1f;
        
        Debug.Log("AI Emergency Performance Mode deactivated");
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!debugMode || playerCamera == null) return;
        
        Vector3 playerPos = playerCamera.transform.position;
        
        // Update distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos, updateDistance);
        
        // Culling distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPos, cullingDistance);
        
        // LOD distances
        if (enableLOD)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerPos, closeDistance);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerPos, mediumDistance);
            
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(playerPos, farDistance);
        }
    }
}

/// <summary>
/// AIAgent wrapper cho performance management
/// </summary>
public class AIAgent : MonoBehaviour
{
    [Header("AI Components")]
    public TeamMember teamMember;
    public EnemyDetector enemyDetector;
    public CombatController combatController;
    public NavMeshAgent navAgent;
    
    private Component aiMovementController;
    private Component aiStateMachine;
    private AILODLevel currentLOD = AILODLevel.High;
    private bool isActive = true;
    
    private void Awake()
    {
        // Auto-find components
        if (teamMember == null)
            teamMember = GetComponent<TeamMember>();
        if (enemyDetector == null)
            enemyDetector = GetComponent<EnemyDetector>();
        if (combatController == null)
            combatController = GetComponent<CombatController>();
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
            
        // Find AI components dynamically
        var aiMovementType = System.Type.GetType("AIMovementController");
        if (aiMovementType != null)
            aiMovementController = GetComponent(aiMovementType);
            
        var stateMachineType = System.Type.GetType("AIStateMachine");
        if (stateMachineType != null)
            aiStateMachine = GetComponent(stateMachineType);
    }
    
    private void Start()
    {
        // Register với performance manager
        if (AIPerformanceManager.Instance != null)
        {
            AIPerformanceManager.Instance.RegisterAI(this);
        }
    }
    
    /// <summary>
    /// Set active state cho AI
    /// </summary>
    public void SetActive(bool active)
    {
        if (isActive == active) return;
        
        isActive = active;
        
        // Enable/disable components
        if (enemyDetector != null)
            enemyDetector.enabled = active;
        if (combatController != null)
            combatController.enabled = active;
        if (navAgent != null)
            navAgent.enabled = active;
        if (aiMovementController != null)
            SetComponentEnabled(aiMovementController, active);
        if (aiStateMachine != null)
            SetComponentEnabled(aiStateMachine, active);
    }
    
    /// <summary>
    /// Set LOD level
    /// </summary>
    public void SetLODLevel(AILODLevel lodLevel)
    {
        if (currentLOD == lodLevel) return;
        
        currentLOD = lodLevel;
        
        switch (lodLevel)
        {
            case AILODLevel.High:
                SetUpdateIntervals(0.1f);
                break;
            case AILODLevel.Medium:
                SetUpdateIntervals(0.2f);
                break;
            case AILODLevel.Low:
                SetUpdateIntervals(0.5f);
                break;
            case AILODLevel.Disabled:
                SetActive(false);
                break;
        }
    }
    
    /// <summary>
    /// Set update intervals cho components
    /// </summary>
    private void SetUpdateIntervals(float interval)
    {
        // Set intervals via reflection
        if (enemyDetector != null)
        {
            var field = enemyDetector.GetType().GetField("updateInterval", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(enemyDetector, interval);
        }
        
        if (aiMovementController != null)
        {
            var field = aiMovementController.GetType().GetField("updateInterval", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(aiMovementController, interval);
        }
    }
    
    /// <summary>
    /// Enable/disable component via reflection
    /// </summary>
    private void SetComponentEnabled(Component component, bool enabled)
    {
        if (component is MonoBehaviour monoBehaviour)
        {
            monoBehaviour.enabled = enabled;
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from performance manager
        if (AIPerformanceManager.Instance != null)
        {
            AIPerformanceManager.Instance.UnregisterAI(this);
        }
    }
}

/// <summary>
/// LOD levels cho AI
/// </summary>
public enum AILODLevel
{
    High,       // Full update rate
    Medium,     // Reduced update rate
    Low,        // Very low update rate
    Disabled    // No updates
}

/// <summary>
/// Performance statistics
/// </summary>
[System.Serializable]
public struct AIPerformanceStats
{
    public int totalAI;
    public int activeAI;
    public float averageFrameTime;
    public float frameRate;
}