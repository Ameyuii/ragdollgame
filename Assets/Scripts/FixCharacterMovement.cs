using UnityEngine;
using UnityEngine.AI;

public class FixCharacterMovement : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Fixing character movement...");
        
        // Find all characters
        RagdollCharacter[] characters = FindObjectsOfType<RagdollCharacter>();
        
        foreach (RagdollCharacter character in characters)
        {
            if (character == null) continue;
            
            // Get NavMeshAgent
            NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = character.gameObject.AddComponent<NavMeshAgent>();
            }
            
            // Configure NavMeshAgent properly
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.stoppingDistance = 1.8f; // Stop just outside attack range
            agent.radius = 0.5f;
            agent.height = 2f;
            agent.baseOffset = 0f;
            agent.enabled = true;
            
            // Make sure Rigidbody is kinematic when using NavMeshAgent
            Rigidbody rb = character.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            
            Debug.Log($"Fixed movement for {character.name}");
        }
        
        Debug.Log("Character movement fix completed!");
    }
}