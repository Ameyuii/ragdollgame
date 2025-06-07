using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Controller xử lý combat logic cho AI character
/// Kết hợp với EnemyDetector để tìm và tấn công kẻ địch
/// Tích hợp với AIMovementController và NavMeshAgent
/// </summary>
public class CombatController : MonoBehaviour
{    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.5f; // 🔧 GIẢM COOLDOWN từ 1s xuống 0.5s để test nhanh hơn
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Combat Behavior")]
    [SerializeField] private CombatBehaviorType behaviorType = CombatBehaviorType.Aggressive;
    [SerializeField] private float engageDistance = 8f;
    [SerializeField] private float disengageDistance = 12f;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Components")]
    [SerializeField] private TeamMember teamMember;
    [SerializeField] private EnemyDetector enemyDetector;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private NavMeshAgent navAgent;
    
    [Header("Combat Grace Period")]
    [SerializeField] private float targetLossGracePeriod = 0.5f; // 0.5s grace period
    private float lastTargetLossTime = -1f;
    
    // Private variables
    private CombatState currentState = CombatState.Idle;
    private TeamMember? currentTarget;
    private float lastAttackTime;
    private Vector3 moveDirection;
    private bool isAttacking = false;
    private bool useNavMeshMovement = false;
    
    // 🎲 Random Animation Attack System variables
    private string[] attackTriggers = {"Attack", "Attack1", "Attack2"};
    private string lastAttackUsed = "";
    private int consecutiveAttackCount = 0;
    private const int maxConsecutiveAttacks = 2;
    private System.Random attackRandom;
    
    // Events
    public System.Action<TeamMember> OnAttackStarted;
    public System.Action<TeamMember, float> OnDamageDealt;
    public System.Action<CombatState> OnStateChanged;
    
    // Properties
    public CombatState CurrentState => currentState;
    public TeamMember? CurrentTarget => currentTarget;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;
    public bool IsInCombat => currentState != CombatState.Idle;
      private void Awake()
    {
        // Auto-find components
        if (teamMember == null)
            teamMember = GetComponent<TeamMember>();
        if (enemyDetector == null)
            enemyDetector = GetComponent<EnemyDetector>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
            
        // Determine movement type
        useNavMeshMovement = navAgent != null;
        
        // 🎲 Initialize Random Animation Attack System
        attackRandom = new System.Random(GetInstanceID());
        Debug.Log($"🎯 [RANDOM ATTACK] {gameObject.name} Random Attack System initialized với seed: {GetInstanceID()}");
          // 🔥 CRITICAL DEBUG: Log NavMeshAgent initial state - TẮT LOG
        if (navAgent != null)
        {
            // Debug.Log($"🤖 [COMBAT INIT] {gameObject.name} NavMeshAgent FOUND:"); // TẮT
            // Debug.Log($"    - enabled: {navAgent.enabled}"); // TẮT
            // Debug.Log($"    - speed: {navAgent.speed:F2}"); // TẮT
            // Debug.Log($"    - stoppingDistance: {navAgent.stoppingDistance:F2}"); // TẮT
            // Debug.Log($"    - isStopped: {navAgent.isStopped}"); // TẮT
            // Debug.Log($"    - useNavMeshMovement: {useNavMeshMovement}"); // TẮT
            
            // 🔥 CRITICAL FIX: Ensure proper NavMeshAgent settings for combat
            ValidateNavMeshAgentSettings();
        }
        else
        {
            Debug.LogWarning($"❌ [COMBAT INIT] {gameObject.name} NavMeshAgent NOT FOUND!");
        }
    }
      /// <summary>
    /// Validate và fix NavMeshAgent settings để tránh các vấn đề movement
    /// </summary>
    private void ValidateNavMeshAgentSettings()
    {
        if (navAgent == null) return;
        
        bool hasChanges = false;
        
        // Đảm bảo speed > 0
        if (navAgent.speed <= 0)
        {
            navAgent.speed = moveSpeed;
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Fixed speed: {navAgent.speed}"); // TẮT
        }
        
        // Đảm bảo stoppingDistance reasonable
        if (navAgent.stoppingDistance <= 0 || navAgent.stoppingDistance > attackRange)
        {
            navAgent.stoppingDistance = Mathf.Max(attackRange * 0.5f, 0.5f);
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Fixed stoppingDistance: {navAgent.stoppingDistance:F2}"); // TẮT
        }
        
        // Đảm bảo acceleration reasonable
        if (navAgent.acceleration <= 0)
        {
            navAgent.acceleration = 8f;
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Fixed acceleration: {navAgent.acceleration}"); // TẮT
        }
        
        // Đảm bảo angularSpeed reasonable
        if (navAgent.angularSpeed <= 0)
        {
            navAgent.angularSpeed = 120f;
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Fixed angularSpeed: {navAgent.angularSpeed}"); // TẮT
        }
        
        // 🔥 ANTI-COLLISION SETTINGS: Ensure proper avoidance
        if (navAgent.radius <= 0)
        {
            navAgent.radius = 0.5f;
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Fixed radius: {navAgent.radius}"); // TẮT
        }
        
        // Set avoidance priority để tránh conflict
        if (navAgent.avoidancePriority == 50) // Default value
        {
            navAgent.avoidancePriority = UnityEngine.Random.Range(40, 60);
            hasChanges = true;
            // Debug.Log($"🔧 [NAVMESH VALIDATE] {gameObject.name} Set avoidancePriority: {navAgent.avoidancePriority}"); // TẮT
        }
        
        if (hasChanges)
        {
            // Debug.Log($"✅ [NAVMESH VALIDATE] {gameObject.name} NavMeshAgent settings validated and fixed"); // TẮT
        }
    }
    
