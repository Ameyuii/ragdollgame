using UnityEngine;

[System.Serializable]
public class UnlockCondition
{
    [Header("Unlock Settings")]
    public UnlockType unlockType;
    public int requiredValue;
    public string requiredCharacterID;
    public bool isUnlocked = false;

    [Header("Description")]
    [TextArea(2, 3)]
    public string description;

    /// <summary>
    /// Check if this unlock condition is met
    /// </summary>
    public bool IsUnlocked()
    {
        if (isUnlocked) return true;

        switch (unlockType)
        {
            case UnlockType.AlwaysUnlocked:
                return true;

            case UnlockType.PlayerLevel:
                // TODO: Implement player level check when PlayerProgressManager is ready
                return true; // Temporarily unlock all for development

            case UnlockType.CharacterUsage:
                // TODO: Implement character usage check when PlayerProgressManager is ready
                return true; // Temporarily unlock all for development

            case UnlockType.BattlesWon:
                // TODO: Implement battles won check when PlayerProgressManager is ready
                return true; // Temporarily unlock all for development

            case UnlockType.SpecificCharacterUnlocked:
                // TODO: Implement specific character unlock check when PlayerProgressManager is ready
                return true; // Temporarily unlock all for development

            case UnlockType.Achievement:
                // TODO: Implement achievement check when PlayerProgressManager is ready
                return true; // Temporarily unlock all for development

            default:
                return false;
        }
    }

    /// <summary>
    /// Get unlock requirement description
    /// </summary>
    public string GetRequirementDescription()
    {
        if (!string.IsNullOrEmpty(description))
            return description;

        switch (unlockType)
        {
            case UnlockType.AlwaysUnlocked:
                return "Available";

            case UnlockType.PlayerLevel:
                return $"Reach Player Level {requiredValue}";

            case UnlockType.CharacterUsage:
                return $"Use {requiredCharacterID} {requiredValue} times";

            case UnlockType.BattlesWon:
                return $"Win {requiredValue} battles";

            case UnlockType.SpecificCharacterUnlocked:
                return $"Unlock {requiredCharacterID} first";

            case UnlockType.Achievement:
                return $"Complete achievement: {requiredCharacterID}";

            default:
                return "Unknown requirement";
        }
    }
}

public enum UnlockType
{
    AlwaysUnlocked,
    PlayerLevel,
    CharacterUsage,
    BattlesWon,
    SpecificCharacterUnlocked,
    Achievement
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}