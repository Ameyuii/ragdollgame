using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Script quản lý nhân vật với các thuộc tính: máu, team, tốc độ di chuyển, tầm đánh, tốc độ đánh, phạm vi phát hiện
/// Nhân vật khác team có thể tấn công nhau
/// 
/// PHIÊN BẢN HOẠT ĐỘNG TỐT - KHÔI PHỤC TỪ BACKUP
/// </summary>
public class NPCController : MonoBehaviour
{
    [Header("Thuộc tính cơ bản")]
    [Tooltip("Máu tối đa của nhân vật")]
    public float maxHealth = 100f;
    
    [Tooltip("Máu hiện tại của nhân vật")]
    public float currentHealth;
    
    [Tooltip("ID của team (nhân vật khác team sẽ tấn công nhau). Đặt giá trị này trực tiếp trong Inspector.")]
    public int team = 0;
    
    [Header("Thiết lập di chuyển")]
    [Tooltip("Tốc độ di chuyển của nhân vật")]
    public float moveSpeed = 3.5f;
    
    [Tooltip("Tốc độ xoay của nhân vật (độ/giây)")]
    public float rotationSpeed = 120f;
    
    [Tooltip("Tốc độ tăng tốc")]
    public float acceleration = 8f;    [Header("Thiết lập tấn công")]
    [Tooltip("Sát thương mỗi đòn tấn công")]
    public float attackDamage = 20f;
    
    [Tooltip("Thời gian hồi chiêu (giây)")]
    public float attackCooldown = 1f;
    
    [Tooltip("Tầm đánh (m)")]
    public float attackRange = 2f;
    
    [Tooltip("Thời gian animation attack (giây) - điều chỉnh theo animation thực tế")]
    public float attackAnimationDuration = 1.0f;
    
    [Tooltip("Timing hit trong animation (0.0-1.0, ví dụ 0.65 = 65% animation)")]
    [Range(0.1f, 0.9f)]
    public float attackHitTiming = 0.65f;
    
    [Header("Thiết lập AI")]
    [Tooltip("Khoảng cách phát hiện kẻ địch (m)")]
    public float detectionRange = 30f;
    
    [Tooltip("Layer chứa kẻ địch")]
    public LayerMask enemyLayerMask;
    
    [Tooltip("Layer chứa chướng ngại vật")]
    public LayerMask obstacleLayerMask;
    
    [Header("Hiệu ứng")]
    [Tooltip("Hiệu ứng khi bị đánh")]
    public GameObject? hitEffect;
    
    [Tooltip("Hiệu ứng khi chết")]
    public GameObject? deathEffect;
    
    [Header("Debug Options")]
    [Tooltip("Hiển thị thông tin debug chi tiết")]
    public bool showDebugLogs = true; // Bật debug mặc định để theo dõi vấn đề    // Biến theo dõi trạng thái
    private float lastAttackTime;
    private bool isDead = false;
    private NPCController? targetEnemy;
    private NPCController? currentAttackTarget; // Target hiện tại đang bị tấn công
    private bool isMoving = false;
    
    // Biến để xử lý smooth transition và tránh trượt
    private bool isTransitioning = false;
    private float transitionStartTime = 0f;
    private float transitionDuration = 0.3f; // Thời gian chờ để animation ổn định
    private Vector3 lastFramePosition;
    private bool hasStartedMoving = false;
    
    // Biến điều khiển
    private bool isInitialized = false; // Đã khởi tạo xong chưa
    
    // Tham chiếu các thành phần
    private Animator? animator;
    private NavMeshAgent? navMeshAgent;    // Tên các tham số animator
    private static readonly string ANIM_IS_WALKING = "IsWalking";
    private static readonly string ANIM_ATTACK = "Attack";      // trigger chính
    private static readonly string ANIM_ATTACK1 = "Attack1";    // trigger attack1
    private static readonly string ANIM_ATTACK2 = "Attack2";    // trigger attack2
    private static readonly string ANIM_HIT = "Hit";
    private static readonly string ANIM_DIE = "Die";
    
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
    
    [Tooltip("Cooldown cho attack1 (nếu khác với attack thông thường)")]
    public float attack1Cooldown = 1.2f;
    
    [Tooltip("Cooldown cho attack2 (nếu khác với attack thông thường)")]
    public float attack2Cooldown = 1.5f;

