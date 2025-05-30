using UnityEngine;

namespace NPCSystem.Core.Skills
{
    /// <summary>
    /// Base class cho tất cả skills
    /// </summary>
    public abstract class SkillBase : MonoBehaviour, ISkill
    {
        [Header("Skill Base Settings")]
        [SerializeField] protected string skillName;
        [SerializeField] protected float cooldown = 5f;
        [SerializeField] protected float range = 5f;
        [SerializeField] protected float damage = 30f;
        [SerializeField] protected bool isAvailable = true;
        
        protected float lastUsedTime = -1f;
        protected NPCBaseController owner;
        
        public string SkillName => skillName;
        public float Cooldown => cooldown;
        public float Range => range;
        public float Damage => damage;
        public bool IsAvailable => isAvailable && CanUse();
        
        protected virtual void Start()
        {
            owner = GetComponent<NPCBaseController>();
        }
        
        public virtual bool CanUse()
        {
            return isAvailable && (Time.time - lastUsedTime) >= cooldown;
        }
        
        public virtual bool CanUse(GameObject caster)
        {
            return isAvailable && (Time.time - lastUsedTime) >= cooldown;
        }
        
        public virtual void UseSkill(Transform target)
        {
            if (!CanUse()) return;
            
            lastUsedTime = Time.time;
            ExecuteSkill(target);
        }
        
        public virtual void Execute(GameObject caster, Vector3 targetPosition)
        {
            if (!CanUse(caster)) return;
            
            lastUsedTime = Time.time;
            ExecuteSkillAtPosition(caster, targetPosition);
        }
        
        public virtual void Execute(GameObject caster, GameObject target)
        {
            if (!CanUse(caster)) return;
            
            lastUsedTime = Time.time;
            ExecuteSkillOnTarget(caster, target);
        }
        
        public virtual float GetCooldown()
        {
            return cooldown;
        }
        
        public virtual float GetCost()
        {
            return 0f; // Default không có cost
        }
        
        public virtual string GetSkillName()
        {
            return skillName;
        }
        
        protected abstract void ExecuteSkill(Transform target);
        
        /// <summary>
        /// Kiểm tra target có trong range không
        /// </summary>
        protected bool IsTargetInRange(Transform target)
        {
            if (target == null || owner == null) return false;
            
            float distance = Vector3.Distance(owner.transform.position, target.position);
            return distance <= range;
        }
        
        /// <summary>
        /// Tính thời gian còn lại của cooldown
        /// </summary>
        public float GetRemainingCooldown()
        {
            if (lastUsedTime < 0) return 0f;
            
            float elapsed = Time.time - lastUsedTime;
            return Mathf.Max(0f, cooldown - elapsed);
        }
        
        /// <summary>
        /// Reset cooldown
        /// </summary>
        public virtual void ResetCooldown()
        {
            lastUsedTime = -1f;
        }
        
        // Abstract methods cho subclasses implement
        protected virtual void ExecuteSkillAtPosition(GameObject caster, Vector3 targetPosition)
        {
            // Default: không làm gì, subclass override nếu cần
        }
        
        protected virtual void ExecuteSkillOnTarget(GameObject caster, GameObject target)
        {
            // Default: gọi ExecuteSkill với target transform
            if (target != null)
                ExecuteSkill(target.transform);
        }
    }
}
