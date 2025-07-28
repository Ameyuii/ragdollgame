using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Character System/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string characterID;
    [SerializeField] private string displayName;
    [SerializeField] private string categoryID;

    [Header("Visual Assets")]
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private Sprite uiIcon;
    [SerializeField] private Sprite portraitImage;
    [SerializeField] private RuntimeAnimatorController animatorController;

    [Header("Stats")]
    [SerializeField] private CharacterStats baseStats;
    [SerializeField] private List<CharacterVariant> variants = new List<CharacterVariant>();

    // ✅ DISABLED: Team Materials - loại bỏ hoàn toàn chức năng màu team
    /*
    [Header("Team Materials")]
    [SerializeField] private List<TeamMaterialSet> teamMaterials = new List<TeamMaterialSet>();
    */

    [Header("Audio")]
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] deathSounds;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;

    [Header("Metadata")]
    [TextArea(3, 5)]
    [SerializeField] private string description;
    [SerializeField] private List<string> tags = new List<string>();
    [SerializeField] private UnlockCondition unlockCondition;
    [SerializeField] private int sortOrder;
    [SerializeField] private Rarity rarity = Rarity.Common;

    // Properties with validation
    public string CharacterID
    {
        get => characterID;
        set => characterID = ValidateCharacterID(value);
    }

    public string DisplayName
    {
        get => displayName;
        set => displayName = value;
    }

    public string CategoryID
    {
        get => categoryID;
        set => categoryID = value;
    }

    public GameObject BasePrefab => basePrefab;
    public Sprite UIIcon => uiIcon;
    public Sprite PortraitImage => portraitImage;
    public CharacterStats BaseStats => baseStats;
    public List<CharacterVariant> Variants => variants;
    public string Description => description;
    public List<string> Tags => tags;
    public UnlockCondition UnlockCondition => unlockCondition;
    public int SortOrder => sortOrder;
    public Rarity Rarity => rarity;
    public RuntimeAnimatorController AnimatorController => animatorController;
    public GameObject HitEffect => hitEffect;
    public GameObject DeathEffect => deathEffect;
    public GameObject SpawnEffect => spawnEffect;

    private void OnValidate()
    {
        // Initialize lists if null
        if (variants == null) variants = new List<CharacterVariant>();
        // ✅ DISABLED: Team materials initialization - loại bỏ hoàn toàn chức năng màu team
        // if (teamMaterials == null) teamMaterials = new List<TeamMaterialSet>();
        if (tags == null) tags = new List<string>();
        
        // Initialize stats if null
        if (baseStats == null) baseStats = new CharacterStats();
        if (unlockCondition == null) unlockCondition = new UnlockCondition();

        // Ensure we have at least one default variant
        if (variants.Count == 0)
        {
            CharacterVariant defaultVariant = new CharacterVariant
            {
                variantID = "default",
                variantName = "Default",
                description = "Default variant",
                isDefault = true
            };
            variants.Add(defaultVariant);
        }

        // Validate character ID format
        if (!string.IsNullOrEmpty(characterID))
        {
            characterID = ValidateCharacterID(characterID);
        }
    }

    /// <summary>
    /// Get final stats for a specific variant
    /// </summary>
    public CharacterStats GetFinalStats(string variantID)
    {
        CharacterStats finalStats = baseStats.Clone();

        CharacterVariant variant = variants.Find(v => v.variantID == variantID);
        if (variant != null && variant.statModifiers != null)
        {
            finalStats.ApplyModifiers(variant.statModifiers);
        }

        return finalStats;
    }

    /// <summary>
    /// DISABLED: Get team materials for a specific team - loại bỏ hoàn toàn chức năng màu team
    /// </summary>
    /*
    public Material[] GetTeamMaterials(int teamID)
    {
        TeamMaterialSet materialSet = teamMaterials.Find(t => t.teamID == teamID);
        return materialSet?.materials ?? new Material[0];
    }
    */

    /// <summary>
    /// Get variant by ID
    /// </summary>
    public CharacterVariant GetVariant(string variantID)
    {
        return variants.Find(v => v.variantID == variantID);
    }

    /// <summary>
    /// Get default variant
    /// </summary>
    public CharacterVariant GetDefaultVariant()
    {
        return variants.Find(v => v.isDefault) ?? variants.FirstOrDefault();
    }

    /// <summary>
    /// Get all unlocked variants
    /// </summary>
    public List<CharacterVariant> GetUnlockedVariants()
    {
        return variants.Where(v => v.IsUnlocked()).ToList();
    }

    /// <summary>
    /// Check if character is unlocked
    /// </summary>
    public bool IsUnlocked()
    {
        return unlockCondition?.IsUnlocked() ?? true;
    }

    /// <summary>
    /// Get prefab for specific variant (with fallback to base prefab)
    /// </summary>
    public GameObject GetPrefab(string variantID)
    {
        CharacterVariant variant = GetVariant(variantID);
        return variant?.customPrefab ?? basePrefab;
    }

    /// <summary>
    /// Get icon for specific variant (with fallback to base icon)
    /// </summary>
    public Sprite GetIcon(string variantID)
    {
        CharacterVariant variant = GetVariant(variantID);
        return variant?.customIcon ?? uiIcon;
    }

    /// <summary>
    /// Validate and format character ID
    /// </summary>
    private string ValidateCharacterID(string id)
    {
        if (string.IsNullOrEmpty(id)) return "unknown_character_default_01";

        // Convert to lowercase and replace spaces with underscores
        string validatedID = id.ToLower().Replace(" ", "_");

        // Ensure format: category_type_variant_version
        string[] parts = validatedID.Split('_');
        if (parts.Length < 4)
        {
            Debug.LogWarning($"Character ID should follow format: category_type_variant_version. Current: {id}");
            
            // Pad with defaults if needed
            while (parts.Length < 4)
            {
                if (parts.Length == 1) parts = new string[] { parts[0], "unknown", "default", "01" };
                else if (parts.Length == 2) parts = new string[] { parts[0], parts[1], "default", "01" };
                else if (parts.Length == 3) parts = new string[] { parts[0], parts[1], parts[2], "01" };
            }
        }

        return string.Join("_", parts);
    }

    /// <summary>
    /// Create a new character definition from existing RagdollCharacter (for migration)
    /// </summary>
    public static CharacterDefinition CreateFromRagdollCharacter(RagdollCharacter ragdoll, string categoryID = "warrior")
    {
        CharacterDefinition definition = CreateInstance<CharacterDefinition>();
        
        definition.characterID = GenerateCharacterID(ragdoll.name, categoryID);
        definition.displayName = ragdoll.name;
        definition.categoryID = categoryID;
        definition.basePrefab = ragdoll.gameObject;

        // Transfer stats
        definition.baseStats = new CharacterStats
        {
            maxHealth = ragdoll.maxHealth,
            moveSpeed = ragdoll.moveSpeed,
            attackDamage = ragdoll.attackDamage,
            attackRange = ragdoll.attackRange,
            attackCooldown = ragdoll.attackCooldown
        };

        // Create default variant
        CharacterVariant defaultVariant = new CharacterVariant
        {
            variantID = "default",
            variantName = "Default",
            description = "Default variant",
            isDefault = true
        };
        definition.variants.Add(defaultVariant);

        return definition;
    }

    /// <summary>
    /// Generate character ID from name and category
    /// </summary>
    public static string GenerateCharacterID(string name, string category)
    {
        string cleanName = name.ToLower().Replace(" ", "_").Replace("(", "").Replace(")", "");
        return $"{category}_{cleanName}_default_01";
    }
}

// ✅ DISABLED: TeamMaterialSet - loại bỏ hoàn toàn chức năng màu team
/*
[System.Serializable]
public class TeamMaterialSet
{
    [Header("Team Info")]
    public int teamID;
    public string teamName;

    [Header("Materials")]
    public Material[] materials;

    [Header("Colors")]
    public Color primaryColor = Color.blue;
    public Color secondaryColor = Color.white;

    public TeamMaterialSet()
    {
        materials = new Material[0];
    }

    public TeamMaterialSet(int id, string name, Material[] mats)
    {
        teamID = id;
        teamName = name;
        materials = mats ?? new Material[0];
    }
}
*/