using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Base class cho tất cả NPCs trong game
/// Chứa các chức năng cơ bản: di chuyển, tấn công, AI
/// Sao chép chức năng từ NPCController cũ
/// Hỗ trợ CharacterData ScriptableObject để dễ dàng tạo nhân vật mới
/// </summary>
public abstract class NPCBaseController : MonoBehaviour
{
    [Header("Character Data")]
    [Tooltip("ScriptableObject chứa thông tin và stats của nhân vật")]
    [SerializeField] protected CharacterData? characterData;
    
    [Header("Thuộc tính cơ bản (Sẽ được cập nhật từ CharacterData)")]
    [Tooltip("Máu tối đa của nhân vật")]
    public float maxHealth = 100f;
    
    [Tooltip("Máu hiện tại của nhân vật")]
    [SerializeField] protected float currentHealth;
    
    [Tooltip("ID của team (nhân vật khác team sẽ tấn công nhau)")]
    public int team = 0;
    
    [Header("Thiết lập di chuyển")]
    [Tooltip("Tốc độ di chuyển của nhân vật")]
    public float moveSpeed = 3.5f;
    
    [Tooltip("Tốc độ xoay của nhân vật (độ/giây)")]
    public float rotationSpeed = 120f;
    
    [Tooltip("Tốc độ tăng tốc")]
    public float acceleration = 8f;
    
    [Header("Thiết lập tấn công")]
    [Tooltip("Sát thương mỗi đòn tấn công")]
    public float attackDamage = 20f;
    
    [Tooltip("Thời gian hồi chiêu (giây)")]
    public float attackCooldown = 1f;
    
    [Tooltip("Tầm đánh (m)")]
    public float attackRange = 2f;
    
    [Tooltip("Thời gian animation attack (giây)")]
    public float attackAnimationDuration = 1.0f;
    
    [Tooltip("Timing hit trong animation (0.0-1.0)")]
    [Range(0.1f, 0.9f)]
    public float attackHitTiming = 0.65f;
    
    [Header("Thiết lập AI")]
    [Tooltip("Khoảng cách phát hiện kẻ địch (m)")]
    public float detectionRange = 30f;
    
    [Tooltip("Layer chứa kẻ địch")]
    public LayerMask enemyLayerMask;
    
    [Tooltip("Layer chứa chướng ngại vật")]
    public LayerMask obstacleLayerMask;
    
    [Header("Physics Impact Settings")]
    [Tooltip("Lực đẩy khi tấn công (impact force)")]
    public float impactForce = 25f;
    
    [Tooltip("Lực knockback khi bị tấn công")]
    public float knockbackForce = 150f;
    
    [Tooltip("Lực nâng lên (upward force) khi knockback")]
    public float knockbackUpwardForce = 50f;
    
    [Tooltip("Khoảng cách tối đa để có physics impact")]
    public float maxImpactDistance = 2.5f;
    
    [Header("Hiệu ứng")]
    [Tooltip("Hiệu ứng khi bị đánh")]
    public GameObject hitEffect;
    
    [Tooltip("Hiệu ứng khi chết")]
    public GameObject deathEffect;
    
    [Header("Debug Options")]
    [Tooltip("Hiển thị thông tin debug chi tiết")]
    public bool showDebugLogs = true;
    
    [Header("Attack Variation Settings")]
    [Tooltip("Tỷ lệ sử dụng attack thông thường (%)")]
    [Range(0f, 100f)]
    public float basicAttackChance = 40f;
    
    [Tooltip("Tỷ lệ sử dụng attack1 (%)")]
    [Range(0f, 100f)]
    public float attack1Chance = 30f;
    
    [Tooltip("Tỷ lệ sử dụng attack2 (%)")]
    [Range(0f, 100f)]
    public float attack2Chance = 30f;
    
    [Header("Advanced Attack Settings")]
    [Tooltip("Có sử dụng cooldown khác nhau cho từng loại attack không")]
    public bool useVariableAttackCooldown = false;
    
    [Tooltip("Cooldown cho attack1")]
    public float attack1Cooldown = 1.2f;
    
    [Tooltip("Cooldown cho attack2")]
    public float attack2Cooldown = 1.5f;
    
    [Header("Ragdoll Reaction Settings")]
    [Tooltip("Có kích hoạt ragdoll mỗi khi bị đánh không")]
    public bool enableRagdollOnHit = true;
    
    [Tooltip("Cooldown giữa các lần kích hoạt ragdoll (giây)")]
    public float ragdollCooldown = 1f;
    
    [Tooltip("Sát thương tối thiểu để kích hoạt ragdoll")]
    public float minDamageForRagdoll = 10f;
    
    [Header("Hit Reaction Settings")]
    [Tooltip("Ngưỡng lực tối thiểu để kích hoạt ragdoll (dưới mức này chỉ có hit reaction)")]
    public float ragdollForceThreshold = 100f;
    
