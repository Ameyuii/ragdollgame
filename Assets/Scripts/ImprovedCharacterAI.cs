using UnityEngine;
using UnityEngine.AI;

public class ImprovedCharacterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    
    private RagdollCharacter character;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private bool isMoving = false;
    
    void Start()
    {
        character = GetComponent<RagdollCharacter>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
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
        {
            SetAnimationState(false, false, false);
            return;
        }
        
        // Handle combat and movement
        if (target != null && !IsTargetDead())
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget <= attackRange)
            {
                // Stop moving and attack
                agent.isStopped = true;
                isMoving = false;
                
                // Face target
                Vector3 direction = (target.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
                
                // Attack
                AttackTarget();
            }
            else if (distanceToTarget <= detectionRange)
            {
                // Move towards target
                agent.isStopped = false;
                agent.SetDestination(target.position);
                isMoving = agent.velocity.magnitude > 0.1f;
                isAttacking = false;
            }
            else
            {
                // Target too far, lose it
                target = null;
                agent.isStopped = false;
                isAttacking = false;
            }
        }
        else
        {
            // No target, patrol randomly
            isAttacking = false;
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                PatrolRandomly();
            }
            isMoving = agent.velocity.magnitude > 0.1f;
        }
        
        // Update animations
        SetAnimationState(isMoving, isAttacking, character.IsDead());
    }
    
    void SetAnimationState(bool moving, bool attacking, bool dead)
    {
        if (animator == null) return;
        
        // Set animation parameters - using correct parameter names from Animation Controller
        animator.SetBool("IsWalking", moving);
        // Note: IsRunning parameter doesn't exist in controller, using IsWalking instead
        // animator.SetBool("IsRunning", moving && agent.velocity.magnitude > 2f);
        animator.SetBool("IsAlive", !dead); // Using IsAlive instead of IsDead
        
        if (attacking)
        {
            animator.SetTrigger("Attack");
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
            // Set attacking state
            isAttacking = true;
            
            // Deal damage
            targetChar.TakeDamage(character.attackDamage);
            lastAttackTime = Time.time;
            
            Debug.Log($"{name} attacks {target.name} for {character.attackDamage} damage!");
            
            // Trigger attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Add attack effect
            CreateAttackEffect();
        }
    }
    
    void CreateAttackEffect()
    {
        // Create simple attack effect
        if (target != null)
        {
            Vector3 effectPosition = target.position + Vector3.up * 1.5f;
            
            // Create a simple particle effect or visual feedback
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = effectPosition;
            effect.transform.localScale = Vector3.one * 0.2f;
            
            // Make it red and temporary
            Renderer renderer = effect.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
            
            // Remove collider
            Collider collider = effect.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
            
            // Destroy after short time
            Destroy(effect, 0.3f);
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