using UnityEngine;

namespace AnimalRevolt.Characters
{
    /// <summary>
    /// ScriptableObject chứa dữ liệu của từng nhân vật
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Animal Revolt/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Basic Info")]
        public string characterName = "New Character";
        public string description = "Character description";
        public Sprite characterIcon;
        public Sprite characterPortrait;
        
        [Header("Character Model")]
        public GameObject characterPrefab;
        public RuntimeAnimatorController animatorController;
        
        [Header("Base Stats")]
        [Range(50f, 200f)]
        public float maxHealth = 100f;
        
        [Range(10f, 50f)]
        public float attackDamage = 20f;
        
        [Range(0f, 20f)]
        public float defense = 5f;
        
        [Range(1f, 10f)]
        public float moveSpeed = 5f;
        
        [Range(0.5f, 3f)]
        public float attackSpeed = 1f;
        
        [Range(1f, 10f)]
        public float jumpPower = 5f;
        
        [Header("Combat Stats")]
        [Range(0f, 1f)]
        public float criticalChance = 0.1f;
        
        [Range(1f, 3f)]
        public float criticalMultiplier = 2f;
        
        [Range(0f, 0.5f)]
        public float blockChance = 0.2f;
        
        [Range(0f, 1f)]
        public float blockDamageReduction = 0.5f;
        
        [Header("Ragdoll Physics")]
        [Range(50f, 200f)]
        public float ragdollMass = 80f;
        
        [Range(0f, 2f)]
        public float ragdollDrag = 0.5f;
        
        [Range(0f, 10f)]
        public float ragdollAngularDrag = 5f;
        
        [Range(1f, 10f)]
        public float knockbackResistance = 5f;
        
        [Range(0.1f, 5f)]
        public float recoveryTime = 2f;
        
        [Header("Special Abilities")]
        public SpecialAbility[] specialAbilities;
        
        [Header("Audio")]
        public AudioClip[] hurtSounds;
        public AudioClip[] attackSounds;
        public AudioClip[] deathSounds;
        public AudioClip[] victoryLines;
        
        [Header("Visual Effects")]
        public GameObject hitEffect;
        public GameObject deathEffect;
        public GameObject specialEffect;
        
        [Header("Character Type")]
        public CharacterType characterType = CharacterType.Balanced;
        public CharacterSize characterSize = CharacterSize.Medium;
        
        [Header("Unlock Requirements")]
        public bool isUnlockedByDefault = true;
        public int unlockCost = 0;
        public string unlockCondition = "";
        
        public enum CharacterType
        {
            Balanced,    // Cân bằng
            Tank,        // Tanky, chậm nhưng máu nhiều
            Assassin,    // Nhanh, damage cao nhưng máu ít
            Brawler      // Damage cao, tầm gần
        }
        
        public enum CharacterSize
        {
            Small,
            Medium,
            Large
        }
        
        [System.Serializable]
        public class SpecialAbility
        {
            public string abilityName;
            public string description;
            public Sprite icon;
            public float cooldown = 10f;
            public float damage = 30f;
            public float range = 2f;
            public GameObject effect;
            public AudioClip sound;
        }
        
        /// <summary>
        /// Tính toán stats dựa trên level (nếu có progression system)
        /// </summary>
        public CharacterStats GetStatsAtLevel(int level = 1)
        {
            float levelMultiplier = 1f + (level - 1) * 0.1f; // Mỗi level tăng 10%
            
            return new CharacterStats
            {
                maxHealth = maxHealth * levelMultiplier,
                currentHealth = maxHealth * levelMultiplier,
                attackDamage = attackDamage * levelMultiplier,
                defense = defense * levelMultiplier,
                moveSpeed = moveSpeed,
                attackSpeed = attackSpeed,
                jumpPower = jumpPower,
                criticalChance = criticalChance,
                criticalMultiplier = criticalMultiplier,
                blockChance = blockChance,
                blockDamageReduction = blockDamageReduction,
                ragdollMass = ragdollMass,
                ragdollDrag = ragdollDrag,
                ragdollAngularDrag = ragdollAngularDrag,
                knockbackResistance = knockbackResistance,
                recoveryTime = recoveryTime
            };
        }
        