    [Tooltip("Có thể phản công ngay lập tức khi bị đánh nhẹ không")]
    public bool canCounterAttack = true;
    
    [Tooltip("Thời gian cooldown cho counter attack (giây)")]
    public float counterAttackCooldown = 0.5f;
    
    [Tooltip("Tỷ lệ damage cho counter attack (% của damage bình thường)")]
    [Range(0.5f, 2f)]
    public float counterAttackDamageMultiplier = 1.2f;
    
    // Biến theo dõi trạng thái
    protected float lastAttackTime;
    protected float lastRagdollTime; // Thời gian ragdoll cuối cùng
    protected float lastCounterAttackTime; // Thời gian counter attack cuối cùng
    protected bool hasDealtDamageThisAttack = false; // Đã gây sát thương cho đòn tấn công hiện tại chưa
    protected bool isDead = false;
    protected NPCBaseController targetEnemy;
    protected NPCBaseController currentAttackTarget;
    protected bool isMoving = false;
    
    // Biến để xử lý smooth transition và tránh trượt
    protected bool isTransitioning = false;
    protected float transitionStartTime = 0f;
    protected float transitionDuration = 0.3f;
    protected Vector3 lastFramePosition;
    protected bool hasStartedMoving = false;
    protected bool isInitialized = false;
    
    // Tham chiếu các thành phần
    protected Animator animator;
    protected NavMeshAgent navMeshAgent;
    
    // Tên các tham số animator
    protected static readonly string ANIM_IS_WALKING = "IsWalking";
    protected static readonly string ANIM_ATTACK = "Attack";
    protected static readonly string ANIM_ATTACK1 = "Attack1";
    protected static readonly string ANIM_ATTACK2 = "Attack2";
    protected static readonly string ANIM_HIT = "Hit";
    protected static readonly string ANIM_DIE = "Die";
    
    // Biến theo dõi loại attack cuối cùng
    protected string lastUsedAttackTrigger = "";
    
    /// <summary>
    /// Property để các class con truy cập CharacterData
    /// </summary>
    public CharacterData? CharacterData => characterData;
    
