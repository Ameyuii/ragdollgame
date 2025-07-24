using UnityEngine;
using System.Collections.Generic;

public class DebugCharacterDifferences : MonoBehaviour
{
    [ContextMenu("Analyze Character Differences")]
    public static void AnalyzeCharacterDifferences()
    {
        Debug.Log("=== ANALYZING CHARACTER DIFFERENCES ===");
        
        // Find all GameObjects with character-like names
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> characters = new List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Character") || obj.name.Contains("npc") || obj.name.Contains("Team"))
            {
                characters.Add(obj);
            }
        }
        
        Debug.Log($"Found {characters.Count} potential character objects:");
        
        foreach (GameObject character in characters)
        {
            AnalyzeCharacter(character);
        }
        
        Debug.Log("=== ANALYSIS COMPLETE ===");
    }
    
    static void AnalyzeCharacter(GameObject character)
    {
        Debug.Log($"\n--- ANALYZING: {character.name} ---");
        Debug.Log($"Position: {character.transform.position}");
        Debug.Log($"Active: {character.activeInHierarchy}");
        
        // Check components
        Component[] components = character.GetComponents<Component>();
        Debug.Log($"Components ({components.Length}):");
        foreach (Component comp in components)
        {
            if (comp != null)
            {
                Debug.Log($"  - {comp.GetType().Name}");
            }
        }
        
        // Check for RagdollCharacter script
        RagdollCharacter ragdollChar = character.GetComponent<RagdollCharacter>();
        if (ragdollChar != null)
        {
            Debug.Log($"  RagdollCharacter: teamId={ragdollChar.teamId}, isDead={ragdollChar.IsDead()}");
        }
        
        // Check for StableCharacter script
        StableCharacter stableChar = character.GetComponent<StableCharacter>();
        if (stableChar != null)
        {
            Debug.Log($"  StableCharacter: teamId={stableChar.teamId}");
        }
        
        // Check rigidbodies
        Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();
        Debug.Log($"Rigidbodies ({rigidbodies.Length}):");
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null)
            {
                Debug.Log($"  - {rb.name}: isKinematic={rb.isKinematic}, useGravity={rb.useGravity}, mass={rb.mass}");
                Debug.Log($"    velocity={rb.linearVelocity}, position={rb.transform.position}");
            }
        }
        
        // Check colliders
        Collider[] colliders = character.GetComponentsInChildren<Collider>();
        Debug.Log($"Colliders ({colliders.Length}):");
        foreach (Collider col in colliders)
        {
            if (col != null)
            {
                Debug.Log($"  - {col.name}: enabled={col.enabled}, isTrigger={col.isTrigger}");
            }
        }
    }
    
    [ContextMenu("Fix All Characters")]
    public static void FixAllCharacters()
    {
        Debug.Log("=== FIXING ALL CHARACTERS ===");
        
        // Find the stable character (Character_Team1_1) as reference
        GameObject stableCharacter = GameObject.Find("Character_Team1_1");
        if (stableCharacter == null)
        {
            Debug.LogError("Could not find Character_Team1_1 as reference!");
            return;
        }
        
        Debug.Log($"Using {stableCharacter.name} as reference configuration");
        
        // Get reference configuration
        RagdollCharacter refRagdoll = stableCharacter.GetComponent<RagdollCharacter>();
        StableCharacter refStable = stableCharacter.GetComponent<StableCharacter>();
        
        // Find all other characters
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> characters = new List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if ((obj.name.Contains("Character") || obj.name.Contains("npc") || obj.name.Contains("Team")) 
                && obj != stableCharacter)
            {
                characters.Add(obj);
            }
        }
        
        Debug.Log($"Found {characters.Count} characters to fix");
        
        foreach (GameObject character in characters)
        {
            FixCharacter(character, stableCharacter);
        }
        
        Debug.Log("=== FIX COMPLETE ===");
    }
    
    static void FixCharacter(GameObject character, GameObject referenceCharacter)
    {
        Debug.Log($"Fixing {character.name}...");
        
        // Reset position to ground level
        Vector3 pos = character.transform.position;
        pos.y = 0.1f;
        character.transform.position = pos;
        character.transform.rotation = Quaternion.identity;
        
        // Fix all rigidbodies
        Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null)
            {
                // Reset velocity (only if not kinematic)
                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                
                // Check if this is main rigidbody
                if (rb.transform == character.transform)
                {
                    // Main rigidbody should be kinematic
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                else
                {
                    // Ragdoll rigidbodies should be kinematic initially
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.mass = 1f;
                    rb.linearDamping = 5f;
                    rb.angularDamping = 10f;
                }
            }
        }
        
        // Disable ragdoll colliders
        Collider[] colliders = character.GetComponentsInChildren<Collider>();
        Collider mainCollider = character.GetComponent<Collider>();
        
        foreach (Collider col in colliders)
        {
            if (col != null && col != mainCollider)
            {
                col.enabled = false;
            }
        }
        
        // Ensure character script is working
        RagdollCharacter ragdollChar = character.GetComponent<RagdollCharacter>();
        if (ragdollChar != null)
        {
            ragdollChar.ResetCharacter();
        }
        
        Debug.Log($"Fixed {character.name}");
    }
}