    // Biến theo dõi loại attack cuối cùng
    private string lastUsedAttackTrigger = "";

    // Khởi tạo các thành phần
    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    // Thiết lập ban đầu
    void Start()
    {
        // Khởi tạo máu
        currentHealth = maxHealth;
        
        // Thiết lập NavMeshAgent nếu có
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.stoppingDistance = 1.0f;
            
            // ✅ QUAN TRỌNG: Thiết lập bình thường để NavMeshAgent hoạt động đúng
            navMeshAgent.updatePosition = true; // BẬT để NPCs có thể di chuyển
            navMeshAgent.updateRotation = true; // BẬT để NPCs có thể xoay
            
            // QUAN TRỌNG: Thiết lập để giảm trượt
            navMeshAgent.autoBraking = true; // Tự động giảm tốc khi gần đích
            navMeshAgent.autoRepath = true; // Tự động tìm đường mới khi cần
            
            // ✅ QUAN TRỌNG: Đảm bảo NavMeshAgent luôn được kích hoạt
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false; // Cho phép di chuyển
            
            // ✅ Cài đặt để tránh NPCs đẩy nhau quá mạnh
            navMeshAgent.radius = 0.4f; // Giảm radius để tránh va chạm
            navMeshAgent.height = 1.8f; // Chiều cao phù hợp
            navMeshAgent.stoppingDistance = 1.5f; // Dừng lại cách mục tiêu 1.5m
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Tránh va chạm
            
            if (showDebugLogs) Debug.Log($"✅ {gameObject.name}: NavMeshAgent đã được kích hoạt và cấu hình");
        }
        else
        {
            Debug.LogError($"❌ {gameObject.name}: KHÔNG CÓ NavMeshAgent! NPC sẽ không thể di chuyển!");
        }
        
