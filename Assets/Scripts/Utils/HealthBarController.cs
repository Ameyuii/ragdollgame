using UnityEngine;

/// <summary>
/// Health Bar Controller - Simple health bar display for characters
/// Lightweight replacement for legacy health bar system
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [Header("🩺 Health Bar Settings")]
    [Tooltip("Health bar UI element")]
    public UnityEngine.UI.Slider healthBar;
    
    [Tooltip("Character to track health for")]
    public RagdollCharacter character;
    
    [Tooltip("Offset above character")]
    public Vector3 offset = new Vector3(0, 2, 0);
    
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        
        if (character == null)
            character = GetComponentInParent<RagdollCharacter>();
            
        if (healthBar == null)
            healthBar = GetComponentInChildren<UnityEngine.UI.Slider>();
    }
    
    private void Update()
    {
        if (character == null || healthBar == null)
            return;
            
        // Update health bar value
        float healthPercent = character.GetCurrentHealth() / character.maxHealth;
        healthBar.value = healthPercent;
        
        // Position health bar above character
        if (mainCamera != null)
        {
            Vector3 worldPosition = character.transform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            transform.position = screenPosition;
        }
        
        // Hide if character is dead
        if (character.IsDead())
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Setup health bar for character
    /// </summary>
    public void SetupHealthBar(RagdollCharacter targetCharacter)
    {
        character = targetCharacter;

        if (healthBar != null)
        {
            healthBar.maxValue = 1f;
            healthBar.value = 1f;
        }
    }

    /// <summary>
    /// Refresh health bar display (for backward compatibility)
    /// </summary>
    public void RefreshHealthBar()
    {
        if (character == null || healthBar == null)
            return;

        // Update health bar value
        float healthPercent = character.GetCurrentHealth() / character.maxHealth;
        healthBar.value = healthPercent;

        // Update visibility based on character state
        if (character.IsDead())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