    private void Start()
    {
        // Validate components
        if (teamMember == null)
        {
            Debug.LogError($"CombatController on {gameObject.name} requires TeamMember component!");
            enabled = false;
            return;
        }
        
        if (enemyDetector == null)
        {
            Debug.LogError($"CombatController on {gameObject.name} requires EnemyDetector component!");
            enabled = false;
            return;
        }
          // Subscribe to events
        enemyDetector.OnTargetChanged += OnTargetChanged;
        teamMember.OnDeath += OnDeath;
        
        // 🔧 RESET ATTACK COOLDOWN để cho phép attack ngay lập tức
        lastAttackTime = Time.time - attackCooldown - 1f; // Reset về quá khứ để có thể attack ngay        // if (debugMode)
            // Debug.Log($"CombatController initialized for {gameObject.name} - Ready to attack!"); // TẮT
    }
    
    private void Update()
    {
        if (!teamMember.IsAlive) return;
          // 🔥 DEBUG: Log Update() execution để verify method được gọi - TẮT
        // if (debugMode && currentState != CombatState.Idle)
        // {
        //     Debug.Log($"🔄 [COMBAT UPDATE] {gameObject.name} Update() executing - State: {currentState}, Target: {(currentTarget != null ? currentTarget.name : "null")}");
        // }
        
        UpdateCombatState();
        HandleCombatBehavior();
        HandleMovement();
        HandleRotation();
    }
    
    /// <summary>
    /// Update combat state machine - FIXED LOGIC
    /// </summary>
    private void UpdateCombatState()
    {
        CombatState newState = currentState;
        float distanceToTarget = GetDistanceToTarget();
        
        // Debug.Log($"🔄 [COMBAT] {gameObject.name} UpdateCombatState - Current: {currentState}, HasEnemies: {enemyDetector.HasEnemies}, Distance: {distanceToTarget:F2}m"); // TẮT
        
        switch (currentState)
        {
            case CombatState.Idle:
                if (enemyDetector.HasEnemies && currentTarget != null)
                {
                    newState = CombatState.Engaging;
                    // Debug.Log($"🎯 [COMBAT] {gameObject.name} Idle -> Engaging với {currentTarget.name}"); // TẮT
                }
                break;
                  case CombatState.Engaging:
                // 🔥 CRITICAL FIX: Kiểm tra target loss với grace period để tránh mất target đột ngột
                bool hasValidTarget = enemyDetector.HasEnemies && currentTarget != null && currentTarget.IsAlive;
                
                if (!hasValidTarget)
                {
                    // Mark thời điểm mất target
                    if (lastTargetLossTime < 0)
                    {
                        lastTargetLossTime = Time.time;
                        // Debug.Log($"⚠️ [COMBAT] {gameObject.name} Mất target - bắt đầu grace period {targetLossGracePeriod}s"); // TẮT
                    }
                    
                    // Chỉ quay về Idle sau grace period
                    if (Time.time - lastTargetLossTime > targetLossGracePeriod)
                    {
                        newState = CombatState.Idle;
                        lastTargetLossTime = -1f;
                        // Debug.Log($"❌ [COMBAT] {gameObject.name} Engaging -> Idle (grace period expired)"); // TẮT
                    }
                }
                else
                {
                    // Reset grace period khi có target trở lại
                    lastTargetLossTime = -1f;
                    
                    if (distanceToTarget <= attackRange)
                    {
                        newState = CombatState.InCombat;
                        // Debug.Log($"⚔️ [COMBAT] {gameObject.name} Engaging -> InCombat (distance: {distanceToTarget:F2}m <= {attackRange:F2}m)"); // TẮT
                    }
                }
                break;
                  case CombatState.InCombat:
                // 🔥 CRITICAL FIX: Grace period trong InCombat cũng để tránh mất target đột ngột
                bool hasValidTargetInCombat = enemyDetector.HasEnemies && currentTarget != null && currentTarget.IsAlive;
                
                if (!hasValidTargetInCombat)
                {
                    if (lastTargetLossTime < 0)
                    {
                        lastTargetLossTime = Time.time;
                        // Debug.Log($"⚠️ [COMBAT] {gameObject.name} InCombat mất target - grace period {targetLossGracePeriod}s"); // TẮT
                    }
                    
                    if (Time.time - lastTargetLossTime > targetLossGracePeriod)
                    {
                        newState = CombatState.Idle;
                        lastTargetLossTime = -1f;
                        // Debug.Log($"❌ [COMBAT] {gameObject.name} InCombat -> Idle (grace period expired)"); // TẮT
                    }
                }
                else
                {
                    lastTargetLossTime = -1f;
                    
                    if (distanceToTarget > disengageDistance)
                    {
                        newState = CombatState.Engaging;
                        // Debug.Log($"🏃 [COMBAT] {gameObject.name} InCombat -> Engaging (distance: {distanceToTarget:F2}m > {disengageDistance:F2}m)"); // TẮT
                    }
                }
                break;
        }
        
        if (newState != currentState)
        {
            // Debug.Log($"🔄 [COMBAT] {gameObject.name} STATE CHANGE: {currentState} -> {newState}"); // TẮT
            ChangeState(newState);
        }
    }
    
