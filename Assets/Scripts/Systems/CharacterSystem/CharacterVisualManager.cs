using UnityEngine;

public class CharacterVisualManager : MonoBehaviour
{
    [Header("References")]
    private Renderer[] renderers;
    private Animator animator;

    [Header("Current State")]
    public CharacterDefinition currentDefinition;
    public string currentVariantID;
    public TeamConfiguration currentTeam;

    void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Apply character definition visuals
    /// </summary>
    public void ApplyCharacterDefinition(CharacterDefinition definition, string variantID)
    {
        currentDefinition = definition;
        currentVariantID = variantID;

        if (definition == null) return;

        // Apply animator controller
        if (animator != null && definition.AnimatorController != null)
        {
            animator.runtimeAnimatorController = definition.AnimatorController;
        }

        // Apply variant-specific visuals
        CharacterVariant variant = definition.GetVariant(variantID);
        if (variant != null)
        {
            ApplyVariantVisuals(variant);
        }
    }

    /// <summary>
    /// Apply variant-specific visual changes
    /// </summary>
    public void ApplyVariantVisuals(CharacterVariant variant)
    {
        if (variant == null) return;

        // Apply custom materials if available
        if (variant.customMaterials != null && variant.customMaterials.Length > 0)
        {
            ApplyMaterials(variant.customMaterials);
        }

        // Apply custom animator if available
        if (variant.customAnimator != null && animator != null)
        {
            animator.runtimeAnimatorController = variant.customAnimator;
        }
    }

    /// <summary>
    /// Apply team configuration visuals
    /// </summary>
    public void ApplyTeamConfiguration(TeamConfiguration team)
    {
        currentTeam = team;
        if (team == null) return;

        // Apply team materials
        if (currentDefinition != null)
        {
            Material[] teamMaterials = currentDefinition.GetTeamMaterials(team.teamID);
            if (teamMaterials.Length > 0)
            {
                ApplyMaterials(teamMaterials);
            }
            else
            {
                // Use team base material if no specific materials
                ApplyTeamBaseMaterial(team);
            }
        }
    }

    /// <summary>
    /// Apply materials to renderers
    /// </summary>
    private void ApplyMaterials(Material[] materials)
    {
        if (materials == null || materials.Length == 0) return;

        for (int i = 0; i < renderers.Length && i < materials.Length; i++)
        {
            if (renderers[i] != null && materials[i] != null)
            {
                renderers[i].material = materials[i];
            }
        }
    }

    /// <summary>
    /// Apply team base material with color tinting
    /// </summary>
    private void ApplyTeamBaseMaterial(TeamConfiguration team)
    {
        if (team.baseMaterial == null) return;

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material = team.baseMaterial;
                
                // Apply team color tinting
                if (renderer.material.HasProperty("_Color"))
                {
                    renderer.material.color = team.primaryColor;
                }
            }
        }
    }

    /// <summary>
    /// Update health bar visuals
    /// </summary>
    public void UpdateHealthBarVisuals()
    {
        // Health bar updates handled by main controller
    }

    /// <summary>
    /// Trigger death effect
    /// </summary>
    public void TriggerDeathEffect()
    {
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Spawn death effect
        if (currentDefinition?.DeathEffect != null)
        {
            GameObject effect = Instantiate(currentDefinition.DeathEffect, transform.position, transform.rotation);
            Destroy(effect, 2f); // Clean up after 2 seconds
        }
    }

    /// <summary>
    /// Trigger hit effect
    /// </summary>
    public void TriggerHitEffect()
    {
        // Play hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Spawn hit effect
        if (currentDefinition?.HitEffect != null)
        {
            GameObject effect = Instantiate(currentDefinition.HitEffect, transform.position, transform.rotation);
            Destroy(effect, 1f); // Clean up after 1 second
        }
    }
}