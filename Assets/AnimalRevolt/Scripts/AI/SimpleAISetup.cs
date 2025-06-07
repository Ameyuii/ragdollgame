using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple AI Setup script với Inspector buttons
/// Thay thế cho context menu setup
/// </summary>
public class SimpleAISetup : MonoBehaviour
{
    [Header("📋 SIMPLE AI SETUP GUIDE")]
    [SerializeField] private bool showHelp = true;
    
    [Header("🎯 Team Configuration")]
    [SerializeField] private TeamType teamType = TeamType.AI_Team1;
    [SerializeField] private string characterName = "AI Character";
    [SerializeField] private Color teamColor = Color.blue;
    
    [Header("⚙️ AI Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRadius = 10f;
    
    [Header("🔧 Setup Status")]
    [SerializeField] private bool isSetupComplete = false;
    [SerializeField] private bool debugMode = true;
    
    // References
    private TeamMember teamMember;
    private EnemyDetector enemyDetector;
    private CombatController combatController;
    private AIMovementController aiMovement;
    private AIStateMachine stateMachine;
    private NavMeshAgent navAgent;
    
    private void Start()
    {
        if (!isSetupComplete)
        {
            if (debugMode)
                Debug.LogWarning($"⚠️ {gameObject.name} chưa được setup! Click 'Setup AI Character' button trong Inspector.");
        }
    }
    
    /// <summary>
    /// Setup complete AI character - Inspector button
    /// </summary>
    [ContextMenu("🚀 Setup AI Character")]
    public void SetupAICharacter()
    {
        if (debugMode)
            Debug.Log($"🔧 Setting up AI Character: {gameObject.name}");
        
        // Step 1: Validate và setup Unity components
        SetupUnityComponents();
        
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
        
        // Step 7: Final validation
        ValidateSetup();
        
        isSetupComplete = true;
        
        if (debugMode)
            Debug.Log($"✅ AI Character setup complete: {gameObject.name}");
    }
    
    /// <summary>
    /// Reset setup để setup lại
    /// </summary>
    [ContextMenu("🔄 Reset Setup")]
    public void ResetSetup()
    {
        isSetupComplete = false;
        
        if (debugMode)
            Debug.Log($"🔄 Reset setup for {gameObject.name}");
    }
    
    /// <summary>
    /// Validate setup hiện tại
    /// </summary>
    [ContextMenu("🔍 Validate Setup")]
    public void ValidateSetup()
    {
        bool isValid = true;
        
        // Check required components
        if (GetComponent<NavMeshAgent>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing NavMeshAgent!");
            isValid = false;
        }
        
        if (GetComponent<TeamMember>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing TeamMember!");
            isValid = false;
        }
        
        if (GetComponent<EnemyDetector>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing EnemyDetector!");
            isValid = false;
        }
        
        if (GetComponent<CombatController>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing CombatController!");
            isValid = false;
        }
        
        if (GetComponent<AIMovementController>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing AIMovementController!");
            isValid = false;
        }
        
        if (GetComponent<AIStateMachine>() == null)
        {
            Debug.LogError($"❌ {gameObject.name} missing AIStateMachine!");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log($"✅ {gameObject.name} setup is valid!");
            isSetupComplete = true;
        }
        else
        {
            isSetupComplete = false;
        }
    }
    
    /// <summary>
    /// Setup Unity components cơ bản
    /// </summary>
    private void SetupUnityComponents()
    {
        // NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
            if (debugMode)
                Debug.Log($"➕ Added NavMeshAgent to {gameObject.name}");
        }
        
        // Configure NavMeshAgent
        navAgent.speed = walkSpeed;
        navAgent.angularSpeed = 120f;
        navAgent.acceleration = 8f;
        navAgent.stoppingDistance = 1.5f;
        navAgent.autoBraking = true;
        navAgent.autoRepath = true;
        
        // Collider
        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.5f;
            capsule.center = new Vector3(0, 1f, 0);
            
            if (debugMode)
                Debug.Log($"➕ Added CapsuleCollider to {gameObject.name}");
        }
        
        // Tag
        if (gameObject.tag == "Untagged")
        {
            gameObject.tag = teamType == TeamType.AI_Team1 ? "BlueTeam" : "RedTeam";
        }
    }
    
