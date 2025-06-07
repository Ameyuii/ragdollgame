using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Simple validation script để verify AI Combat System behavior
/// Theo ROO rules: Không phải automated test script, mà là manual validation helper
/// </summary>
public class AICombatSystemValidator : MonoBehaviour
{
    [Header("🎯 AI Combat System Validator")]
    [SerializeField] private bool enableValidation = true;
    [SerializeField] private bool enableDetailedLogs = true;
    [SerializeField] private float validationInterval = 2f;
    
    [Header("📊 Auto-Detection Settings")]
    [SerializeField] private bool autoFindAI = true;
    [SerializeField] private List<GameObject> manualAIList = new List<GameObject>();
    
    [Header("🎮 Manual Testing Buttons")]
    [SerializeField] private bool showInspectorButtons = true;
    
    // Private variables
    private List<AIMovementController> allAIControllers = new List<AIMovementController>();
    private List<EnemyDetector> allEnemyDetectors = new List<EnemyDetector>();
    private List<TeamMember> allTeamMembers = new List<TeamMember>();
    private float lastValidationTime;
    private int validationCount = 0;
    
    // Validation results
    private int successfulDetections = 0;
    private int movementInstances = 0;
    private int combatEngagements = 0;
    private int stateTransitions = 0;
    
    private void Start()
    {
        if (!enableValidation) return;
        
        Debug.Log("🎯 [AI VALIDATOR] BẮT ĐẦU AI Combat System Validation");
        
        if (autoFindAI)
        {
            AutoDiscoverAIComponents();
        }
        else
        {
            ManualSetupValidation();
        }
        
        LogValidationSetup();
        
        // Delay initial validation để AI components khởi tạo xong
        Invoke(nameof(RunInitialValidation), 1f);
    }
    
    private void Update()
    {
        if (!enableValidation) return;
        
        // Periodic validation
        if (Time.time - lastValidationTime >= validationInterval)
        {
            RunPeriodicValidation();
            lastValidationTime = Time.time;
        }
    }
    
    /// <summary>
    /// Auto-discover tất cả AI components trong scene
    /// </summary>
    private void AutoDiscoverAIComponents()
    {
        // Find all AI Movement Controllers
        AIMovementController[] foundControllers = FindObjectsOfType<AIMovementController>();
        allAIControllers.AddRange(foundControllers);
        
        // Find all Enemy Detectors
        EnemyDetector[] foundDetectors = FindObjectsOfType<EnemyDetector>();
        allEnemyDetectors.AddRange(foundDetectors);
        
        // Find all Team Members
        TeamMember[] foundTeamMembers = FindObjectsOfType<TeamMember>();
        allTeamMembers.AddRange(foundTeamMembers);
        
        Debug.Log($"🔍 [AI VALIDATOR] AUTO-DISCOVERED: {allAIControllers.Count} AI Controllers, " +
                 $"{allEnemyDetectors.Count} Enemy Detectors, {allTeamMembers.Count} Team Members");
    }
    
    /// <summary>
    /// Manual setup validation từ danh sách manual
    /// </summary>
    private void ManualSetupValidation()
    {
        foreach (GameObject aiObj in manualAIList)
        {
            if (aiObj == null) continue;
            
            var controller = aiObj.GetComponent<AIMovementController>();
            var detector = aiObj.GetComponent<EnemyDetector>();
            var teamMember = aiObj.GetComponent<TeamMember>();
            
            if (controller != null) allAIControllers.Add(controller);
            if (detector != null) allEnemyDetectors.Add(detector);
            if (teamMember != null) allTeamMembers.Add(teamMember);
        }
        
        Debug.Log($"📝 [AI VALIDATOR] MANUAL SETUP: {allAIControllers.Count} AI Controllers, " +
                 $"{allEnemyDetectors.Count} Enemy Detectors, {allTeamMembers.Count} Team Members");
    }
    
    /// <summary>
    /// Log validation setup information
    /// </summary>
    private void LogValidationSetup()
    {
        if (!enableDetailedLogs) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("🎯 [AI VALIDATOR] === VALIDATION SETUP REPORT ===");
        
        // AI Controllers info
        sb.AppendLine($"🤖 AI MOVEMENT CONTROLLERS ({allAIControllers.Count}):");
        foreach (var controller in allAIControllers)
        {
            if (controller != null)
            {
                sb.AppendLine($"  ✅ {controller.name} - State: {controller.CurrentState}");
            }
        }
        
        // Team Members info
        sb.AppendLine($"\n👥 TEAM MEMBERS ({allTeamMembers.Count}):");
        foreach (var member in allTeamMembers)
        {
            if (member != null)
            {
                sb.AppendLine($"  🎯 {member.name} - Team: {member.TeamType} ({member.TeamName})");
            }
        }
        
        Debug.Log(sb.ToString());
    }
    
