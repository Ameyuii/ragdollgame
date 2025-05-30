using UnityEngine;
using NPCSystem.Core.Skills;

namespace NPCSystem.Characters.Warrior
{
    /// <summary>
    /// Charge skill cho Warrior - lao về phía target và gây damage
    /// </summary>
    public class ChargeSkill : SkillBase
    {
        [Header("Charge Skill Settings")]
        [SerializeField] private float chargeSpeed = 15f;
        [SerializeField] private float chargeDuration = 1f;
        [SerializeField] private GameObject chargeEffect;
        
        private bool isCharging = false;
        private Vector3 chargeDirection;
        private float chargeStartTime;
        
        protected override void Start()
        {
            base.Start();
            skillName = "Warrior Charge";
            cooldown = 8f;
            range = 10f;
            damage = 50f;
        }
        
        protected override void ExecuteSkill(Transform target)
        {
            if (!IsTargetInRange(target) || isCharging) return;
            
            // Bắt đầu charge
            isCharging = true;
            chargeStartTime = Time.time;
            chargeDirection = (target.position - transform.position).normalized;
            
            Debug.Log($"{owner.name} đang charge về phía {target.name}!");
            
            // Trigger animation
            if (owner.GetComponent<Animator>() != null)
            {
                owner.GetComponent<Animator>().SetTrigger("charge");
            }
            
            // Spawn effect
            if (chargeEffect != null)
            {
                Instantiate(chargeEffect, transform.position, transform.rotation);
            }
        }
        
        private void Update()
        {
            if (isCharging)
            {
                // Di chuyển trong quá trình charge
                transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
                
                // Kiểm tra thời gian charge
                if (Time.time - chargeStartTime >= chargeDuration)
                {
                    isCharging = false;
                    Debug.Log($"{owner.name} kết thúc charge!");
                }
            }
        }
        
        /// <summary>
        /// Animation Event - gây damage khi charge hit
        /// </summary>
        public void OnChargeHit()
        {
            // Tìm tất cả enemies trong radius nhỏ
            Collider[] hitTargets = Physics.OverlapSphere(transform.position, 2f);
            
            foreach (var collider in hitTargets)
            {
                if (collider.CompareTag("Player"))
                {
                    // Gây damage cho player
                    var targetHealth = collider.GetComponent<ITargetable>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(damage, owner?.gameObject);
                        Debug.Log($"Charge hit {collider.name} gây {damage} damage!");
                    }
                }
            }
        }
        
        public override bool CanUse()
        {
            return base.CanUse() && !isCharging;
        }
    }
}
