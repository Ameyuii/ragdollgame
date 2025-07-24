using UnityEngine;

public class IncreaseAllHealth : MonoBehaviour
{
    public static void Execute()
    {
        int updatedCount = 0;
        
        // Find all StableCharacter components
        StableCharacter[] stableCharacters = FindObjectsOfType<StableCharacter>();
        foreach (StableCharacter character in stableCharacters)
        {
            character.maxHealth = 1000f;
            // Also update current health if it's less than 1000
            var healthField = typeof(StableCharacter).GetField("health", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthField != null)
            {
                float currentHealth = (float)healthField.GetValue(character);
                if (currentHealth < 1000f)
                {
                    healthField.SetValue(character, 1000f);
                }
            }
            updatedCount++;
            Debug.Log($"Updated {character.name} maxHealth to 1000");
        }
        
        // Find all RagdollCharacter components
        RagdollCharacter[] ragdollCharacters = FindObjectsOfType<RagdollCharacter>();
        foreach (RagdollCharacter character in ragdollCharacters)
        {
            character.maxHealth = 1000f;
            // Also update current health if it's less than 1000
            var healthField = typeof(RagdollCharacter).GetField("health", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthField != null)
            {
                float currentHealth = (float)healthField.GetValue(character);
                if (currentHealth < 1000f)
                {
                    healthField.SetValue(character, 1000f);
                }
            }
            updatedCount++;
            Debug.Log($"Updated {character.name} maxHealth to 1000");
        }
        
        Debug.Log($"✓ Successfully updated health for {updatedCount} characters to 1000!");
    }
}