    /// <summary>
    /// Initial validation sau khi AI setup xong
    /// </summary>
    private void RunInitialValidation()
    {
        Debug.Log("🚀 [AI VALIDATOR] === CHẠY INITIAL VALIDATION ===");
        
        ValidateComponentSetup();
        ValidateTeamConfiguration();
        ValidateNavMeshSetup();
        
        Debug.Log("✅ [AI VALIDATOR] Initial validation hoàn thành");
    }
    
    /// <summary>
    /// Periodic validation để monitor runtime behavior
    /// </summary>
    private void RunPeriodicValidation()
    {
        validationCount++;
        
        if (enableDetailedLogs)
            Debug.Log($"🔄 [AI VALIDATOR] Periodic validation #{validationCount}");
        
        ValidateDetectionBehavior();
        ValidateMovementBehavior();
        ValidateCombatBehavior();
        ValidateStateTransitions();
        
        // Log summary mỗi 10 lần validation
        if (validationCount % 10 == 0)
        {
            LogValidationSummary();
        }
    }
    
    /// <summary>
    /// Validate component setup
    /// </summary>
    private void ValidateComponentSetup()
    {
        bool allValid = true;
        
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            
            // Check required components
            var detector = controller.GetComponent<EnemyDetector>();
            var teamMember = controller.GetComponent<TeamMember>();
            var navAgent = controller.GetComponent<UnityEngine.AI.NavMeshAgent>();
            
            if (detector == null)
            {
                Debug.LogError($"❌ [AI VALIDATOR] {controller.name} thiếu EnemyDetector component!");
                allValid = false;
            }
            
            if (teamMember == null)
            {
                Debug.LogError($"❌ [AI VALIDATOR] {controller.name} thiếu TeamMember component!");
                allValid = false;
            }
            
            if (navAgent == null)
            {
                Debug.LogError($"❌ [AI VALIDATOR] {controller.name} thiếu NavMeshAgent component!");
                allValid = false;
            }
        }
        