    /// <summary>
    /// Change combat state - ENHANCED DEBUG
    /// </summary>
    private void ChangeState(CombatState newState)
    {
        CombatState previousState = currentState;
        currentState = newState;
        
        // Always log state changes
        // Debug.Log($"🔄 [COMBAT] {gameObject.name} combat state: {previousState} -> {currentState}"); // TẮT
        
        // Invoke state changed event
        OnStateChanged?.Invoke(currentState);
        // Debug.Log($"📢 [COMBAT] {gameObject.name} OnStateChanged event fired: {currentState}"); // TẮT
        
        // State entry actions
        switch (currentState)
        {
            case CombatState.Idle:
                SetAnimationState("idle");
                // Debug.Log($"😴 [COMBAT] {gameObject.name} entered IDLE state"); // TẮT
                break;
            case CombatState.Engaging:
                SetAnimationState("walking");
                // Debug.Log($"🏃 [COMBAT] {gameObject.name} entered ENGAGING state với target {currentTarget?.name}"); // TẮT
                
                // ✅ CRITICAL FIX: Ensure NavMeshAgent ready cho movement trong Engaging state
                if (useNavMeshMovement && navAgent != null)
                {
                    if (!navAgent.enabled)
                    {
                        navAgent.enabled = true;
                        // Debug.Log($"🔧 [COMBAT] {gameObject.name} ENABLED NavMeshAgent for Engaging state"); // TẮT
                    }
                    
                    if (navAgent.isStopped)
                    {
                        navAgent.isStopped = false;
                        // Debug.Log($"🔧 [COMBAT] {gameObject.name} UNSTOP NavMeshAgent for Engaging state"); // TẮT
                    }
                    
                    if (navAgent.speed <= 0)
                    {
                        navAgent.speed = moveSpeed;
                        // Debug.Log($"🔧 [COMBAT] {gameObject.name} SET NavMeshAgent speed = {moveSpeed} for Engaging state"); // TẮT
                    }
                    
                    // Debug.Log($"✅ [COMBAT] {gameObject.name} NavMeshAgent ready for Engaging movement - enabled: {navAgent.enabled}, isStopped: {navAgent.isStopped}, speed: {navAgent.speed:F2}"); // TẮT
                }
                break;
            case CombatState.InCombat:
                SetAnimationState("combat");
                // Debug.Log($"⚔️ [COMBAT] {gameObject.name} entered IN_COMBAT state với target {currentTarget?.name}"); // TẮT
                break;
        }
        
        // Verify state change
        if (currentState == newState)
        {
            // Debug.Log($"✅ [COMBAT] {gameObject.name} state change SUCCESSFUL: {newState}"); // TẮT
        }
        else
        {
            Debug.LogError($"❌ [COMBAT] {gameObject.name} state change FAILED! Expected: {newState}, Actual: {currentState}");
        }
    }
    
