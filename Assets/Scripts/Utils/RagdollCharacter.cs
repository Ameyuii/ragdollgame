using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RagdollCharacter : MonoBehaviour
{
    [Header("Character Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    
    [Header("Team")]
    public int teamId = 1;
    
    [Header("Health Bar")]
    public Canvas? healthBarCanvas;
    public Slider? healthSlider;
    private float currentDisplayedHealth = 100f; // Để làm smooth animation
    
    // Private variables
    private float health;
    private bool isDead = false;
    private bool isRagdoll = false;
    private Transform? target;
    private float lastAttackTime = 0f;
    
    // Components
    private Animator? animator;
    private Rigidbody? mainRigidbody;
    private Collider? mainCollider;
    private Rigidbody[]? ragdollRigidbodies;
    private Collider[]? ragdollColliders;
    
    // Movement
    private Vector3 moveDirection;
    private float randomMoveTimer = 0f;
    
    void Start()
    {
        // Initialize health
        health = maxHealth;
        currentDisplayedHealth = maxHealth;
        
        // Get components
        animator = GetComponent<Animator>();
        mainRigidbody = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        
        // Ensure main rigidbody is kinematic initially
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;
            mainRigidbody.useGravity = false;
        }
        
        // Setup ragdoll
        SetupRagdoll();
        SetupHealthBar();

        // Note: ResetCharacter() will be called after ragdoll setup is complete
    }
    
    void SetupRagdoll()
    {
        // Wait one frame to ensure all components are initialized
        StartCoroutine(SetupRagdollDelayed());
    }
    
    System.Collections.IEnumerator SetupRagdollDelayed()
    {
        yield return null; // Wait one frame
        
        // Get all rigidbodies and colliders in children
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        
        // Disable all ragdoll parts initially with better safety checks
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != mainRigidbody && rb != null)
            {
                // Set kinematic first to avoid velocity warnings
                rb.isKinematic = true;
                rb.useGravity = false;
                // Only reset velocity if not kinematic
                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                // Add better mass and damping defaults
                rb.mass = 1f;
                rb.linearDamping = 5f;
                rb.angularDamping = 10f;
            }
        }
        
        foreach (Collider col in ragdollColliders)
        {
            if (col != mainCollider && col != null)
            {
                col.enabled = false;
            }
        }

        // Now that ragdoll is setup, reset character to proper state
        ResetCharacter();
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
        if (healthBarCanvas != null && Camera.main != null)
        {
            // Position above character
            Vector3 headPosition = transform.position + Vector3.up * 2.2f;
            healthBarCanvas.transform.position = headPosition;
            
            // Face camera
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
        
        // Check if character is out of bounds and reset if needed
        if (transform.position.x < -25f || transform.position.x > 25f || 
            transform.position.z < -25f || transform.position.z > 25f ||
            transform.position.y < -5f || transform.position.y > 50f)
        {
            ResetToGroundLevel();
        }
        
        // Only move and fight if battle is in progress and no external AI is managing this character
        BattleGameManager gameManager = FindAnyObjectByType<BattleGameManager>();
        SimpleCharacterAI externalAI = GetComponent<SimpleCharacterAI>();
        
        // AI COORDINATION LOGIC - Support both AutoAI and Internal AI
        // Use internal AI only if:
        // 1. Battle is started
        // 2. No external SimpleCharacterAI is attached
        // 3. AutoAIManager is disabled or not found
        // OR: Enable hybrid mode for combat logic even with external AI
        AutoAIManager autoAI = FindObjectOfType<AutoAIManager>();
        AICoordinator aiCoordinator = FindObjectOfType<AICoordinator>();
        
        bool shouldUseInternalAI = gameManager != null && gameManager.gameStarted && 
                                  externalAI == null && 
                                  (autoAI == null || !autoAI.enableAutoAI);
        
        // HYBRID MODE: Enable internal AI for combat even with external AI present
        bool hybridMode = aiCoordinator != null && 
                         aiCoordinator.primaryAISystem == AICoordinator.AISystemType.Hybrid;
        
        bool shouldUseHybridCombat = gameManager != null && gameManager.gameStarted && 
                                    externalAI != null && hybridMode && 
                                    autoAI != null && autoAI.enableAutoAI;
        
        if (shouldUseInternalAI || shouldUseHybridCombat)
        {
            if (!isRagdoll)
            {
                // Find target for combat logic
                FindNearestEnemy();
                
                // In hybrid mode, only handle combat, let SimpleAI handle movement
                if (shouldUseHybridCombat)
                {
                    // Only attack logic, no movement
                    TryAttack();
                }
                else
                {
                    // Full AI: movement + combat
                    HandleMovement();
                    TryAttack();
                }
            }
        }
        
        // Update animator
        UpdateAnimator();
    }
    
    void UpdateHealthBar()
    {
        // Cập nhật vị trí và rotation
        if (healthBarCanvas != null && Camera.main != null)
        {
            // Position above character
            Vector3 headPosition = transform.position + Vector3.up * 2.2f;
            healthBarCanvas.transform.position = headPosition;
            
            // Face camera
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
        
        // Cập nhật health bar - CHỈ SỬ DỤNG IMAGE FILLAMOUNT, KHÔNG DÙNG SLIDER
        Transform healthBarTransform = transform.Find("HealthBar");
        if (healthBarTransform != null)
        {
            Transform fillTransform = healthBarTransform.Find("HealthBarBG/HealthBarFill");
            if (fillTransform != null)
            {
                UnityEngine.UI.Image fillImage = fillTransform.GetComponent<UnityEngine.UI.Image>();
                if (fillImage != null)
                {
                    float healthPercentage = currentDisplayedHealth / maxHealth;
                    
                    // Luôn màu xanh, fillAmount giảm dần theo HP (từ 1.0 về 0.0)
                    fillImage.color = Color.green;
                    fillImage.fillAmount = healthPercentage;
                    
                    Debug.Log($"UpdateHealthBar {name}: fillAmount = {fillImage.fillAmount:P0}, health = {health}/{maxHealth}");
                }
            }
        }
    }
    
    IEnumerator SmoothUpdateHealthBar()
    {
        float startHealth = currentDisplayedHealth;
        float targetHealth = health;
        float duration = 0.5f; // 0.5 giây để animate
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth interpolation
            currentDisplayedHealth = Mathf.Lerp(startHealth, targetHealth, t);
            UpdateHealthBar();
            
            yield return null;
        }
        
        // Đảm bảo kết thúc đúng giá trị
        currentDisplayedHealth = targetHealth;
        UpdateHealthBar();
    }
    
    void FindNearestEnemy()
    {
        target = null;
        float nearestDistance = float.MaxValue;
        
        RagdollCharacter[] allCharacters = FindObjectsOfType<RagdollCharacter>();
        
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character != this && character.teamId != teamId && !character.isDead)
            {
                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < nearestDistance)
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
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // Keep on ground
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget > attackRange && distanceToTarget < 50f) // Don't chase too far
            {
                // Move towards target with reduced speed
                moveDirection = direction;
                MoveCharacter(moveDirection * 0.8f); // Reduce speed
                
                // Look at target smoothly
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
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
                randomMoveTimer = Random.Range(2f, 5f); // Longer intervals
                moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            }
            
            MoveCharacter(moveDirection * 0.3f); // Even slower random movement
        }
    }
    
    void MoveCharacter(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            // Calculate new position
            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
            
            // Clamp position to stay within map bounds (assuming ground is 50x50)
            newPosition.x = Mathf.Clamp(newPosition.x, -20f, 20f);
            newPosition.z = Mathf.Clamp(newPosition.z, -20f, 20f);
            
            // Keep character on ground using raycast
            RaycastHit hit;
            if (Physics.Raycast(newPosition + Vector3.up * 1f, Vector3.down, out hit, 5f))
            {
                newPosition.y = hit.point.y + 0.1f;
            }
            else
            {
                newPosition.y = 0.1f; // Fallback
            }
            
            transform.position = newPosition;
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
        RagdollCharacter targetCharacter = target.GetComponent<RagdollCharacter>();
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(attackDamage);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        health -= damage;
        
        // Update health bar với smooth animation
        StartCoroutine(SmoothUpdateHealthBar());
        
        Debug.Log($"{name} nhận {damage} damage. Health: {health}/{maxHealth} ({health/maxHealth:P0})");
        
        // Check if dead
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Trigger ragdoll briefly when hit
            StartCoroutine(TriggerRagdollTemporary(0.5f));
        }
    }
    
    void Die()
    {
        isDead = true;
        
        // Enable ragdoll permanently
        EnableRagdoll();
        
        // Disable animator
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // GameManager notification removed - using simplified system  
        Debug.Log($"Character {name} died");
    }
    
    IEnumerator TriggerRagdollTemporary(float duration)
    {
        if (isDead) yield break;
        
        EnableRagdoll();
        yield return new WaitForSeconds(duration);
        
        if (!isDead)
        {
            DisableRagdoll();
        }
    }
    
    void EnableRagdoll()
    {
        isRagdoll = true;
        
        // Disable animator
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // Disable main rigidbody and collider
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;
            mainRigidbody.useGravity = false;
        }
        
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }
        
        // Enable ragdoll parts with careful physics setup (with null checks)
        if (ragdollRigidbodies != null)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != null && rb != mainRigidbody)
                {
                    // Enable physics first, then reset velocity
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.mass = 0.5f; // Lighter mass
                    rb.linearDamping = 5f; // Higher damping
                    rb.angularDamping = 10f; // Higher angular damping

                    // Reset velocity after setting non-kinematic
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    // Limit velocity to prevent flying
                    rb.maxLinearVelocity = 5f;
                    rb.maxAngularVelocity = 5f;
                }
            }
        }

        if (ragdollColliders != null)
        {
            foreach (Collider col in ragdollColliders)
            {
                if (col != null && col != mainCollider)
                {
                    col.enabled = true;
                }
            }
        }
    }
    
    void DisableRagdoll()
    {
        isRagdoll = false;
        
        // Enable animator
        if (animator != null)
        {
            animator.enabled = true;
        }
        
        // Enable main rigidbody and collider
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;
        }
        
        if (mainCollider != null)
        {
            mainCollider.enabled = true;
        }
        
        // Disable ragdoll parts (with null checks)
        if (ragdollRigidbodies != null)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != null && rb != mainRigidbody)
                {
                    // Set kinematic first to avoid velocity warnings
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    // Only reset velocity if not kinematic (redundant but safe)
                    if (!rb.isKinematic)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }

        if (ragdollColliders != null)
        {
            foreach (Collider col in ragdollColliders)
            {
                if (col != null && col != mainCollider)
                {
                    col.enabled = false;
                }
            }
        }
        
        // Reset position to ground
        ResetToGroundLevel();
    }
    
    void ResetToGroundLevel()
    {
        // Clamp position to stay within map bounds first
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -20f, 20f);
        pos.z = Mathf.Clamp(pos.z, -20f, 20f);
        
        // Raycast down to find ground
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 2f, Vector3.down, out hit, 10f))
        {
            pos.y = hit.point.y + 0.1f;
        }
        else
        {
            pos.y = 0.1f;
        }
        
        transform.position = pos;
        
        // Reset rotation
        transform.rotation = Quaternion.identity;
    }
    
    void UpdateAnimator()
    {
        if (animator != null && !isRagdoll)
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
        
        // Disable ragdoll
        DisableRagdoll();
        
        // Clear target
        target = null;
        lastAttackTime = 0f;
        moveDirection = Vector3.zero;
        randomMoveTimer = 0f;
    }
    
    // Public methods for GameManager
    public void EnablePhysics()
    {
        // Just ensure character is in normal state
        if (!isDead)
        {
            DisableRagdoll();
            
            // Ensure main rigidbody stays kinematic for controlled movement
            if (mainRigidbody != null)
            {
                mainRigidbody.isKinematic = true;
                mainRigidbody.useGravity = false;
            }
        }
    }
    
    public int GetTeamId()
    {
        return teamId;
    }
    
    public bool IsDead()
    {
        return isDead;
    }

    // Force reset character to stable state (for debugging)
    public void ForceStableReset()
    {
        // Reset basic state
        isDead = false;
        isRagdoll = false;
        health = maxHealth;

        // Reset position
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;
        transform.rotation = Quaternion.identity;

        // Reset main rigidbody
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;
            mainRigidbody.useGravity = false;
            // Only reset velocity if not kinematic (redundant but safe)
            if (!mainRigidbody.isKinematic)
            {
                mainRigidbody.linearVelocity = Vector3.zero;
                mainRigidbody.angularVelocity = Vector3.zero;
            }
        }

        // Force disable all ragdoll parts
        Rigidbody[] allRbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in allRbs)
        {
            if (rb != null && rb != mainRigidbody)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                // Only reset velocity if not kinematic (redundant but safe)
                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        // Force disable all ragdoll colliders
        Collider[] allCols = GetComponentsInChildren<Collider>();
        foreach (Collider col in allCols)
        {
            if (col != null && col != mainCollider)
            {
                col.enabled = false;
            }
        }

        // Enable main collider
        if (mainCollider != null)
        {
            mainCollider.enabled = true;
        }

        // Enable animator
        if (animator != null)
        {
            animator.enabled = true;
        }

        Debug.Log($"Force reset {name} to stable state");
    }
    
    // Method called when character dies - notify GameManager
    void NotifyGameManager()
    {
        BattleGameManager gameManager = FindAnyObjectByType<BattleGameManager>();
        if (gameManager != null)
        {
            gameManager.OnCharacterDeath(this);
        }
    }
    
    // Get current health for UI and debugging
    public float GetCurrentHealth()
    {
        return health;
    }
}