using UnityEngine;

namespace NPCSystem.Characters.Archer
{
    /// <summary>
    /// Attack system cho Archer - chuyên về ranged attacks
    /// </summary>
    public class ArcherAttackSystem : AttackSystemBase
    {
        // Archer attack triggers
        private readonly string[] archerAttackTriggers = { "bow_shot", "power_shot", "rapid_fire" };
        
        // Tỷ lệ cho các loại tấn công của Archer
        private readonly float[] attackPercentages = { 50f, 30f, 20f }; // Bow Shot, Power Shot, Rapid Fire
          public ArcherAttackSystem(Animator animator, CharacterData data) : base(animator, data)
        {
        }
        
        public override string GetAttackAnimationTrigger()
        {
            return GetRandomArcherAttack();
        }
        
        public override void PerformAttack()
        {
            if (!CanAttack()) return;
            
            isAttacking = true;
            lastAttackTime = Time.time;
            
            // Chọn random attack cho archer
            string attackTrigger = GetRandomArcherAttack();
            
            Debug.Log($"Archer đang thực hiện {attackTrigger}!");
            
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger(attackTrigger);
            }
            
            // Set cooldown dựa trên loại attack
            SetArcherCooldown(attackTrigger);
        }
        
        /// <summary>
        /// Chọn random archer attack dựa trên tỷ lệ
        /// </summary>
        private string GetRandomArcherAttack()
        {
            float randomValue = Random.Range(0f, 100f);
            float cumulativePercentage = 0f;
            
            for (int i = 0; i < attackPercentages.Length; i++)
            {
                cumulativePercentage += attackPercentages[i];
                if (randomValue <= cumulativePercentage)
                {
                    return archerAttackTriggers[i];
                }
            }
            
            // Fallback về bow_shot
            return archerAttackTriggers[0];
        }
        
        /// <summary>
        /// Set cooldown khác nhau cho từng loại attack
        /// </summary>
        private void SetArcherCooldown(string attackType)
        {
            switch (attackType)
            {
                case "bow_shot":
                    attackCooldown = 1.5f; // Bow shot nhanh nhất
                    break;
                case "power_shot":
                    attackCooldown = 3.0f; // Power shot chậm hơn nhưng mạnh
                    break;
                case "rapid_fire":
                    attackCooldown = 4.0f; // Rapid fire chậm nhất nhưng bắn nhiều mũi tên
                    break;
                default:
                    attackCooldown = characterData?.attackCooldown ?? 2.0f;
                    break;
            }
        }
        
        public override bool CanAttack()
        {
            return base.CanAttack();
        }
        
        /// <summary>
        /// Kiểm tra có đủ mũi tên để tấn công không
        /// </summary>
        public bool HasEnoughArrows(string attackType)
        {
            // Có thể check từ ArcherController
            switch (attackType)
            {
                case "bow_shot":
                    return true; // Chỉ cần 1 mũi tên
                case "power_shot":
                    return true; // Chỉ cần 1 mũi tên
                case "rapid_fire":
                    return true; // Cần 3 mũi tên, có thể check sau
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Get attack damage dựa trên loại attack
        /// </summary>
        public float GetAttackDamage(string attackType)
        {
            float baseDamage = characterData?.baseDamage ?? 20f;
            
            switch (attackType)
            {
                case "bow_shot":
                    return baseDamage * 1.0f; // Damage chuẩn
                case "power_shot":
                    return baseDamage * 1.8f; // Damage cao hơn
                case "rapid_fire":
                    return baseDamage * 0.7f; // Damage thấp hơn nhưng bắn nhiều phát
                default:
                    return baseDamage;
            }
        }
        
        /// <summary>
        /// Get số mũi tên cần thiết cho attack
        /// </summary>
        public int GetArrowsRequired(string attackType)
        {
            switch (attackType)
            {
                case "bow_shot":
                    return 1;
                case "power_shot":
                    return 1;
                case "rapid_fire":
                    return 3; // Bắn 3 mũi tên liên tiếp
                default:
                    return 1;
            }
        }
        
        /// <summary>
        /// Tính accuracy dựa trên khoảng cách
        /// </summary>
        public float CalculateAccuracy(float distance)
        {
            // Accuracy giảm theo khoảng cách
            float maxRange = characterData?.attackRange ?? 15f;
            float accuracy = 1.0f - (distance / maxRange) * 0.3f; // Giảm tối đa 30% accuracy
            return Mathf.Clamp01(accuracy);
        }
    }
}
