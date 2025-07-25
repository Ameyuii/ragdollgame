using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("Combat")]
    public float maxHealth = 100f;
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float criticalChance = 0.1f;
    public float armor = 0f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 180f;
    public float jumpHeight = 1f;

    [Header("Special")]
    public float mana = 50f;
    public float manaRegenRate = 5f;
    public float specialAbilityCooldown = 10f;

    /// <summary>
    /// Creates a deep copy of this CharacterStats
    /// </summary>
    public CharacterStats Clone()
    {
        return JsonUtility.FromJson<CharacterStats>(JsonUtility.ToJson(this));
    }

    /// <summary>
    /// Apply stat modifiers from variants
    /// </summary>
    public void ApplyModifiers(CharacterStats modifiers)
    {
        maxHealth += modifiers.maxHealth;
        attackDamage += modifiers.attackDamage;
        attackRange += modifiers.attackRange;
        attackCooldown += modifiers.attackCooldown;
        criticalChance += modifiers.criticalChance;
        armor += modifiers.armor;
        moveSpeed += modifiers.moveSpeed;
        rotationSpeed += modifiers.rotationSpeed;
        jumpHeight += modifiers.jumpHeight;
        mana += modifiers.mana;
        manaRegenRate += modifiers.manaRegenRate;
        specialAbilityCooldown += modifiers.specialAbilityCooldown;
    }

    /// <summary>
    /// Get stats formatted for display
    /// </summary>
    public string GetDisplayString()
    {
        return $"Health: {maxHealth}\n" +
               $"Attack: {attackDamage}\n" +
               $"Speed: {moveSpeed}\n" +
               $"Range: {attackRange}";
    }
}