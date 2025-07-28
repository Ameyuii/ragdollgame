using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Validation system for character system integrity
/// </summary>
public class CharacterSystemValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool validateOnStart = true;
    public bool logValidationResults = true;
    public bool autoFixIssues = false;

    [Header("Validation Rules")]
    public bool validateCharacterIDs = true;
    public bool validatePrefabReferences = true;
    public bool validateTeamConfigurations = true;
    public bool validateUnlockConditions = true;
    public bool validateVariants = true;

    // Validation results
    private List<ValidationIssue> validationIssues = new List<ValidationIssue>();
    private CharacterDatabase characterDatabase;
    private GameDatabase gameDatabase;

    void Start()
    {
        if (validateOnStart)
        {
            ValidateSystem();
        }
    }

    /// <summary>
    /// Validate entire character system
    /// </summary>
    [ContextMenu("Validate Character System")]
    public void ValidateSystem()
    {
        validationIssues.Clear();
        
        LoadDatabases();
        
        if (characterDatabase == null)
        {
            AddIssue(ValidationSeverity.Critical, "CharacterDatabase not found", "System cannot function without character database");
            return;
        }

        ValidateDatabase();
        ValidateCharacters();
        ValidateTeams();
        
        ReportResults();
        
        if (autoFixIssues)
        {
            FixIssues();
        }
    }

    /// <summary>
    /// Load required databases
    /// </summary>
    private void LoadDatabases()
    {
        gameDatabase = GameDatabase.Instance;
        if (gameDatabase != null)
        {
            characterDatabase = gameDatabase.characterDatabase;
        }
    }

    /// <summary>
    /// Validate database integrity
    /// </summary>
    private void ValidateDatabase()
    {
        if (characterDatabase.characters == null)
        {
            AddIssue(ValidationSeverity.Critical, "Characters list is null", "Database.characters");
            return;
        }

        if (characterDatabase.categories == null)
        {
            AddIssue(ValidationSeverity.Warning, "Categories list is null", "Database.categories");
        }

        if (characterDatabase.teams == null)
        {
            AddIssue(ValidationSeverity.Warning, "Teams list is null", "Database.teams");
        }

        // Check for duplicate character IDs
        var duplicateIDs = characterDatabase.characters
            .Where(c => c != null)
            .GroupBy(c => c.CharacterID)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (string duplicateID in duplicateIDs)
        {
            AddIssue(ValidationSeverity.Error, $"Duplicate character ID: {duplicateID}", "Database.characters");
        }
    }

    /// <summary>
    /// Validate all characters
    /// </summary>
    private void ValidateCharacters()
    {
        foreach (CharacterDefinition character in characterDatabase.characters)
        {
            if (character == null)
            {
                AddIssue(ValidationSeverity.Warning, "Null character in database", "Database.characters");
                continue;
            }

            ValidateCharacter(character);
        }
    }

    /// <summary>
    /// Validate single character
    /// </summary>
    private void ValidateCharacter(CharacterDefinition character)
    {
        string context = $"Character: {character.CharacterID}";

        // Validate character ID
        if (validateCharacterIDs)
        {
            ValidateCharacterID(character, context);
        }

        // Validate prefab references
        if (validatePrefabReferences)
        {
            ValidatePrefabReferences(character, context);
        }

        // Validate variants
        if (validateVariants)
        {
            ValidateVariants(character, context);
        }

        // Validate unlock conditions
        if (validateUnlockConditions)
        {
            ValidateUnlockConditions(character, context);
        }

        // Validate stats
        ValidateStats(character, context);
    }

    /// <summary>
    /// Validate character ID format
    /// </summary>
    private void ValidateCharacterID(CharacterDefinition character, string context)
    {
        if (string.IsNullOrEmpty(character.CharacterID))
        {
            AddIssue(ValidationSeverity.Error, "Character ID is empty", context);
            return;
        }

        string[] parts = character.CharacterID.Split('_');
        if (parts.Length != 4)
        {
            AddIssue(ValidationSeverity.Warning, 
                $"Character ID format should be 'category_type_variant_version': {character.CharacterID}", context);
        }

        if (character.CharacterID != character.CharacterID.ToLower())
        {
            AddIssue(ValidationSeverity.Warning, "Character ID should be lowercase", context);
        }
    }

    /// <summary>
    /// Validate prefab references
    /// </summary>
    private void ValidatePrefabReferences(CharacterDefinition character, string context)
    {
        if (character.BasePrefab == null)
        {
            AddIssue(ValidationSeverity.Error, "Base prefab is null", context);
        }
        else
        {
            // Check if prefab has required components
            if (character.BasePrefab.GetComponent<RagdollCharacter>() == null && 
                character.BasePrefab.GetComponent<EnhancedCharacterController>() == null)
            {
                AddIssue(ValidationSeverity.Warning, 
                    "Prefab missing character controller component", context);
            }
        }

        if (character.UIIcon == null)
        {
            AddIssue(ValidationSeverity.Warning, "UI Icon is null", context);
        }
    }

    /// <summary>
    /// Validate character variants
    /// </summary>
    private void ValidateVariants(CharacterDefinition character, string context)
    {
        if (character.Variants == null || character.Variants.Count == 0)
        {
            AddIssue(ValidationSeverity.Warning, "No variants defined", context);
            return;
        }

        bool hasDefault = character.Variants.Any(v => v.isDefault);
        if (!hasDefault)
        {
            AddIssue(ValidationSeverity.Warning, "No default variant found", context);
        }

        // Check for duplicate variant IDs
        var duplicateVariantIDs = character.Variants
            .GroupBy(v => v.variantID)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (string duplicateID in duplicateVariantIDs)
        {
            AddIssue(ValidationSeverity.Error, $"Duplicate variant ID: {duplicateID}", context);
        }

        // Validate each variant
        foreach (CharacterVariant variant in character.Variants)
        {
            if (string.IsNullOrEmpty(variant.variantID))
            {
                AddIssue(ValidationSeverity.Error, "Variant ID is empty", context);
            }

            if (string.IsNullOrEmpty(variant.variantName))
            {
                AddIssue(ValidationSeverity.Warning, $"Variant name is empty: {variant.variantID}", context);
            }
        }
    }

    /// <summary>
    /// Validate unlock conditions
    /// </summary>
    private void ValidateUnlockConditions(CharacterDefinition character, string context)
    {
        if (character.UnlockCondition == null)
        {
            AddIssue(ValidationSeverity.Info, "No unlock condition defined", context);
            return;
        }

        UnlockCondition condition = character.UnlockCondition;
        
        if (condition.unlockType == UnlockType.SpecificCharacterUnlocked)
        {
            if (string.IsNullOrEmpty(condition.requiredCharacterID))
            {
                AddIssue(ValidationSeverity.Error, "Required character ID not specified for unlock condition", context);
            }
            else if (!characterDatabase.HasCharacter(condition.requiredCharacterID))
            {
                AddIssue(ValidationSeverity.Error, 
                    $"Required character not found: {condition.requiredCharacterID}", context);
            }
        }

        if (condition.unlockType == UnlockType.PlayerLevel || 
            condition.unlockType == UnlockType.BattlesWon ||
            condition.unlockType == UnlockType.CharacterUsage)
        {
            if (condition.requiredValue <= 0)
            {
                AddIssue(ValidationSeverity.Warning, "Required value should be greater than 0", context);
            }
        }
    }

    /// <summary>
    /// Validate character stats
    /// </summary>
    private void ValidateStats(CharacterDefinition character, string context)
    {
        if (character.BaseStats == null)
        {
            AddIssue(ValidationSeverity.Error, "Base stats are null", context);
            return;
        }

        CharacterStats stats = character.BaseStats;

        if (stats.maxHealth <= 0)
        {
            AddIssue(ValidationSeverity.Error, "Max health must be greater than 0", context);
        }

        if (stats.moveSpeed < 0)
        {
            AddIssue(ValidationSeverity.Warning, "Move speed should not be negative", context);
        }

        if (stats.attackDamage < 0)
        {
            AddIssue(ValidationSeverity.Warning, "Attack damage should not be negative", context);
        }

        if (stats.attackRange < 0)
        {
            AddIssue(ValidationSeverity.Warning, "Attack range should not be negative", context);
        }

        if (stats.criticalChance < 0 || stats.criticalChance > 1)
        {
            AddIssue(ValidationSeverity.Warning, "Critical chance should be between 0 and 1", context);
        }
    }

    /// <summary>
    /// Validate team configurations
    /// </summary>
    private void ValidateTeams()
    {
        if (!validateTeamConfigurations) return;

        foreach (TeamConfiguration team in characterDatabase.teams)
        {
            if (team == null)
            {
                AddIssue(ValidationSeverity.Warning, "Null team in database", "Database.teams");
                continue;
            }

            string context = $"Team: {team.teamID}";

            if (string.IsNullOrEmpty(team.teamName))
            {
                AddIssue(ValidationSeverity.Warning, "Team name is empty", context);
            }

            if (team.teamID <= 0)
            {
                AddIssue(ValidationSeverity.Error, "Team ID must be greater than 0", context);
            }
        }

        // Check for duplicate team IDs
        var duplicateTeamIDs = characterDatabase.teams
            .Where(t => t != null)
            .GroupBy(t => t.teamID)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (int duplicateID in duplicateTeamIDs)
        {
            AddIssue(ValidationSeverity.Error, $"Duplicate team ID: {duplicateID}", "Database.teams");
        }
    }

    /// <summary>
    /// Add validation issue
    /// </summary>
    private void AddIssue(ValidationSeverity severity, string message, string context)
    {
        validationIssues.Add(new ValidationIssue
        {
            severity = severity,
            message = message,
            context = context
        });
    }

    /// <summary>
    /// Report validation results
    /// </summary>
    private void ReportResults()
    {
        if (!logValidationResults) return;

        int criticalCount = validationIssues.Count(i => i.severity == ValidationSeverity.Critical);
        int errorCount = validationIssues.Count(i => i.severity == ValidationSeverity.Error);
        int warningCount = validationIssues.Count(i => i.severity == ValidationSeverity.Warning);
        int infoCount = validationIssues.Count(i => i.severity == ValidationSeverity.Info);

        Debug.Log($"[CharacterSystemValidator] Validation completed: {criticalCount} critical, {errorCount} errors, {warningCount} warnings, {infoCount} info");

        foreach (ValidationIssue issue in validationIssues)
        {
            string logMessage = $"[{issue.severity}] {issue.message} ({issue.context})";
            
            switch (issue.severity)
            {
                case ValidationSeverity.Critical:
                case ValidationSeverity.Error:
                    Debug.LogError(logMessage);
                    break;
                case ValidationSeverity.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case ValidationSeverity.Info:
                    Debug.Log(logMessage);
                    break;
            }
        }
    }

    /// <summary>
    /// Attempt to fix validation issues automatically
    /// </summary>
    private void FixIssues()
    {
        int fixedCount = 0;

        foreach (ValidationIssue issue in validationIssues)
        {
            if (TryFixIssue(issue))
            {
                fixedCount++;
            }
        }

        if (fixedCount > 0)
        {
            Debug.Log($"[CharacterSystemValidator] Auto-fixed {fixedCount} issues");
            
            // Mark database as dirty for saving
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(characterDatabase);
            #endif
        }
    }

    /// <summary>
    /// Try to fix a specific issue
    /// </summary>
    private bool TryFixIssue(ValidationIssue issue)
    {
        // Implement auto-fix logic for common issues
        if (issue.message.Contains("No default variant found"))
        {
            // Find character and add default variant
            string characterID = ExtractCharacterIDFromContext(issue.context);
            CharacterDefinition character = characterDatabase.GetCharacter(characterID);
            
            if (character != null && character.Variants.Count > 0)
            {
                character.Variants[0].isDefault = true;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Extract character ID from context string
    /// </summary>
    private string ExtractCharacterIDFromContext(string context)
    {
        if (context.StartsWith("Character: "))
        {
            return context.Substring("Character: ".Length);
        }
        return "";
    }

    /// <summary>
    /// Get validation summary
    /// </summary>
    public ValidationSummary GetValidationSummary()
    {
        return new ValidationSummary
        {
            totalIssues = validationIssues.Count,
            criticalCount = validationIssues.Count(i => i.severity == ValidationSeverity.Critical),
            errorCount = validationIssues.Count(i => i.severity == ValidationSeverity.Error),
            warningCount = validationIssues.Count(i => i.severity == ValidationSeverity.Warning),
            infoCount = validationIssues.Count(i => i.severity == ValidationSeverity.Info),
            issues = validationIssues.ToList()
        };
    }
}

/// <summary>
/// Validation issue data
/// </summary>
[System.Serializable]
public class ValidationIssue
{
    public ValidationSeverity severity;
    public string message;
    public string context;
}

/// <summary>
/// Validation severity levels
/// </summary>
public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Validation summary
/// </summary>
[System.Serializable]
public class ValidationSummary
{
    public int totalIssues;
    public int criticalCount;
    public int errorCount;
    public int warningCount;
    public int infoCount;
    public List<ValidationIssue> issues;
}