        /// <summary>
        /// Get character stats with modifiers
        /// </summary>
        public CharacterStats GetModifiedStats(StatModifier[] modifiers = null)
        {
            CharacterStats baseStats = GetStatsAtLevel(1);
            
            if (modifiers != null)
            {
                foreach (StatModifier modifier in modifiers)
                {
                    baseStats.ApplyModifier(modifier);
                }
            }
            
            return baseStats;
        }
        
        /// <summary>
        /// Validate character data
        /// </summary>
        private void OnValidate()
        {
            // Đảm bảo các giá trị trong phạm vi hợp lệ
            maxHealth = Mathf.Max(1f, maxHealth);
            attackDamage = Mathf.Max(0f, attackDamage);
            defense = Mathf.Max(0f, defense);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            attackSpeed = Mathf.Max(0.1f, attackSpeed);
            jumpPower = Mathf.Max(0.1f, jumpPower);
            
            criticalChance = Mathf.Clamp01(criticalChance);
            blockChance = Mathf.Clamp01(blockChance);
            blockDamageReduction = Mathf.Clamp01(blockDamageReduction);
            
            ragdollMass = Mathf.Max(1f, ragdollMass);
            knockbackResistance = Mathf.Max(0.1f, knockbackResistance);
            recoveryTime = Mathf.Max(0.1f, recoveryTime);
        }
    }
    
    /// <summary>
    /// Runtime character stats
    /// </summary>
    [System.Serializable]
    public class CharacterStats
    {
        public float maxHealth;
        public float currentHealth;
        public float attackDamage;
        public float defense;
        public float moveSpeed;
        public float attackSpeed;
        public float jumpPower;
        public float criticalChance;
        public float criticalMultiplier;
        public float blockChance;
        public float blockDamageReduction;
        public float ragdollMass;
        public float ragdollDrag;
        public float ragdollAngularDrag;
        public float knockbackResistance;
        public float recoveryTime;
        
        /// <summary>
        /// Apply stat modifier
        /// </summary>
        public void ApplyModifier(StatModifier modifier)
        {
            switch (modifier.statType)
            {
                case StatType.MaxHealth:
                    maxHealth = ApplyModifierValue(maxHealth, modifier);
                    break;
                case StatType.AttackDamage:
                    attackDamage = ApplyModifierValue(attackDamage, modifier);
                    break;
                case StatType.Defense:
                    defense = ApplyModifierValue(defense, modifier);
                    break;
                case StatType.MoveSpeed:
                    moveSpeed = ApplyModifierValue(moveSpeed, modifier);
                    break;
                case StatType.AttackSpeed:
                    attackSpeed = ApplyModifierValue(attackSpeed, modifier);
                    break;
                case StatType.CriticalChance:
                    criticalChance = ApplyModifierValue(criticalChance, modifier);
                    break;
            }
        }
        
        private float ApplyModifierValue(float baseValue, StatModifier modifier)
        {
            switch (modifier.modifierType)
            {
                case ModifierType.Additive:
                    return baseValue + modifier.value;
                case ModifierType.Multiplicative:
                    return baseValue * modifier.value;
                case ModifierType.Percentage:
                    return baseValue * (1f + modifier.value / 100f);
                default:
                    return baseValue;
            }
        }
    }
    
    /// <summary>
    /// Stat modifier system
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
        public float duration = -1f; // -1 = permanent
        public string source = "";
    }
    
    public enum StatType
    {
        MaxHealth,
        AttackDamage,
        Defense,
        MoveSpeed,
        AttackSpeed,
        CriticalChance,
        BlockChance
    }
    
    public enum ModifierType
    {
        Additive,      // +X
        Multiplicative, // *X
        Percentage     // +X%
    }
}