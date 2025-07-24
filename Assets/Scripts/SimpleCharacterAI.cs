using UnityEngine;
using UnityEngine.AI;

public class SimpleCharacterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    
    private RagdollCharacter character;
    private NavMeshAgent agent;
    private Transform target;
    private float lastAttackTime = 0f;
    private float searchTimer = 0f;
    
    void Start()
    {
        character = GetComponent<RagdollCharacter>();
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError($"NavMeshAgent not found on {name}!");
            enabled = false;
            return;
        }
        
        // Configure agent
        agent.stoppingDistance = attackRange * 0.8f;
        agent.speed = 3f;
        
        // Start searching for enemies
        InvokeRepeating(nameof(SearchForEnemies), 0f, 0.5f);
    }
    
    void Update()
    {
        if (character == null || character.IsDead() || agent == null)
            return;
        
        // Handle combat and movement
        if (target != null && !IsTargetDead())
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget <= attackRange)
            {
                // Stop moving and attack
                agent.isStopped = true;
                AttackTarget();
                
                // Face target
                Vector3 direction = (target.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            else if (distanceToTarget <= detectionRange)
            {
                // Move towards target
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                // Target too far, lose it
                target = null;
                agent.isStopped = false;
            }
        }
        else
        {
            // No target, patrol randomly
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
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
        
        // Find all characters
        RagdollCharacter[] allCharacters = FindObjectsOfType<RagdollCharacter>();
        
        foreach (RagdollCharacter otherChar in allCharacters)
        {
            if (otherChar == character || otherChar.IsDead())
                continue;
            
            // Check if different team
            if (otherChar.teamId != character.teamId)
            {
                float distance = Vector3.Distance(transform.position, otherChar.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    target = otherChar.transform;
                }
            }
        }
        
        if (target != null)
        {
            Debug.Log($"{name} found enemy: {target.name} at distance {nearestDistance:F1}");
        }
    }
    
    void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;
        
        if (target == null)
            return;
        
        RagdollCharacter targetChar = target.GetComponent<RagdollCharacter>();
        if (targetChar != null && !targetChar.IsDead())
        {
            // Deal damage
            targetChar.TakeDamage(character.attackDamage);
            lastAttackTime = Time.time;
            
            Debug.Log($"{name} attacks {target.name} for {character.attackDamage} damage!");
            
            // Trigger attack animation if available
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
    
    void PatrolRandomly()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;
        
        // Generate random point within patrol range
        Vector3 randomDirection = Random.insideUnitSphere * 5f;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    
    bool IsTargetDead()
    {
        if (target == null)
            return true;
        
        RagdollCharacter targetChar = target.GetComponent<RagdollCharacter>();
        return targetChar == null || targetChar.IsDead();
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
}