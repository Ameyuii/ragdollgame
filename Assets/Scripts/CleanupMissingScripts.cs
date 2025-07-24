using UnityEngine;
using UnityEditor;

public class CleanupMissingScripts : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Starting cleanup of missing scripts...");
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int cleanedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Get all components
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                // Check if component is null (missing script)
                if (components[i] == null)
                {
                    Debug.Log($"Removing missing script from {obj.name}");
                    
                    // Remove the missing component
                    #if UNITY_EDITOR
                    SerializedObject serializedObject = new SerializedObject(obj);
                    SerializedProperty prop = serializedObject.FindProperty("m_Component");
                    
                    for (int j = 0; j < prop.arraySize; j++)
                    {
                        SerializedProperty componentProp = prop.GetArrayElementAtIndex(j);
                        if (componentProp.objectReferenceValue == null)
                        {
                            prop.DeleteArrayElementAtIndex(j);
                            cleanedCount++;
                            break;
                        }
                    }
                    
                    serializedObject.ApplyModifiedProperties();
                    #endif
                }
            }
        }
        
        Debug.Log($"Cleanup completed! Removed {cleanedCount} missing script references.");
        
        #if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        #endif
    }
}