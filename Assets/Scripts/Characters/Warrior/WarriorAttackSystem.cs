using UnityEngine;

namespace NPCSystem.Characters.Warrior
{
    /// <summary>
    /// Attack system cho Warrior - chuyên về melee attacks
    /// </summary>
    public class WarriorAttackSystem : AttackSystemBase
    {
        // Warrior attack triggers
        private readonly string[] warriorAttackTriggers = { "Attack", "Attack1", "Attack2" };
        
        // Tỷ lệ cho các loại tấn công của Warrior
        private readonly float[] attackPercentages = { 40f, 30f, 30f }; // Basic, Heavy, Combo
        
        public WarriorAttackSystem(Animator? animator, CharacterData? data) : base(animator, data)
        {
        }
        
        public override void PerformAttack()
        {
            if (!CanAttack()) return;
            
            SetAttacking(true);
            
            // Chọn random attack cho warrior
            string attackTrigger = GetRandomWarriorAttack();
            
            Debug.Log($"Warrior đang thực hiện {attackTrigger}!");
            
            // Trigger animation
            TriggerAttackAnimation(attackTrigger);
            
            // Set cooldown dựa trên loại attack
            SetWarriorCooldown(attackTrigger);
        }
        
        public override string GetAttackAnimationTrigger()
        {
            return GetRandomWarriorAttack();
        }
        
        /// <summary>
        /// Chọn random warrior attack dựa trên tỷ lệ
        /// </summary>
        private string GetRandomWarriorAttack()
        {
            float randomValue = Random.Range(0f, 100f);
            float cumulativePercentage = 0f;
            
            for (int i = 0; i < attackPercentages.Length; i++)
            {
                cumulativePercentage += attackPercentages[i];
                if (randomValue <= cumulativePercentage)
                {
                    return warriorAttackTriggers[i];
                }
            }
            
            // Fallback về attack cơ bản
            return warriorAttackTriggers[0];
        }
        
        /// <summary>
        /// Set cooldown khác nhau cho từng loại attack
        /// </summary>
        private void SetWarriorCooldown(string attackType)
        {
            switch (attackType)
            {
                case "attack":
                    attackCooldown = 1.5f; // Basic attack nhanh nhất
                    break;
                case "attack1":
                    attackCooldown = 2.5f; // Heavy attack chậm hơn
                    break;
                case "attack2":
                    attackCooldown = 3.0f; // Combo attack chậm nhất
                    break;
                default:
                    attackCooldown = characterData?.attackCooldown ?? 2.0f;
                    break;
            }
        }
        
        /// <summary>
        /// Get attack damage dựa trên loại attack
        /// </summary>
        public float GetAttackDamage(string attackType)
        {
            float baseAttackDamage = characterData?.baseDamage ?? 30f;
            
            switch (attackType)
            {
                case "attack":
                    return baseAttackDamage * 1.0f; // Damage chuẩn
                case "attack1":
                    return baseAttackDamage * 1.5f; // Heavy attack damage cao hơn
                case "attack2":
                    return baseAttackDamage * 1.2f; // Combo attack damage trung bình
                default:
                    return baseAttackDamage;
            }
        }
        
        /// <summary>
        /// Check xem có thể thực hiện heavy attack không
        /// </summary>
        public bool CanPerformHeavyAttack()
        {
            return CanAttack(); // Có thể mở rộng thêm conditions
        }
        
        /// <summary>
        /// Check xem có thể thực hiện combo attack không
        /// </summary>
        public bool CanPerformComboAttack()
        {
            return CanAttack(); // Có thể mở rộng thêm conditions
        }
    }
}