        if (allValid)
        {
            Debug.Log("✅ [AI VALIDATOR] Component setup validation PASSED");
        }
    }
    
    /// <summary>
    /// Validate team configuration
    /// </summary>
    private void ValidateTeamConfiguration()
    {
        Dictionary<TeamType, int> teamCounts = new Dictionary<TeamType, int>();
        
        foreach (var member in allTeamMembers)
        {
            if (member == null) continue;
            
            if (!teamCounts.ContainsKey(member.TeamType))
            {
                teamCounts[member.TeamType] = 0;
            }
            teamCounts[member.TeamType]++;
        }
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("🎯 [AI VALIDATOR] === TEAM CONFIGURATION ===");
        
        foreach (var kvp in teamCounts)
        {
            sb.AppendLine($"  {kvp.Key}: {kvp.Value} members");
        }
        
        // Check có ít nhất 2 teams khác nhau
        if (teamCounts.Count < 2)
        {
            Debug.LogWarning("⚠️ [AI VALIDATOR] Chỉ có 1 team - không thể test combat!");
        }
        else
        {
            Debug.Log("✅ [AI VALIDATOR] Team configuration hợp lệ cho combat testing");
        }
        
        Debug.Log(sb.ToString());
    }
    
    /// <summary>
    /// Validate NavMesh setup
    /// </summary>
    private void ValidateNavMeshSetup()
    {
        int onNavMeshCount = 0;
        int totalAI = 0;
        
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            totalAI++;
            
            var navAgent = controller.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null && navAgent.isOnNavMesh)
            {
                onNavMeshCount++;
            }
            else
            {
                Debug.LogWarning($"⚠️ [AI VALIDATOR] {controller.name} KHÔNG on NavMesh!");
            }
        }
        
        if (onNavMeshCount == totalAI)
        {
            Debug.Log($"✅ [AI VALIDATOR] NavMesh setup validation PASSED ({onNavMeshCount}/{totalAI} AI on NavMesh)");
        }
        else
        {
            Debug.LogError($"❌ [AI VALIDATOR] NavMesh setup FAILED ({onNavMeshCount}/{totalAI} AI on NavMesh)");
        }
    }
    
    /// <summary>
    /// Validate detection behavior
    /// </summary>
    private void ValidateDetectionBehavior()
    {
        int detectingAI = 0;
        
        foreach (var detector in allEnemyDetectors)
        {
            if (detector == null) continue;
            
            if (detector.HasEnemies)
            {
                detectingAI++;
                successfulDetections++;
                
                if (enableDetailedLogs)
                {
                    Debug.Log($"🎯 [AI VALIDATOR] {detector.name} đang detect {detector.DetectedEnemies.Count} enemies");
                }
            }
        }
        
        if (detectingAI > 0 && enableDetailedLogs)
        {
            Debug.Log($"🔍 [AI VALIDATOR] {detectingAI}/{allEnemyDetectors.Count} AI đang detect enemies");
        }
    }
    
    /// <summary>
    /// Validate movement behavior
    /// </summary>
    private void ValidateMovementBehavior()
    {
        int movingAI = 0;
        
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            
            if (controller.IsMoving)
            {
                movingAI++;
                movementInstances++;
                
                if (enableDetailedLogs)
                {
                    Debug.Log($"🏃 [AI VALIDATOR] {controller.name} đang movement - State: {controller.CurrentState}");
                }
            }
        }
        
        if (movingAI > 0 && enableDetailedLogs)
        {
            Debug.Log($"🚶 [AI VALIDATOR] {movingAI}/{allAIControllers.Count} AI đang di chuyển");
        }
    }
    
    /// <summary>
    /// Validate combat behavior
    /// </summary>
    private void ValidateCombatBehavior()
    {
        int inCombatAI = 0;
        
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            
            if (controller.CurrentState == AIState.Combat)
            {
                inCombatAI++;
                combatEngagements++;
                
                if (enableDetailedLogs)
                {
                    Debug.Log($"⚔️ [AI VALIDATOR] {controller.name} đang combat với {controller.CurrentTarget?.name}");
                }
            }
        }
        
        if (inCombatAI > 0 && enableDetailedLogs)
        {
            Debug.Log($"🥊 [AI VALIDATOR] {inCombatAI}/{allAIControllers.Count} AI đang combat");
        }
    }
    
    /// <summary>
    /// Validate state transitions
    /// </summary>
    private void ValidateStateTransitions()
    {
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            
            // Log state changes để track transitions
            var currentState = controller.CurrentState;
            
            // Simple transition validation
            if (currentState != AIState.Idle)
            {
                stateTransitions++;
            }
        }
    }
    
    /// <summary>
    /// Log validation summary
    /// </summary>
    private void LogValidationSummary()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("📊 [AI VALIDATOR] === VALIDATION SUMMARY ===");
        sb.AppendLine($"🔍 Successful Detections: {successfulDetections}");
        sb.AppendLine($"🏃 Movement Instances: {movementInstances}");
        sb.AppendLine($"⚔️ Combat Engagements: {combatEngagements}");
        sb.AppendLine($"🔄 State Transitions: {stateTransitions}");
        sb.AppendLine($"📈 Validation Cycles: {validationCount}");
        
        Debug.Log(sb.ToString());
    }
    
    // ============ INSPECTOR BUTTONS ============
    
    /// <summary>
    /// Manual validation trigger button
    /// </summary>
    [ContextMenu("🎯 Run Manual Validation")]
    public void RunManualValidation()
    {
        Debug.Log("🎮 [AI VALIDATOR] === MANUAL VALIDATION TRIGGERED ===");
        
        RunInitialValidation();
        RunPeriodicValidation();
        LogValidationSummary();
    }
    
    /// <summary>
    /// Force refresh AI components
    /// </summary>
    [ContextMenu("🔄 Refresh AI Components")]
    public void RefreshAIComponents()
    {
        allAIControllers.Clear();
        allEnemyDetectors.Clear();
        allTeamMembers.Clear();
        
        AutoDiscoverAIComponents();
        LogValidationSetup();
        
        Debug.Log("🔄 [AI VALIDATOR] AI Components refreshed!");
    }
    
    /// <summary>
    /// Reset validation statistics
    /// </summary>
    [ContextMenu("📊 Reset Statistics")]
    public void ResetStatistics()
    {
        successfulDetections = 0;
        movementInstances = 0;
        combatEngagements = 0;
        stateTransitions = 0;
        validationCount = 0;
        
        Debug.Log("📊 [AI VALIDATOR] Statistics reset!");
    }
    
    /// <summary>
    /// Log current AI states
    /// </summary>
    [ContextMenu("📋 Log Current AI States")]
    public void LogCurrentAIStates()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("📋 [AI VALIDATOR] === CURRENT AI STATES ===");
        
        foreach (var controller in allAIControllers)
        {
            if (controller == null) continue;
            
            var teamMember = controller.GetComponent<TeamMember>();
            var detector = controller.GetComponent<EnemyDetector>();
            
            sb.AppendLine($"🤖 {controller.name}:");
            sb.AppendLine($"  State: {controller.CurrentState}");
            sb.AppendLine($"  Team: {teamMember?.TeamType} ({teamMember?.TeamName})");
            sb.AppendLine($"  Target: {controller.CurrentTarget?.name ?? "None"}");
            sb.AppendLine($"  Enemies: {detector?.DetectedEnemies.Count ?? 0}");
            sb.AppendLine($"  Moving: {controller.IsMoving}");
        }
        
        Debug.Log(sb.ToString());
    }
}