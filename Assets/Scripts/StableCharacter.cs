using UnityEngine;
using UnityEngine.UI;

public class StableCharacter : MonoBehaviour
{
    [Header("Character Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    
    [Header("Team")]
    public int teamId = 1;
    
    [Header("Health Bar")]
    public Canvas? healthBarCanvas;
    public Slider? healthSlider;
    
    // Private variables
    private float health;
    private bool isDead = false;
    private Transform? target;
    private float lastAttackTime = 0f;
    
    // Components
    private Animator? animator;
    
    // Movement
    private Vector3 moveDirection;
    private float randomMoveTimer = 0f;
    
    void Start()
    {
        // Initialize health
        health = maxHealth;
        
        // Get components
        animator = GetComponent<Animator>();
        
        // Remove ALL rigidbodies to prevent physics issues
        RemoveAllRigidbodies();
        
        // Setup health bar
        SetupHealthBar();
        
        // Ensure character starts at ground level
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        
        Debug.Log($"StableCharacter {name} initialized at position {transform.position}");
    }
    
    void RemoveAllRigidbodies()
    {
        // Remove all rigidbodies from this object and children
        Rigidbody[] allRbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in allRbs)
        {
            if (rb != null)
            {
                DestroyImmediate(rb);
            }
        }
        
        // Also disable all colliders except main one
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        Collider mainCollider = GetComponent<Collider>();
        
        foreach (Collider col in allColliders)
        {
            if (col != mainCollider && col != null)
            {
                col.enabled = false;
            }
        }
        
        Debug.Log($"Removed all rigidbodies from {name}");
    }
    
    void SetupHealthBar()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.worldCamera = Camera.main;
            healthBarCanvas.sortingOrder = 10;
        }
        
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Update health bar position and rotation
        UpdateHealthBar();
        
        // FORCE character to stay on ground - this is critical
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;
        
        // Only move and fight if battle is in progress
        if (GameManager.Instance != null && GameManager.Instance.IsBattleInProgress())
        {
            // Find target and move/attack
            FindNearestEnemy();
            HandleMovement();
            TryAttack();
        }
        
        // Update animator
        UpdateAnimator();
    }
    
    void UpdateHealthBar()
    {
        if (healthBarCanvas != null && Camera.main != null)
        {
            // Position above character
            Vector3 headPosition = transform.position + Vector3.up * 2.2f;
            healthBarCanvas.transform.position = headPosition;
            
            // Face camera
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
    }
    
    void FindNearestEnemy()
    {
        target = null!;
        float nearestDistance = float.MaxValue;
        
        StableCharacter[] allCharacters = FindObjectsOfType<StableCharacter>();
        
        foreach (StableCharacter character in allCharacters)
        {
            if (character != this && character.teamId != teamId && !character.isDead)
            {
                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < nearestDistance && distance < 15f) // Max search range
                {
                    nearestDistance = distance;
                    target = character.transform;
                }
            }
        }
    }
    
    void HandleMovement()
    {
        if (target != null)
        {
            // Move towards target
            Vector3 direction = (target.position - transform.position);
            direction.y = 0; // Keep on ground
            direction = direction.normalized;
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget > attackRange)
            {
                // Simple transform-based movement - NO PHYSICS
                Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
                
                // ALWAYS keep Y at ground level
                newPosition.y = 0.1f;
                
                // Clamp to map bounds
                newPosition.x = Mathf.Clamp(newPosition.x, -15f, 15f);
                newPosition.z = Mathf.Clamp(newPosition.z, -15f, 15f);
                
                transform.position = newPosition;
                moveDirection = direction;
                
                // Look at target smoothly
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
                }
            }
            else
            {
                moveDirection = Vector3.zero;
            }
        }
        else
        {
            // Random movement with less frequency
            randomMoveTimer -= Time.deltaTime;
            if (randomMoveTimer <= 0f)
            {
                randomMoveTimer = Random.Range(3f, 6f);
                moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            }
            
            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime * 0.2f;
                
                // ALWAYS keep Y at ground level
                newPosition.y = 0.1f;
                
                // Clamp to map bounds
                newPosition.x = Mathf.Clamp(newPosition.x, -15f, 15f);
                newPosition.z = Mathf.Clamp(newPosition.z, -15f, 15f);
                
                transform.position = newPosition;
            }
        }
    }
    
    void TryAttack()
    {
        if (target == null) return;
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }
    
    void Attack()
    {
        if (target == null) return;
        
        // Trigger attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Apply damage to target
        StableCharacter targetCharacter = target.GetComponent<StableCharacter>();
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(attackDamage);
        }
        
        Debug.Log($"{name} attacks {target.name}!");
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        health -= damage;
        
        // Update health bar
        if (healthSlider != null)
        {
            healthSlider.value = health / maxHealth;
        }
        
        // Check if dead
        if (health <= 0)
        {
            Die();
        }
        
        Debug.Log($"{name} takes {damage} damage, health: {health}");
    }
    
    void Die()
    {
        isDead = true;
        
        // Simple death - just disable animator and mark as dead
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // Change color to indicate death
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                renderer.material.color = Color.gray;
            }
        }
        
        Debug.Log($"{name} died!");
    }
    
    void UpdateAnimator()
    {
        if (animator != null && !isDead)
        {
            // Set movement speed for animation
            float speed = moveDirection.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }
    
    public void ResetCharacter()
    {
        // Reset health
        health = maxHealth;
        isDead = false;
        
        // Update health bar
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
        
        // Enable animator
        if (animator != null)
        {
            animator.enabled = true;
        }
        
        // Reset color
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                renderer.material.color = Color.white;
            }
        }
        
        // Clear target
        target = null!;
        lastAttackTime = 0f;
        moveDirection = Vector3.zero;
        randomMoveTimer = 0f;
        
        // Force position to ground
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        
        Debug.Log($"{name} reset!");
    }
    
    // Public methods for GameManager compatibility
    public int GetTeamId()
    {
        return teamId;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}