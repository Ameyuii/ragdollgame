using UnityEngine;

/// <summary>
/// Base class cho tất cả attack systems
/// Chứa logic chung cho việc tấn công
/// </summary>
public abstract class AttackSystemBase
{
    // Protected fields
    protected float lastAttackTime;
    protected Animator? animator;
    protected CharacterData? characterData;
    protected bool isAttacking;
    
    // Attack settings
    protected float baseDamage = 20f;
    protected float attackRange = 2f;
    protected float attackCooldown = 1f;
    
    // Constructor
    public AttackSystemBase(Animator? anim, CharacterData? data)
    {
        animator = anim;
        characterData = data;
          if (data != null)
        {
            baseDamage = data.baseDamage;
            attackRange = data.attackRange;
            attackCooldown = data.attackCooldown;
        }
    }
    
    /// <summary>
    /// Perform attack - phải implement trong derived classes
    /// </summary>
    public abstract void PerformAttack();
    
    /// <summary>
    /// Get attack animation trigger - phải implement trong derived classes
    /// </summary>
    public abstract string GetAttackAnimationTrigger();
    
    /// <summary>
    /// Check nếu có thể tấn công
    /// </summary>
    public virtual bool CanAttack()
    {
        return !isAttacking && (Time.time - lastAttackTime) >= attackCooldown;
    }
    
    /// <summary>
    /// Check target trong tầm tấn công
    /// </summary>
    public virtual bool IsInAttackRange(NPCBaseController target)
    {
        if (target == null || animator?.transform == null) return false;
        
        float distance = Vector3.Distance(
            animator.transform.position, 
            target.transform.position
        );
        return distance <= attackRange;
    }
    
    /// <summary>
    /// Trigger attack animation
    /// </summary>
    protected virtual void TriggerAttackAnimation(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
            Debug.Log($"🎯 Attack triggered: {trigger}");
        }
    }
    
    /// <summary>
    /// Set attack state
    /// </summary>
    protected virtual void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
        if (attacking)
        {
            lastAttackTime = Time.time;
        }
    }
    
    /// <summary>
    /// Reset attack state
    /// </summary>
    public virtual void ResetAttack()
    {
        isAttacking = false;
    }
    
    /// <summary>
    /// Get current attack cooldown remaining
    /// </summary>
    public virtual float GetCooldownRemaining()
    {
        float elapsed = Time.time - lastAttackTime;
        return Mathf.Max(0f, attackCooldown - elapsed);
    }
    
    /// <summary>
    /// Get damage value cho attack
    /// </summary>
    public virtual float GetDamage()
    {
        return baseDamage;
    }
}