    /// <summary>
    /// Cập nhật Inspector values từ CharacterData (gọi trong Editor)
    /// </summary>
    [ContextMenu("🔄 Update From CharacterData")]
    public virtual void UpdateFromCharacterData()
    {
        if (characterData != null)
        {
            // Backup current health percentage
            float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 1f;
            
            // Apply CharacterData stats
            ApplyCharacterData();
            
            // Restore health percentage
            currentHealth = maxHealth * healthPercentage;
            
            Debug.Log($"🔄 {gameObject.name}: Inspector đã được cập nhật từ CharacterData '{characterData.characterName}'");
            
            #if UNITY_EDITOR
            // Mark object as dirty để Unity biết có thay đổi
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        else
        {
            Debug.LogWarning($"⚠️ {gameObject.name}: Không có CharacterData để cập nhật!");
        }
    }
    
    /// <summary>
    /// Validation khi CharacterData thay đổi trong Inspector
    /// </summary>
    protected virtual void OnValidate()
    {
        // Chỉ chạy trong Editor và khi game không đang chạy
        #if UNITY_EDITOR
        if (!Application.isPlaying && characterData != null)
        {
            // Delay để tránh lỗi khi Unity đang serialize
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && characterData != null)
                {
                    UpdateFromCharacterData();
                }
            };
        }
        #endif
    }
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    protected virtual void Start()
    {
        // Apply CharacterData nếu có
        ApplyCharacterData();
        
        // Khởi tạo máu
        currentHealth = maxHealth;
        
        // Thiết lập NavMeshAgent
        InitializeNavMeshAgent();
        
        // Thiết lập layer mask nếu chưa được thiết lập
        if (enemyLayerMask.value == 0)
        {
            enemyLayerMask = -1; // All layers
            if (showDebugLogs) Debug.Log($"⚠️ {gameObject.name}: Auto-set enemyLayerMask to ALL LAYERS");
        }
        
        if (obstacleLayerMask.value == 0)
        {
            obstacleLayerMask = LayerMask.GetMask("Default");
        }
        
        // Validate và normalize tỷ lệ attack
        ValidateAttackChances();
        
        // Lưu trạng thái khởi tạo
        isInitialized = true;
        
        // Log thông tin team để debug
        if (showDebugLogs) Debug.Log($"{gameObject.name}: Đã khởi tạo với team {team}");
        
        // Bắt đầu tìm kiếm kẻ địch
        StartCoroutine(FindEnemyRoutine());
        
        // Bắt đầu patrol nếu không có mục tiêu
        StartCoroutine(PatrolWhenIdle());
        
        // Khởi tạo vị trí theo dõi
        lastFramePosition = transform.position;
    }
      protected virtual void Update()
    {
        if (isDead || !isInitialized) return;
        
        // Tìm kẻ địch trong phạm vi
        FindEnemyInRange();
        
        // Nếu có mục tiêu thì tấn công khi trong tầm
        if (targetEnemy != null && !targetEnemy.IsDead())
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
            
            if (distanceToTarget <= attackRange)
            {
                // Dừng di chuyển trước tiên để tránh trượt
                StopMoving();
                
                // Xoay về phía mục tiêu
                RotateTowards(targetEnemy.transform.position);
                
                // Tấn công nếu có thể
                if (CanAttack())
                {
                    Attack(targetEnemy);
                }
            }
            else
            {
                // Di chuyển đến mục tiêu với transition smooth
                MoveToTarget(targetEnemy.transform.position);
            }
        }
    }
    
    protected virtual void InitializeNavMeshAgent()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.stoppingDistance = 1.0f;
            
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            navMeshAgent.autoBraking = true;
            navMeshAgent.autoRepath = true;
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            
            navMeshAgent.radius = 0.4f;
            navMeshAgent.height = 1.8f;
            navMeshAgent.stoppingDistance = 1.5f;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            
            if (showDebugLogs) Debug.Log($"✅ {gameObject.name}: NavMeshAgent đã được kích hoạt và cấu hình");
        }
        else
        {
            Debug.LogError($"❌ {gameObject.name}: KHÔNG CÓ NavMeshAgent! NPC sẽ không thể di chuyển!");
        }
    }
    
    // Xử lý di chuyển mượt mà với transition
    protected virtual void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            // Nếu đang trong transition, chờ hoàn thành
            if (isTransitioning)
            {
                if (Time.time - transitionStartTime >= transitionDuration)
                {
                    isTransitioning = false;
                    hasStartedMoving = true;
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(targetPosition);
                    
                    if (showDebugLogs)
                        Debug.Log($"{gameObject.name}: ✅ Hoàn thành transition, bắt đầu di chuyển thực sự");
                }
                return;
            }
            
            // Nếu chưa bắt đầu di chuyển, kích hoạt animation và bắt đầu transition
            if (!hasStartedMoving)
            {
                StartMovementTransition();
                navMeshAgent.SetDestination(targetPosition);
                navMeshAgent.isStopped = true;
                return;
            }
            
            // Di chuyển bình thường
            navMeshAgent.SetDestination(targetPosition);
            navMeshAgent.isStopped = false;
            
            // Cập nhật animation dựa trên tốc độ thực tế
            bool isCurrentlyMoving = navMeshAgent.velocity.magnitude > 0.1f;
            UpdateAnimationState(isCurrentlyMoving);
        }
    }
    
    // Bắt đầu transition khi chuyển từ idle sang walking
    protected virtual void StartMovementTransition()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            transitionStartTime = Time.time;
            
            // Kích hoạt animation đi bộ ngay lập tức
            UpdateAnimationState(true);
            
            if (showDebugLogs)
                Debug.Log($"{gameObject.name}: 🎬 Bắt đầu transition từ idle sang walking");
        }
    }
    
    // Tìm kiếm kẻ địch gần nhất định kỳ
    protected virtual IEnumerator FindEnemyRoutine()
    {
        while (!isDead)
        {
            FindClosestEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Tìm kẻ địch gần nhất
    protected virtual void FindClosestEnemy()
    {
        if (!isInitialized) return;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayerMask);
        
        if (showDebugLogs) Debug.Log($"🔍 {gameObject.name}: Tìm thấy {hitColliders.Length} objects trong phạm vi {detectionRange}m");
        
        float shortestDistance = float.MaxValue;
        NPCBaseController? nearestEnemy = null;
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject) continue;
            
            NPCBaseController otherCharacter = hitCollider.GetComponent<NPCBaseController>();
            if (otherCharacter != null && !otherCharacter.IsDead())
            {
                if (otherCharacter.team != team)
                {
                    float distance = Vector3.Distance(transform.position, otherCharacter.transform.position);
                    
                    if (HasLineOfSight(otherCharacter.transform))
                    {
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            nearestEnemy = otherCharacter;
                        }
                    }
                }
            }
        }
        
        // Cập nhật mục tiêu
        if (nearestEnemy != null && targetEnemy != nearestEnemy)
        {
            targetEnemy = nearestEnemy;
            hasStartedMoving = false;
            
            if (showDebugLogs)
                Debug.Log($"🎯 {gameObject.name}: Phát hiện kẻ địch mới {targetEnemy.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Tìm kẻ địch trong phạm vi phát hiện
    /// </summary>
    protected virtual void FindEnemyInRange()
    {
        if (isDead) return;
        
        // Nếu đã có target và target vẫn sống, không cần tìm mới
        if (targetEnemy != null && !targetEnemy.IsDead())
        {
            float distanceToCurrentTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToCurrentTarget <= detectionRange)
            {
                return; // Giữ target hiện tại
            }
        }
          // Tìm kẻ địch mới
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayerMask);
        NPCBaseController? closestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (var enemyCollider in enemies)
        {
            NPCBaseController enemy = enemyCollider.GetComponent<NPCBaseController>();
            if (enemy != null && enemy != this && enemy.team != team && !enemy.IsDead())
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        
        if (closestEnemy != null && closestEnemy != targetEnemy)
        {
            targetEnemy = closestEnemy;
            if (showDebugLogs) Debug.Log($"🎯 {gameObject.name}: Phát hiện kẻ địch mới: {targetEnemy.gameObject.name} (khoảng cách: {closestDistance:F1}m)");
        }
    }

    // Kiểm tra xem có đường nhìn đến mục tiêu không
    protected virtual bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;
        
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        
        Vector3 startPoint = transform.position + Vector3.up * 1f;
        Vector3 targetPoint = target.position + Vector3.up * 1f;
        
        if (Physics.Raycast(startPoint, targetPoint - startPoint, out RaycastHit hit, distanceToTarget, obstacleLayerMask))
        {
            return false;
        }
        
        return true;
    }
    
    public virtual bool CanAttack()
    {
        float currentCooldown = attackCooldown;
        
        if (useVariableAttackCooldown)
        {
            switch (lastUsedAttackTrigger)
            {
                case var trigger when trigger == ANIM_ATTACK1:
                    currentCooldown = attack1Cooldown;
                    break;
                case var trigger when trigger == ANIM_ATTACK2:
                    currentCooldown = attack2Cooldown;
                    break;
                default:
                    currentCooldown = attackCooldown;
                    break;
            }
        }
        
        return Time.time >= lastAttackTime + currentCooldown;
    }
    
    public virtual void Attack(NPCBaseController target)
    {
        if (isDead || !CanAttack() || target == null) return;
          // Cập nhật thời gian tấn công
        lastAttackTime = Time.time;
        hasDealtDamageThisAttack = false; // Reset flag cho đòn tấn công mới
        
        // Lưu target để sử dụng khi animation hit
        currentAttackTarget = target;
        
        // Kích hoạt animation tấn công với random attack
        if (animator != null)
        {
            string attackTrigger = GetRandomAttackTrigger();
            animator.SetTrigger(attackTrigger);
            lastUsedAttackTrigger = attackTrigger;
            
            if (showDebugLogs) Debug.Log($"🎯 {gameObject.name} thực hiện attack ({attackTrigger}) → {target.gameObject.name}");
            
            // Bắt đầu coroutine để delay damage đến hit frame
            StartCoroutine(DelayedAttackHit());
        }
        else
        {
            // Fallback: nếu không có animator thì gây damage ngay
            DealDamageToTarget();
        }
    }
    
    // Coroutine để delay damage đến timing phù hợp với animation
    protected virtual System.Collections.IEnumerator DelayedAttackHit()
    {
        // Chờ đến timing hit được cấu hình
        yield return new WaitForSeconds(attackAnimationDuration * attackHitTiming);
        
        // Gây damage tại thời điểm hit
        OnAttackHit();
    }
    
    // Method này sẽ được gọi từ Animation Event tại hit frame
    public virtual void OnAttackHit()
    {
        DealDamageToTarget();
    }
      // Gây sát thương thực sự cho target hiện tại
    protected virtual void DealDamageToTarget()
    {
        if (currentAttackTarget == null || currentAttackTarget.IsDead()) 
        {
            if (showDebugLogs) Debug.Log($"❌ {gameObject.name}: Không có target hợp lệ để gây damage");
            return;
        }

        // Kiểm tra đã gây sát thương cho đòn tấn công này chưa
        if (hasDealtDamageThisAttack)
        {
            if (showDebugLogs) Debug.Log($"❌ {gameObject.name}: Đã gây sát thương cho đòn tấn công này rồi");
            return;
        }
        
        // Kiểm tra target vẫn trong tầm đánh
        float distanceToTarget = Vector3.Distance(transform.position, currentAttackTarget.transform.position);
        if (distanceToTarget > attackRange * 1.2f)
        {
            if (showDebugLogs) Debug.Log($"❌ {gameObject.name}: Target {currentAttackTarget.gameObject.name} đã ra khỏi tầm đánh");
            return;
        }
        
        // Đánh dấu đã gây sát thương cho đòn này
        hasDealtDamageThisAttack = true;
        
        // Gây sát thương
        currentAttackTarget.TakeDamage(attackDamage, this);
        
        // Thêm impact vật lý
        AddPhysicsImpact(currentAttackTarget);
        
        // Debug log
        if (showDebugLogs) Debug.Log($"⚔️ {gameObject.name} (Team {team}) gây {attackDamage} sát thương cho {currentAttackTarget.gameObject.name} (Team {currentAttackTarget.team})!");
          // Reset target sau khi attack
        currentAttackTarget = null!;
    }
    
    // Thêm tác động vật lý khi tấn công
    protected virtual void AddPhysicsImpact(NPCBaseController target)
    {
        if (target == null) return;
        
        Vector3 impactDirection = (target.transform.position - transform.position).normalized;
        
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > maxImpactDistance) return;
          Rigidbody? targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null && !target.isDead)
        {
            Vector3 forceDirection = impactDirection + Vector3.up * 0.1f;
            targetRb.AddForce(forceDirection * impactForce, ForceMode.Impulse);
            
            if (showDebugLogs) Debug.Log($"💥 {gameObject.name}: Đẩy nhẹ {target.gameObject.name} với lực {impactForce}");
        }
        
        // Kích hoạt ragdoll khi máu thấp hoặc chết
        if (target.currentHealth <= target.maxHealth * 0.3f || target.isDead)
        {
            TriggerRagdollOnTarget(target, impactDirection, impactForce);
        }
    }
    
    // Kích hoạt ragdoll trên target
    protected virtual void TriggerRagdollOnTarget(NPCBaseController target, Vector3 impactDirection, float impactForce)
    {
        // Tìm RagdollController
        RagdollController ragdollController = target.GetComponent<RagdollController>();
        if (ragdollController != null)
        {
            Vector3 forceDirection = impactDirection + Vector3.up * 0.2f;
            Vector3 impactPoint = target.transform.position + Vector3.up * 1f;
            ragdollController.KichHoatRagdoll(forceDirection * (impactForce * 2f), impactPoint);
            if (showDebugLogs) Debug.Log($"💀 {target.gameObject.name}: Kích hoạt ragdoll do máu thấp!");
            return;
        }
        
        // Fallback: Tìm NPCRagdollManager
        NPCRagdollManager ragdollManager = FindFirstObjectByType<NPCRagdollManager>();
        if (ragdollManager != null)
        {
            ragdollManager.TanCongNPC(target.gameObject, impactDirection);
            if (showDebugLogs) Debug.Log($"💀 {target.gameObject.name}: Kích hoạt Ragdoll qua Manager!");
        }
    }
    
    public virtual void TakeDamage(float damage, NPCBaseController attacker)
    {
        if (isDead) return;
        
        currentHealth -= damage;        // Tính toán lực tác động và quyết định phản ứng
        if (attacker != null)
        {
            Vector3 knockbackDirection = (transform.position - attacker.transform.position).normalized;
            float totalForce = knockbackForce + knockbackUpwardForce;
            
            // Quyết định loại phản ứng dựa trên lực tác động
            if (totalForce >= ragdollForceThreshold && enableRagdollOnHit && 
                damage >= minDamageForRagdoll && (Time.time - lastRagdollTime) >= ragdollCooldown)
            {
                // Lực đủ mạnh → Kích hoạt ragdoll
                ActivateRagdollReaction(knockbackDirection, attacker);
            }
            else
            {
                // Lực nhẹ → Hit reaction và có thể counter attack
                ActivateHitReaction(knockbackDirection, attacker, damage);
            }
        }
          // Phát hiệu ứng bị đánh nếu có
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        if (showDebugLogs) Debug.Log($"💔 {gameObject.name} nhận {damage} sát thương từ {(attacker ? attacker.gameObject.name : "Unknown")}. Máu còn: {currentHealth:F1}/{maxHealth}");
          // Kiểm tra nếu đã chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        if (isDead) return;
        
        isDead = true;
        currentHealth = 0;
        
        // Kích hoạt ragdoll khi chết
        TriggerRagdoll();
        
        // Kích hoạt animation chết nếu có
        if (animator != null)
        {
            animator.SetTrigger(ANIM_DIE);
        }
        
        // Phát hiệu ứng chết nếu có
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Vô hiệu hóa NavMeshAgent sau một chút để ragdoll có thời gian hoạt động
        StartCoroutine(DisableNavMeshAfterDelay(1f));
        
        // Xóa gameObject sau thời gian
        Destroy(gameObject, 10f);
        
        if (showDebugLogs) Debug.Log($"💀 {gameObject.name} (Team {team}) đã chết và kích hoạt ragdoll");
    }
    
    // Coroutine để vô hiệu hóa NavMesh sau delay
    protected virtual IEnumerator DisableNavMeshAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
    }
    
    // Kích hoạt ragdoll cho bản thân
    protected virtual void TriggerRagdoll()
    {
        // Tìm RagdollController
        RagdollController ragdollController = GetComponent<RagdollController>();
        if (ragdollController != null)
        {
            ragdollController.KichHoatRagdollNgayLapTuc();
            if (showDebugLogs) Debug.Log($"💀 {gameObject.name}: Kích hoạt Ragdoll!");
            return;
        }
        
        // Fallback: Tìm NPCRagdollManager
        NPCRagdollManager ragdollManager = FindFirstObjectByType<NPCRagdollManager>();
        if (ragdollManager != null)
        {
            Vector3 attackDirection = Vector3.forward;
            ragdollManager.TanCongNPC(gameObject, attackDirection);
            if (showDebugLogs) Debug.Log($"💀 {gameObject.name}: Kích hoạt Ragdoll qua Manager!");
        }
        else
        {
            if (showDebugLogs) Debug.LogWarning($"⚠️ {gameObject.name}: Không tìm thấy Ragdoll system!");
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    // Cập nhật trạng thái animation
    protected virtual void UpdateAnimationState(bool moving)
    {
        if (animator != null)
        {
            if (isMoving != moving)
            {
                isMoving = moving;
                
                if (moving)
                {
                    animator.SetBool(ANIM_IS_WALKING, true);
                    if (showDebugLogs) 
                        Debug.Log($"{gameObject.name}: 🚶 Chuyển sang animation đi bộ");
                }
                else
                {
                    animator.SetBool(ANIM_IS_WALKING, false);
                    if (showDebugLogs) 
                        Debug.Log($"{gameObject.name}: 🧍 Chuyển về animation đứng yên");
                }
            }
            
            try
            {
                animator.SetBool(ANIM_IS_WALKING, isMoving);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{gameObject.name}: Không thể đặt tham số {ANIM_IS_WALKING} trên Animator: {e.Message}");
            }
        }
    }
    
    // Dừng di chuyển
    protected virtual void StopMoving()
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            UpdateAnimationState(false);
        }
        
        // Reset movement states
        isTransitioning = false;
        hasStartedMoving = false;
        
        if (showDebugLogs) 
            Debug.Log($"{gameObject.name}: ⏹️ Đã dừng di chuyển hoàn toàn");
    }
    
    // Xoay về phía mục tiêu
    protected virtual void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            bool wasMoving = isMoving;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
            
            bool needRotation = angleToTarget > 20f;
            
            if (needRotation)
            {
                UpdateAnimationState(true);
            }
            
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
            
            if (angleToTarget < 5f && !wasMoving && needRotation)
            {
                UpdateAnimationState(false);
                
                if (showDebugLogs) Debug.Log($"{gameObject.name}: Đã hoàn thành quay, chuyển về trạng thái đứng yên");
            }
        }
    }
    
    // Tìm kiếm và di chuyển đến vị trí ngẫu nhiên trong khi không có mục tiêu
    protected virtual IEnumerator PatrolWhenIdle()
    {
        yield return new WaitForSeconds(2f);
        
        while (!isDead)
        {
            if (targetEnemy == null && isInitialized && navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                Vector3 randomDirection = Random.insideUnitSphere * (detectionRange * 0.5f);
                randomDirection += transform.position;
                randomDirection.y = transform.position.y;
                
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, detectionRange, NavMesh.AllAreas))
                {
                    hasStartedMoving = false;
                    MoveToTarget(hit.position);
                    
                    if (showDebugLogs) Debug.Log($"🚶 {gameObject.name}: Không có mục tiêu, patrol đến {hit.position}");
                }
                
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    // Random chọn attack trigger theo tỷ lệ được cấu hình
    protected virtual string GetRandomAttackTrigger()
    {
        float randomValue = Random.Range(0f, 100f);
        string selectedTrigger;
        
        if (randomValue <= basicAttackChance)
        {
            selectedTrigger = ANIM_ATTACK;
        }
        else if (randomValue <= basicAttackChance + attack1Chance)
        {
            selectedTrigger = ANIM_ATTACK1;
        }
        else
        {
            selectedTrigger = ANIM_ATTACK2;
        }
        
        if (showDebugLogs) 
            Debug.Log($"🎲 {gameObject.name}: Random value: {randomValue:F1}% → Attack trigger: {selectedTrigger}");
            
        return selectedTrigger;
    }

    // Validate và normalize tỷ lệ attack để đảm bảo tổng = 100%
    protected virtual void ValidateAttackChances()
    {
        float totalChance = basicAttackChance + attack1Chance + attack2Chance;
        
        if (Mathf.Abs(totalChance - 100f) > 0.1f)
        {
            if (showDebugLogs) 
                Debug.LogWarning($"⚠️ {gameObject.name}: Tổng tỷ lệ attack không bằng 100% ({totalChance:F1}%). Đang normalize...");
            
            if (totalChance > 0)
            {
                basicAttackChance = (basicAttackChance / totalChance) * 100f;
                attack1Chance = (attack1Chance / totalChance) * 100f;
                attack2Chance = (attack2Chance / totalChance) * 100f;
            }
            else
            {
                basicAttackChance = 40f;
                attack1Chance = 30f;
                attack2Chance = 30f;
            }
        }
        
        if (showDebugLogs)
            Debug.Log($"✅ {gameObject.name}: Attack chances - Basic: {basicAttackChance:F1}%, Attack1: {attack1Chance:F1}%, Attack2: {attack2Chance:F1}%");
    }
    
    // Animation Events
    public virtual void OnFootstep()
    {
        if (showDebugLogs) Debug.Log($"{gameObject.name}: Bước chân");
    }
    
    // Hiển thị phạm vi phát hiện và tầm đánh trong Scene View
    protected virtual void OnDrawGizmosSelected()
    {
        // Phạm vi phát hiện
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Tầm đánh
        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Hiển thị mục tiêu hiện tại
        if (targetEnemy != null && !targetEnemy.IsDead())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, targetEnemy.transform.position + Vector3.up);
        }
    }
    
    // Method để debug thông tin attack system
    [ContextMenu("Debug Attack System Info")]
    public void DebugAttackSystemInfo()
    {
        Debug.Log($"=== ATTACK SYSTEM INFO - {gameObject.name} ===");
        Debug.Log($"📊 Attack Chances: Basic {basicAttackChance:F1}%, Attack1 {attack1Chance:F1}%, Attack2 {attack2Chance:F1}%");
        Debug.Log($"⏰ Cooldowns: Basic {attackCooldown}s, Attack1 {attack1Cooldown}s, Attack2 {attack2Cooldown}s");
        Debug.Log($"🎯 Last Used Attack: {(string.IsNullOrEmpty(lastUsedAttackTrigger) ? "None" : lastUsedAttackTrigger)}");
        Debug.Log($"🔄 Variable Cooldown: {(useVariableAttackCooldown ? "Enabled" : "Disabled")}");
        Debug.Log($"⚔️ Can Attack Now: {CanAttack()}");
        if (!CanAttack())
        {
            float timeRemaining = (lastAttackTime + attackCooldown) - Time.time;
            Debug.Log($"⏳ Time until next attack: {timeRemaining:F1}s");
        }
        Debug.Log("==========================================");
    }
    
    // Test methods
    [ContextMenu("Test Attack")]
    public void TestAttack()
    {
        if (showDebugLogs) Debug.Log($"🧪 {gameObject.name}: Test Attack method called");
    }    /// <summary>
    /// Apply CharacterData ScriptableObject vào NPCBaseController
    /// Bây giờ load TOÀN BỘ fields từ CharacterData
    /// </summary>
    protected virtual void ApplyCharacterData()
    {
        if (characterData != null)
        {
            // Basic Stats
            maxHealth = characterData.maxHealth;
            team = characterData.teamId;
            
            // Movement Settings
            moveSpeed = characterData.moveSpeed;
            rotationSpeed = characterData.rotationSpeed;
            acceleration = characterData.acceleration;
            
            // Combat Stats
            attackDamage = characterData.baseDamage;
            attackCooldown = characterData.attackCooldown;
            attackRange = characterData.attackRange;
            attackAnimationDuration = characterData.attackAnimationDuration;
            attackHitTiming = characterData.attackHitTiming;
            
            // AI Settings
            detectionRange = characterData.detectionRange;
            enemyLayerMask = characterData.enemyLayerMask;
            obstacleLayerMask = characterData.obstacleLayerMask;
            
            // Attack Variation Settings
            basicAttackChance = characterData.basicAttackChance;
            attack1Chance = characterData.attack1Chance;
            attack2Chance = characterData.attack2Chance;
            
            // Advanced Attack Settings
            useVariableAttackCooldown = characterData.useVariableAttackCooldown;
            attack1Cooldown = characterData.attack1Cooldown;
            attack2Cooldown = characterData.attack2Cooldown;
            
            // Effects (nếu có trong CharacterData)
            if (characterData.hitEffect != null)
                hitEffect = characterData.hitEffect;
            if (characterData.deathEffect != null)
                deathEffect = characterData.deathEffect;
            
            // Physics Impact Settings
            impactForce = characterData.impactForce;
            knockbackForce = characterData.knockbackForce;
            knockbackUpwardForce = characterData.knockbackUpwardForce;
            maxImpactDistance = characterData.maxImpactDistance;
            
            // Ragdoll Reaction Settings
            enableRagdollOnHit = characterData.enableRagdollOnHit;
            ragdollCooldown = characterData.ragdollCooldown;
            minDamageForRagdoll = characterData.minDamageForRagdoll;
            
            // Hit Reaction Settings
            ragdollForceThreshold = characterData.ragdollForceThreshold;
            canCounterAttack = characterData.canCounterAttack;
            counterAttackCooldown = characterData.counterAttackCooldown;
            counterAttackDamageMultiplier = characterData.counterAttackDamageMultiplier;
            
            // Debug Settings
            showDebugLogs = characterData.showDebugLogs;

            if (showDebugLogs) Debug.Log($"📜 {gameObject.name}: Đã áp dụng TOÀN BỘ CharacterData '{characterData.characterName}' - HP:{maxHealth}, Damage:{attackDamage}, Team:{team}, Speed:{moveSpeed}, Impact:{impactForce}");
        }
        else
        {
            if (showDebugLogs) Debug.Log($"⚠️ {gameObject.name}: Không có CharacterData, sử dụng giá trị mặc định");
        }
    }

    /// <summary>
    /// Kích hoạt phản ứng ragdoll khi bị tấn công mạnh
    /// </summary>
    protected virtual void ActivateRagdollReaction(Vector3 knockbackDirection, NPCBaseController attacker)
    {
        RagdollController ragdollController = GetComponent<RagdollController>();
        if (ragdollController != null)
        {
            Vector3 impactForce = knockbackDirection * knockbackForce + Vector3.up * knockbackUpwardForce;
            Vector3 impactPoint = transform.position + Vector3.up * 1f;
            ragdollController.KichHoatRagdoll(impactForce, impactPoint);
            
            lastRagdollTime = Time.time;
            
            if (showDebugLogs) Debug.Log($"💥 {gameObject.name}: Kích hoạt ragdoll - lực mạnh từ {attacker.gameObject.name}");
        }
        else
        {
            // Fallback: Knockback mạnh
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(knockbackDirection * knockbackForce + Vector3.up * knockbackUpwardForce, ForceMode.Impulse);
                if (showDebugLogs) Debug.Log($"💥 {gameObject.name}: Knockback mạnh (no ragdoll controller)");
            }
        }
    }
    
    /// <summary>
    /// Kích hoạt phản ứng hit nhẹ và có thể counter attack
    /// </summary>
    protected virtual void ActivateHitReaction(Vector3 knockbackDirection, NPCBaseController attacker, float damage)
    {
        // Hit reaction nhẹ - chỉ làm giật mình và có thể counter
        if (animator != null)
        {
            // Kích hoạt animation hit nhẹ nếu có
            try 
            {
                bool hasHitParameter = false;
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.name == ANIM_HIT && param.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasHitParameter = true;
                        break;
                    }
                }
                
                if (hasHitParameter)
                {
                    animator.SetTrigger(ANIM_HIT);
                }
            }
            catch (System.Exception e)
            {
                if (showDebugLogs)
                    Debug.LogWarning($"⚠️ Lỗi khi trigger animation Hit: {e.Message}");
            }
        }
        
        // Knockback nhẹ để tạo cảm giác bị đẩy
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 lightKnockback = knockbackDirection * (knockbackForce * 0.2f);
            rb.AddForce(lightKnockback, ForceMode.Impulse);
        }
        
        // Cố gắng counter attack nếu có thể
        if (canCounterAttack && (Time.time - lastCounterAttackTime) >= counterAttackCooldown)
        {
            StartCoroutine(DelayedCounterAttack(attacker));
        }
        
        if (showDebugLogs) Debug.Log($"🥊 {gameObject.name}: Hit reaction nhẹ từ {attacker.gameObject.name} - damage: {damage}");
    }
    
    /// <summary>
    /// Counter attack sau một khoảng delay ngắn
    /// </summary>
    protected virtual System.Collections.IEnumerator DelayedCounterAttack(NPCBaseController attacker)
    {
        // Delay ngắn để tạo cảm giác tự nhiên
        yield return new WaitForSeconds(0.1f);
        
        if (attacker != null && !attacker.IsDead() && !isDead)
        {
            float distanceToAttacker = Vector3.Distance(transform.position, attacker.transform.position);
            
            // Chỉ counter nếu attacker vẫn trong tầm
            if (distanceToAttacker <= attackRange * 1.5f)
            {
                lastCounterAttackTime = Time.time;
                
                // Xoay về phía attacker
                RotateTowards(attacker.transform.position);
                
                // Thực hiện counter attack với damage cao hơn
                float originalDamage = attackDamage;
                attackDamage *= counterAttackDamageMultiplier;
                
                if (showDebugLogs) Debug.Log($"🔥 {gameObject.name}: COUNTER ATTACK → {attacker.gameObject.name} (damage: {attackDamage:F1})");
                
                Attack(attacker);
                
                // Khôi phục damage gốc
                attackDamage = originalDamage;
            }
        }
    }
}
