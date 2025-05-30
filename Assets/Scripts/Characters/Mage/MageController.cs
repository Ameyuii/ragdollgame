using UnityEngine;

namespace NPCSystem.Characters.Mage
{    /// <summary>
    /// Controller cho NPC Mage - chuyên về tấn công tầm xa và magic
    /// </summary>
    public class MageController : NPCBaseController
    {
        [Header("Mage Specific Settings")]
        [SerializeField] private float castRange = 10f;
        [SerializeField] private Transform staffTip; // Vị trí cast spell
        [SerializeField] private GameObject[] spellEffects; // Array các effect spell
          protected override void Start()
        {
            base.Start();
            
            // Set attack range cho Mage (tầm xa)
            castRange = attackRange; // Sử dụng attackRange từ NPCBaseController
        }
        
        protected bool IsInAttackRange(Transform target)
        {
            if (target == null) return false;
            
            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= castRange;
        }
          protected void RotateTowardsTarget()
        {
            if (targetEnemy == null) return;
            
            // Mage quay chậm hơn warrior
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            direction.y = 0; // Chỉ quay theo trục Y
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            }
        }
        
        /// <summary>
        /// Method để cast spell từ staff tip
        /// </summary>
        public void CastSpell(int spellIndex)
        {
            if (staffTip != null && spellEffects != null && spellIndex < spellEffects.Length)
            {                if (spellEffects[spellIndex] != null)
                {
                    GameObject spellEffect = Instantiate(spellEffects[spellIndex], staffTip.position, staffTip.rotation);
                    
                    // Hướng spell về phía target
                    if (targetEnemy != null)
                    {
                        Vector3 direction = (targetEnemy.transform.position - staffTip.position).normalized;
                        spellEffect.transform.LookAt(targetEnemy.transform.position);
                    }
                }
            }
        }
        
        /// <summary>
        /// Animation Event - được gọi khi cast spell
        /// </summary>
        public void OnSpellCast()
        {
            // Được gọi từ animation event
            int randomSpell = Random.Range(0, spellEffects.Length);
            CastSpell(randomSpell);        }
        
        /// <summary>
        /// Kiểm tra xem có thể cast spell không
        /// </summary>
        public bool CanCastSpell()
        {
            return targetEnemy != null && IsInAttackRange(targetEnemy.transform);
        }
        
        // Override TakeDamage để custom hóa cho Mage
        public override void TakeDamage(float damage, NPCBaseController attacker)
        {
            base.TakeDamage(damage, attacker);
            
            // Mage có ít máu hơn nên phản ứng mạnh hơn khi bị tấn công
            Debug.Log($"🧙‍♂️ Mage {gameObject.name} bị tấn công! Máu còn: {currentHealth}");
        }

        // Override OnFootstep để tương thích với Animation Events
        public override void OnFootstep()
        {
            base.OnFootstep();
            Debug.Log($"🧙‍♂️ {gameObject.name}: Mage bước chân");
        }
        
        // Override OnAttackHit để tương thích với Animation Events
        public override void OnAttackHit()
        {
            base.OnAttackHit();
            Debug.Log($"🧙‍♂️ {gameObject.name}: Mage spell hit!");
        }
        
        // Test method để kiểm tra Mage
        public new void TestAttack()
        {
            Debug.Log($"🧪🧙‍♂️ {gameObject.name}: Test Mage Attack method called");
            base.TestAttack();
        }
    }
}
