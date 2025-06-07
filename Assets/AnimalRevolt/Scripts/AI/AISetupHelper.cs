using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Helper script để setup AI system cho characters
/// Tự động add và configure các components cần thiết
/// </summary>
public class AISetupHelper : MonoBehaviour
{
    [Header("AI Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float seekRadius = 15f;
    
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float engageDistance = 8f;
    [SerializeField] private float detectionRadius = 10f;
    
    [Header("Team Settings")]
    [SerializeField] private TeamType teamType = TeamType.AI_Team1;
    [SerializeField] private string teamName = "AI_Team1";
    [SerializeField] private Color teamColor = Color.blue;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupAISystem();
        }
    }
    
    /// <summary>
    /// Setup complete AI system cho character
    /// </summary>
    [ContextMenu("Setup AI System")]
    public void SetupAISystem()
    {
        if (debugMode)
            Debug.Log($"Setting up AI system for {gameObject.name}...");
        
        // 1. Setup TeamMember
        SetupTeamMember();
        
        // 2. Setup NavMeshAgent
        SetupNavMeshAgent();
        
        // 3. Setup EnemyDetector
        SetupEnemyDetector();
        
        // 4. Setup CombatController
        SetupCombatController();
        
        // 5. Setup AIMovementController
        SetupAIMovementController();
        
        // 6. Setup AIStateMachine
        SetupAIStateMachine();
        
        if (debugMode)
            Debug.Log($"AI system setup completed for {gameObject.name}");
    }
    