        // Thiết lập layer mask nếu chưa được thiết lập
        if (enemyLayerMask.value == 0)
        {
            // Sử dụng tất cả layers để đảm bảo phát hiện được NPCs
            enemyLayerMask = -1; // All layers
            if (showDebugLogs) Debug.Log($"⚠️ {gameObject.name}: Auto-set enemyLayerMask to ALL LAYERS để phát hiện NPC");
        }
        else if (showDebugLogs)
        {
            Debug.Log($"✅ {gameObject.name}: enemyLayerMask đã được set: {enemyLayerMask.value}");
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
    
    // Cập nhật mỗi khung hình
    void Update()
    {
        if (isDead || !isInitialized) return;        
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
    
    // Xử lý di chuyển mượt mà với transition
    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            // Nếu đang trong transition, chờ hoàn thành
            if (isTransitioning)
            {
                if (Time.time - transitionStartTime >= transitionDuration)
                {
                    // Hoàn thành transition, bắt đầu di chuyển thực sự
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
                navMeshAgent.SetDestination(targetPosition); // Đặt destination nhưng dừng lại
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
    void StartMovementTransition()
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
    IEnumerator FindEnemyRoutine()
    {
        while (!isDead)
        {
            FindClosestEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Tìm kẻ địch gần nhất
    void FindClosestEnemy()
    {
        // Kiểm tra nếu chưa khởi tạo xong thì không tìm kẻ địch
        if (!isInitialized)
        {
            return;
        }
    
        // Tìm tất cả các collider trong phạm vi
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayerMask);
        
        if (showDebugLogs) Debug.Log($"🔍 {gameObject.name}: Tìm thấy {hitColliders.Length} objects trong phạm vi {detectionRange}m");
        
        float shortestDistance = float.MaxValue;
        NPCController? nearestEnemy = null;
        
        foreach (var hitCollider in hitColliders)
        {
            // Không nhắm vào chính mình
            if (hitCollider.gameObject == gameObject) continue;
            
            // Kiểm tra xem đối tượng có phải NPCController không
            NPCController otherCharacter = hitCollider.GetComponent<NPCController>();
            if (otherCharacter != null && !otherCharacter.IsDead())
            {
                // Kiểm tra team - chỉ tấn công team khác
                if (otherCharacter.team != team)
                {
                    float distance = Vector3.Distance(transform.position, otherCharacter.transform.position);
                    
                    // Kiểm tra line-of-sight
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
            hasStartedMoving = false; // Reset movement state cho mục tiêu mới
            
            if (showDebugLogs)
                Debug.Log($"🎯 {gameObject.name}: Phát hiện kẻ địch mới {targetEnemy.gameObject.name}");
        }
    }
    
    // Kiểm tra xem có đường nhìn đến mục tiêu không
    bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;
        
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        
        // Tạo điểm bắt đầu cao hơn một chút để tránh va chạm với mặt đất
        Vector3 startPoint = transform.position + Vector3.up * 1f;
        Vector3 targetPoint = target.position + Vector3.up * 1f;
        
        // Kiểm tra xem có chướng ngại vật không
        if (Physics.Raycast(startPoint, targetPoint - startPoint, out RaycastHit hit, distanceToTarget, obstacleLayerMask))
        {
            // Có chướng ngại vật chặn tầm nhìn
            return false;
        }
        
        return true;
    }
      // Kiểm tra xem có thể tấn công không (dựa vào cooldown)
    public bool CanAttack()
    {
        float currentCooldown = attackCooldown;
        
        // Nếu sử dụng cooldown khác nhau cho từng attack
        if (useVariableAttackCooldown)
        {
            switch (lastUsedAttackTrigger)
            {
                case var trigger when trigger == ANIM_ATTACK1:
                    currentCooldown = attack1Cooldown;
                    break;
                case var trigger when trigger == ANIM_ATTACK2:
                    currentCooldown = attack2Cooldown;
                    break;                default:
                    currentCooldown = attackCooldown; // Basic attack
                    break;
            }
        }
        
        return Time.time >= lastAttackTime + currentCooldown;
    }
    
    // Bắt đầu tấn công đơn giản (đã bỏ combo system)
    public void Attack(NPCController target)
    {
        if (isDead || !CanAttack() || target == null) return;
        
        // Cập nhật thời gian tấn công
        lastAttackTime = Time.time;
        
        // Lưu target để sử dụng khi animation hit
        currentAttackTarget = target;
        
        // Kích hoạt animation tấn công với random attack
        if (animator != null)
        {
            string attackTrigger = GetRandomAttackTrigger();
            animator.SetTrigger(attackTrigger);
            lastUsedAttackTrigger = attackTrigger; // Lưu lại loại attack đã sử dụng
            
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
    private System.Collections.IEnumerator DelayedAttackHit()
    {
        // Chờ đến timing hit được cấu hình
        yield return new WaitForSeconds(attackAnimationDuration * attackHitTiming);
        
        // Gây damage tại thời điểm hit
        OnAttackHit();
    }
    
    // Method này sẽ được gọi từ Animation Event tại hit frame
    public void OnAttackHit()
    {
        DealDamageToTarget();
    }
    
    // Gây sát thương thực sự cho target hiện tại
    private void DealDamageToTarget()
    {
        if (currentAttackTarget == null || currentAttackTarget.IsDead()) 
        {
            if (showDebugLogs) Debug.Log($"❌ {gameObject.name}: Không có target hợp lệ để gây damage");
            return;
        }
        
        // Kiểm tra target vẫn trong tầm đánh
        float distanceToTarget = Vector3.Distance(transform.position, currentAttackTarget.transform.position);
        if (distanceToTarget > attackRange * 1.2f) // Cho phép một chút sai số
        {
            if (showDebugLogs) Debug.Log($"❌ {gameObject.name}: Target {currentAttackTarget.gameObject.name} đã ra khỏi tầm đánh");
            return;
        }
        
        // Gây sát thương
        currentAttackTarget.TakeDamage(attackDamage, this);
        
        // Thêm impact vật lý
        AddPhysicsImpact(currentAttackTarget);
        
        // Debug log
        if (showDebugLogs) Debug.Log($"⚔️ {gameObject.name} (Team {team}) gây {attackDamage} sát thương cho {currentAttackTarget.gameObject.name} (Team {currentAttackTarget.team})!");
        
        // Reset target sau khi attack
        currentAttackTarget = null;
    }
    
    // Thêm tác động vật lý khi tấn công
    void AddPhysicsImpact(NPCController target)
    {
        if (target == null) return;
        
        // Tính toán hướng tác động
        Vector3 impactDirection = (target.transform.position - transform.position).normalized;
        float impactForce = 25f; // Lực vừa phải
        
        // Kiểm tra khoảng cách - chỉ đẩy nếu gần
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > 2.5f) return; // Không đẩy nếu quá xa
        
        // Áp dụng physics force nhẹ
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null && !target.isDead)
        {
            Vector3 forceDirection = impactDirection + Vector3.up * 0.1f; // Ít lực lên trên
            targetRb.AddForce(forceDirection * impactForce, ForceMode.Impulse);
            
            if (showDebugLogs) Debug.Log($"💥 {gameObject.name}: Đẩy nhẹ {target.gameObject.name} với lực {impactForce}");
        }
        
        // Kích hoạt ragdoll khi máu thấp hoặc chết
        if (target.currentHealth <= target.maxHealth * 0.3f || target.isDead) // 30% máu hoặc chết
        {
            TriggerRagdollOnTarget(target, impactDirection, impactForce);
        }
    }
    
    // Kích hoạt ragdoll trên target
    void TriggerRagdollOnTarget(NPCController target, Vector3 impactDirection, float impactForce)
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
    
    // Kích hoạt ragdoll cho bản thân
    void TriggerRagdoll()
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
            Vector3 attackDirection = Vector3.forward; // Hướng tấn công mặc định
            ragdollManager.TanCongNPC(gameObject, attackDirection);
            if (showDebugLogs) Debug.Log($"💀 {gameObject.name}: Kích hoạt Ragdoll qua Manager!");
        }
        else
        {
            if (showDebugLogs) Debug.LogWarning($"⚠️ {gameObject.name}: Không tìm thấy Ragdoll system!");
        }
    }
    
    // Nhận sát thương
    public void TakeDamage(float damage, NPCController attacker)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Thêm knockback effect khi bị tấn công
        if (attacker != null)
        {
            Vector3 knockbackDirection = (transform.position - attacker.transform.position).normalized;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(knockbackDirection * 150f + Vector3.up * 50f, ForceMode.Impulse);
            }
        }
        
        // Phát hiệu ứng bị đánh nếu có
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        // Kích hoạt animation bị đánh nếu có
        if (animator != null)
        {
            animator.SetTrigger(ANIM_HIT);
        }
        
        if (showDebugLogs) Debug.Log($"💔 {gameObject.name} nhận {damage} sát thương từ {(attacker ? attacker.gameObject.name : "Unknown")}. Máu còn: {currentHealth:F1}/{maxHealth}");
        
        // Kiểm tra nếu đã chết
        if (currentHealth <= 0)
        {
            Die();
        }
        // Kích hoạt ragdoll khi máu thấp (dưới 30%)
        else if (currentHealth <= maxHealth * 0.3f)
        {
            TriggerRagdoll();
        }
    }
    
    // Xử lý khi nhân vật chết
    void Die()
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
    IEnumerator DisableNavMeshAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Vô hiệu hóa NavMeshAgent sau khi ragdoll đã được kích hoạt
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
    }
    
    // Kiểm tra xem có phải đồng minh không (cùng team)
    public bool IsAlly(NPCController other)
    {
        return other != null && team == other.team;
    }
    
    // Kiểm tra xem đã chết chưa
    public bool IsDead()
    {
        return isDead;
    }
    
    // Cập nhật trạng thái animation
    void UpdateAnimationState(bool moving)
    {
        if (animator != null)
        {
            // Tránh cập nhật animation nếu không cần thiết
            if (isMoving != moving)
            {
                // Cập nhật trạng thái di chuyển
                isMoving = moving;
                
                if (moving)
                {
                    // Chuyển sang animation đi bộ
                    animator.SetBool(ANIM_IS_WALKING, true);
                    if (showDebugLogs) 
                        Debug.Log($"{gameObject.name}: 🚶 Chuyển sang animation đi bộ");
                }
                else
                {
                    // Chuyển về animation đứng yên
                    animator.SetBool(ANIM_IS_WALKING, false);
                    if (showDebugLogs) 
                        Debug.Log($"{gameObject.name}: 🧍 Chuyển về animation đứng yên");
                }
            }
            
            // Xử lý trường hợp animator không có tham số
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
    void StopMoving()
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
    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Giữ nhân vật không bị nghiêng lên xuống
        
        if (direction != Vector3.zero)
        {
            // Lưu trạng thái di chuyển trước khi quay
            bool wasMoving = isMoving;
            
            // Tính góc quay hiện tại so với hướng cần quay
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
            
            // Kiểm tra xem cần quay nhiều không để quyết định kích hoạt animation
            bool needRotation = angleToTarget > 20f;
            
            if (needRotation)
            {
                // Nếu góc quay lớn, kích hoạt animation đi bộ trước khi xoay
                UpdateAnimationState(true);
            }
            
            // Thực hiện quay
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Kiểm tra xem quay đã gần hoàn thành chưa
            angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
            
            // Nếu đã quay xong và trước đó không di chuyển, tắt animation
            if (angleToTarget < 5f && !wasMoving && needRotation)
            {
                // Trả về animation idle sau khi hoàn thành quay
                UpdateAnimationState(false);
                
                if (showDebugLogs) Debug.Log($"{gameObject.name}: Đã hoàn thành quay, chuyển về trạng thái đứng yên");
            }
        }
    }
    
    // Tìm kiếm và di chuyển đến vị trí ngẫu nhiên trong khi không có mục tiêu
    IEnumerator PatrolWhenIdle()
    {
        yield return new WaitForSeconds(2f); // Chờ 2 giây để system khởi tạo
        
        while (!isDead)
        {
            // Nếu không có mục tiêu và đã khởi tạo xong, tự động di chuyển
            if (targetEnemy == null && isInitialized && navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                // Tạo điểm đến ngẫu nhiên trong phạm vi detection
                Vector3 randomDirection = Random.insideUnitSphere * (detectionRange * 0.5f);
                randomDirection += transform.position;
                randomDirection.y = transform.position.y; // Giữ cùng độ cao
                
                // Kiểm tra xem điểm đến có hợp lệ không
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, detectionRange, NavMesh.AllAreas))
                {
                    // Reset movement state cho patrol
                    hasStartedMoving = false;
                    MoveToTarget(hit.position);
                    
                    if (showDebugLogs) Debug.Log($"🚶 {gameObject.name}: Không có mục tiêu, patrol đến {hit.position}");
                }
                
                // Chờ một khoảng thời gian trước khi patrol tiếp
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
            else
            {
                // Nếu có mục tiêu hoặc chưa khởi tạo, chờ ngắn hơn
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    // Hiển thị phạm vi phát hiện và tầm đánh trong Scene View
    void OnDrawGizmosSelected()
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
    
    // Xử lý animation event từ animation đi bộ
    void OnFootstep()
    {
        // Xử lý sự kiện khi nhân vật bước đi
        if (showDebugLogs) Debug.Log($"{gameObject.name}: Bước chân");
    }
      // Random chọn attack trigger theo tỷ lệ được cấu hình
    private string GetRandomAttackTrigger()
    {
        // Tạo random số từ 0 đến 100
        float randomValue = Random.Range(0f, 100f);
        string selectedTrigger;
        
        // Chọn attack dựa trên tỷ lệ
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
    private void ValidateAttackChances()
    {
        float totalChance = basicAttackChance + attack1Chance + attack2Chance;
        
        if (Mathf.Abs(totalChance - 100f) > 0.1f)
        {
            if (showDebugLogs) 
                Debug.LogWarning($"⚠️ {gameObject.name}: Tổng tỷ lệ attack không bằng 100% ({totalChance:F1}%). Đang normalize...");
            
            // Normalize về 100%
            if (totalChance > 0)
            {
                basicAttackChance = (basicAttackChance / totalChance) * 100f;
                attack1Chance = (attack1Chance / totalChance) * 100f;
                attack2Chance = (attack2Chance / totalChance) * 100f;
            }
            else
            {
                // Fallback nếu tất cả đều = 0
                basicAttackChance = 40f;
                attack1Chance = 30f;
                attack2Chance = 30f;
            }
        }
        
        if (showDebugLogs)
            Debug.Log($"✅ {gameObject.name}: Attack chances - Basic: {basicAttackChance:F1}%, Attack1: {attack1Chance:F1}%, Attack2: {attack2Chance:F1}%");
    }

    // Method để debug thông tin attack system (có thể gọi từ Inspector hoặc console)
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
}
