using UnityEngine;
using UnityEngine.AI;

public class SimpleCharacterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float patrolRadius = 10f;
    public float attackCooldown = 1.5f;
    
    private NavMeshAgent agent;
    private RagdollCharacter character;
    private Transform target;
    private bool isMoving = false;
    private bool isAttacking = false;
    private float searchTimer = 0f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<RagdollCharacter>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        // IMPORTANT: Disable AI by default until battle starts
        if (!IsBattleActive())
        {
            enabled = false;
            Debug.Log($"⏸️ {gameObject.name} AI disabled - Waiting for battle start");
        }
    }
    
    void Update()
    {
        if (character == null || character.IsDead())
            return;

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        // IMPORTANT: Only act if battle has started
        if (!IsBattleActive())
        {
            // Stop all AI activity if battle hasn't started
            SafeNavMeshHelper.SafeSetStopped(agent, true);
            isAttacking = false;
            isMoving = false;
            return;
        }
        
        // Search for enemies periodically
        searchTimer += Time.deltaTime;
        if (searchTimer >= 1f)
        {
            SearchForEnemies();
            searchTimer = 0f;
        }
        
        if (target != null && !target.GetComponent<RagdollCharacter>().IsDead())
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget <= attackRange)
            {
                // Close enough to attack
                SafeNavMeshHelper.SafeSetStopped(agent, true);
                AttackTarget();
            }
            else if (distanceToTarget <= detectionRange)
            {
                // Move towards target
                SafeNavMeshHelper.SafeSetStopped(agent, false);
                SafeNavMeshHelper.SafeSetDestination(agent, target.position);
                isMoving = SafeNavMeshHelper.IsAgentValid(agent) && agent.velocity.magnitude > 0.1f;
                isAttacking = false;
            }
            else
            {
                // Target too far, lose it
                target = null;
                SafeNavMeshHelper.SafeSetStopped(agent, false);
            }
        }
        else
        {
            // No target, patrol randomly
            SafeNavMeshHelper.SafeSetStopped(agent, false);
            if (SafeNavMeshHelper.IsAgentValid(agent) && (!agent.pathPending && (!agent.hasPath || agent.remainingDistance < 0.5f)))
            {
                PatrolRandomly();
            }
        }
    }
    
    void SearchForEnemies()
    {
        if (character == null || character.IsDead())
            return;
        
        target = null;
        float nearestDistance = detectionRange;
        
        RagdollCharacter[] allCharacters = FindObjectsOfType<RagdollCharacter>();
        
        foreach (RagdollCharacter otherChar in allCharacters)
        {
            if (otherChar == character || otherChar.IsDead())
                continue;
            
            if (otherChar.teamId != character.teamId)
            {
                float distance = Vector3.Distance(transform.position, otherChar.transform.position);
                if (distance < nearestDistance)
                {
                    target = otherChar.transform;
                    nearestDistance = distance;
                    Debug.Log($"{character.name} found enemy: {otherChar.name} at distance {distance:F1}");
                }
            }
        }
    }
    
    void AttackTarget()
    {
        if (target == null) return;

        // Double-check battle state before attacking
        if (!IsBattleActive())
        {
            Debug.Log($"⏸️ {character.name} prevented from attacking - Battle not started");
            return;
        }

        isAttacking = true;
        isMoving = false;
        
        // Face the target
        Vector3 lookDirection = (target.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        // Deal damage
        RagdollCharacter targetChar = target.GetComponent<RagdollCharacter>();
        if (targetChar != null && character != null)
        {
            targetChar.TakeDamage(character.attackDamage);
            Debug.Log($"{character.name} attacked {targetChar.name} for {character.attackDamage} damage");
        }
    }
    
    void PatrolRandomly()
    {
        if (!SafeNavMeshHelper.IsAgentValid(agent))
            return;
        
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            SafeNavMeshHelper.SafeSetDestination(agent, hit.position);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw line to target
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    /// <summary>
    /// Check if battle is currently active
    /// </summary>
    private bool IsBattleActive()
    {
        // Check BattleGameManager first
        BattleGameManager battleManager = FindObjectOfType<BattleGameManager>();
        if (battleManager != null)
        {
            return battleManager.gameStarted;
        }

        // Check UnifiedGameManager as fallback
        UnifiedGameManager unifiedManager = UnifiedGameManager.Instance;
        if (unifiedManager != null && unifiedManager.enableLegacyBridge)
        {
            var legacyManager = unifiedManager.GetComponent<BattleGameManager>();
            if (legacyManager != null)
            {
                return legacyManager.gameStarted;
            }
        }

        // If no battle manager found, assume battle is not active (safe default)
        return false;
    }
}