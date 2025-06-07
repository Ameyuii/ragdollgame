using UnityEngine;

/// <summary>
/// Helper script để test NPC detection và movement nhanh
/// </summary>
public class NPCTestHelper : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableLogs = true;
    public float logInterval = 2f;
    
    private CombatController combatController;
    private EnemyDetector enemyDetector;
    private float lastLogTime;
    
    private void Awake()
    {
        combatController = GetComponent<CombatController>();
        enemyDetector = GetComponent<EnemyDetector>();
    }
    
    private void Update()
    {
        if (!enableLogs) return;
        
        if (Time.time - lastLogTime >= logInterval)
        {
            LogNPCStatus();
            lastLogTime = Time.time;
        }
    }
    
    private void LogNPCStatus()
    {
        string npcName = gameObject.name;
        
        // Log detection status
        if (enemyDetector != null)
        {
            bool hasEnemies = enemyDetector.HasEnemies;
            int enemyCount = enemyDetector.DetectedEnemies.Count;
            var currentTarget = enemyDetector.CurrentTarget;
            
            Debug.Log($"📊 [NPC TEST] {npcName} - HasEnemies: {hasEnemies}, Count: {enemyCount}, Target: {(currentTarget != null ? currentTarget.name : "null")}");
            
            if (enemyDetector.DetectedEnemies.Count > 0)
            {
                foreach (var enemy in enemyDetector.DetectedEnemies)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    Debug.Log($"  🎯 [NPC TEST] {npcName} detects {enemy.name} at distance: {distance:F2}m");
                }
            }
        }
        
        // Log combat status
        if (combatController != null)
        {
            var combatState = combatController.CurrentState;
            var combatTarget = combatController.CurrentTarget;
            
            Debug.Log($"⚔️ [NPC TEST] {npcName} - CombatState: {combatState}, CombatTarget: {(combatTarget != null ? combatTarget.name : "null")}");
        }
        
        // Log NavMeshAgent status
        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            Debug.Log($"🤖 [NPC TEST] {npcName} - NavAgent: enabled={navAgent.enabled}, isStopped={navAgent.isStopped}, hasPath={navAgent.hasPath}, velocity={navAgent.velocity}");
        }
    }
}
