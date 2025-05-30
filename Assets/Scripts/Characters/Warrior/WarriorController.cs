using UnityEngine;
using NPCSystem.Characters.Warrior;

/// <summary>
/// Warrior Controller - kế thừa từ NPCBaseController
/// Tích hợp WarriorAttackSystem và sử dụng CharacterData
/// </summary>
public class WarriorController : NPCBaseController
{
    [Header("Warrior Specific")]
    [SerializeField] private bool useWarriorSpecificAttacks = true;
    
    private WarriorAttackSystem? warriorAttackSystem;
    
    protected override void Awake()
    {
        base.Awake();
        
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: WarriorController đã được khởi tạo");
    }
    
    protected override void Start()
    {
        base.Start();
        
        // Khởi tạo WarriorAttackSystem
        InitializeWarriorSpecificSystems();
        
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: WarriorController hoàn thành khởi tạo với team {team}");
    }
    
    /// <summary>
    /// Override OnValidate để cập nhật Warrior-specific logic khi CharacterData thay đổi
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate(); // Gọi OnValidate của NPCBaseController trước
        
        #if UNITY_EDITOR
        if (!Application.isPlaying && CharacterData != null)
        {
            // Warrior-specific validation
            if (CharacterData.attackRange > 5f && showDebugLogs)
            {
                Debug.LogWarning($"🗡️ {gameObject.name}: Attack Range {CharacterData.attackRange}m có vẻ quá xa cho Warrior!");
            }
            
            if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Warrior stats đã được cập nhật từ CharacterData '{CharacterData.characterName}'");
        }
        #endif
    }
    
    /// <summary>
    /// Khởi tạo hệ thống đặc thù của Warrior
    /// </summary>
    private void InitializeWarriorSpecificSystems()
    {
        if (useWarriorSpecificAttacks && animator != null)
        {
            // Sử dụng CharacterData từ NPCBaseController
            warriorAttackSystem = new WarriorAttackSystem(animator, CharacterData);
            
            if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: WarriorAttackSystem đã được khởi tạo với CharacterData: {(CharacterData != null ? CharacterData.characterName : "NULL")}");
        }
        else
        {
            if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Không khởi tạo WarriorAttackSystem (useWarriorSpecificAttacks: {useWarriorSpecificAttacks}, animator: {animator != null})");
        }
    }    
    // Override OnFootstep để xử lý animation event riêng cho Warrior
    public override void OnFootstep()
    {
        base.OnFootstep();
        
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Warrior bước chân");
    }
    
    // Override OnAttackHit để xử lý riêng cho Warrior
    public override void OnAttackHit()
    {
        base.OnAttackHit();
        
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Warrior attack hit!");
    }    // Override TakeDamage để xử lý riêng cho Warrior  
    public override void TakeDamage(float damage, NPCBaseController attacker)
    {
        base.TakeDamage(damage, attacker);
        
        Debug.Log($"🗡️ {gameObject.name}: Warrior nhận {damage} sát thương từ {(attacker != null ? attacker.gameObject.name : "unknown")}");
    }
    
    // Override Die để xử lý riêng cho Warrior
    protected override void Die()
    {
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Warrior đã bị tiêu diệt!");
        
        base.Die();
    }
    
    // Override Attack để có thể thêm logic riêng cho Warrior
    public override void Attack(NPCBaseController target)
    {
        if (showDebugLogs) Debug.Log($"🗡️ {gameObject.name}: Warrior tấn công {target.gameObject.name}");
        
        base.Attack(target);
    }
    
    // Test method riêng cho Warrior
    [ContextMenu("Test Warrior Attack")]
    public void TestWarriorAttack()
    {
        if (showDebugLogs) Debug.Log($"🧪🗡️ {gameObject.name}: Test Warrior Attack method called");
        
        // Gọi test method của base class        TestAttack();
    }
    
    // Test method để kiểm tra hit reaction system
    [ContextMenu("🧪 Test Hit Reaction System")]
    public void TestHitReactionSystem()
    {
        Debug.Log($"=== HIT REACTION TEST - {gameObject.name} ===");
        Debug.Log($"📊 Ragdoll Force Threshold: {ragdollForceThreshold}");
        Debug.Log($"📊 Current Knockback Force: {knockbackForce}");
        Debug.Log($"📊 Current Knockback Upward: {knockbackUpwardForce}");
        Debug.Log($"📊 Total Force: {knockbackForce + knockbackUpwardForce}");
        Debug.Log($"⚔️ Can Counter Attack: {canCounterAttack}");
        Debug.Log($"⏱️ Counter Attack Cooldown: {counterAttackCooldown}s");
        Debug.Log($"💪 Counter Attack Damage Multiplier: {counterAttackDamageMultiplier}x");
        
        if (knockbackForce + knockbackUpwardForce >= ragdollForceThreshold)
        {
            Debug.Log($"💥 RESULT: Attack sẽ gây RAGDOLL (lực {knockbackForce + knockbackUpwardForce} >= {ragdollForceThreshold})");
        }
        else
        {
            Debug.Log($"🥊 RESULT: Attack sẽ gây HIT REACTION nhẹ + có thể COUNTER ATTACK");
        }
        Debug.Log("============================================");
    }

    // Debug info riêng cho Warrior
    [ContextMenu("Debug Warrior Info")]
    public void DebugWarriorInfo()
    {
        Debug.Log($"=== WARRIOR INFO - {gameObject.name} ===");
        Debug.Log($"📜 CharacterData: {(CharacterData != null ? CharacterData.characterName : "NONE")}");
        Debug.Log($"💔 Health: {currentHealth:F1}/{maxHealth}");
        Debug.Log($"⚔️ Attack Damage: {attackDamage}");
        Debug.Log($"🏃 Move Speed: {moveSpeed}");
        Debug.Log($"👥 Team: {team}");
        Debug.Log($"💀 Is Dead: {isDead}");
        Debug.Log($"🎯 Has Target: {(targetEnemy != null ? targetEnemy.gameObject.name : "None")}");
        Debug.Log($"🗡️ Warrior Attack System: {(warriorAttackSystem != null ? "ACTIVE" : "NULL")}");
        
        // Debug ragdoll components
        var ragdollController = GetComponent<RagdollController>();
        var rigidbody = GetComponent<Rigidbody>();
        Debug.Log($"🎭 RagdollController: {(ragdollController != null ? "FOUND" : "MISSING")}");
        Debug.Log($"🏗️ Rigidbody: {(rigidbody != null ? $"FOUND - Kinematic:{rigidbody.isKinematic}" : "MISSING")}");
        
        Debug.Log("=====================================");
        
        // Gọi debug method của base class
        DebugAttackSystemInfo();
    }
}
