using UnityEngine;
using UnityEngine.AI;

public class SetupNavMesh : MonoBehaviour
{
    public static void Execute()
    {
        // Find Ground object
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            // Set Navigation Static flag
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(ground, UnityEditor.StaticEditorFlags.NavigationStatic);
            Debug.Log("Set Ground as Navigation Static");
        }
        
        // Bake NavMesh
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        Debug.Log("NavMesh baked successfully");
    }
}