    /// <summary>
    /// Setup TeamMember component
    /// </summary>
    private void SetupTeamMember()
    {
        TeamMember teamMember = GetComponent<TeamMember>();
        if (teamMember == null)
        {
            teamMember = gameObject.AddComponent<TeamMember>();
            if (debugMode)
                Debug.Log($"Added TeamMember component to {gameObject.name}");
        }
        
        // Configure team settings via reflection để avoid compilation issues
        var teamMemberType = teamMember.GetType();
        var teamTypeField = teamMemberType.GetField("teamType", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var teamNameField = teamMemberType.GetField("teamName", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var teamColorField = teamMemberType.GetField("teamColor", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (teamTypeField != null) teamTypeField.SetValue(teamMember, teamType);
        if (teamNameField != null) teamNameField.SetValue(teamMember, teamName);
        if (teamColorField != null) teamColorField.SetValue(teamMember, teamColor);
    }
    
    /// <summary>
    /// Setup NavMeshAgent component
    /// </summary>
    private void SetupNavMeshAgent()
    {
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
            if (debugMode)
                Debug.Log($"Added NavMeshAgent component to {gameObject.name}");
        }
        
        // Configure NavMeshAgent
        navAgent.speed = walkSpeed;
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.angularSpeed = 120f;
        navAgent.acceleration = 8f;
        navAgent.autoBraking = true;
        navAgent.autoRepath = true;
        navAgent.radius = 0.5f;
        navAgent.height = 2f;
        navAgent.baseOffset = 0f;
    }
    
    /// <summary>
    /// Setup EnemyDetector component
    /// </summary>
    private void SetupEnemyDetector()
    {
        EnemyDetector enemyDetector = GetComponent<EnemyDetector>();
        if (enemyDetector == null)
        {
            enemyDetector = gameObject.AddComponent<EnemyDetector>();
            if (debugMode)
                Debug.Log($"Added EnemyDetector component to {gameObject.name}");
        }
        
        // Configure EnemyDetector via reflection
        var detectorType = enemyDetector.GetType();
        var detectionRadiusField = detectorType.GetField("detectionRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var detectionAngleField = detectorType.GetField("detectionAngle", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (detectionRadiusField != null) detectionRadiusField.SetValue(enemyDetector, detectionRadius);
        if (detectionAngleField != null) detectionAngleField.SetValue(enemyDetector, 90f);
    }
    
    /// <summary>
    /// Setup CombatController component
    /// </summary>
    private void SetupCombatController()
    {
        CombatController combatController = GetComponent<CombatController>();
        if (combatController == null)
        {
            combatController = gameObject.AddComponent<CombatController>();
            if (debugMode)
                Debug.Log($"Added CombatController component to {gameObject.name}");
        }
        
        // Configure CombatController via reflection
        var combatType = combatController.GetType();
        var attackRangeField = combatType.GetField("attackRange", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var engageDistanceField = combatType.GetField("engageDistance", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (attackRangeField != null) attackRangeField.SetValue(combatController, attackRange);
        if (engageDistanceField != null) engageDistanceField.SetValue(combatController, engageDistance);
    }
    
    /// <summary>
    /// Setup AIMovementController component
    /// </summary>
    private void SetupAIMovementController()
    {
        // Sử dụng reflection để tránh compilation issues
        var aiMovementType = System.Type.GetType("AIMovementController");
        if (aiMovementType == null)
        {
            if (debugMode)
                Debug.LogWarning($"AIMovementController type not found. Make sure the script is compiled.");
            return;
        }
        
        Component aiMovement = GetComponent(aiMovementType);
        if (aiMovement == null)
        {
            aiMovement = gameObject.AddComponent(aiMovementType);
            if (debugMode)
                Debug.Log($"Added AIMovementController component to {gameObject.name}");
        }
        
        // Configure AIMovementController via reflection
        var walkSpeedField = aiMovementType.GetField("walkSpeed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var runSpeedField = aiMovementType.GetField("runSpeed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var seekRadiusField = aiMovementType.GetField("seekRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (walkSpeedField != null) walkSpeedField.SetValue(aiMovement, walkSpeed);
        if (runSpeedField != null) runSpeedField.SetValue(aiMovement, runSpeed);
        if (seekRadiusField != null) seekRadiusField.SetValue(aiMovement, seekRadius);
    }
    
    /// <summary>
    /// Setup AIStateMachine component
    /// </summary>
    private void SetupAIStateMachine()
    {
        var stateMachineType = System.Type.GetType("AIStateMachine");
        if (stateMachineType == null)
        {
            if (debugMode)
                Debug.LogWarning($"AIStateMachine type not found. Make sure the script is compiled.");
            return;
        }
        
        Component stateMachine = GetComponent(stateMachineType);
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent(stateMachineType);
            if (debugMode)
                Debug.Log($"Added AIStateMachine component to {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Remove all AI components
    /// </summary>
    [ContextMenu("Remove AI System")]
    public void RemoveAISystem()
    {
        if (debugMode)
            Debug.Log($"Removing AI system from {gameObject.name}...");
        
        // Remove components in reverse order
        var aiMovementType = System.Type.GetType("AIMovementController");
        if (aiMovementType != null)
        {
            Component aiMovement = GetComponent(aiMovementType);
            if (aiMovement != null) DestroyImmediate(aiMovement);
        }
        
        var stateMachineType = System.Type.GetType("AIStateMachine");
        if (stateMachineType != null)
        {
            Component stateMachine = GetComponent(stateMachineType);
            if (stateMachine != null) DestroyImmediate(stateMachine);
        }
        
        CombatController combatController = GetComponent<CombatController>();
        if (combatController != null) DestroyImmediate(combatController);
        
        EnemyDetector enemyDetector = GetComponent<EnemyDetector>();
        if (enemyDetector != null) DestroyImmediate(enemyDetector);
        
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null) DestroyImmediate(navAgent);
        
        TeamMember teamMember = GetComponent<TeamMember>();
        if (teamMember != null) DestroyImmediate(teamMember);
        
        if (debugMode)
            Debug.Log($"AI system removed from {gameObject.name}");
    }
    
    /// <summary>
    /// Validate AI setup
    /// </summary>
    [ContextMenu("Validate AI Setup")]
    public void ValidateSetup()
    {
        bool isValid = true;
        
        if (GetComponent<TeamMember>() == null)
        {
            Debug.LogError($"{gameObject.name} missing TeamMember component!");
            isValid = false;
        }
        
        if (GetComponent<NavMeshAgent>() == null)
        {
            Debug.LogError($"{gameObject.name} missing NavMeshAgent component!");
            isValid = false;
        }
        
        if (GetComponent<EnemyDetector>() == null)
        {
            Debug.LogError($"{gameObject.name} missing EnemyDetector component!");
            isValid = false;
        }
        
        if (GetComponent<CombatController>() == null)
        {
            Debug.LogError($"{gameObject.name} missing CombatController component!");
            isValid = false;
        }
        
        var aiMovementType = System.Type.GetType("AIMovementController");
        if (aiMovementType == null || GetComponent(aiMovementType) == null)
        {
            Debug.LogError($"{gameObject.name} missing AIMovementController component!");
            isValid = false;
        }
        
        var stateMachineType = System.Type.GetType("AIStateMachine");
        if (stateMachineType == null || GetComponent(stateMachineType) == null)
        {
            Debug.LogError($"{gameObject.name} missing AIStateMachine component!");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log($"{gameObject.name} AI setup is valid!");
        }
    }
    
    /// <summary>
    /// Auto-detect team based on GameObject name
    /// </summary>
    [ContextMenu("Auto Detect Team")]
    public void AutoDetectTeam()
    {
        string objName = gameObject.name.ToLower();
        
        if (objName.Contains("player"))
        {
            teamType = TeamType.Player;
            teamName = "Player";
            teamColor = Color.blue;
        }
        else if (objName.Contains("enemy"))
        {
            teamType = TeamType.Enemy;
            teamName = "Enemy";
            teamColor = Color.red;
        }
        else if (objName.Contains("team1"))
        {
            teamType = TeamType.AI_Team1;
            teamName = "AI_Team1";
            teamColor = Color.green;
        }
        else if (objName.Contains("team2"))
        {
            teamType = TeamType.AI_Team2;
            teamName = "AI_Team2";
            teamColor = Color.yellow;
        }
        else if (objName.Contains("team3"))
        {
            teamType = TeamType.AI_Team3;
            teamName = "AI_Team3";
            teamColor = Color.magenta;
        }
        else
        {
            teamType = TeamType.Neutral;
            teamName = "Neutral";
            teamColor = Color.gray;
        }
        
        if (debugMode)
            Debug.Log($"{gameObject.name} team detected as: {teamName} ({teamType})");
    }
}