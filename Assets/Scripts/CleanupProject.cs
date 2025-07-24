using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CleanupProject : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Starting complete project cleanup...");
        
        // Get current scene
        Scene currentScene = SceneManager.GetActiveScene();
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int deletedCount = 0;
        
        // List of essential objects to keep
        string[] keepObjects = { "Main Camera", "Directional Light", "Ground", "GameManager", "EventSystem", "UI Canvas" };
        
        foreach (GameObject obj in allObjects)
        {
            bool shouldKeep = false;
            
            // Check if this is an essential object
            foreach (string keepName in keepObjects)
            {
                if (obj.name.Contains(keepName))
                {
                    shouldKeep = true;
                    break;
                }
            }
            
            // Delete non-essential objects
            if (!shouldKeep)
            {
                Debug.Log($"Deleting GameObject: {obj.name}");
                DestroyImmediate(obj);
                deletedCount++;
            }
        }
        
        Debug.Log($"Cleanup completed! Deleted {deletedCount} GameObjects.");
        
        // Clean up any remaining missing script references
        CleanupMissingScriptReferences();
        
        #if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currentScene);
        #endif
        
        Debug.Log("Project is now clean and ready for new features!");
    }
    
    static void CleanupMissingScriptReferences()
    {
        GameObject[] remainingObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in remainingObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    Debug.Log($"Removing missing script from {obj.name}");
                    
                    #if UNITY_EDITOR
                    SerializedObject serializedObject = new SerializedObject(obj);
                    SerializedProperty prop = serializedObject.FindProperty("m_Component");
                    
                    for (int j = 0; j < prop.arraySize; j++)
                    {
                        SerializedProperty componentProp = prop.GetArrayElementAtIndex(j);
                        if (componentProp.objectReferenceValue == null)
                        {
                            prop.DeleteArrayElementAtIndex(j);
                            break;
                        }
                    }
                    
                    serializedObject.ApplyModifiedProperties();
                    #endif
                }
            }
        }
    }
}