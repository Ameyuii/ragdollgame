using UnityEngine;

public class ReplaceAI : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Replacing SimpleCharacterAI with ImprovedCharacterAI...");
        
        // Find all characters with SimpleCharacterAI
        SimpleCharacterAI[] oldAIs = FindObjectsOfType<SimpleCharacterAI>();
        
        foreach (SimpleCharacterAI oldAI in oldAIs)
        {
            if (oldAI == null) continue;
            
            GameObject character = oldAI.gameObject;
            
            // Remove old AI
            DestroyImmediate(oldAI);
            
            // Add new AI
            ImprovedCharacterAI newAI = character.AddComponent<ImprovedCharacterAI>();
            
            Debug.Log($"Replaced AI on {character.name}");
        }
        
        Debug.Log("AI replacement completed!");
    }
}