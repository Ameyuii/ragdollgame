using UnityEngine;

[System.Serializable]
public class CharacterVariant
{
    [Header("Identity")]
    public string variantID;
    public string variantName;
    [TextArea(2, 3)]
    public string description;

    [Header("Stats")]
    public CharacterStats statModifiers;

    [Header("Visual Overrides")]
    public GameObject customPrefab;
    public Sprite customIcon;
    public Material[] customMaterials;
    public RuntimeAnimatorController customAnimator;

    [Header("Audio Overrides")]
    public AudioClip[] customAttackSounds;
    public AudioClip[] customHitSounds;
    public AudioClip[] customDeathSounds;

    [Header("Unlock")]
    public UnlockCondition unlockCondition;
    public bool isDefault = false;

    public CharacterVariant()
    {
        statModifiers = new CharacterStats();
        unlockCondition = new UnlockCondition();
    }

    /// <summary>
    /// Check if this variant is unlocked
    /// </summary>
    public bool IsUnlocked()
    {
        if (isDefault) return true;
        return unlockCondition?.IsUnlocked() ?? true;
    }

    /// <summary>
    /// Get display name with unlock status
    /// </summary>
    public string GetDisplayName()
    {
        return IsUnlocked() ? variantName : $"{variantName} (Locked)";
    }
}