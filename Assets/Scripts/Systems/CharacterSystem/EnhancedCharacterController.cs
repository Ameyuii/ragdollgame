using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnhancedCharacterController : MonoBehaviour, ICharacter
{
    [Header("Character Definition")]
    public CharacterDefinition characterDefinition;
    public string selectedVariantID;
    public int assignedTeamID;

    [Header("Runtime Data")]
    public CharacterRuntimeData runtimeData;

    [Header("Health Bar")]
    public Canvas healthBarCanvas;
    public Slider healthSlider;

    // Component references
    private CharacterVisualManager visualManager;
    private CharacterStatsManager statsManager;
    private CharacterTeamManager teamManager;

    // Legacy compatibility
    private RagdollCharacter legacyController;
    private bool useLegacySystem = false;

    // ICharacter implementation
    public string CharacterID => characterDefinition?.CharacterID ?? "unknown";
    public string VariantID => selectedVariantID ?? "default";
    public int TeamID => assignedTeamID;
    public CharacterStats CurrentStats => statsManager?.GetFinalStats() ?? new CharacterStats();
    public bool IsAlive => runtimeData?.currentHealth > 0;

    // Legacy ICharacter interface implementation
    public int GetTeamId() => assignedTeamID;
    public bool IsDead() => !IsAlive;
    public void ResetCharacter()
    {
        if (runtimeData != null)
        {
            runtimeData.currentHealth = runtimeData.maxHealth;
            runtimeData.isDead = false;
            UpdateHealthBar();
        }
    }

    void Awake()
    {
        InitializeRuntimeData();
        InitializeComponents();
        CheckLegacyCompatibility();
    }

    void Start()
    {
        if (useLegacySystem)
        {
            // Use legacy system
            if (legacyController != null)
            {
                legacyController.enabled = true;
                this.enabled = false;
                return;
            }
        }

        // Use new system
        if (characterDefinition != null)
        {
            ApplyCharacterData();
            ApplyTeamConfiguration();
        }
    }

    /// <summary>
    /// Initialize character with definition, variant, and team
    /// </summary>
    public void Initialize(CharacterDefinition definition, string variantID, int teamID)
    {
        characterDefinition = definition;
        selectedVariantID = variantID;
        assignedTeamID = teamID;

        InitializeComponents();
        ApplyCharacterData();
        ApplyTeamConfiguration();
    }

    /// <summary>
    /// Initialize runtime data
    /// </summary>
    private void InitializeRuntimeData()
    {
        if (runtimeData == null)
        {
            runtimeData = new CharacterRuntimeData();
        }
    }

    /// <summary>
    /// Initialize component managers
    /// </summary>
    private void InitializeComponents()
    {
        // Get or add visual manager
        visualManager = GetComponent<CharacterVisualManager>();
        if (visualManager == null)
        {
            visualManager = gameObject.AddComponent<CharacterVisualManager>();
        }

        // Get or add stats manager
        statsManager = GetComponent<CharacterStatsManager>();
        if (statsManager == null)
        {
            statsManager = gameObject.AddComponent<CharacterStatsManager>();
        }

        // Get or add team manager
        teamManager = GetComponent<CharacterTeamManager>();
        if (teamManager == null)
        {
            teamManager = gameObject.AddComponent<CharacterTeamManager>();
        }
    }

    /// <summary>
    /// Check if we should use legacy system for backward compatibility
    /// </summary>
    private void CheckLegacyCompatibility()
    {
        // Check if GameDatabase wants to use new system
        GameDatabase gameDB = GameDatabase.Instance;
        if (gameDB != null && gameDB.IsNewSystemEnabled())
        {
            useLegacySystem = false;
            return;
        }

        // Check if we have legacy controller
        legacyController = GetComponent<RagdollCharacter>();
        if (legacyController != null)
        {
            useLegacySystem = true;
            Debug.Log($"Using legacy system for {name}");
        }
    }

    /// <summary>
    /// Apply character definition data to this character
    /// </summary>
    private void ApplyCharacterData()
    {
        if (characterDefinition == null) return;

        // Apply stats
        CharacterStats finalStats = characterDefinition.GetFinalStats(selectedVariantID);
        statsManager.ApplyFinalStats(finalStats);

        // Initialize runtime health
        runtimeData.currentHealth = finalStats.maxHealth;
        runtimeData.maxHealth = finalStats.maxHealth;

        // Apply visual data
        visualManager.ApplyCharacterDefinition(characterDefinition, selectedVariantID);

        // Update name
        if (string.IsNullOrEmpty(gameObject.name) || gameObject.name == "GameObject")
        {
            gameObject.name = characterDefinition.DisplayName;
        }
    }

    /// <summary>
    /// Apply team configuration
    /// </summary>
    private void ApplyTeamConfiguration()
    {
        TeamConfiguration team = GameDatabase.Instance?.GetTeam(assignedTeamID);
        if (team != null)
        {
            teamManager.SetTeam(team);
            visualManager.ApplyTeamConfiguration(team);
        }
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        // Apply armor reduction
        float actualDamage = Mathf.Max(0, damage - CurrentStats.armor);
        runtimeData.currentHealth -= actualDamage;

        // Update health bar
        UpdateHealthBar();

        // Check if dead
        if (runtimeData.currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal character
    /// </summary>
    public void Heal(float amount)
    {
        if (!IsAlive) return;

        runtimeData.currentHealth = Mathf.Min(runtimeData.maxHealth, runtimeData.currentHealth + amount);
        UpdateHealthBar();
    }

    /// <summary>
    /// Set team
    /// </summary>
    public void SetTeam(int teamID)
    {
        assignedTeamID = teamID;
        ApplyTeamConfiguration();
    }

    /// <summary>
    /// Apply status effect
    /// </summary>
    public void ApplyStatusEffect(StatusEffect effect)
    {
        // TODO: Implement status effects
        Debug.Log($"Applied status effect {effect} to {name}");
    }

    /// <summary>
    /// Character death
    /// </summary>
    private void Die()
    {
        runtimeData.isDead = true;
        
        // Trigger death animations/effects
        visualManager?.TriggerDeathEffect();
        
        // Notify game systems
        CharacterEvents.TriggerCharacterDied(this);
    }

    /// <summary>
    /// Update health bar
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthSlider != null && runtimeData != null)
        {
            healthSlider.value = runtimeData.currentHealth / runtimeData.maxHealth;
        }
    }

    /// <summary>
    /// Get character info for compatibility with existing systems
    /// </summary>
    public CharacterInfo? GetLegacyCharacterInfo()
    {
        if (characterDefinition == null) return null;

        CharacterStats stats = CurrentStats;
        return new CharacterInfo
        {
            characterName = characterDefinition.DisplayName,
            prefab = characterDefinition.BasePrefab,
            uiIcon = characterDefinition.UIIcon,
            health = (int)stats.maxHealth,
            speed = stats.moveSpeed,
            attackDamage = stats.attackDamage,
            attackRange = stats.attackRange,
            description = characterDefinition.Description,
            teamColor = Color.white,
            characterType = CharacterType.Soldier
        };
    }
}

/// <summary>
/// Runtime data for character state
/// </summary>
[System.Serializable]
public class CharacterRuntimeData
{
    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    public bool isDead = false;

    [Header("Status")]
    public bool isStunned = false;
    public float stunDuration = 0f;

    [Header("Combat")]
    public float lastAttackTime = 0f;
    public int killCount = 0;
    public float damageDealt = 0f;
    public float damageTaken = 0f;

    public CharacterRuntimeData()
    {
        currentHealth = 100f;
        maxHealth = 100f;
        isDead = false;
    }
}

/// <summary>
/// Status effects for characters
/// </summary>
public enum StatusEffect
{
    None,
    Stun,
    Poison,
    Heal,
    SpeedBoost,
    DamageBoost,
    Shield
}