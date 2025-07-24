using UnityEngine;

public class ForceStabilizeCharacters : MonoBehaviour
{
    [ContextMenu("Force Stabilize All Characters")]
    public static void Execute()
    {
        Debug.Log("=== FORCE STABILIZING ALL CHARACTERS ===");
        
        // Find all objects that might be characters
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int fixedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Check if this looks like a character
            if (obj.name.Contains("Character") || obj.name.Contains("npc") || 
                obj.name.Contains("Team") || obj.name.Contains("Warrok"))
            {
                if (StabilizeCharacter(obj))
                {
                    fixedCount++;
                }
            }
        }
        
        Debug.Log($"=== STABILIZED {fixedCount} CHARACTERS ===");
    }
    
    static bool StabilizeCharacter(GameObject character)
    {
        Debug.Log($"Stabilizing {character.name}...");
        
        try
        {
            // 1. Reset position to ground
            Vector3 pos = character.transform.position;
            pos.y = 0.1f;
            character.transform.position = pos;
            character.transform.rotation = Quaternion.identity;
            
            // 2. Handle all rigidbodies
            Rigidbody[] allRigidbodies = character.GetComponentsInChildren<Rigidbody>();
            Rigidbody mainRigidbody = character.GetComponent<Rigidbody>();
            
            foreach (Rigidbody rb in allRigidbodies)
            {
                if (rb != null)
                {
                    // Stop all movement (only if not kinematic)
                    if (!rb.isKinematic)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                    
                    if (rb == mainRigidbody)
                    {
                        // Main rigidbody: kinematic, no gravity
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                    else
                    {
                        // Ragdoll rigidbodies: kinematic initially, good damping
                        rb.isKinematic = true;
                        rb.useGravity = false;
                        rb.mass = 1f;
                        rb.linearDamping = 5f;
                        rb.angularDamping = 10f;
                        
                        // Set velocity limits
                        rb.maxLinearVelocity = 5f;
                        rb.maxAngularVelocity = 5f;
                    }
                }
            }
            
            // 3. Handle colliders
            Collider[] allColliders = character.GetComponentsInChildren<Collider>();
            Collider mainCollider = character.GetComponent<Collider>();
            
            foreach (Collider col in allColliders)
            {
                if (col != null && col != mainCollider)
                {
                    // Disable ragdoll colliders
                    col.enabled = false;
                }
            }
            
            // 4. Reset character script if exists
            RagdollCharacter ragdollChar = character.GetComponent<RagdollCharacter>();
            if (ragdollChar != null)
            {
                // Use the new force reset method
                ragdollChar.ForceStableReset();
            }
            
            Debug.Log($"✓ Stabilized {character.name}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Failed to stabilize {character.name}: {e.Message}");
            return false;
        }
    }
    
    [ContextMenu("Emergency Stop All Physics")]
    public static void EmergencyStopAllPhysics()
    {
        Debug.Log("=== EMERGENCY PHYSICS STOP ===");
        
        // Find ALL rigidbodies in scene
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb != null)
            {
                // Stop all movement immediately (reset velocity before setting kinematic)
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
                
                // Reset position if flying
                if (rb.transform.position.y > 5f)
                {
                    Vector3 pos = rb.transform.position;
                    pos.y = 0.1f;
                    rb.transform.position = pos;
                }
            }
        }
        
        Debug.Log($"Stopped {allRigidbodies.Length} rigidbodies");
    }
    
    void Update()
    {
        // Auto-fix flying objects
        if (Input.GetKeyDown(KeyCode.F))
        {
            Execute();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            EmergencyStopAllPhysics();
        }
    }
}
