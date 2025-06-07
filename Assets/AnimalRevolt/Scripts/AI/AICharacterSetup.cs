using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Unified setup script để tự động cấu hình AI character hoàn chỉnh
/// Bao gồm: Movement, Combat, Team, Enemy Detection, Ragdoll integration
/// </summary>
public class AICharacterSetup : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private TeamType teamType = TeamType.AI_Team1;
    [SerializeField] private string characterName = "AI Character";
    [SerializeField] private float maxHealth = 100f;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private AIBehaviorType behaviorType = AIBehaviorType.Aggressive;
    
    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private CombatBehaviorType combatBehavior = CombatBehaviorType.Aggressive;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float engageDistance = 8f;
    [SerializeField] private LayerMask enemyLayers = -1;
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Component References")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider mainCollider;
    
    // Setup status
    private bool isSetupComplete = false;
    
    // Components được tạo
    private TeamMember teamMember;
    private EnemyDetector enemyDetector;
    private CombatController combatController;
    private AIMovementController aiMovement;
    private AIStateMachine stateMachine;
    private RagdollPhysicsController ragdollController;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupAICharacter();
        }
    }
    
    /// <summary>
    /// Setup hoàn chỉnh AI character
    /// </summary>
    [ContextMenu("Setup AI Character")]
    public void SetupAICharacter()
    {
        if (isSetupComplete)
        {
            if (debugMode)
                Debug.LogWarning($"AI Character {gameObject.name} already setup!");
            return;
        }
        
        if (debugMode)
            Debug.Log($"Setting up AI Character: {gameObject.name}");
        
        // Step 1: Setup required components
        SetupRequiredComponents();
        
        // Step 2: Setup TeamMember
        SetupTeamMember();
        
        // Step 3: Setup EnemyDetector
        SetupEnemyDetector();
        
        // Step 4: Setup CombatController
        SetupCombatController();
        
        // Step 5: Setup AIMovementController
        SetupAIMovementController();
        
        // Step 6: Setup AIStateMachine
        SetupAIStateMachine();
        
        // Step 7: Setup Ragdoll integration (optional)
        SetupRagdollIntegration();
        
        // Step 8: Final configuration
        FinalizeSetup();
        
        isSetupComplete = true;
        
        if (debugMode)
            Debug.Log($"AI Character setup complete: {gameObject.name}");
    }
    
    /// <summary>
    /// Setup required Unity components
    /// </summary>
    private void SetupRequiredComponents()
    {
        // NavMeshAgent
        if (navAgent == null)
        {
            navAgent = gameObject.GetComponent<NavMeshAgent>();
            if (navAgent == null)
                navAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        
        // Configure NavMeshAgent
        navAgent.speed = walkSpeed;
        navAgent.angularSpeed = rotationSpeed * 50f;
        navAgent.acceleration = 8f;
        navAgent.stoppingDistance = 1.5f;
        navAgent.autoBraking = true;
        navAgent.autoRepath = true;
        
        // Animator
        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }
        
        // Main Collider
        if (mainCollider == null)
        {
            mainCollider = gameObject.GetComponent<Collider>();
            if (mainCollider == null)
            {
                // Tạo CapsuleCollider mặc định
                CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 2f;
                capsule.radius = 0.5f;
                capsule.center = new Vector3(0, 1f, 0);
                mainCollider = capsule;
            }
        }
        
        if (debugMode)
            Debug.Log($"🏷️ {gameObject.name} setup without Unity tags - using TeamMember component for team identification");
    }
    
    /// <summary>
    /// Setup TeamMember component
    /// </summary>
    private void SetupTeamMember()
    {
        teamMember = gameObject.GetComponent<TeamMember>();
        if (teamMember == null)
            teamMember = gameObject.AddComponent<TeamMember>();
        
        teamMember.InitializeTeam(teamType, characterName, maxHealth);
        teamMember.SetDebugMode(debugMode);
        teamMember.RemoveTeamVisualIndicator();
        
        if (debugMode)
            Debug.Log($"✅ {gameObject.name} TeamMember setup complete - Team: {teamMember.TeamName} ({teamMember.TeamType})");
    }
    
    /// <summary>
    /// Setup EnemyDetector component
    /// </summary>
    private void SetupEnemyDetector()
    {
        enemyDetector = gameObject.GetComponent<EnemyDetector>();
        if (enemyDetector == null)
            enemyDetector = gameObject.AddComponent<EnemyDetector>();
    }
    
    /// <summary>
    /// Setup CombatController component
    /// </summary>
    private void SetupCombatController()
    {
        combatController = gameObject.GetComponent<CombatController>();
        if (combatController == null)
        {
            Debug.LogWarning($"CombatController not found on {gameObject.name}. Please add it manually.");
            return;
        }
    }
    
    /// <summary>
    /// Setup AIMovementController component
    /// </summary>
    private void SetupAIMovementController()
    {
        aiMovement = gameObject.GetComponent<AIMovementController>();
        if (aiMovement == null)
            aiMovement = gameObject.AddComponent<AIMovementController>();
    }
    
    /// <summary>
    /// Setup AIStateMachine component
    /// </summary>
    private void SetupAIStateMachine()
    {
        stateMachine = gameObject.GetComponent<AIStateMachine>();
        if (stateMachine == null)
            stateMachine = gameObject.AddComponent<AIStateMachine>();
    }
    
    /// <summary>
    /// Setup Ragdoll integration nếu có
    /// </summary>
    private void SetupRagdollIntegration()
    {
        ragdollController = gameObject.GetComponent<RagdollPhysicsController>();
        
        if (ragdollController != null && debugMode)
        {
            Debug.Log($"Ragdoll integration found for {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Finalize setup và kiểm tra
    /// </summary>
    private void FinalizeSetup()
    {
        // Force component refresh
        if (teamMember != null)
        {
            teamMember.enabled = false;
            teamMember.enabled = true;
        }
        
        if (enemyDetector != null)
        {
            enemyDetector.enabled = false;
            enemyDetector.enabled = true;
        }
        
        if (aiMovement != null)
        {
            aiMovement.enabled = false;
            aiMovement.enabled = true;
        }
        
        if (stateMachine != null)
        {
            stateMachine.enabled = false;
            stateMachine.enabled = true;
        }
        
        // Check NavMesh
        if (navAgent != null && !navAgent.isOnNavMesh)
        {
            if (debugMode)
                Debug.LogWarning($"NavMeshAgent for {gameObject.name} is not on NavMesh! Make sure scene has NavMesh baked.");
        }
    }
    
    /// <summary>
    /// Reset setup để có thể setup lại
    /// </summary>
    [ContextMenu("Reset Setup")]
    public void ResetSetup()
    {
        isSetupComplete = false;
        
        if (debugMode)
            Debug.Log($"Reset setup for {gameObject.name}");
    }
    
    /// <summary>
    /// Validate setup status
    /// </summary>
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        bool isValid = true;
        System.Text.StringBuilder report = new System.Text.StringBuilder();
        
        report.AppendLine($"=== Setup Validation for {gameObject.name} ===");
        
        // Check components
        if (teamMember == null)
        {
            report.AppendLine("❌ TeamMember missing");
            isValid = false;
        }
        else
        {
            report.AppendLine("✅ TeamMember OK");
        }
        
        if (navAgent != null && !navAgent.isOnNavMesh)
        {
            report.AppendLine("⚠️ Not on NavMesh - bake NavMesh for scene");
        }
        
        report.AppendLine($"=== Overall Status: {(isValid ? "✅ VALID" : "❌ INVALID")} ===");
        
        Debug.Log(report.ToString());
    }
    
    // Properties for access
    public bool IsSetupComplete => isSetupComplete;
    public TeamMember TeamMember => teamMember;
    public AIMovementController AIMovement => aiMovement;
    public CombatController CombatController => combatController;
    
    /// <summary>
    /// Set team type
    /// </summary>
    public void SetTeamType(TeamType newTeamType)
    {
        teamType = newTeamType;
        
        if (teamMember != null)
        {
            teamMember.SetTeamType(newTeamType);
        }
        
        if (debugMode)
            Debug.Log($"🔄 {gameObject.name} team changed to: {teamType}");
    }
}
