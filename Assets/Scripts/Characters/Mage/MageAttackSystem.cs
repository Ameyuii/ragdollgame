using UnityEngine;

namespace NPCSystem.Characters.Mage
{
    /// <summary>
    /// Attack system cho Mage - chuyên về magic attacks
    /// </summary>
    public class MageAttackSystem : AttackSystemBase
    {
        // Mage attack triggers
        private readonly string[] mageAttackTriggers = { "fireball", "lightning", "ice_shard" };
        
        // Tỷ lệ cho các loại tấn công của Mage
        private readonly float[] attackPercentages = { 40f, 35f, 25f }; // Fireball, Lightning, Ice Shard
          public MageAttackSystem(Animator? animator, CharacterData? data) : base(animator, data)
        {
        }
        
        public override void PerformAttack()
        {
            if (!CanAttack()) return;
            
            SetAttacking(true);
            
            // Chọn random attack cho mage
            string attackTrigger = GetRandomMagicAttack();
            
            Debug.Log($"Mage đang cast {attackTrigger}!");
            
            // Trigger animation
            TriggerAttackAnimation(attackTrigger);
            
            // Set cooldown dựa trên loại spell
            SetSpellCooldown(attackTrigger);
        }
        
        public override string GetAttackAnimationTrigger()
        {
            return GetRandomMagicAttack();
        }
        
        /// <summary>
        /// Chọn random magic attack dựa trên tỷ lệ
        /// </summary>
        private string GetRandomMagicAttack()
        {
            float randomValue = Random.Range(0f, 100f);
            float cumulativePercentage = 0f;
            
            for (int i = 0; i < attackPercentages.Length; i++)
            {
                cumulativePercentage += attackPercentages[i];
                if (randomValue <= cumulativePercentage)
                {
                    return mageAttackTriggers[i];
                }
            }
            
            // Fallback về fireball
            return mageAttackTriggers[0];
        }
        
        /// <summary>
        /// Set cooldown khác nhau cho từng loại spell
        /// </summary>
        private void SetSpellCooldown(string spellType)
        {
            switch (spellType)
            {
                case "fireball":
                    attackCooldown = 2.0f; // Fireball nhanh nhất
                    break;
                case "lightning":
                    attackCooldown = 2.5f; // Lightning trung bình
                    break;
                case "ice_shard":
                    attackCooldown = 3.0f; // Ice shard chậm nhất nhưng mạnh
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
        /// Kiểm tra spell có sẵn sàng cast không
        /// </summary>
        public bool IsSpellReady(string spellType)
        {
            // Có thể mở rộng để check mana, spell components, etc.
            return CanAttack();
        }
        
        /// <summary>
        /// Get spell damage dựa trên loại spell
        /// </summary>
        public float GetSpellDamage(string spellType)
        {
            float baseDamage = characterData?.baseDamage ?? 25f;
            
            switch (spellType)
            {
                case "fireball":
                    return baseDamage * 1.0f; // Damage chuẩn
                case "lightning":
                    return baseDamage * 1.2f; // Damage cao hơn
                case "ice_shard":
                    return baseDamage * 1.5f; // Damage cao nhất
                default:
                    return baseDamage;
            }
        }
    }
}