    /// <summary>
    /// Handle combat behavior based on current state
    /// </summary>
    private void HandleCombatBehavior()
    {
        switch (currentState)
        {
            case CombatState.Idle:
                // Do nothing, just idle
                moveDirection = Vector3.zero;
                break;
                
            case CombatState.Engaging:
                HandleEngaging();
                break;
                
            case CombatState.InCombat:
                HandleInCombat();
                break;
        }
    }
      /// <summary>
    /// Handle engaging behavior - moving toward enemy với NavMeshAgent integration
    /// FIX: Thêm logic anti-loop và stoppingDistance
    /// </summary>
    private void HandleEngaging()
    {
        // Debug.Log($"🏃 [COMBAT ENGAGING] {gameObject.name} HandleEngaging() START"); // TẮT
        
        if (currentTarget == null)
        {
            // Debug.Log($"❌ [COMBAT ENGAGING] {gameObject.name} currentTarget is null, tìm target mới"); // TẮT
            // Tìm target mới từ EnemyDetector
            if (enemyDetector.HasEnemies)
            {
                currentTarget = enemyDetector.GetClosestEnemy();
                // Debug.Log($"✅ [COMBAT ENGAGING] {gameObject.name} tìm được target mới: {(currentTarget != null ? currentTarget.name : "null")}"); // TẮT
            }
            return;
        }
        
        float distanceToTarget = GetDistanceToTarget();        // Debug.Log($"📏 [COMBAT ENGAGING] {gameObject.name} Distance to {currentTarget.name}: {distanceToTarget:F2}m, AttackRange: {attackRange:F2}m"); // TẮT
        // Debug.Log($"🔧 [COMBAT ENGAGING] {gameObject.name} CanMove: {canMove}, UseNavMesh: {useNavMeshMovement}"); // TẮT// 🔥 CRITICAL FIX: Điều chỉnh stopping distance để NPC đến RẤT GẦN nhau cho combat
        float effectiveStoppingDistance = 0.15f;
        
        if (canMove && distanceToTarget > effectiveStoppingDistance)
        {
            // Debug.Log($"🎯 [COMBAT ENGAGING] {gameObject.name} CẦN DI CHUYỂN đến target - Distance: {distanceToTarget:F2}m > StopDistance: {effectiveStoppingDistance:F2}m"); // TẮT
            
            // Move toward target - sử dụng NavMeshAgent nếu có
            if (useNavMeshMovement && navAgent != null && navAgent.enabled)
            {                // Debug.Log($"🤖 [COMBAT ENGAGING] {gameObject.name} SỬ DỤNG NAVMESH MOVEMENT"); // TẮT
                // Debug.Log($"    - NavAgent.enabled: {navAgent.enabled}"); // TẮT
                // Debug.Log($"    - NavAgent.isStopped: {navAgent.isStopped}"); // TẮT
                // Debug.Log($"    - NavAgent.speed: {navAgent.speed:F2}"); // TẮT
                // Debug.Log($"    - Target position: {currentTarget.transform.position}"); // TẮT
                
                // ✅ CRITICAL FIX: Ensure NavMeshAgent can move
                if (navAgent.isStopped)
                {
                    navAgent.isStopped = false;
                    // Debug.Log($"🔧 [COMBAT ENGAGING] {gameObject.name} FIXED NavAgent.isStopped = false"); // TẮT
                }
                
                if (navAgent.speed <= 0)
                {
                    navAgent.speed = moveSpeed;
                    // Debug.Log($"🔧 [COMBAT ENGAGING] {gameObject.name} FIXED NavAgent.speed = {moveSpeed}"); // TẮT
                }
                
                // 🔥 CRITICAL FIX: Set proper stopping distance trên NavMeshAgent
                navAgent.stoppingDistance = effectiveStoppingDistance;
                // Debug.Log($"🔧 [COMBAT ENGAGING] {gameObject.name} SET NavAgent.stoppingDistance = {effectiveStoppingDistance:F2}"); // TẮT
                  // 🔥 ANTI-COLLISION FIX: Chỉ offset nếu target cũng đang target mình để tránh cả 2 cùng offset
                Vector3 targetPosition = currentTarget.transform.position;
                Vector3 myPosition = transform.position;
                Vector3 directionToTarget = (targetPosition - myPosition).normalized;
                
                Vector3 finalTargetPos;
                
                // Kiểm tra nếu target cũng đang target mình thì chỉ một bên offset
                CombatController targetCombat = currentTarget.GetComponent<CombatController>();
                bool targetIsTargetingMe = targetCombat != null && targetCombat.currentTarget == teamMember;                if (targetIsTargetingMe && gameObject.name.CompareTo(currentTarget.name) < 0)
                {
                    // NPC với tên "nhỏ hơn" sẽ offset cực ít (chỉ 0.05m), NPC kia đi thẳng
                    finalTargetPos = targetPosition - directionToTarget * 0.05f;
                    // Debug.Log($"🔄 [COMBAT ENGAGING] {gameObject.name} SỬ DỤNG MINIMAL OFFSET position (0.05m)"); // TẮT
                }
                else
                {
                    // Đi thẳng đến target position với stopping distance của NavAgent
                    finalTargetPos = targetPosition;
                    // Debug.Log($"🎯 [COMBAT ENGAGING] {gameObject.name} ĐI THẲNG đến target position"); // TẮT
                }
                  navAgent.SetDestination(finalTargetPos);
                // Debug.Log($"✅ [COMBAT ENGAGING] {gameObject.name} NavAgent.SetDestination() CALLED với offset position {finalTargetPos}"); // TẮT
                
                // 🔥 CRITICAL FIX: Set moveDirection để HandleNavMeshMovement nhận diện movement
                moveDirection = directionToTarget;
            }
            else
            {
                // Debug.Log($"🚶 [COMBAT ENGAGING] {gameObject.name} SỬ DỤNG DIRECT MOVEMENT (fallback)"); // TẮT
                // Fallback to direct movement
                Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
                moveDirection = directionToTarget;
                // Debug.Log($"    - moveDirection: {moveDirection}"); // TẮT
            }
        }
        else
        {
            // Debug.Log($"🛑 [COMBAT ENGAGING] {gameObject.name} KHÔNG CẦN DI CHUYỂN - đã đủ gần target (Distance: {distanceToTarget:F2}m <= StopDistance: {effectiveStoppingDistance:F2}m)"); // TẮT
            moveDirection = Vector3.zero;
            // Dừng NavMesh movement khi đã gần target
            if (useNavMeshMovement && navAgent != null && navAgent.hasPath)
            {
                navAgent.isStopped = true;
                // Debug.Log($"🛑 [COMBAT ENGAGING] {gameObject.name} NavAgent.isStopped = true - dừng movement"); // TẮT
            }
        }
        
        // Debug.Log($"🏁 [COMBAT ENGAGING] {gameObject.name} HandleEngaging() END"); // TẮT
    }
    
