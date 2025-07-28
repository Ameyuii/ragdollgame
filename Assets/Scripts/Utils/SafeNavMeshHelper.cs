using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Safe NavMesh Helper - Provides safe NavMeshAgent operations
/// Prevents crashes when NavMeshAgent is not properly initialized
/// </summary>
public static class SafeNavMeshHelper
{
    /// <summary>
    /// Safely set NavMeshAgent destination
    /// </summary>
    public static bool SafeSetDestination(NavMeshAgent agent, Vector3 destination)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return false;
            
        try
        {
            agent.SetDestination(destination);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SafeNavMeshHelper: Failed to set destination - {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Safely stop NavMeshAgent
    /// </summary>
    public static bool SafeSetStopped(NavMeshAgent agent, bool stopped)
    {
        if (agent == null || !agent.enabled)
            return false;
            
        try
        {
            agent.isStopped = stopped;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SafeNavMeshHelper: Failed to set stopped state - {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Check if NavMeshAgent is ready for operations
    /// </summary>
    public static bool IsAgentReady(NavMeshAgent agent)
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }

    /// <summary>
    /// Check if NavMeshAgent is valid (alias for IsAgentReady for backward compatibility)
    /// </summary>
    public static bool IsAgentValid(NavMeshAgent agent)
    {
        return IsAgentReady(agent);
    }
}
