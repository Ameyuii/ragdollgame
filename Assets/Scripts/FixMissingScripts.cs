using UnityEngine;
using UnityEditor;

public class FixMissingScripts : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("🔧 Starting to fix missing script references...");
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Get all components
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log($"🗑️ Removing missing script from: {obj.name}");
                    
#if UNITY_EDITOR
                    // Remove missing component in editor
                    SerializedObject serializedObject = new SerializedObject(obj);
                    SerializedProperty prop = serializedObject.FindProperty("m_Component");
                    
                    for (int j = prop.arraySize - 1; j >= 0; j--)
                    {
                        SerializedProperty component = prop.GetArrayElementAtIndex(j);
                        if (component.FindPropertyRelative("component").objectReferenceValue == null)
                        {
                            prop.DeleteArrayElementAtIndex(j);
                            fixedCount++;
                        }
                    }
                    
                    serializedObject.ApplyModifiedProperties();
#endif
                }
            }
        }
        
        Debug.Log($"✅ Fixed {fixedCount} missing script references!");
        
#if UNITY_EDITOR
        // Mark scene dirty so changes are saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }
}