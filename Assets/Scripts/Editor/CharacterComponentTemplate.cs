using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Template system for character components
/// </summary>
[System.Serializable]
public class ComponentTemplate
{
    public string templateName;
    public string[] requiredComponents;
    public string description;
    
    public ComponentTemplate(string name, string[] components, string desc)
    {
        templateName = name;
        requiredComponents = components;
        description = desc;
    }
}

public class CharacterComponentTemplate : EditorWindow
{
    private static List<ComponentTemplate> templates = new List<ComponentTemplate>
    {
        new ComponentTemplate("Basic Character", 
            new string[] { "RagdollCharacter", "Rigidbody", "CapsuleCollider" },
            "Basic character with physics and ragdoll"),
            
        new ComponentTemplate("AI Character", 
            new string[] { "RagdollCharacter", "UnityEngine.AI.NavMeshAgent", "Rigidbody", "CapsuleCollider", "Animator" },
            "AI-controlled character with navigation"),
            
        new ComponentTemplate("Player Character", 
            new string[] { "RagdollCharacter", "Rigidbody", "CapsuleCollider", "Animator", "AudioSource" },
            "Player-controlled character with audio"),
            
        new ComponentTemplate("Combat Character", 
            new string[] { "RagdollCharacter", "UnityEngine.AI.NavMeshAgent", "Rigidbody", "CapsuleCollider", "Animator", "AudioSource" },
            "Full combat character with all systems"),
            
        new ComponentTemplate("Vehicle Character", 
            new string[] { "RagdollCharacter", "Rigidbody", "BoxCollider", "AudioSource" },
            "Vehicle-based character"),
    };
    
    private GameObject targetObject;
    private int selectedTemplate = 0;
    private Vector2 scrollPosition;
    private List<string> setupLog = new List<string>();
    
    [MenuItem("Tools/Character Component Template")]
    public static void ShowWindow()
    {
        CharacterComponentTemplate window = GetWindow<CharacterComponentTemplate>();
        window.titleContent = new GUIContent("Component Template");
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Character Component Template", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Target selection
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
        
        if (targetObject == null && Selection.activeGameObject != null)
        {
            if (GUILayout.Button("Use Selected GameObject"))
            {
                targetObject = Selection.activeGameObject;
            }
        }
        
        GUILayout.Space(10);
        
        // Template selection
        GUILayout.Label("Select Template:", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));
        
        for (int i = 0; i < templates.Count; i++)
        {
            var template = templates[i];
            
            GUILayout.BeginVertical("box");
            
            // Template selection
            bool isSelected = selectedTemplate == i;
            if (GUILayout.Toggle(isSelected, template.templateName, EditorStyles.radioButton) && !isSelected)
            {
                selectedTemplate = i;
            }
            
            if (isSelected)
            {
                EditorGUI.indentLevel++;
                GUILayout.Label(template.description, EditorStyles.wordWrappedMiniLabel);
                
                GUILayout.Label("Components:", EditorStyles.miniBoldLabel);
                foreach (string component in template.requiredComponents)
                {
                    GUILayout.Label($"• {component}", EditorStyles.miniLabel);
                }
                EditorGUI.indentLevel--;
            }
            
            GUILayout.EndVertical();
        }
        
        EditorGUILayout.EndScrollView();
        
        GUILayout.Space(10);
        
        // Apply button
        GUI.enabled = targetObject != null;
        if (GUILayout.Button("Apply Template", GUILayout.Height(30)))
        {
            ApplyTemplate();
        }
        GUI.enabled = true;
        
        GUILayout.Space(10);
        
        // Batch apply
        if (Selection.gameObjects.Length > 1)
        {
            GUILayout.Label($"Apply to {Selection.gameObjects.Length} selected objects:", EditorStyles.boldLabel);
            if (GUILayout.Button("Apply Template to All Selected"))
            {
                ApplyTemplateToSelected();
            }
        }
        
        GUILayout.Space(10);
        
        // Setup log
        if (setupLog.Count > 0)
        {
            GUILayout.Label("Setup Log:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box", GUILayout.MaxHeight(100));
            Vector2 logScrollPosition = EditorGUILayout.BeginScrollView(Vector2.zero);
            
            foreach (string log in setupLog)
            {
                EditorGUILayout.LabelField(log, EditorStyles.wordWrappedMiniLabel);
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("Clear Log"))
            {
                setupLog.Clear();
            }
        }
    }
    
    void ApplyTemplate()
    {
        if (targetObject == null || selectedTemplate >= templates.Count) return;
        
        setupLog.Clear();
        var template = templates[selectedTemplate];
        
        setupLog.Add($"Applying template '{template.templateName}' to {targetObject.name}...");
        
        bool isModified = false;
        
        foreach (string componentName in template.requiredComponents)
        {
            if (AddComponentByName(targetObject, componentName))
            {
                isModified = true;
            }
        }
        
        if (isModified)
        {
            EditorUtility.SetDirty(targetObject);
            setupLog.Add("✅ Template applied successfully!");
        }
        else
        {
            setupLog.Add("ℹ️ All components already exist");
        }
    }
    
    void ApplyTemplateToSelected()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length == 0) return;
        
        setupLog.Clear();
        var template = templates[selectedTemplate];
        
        setupLog.Add($"Applying template '{template.templateName}' to {selected.Length} objects...");
        
        int modifiedCount = 0;
        
        foreach (GameObject obj in selected)
        {
            bool isModified = false;
            
            foreach (string componentName in template.requiredComponents)
            {
                if (AddComponentByName(obj, componentName))
                {
                    isModified = true;
                }
            }
            
            if (isModified)
            {
                EditorUtility.SetDirty(obj);
                modifiedCount++;
            }
        }
        
        setupLog.Add($"✅ Template applied to {modifiedCount} objects!");
    }
    
    bool AddComponentByName(GameObject target, string componentName)
    {
        // Handle Unity built-in components
        System.Type componentType = null;
        
        // Try to get type directly
        componentType = System.Type.GetType(componentName);
        
        // If not found, try with UnityEngine namespace
        if (componentType == null)
        {
            componentType = System.Type.GetType($"UnityEngine.{componentName}");
        }
        
        // Try with full namespace for AI components
        if (componentType == null && componentName.Contains("NavMeshAgent"))
        {
            componentType = typeof(UnityEngine.AI.NavMeshAgent);
        }
        
        if (componentType != null)
        {
            // Check if component already exists
            if (target.GetComponent(componentType) == null)
            {
                target.AddComponent(componentType);
                setupLog.Add($"+ Added {componentName}");
                return true;
            }
            else
            {
                setupLog.Add($"- {componentName} already exists");
                return false;
            }
        }
        else
        {
            setupLog.Add($"⚠️ Component type '{componentName}' not found");
            return false;
        }
    }
    
    [MenuItem("GameObject/Character Management/Apply Component Template", false, 12)]
    public static void ApplyTemplateToSelection()
    {
        if (Selection.activeGameObject != null)
        {
            ShowWindow();
        }
        else
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject to apply template to.", "OK");
        }
    }
    
    [MenuItem("GameObject/Character Management/Apply Component Template", true)]
    public static bool ValidateApplyTemplateToSelection()
    {
        return Selection.activeGameObject != null;
    }
}