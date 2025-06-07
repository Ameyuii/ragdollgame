using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component phát hiện kẻ địch trong tầm nhìn
/// Sử dụng sphere detection và raycast để check line of sight
/// </summary>
public class EnemyDetector : MonoBehaviour
{    [Header("Detection Settings")]    [SerializeField] private float detectionRadius = 35f; // 🔧 TĂNG THÊM DETECTION RANGE
    [SerializeField] private float detectionAngle = 150f; // 🔧 TĂNG DETECTION ANGLE
    [SerializeField] private LayerMask detectionMask = -1;
    [SerializeField] private LayerMask obstacleMask = 1; // Default layer
    
    [Header("Performance")]
    [SerializeField] private float updateInterval = 0.05f; // 🔧 TĂNG TẦN SUẤT UPDATE cho close combat
    [SerializeField] private int maxTargets = 5;
    [SerializeField] private bool debugMode = true;
    
    [Header("Components")]
    [SerializeField] private TeamMember myTeam;
    [SerializeField] private Transform eyePosition;
    
    // Private variables
    private List<TeamMember> detectedEnemies = new List<TeamMember>();
    private List<TeamMember> allNearbyUnits = new List<TeamMember>();
    private TeamMember currentTarget;
    private float lastUpdateTime;
    
    // Events
    public System.Action<TeamMember> OnEnemyDetected;
    public System.Action<TeamMember> OnEnemyLost;
    public System.Action<TeamMember> OnTargetChanged;
    
    // Properties
    public List<TeamMember> DetectedEnemies => detectedEnemies;
    public TeamMember CurrentTarget => currentTarget;
    public bool HasEnemies => detectedEnemies.Count > 0;
    public float DetectionRadius => detectionRadius;
    
    private void Awake()
    {
        // 🛠️ CRITICAL FIX: Force DetectionMask ngay trong Awake()
        if (detectionMask == 0)
        {
            detectionMask = -1; // Everything layer
            Debug.LogWarning($"🔧 [AWAKE FIX] DetectionMask was 0, forced to -1 for {gameObject.name}");
        }
        
        // Auto-find components
        if (myTeam == null)
            myTeam = GetComponent<TeamMember>();
            
        if (eyePosition == null)
            eyePosition = transform;
    }
    
    private void Start()
    {
        // 🔥 FORCE DEBUG: Always log initialization
        // Debug.Log($"[EnemyDetector.Start] INITIALIZING {gameObject.name}"); // TẮT
        
        // 🛠️ FIX CRITICAL: Force DetectionMask if it's 0
        if (detectionMask == 0)
        {
            detectionMask = -1; // Everything layer
            Debug.LogWarning($"[EnemyDetector] DetectionMask was 0, setting to Everything for {gameObject.name}");
        }
        
        if (myTeam == null)
        {
            Debug.LogError($"EnemyDetector on {gameObject.name} requires TeamMember component!");
            enabled = false;
            return;
        }
          // Debug.Log($"[EnemyDetector.Start] READY for {gameObject.name} (Team: {myTeam.TeamName})"); // TẮT
        // Debug.Log($"[EnemyDetector.Start] DetectionMask validated: {detectionMask.value}"); // TẮT
        
        // Force immediate detection test
        Invoke(nameof(ForceDetectionTest), 1f);
    }
    