    /// <summary>
    /// Handle in combat behavior - attack and position
    /// </summary>
    private void HandleInCombat()
    {
        if (currentTarget == null)
        {
            Debug.Log($"❌ [COMBAT] {gameObject.name} HandleInCombat - currentTarget is null!");
            return;
        }
        
        float distanceToTarget = GetDistanceToTarget();
        // Debug.Log($"⚔️ [COMBAT] {gameObject.name} HandleInCombat - attacking target: {currentTarget.name}, distance: {distanceToTarget:F2}m"); // TẮT
        
        // Combat range check log
        // Debug.Log($"📏 [COMBAT] {gameObject.name} Combat range check: distance={distanceToTarget:F2}m, attackRange={attackRange:F2}m, CanAttack={CanAttack}, isAttacking={isAttacking}"); // TẮT
        
        // Attack if in range and cooldown ready
        if (distanceToTarget <= attackRange && CanAttack && !isAttacking)
        {
            Debug.Log($"🎯 [COMBAT] {gameObject.name} Attack conditions met - executing attack on {currentTarget.name}");
            StartCoroutine(PerformAttack());
        }
        else if (distanceToTarget > attackRange)
        {
            // Debug.Log($"📏 [COMBAT] {gameObject.name} Target out of attack range: {distanceToTarget:F2}m > {attackRange:F2}m"); // TẮT
        }
        else if (!CanAttack)
        {
            // Debug.Log($"⏰ [COMBAT] {gameObject.name} Attack on cooldown - time since last attack: {Time.time - lastAttackTime:F2}s"); // TẮT
        }
        
        // Movement behavior trong combat
        switch (behaviorType)
        {
            case CombatBehaviorType.Aggressive:
                // Luôn tiến về phía enemy
                if (distanceToTarget > attackRange * 0.8f)
                {
                    Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
                    moveDirection = directionToTarget;
                }
                else
                {
                    moveDirection = Vector3.zero;
                }
                break;
                
            case CombatBehaviorType.Defensive:
                // Giữ khoảng cách, chỉ tấn công khi enemy đến gần
                if (distanceToTarget < attackRange * 0.5f)
                {
                    Vector3 directionAwayFromTarget = (transform.position - currentTarget.transform.position).normalized;
                    moveDirection = directionAwayFromTarget;
                }
                else
                {
                    moveDirection = Vector3.zero;
                }
                break;
                
            case CombatBehaviorType.Balanced:
                // Kết hợp, tùy vào health
                if (teamMember.HealthPercent > 0.5f)
                {
                    // Aggressive when healthy
                    if (distanceToTarget > attackRange * 0.8f)
                    {
                        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
                        moveDirection = directionToTarget;
                    }
                    else
                    {
                        moveDirection = Vector3.zero;
                    }
                }
                else
                {
                    // Defensive when low health
                    if (distanceToTarget < attackRange * 0.7f)
                    {
                        Vector3 directionAwayFromTarget = (transform.position - currentTarget.transform.position).normalized;
                        moveDirection = directionAwayFromTarget;
                    }
                    else
                    {
                        moveDirection = Vector3.zero;
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// Perform attack coroutine
    /// </summary>
    private IEnumerator PerformAttack()
    {
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            Debug.LogWarning($"⚠️ [COMBAT] {gameObject.name} PerformAttack cancelled - target invalid");
            yield break;
        }
        
        isAttacking = true;
        lastAttackTime = Time.time;
        
        Debug.Log($"🥊 [COMBAT] {gameObject.name} STARTING ATTACK on {currentTarget.name}");
        
        OnAttackStarted?.Invoke(currentTarget);
        
        // 🎲 Trigger random attack animation thay vì fixed "attack" trigger
        TriggerAttackAnimation();
        
        Debug.Log($"🎬 [COMBAT] {gameObject.name} Random attack animation triggered for {currentTarget.name}");
        
        // Wait for animation timing (có thể adjust)
        yield return new WaitForSeconds(0.3f);
        
        // Check if target still valid and in range
        if (currentTarget != null && currentTarget.IsAlive && GetDistanceToTarget() <= attackRange)
        {
            Debug.Log($"💥 [COMBAT] {gameObject.name} Attack executed on target: {currentTarget.name} - dealing {attackDamage} damage");
            
            // Deal damage
            currentTarget.TakeDamage(attackDamage, teamMember);
            OnDamageDealt?.Invoke(currentTarget, attackDamage);
            
            // Apply knockback force if target has ragdoll
            RagdollPhysicsController targetRagdoll = currentTarget.GetComponent<RagdollPhysicsController>();
            if (targetRagdoll != null && !currentTarget.IsAlive)
            {
                Debug.Log($"💀 [COMBAT] {gameObject.name} Target killed - applying ragdoll force to {currentTarget.name}");
                
                // Apply death force
                Vector3 forceDirection = (currentTarget.transform.position - transform.position).normalized;
                Vector3 force = forceDirection * 300f + Vector3.up * 100f;
                Vector3 position = currentTarget.transform.position + Vector3.up;
                
                targetRagdoll.ApplyForce(force, position);
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ [COMBAT] {gameObject.name} Attack missed - target moved out of range or died");
        }
        
        // Wait for attack cooldown
        yield return new WaitForSeconds(0.2f);
        
        isAttacking = false;
        Debug.Log($"✅ [COMBAT] {gameObject.name} Attack sequence completed");
    }
      /// <summary>
    /// Handle movement - tích hợp với NavMeshAgent hoặc CharacterController
    /// </summary>
    private void HandleMovement()
    {
        // Debug.Log($"🚶 [COMBAT MOVEMENT] {gameObject.name} HandleMovement() START"); // TẮT
        // Debug.Log($"    - canMove: {canMove}"); // TẮT
        // Debug.Log($"    - useNavMeshMovement: {useNavMeshMovement}"); // TẮT
        // Debug.Log($"    - navAgent != null: {navAgent != null}"); // TẮT
        // Debug.Log($"    - navAgent.enabled: {(navAgent != null ? navAgent.enabled.ToString() : "N/A")}"); // TẮT
        // Debug.Log($"    - characterController != null: {characterController != null}"); // TẮT
        
        if (!canMove)
        {
            // Debug.Log($"❌ [COMBAT MOVEMENT] {gameObject.name} canMove = false, SKIP movement"); // TẮT
            return;
        }
        
        if (useNavMeshMovement && navAgent != null && navAgent.enabled)
        {
            // Debug.Log($"🤖 [COMBAT MOVEMENT] {gameObject.name} SỬ DỤNG NavMeshAgent movement"); // TẮT
            // Sử dụng NavMeshAgent cho movement trong combat
            HandleNavMeshMovement();
        }
        else if (characterController != null)
        {
            // Debug.Log($"🚶 [COMBAT MOVEMENT] {gameObject.name} SỬ DỤNG CharacterController movement (fallback)"); // TẮT
            // Fallback to CharacterController
            HandleCharacterControllerMovement();
        }
        else
        {
            // Debug.LogWarning($"⚠️ [COMBAT MOVEMENT] {gameObject.name} KHÔNG CÓ MOVEMENT METHOD AVAILABLE!"); // TẮT
        }
        
        // Debug.Log($"🏁 [COMBAT MOVEMENT] {gameObject.name} HandleMovement() END"); // TẮT
    }
      /// <summary>
    /// Handle movement với NavMeshAgent - cải tiến cho combat với anti-stuck logic
    /// </summary>
    private void HandleNavMeshMovement()
    {
        if (moveDirection.magnitude > 0.1f && currentTarget != null)
        {
            // Trong combat, di chuyển trực tiếp đến position target với offset
            Vector3 targetPosition = currentTarget.transform.position;
            // Debug.Log($"    - Original target position: {targetPosition}"); // TẮT
            
            // 🔥 CRITICAL FIX: Calculate smart target position để tránh collision
            Vector3 directionToTarget = (transform.position - targetPosition).normalized;
            float offsetDistance = Mathf.Max(attackRange * 0.8f, navAgent.stoppingDistance, 1.0f);
            
            // Adjust target position based on behavior
            switch (behaviorType)
            {
                case CombatBehaviorType.Aggressive:
                    targetPosition = targetPosition + directionToTarget * offsetDistance;
                    // Debug.Log($"    - Aggressive behavior: Offset position: {targetPosition} (offset: {offsetDistance:F2}m)"); // TẮT
                    break;
                    
                case CombatBehaviorType.Defensive:
                    targetPosition = targetPosition + directionToTarget * (offsetDistance * 1.5f);
                    // Debug.Log($"    - Defensive behavior: Far offset position: {targetPosition}"); // TẮT
                    break;
                    
                default:
                    targetPosition = targetPosition + directionToTarget * offsetDistance;
                    // Debug.Log($"    - Default behavior: Standard offset position: {targetPosition}"); // TẮT
                    break;
            }
            
            // 🔥 ANTI-STUCK FIX: Check nếu target position quá gần current position
            float distanceToNewTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToNewTarget < 0.5f)
            {
                // Debug.Log($"⚠️ [NAVMESH MOVEMENT] {gameObject.name} Target quá gần ({distanceToNewTarget:F2}m) - SKIP SetDestination"); // TẮT
                navAgent.isStopped = true;
                return;
            }
            
            // Debug.Log($"🤖 [NAVMESH MOVEMENT] {gameObject.name} CALLING navAgent.SetDestination({targetPosition})"); // TẮT
            // Debug.Log($"    - Distance to new target: {distanceToNewTarget:F2}m"); // TẮT
            
            // Ensure agent is ready to move
            if (navAgent.isStopped)
            {
                navAgent.isStopped = false;
                // Debug.Log($"🔧 [NAVMESH MOVEMENT] {gameObject.name} Un-stopped NavAgent for movement"); // TẮT
            }
            
            navAgent.SetDestination(targetPosition);
            
            // 🔥 CRITICAL CHECK: Verify NavMeshAgent state sau khi SetDestination
            // Debug.Log($"✅ [NAVMESH MOVEMENT] {gameObject.name} VERIFY SAU SetDestination:"); // TẮT
            // Debug.Log($"    - navAgent.hasPath: {navAgent.hasPath}"); // TẮT
            // Debug.Log($"    - navAgent.pathStatus: {navAgent.pathStatus}"); // TẮT
            // Debug.Log($"    - navAgent.remainingDistance: {navAgent.remainingDistance:F2}"); // TẮT
            // Debug.Log($"    - navAgent.destination: {navAgent.destination}"); // TẮT
            
            // 🔥 STUCK PREVENTION: Kiểm tra nếu NavAgent bị stuck
            if (navAgent.hasPath && navAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete)
            {
                if (navAgent.remainingDistance <= navAgent.stoppingDistance && navAgent.velocity.magnitude < 0.1f)
                {
                    // Debug.Log($"✅ [NAVMESH MOVEMENT] {gameObject.name} Đã đến đích hoặc gần đích - dừng movement"); // TẮT
                    navAgent.isStopped = true;
                }
            }
            else if (navAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                // Debug.LogWarning($"⚠️ [NAVMESH MOVEMENT] {gameObject.name} Path INVALID - trying direct approach"); // TẮT
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
        else
        {
            // Debug.Log($"❌ [NAVMESH MOVEMENT] {gameObject.name} ĐIỀU KIỆN MOVEMENT KHÔNG ĐẠT"); // TẮT
            // if (moveDirection.magnitude <= 0.1f) // TẮT
            //     Debug.Log($"    - moveDirection too small: {moveDirection.magnitude:F2}"); // TẮT
            // if (currentTarget == null) // TẮT
            //     Debug.Log($"    - currentTarget is null"); // TẮT
                
            // Stop movement khi không cần di chuyển
            if (!navAgent.isStopped)
            {
                navAgent.isStopped = true;
                // Debug.Log($"🛑 [NAVMESH MOVEMENT] {gameObject.name} Stopped NavAgent - no movement needed"); // TẮT
            }
        }
        
        // Update animator với NavMesh velocity
        if (animator != null)
        {
            float speed = navAgent.velocity.magnitude / moveSpeed;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsMoving", speed > 0.1f);
            // Debug.Log($"🎬 [NAVMESH MOVEMENT] {gameObject.name} Animation updated - Speed: {speed:F2}, IsMoving: {speed > 0.1f}"); // TẮT
        }        
        // Debug.Log($"🏁 [NAVMESH MOVEMENT] {gameObject.name} HandleNavMeshMovement() END"); // TẮT
    }
    
    /// <summary>
    /// Handle movement với CharacterController (legacy)
    /// </summary>
    private void HandleCharacterControllerMovement()
    {
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        
        // Add gravity
        if (!characterController.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
        
        characterController.Move(movement);
        
        // Update animator
        if (animator != null)
        {
            float speed = moveDirection.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsMoving", speed > 0.1f);
        }
    }
      /// <summary>
    /// Handle rotation toward target với anti-loop protection
    /// </summary>
    private void HandleRotation()
    {
        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            directionToTarget.y = 0; // Keep rotation on Y axis only
            
            if (directionToTarget != Vector3.zero)
            {
                // 🔥 CRITICAL FIX: Chỉ rotate khi đang di chuyển hoặc khoảng cách đủ xa
                float distanceToTarget = GetDistanceToTarget();
                bool shouldRotate = true;
                
                // Nếu 2 NPC quá gần nhau và cùng target vào nhau, tạm dừng rotation để tránh loop
                if (distanceToTarget < attackRange * 1.2f)
                {
                    // Check xem target có đang target ngược lại không
                    CombatController targetCombat = currentTarget.GetComponent<CombatController>();
                    if (targetCombat != null && targetCombat.CurrentTarget == teamMember)
                    {
                        // Cả 2 đang target nhau và quá gần -> chỉ cho NPC có instanceID thấp hơn được rotate
                        shouldRotate = GetInstanceID() < currentTarget.GetInstanceID();
                        Debug.Log($"🔄 [ROTATION ANTI-LOOP] {gameObject.name} shouldRotate = {shouldRotate} (vs {currentTarget.name})");
                    }
                }
                
                if (shouldRotate)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
    
    /// <summary>
    /// Get distance to current target
    /// </summary>
    private float GetDistanceToTarget()
    {
        if (currentTarget == null) return float.MaxValue;
        return Vector3.Distance(transform.position, currentTarget.transform.position);
    }
    
    /// <summary>
    /// Event handler for target changed
    /// </summary>
    private void OnTargetChanged(TeamMember newTarget)
    {
        TeamMember? previousTarget = currentTarget;
        currentTarget = newTarget;
        
        if (newTarget != null)
        {
            Debug.Log($"🎯 [COMBAT] {gameObject.name} CurrentTarget set to: {newTarget.name}");
            if (previousTarget != null && previousTarget != newTarget)
            {
                Debug.Log($"🔄 [COMBAT] {gameObject.name} Target changed from {previousTarget.name} to {newTarget.name}");
            }
        }
        else
        {
            Debug.Log($"❌ [COMBAT] {gameObject.name} Target lost - currentTarget set to null");
            if (currentState != CombatState.Idle)
            {
                Debug.Log($"🛑 [COMBAT] {gameObject.name} ExitCombat called - target lost/out of range");
                ChangeState(CombatState.Idle);
            }
        }
    }
    
    /// <summary>
    /// Event handler for death
    /// </summary>
    private void OnDeath(TeamMember deadMember)
    {
        ChangeState(CombatState.Idle);
        currentTarget = null;
        moveDirection = Vector3.zero;
        
        if (debugMode)
            Debug.Log($"{gameObject.name} died, stopping combat");
    }
      /// <summary>
    /// Set animation state
    /// </summary>
    private void SetAnimationState(string stateName)
    {
        if (animator == null) return;
        
        // TẮT ANIMATION LOGS - chỉ focus vào detection/combat
        try
        {
            // Reset all state bools
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsInCombat", false);
            
            // Set current state
            switch (stateName)
            {
                case "idle":
                    animator.SetBool("IsIdle", true);
                    break;
                case "walking":
                    animator.SetBool("IsWalking", true);
                    break;
                case "combat":
                    animator.SetBool("IsInCombat", true);
                    break;
            }
        }
        catch
        {
            // Silently ignore animation errors
        }
    }
      /// <summary>
    /// Get random attack trigger với consecutive attack limitation
    /// </summary>
    private string GetRandomAttackTrigger()
    {
        string selectedTrigger;
        
        // Nếu đã sử dụng cùng attack quá nhiều lần liên tiếp, force chọn attack khác
        if (consecutiveAttackCount >= maxConsecutiveAttacks && !string.IsNullOrEmpty(lastAttackUsed))
        {
            // Tạo list attack khác (loại bỏ lastAttackUsed)
            var availableAttacks = new System.Collections.Generic.List<string>();
            foreach (string attack in attackTriggers)
            {
                if (attack != lastAttackUsed)
                {
                    availableAttacks.Add(attack);
                }
            }
            
            // Random chọn từ list attacks khác
            int randomIndex = attackRandom.Next(availableAttacks.Count);
            selectedTrigger = availableAttacks[randomIndex];
            
            Debug.Log($"🔄 [RANDOM ATTACK] {gameObject.name} FORCE CHANGE attack (consecutiveCount: {consecutiveAttackCount}) - từ '{lastAttackUsed}' sang '{selectedTrigger}'");
            
            // Reset consecutive count
            consecutiveAttackCount = 1;
        }
        else
        {
            // Random bình thường từ tất cả attacks
            int randomIndex = attackRandom.Next(attackTriggers.Length);
            selectedTrigger = attackTriggers[randomIndex];
            
            // Update consecutive count
            if (selectedTrigger == lastAttackUsed)
            {
                consecutiveAttackCount++;
            }
            else
            {
                consecutiveAttackCount = 1;
            }
            
            Debug.Log($"🎲 [RANDOM ATTACK] {gameObject.name} RANDOM SELECT '{selectedTrigger}' (consecutiveCount: {consecutiveAttackCount})");
        }
        
        // Update lastAttackUsed
        lastAttackUsed = selectedTrigger;
        
        Debug.Log($"✅ [RANDOM ATTACK] {gameObject.name} Selected: '{selectedTrigger}' | LastUsed: '{lastAttackUsed}' | ConsecutiveCount: {consecutiveAttackCount}");
        
        return selectedTrigger;
    }
    
    /// <summary>
    /// Trigger random attack animation thay thế cho SetAnimationTrigger("attack")
    /// </summary>
    private void TriggerAttackAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning($"⚠️ [RANDOM ATTACK] {gameObject.name} Animator is null - cannot trigger attack animation");
            return;
        }
        
        string attackTrigger = GetRandomAttackTrigger();
        
        Debug.Log($"🎬 [RANDOM ATTACK] {gameObject.name} Triggering attack animation: '{attackTrigger}'");
        
        try
        {
            animator.SetTrigger(attackTrigger);
            Debug.Log($"✅ [RANDOM ATTACK] {gameObject.name} Successfully triggered '{attackTrigger}' animation");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [RANDOM ATTACK] {gameObject.name} Failed to trigger '{attackTrigger}' - Error: {e.Message}");
            
            // Fallback to basic "attack" trigger nếu random attack fails
            try
            {
                animator.SetTrigger("attack");
                Debug.Log($"🔄 [RANDOM ATTACK] {gameObject.name} FALLBACK to basic 'attack' trigger");
            }
            catch
            {
                Debug.LogError($"❌ [RANDOM ATTACK] {gameObject.name} Even fallback 'attack' trigger failed!");
            }
        }
    }

    /// <summary>
    /// Set animation trigger (legacy method, kept for compatibility)
    /// </summary>
    private void SetAnimationTrigger(string triggerName)
    {
        if (animator == null) return;
        
        try
        {
            animator.SetTrigger(triggerName);
        }
        catch
        {
            // Silently ignore animation errors
        }
    }
    
    /// <summary>
    /// Start combat with specific target - FIXED WITH DEBUG LOGS
    /// </summary>
    public void StartCombat(TeamMember target)
    {
        Debug.Log($"🥊 [COMBAT] StartCombat() được gọi cho {gameObject.name} với target: {(target != null ? target.name : "null")}");
        
        if (target == null)
        {
            Debug.LogError($"❌ [COMBAT] {gameObject.name} StartCombat() với null target!");
            return;
        }
        
        if (!target.IsAlive)
        {
            Debug.LogError($"💀 [COMBAT] {gameObject.name} StartCombat() với dead target: {target.name}");
            return;
        }
        
        // Set target và change state
        currentTarget = target;
        Debug.Log($"✅ [COMBAT] {gameObject.name} đã set currentTarget = {target.name}");
        
        // Force state change
        ChangeState(CombatState.Engaging);
        Debug.Log($"🔄 [COMBAT] {gameObject.name} đã chuyển sang CombatState.Engaging");
        
        // Verify state change
        if (currentState == CombatState.Engaging)
        {
            Debug.Log($"✅ [COMBAT] {gameObject.name} COMBAT STATE ACTIVE với target {target.name}");
        }
        else
        {
            Debug.LogError($"❌ [COMBAT] {gameObject.name} FAILED TO CHANGE TO COMBAT STATE! Current: {currentState}");
        }
    }
    
    /// <summary>
    /// Stop combat and return to idle
    /// </summary>
    public void StopCombat()
    {
        Debug.Log($"🛑 [COMBAT] {gameObject.name} ExitCombat called - stopping combat");
        
        ChangeState(CombatState.Idle);
        currentTarget = null;
        
        Debug.Log($"🏁 [COMBAT] {gameObject.name} Combat stopped - returned to idle state");
    }
    
    /// <summary>
    /// Force attack target (for testing)
    /// </summary>
    [ContextMenu("Force Attack Current Target")]
    public void ForceAttack()
    {
        if (currentTarget != null && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }
    
    private void OnDestroy()
    {        // Unsubscribe from events safely
        try
        {
            if (enemyDetector != null)
                enemyDetector.OnTargetChanged -= OnTargetChanged;
            if (teamMember != null)
                teamMember.OnDeath -= OnDeath;
        }
        catch
        {
            // Ignore errors during cleanup
        }
    }
    
    /// <summary>
    /// Debug visualization
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!debugMode) return;
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Engage distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, engageDistance);
        
        // Move direction
        if (Application.isPlaying && moveDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
        }
    }
}

/// <summary>
/// Enum cho combat states
/// </summary>
public enum CombatState
{
    Idle,
    Engaging,
    InCombat
}

/// <summary>
/// Enum cho combat behavior types
/// </summary>
public enum CombatBehaviorType
{
    Aggressive,
    Defensive, 
    Balanced
}
