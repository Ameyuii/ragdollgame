using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Helper script để debug và fix vấn đề combat movement
/// Attach vào một GameObject trống trong scene để monitor
/// </summary>
public class CombatDebugHelper : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool autoFixMovementIssues = true;
    [SerializeField] private float checkInterval = 2f;
    
    [Header("Detection")]
    [SerializeField] private float stuckDetectionTime = 3f;
    [SerializeField] private float minMovementThreshold = 0.1f;
    
    private void Start()
    {
        InvokeRepeating(nameof(CheckCombatIssues), 1f, checkInterval);
        
        if (enableDebugLogs)
            Debug.Log($"🔍 [COMBAT DEBUG] CombatDebugHelper started - checking every {checkInterval}s");
    }
    
    /// <summary>
    /// Kiểm tra các vấn đề thường gặp trong combat
    /// </summary>
    private void CheckCombatIssues()
    {
        CombatController[] allCombatControllers = FindObjectsByType<CombatController>(FindObjectsSortMode.None);
        
        if (enableDebugLogs)
            Debug.Log($"🔍 [COMBAT DEBUG] Checking {allCombatControllers.Length} combat controllers");
        
        int stuckCount = 0;
        int rotationLoopCount = 0;
        
        foreach (CombatController combat in allCombatControllers)
        {
            if (combat == null) continue;
            
            // Check stuck movement
            if (IsMovementStuck(combat))
            {
                stuckCount++;
                if (autoFixMovementIssues)
                {
                    FixStuckMovement(combat);
                }
            }
            
            // Check rotation loops
            if (IsInRotationLoop(combat))
            {
                rotationLoopCount++;
                if (autoFixMovementIssues)
                {
                    FixRotationLoop(combat);
                }
            }
        }
        
        if (enableDebugLogs && (stuckCount > 0 || rotationLoopCount > 0))
        {
            Debug.Log($"🚨 [COMBAT DEBUG] Issues found - Stuck: {stuckCount}, Rotation loops: {rotationLoopCount}");
        }
    }
    
    /// <summary>
    /// Kiểm tra xem NPC có bị stuck movement không
    /// </summary>
    private bool IsMovementStuck(CombatController combat)
    {
        if (!combat.IsInCombat) return false;
        
        NavMeshAgent navAgent = combat.GetComponent<NavMeshAgent>();
        if (navAgent == null) return false;
        
        // Check nếu NavAgent có path nhưng không di chuyển
        if (navAgent.hasPath && navAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (navAgent.remainingDistance > navAgent.stoppingDistance && navAgent.velocity.magnitude < minMovementThreshold)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Kiểm tra xem NPC có bị rotation loop không
    /// </summary>
    private bool IsInRotationLoop(CombatController combat)
    {
        if (!combat.IsInCombat || combat.CurrentTarget == null) return false;
        
        // Check nếu 2 NPC đang target vào nhau và quá gần
        CombatController targetCombat = combat.CurrentTarget.GetComponent<CombatController>();
        if (targetCombat == null) return false;
        
        if (targetCombat.CurrentTarget != null && targetCombat.CurrentTarget.GetComponent<CombatController>() == combat)
        {
            float distance = Vector3.Distance(combat.transform.position, combat.CurrentTarget.transform.position);
            if (distance < 3f) // Quá gần và target lẫn nhau
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Fix stuck movement
    /// </summary>
    private void FixStuckMovement(CombatController combat)
    {
        NavMeshAgent navAgent = combat.GetComponent<NavMeshAgent>();
        if (navAgent == null) return;
        
        Debug.Log($"🔧 [COMBAT FIX] Fixing stuck movement for {combat.name}");
        
        // Reset NavMeshAgent
        navAgent.ResetPath();
        navAgent.isStopped = true;
        
        // Wait a frame then restart
        StartCoroutine(RestartMovementAfterDelay(combat, 0.1f));
    }
    
    /// <summary>
    /// Fix rotation loop
    /// </summary>
    private void FixRotationLoop(CombatController combat)
    {
        Debug.Log($"🔧 [COMBAT FIX] Fixing rotation loop for {combat.name}");
        
        // Tạm thời di chuyển NPC đi một chút để thoát khỏi loop
        Vector3 randomOffset = new Vector3(
            Random.Range(-2f, 2f),
            0,
            Random.Range(-2f, 2f)
        );
        
        Vector3 newPosition = combat.transform.position + randomOffset;
        
        // Ensure position is on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPosition, out hit, 5f, NavMesh.AllAreas))
        {
            NavMeshAgent navAgent = combat.GetComponent<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.Warp(hit.position);
                Debug.Log($"✅ [COMBAT FIX] Moved {combat.name} to {hit.position} to break rotation loop");
            }
        }
    }
    
    /// <summary>
    /// Restart movement sau delay
    /// </summary>
    private System.Collections.IEnumerator RestartMovementAfterDelay(CombatController combat, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        NavMeshAgent navAgent = combat.GetComponent<NavMeshAgent>();
        if (navAgent != null && combat.CurrentTarget != null)
        {
            navAgent.isStopped = false;
            // Let combat controller handle the destination setting
            Debug.Log($"✅ [COMBAT FIX] Restarted movement for {combat.name}");
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!enableDebugLogs) return;
        
        // Draw debug info in scene view
        CombatController[] allCombatControllers = FindObjectsByType<CombatController>(FindObjectsSortMode.None);
        
        foreach (CombatController combat in allCombatControllers)
        {
            if (combat == null || !combat.IsInCombat) continue;
            
            // Draw line to target
            if (combat.CurrentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(combat.transform.position + Vector3.up, 
                               combat.CurrentTarget.transform.position + Vector3.up);
                
                // Draw distance text would require Handles, which needs UnityEditor
            }
            
            // Draw movement status
            NavMeshAgent navAgent = combat.GetComponent<NavMeshAgent>();
            if (navAgent != null && navAgent.hasPath)
            {
                Gizmos.color = navAgent.velocity.magnitude > minMovementThreshold ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(combat.transform.position + Vector3.up * 2, 0.5f);
            }
        }
    }
}
