using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Current Stats")]
    public CharacterStats baseStats;
    public CharacterStats variantModifiers;
    public CharacterStats finalStats;

    [Header("Runtime Modifiers")]
    public CharacterStats temporaryModifiers;

    private void Awake()
    {
        if (baseStats == null) baseStats = new CharacterStats();
        if (variantModifiers == null) variantModifiers = new CharacterStats();
        if (temporaryModifiers == null) temporaryModifiers = new CharacterStats();
    }

    /// <summary>
    /// Apply base stats from character definition
    /// </summary>
    public void ApplyBaseStats(CharacterStats stats)
    {
        if (stats == null) return;
        baseStats = stats.Clone();
        RecalculateFinalStats();
    }

    /// <summary>
    /// Apply variant modifiers
    /// </summary>
    public void ApplyVariantModifiers(CharacterStats modifiers)
    {
        if (modifiers == null) 
        {
            variantModifiers = new CharacterStats();
        }
        else
        {
            variantModifiers = modifiers.Clone();
        }
        RecalculateFinalStats();
    }

    /// <summary>
    /// Apply final stats directly (for convenience)
    /// </summary>
    public void ApplyFinalStats(CharacterStats stats)
    {
        if (stats == null) return;
        
        baseStats = stats.Clone();
        variantModifiers = new CharacterStats();
        RecalculateFinalStats();
    }

    /// <summary>
    /// Get final calculated stats
    /// </summary>
    public CharacterStats GetFinalStats()
    {
        if (finalStats == null)
        {
            RecalculateFinalStats();
        }
        return finalStats;
    }

    /// <summary>
    /// Add temporary stat modifier (for buffs/debuffs)
    /// </summary>
    public void AddTemporaryModifier(CharacterStats modifier)
    {
        if (modifier == null) return;
        temporaryModifiers.ApplyModifiers(modifier);
        RecalculateFinalStats();
    }

    /// <summary>
    /// Clear all temporary modifiers
    /// </summary>
    public void ClearTemporaryModifiers()
    {
        temporaryModifiers = new CharacterStats();
        RecalculateFinalStats();
    }

    /// <summary>
    /// Recalculate final stats from all sources
    /// </summary>
    private void RecalculateFinalStats()
    {
        finalStats = baseStats.Clone();
        
        // Apply variant modifiers
        if (variantModifiers != null)
        {
            finalStats.ApplyModifiers(variantModifiers);
        }
        
        // Apply temporary modifiers
        if (temporaryModifiers != null)
        {
            finalStats.ApplyModifiers(temporaryModifiers);
        }

        // Ensure minimum values
        finalStats.maxHealth = Mathf.Max(1f, finalStats.maxHealth);
        finalStats.moveSpeed = Mathf.Max(0f, finalStats.moveSpeed);
        finalStats.attackDamage = Mathf.Max(0f, finalStats.attackDamage);
        finalStats.attackRange = Mathf.Max(0.1f, finalStats.attackRange);
        finalStats.attackCooldown = Mathf.Max(0.1f, finalStats.attackCooldown);
    }

    /// <summary>
    /// Get stat value by name
    /// </summary>
    public float GetStatValue(string statName)
    {
        CharacterStats stats = GetFinalStats();
        
        switch (statName.ToLower())
        {
            case "maxhealth": return stats.maxHealth;
            case "attackdamage": return stats.attackDamage;
            case "attackrange": return stats.attackRange;
            case "attackcooldown": return stats.attackCooldown;
            case "criticalchance": return stats.criticalChance;
            case "armor": return stats.armor;
            case "movespeed": return stats.moveSpeed;
            case "rotationspeed": return stats.rotationSpeed;
            case "jumpheight": return stats.jumpHeight;
            case "mana": return stats.mana;
            case "manaregenrate": return stats.manaRegenRate;
            case "specialabilitycooldown": return stats.specialAbilityCooldown;
            default:
                Debug.LogWarning($"Unknown stat name: {statName}");
                return 0f;
        }
    }
}