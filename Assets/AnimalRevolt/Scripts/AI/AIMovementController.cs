using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Controller chính cho AI movement và enemy seeking
/// Tích hợp NavMeshAgent, TeamMember, EnemyDetector và CombatController
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TeamMember))]
[RequireComponent(typeof(EnemyDetector))]
public class AIMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float engageDistance = 8f;
    [SerializeField] private float combatRange = 5f;
    
    [Header("AI Behavior")]
    [SerializeField] private AIBehaviorType behaviorType = AIBehaviorType.Aggressive;
    [SerializeField] private float seekRadius = 15f;
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private bool enablePatrol = true;
    
    [Header("Performance")]
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float pathRecalculateInterval = 0.5f;
    [SerializeField] private bool debugMode = true;
    
    [Header("Animation Settings")]
    [SerializeField] private bool enableRootMotion = false;
    [SerializeField] private float animationSmoothTime = 0.1f;
    
    [Header("Components")]
    [SerializeField] private TeamMember teamMember;
    [SerializeField] private EnemyDetector enemyDetector;
    [SerializeField] private CombatController combatController;
    [SerializeField] private AIStateMachine stateMachine;
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Animator animator;
    
    // Private variables
    private Vector3 startPosition;
    private Vector3 currentDestination;
    private TeamMember currentTarget;
    private float lastUpdateTime;
    private float lastPathRecalculateTime;
    private float idleStartTime;
    private bool isMoving = false;
    private bool isInCombat = false;
    
    // Animation sync variables
    private float currentAnimationSpeed = 0f;
    private const float MOVEMENT_THRESHOLD = 0.01f;
    
    // Ragdoll integration
    private RagdollPhysicsController ragdollController;
    private bool wasRagdolled = false;
    
    // Events
    public System.Action<TeamMember> OnTargetFound;
    public System.Action<TeamMember> OnTargetLost;
    public System.Action<Vector3> OnDestinationSet;
    public System.Action OnDestinationReached;
    
    // Properties
    public AIState CurrentState => stateMachine.CurrentState;
    public TeamMember CurrentTarget => currentTarget;
    public bool IsMoving => isMoving;
    public bool CanMove => teamMember.IsAlive && navAgent.enabled;
    public float DistanceToTarget => currentTarget != null ? 
        Vector3.Distance(transform.position, currentTarget.transform.position) : float.MaxValue;
    
    private void Awake()
    {
        // Auto-find components
        if (teamMember == null)
            teamMember = GetComponent<TeamMember>();
        if (enemyDetector == null)
            enemyDetector = GetComponent<EnemyDetector>();
        if (combatController == null)
            combatController = GetComponent<CombatController>();
        if (stateMachine == null)
            stateMachine = GetComponent<AIStateMachine>();
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (ragdollController == null)
            ragdollController = GetComponent<RagdollPhysicsController>();
            
        // Add AIStateMachine if not present
        if (stateMachine == null)
            stateMachine = gameObject.AddComponent<AIStateMachine>();
    }
    
    private void Start()
    {
        // Validate required components
        if (!ValidateComponents()) return;
        
        // ✅ VALIDATE COMBAT RANGE SETTINGS
        ValidateCombatRangeSettings();
        
        // Setup initial state
        startPosition = transform.position;
        currentDestination = startPosition;
        
        // Configure NavMeshAgent
        SetupNavMeshAgent();
        
        // Subscribe to events
        SubscribeToEvents();
        
        // ✅ ENHANCED: Log animator parameters info
        LogAnimatorParametersInfo();
        
        // Initialize state machine
        stateMachine.ChangeState(AIState.Idle);
        idleStartTime = Time.time;
        
        if (debugMode)
            Debug.Log($"🤖 [AI] AIMovementController đã khởi tạo cho {gameObject.name}");
    }
    
    private void Update()
    {
        if (!teamMember.IsAlive) return;
        
        // Handle ragdoll recovery
        HandleRagdollRecovery();
        
        // ✅ FIX: Update animations EVERY frame for instant response
        UpdateAnimations();
        
        // Update AI logic với interval
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAILogic();
            lastUpdateTime = Time.time;
        }
        
        // Update movement
        UpdateMovement();
    }
    
    /// <summary>
    /// Validate required components
    /// </summary>
    private bool ValidateComponents()
    {
        if (teamMember == null)
        {
            Debug.LogError($"AIMovementController on {gameObject.name} requires TeamMember component!");
            enabled = false;
            return false;
        }
        
        if (enemyDetector == null)
        {
            Debug.LogError($"AIMovementController on {gameObject.name} requires EnemyDetector component!");
            enabled = false;
            return false;
        }
        
        if (navAgent == null)
        {
            Debug.LogError($"AIMovementController on {gameObject.name} requires NavMeshAgent component!");
            enabled = false;
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validate combat range settings để đảm bảo logic hoạt động đúng
    /// </summary>
    private void ValidateCombatRangeSettings()
    {
        Debug.Log($"🔧 [AI] {gameObject.name} Validating combat range settings...");
        
        // Log all range values
        Debug.Log($"📏 [AI] {gameObject.name} Range Settings:");
        Debug.Log($"  - combatRange: {combatRange:F2}m");
        Debug.Log($"  - engageDistance: {engageDistance:F2}m");
        Debug.Log($"  - stoppingDistance: {stoppingDistance:F2}m");
        Debug.Log($"  - seekRadius: {seekRadius:F2}m");
        
        // Validate ranges make sense
        bool hasErrors = false;
        
        if (combatRange <= 0)
        {
            Debug.LogError($"❌ [AI] {gameObject.name} combatRange ({combatRange}) phải > 0!");
            hasErrors = true;
        }
        
        if (engageDistance <= combatRange)
        {
            Debug.LogWarning($"⚠️ [AI] {gameObject.name} engageDistance ({engageDistance}) nên > combatRange ({combatRange}) để tránh switching liên tục");
        }
        
        if (seekRadius <= engageDistance)
        {
            Debug.LogWarning($"⚠️ [AI] {gameObject.name} seekRadius ({seekRadius}) nên > engageDistance ({engageDistance}) để detect enemies tốt hơn");
        }
        
        if (stoppingDistance >= combatRange)
        {
            Debug.LogWarning($"⚠️ [AI] {gameObject.name} stoppingDistance ({stoppingDistance}) nên < combatRange ({combatRange})");
        }
        
        // Check CombatController ranges
        if (combatController != null)
        {
            // Sử dụng reflection để access private fields
            var attackRangeField = typeof(CombatController).GetField("attackRange",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engageDistanceField = typeof(CombatController).GetField("engageDistance",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
            if (attackRangeField != null && engageDistanceField != null)
            {
                float combatAttackRange = (float)attackRangeField.GetValue(combatController);
                float combatEngageDistance = (float)engageDistanceField.GetValue(combatController);
                
                Debug.Log($"🥊 [AI] {gameObject.name} CombatController Settings:");
                Debug.Log($"  - attackRange: {combatAttackRange:F2}m");
                Debug.Log($"  - engageDistance: {combatEngageDistance:F2}m");
                
                if (Mathf.Abs(combatRange - combatAttackRange) > 0.1f)
                {
                    Debug.LogWarning($"⚠️ [AI] {gameObject.name} AIMovementController.combatRange ({combatRange}) khác CombatController.attackRange ({combatAttackRange})");
                }
                
                if (Mathf.Abs(engageDistance - combatEngageDistance) > 0.1f)
                {
                    Debug.LogWarning($"⚠️ [AI] {gameObject.name} AIMovementController.engageDistance ({engageDistance}) khác CombatController.engageDistance ({combatEngageDistance})");
                }
            }
        }
        
        if (hasErrors)
        {
            Debug.LogError($"❌ [AI] {gameObject.name} có lỗi range settings! AI có thể không hoạt động đúng.");
        }
        else
        {
            Debug.Log($"✅ [AI] {gameObject.name} range settings validation completed");
        }
    }
    
    /// <summary>
    /// Setup NavMeshAgent properties
    /// </summary>
    private void SetupNavMeshAgent()
    {
        navAgent.speed = walkSpeed;
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.angularSpeed = rotationSpeed * 50f; // Convert to degrees/second
        navAgent.acceleration = 8f;
        navAgent.autoBraking = true;
        navAgent.autoRepath = true;
        
        // CRITICAL: Ensure NavMeshAgent is enabled and not stopped
        navAgent.enabled = true;
        navAgent.isStopped = false;
        
        if (debugMode)
            Debug.Log($"🔧 [AI] NavMeshAgent được setup cho {gameObject.name} - Speed: {navAgent.speed}, StopDistance: {navAgent.stoppingDistance}");
    }
    
    /// <summary>
    /// Subscribe to events
    /// </summary>
    private void SubscribeToEvents()
    {
        if (enemyDetector != null)
        {
            enemyDetector.OnTargetChanged += OnEnemyTargetChanged;
            enemyDetector.OnEnemyDetected += OnEnemyDetected;
            enemyDetector.OnEnemyLost += OnEnemyLost;
        }
        
        if (combatController != null)
        {
            combatController.OnStateChanged += OnCombatStateChanged;
        }
        
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += OnAIStateChanged;
        }
        
        if (teamMember != null)
        {
            teamMember.OnDeath += OnDeath;
        }
    }    
    /// <summary>
    /// Main AI logic update - FIXED PRIORITY LOGIC & COMBAT PERSISTENCE
    /// </summary>
    private void UpdateAILogic()
    {
        // 🔥 DEBUG UpdateAILogic: Log trạng thái hiện tại
        // Debug.Log($"🧠 [DEBUG] {gameObject.name} UpdateAILogic");
        // Debug.Log($"    - State: {stateMachine.CurrentState}");
        // Debug.Log($"    - HasEnemies: {enemyDetector.HasEnemies}");
        // Debug.Log($"    - CurrentTarget: {(currentTarget != null ? currentTarget.name : "null")}");
        
        // ✅ FIX: THÊM CONDITION CHECK - nếu có currentTarget thì chuyển sang Seeking state
        if (currentTarget != null && currentTarget.IsAlive && stateMachine.CurrentState == AIState.Idle)
        {
            // Debug.Log($"🎯 [DEBUG] {gameObject.name} CÓ TARGET trong Idle state, chuyển sang Seeking!");
            stateMachine.ChangeState(AIState.Seeking);
            return;
        }
        
        // ✅ PRIORITY 1: MAINTAIN COMBAT STATE - AI không được thoát Combat khi vẫn có enemies
        if (stateMachine.CurrentState == AIState.Combat && enemyDetector.HasEnemies)
        {
            Debug.Log($"⚔️ [DEBUG] {gameObject.name} MAINTAINING COMBAT STATE");
            HandleCombatState();
            return; // Ưu tiên tuyệt đối cho Combat state
        }
        
        // ✅ PRIORITY 2: Nếu có currentTarget và đang alive, ưu tiên Combat/Seeking
        if (currentTarget != null && currentTarget.IsAlive && enemyDetector.HasEnemies)
        {
            float distanceToTarget = DistanceToTarget;
            
            if (distanceToTarget <= combatRange && stateMachine.CurrentState != AIState.Combat)
            {
                Debug.Log($"🎯 [DEBUG] {gameObject.name} PRIORITY SWITCH TO COMBAT - Target in range: {distanceToTarget:F2}m");
                if (combatController != null)
                {
                    combatController.StartCombat(currentTarget);
                }
                stateMachine.ChangeState(AIState.Combat);
                return;
            }
            else if (distanceToTarget > combatRange && stateMachine.CurrentState != AIState.Seeking && stateMachine.CurrentState != AIState.Combat)
            {
                Debug.Log($"🎯 [DEBUG] {gameObject.name} PRIORITY SWITCH TO SEEKING - Target too far: {distanceToTarget:F2}m");
                stateMachine.ChangeState(AIState.Seeking);
                return;
            }
        }
        
        // ✅ STANDARD STATE HANDLING (chỉ khi không phải Combat state)
        switch (stateMachine.CurrentState)
        {
            case AIState.Idle:
                HandleIdleState();
                break;
                
            case AIState.Seeking:
                HandleSeekingState();
                break;
                
            case AIState.Moving:
                HandleMovingState();
                break;
                
            case AIState.Combat:
                // Combat state đã được handle ở priority section
                HandleCombatState();
                break;
        }
    }
    /// <summary>
    /// Handle idle state logic
    /// </summary>
    private void HandleIdleState()
    {
        // Kiểm tra có enemy không
        if (enemyDetector.HasEnemies)
        {
            stateMachine.ChangeState(AIState.Seeking);
            return;
        }
        
        // Patrol behavior
        if (enablePatrol && Time.time - idleStartTime > idleTime)
        {
            Vector3 patrolPoint = GetRandomPatrolPoint();
            if (patrolPoint != Vector3.zero)
            {
                SetDestination(patrolPoint);
                stateMachine.ChangeState(AIState.Moving);
            }
            else
            {
                idleStartTime = Time.time; // Reset idle timer
            }
        }
    }
    
    /// <summary>
    /// Handle seeking state logic - FIXED COMBAT TRANSITION
    /// </summary>
    private void HandleSeekingState()
    {
        // ENHANCED: Fallback target assignment nếu currentTarget bị null
        if (currentTarget == null)
        {
            if (enemyDetector.HasEnemies)
            {
                currentTarget = SelectBestTarget();
                if (currentTarget != null)
                {
                    // Debug.Log($"🔄 [AI] {gameObject.name} đã tự động chọn target: {currentTarget.name}");
                }
            }
            
            if (currentTarget == null)
            {
                // Debug.Log($"❌ [AI] {gameObject.name} không có target, về Idle state");
                stateMachine.ChangeState(AIState.Idle);
                return;
            }
        }

        // VALIDATE TARGET is still alive and valid
        if (!currentTarget.IsAlive || currentTarget == null)
        {
            // Debug.Log($"💀 [AI] {gameObject.name} target đã chết hoặc invalid, về Idle state");
            currentTarget = null;
            stateMachine.ChangeState(AIState.Idle);
            return;
        }

        // FORCE MOVEMENT TOWARD TARGET
        Vector3 direction = (currentTarget.transform.position - transform.position);
        float distance = direction.magnitude;
        
        // Debug.Log($"📏 [AI] {gameObject.name} đang seeking - Distance: {distance:F2}m, CombatRange: {combatRange:F2}m");
        
        // DI CHUYỂN ĐẾN TARGET
        if (distance > combatRange)
        {
            direction.y = 0;
            direction.Normalize();
            
            // FORCE NAVMESH MOVEMENT
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.SetDestination(currentTarget.transform.position);
                navAgent.isStopped = false;
                // Debug.Log($"🏃 [AI] {gameObject.name} đang di chuyển đến {currentTarget.name}, khoảng cách: {distance:F2}m");
            }
        }
        else
        {
            // ✅ FIX: FORCE STAY IN COMBAT - KHÔNG CHO QUAY VỀ IDLE
            // Debug.Log($"⚔️ [AI] {gameObject.name} ĐỦ GẦN để combat với {currentTarget.name} (distance: {distance:F2}m <= range: {combatRange:F2}m)");
            
            // STOP NAVMESH MOVEMENT trước khi chuyển state
            if (navAgent != null && navAgent.enabled && navAgent.hasPath)
            {
                navAgent.ResetPath();
                navAgent.isStopped = true;
                // Debug.Log($"🛑 [AI] {gameObject.name} dừng NavMesh movement để chuẩn bị combat");
            }
            
            // START COMBAT CONTROLLER trước
            if (combatController != null)
            {
                combatController.StartCombat(currentTarget);
                // Debug.Log($"🥊 [AI] {gameObject.name} StartCombat() được gọi với target {currentTarget.name}");
            }
            
            // FORCE CHANGE TO COMBAT STATE
            stateMachine.ChangeState(AIState.Combat);
            // Debug.Log($"🎯 [AI] {gameObject.name} FORCE SWITCHING TO COMBAT STATE với {currentTarget.name}");
        }
    }
    
    /// <summary>
    /// Handle moving state logic
    /// </summary>
    private void HandleMovingState()
    {
        // Kiểm tra có enemy mới không
        if (enemyDetector.HasEnemies && currentTarget != null)
        {
            float distanceToTarget = DistanceToTarget;
            
            // Đủ gần để combat
            if (distanceToTarget <= engageDistance)
            {
                stateMachine.ChangeState(AIState.Combat);
                return;
            }
            
            // Update destination nếu target di chuyển xa
            if (ShouldRecalculatePath())
            {
                SetDestination(currentTarget.transform.position);
            }
        }
        
        // Đã đến destination
        if (HasReachedDestination())
        {
            OnDestinationReached?.Invoke();
            
            if (enemyDetector.HasEnemies)
            {
                stateMachine.ChangeState(AIState.Seeking);
            }
            else
            {
                stateMachine.ChangeState(AIState.Idle);
                idleStartTime = Time.time;
            }
        }
    }
      /// <summary>
    /// Handle combat state logic - FIXED COMBAT EXECUTION
    /// </summary>
    private void HandleCombatState()
    {
        // Debug.Log($"🥊 [AI] {gameObject.name} trong HandleCombatState() - HasEnemies: {enemyDetector.HasEnemies}, CurrentTarget: {(currentTarget != null ? currentTarget.name : "null")}");
        
        // ✅ FIX: KIỂM TRA CHẶT CHẼ TRƯỚC KHI THOÁT COMBAT
        if (!enemyDetector.HasEnemies)
        {
            // Debug.Log($"❌ [AI] {gameObject.name} không còn enemies, thoát Combat về Idle");
            if (combatController != null)
            {
                combatController.StopCombat();
            }
            stateMachine.ChangeState(AIState.Idle);
            idleStartTime = Time.time;
            return;
        }
        
        // ✅ FIX: VALIDATE TARGET TRONG COMBAT
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            Debug.Log($"⚠️ [AI] {gameObject.name} target invalid trong combat, tìm target mới");
            currentTarget = SelectBestTarget();
            
            if (currentTarget == null)
            {
                Debug.Log($"❌ [AI] {gameObject.name} không tìm được target mới, về Seeking");
                stateMachine.ChangeState(AIState.Seeking);
                return;
            }
            else
            {
                Debug.Log($"✅ [AI] {gameObject.name} đã tìm được target mới: {currentTarget.name}");
                if (combatController != null)
                {
                    combatController.StartCombat(currentTarget);
                }
            }
        }
        
        float distanceToTarget = DistanceToTarget;
        Debug.Log($"📏 [AI] {gameObject.name} trong Combat - Distance to target: {distanceToTarget:F2}m, CombatRange: {combatRange:F2}m, EngageDistance: {engageDistance:F2}m");
        
        // ✅ CORE FIX: ENSURE COMBAT CONTROLLER IS ACTIVE VÀ EXECUTING
        if (combatController != null && currentTarget != null)
        {
            // Đảm bảo CombatController đã được start với đúng target
            if (!combatController.IsInCombat)
            {
                Debug.Log($"🔄 [AI] {gameObject.name} CombatController không active, restart combat với {currentTarget.name}");
                combatController.StartCombat(currentTarget);
            }
            
            // ✅ CRITICAL FIX: Update CombatController để handle combat logic
            // CombatController sẽ tự động handle attack, movement, và rotation trong Update()
            Debug.Log($"⚔️ [AI] {gameObject.name} DELEGATING COMBAT EXECUTION to CombatController với target {currentTarget.name}");
        }
        else if (combatController == null)
        {
            Debug.LogError($"❌ [AI] {gameObject.name} CombatController is null! Cannot execute combat!");
        }
        
        // ✅ FIX: CHỈ THOÁT COMBAT KHI TARGET THẬT SỰ QUÁ XA (sử dụng disengage distance)
        if (currentTarget != null && distanceToTarget > engageDistance * 1.5f) // Moderate threshold để tránh switching liên tục
        {
            Debug.Log($"🏃 [AI] {gameObject.name} target quá xa ({distanceToTarget:F2}m > {engageDistance * 1.5f:F2}m), về Seeking");
            if (combatController != null)
            {
                combatController.StopCombat();
            }
            stateMachine.ChangeState(AIState.Seeking);
            return;
        }        
        // 🔥 CRITICAL DEBUG: Check NavMesh state trong Combat
        // Debug.Log($"🤖 [AI COMBAT] {gameObject.name} NAVMESH STATE CHECK:");
        // Debug.Log($"    - navAgent.enabled: {navAgent.enabled}");
        // Debug.Log($"    - navAgent.isStopped: {navAgent.isStopped}");
        // Debug.Log($"    - navAgent.hasPath: {navAgent.hasPath}");
        // Debug.Log($"    - navAgent.speed: {navAgent.speed:F2}");
        // Debug.Log($"    - navAgent.velocity: {navAgent.velocity}");
        
        // ✅ CRITICAL FIX: KHÔNG DISABLE NavMeshAgent trong Combat - Let CombatController use it
        if (navAgent != null && navAgent.enabled)
        {
            // Debug.Log($"🤝 [AI COMBAT] {gameObject.name} ENABLING NavMesh cho CombatController control");
            
            // Reset path chỉ khi có path cũ từ AIMovementController
            if (navAgent.hasPath && navAgent.destination != currentTarget?.transform.position)
            {
                navAgent.ResetPath();
                Debug.Log($"🔄 [AI COMBAT] {gameObject.name} Reset old path for CombatController");
            }
            
            // ✅ KEY FIX: ENSURE NavMeshAgent CAN MOVE
            navAgent.isStopped = false;
            Debug.Log($"✅ [AI COMBAT] {gameObject.name} NavMesh ENABLED for CombatController - isStopped: {navAgent.isStopped}");
        }
        else
        {
            Debug.Log($"ℹ️ [AI COMBAT] {gameObject.name} NavMesh not available");
        }
        
        Debug.Log($"⚔️ [AI] {gameObject.name} STAYING IN COMBAT STATE - CombatController handling execution với target {currentTarget?.name}");
    }
    
    /// <summary>
    /// Cập nhật target selection thông minh
    /// </summary>
    private void UpdateTargetSelection()
    {
        if (!enemyDetector.HasEnemies) return;
        
        // Nếu chưa có target hoặc target hiện tại đã chết
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            currentTarget = SelectBestTarget();
            return;
        }
        
        // Nếu có target mới tốt hơn
        TeamMember betterTarget = SelectBestTarget();
        if (betterTarget != currentTarget && betterTarget != null)
        {
            float currentTargetScore = CalculateTargetScore(currentTarget);
            float betterTargetScore = CalculateTargetScore(betterTarget);
            
            // Switch target nếu target mới tốt hơn đáng kể (threshold để tránh switching liên tục)
            if (betterTargetScore > currentTargetScore + 0.2f)
            {
                currentTarget = betterTarget;
                if (debugMode)
                    Debug.Log($"{gameObject.name} switched target to {currentTarget.gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// Chọn target tốt nhất dựa trên nhiều criteria
    /// </summary>
    private TeamMember SelectBestTarget()
    {
        if (!enemyDetector.HasEnemies) return null;
        
        TeamMember bestTarget = null;
        float bestScore = -1f;
        
        foreach (TeamMember enemy in enemyDetector.DetectedEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            
            float score = CalculateTargetScore(enemy);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }
        
        return bestTarget;
    }
    
    /// <summary>
    /// Tính điểm cho target selection
    /// </summary>
    private float CalculateTargetScore(TeamMember enemy)
    {
        if (enemy == null || !enemy.IsAlive) return 0f;
        
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        float maxDistance = seekRadius;
        
        // Các yếu tố tính điểm:
        float distanceScore = 1f - Mathf.Clamp01(distance / maxDistance); // Gần = tốt
        float healthScore = 1f - enemy.HealthPercent; // Ít máu = priority cao
        float stabilityBonus = (currentTarget == enemy) ? 0.3f : 0f; // Bonus cho target hiện tại
        
        // Weighted final score
        return distanceScore * 0.5f + healthScore * 0.3f + stabilityBonus;
    }
    
    /// <summary>
    /// Predict vị trí target sẽ di chuyển đến
    /// </summary>
    private Vector3 PredictTargetPosition(TeamMember target)
    {
        if (target == null) return Vector3.zero;
        
        // Lấy NavMeshAgent của target nếu có
        NavMeshAgent targetAgent = target.GetComponent<NavMeshAgent>();
        if (targetAgent != null && targetAgent.hasPath)
        {
            // Predict based on target's movement direction
            Vector3 targetVelocity = targetAgent.velocity;
            if (targetVelocity.magnitude > 0.1f)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                float predictionTime = distance / navAgent.speed; // Time to reach target
                return target.transform.position + targetVelocity * predictionTime;
            }
        }
        
        // Fallback to current position
        return target.transform.position;
    }
    
    /// <summary>
    /// Set destination cho NavMeshAgent
    /// </summary>
    public void SetDestination(Vector3 destination)
    {
        if (!CanMove) return;
        
        // Kiểm tra destination có valid không
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, 2f, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
            navAgent.SetDestination(currentDestination);
            lastPathRecalculateTime = Time.time;
            
            OnDestinationSet?.Invoke(currentDestination);
            
            if (debugMode)
                Debug.Log($"{gameObject.name} set destination to {currentDestination}");
        }
        else if (debugMode)
        {
            Debug.LogWarning($"{gameObject.name} invalid destination: {destination}");
        }
    }
    
    /// <summary>
    /// Stop movement
    /// </summary>
    public void StopMovement()
    {
        if (navAgent.hasPath)
        {
            navAgent.ResetPath();
        }
        currentDestination = transform.position;
    }
    
    /// <summary>
    /// Kiểm tra có cần recalculate path không
    /// </summary>
    private bool ShouldRecalculatePath()
    {
        if (currentTarget == null) return false;
        
        // Recalculate theo interval
        if (Time.time - lastPathRecalculateTime < pathRecalculateInterval)
            return false;
        
        // Recalculate nếu target di chuyển xa từ destination hiện tại
        float distanceFromTargetToDestination = Vector3.Distance(
            currentTarget.transform.position, currentDestination);
        
        return distanceFromTargetToDestination > stoppingDistance * 2f;
    }
    
    /// <summary>
    /// Kiểm tra đã đến destination chưa
    /// </summary>
    private bool HasReachedDestination()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            // ✅ FIX 6: Sử dụng consistent threshold
            return !navAgent.hasPath || navAgent.velocity.sqrMagnitude < MOVEMENT_THRESHOLD * MOVEMENT_THRESHOLD;
        }
        return false;
    }
    
    /// <summary>
    /// Lấy random patrol point quanh start position
    /// </summary>
    private Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++) // Try 10 times
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += startPosition;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        
        return Vector3.zero; // Failed to find patrol point
    }
    
    /// <summary>
    /// Update movement and speed - Cải thiện animation sync
    /// </summary>
    private void UpdateMovement()
    {
        if (!navAgent.enabled) return;
        
        // ✅ FIX 1: Giảm threshold cho immediate response
        isMoving = navAgent.velocity.sqrMagnitude > MOVEMENT_THRESHOLD * MOVEMENT_THRESHOLD;
        
        // Handle Root Motion
        HandleRootMotion();
        
        // Adjust speed based on state
        float targetSpeed = walkSpeed;
        
        if (stateMachine.CurrentState == AIState.Seeking ||
            (stateMachine.CurrentState == AIState.Moving && currentTarget != null))
        {
            targetSpeed = runSpeed;
        }
        
        navAgent.speed = Mathf.Lerp(navAgent.speed, targetSpeed, Time.deltaTime * 2f);
        
        // ✅ DEBUG: Log movement state với emoji (DISABLED FOR COMBAT DEBUG)
        // if (debugMode && isMoving)
        // {
        //     Debug.Log($"🏃 {gameObject.name} Moving: Velocity={navAgent.velocity.magnitude:F2}, Speed={navAgent.speed:F2}, Target={targetSpeed:F2}");
        // }
    }
    
    /// <summary>
    /// ✅ ENHANCED: Update animations với safe parameter checking và Vietnamese logs
    /// </summary>
    private void UpdateAnimations()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            if (debugMode)
                Debug.LogWarning($"🎭 [ANIMATOR] Animator hoặc Controller không tồn tại trên {gameObject.name}");
            return;
        }
        
        // ✅ INSTANT RESPONSE: Sử dụng NavMeshAgent.velocity.magnitude trực tiếp
        float speed = navAgent != null ? navAgent.velocity.magnitude : 0f;
        bool isWalking = speed > 0.1f; // Threshold 0.1f như yêu cầu
        
        // ✅ CORE FIX: Set animation parameters mỗi frame cho instant response với safe checking
        SetAnimatorParameter("Speed", speed);
        SetAnimatorParameter("IsWalking", isWalking);
        
        // Additional animation parameters để tương thích với nhiều Animator Controller
        SetAnimatorParameter("Velocity", speed);
        SetAnimatorParameter("MoveSpeed", speed);
        SetAnimatorParameter("IsMoving", isWalking);
        SetAnimatorParameter("Moving", isWalking);
        SetAnimatorParameter("Walking", isWalking);
        SetAnimatorParameter("Walk", isWalking);
        
        // Normalized speed (0-1 range) cho blend trees
        float normalizedSpeed = navAgent != null && navAgent.speed > 0 ? Mathf.Clamp01(speed / navAgent.speed) : 0f;
        SetAnimatorParameter("NormalizedSpeed", normalizedSpeed);
        
        // State-specific parameters
        bool isIdle = stateMachine.CurrentState == AIState.Idle && !isWalking;
        SetAnimatorParameter("IsIdle", isIdle);
        SetAnimatorParameter("Idle", isIdle);
        
        bool isSeeking = stateMachine.CurrentState == AIState.Seeking;
        SetAnimatorParameter("IsSeeking", isSeeking);
        SetAnimatorParameter("Seeking", isSeeking);
        
        bool inCombat = stateMachine.CurrentState == AIState.Combat;
        SetAnimatorParameter("IsInCombat", inCombat);
        SetAnimatorParameter("Combat", inCombat);
        SetAnimatorParameter("InCombat", inCombat);
        
        // ✅ ENHANCED DEBUG: Vietnamese logs với detailed info (DISABLED FOR COMBAT DEBUG)
        // if (debugMode && (isWalking || speed > 0.01f))
        // {
        //     Debug.Log($"🎬 [ANIMATION] {gameObject.name} - Tốc độ: {speed:F2}, Đang đi: {isWalking}, " +
        //              $"Normalized: {normalizedSpeed:F2}, Trạng thái: {stateMachine.CurrentState}");
        // }
    }
    
    /// <summary>
    /// ✅ ENHANCED: Log danh sách parameters có sẵn trong Animator Controller
    /// </summary>
    private void LogAnimatorParametersInfo()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Không có Animator hoặc Controller trên {gameObject.name}");
            return;
        }
        
        if (!debugMode) return;
        
        var parameters = animator.parameters;
        if (parameters.Length == 0)
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Animator Controller trên {gameObject.name} không có parameters nào!");
            return;
        }
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"🎭 [ANIMATOR] Danh sách parameters có sẵn trong {gameObject.name}:");
        
        foreach (AnimatorControllerParameter param in parameters)
        {
            string typeIcon = GetParameterTypeIcon(param.type);
            sb.AppendLine($"  {typeIcon} {param.name} ({param.type})");
        }
        
        Debug.Log(sb.ToString());
    }
    
    /// <summary>
    /// ✅ HELPER: Lấy icon cho parameter type
    /// </summary>
    private string GetParameterTypeIcon(AnimatorControllerParameterType type)
    {
        switch (type)
        {
            case AnimatorControllerParameterType.Bool: return "🔘";
            case AnimatorControllerParameterType.Float: return "📊";
            case AnimatorControllerParameterType.Int: return "🔢";
            case AnimatorControllerParameterType.Trigger: return "⚡";
            default: return "❓";
        }
    }
    
    /// <summary>
    /// ✅ ENHANCED: Safe parameter checking với HasParameter() method
    /// </summary>
    private bool HasParameter(string paramName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// ✅ ENHANCED: SetAnimatorParameter với comprehensive error handling và Vietnamese logs
    /// </summary>
    private void SetAnimatorParameter(string paramName, bool value)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        // ✅ THÊM CHECK PARAMETER EXISTS
        if (HasParameter(paramName))
        {
            try
            {
                animator.SetBool(paramName, value);
                // if (debugMode)
                //     Debug.Log($"🎭 [ANIMATOR] Parameter '{paramName}' được set thành {value} cho {gameObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🎭 [ANIMATOR] Lỗi khi set parameter '{paramName}': {e.Message} trên {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Parameter '{paramName}' không tồn tại trong Animator Controller của {gameObject.name}");
        }
    }
    
    /// <summary>
    /// ✅ ENHANCED: SetAnimatorParameter overload cho float values
    /// </summary>
    private void SetAnimatorParameter(string paramName, float value)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        if (HasParameter(paramName))
        {
            try
            {
                animator.SetFloat(paramName, value);
                // if (debugMode)
                //     Debug.Log($"🎭 [ANIMATOR] Parameter '{paramName}' được set thành {value:F2} cho {gameObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🎭 [ANIMATOR] Lỗi khi set parameter '{paramName}': {e.Message} trên {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Parameter '{paramName}' không tồn tại trong Animator Controller của {gameObject.name}");
        }
    }
    
    /// <summary>
    /// ✅ ENHANCED: SetAnimatorParameter overload cho integer values
    /// </summary>
    private void SetAnimatorParameter(string paramName, int value)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        if (HasParameter(paramName))
        {
            try
            {
                animator.SetInteger(paramName, value);
                // if (debugMode)
                //     Debug.Log($"🎭 [ANIMATOR] Parameter '{paramName}' được set thành {value} cho {gameObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🎭 [ANIMATOR] Lỗi khi set parameter '{paramName}': {e.Message} trên {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Parameter '{paramName}' không tồn tại trong Animator Controller của {gameObject.name}");
        }
    }
    
    /// <summary>
    /// ✅ ENHANCED: SetAnimatorTrigger với safe parameter checking
    /// </summary>
    private void SetAnimatorTrigger(string paramName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        if (HasParameter(paramName))
        {
            try
            {
                animator.SetTrigger(paramName);
                // if (debugMode)
                //     Debug.Log($"🎭 [ANIMATOR] Trigger '{paramName}' được kích hoạt cho {gameObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🎭 [ANIMATOR] Lỗi khi set trigger '{paramName}': {e.Message} trên {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"🎭 [ANIMATOR] Trigger '{paramName}' không tồn tại trong Animator Controller của {gameObject.name}");
        }
    }
    
    /// <summary>
    /// ✅ LEGACY SUPPORT: Safely set animator parameter với error handling (for backward compatibility)
    /// </summary>
    private void SetAnimatorParameterSafely(string paramName, object value)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        // Sử dụng các method mới
        switch (value)
        {
            case bool boolValue:
                SetAnimatorParameter(paramName, boolValue);
                break;
            case float floatValue:
                SetAnimatorParameter(paramName, floatValue);
                break;
            case int intValue:
                SetAnimatorParameter(paramName, intValue);
                break;
            default:
                Debug.LogWarning($"🎭 [ANIMATOR] Unsupported parameter type {value.GetType()} cho parameter '{paramName}' trên {gameObject.name}");
                break;
        }
    }
    
    
    /// <summary>
    /// ✅ FIX 5: Handle Root Motion để tránh sliding
    /// </summary>
    private void HandleRootMotion()
    {
        if (animator == null) return;
        
        if (enableRootMotion && animator.applyRootMotion && isMoving)
        {
            // Let animation control position khi Root Motion enabled
            navAgent.updatePosition = false;
            navAgent.updateRotation = true; // NavAgent vẫn handle rotation
            
            // if (debugMode)
            //     Debug.Log($"🎯 {gameObject.name} Root Motion enabled - Animation controls position");
        }
        else
        {
            // NavAgent controls both position và rotation
            navAgent.updatePosition = true;
            navAgent.updateRotation = true;
        }
    }
    
    /// <summary>
    /// Handle ragdoll recovery after being knocked down
    /// </summary>
    private void HandleRagdollRecovery()
    {
        if (ragdollController == null) return;
        
        bool isCurrentlyRagdolled = ragdollController != null && ragdollController.IsRagdollActive;
        
        // Phát hiện chuyển từ ragdoll về active
        if (wasRagdolled && !isCurrentlyRagdolled && teamMember.IsAlive)
        {
            StartCoroutine(RecoverFromRagdoll());
        }
        
        wasRagdolled = isCurrentlyRagdolled;
        
        // Disable NavMesh khi ragdoll
        if (navAgent.enabled && isCurrentlyRagdolled)
        {
            navAgent.enabled = false;
        }
    }
    
    /// <summary>
    /// Coroutine phục hồi sau ragdoll
    /// </summary>
    private IEnumerator RecoverFromRagdoll()
    {
        if (debugMode)
            Debug.Log($"{gameObject.name} recovering from ragdoll...");
        
        // Wait a moment for ragdoll to fully disable
        yield return new WaitForSeconds(0.2f);
        
        // Re-enable NavMeshAgent
        if (!navAgent.enabled)
        {
            navAgent.enabled = true;
            
            // Warp to current position để sync NavMesh
            navAgent.Warp(transform.position);
            
            // Reset state
            stateMachine.ForceChangeState(AIState.Idle);
            idleStartTime = Time.time;
            
            if (debugMode)
                Debug.Log($"{gameObject.name} recovered from ragdoll");
        }
    }
    
    /// <summary>
    /// Event handlers
    /// </summary>
    private void OnEnemyTargetChanged(TeamMember newTarget)
    {
        // 🔥 DEBUG OnTargetChanged: Thêm log khi OnTargetChanged được gọi
        Debug.Log($"🎯 [DEBUG] {gameObject.name} OnTargetChanged được gọi!");
        Debug.Log($"    - Old target: {(currentTarget != null ? currentTarget.name : "null")}");
        Debug.Log($"    - New target: {(newTarget != null ? newTarget.name : "null")}");
        Debug.Log($"    - Current state: {stateMachine.CurrentState}");
        
        currentTarget = newTarget;
        
        if (currentTarget != null)
        {
            OnTargetFound?.Invoke(currentTarget);
            
            Debug.Log($"🎯 [DEBUG] {gameObject.name} CONFIRM currentTarget được set: {currentTarget.name}");
            
            if (stateMachine.CurrentState == AIState.Idle)
            {
                stateMachine.ChangeState(AIState.Seeking);
                Debug.Log($"🔄 [DEBUG] {gameObject.name} chuyển từ Idle sang Seeking state");
            }
        }
        else
        {
            OnTargetLost?.Invoke(null);
            Debug.Log($"❌ [DEBUG] {gameObject.name} currentTarget = null");
        }
    }
    
    private void OnEnemyDetected(TeamMember enemy)
    {
        if (stateMachine.CurrentState == AIState.Idle)
        {
            stateMachine.ChangeState(AIState.Seeking);
        }
    }
    
    private void OnEnemyLost(TeamMember enemy)
    {
        if (enemy == currentTarget && !enemyDetector.HasEnemies)
        {
            currentTarget = null;
            OnTargetLost?.Invoke(enemy);
        }
    }
    
    private void OnCombatStateChanged(CombatState combatState)
    {
        isInCombat = combatState != CombatState.Idle;
        
        // Sync AI state với combat state
        if (isInCombat && stateMachine.CurrentState != AIState.Combat)
        {
            stateMachine.ChangeState(AIState.Combat);
        }
        else if (!isInCombat && stateMachine.CurrentState == AIState.Combat)
        {
            if (enemyDetector.HasEnemies)
            {
                stateMachine.ChangeState(AIState.Seeking);
            }
            else
            {
                stateMachine.ChangeState(AIState.Idle);
                idleStartTime = Time.time;
            }
        }
    }
    
    private void OnAIStateChanged(AIState previousState, AIState newState)
    {
        if (debugMode)
            Debug.Log($"{gameObject.name} AI State: {previousState} -> {newState}");
    }
    
    private void OnDeath(TeamMember deadMember)
    {
        StopMovement();
        stateMachine.ForceChangeState(AIState.Idle);
        
        if (navAgent.enabled)
        {
            navAgent.enabled = false;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe events
        if (enemyDetector != null)
        {
            enemyDetector.OnTargetChanged -= OnEnemyTargetChanged;
            enemyDetector.OnEnemyDetected -= OnEnemyDetected;
            enemyDetector.OnEnemyLost -= OnEnemyLost;
        }
        
        if (combatController != null)
        {
            combatController.OnStateChanged -= OnCombatStateChanged;
        }
        
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= OnAIStateChanged;
        }
        
        if (teamMember != null)
        {
            teamMember.OnDeath -= OnDeath;
        }
    }
    
    /// <summary>
    /// Debug visualization
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!debugMode) return;
        
        // Seek radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seekRadius);
        
        // Patrol radius
        if (enablePatrol)
        {
            Vector3 patrolCenter = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
        }
        
        // Current path
        if (Application.isPlaying && navAgent != null && navAgent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Vector3[] pathCorners = navAgent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }
        
        // Current destination
        if (Application.isPlaying && currentDestination != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentDestination, 0.5f);
            Gizmos.DrawLine(transform.position, currentDestination);
        }
        
        // Current target
        if (Application.isPlaying && currentTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            Gizmos.DrawWireSphere(currentTarget.transform.position, 1f);
        }
    }
}

/// <summary>
/// Enum cho AI behavior types
/// </summary>
public enum AIBehaviorType
{
    Aggressive,     // Luôn tấn công, chase enemy
    Defensive,      // Chỉ tấn công khi bị tấn công
    Patrol,         // Patrol area, tấn công khi phát hiện
    Guard           // Giữ position, chỉ tấn công trong range
}
