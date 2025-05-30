using UnityEngine;

/// <summary>
/// ScriptableObject chứa TOÀN BỘ STATS cấu hình cho character
/// Bao gồm tất cả thông tin từ NPCBaseController để tối ưu CharacterData system
/// </summary>
[CreateAssetMenu(fileName = "New Character Data", menuName = "NPC System/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Stats")]
    [Tooltip("Tên character")]
    public string characterName = "Unknown";
    
    [Tooltip("Máu tối đa")]
    public float maxHealth = 100f;
    
    [Tooltip("ID team")]
    public int teamId = 0;
    
    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển")]
    public float moveSpeed = 3.5f;
    
    [Tooltip("Tốc độ xoay (độ/giây)")]
    public float rotationSpeed = 120f;
    
    [Tooltip("Tốc độ tăng tốc")]
    public float acceleration = 8f;
    
    [Header("Combat Stats")]
    [Tooltip("Sát thương cơ bản")]
    public float baseDamage = 20f;
    
    [Tooltip("Thời gian hồi chiêu")]
    public float attackCooldown = 1f;
    
    [Tooltip("Tầm đánh")]
    public float attackRange = 2f;
    
    [Tooltip("Thời gian animation attack (giây)")]
    public float attackAnimationDuration = 1.0f;
    
    [Tooltip("Timing hit trong animation (0.0-1.0)")]
    [Range(0.1f, 0.9f)]
    public float attackHitTiming = 0.65f;
    
    [Header("AI Settings")]
    [Tooltip("Phạm vi phát hiện kẻ địch")]
    public float detectionRange = 15f;
    
    [Tooltip("Layer chứa kẻ địch")]
    public LayerMask enemyLayerMask = -1;
    
    [Tooltip("Layer chứa chướng ngại vật")]
    public LayerMask obstacleLayerMask = -1;
    
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
    
    [Header("Effects (Optional)")]
    [Tooltip("Hiệu ứng khi bị đánh - Optional override")]
    public GameObject hitEffect;
    
    [Tooltip("Hiệu ứng khi chết - Optional override")]
    public GameObject deathEffect;
    
    [Header("Debug Settings")]
    [Tooltip("Hiển thị thông tin debug chi tiết")]
    public bool showDebugLogs = true;
      [Header("AI Behavior (For future use)")]
    [Tooltip("Tốc độ tuần tra")]
    public float patrolSpeed = 2f;
    
    [Tooltip("Thời gian nghỉ giữa các lần tuần tra")]
    public Vector2 patrolRestTime = new Vector2(3f, 8f);    
    
    [Tooltip("Có thể tấn công khi đang tuần tra")]
    public bool canAttackWhilePatrolling = true;
      [Header("Physics Impact Settings")]
    [Tooltip("Lực đẩy khi tấn công (impact force)")]
    public float impactForce = 25f;
    
    [Tooltip("Lực knockback khi bị tấn công")]
    public float knockbackForce = 150f;
    
    [Tooltip("Lực nâng lên (upward force) khi knockback")]
    public float knockbackUpwardForce = 50f;
    
    [Tooltip("Khoảng cách tối đa để có physics impact")]
    public float maxImpactDistance = 2.5f;
    
    [Header("Ragdoll Reaction Settings")]
    [Tooltip("Có kích hoạt ragdoll mỗi khi bị đánh không")]
    public bool enableRagdollOnHit = true;
    
    [Tooltip("Cooldown giữa các lần kích hoạt ragdoll (giây)")]
    public float ragdollCooldown = 1f;
    
    [Tooltip("Sát thương tối thiểu để kích hoạt ragdoll")]
    public float minDamageForRagdoll = 10f;
    
    [Header("Ragdoll Auto Recovery")]
    [Tooltip("Thời gian tự động khôi phục từ ragdoll (0 = không tự động)")]
    public float autoRecoveryTime = 3f;
    
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
    
    /// <summary>
    /// Apply TOÀN BỘ data này vào NPCBaseController
    /// Bây giờ bao gồm tất cả fields để tối ưu CharacterData system
    /// </summary>
    public void ApplyToCharacter(NPCBaseController character)
    {
        if (character == null) return;
        
        // Basic Stats
        character.maxHealth = maxHealth;
        character.team = teamId;
        
        // Movement Settings
        character.moveSpeed = moveSpeed;
        character.rotationSpeed = rotationSpeed;
        character.acceleration = acceleration;
        
        // Combat Stats
        character.attackDamage = baseDamage;
        character.attackCooldown = attackCooldown;
        character.attackRange = attackRange;
        character.attackAnimationDuration = attackAnimationDuration;
        character.attackHitTiming = attackHitTiming;
        
        // AI Settings
        character.detectionRange = detectionRange;
        character.enemyLayerMask = enemyLayerMask;
        character.obstacleLayerMask = obstacleLayerMask;
        
        // Attack Variation Settings
        character.basicAttackChance = basicAttackChance;
        character.attack1Chance = attack1Chance;
        character.attack2Chance = attack2Chance;
        
        // Advanced Attack Settings
        character.useVariableAttackCooldown = useVariableAttackCooldown;
        character.attack1Cooldown = attack1Cooldown;
        character.attack2Cooldown = attack2Cooldown;
        
        // Effects (nếu có)
        if (hitEffect != null)
            character.hitEffect = hitEffect;
        if (deathEffect != null)
            character.deathEffect = deathEffect;        // Debug Settings
        character.showDebugLogs = showDebugLogs;
        
        // Physics Impact Settings
        character.impactForce = impactForce;
        character.knockbackForce = knockbackForce;
        character.knockbackUpwardForce = knockbackUpwardForce;
        character.maxImpactDistance = maxImpactDistance;
          // Ragdoll Reaction Settings
        character.enableRagdollOnHit = enableRagdollOnHit;
        character.ragdollCooldown = ragdollCooldown;
        character.minDamageForRagdoll = minDamageForRagdoll;
        
        // Cập nhật RagdollController nếu có
        var ragdollController = character.GetComponent<RagdollController>();
        if (ragdollController != null)
        {
            // Set auto recovery time thông qua reflection hoặc public property
            var autoRecoveryField = ragdollController.GetType().GetField("autoRecoveryTime", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (autoRecoveryField != null)
            {
                autoRecoveryField.SetValue(ragdollController, autoRecoveryTime);
            }
        }
        
        // Hit Reaction Settings
        character.ragdollForceThreshold = ragdollForceThreshold;
        character.canCounterAttack = canCounterAttack;
        character.counterAttackCooldown = counterAttackCooldown;
        character.counterAttackDamageMultiplier = counterAttackDamageMultiplier;
        
        Debug.Log($"✅ Applied COMPLETE {characterName} data to {character.gameObject.name} - {maxHealth}HP, {baseDamage}DMG, Team{teamId}, Ragdoll Recovery:{autoRecoveryTime}s");
    }
}