    /// <summary>
    /// Setup TeamMember component
    /// </summary>
    private void SetupTeamMember()
    {
        teamMember = GetComponent<TeamMember>();
        if (teamMember == null)
        {
            teamMember = gameObject.AddComponent<TeamMember>();
            if (debugMode)
                Debug.Log($"➕ Added TeamMember to {gameObject.name}");
        }
        
        // Set team properties using reflection
        var teamMemberType = teamMember.GetType();
        
        var teamTypeField = teamMemberType.GetField("teamType", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var teamNameField = teamMemberType.GetField("teamName", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var teamColorField = teamMemberType.GetField("teamColor", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxHealthField = teamMemberType.GetField("maxHealth", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var healthField = teamMemberType.GetField("health", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        teamTypeField?.SetValue(teamMember, teamType);
        teamNameField?.SetValue(teamMember, characterName);
        teamColorField?.SetValue(teamMember, teamColor);
        maxHealthField?.SetValue(teamMember, 100f);
        healthField?.SetValue(teamMember, 100f);
        
        // Remove any existing visual team indicators (visual indicators are disabled)
        teamMember.RemoveTeamVisualIndicator();
        
        if (debugMode)
            Debug.Log($"🚫 Visual team indicators disabled for {gameObject.name}");
    }
    
    /// <summary>
    /// Setup EnemyDetector component
    /// </summary>
    private void SetupEnemyDetector()
    {
        enemyDetector = GetComponent<EnemyDetector>();
        if (enemyDetector == null)
        {
            enemyDetector = gameObject.AddComponent<EnemyDetector>();
            if (debugMode)
                Debug.Log($"➕ Added EnemyDetector to {gameObject.name}");
        }
        
        // Configure EnemyDetector
        var detectorType = enemyDetector.GetType();
        var radiusField = detectorType.GetField("detectionRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var angleField = detectorType.GetField("detectionAngle", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var debugField = detectorType.GetField("debugMode", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        radiusField?.SetValue(enemyDetector, detectionRadius);
        angleField?.SetValue(enemyDetector, 90f);
        debugField?.SetValue(enemyDetector, debugMode);
    }
    
    /// <summary>
    /// Setup CombatController component
    /// </summary>
    private void SetupCombatController()
    {
        combatController = GetComponent<CombatController>();
        if (combatController == null)
        {
            Debug.LogWarning($"⚠️ CombatController not found on {gameObject.name}. Please add it manually.");
            return;
        }
        
        // Configure CombatController
        var combatType = combatController.GetType();
        var damageField = combatType.GetField("attackDamage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rangeField = combatType.GetField("attackRange", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var debugField = combatType.GetField("debugMode", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        damageField?.SetValue(combatController, attackDamage);
        rangeField?.SetValue(combatController, attackRange);
        debugField?.SetValue(combatController, debugMode);
    }
    
    /// <summary>
    /// Setup AIMovementController component
    /// </summary>
    private void SetupAIMovementController()
    {
        aiMovement = GetComponent<AIMovementController>();
        if (aiMovement == null)
        {
            aiMovement = gameObject.AddComponent<AIMovementController>();
            if (debugMode)
                Debug.Log($"➕ Added AIMovementController to {gameObject.name}");
        }
        
        // Configure AIMovementController
        var movementType = aiMovement.GetType();
        var walkField = movementType.GetField("walkSpeed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var runField = movementType.GetField("runSpeed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var debugField = movementType.GetField("debugMode", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        walkField?.SetValue(aiMovement, walkSpeed);
        runField?.SetValue(aiMovement, runSpeed);
        debugField?.SetValue(aiMovement, debugMode);
    }
    
    /// <summary>
    /// Setup AIStateMachine component
    /// </summary>
    private void SetupAIStateMachine()
    {
        stateMachine = GetComponent<AIStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<AIStateMachine>();
            if (debugMode)
                Debug.Log($"➕ Added AIStateMachine to {gameObject.name}");
        }
        
        // Configure AIStateMachine
        var stateMachineType = stateMachine.GetType();
        var debugField = stateMachineType.GetField("debugMode", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        debugField?.SetValue(stateMachine, debugMode);
    }
    
    /// <summary>
    /// Quick setup for multiple characters
    /// </summary>
    public static void SetupMultipleCharacters(GameObject[] characters, TeamType team)
    {
        foreach (GameObject character in characters)
        {
            if (character == null) continue;
            
            SimpleAISetup setup = character.GetComponent<SimpleAISetup>();
            if (setup == null)
            {
                setup = character.AddComponent<SimpleAISetup>();
            }
            
            setup.teamType = team;
            setup.SetupAICharacter();
        }
        
        Debug.Log($"✅ Setup {characters.Length} characters for team {team}");
    }
    
    private void OnValidate()
    {
        // Update team color khi thay đổi team type
        if (teamType == TeamType.AI_Team1)
        {
            teamColor = Color.blue;
        }
        else if (teamType == TeamType.AI_Team2)
        {
            teamColor = Color.red;
        }
    }
}