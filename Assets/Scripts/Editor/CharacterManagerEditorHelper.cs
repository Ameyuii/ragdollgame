using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper class to make CharacterManager work in Editor mode
/// </summary>
[InitializeOnLoad]
public class CharacterManagerEditorHelper
{
    static CharacterManagerEditorHelper()
    {
        // Subscribe to selection change to auto-refresh
        Selection.selectionChanged += OnSelectionChanged;
        
        // Subscribe to hierarchy change
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }
    
    static void OnSelectionChanged()
    {
        // Auto-refresh Character Manager when selected
        if (Selection.activeGameObject != null)
        {
            CharacterManager manager = Selection.activeGameObject.GetComponent<CharacterManager>();
            if (manager != null)
            {
                // Force refresh UI in editor
                EditorApplication.delayCall += () => {
                    if (manager != null)
                    {
                        manager.RefreshUI();
                    }
                };
            }
        }
    }
    
    static void OnHierarchyChanged()
    {
        // Find all Character Managers and refresh them
        CharacterManager[] managers = Object.FindObjectsOfType<CharacterManager>();
        foreach (var manager in managers)
        {
            if (manager != null)
            {
                EditorApplication.delayCall += () => {
                    if (manager != null)
                    {
                        manager.RefreshUI();
                    }
                };
            }
        }
    }
    
    [MenuItem("Tools/Character Manager/Force Refresh All")]
    public static void ForceRefreshAll()
    {
        CharacterManager[] managers = Object.FindObjectsOfType<CharacterManager>();
        foreach (var manager in managers)
        {
            manager.RefreshUI();
            EditorUtility.SetDirty(manager);
        }
        
        Debug.Log($"Force refreshed {managers.Length} Character Managers");
    }
    
    [MenuItem("Tools/Character Manager/Initialize All")]
    public static void InitializeAll()
    {
        CharacterManager[] managers = Object.FindObjectsOfType<CharacterManager>();
        foreach (var manager in managers)
        {
            if (manager.CategoryCount == 0)
            {
                manager.InitializeDefaultCategories();
                EditorUtility.SetDirty(manager);
            }
        }
        
        Debug.Log($"Initialized {managers.Length} Character Managers");
    }
    
    [MenuItem("Tools/Character Manager/Validate All Data")]
    public static void ValidateAllData()
    {
        CharacterManager[] managers = Object.FindObjectsOfType<CharacterManager>();
        foreach (var manager in managers)
        {
            manager.ValidateCharacterData();
        }
        
        Debug.Log($"Validated {managers.Length} Character Managers");
    }
}