    /// <summary>
    /// Force detection test để validate enhanced debug
    /// </summary>
    private void ForceDetectionTest()
    {
        // Debug.Log($"[EnemyDetector.ForceTest] Running detection test for {gameObject.name}"); // TẮT
        UpdateDetection();
    }
      private void Update()
    {
        // Update detection với interval để tối ưu performance
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            // Debug.Log($"🔄 [ENEMY DETECTOR] {gameObject.name} Update() chạy - scanning for enemies..."); // TẮT
            UpdateDetection();
            lastUpdateTime = Time.time;
        }
    }
    
    /// <summary>
    /// Update detection logic chính - Enhanced với better enemy finding
    /// </summary>
    private void UpdateDetection()
    {
        if (!myTeam.IsAlive) return;
        
        // 🛠️ RUNTIME FIX: Double check DetectionMask mỗi lần update
        if (detectionMask == 0)
        {
            detectionMask = -1; // Everything layer
            Debug.LogError($"🚨 [RUNTIME FIX] DetectionMask was still 0 during update! Fixed to -1 for {gameObject.name}");
        }
        
        // 🔥 DEBUG TARGET DETECTION: Thêm log để confirm có detect enemy không        // Debug.Log($"🔍 [DEBUG] {gameObject.name} BẮT ĐẦU QUÉT ENEMY"); // TẮT
        // Debug.Log($"🔍 [DEBUG] Team: {myTeam?.TeamType}, Radius: {detectionRadius}m, Position: {transform.position}"); // TẮT
        // Debug.Log($"🔍 [DEBUG] DetectionMask: {detectionMask.value}"); // TẮT
        
        // Clear danh sách cũ
        allNearbyUnits.Clear();
        List<TeamMember> previousEnemies = new List<TeamMember>(detectedEnemies);
        detectedEnemies.Clear();
        
        // Tìm tất cả units trong radius với LayerMask tối ưu
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);
        
        // 🔥 DEBUG: Always log found objects count
        // Debug.Log($"🎯 [DEBUG] {gameObject.name} TÌM THẤY {nearbyColliders.Length} đối tượng trong phạm vi {detectionRadius}m"); // TẮT
        
        foreach (Collider col in nearbyColliders)
        {
            TeamMember unit = col.GetComponent<TeamMember>();
            // Debug.Log($"🔍 [DEBUG] {gameObject.name} kiểm tra: {col.name}, Có TeamMember: {unit != null}"); // TẮT
            
            if (unit != null && unit != myTeam && unit.IsAlive)
            {
                allNearbyUnits.Add(unit);
                
                // 🔥 ENHANCED DEBUG: Detailed team checking với tiếng Việt
                bool isEnemyResult = myTeam.IsEnemy(unit);
                float distance = Vector3.Distance(transform.position, col.transform.position);                // Debug.Log($"🔍 [DEBUG] {gameObject.name} phân tích: {unit.name}"); // TẮT
                // Debug.Log($"    - Team tôi: {myTeam.TeamType}, Team họ: {unit.TeamType}"); // TẮT
                // Debug.Log($"    - Là địch: {isEnemyResult}, Khoảng cách: {distance:F2}m"); // TẮT
                
                // Enhanced enemy checking với better team detection
                if (isEnemyResult)
                {
                    // Debug.Log($"⚡ ENEMY FOUND! {gameObject.name} XÁC NHẬN ĐỊCH: {unit.gameObject.name}"); // TẮT để giảm spam log
                        
                    float distanceToUnit = Vector3.Distance(transform.position, unit.transform.position);
                    
                    // Relaxed detection - chỉ cần trong radius và line of sight
                    // Bỏ angle restriction để AI có thể detect 360 độ
                    if (HasLineOfSight(unit))
                    {
                        detectedEnemies.Add(unit);
                        
                        // Trigger event nếu enemy mới
                        if (!previousEnemies.Contains(unit))
                        {
                            OnEnemyDetected?.Invoke(unit);
                            Debug.Log($"🎯 [ENEMY DETECTION] {gameObject.name} PHÁT HIỆN ĐỊCH MỚI: {unit.gameObject.name} (Team: {unit.TeamType}) cách {distanceToUnit:F1}m"); // GIỮ LẠI log quan trọng
                        }
                    }
                    else
                    {
                        // Debug.Log($"🚫 {gameObject.name} bị chặn tầm nhìn đến {unit.gameObject.name}"); // TẮT
                    }
                }
                else
                {
                    // Debug.Log($"❌ {gameObject.name} KHÔNG PHẢI ĐỊCH: {unit.gameObject.name} (Cùng team hoặc trung lập)"); // TẮT
                }
            }
            else if (unit != null)
            {
                Debug.Log($"🔍 [EnemyDetector] Bỏ qua {unit.name}: Cùng unit={unit == myTeam}, Sống={unit.IsAlive}");
            }
        }
        
        // Check enemies bị mất
        foreach (TeamMember enemy in previousEnemies)
        {
            if (!detectedEnemies.Contains(enemy))
            {
                OnEnemyLost?.Invoke(enemy);
                
                if (debugMode)
                    Debug.Log($"❌ {gameObject.name} lost enemy: {enemy.gameObject.name}");
            }
        }
        
        // Update target
        UpdateCurrentTarget();
        
        // 🔥 ENHANCED DEBUG: Báo cáo kết quả quét với tiếng Việt
        if (detectedEnemies.Count > 0)
        {
            Debug.Log($"⚔️ KẾT QUẢ QUÉT: {gameObject.name} đang theo dõi {detectedEnemies.Count} địch");
            string enemyList = "";
            foreach (var enemy in detectedEnemies)
            {
                enemyList += $"{enemy.name} ";
            }
            Debug.Log($"📋 DANH SÁCH ĐỊCH: {enemyList}");
        }
        else
        {
            Debug.Log($"🔍 KẾT QUẢ QUÉT: {gameObject.name} KHÔNG TÌM THẤY địch nào");
        }
        
        // Limit số lượng detected enemies
        if (detectedEnemies.Count > maxTargets)
        {
            detectedEnemies.RemoveRange(maxTargets, detectedEnemies.Count - maxTargets);
        }
    }
    
    /// <summary>
    /// Kiểm tra unit có trong góc phát hiện không - Enhanced với 360° detection
    /// </summary>
    private bool IsInDetectionAngle(TeamMember unit)
    {
        // Nếu detectionAngle >= 360, detect mọi hướng
        if (detectionAngle >= 360f)
            return true;
            
        Vector3 directionToUnit = (unit.transform.position - eyePosition.position).normalized;
        Vector3 forward = eyePosition.forward;
        
        float angle = Vector3.Angle(forward, directionToUnit);
        bool inAngle = angle <= detectionAngle * 0.5f;
        
        if (debugMode && !inAngle)
        {
            Debug.Log($"🔍 {gameObject.name} - {unit.gameObject.name} outside detection angle: {angle:F1}° (max: {detectionAngle * 0.5f:F1}°)");
        }
        
        return inAngle;
    }
    
    /// <summary>
    /// Kiểm tra line of sight đến unit
    /// </summary>
    private bool HasLineOfSight(TeamMember unit)
    {
        Vector3 directionToUnit = unit.transform.position - eyePosition.position;
        float distanceToUnit = directionToUnit.magnitude;
        
        // Raycast từ eye position đến unit
        if (Physics.Raycast(eyePosition.position, directionToUnit.normalized, out RaycastHit hit, distanceToUnit, obstacleMask))
        {
            // Nếu hit something trước khi đến unit = bị chặn
            return hit.collider.GetComponent<TeamMember>() == unit;
        }
        
        return true; // Không hit obstacle = có line of sight
    }
    
    /// <summary>
    /// Update current target dựa trên priority
    /// </summary>
    private void UpdateCurrentTarget()
    {
        TeamMember newTarget = GetBestTarget();
        
        if (newTarget != currentTarget)
        {
            TeamMember previousTarget = currentTarget;
            currentTarget = newTarget;
            
            OnTargetChanged?.Invoke(currentTarget);
            
            if (debugMode)
            {
                if (currentTarget != null)
                    Debug.Log($"{gameObject.name} targeting: {currentTarget.gameObject.name}");
                else
                    Debug.Log($"{gameObject.name} lost target");
            }
        }
    }
    
    /// <summary>
    /// Lấy target tốt nhất dựa trên criteria
    /// </summary>
    private TeamMember GetBestTarget()
    {
        if (detectedEnemies.Count == 0) return null;
        
        TeamMember bestTarget = null;
        float bestScore = -1f;
        
        foreach (TeamMember enemy in detectedEnemies)
        {
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
    /// Tính score cho target selection
    /// </summary>
    private float CalculateTargetScore(TeamMember enemy)
    {
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        float maxDistance = detectionRadius;
        
        // Score dựa trên: khoảng cách (gần hơn = tốt hơn), health thấp = priority cao
        float distanceScore = 1f - (distance / maxDistance); // 0-1, gần = cao
        float healthScore = 1f - enemy.HealthPercent; // 0-1, ít máu = cao
        
        // Bonus nếu đang target enemy này rồi (target stability)
        float stabilityBonus = (currentTarget == enemy) ? 0.2f : 0f;
        
        return distanceScore * 0.6f + healthScore * 0.3f + stabilityBonus;
    }
    
    /// <summary>
    /// Force target một enemy cụ thể
    /// </summary>
    public void SetTarget(TeamMember target)
    {
        if (target != null && detectedEnemies.Contains(target))
        {
            currentTarget = target;
            OnTargetChanged?.Invoke(currentTarget);
            
            if (debugMode)
                Debug.Log($"{gameObject.name} forced target: {target.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Clear current target
    /// </summary>
    public void ClearTarget()
    {
        currentTarget = null;
        OnTargetChanged?.Invoke(null);
    }
    
    /// <summary>
    /// Lấy enemy gần nhất
    /// </summary>
    public TeamMember GetClosestEnemy()
    {
        if (detectedEnemies.Count == 0) return null;
        
        TeamMember closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (TeamMember enemy in detectedEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }
        
        return closest;
    }
    
    /// <summary>
    /// Lấy tất cả enemies trong team cụ thể
    /// </summary>
    public List<TeamMember> GetEnemiesInTeam(TeamType targetTeam)
    {
        List<TeamMember> teamEnemies = new List<TeamMember>();
        
        foreach (TeamMember enemy in detectedEnemies)
        {
            if (enemy != null && enemy.IsAlive && enemy.TeamType == targetTeam)
            {
                teamEnemies.Add(enemy);
            }
        }
        
        return teamEnemies;
    }
    
    /// <summary>
    /// Kiểm tra có enemy trong team cụ thể không
    /// </summary>
    public bool HasEnemiesInTeam(TeamType targetTeam)
    {
        foreach (TeamMember enemy in detectedEnemies)
        {
            if (enemy != null && enemy.IsAlive && enemy.TeamType == targetTeam)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Lấy enemy yếu nhất (ít máu nhất)
    /// </summary>
    public TeamMember GetWeakestEnemy()
    {
        if (detectedEnemies.Count == 0) return null;
        
        TeamMember weakest = null;
        float lowestHealthPercent = 1f;
        
        foreach (TeamMember enemy in detectedEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            
            if (enemy.HealthPercent < lowestHealthPercent)
            {
                lowestHealthPercent = enemy.HealthPercent;
                weakest = enemy;
            }
        }
        
        return weakest;
    }
    
    /// <summary>
    /// Debug visualization
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!debugMode) return;
        
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Detection angle
        Vector3 forward = eyePosition != null ? eyePosition.forward : transform.forward;
        Vector3 leftBoundary = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.up) * forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.up) * forward;
        
        Gizmos.color = Color.blue;
        Vector3 eyePos = eyePosition != null ? eyePosition.position : transform.position;
        Gizmos.DrawRay(eyePos, leftBoundary * detectionRadius);
        Gizmos.DrawRay(eyePos, rightBoundary * detectionRadius);
        
        // Detected enemies
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            foreach (TeamMember enemy in detectedEnemies)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(eyePos, enemy.transform.position);
                    Gizmos.DrawWireSphere(enemy.transform.position, 0.5f);
                }
            }
            
            // Current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(currentTarget.transform.position, 1f);
            }
        }
    }
}